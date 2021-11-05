using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Dtos;
using System.Linq;
using Orbital.Pocos;
using Orbital.Services.AntivirusBackends;

namespace Orbital.Services.Antivirus
{
    public interface IScannerService
    {
        public Task<List<ScanResult>> LaunchScans(
            string[] payloadPathes,
            IList<ContainerListResponse> containers);
    }

    public class ScannerService : IScannerService
    {
        private class DispatchedScans
        {
            public string ContainerId { get; init; }
            public List<string> FilesToScanPathes { get; set; }
        }

        private SupportedAntivirus SupportedAntivirus { get; init; }
        private IAntivirusBackend AntivirusBackend { get; init; }
        private readonly DockerClient DockerClient;
        private readonly ILogger<ScannerService> Logger;

        public ScannerService(DockerClientConfiguration dockerClientConf,
            ILogger<ScannerService> logger,
            SupportedAntivirus supportedAntivirus,
            IAntivirusBackendFactory antivirusBackendFactory)
        {
            DockerClient = dockerClientConf.CreateClient();
            Logger = logger;
            SupportedAntivirus = supportedAntivirus;
            AntivirusBackend = antivirusBackendFactory.Create(supportedAntivirus);
        }

        public async Task<List<ScanResult>> LaunchScans(string[] payloadPathes,
            IList<ContainerListResponse> containers)
        {

            Logger.LogInformation($"cmd lines configured: {string.Join(' ', AntivirusBackend.GetFullCmd("[[PAYLOAD_NAME]]"))}");

            var dispatchedScansTasks = DispatchScanToContainers(payloadPathes.ToList(), containers);

            var scanBatchTasks = dispatchedScansTasks.Select(ScanBatch).ToList();

            await Task.WhenAll(scanBatchTasks.ToArray());

            return scanBatchTasks.SelectMany(st => st.Result).ToList();
        }

        private async Task<List<ScanResult>> ScanBatch(DispatchedScans dispatchedScans)
        {
            var scanResults = new List<ScanResult>();

            foreach (var fileToScanPath in dispatchedScans.FilesToScanPathes)
            {
                var fullCmd = AntivirusBackend.GetFullCmd(Path.GetFileName(fileToScanPath));
                var containerExecCreateParams = new ContainerExecCreateParameters
                {
                    AttachStderr = true,
                    AttachStdin = true,
                    AttachStdout = true,
                    Cmd = fullCmd
                };

                scanResults.Add(await Scan(fileToScanPath, dispatchedScans.ContainerId, containerExecCreateParams));
            }

            return scanResults;
        }


        private async Task<ScanResult> Scan(string payloadPath,
            string containerId,
            ContainerExecCreateParameters containerExecCreateParams)
        {
            var scanResult = new ScanResult()
            {
                FilePath = payloadPath,
                FlaggedState = FlaggedState.Unknown,
                IsError = true
            };
            try
            {
                await UploadPayload(payloadPath, containerId);

                Logger.LogInformation($"Launching scan for {payloadPath}");
                var created = await DockerClient.Exec
                    .ExecCreateContainerAsync(containerId, containerExecCreateParams);
                var multiplexedStream = await DockerClient.Exec
                    .StartAndAttachContainerExecAsync(created.ID, true);
                Stream outputStream = new MemoryStream();
                await multiplexedStream.CopyOutputToAsync(null, outputStream, outputStream, default);
                scanResult.FlaggedState = IsResultPositive(outputStream, containerId) ? FlaggedState.Positive : FlaggedState.Negative;
                scanResult.IsError = false;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while trying to scan payload :" + ex.Message);
            }

            return scanResult;
        }


        private async Task UploadPayload(string payloadPath, string containerId)
        {
            var tarStream = CreateTarGz($"{payloadPath}");
            await DockerClient.Containers.ExtractArchiveToContainerAsync(
                containerId,
                new ContainerPathStatParameters()
                {
                    Path = "/"
                },
                tarStream,
                default);
            await tarStream.DisposeAsync();
        }


        private static Stream CreateTarGz(string filePath)
        {
            var outStream = new MemoryStream();
            using (var gzoStream = new GZipOutputStream(outStream))
            using (var tar = new TarOutputStream(gzoStream, Encoding.Default))
            {
                // Prevent closing of the memory stream
                gzoStream.IsStreamOwner = false;

                var fileContents = File.ReadAllBytes(filePath);
                var tarEntry = TarEntry.CreateTarEntry(Path.GetFileName(filePath));

                tarEntry.Size = fileContents.Length;
                tarEntry.Name = Path.GetFileName(filePath);

                tar.PutNextEntry(tarEntry);
                tar.Write(fileContents, 0, fileContents.Length);
                tar.CloseEntry();
                tar.IsStreamOwner = false;
                tar.Close();
                tar.Flush();
            }
            outStream.Position = 0;
            return outStream;
        }



        private bool IsResultPositive(Stream outputStream, string payloadFileName)
        {
            outputStream.Position = 0;
            var outputResult = "";
            using (var reader = new StreamReader(outputStream))
            {
                outputResult = reader.ReadToEnd();
                Logger.LogInformation($"Output from {SupportedAntivirus} for {payloadFileName} : \n{outputResult}");
            }
            outputStream.Dispose();
            return AntivirusBackend.OutputParser.Match(outputResult).Success;
        }

        private IEnumerable<DispatchedScans> DispatchScanToContainers(List<string> payloadPathes, IList<ContainerListResponse> containers)
        {

            // containers
            //
            // var numberOfScansPerContainer = payloadPathes.Count() / containers.Count();

            var dispatchedScansWithContainerId= containers.Select(c => new DispatchedScans()
                {
                    ContainerId = c.ID, 
                    FilesToScanPathes = new List<string>()
                }).ToList();

            var i = 0;
            foreach (var payloadPath in payloadPathes)
            {
                var containerDoingTheScan = containers[i % containers.Count];
                Logger.LogInformation($"{payloadPath} dispatched to {containerDoingTheScan.ID}");
                dispatchedScansWithContainerId.Single(d => d.ContainerId == containerDoingTheScan.ID).FilesToScanPathes.Add(payloadPath);
                i++;
            }
            return dispatchedScansWithContainerId;
        }


    }
}

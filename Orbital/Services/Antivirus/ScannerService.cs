using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Orbital.Static;
using Shared.Config;
using Shared.Enums;
using Shared.Dtos;
using System.Linq;
using Rodin.Static;
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
        private class DispatchedScan
        {
            public string ContainerId;
            public string FileToScanPath;
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

        private string FormatPayloadArg(string payloadFileName)
        {
            if (payloadFileName.Contains('\\') || payloadFileName.Contains('/'))
            {
                throw new ArgumentException("PayloadFileName is a path");
            }
            return $"{AntivirusBackend.PayloadPathArg} {payloadFileName}";
        }

        public async Task<List<ScanResult>> LaunchScans(string[] payloadPathes,
            IList<ContainerListResponse> containers)
        {

            Logger.LogInformation($"cmd lines configured: {String.Join(' ', AntivirusBackend.GetFullCmd("[[PAYLOADNAME]]"))}");

            List<DispatchedScan> dispatchedScanTasks = DispatchScanToContainers(payloadPathes, containers);

            List<Task<ScanResult>> scanTasks = new List<Task<ScanResult>>();

            foreach (var dispatchedScanTask in dispatchedScanTasks)
            {
                var ContainerExecCreateParams = new ContainerExecCreateParameters
                {
                    AttachStderr = true,
                    AttachStdin = true,
                    AttachStdout = true,
                    Cmd = AntivirusBackend.GetFullCmd(Path.GetFileName(dispatchedScanTask.FileToScanPath))
                };
                scanTasks.Add(Scan(dispatchedScanTask, ContainerExecCreateParams));

            }

            await Task.WhenAll(scanTasks.ToArray());

            return scanTasks.Select(st => st.Result).ToList();
        }


        private async Task<ScanResult> Scan(DispatchedScan dispatchedScanTask,
            ContainerExecCreateParameters ContainerExecCreateParams)
        {
            await UploadPayload(dispatchedScanTask);

            Logger.LogInformation($"Launching scan for {dispatchedScanTask.FileToScanPath}");
            var created = await DockerClient.Exec
                .ExecCreateContainerAsync(dispatchedScanTask.ContainerId, ContainerExecCreateParams);
            var multiplexedStream = await DockerClient.Exec
                .StartAndAttachContainerExecAsync(created.ID, true);
            Stream outputStream = new MemoryStream();
            await multiplexedStream.CopyOutputToAsync(null, outputStream, outputStream, default);
            return new ScanResult()
            {
                Antivirus = SupportedAntivirus,
                IsFlagged = isResultPositive(outputStream, dispatchedScanTask.FileToScanPath),
            }; ;
        }


        private async Task UploadPayload(DispatchedScan dispatchedScanTask)
        {
            var tarStream = CreateTarGZ($"{dispatchedScanTask.FileToScanPath}");
            await DockerClient.Containers.ExtractArchiveToContainerAsync(
                dispatchedScanTask.ContainerId,
                new ContainerPathStatParameters()
                {
                    Path = "/"
                },
                tarStream,
                default);
            tarStream.Dispose();
        }


        private Stream CreateTarGZ(string filePath)
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



        private bool isResultPositive(Stream outputStream, string payloadFileName)
        {
            outputStream.Position = 0;
            var outputResult = "";
            using (var reader = new StreamReader(outputStream))
            {
                outputResult = reader.ReadToEnd();
                Logger.LogInformation($"Output from {SupportedAntivirus} for {payloadFileName} : {outputResult}");
            }
            outputStream.Dispose();
            return AntivirusBackend.OutputParser.Match(outputResult).Success;
        }

        private List<DispatchedScan> DispatchScanToContainers(string[] payloadPathes, IList<ContainerListResponse> containers)
        {
            List<DispatchedScan> dispatchedScanTasks = new List<DispatchedScan>();

            int i = 0;
            foreach (var payloadPath in payloadPathes)
            {
                var containerDoingTheScan = containers[i % containers.Count];
                Logger.LogInformation($"{payloadPath} dispatched to {containerDoingTheScan.ID}");
                dispatchedScanTasks.Add(new DispatchedScan() { FileToScanPath = payloadPath, ContainerId = containerDoingTheScan.ID });
                i = i++;
            }
            return dispatchedScanTasks;
        }


    }
}

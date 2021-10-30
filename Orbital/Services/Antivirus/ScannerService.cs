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

        public async Task<List<ScanResult>> LaunchScans(string[] payloadPathes,
            IList<ContainerListResponse> containers)
        {

            Logger.LogInformation($"cmd lines configured: {string.Join(' ', AntivirusBackend.GetFullCmd("[[PAYLOAD_NAME]]"))}");

            var dispatchedScanTasks = DispatchScanToContainers(payloadPathes, containers);

            var scanTasks = new List<Task<ScanResult>>();


            foreach (var dispatchedScanTask in dispatchedScanTasks)
            {
                var fullCmd = AntivirusBackend.GetFullCmd(Path.GetFileName(dispatchedScanTask.FileToScanPath));
                var containerExecCreateParams = new ContainerExecCreateParameters
                {
                    AttachStderr = true,
                    AttachStdin = true,
                    AttachStdout = true,
                    Cmd = fullCmd
                };

                scanTasks.Add(Scan(dispatchedScanTask, containerExecCreateParams));

            }

            await Task.WhenAll(scanTasks.ToArray());

            return scanTasks.Select(st => st.Result).ToList();
        }


        private async Task<ScanResult> Scan(DispatchedScan dispatchedScanTask,
            ContainerExecCreateParameters containerExecCreateParams)
        {
            await UploadPayload(dispatchedScanTask);

            Logger.LogInformation($"Launching scan for {dispatchedScanTask.FileToScanPath}");
            var created = await DockerClient.Exec
                .ExecCreateContainerAsync(dispatchedScanTask.ContainerId, containerExecCreateParams);
            var multiplexedStream = await DockerClient.Exec
                .StartAndAttachContainerExecAsync(created.ID, true);
            Stream outputStream = new MemoryStream();
            await multiplexedStream.CopyOutputToAsync(null, outputStream, outputStream, default);
            return new ScanResult()
            {
                Antivirus = SupportedAntivirus,
                IsFlagged = IsResultPositive(outputStream, dispatchedScanTask.FileToScanPath),
            }; ;
        }


        private async Task UploadPayload(DispatchedScan dispatchedScanTask)
        {
            var tarStream = CreateTarGz($"{dispatchedScanTask.FileToScanPath}");
            await DockerClient.Containers.ExtractArchiveToContainerAsync(
                dispatchedScanTask.ContainerId,
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
                Logger.LogInformation($"Output from {SupportedAntivirus} for {payloadFileName} : {outputResult}");
            }
            outputStream.Dispose();
            return AntivirusBackend.OutputParser.Match(outputResult).Success;
        }

        private List<DispatchedScan> DispatchScanToContainers(IEnumerable<string> payloadPathes, IList<ContainerListResponse> containers)
        {
            var dispatchedScanTasks = new List<DispatchedScan>();

            var i = 0;
            foreach (var payloadPath in payloadPathes)
            {
                var containerDoingTheScan = containers[i % containers.Count];
                Logger.LogInformation($"{payloadPath} dispatched to {containerDoingTheScan.ID}");
                dispatchedScanTasks.Add(new DispatchedScan() { FileToScanPath = payloadPath, ContainerId = containerDoingTheScan.ID });
                i++;
            }
            return dispatchedScanTasks;
        }


    }
}

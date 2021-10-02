using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Docker.DotNet;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Orbital.Static;

namespace Orbital.Services.Antivirus
{
    public interface IAntivirusImageBuilder
    {
        Task Build(SupportedAntivirus name);
        Task<bool> ExistsImage(SupportedAntivirus name);
    }

    public class AntivirusImageBuilder : IAntivirusImageBuilder
    {
        private readonly IDockerClient DockerClient;
        private readonly ILogger<AntivirusImageBuilder> Logger;
        protected string DockerFileDirectory { get; }
        protected string Tag { get; }

        public AntivirusImageBuilder(
            IDockerClient dockerClient, ILogger<AntivirusImageBuilder> logger)
        {
            DockerClient = dockerClient;
            Logger = logger;
        }

        public async Task<bool> ExistsImage(SupportedAntivirus name)
        {
            var images = await DockerClient.Images.ListImagesAsync(
                new ImagesListParameters()
                {
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                    { ["reference"] = new Dictionary<string, bool> { [name.ToString().ToLower()] = true } }
                });
            var existsImage = images.Any();
            if (existsImage) Logger.LogInformation($"Docker image for {name} already exists.");
            return existsImage;
        }


        public async Task Build(SupportedAntivirus name)
        {
            if (await ExistsImage(name)) return;

            using var dockerFileStream = CreateTarballForDockerfileDirectory(LocalPathes.DockerFilesDirectory + name.ToString());
            using var responseStream = await DockerClient.Images
                .BuildImageFromDockerfileAsync(
                    dockerFileStream,
                    new ImageBuildParameters
                    {
                        Tags = new List<string> { name.ToString().ToLower() }
                    });
            LoggerHelper.LogStream(responseStream, Logger);
        }

        // from https://github.com/dotnet/Docker.DotNet/issues/309
        private static Stream CreateTarballForDockerfileDirectory(string directory)
        {
            var tarball = new MemoryStream();
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

            using var archive = new TarOutputStream(tarball, Encoding.UTF8)
            {
                //Prevent the TarOutputStream from closing the underlying memory stream when done
                IsStreamOwner = false

            };

            foreach (var file in files)
            {
                //Replacing slashes as KyleGobel suggested and removing leading /
                string tarName = file.Substring(directory.Length).Replace('\\', '/').TrimStart('/');

                //Let's create the entry header
                var entry = TarEntry.CreateTarEntry(tarName);
                using var fileStream = File.OpenRead(file);
                entry.Size = fileStream.Length;
                entry.TarHeader.Mode = Convert.ToInt32("100755", 8); //chmod 755

                archive.PutNextEntry(entry);

                //Now write the bytes of data
                byte[] localBuffer = new byte[32 * 1024];
                while (true)
                {
                    int numRead = fileStream.Read(localBuffer, 0, localBuffer.Length);
                    if (numRead <= 0)
                        break;

                    archive.Write(localBuffer, 0, numRead);
                }

                //Nothing more to do with this entry
                archive.CloseEntry();
            }
            archive.Close();

            //Reset the stream and return it, so it can be used by the caller
            tarball.Position = 0;
            return tarball;
        }
    }
}

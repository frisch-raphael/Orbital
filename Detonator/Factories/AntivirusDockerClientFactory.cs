using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rodin.Services;
using Docker.DotNet;
using Shared.Enums;

namespace Rodin.Factories
{
    public interface IAntivirusDockerClientFactory
    {
        AntivirusImageBuilderService Create(SupportedAntivirus supportedAntivirus);
    }

    public class AntivirusDockerClientFactory : IAntivirusDockerClientFactory
    {
        private readonly Dictionary<string, Func<>>

        private readonly DockerClientConfiguration DockerClientConf;

        public AntivirusDockerClientFactory(
            DockerClientConfiguration dockerClientConf)
        {
            DockerClientConf = dockerClientConf;
        }

        public AntivirusImageBuilderService Create(SupportedAntivirus supportedAntivirus)
        {
            switch (supportedAntivirus)
            {
                case SupportedAntivirus.Defender:
                    return new DefenderDockerClient(DockerClientConf);
                case SupportedAntivirus.Clamav:
                    return new ClamavDockerClient(DockerClientConf);
                //case SupportedAntivirus.Avast:
                //return new DefenderDockerClient(DockerClientConf);
                default:
                    throw new Exception(
                        "Trying to create an image for an unsupported antivirus: " + supportedAntivirus.ToString());
            }
        }
    }
}

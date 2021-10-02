using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Docker.DotNet;
using Rodin.Services;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using System.Text;
using System.Threading;
using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.Logging;

namespace Rodin.Services
{
    public interface IClamavDockerClient
    {

    }

    public class ClamavDockerClient : IClamavDockerClient
    {
        protected override string DockerFileDirectory { get; } = "DockerFiles/Clamav";
        protected override string Tag { get; } = "clamav";


        public ClamavDockerClient(DockerClientConfiguration dockerClientConf, ILogger<DefenderDockerClient> logger)
            : base(dockerClientConf, logger)
        {
        }


    }
}

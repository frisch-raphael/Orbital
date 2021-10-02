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

    public interface IDefendersDockerClient
    {

    }

    public class DefenderDockerClient : IDefenderDockerClient
    {
        protected override string DockerFileDirectory { get; } = "DockerFiles/Defender";
        protected override string Tag { get; } = "defender";


        public DefenderDockerClient(DockerClientConfiguration dockerClientConf, ILogger<DefenderDockerClient> logger)
            : base(dockerClientConf, logger)
        {

        }


    }
}

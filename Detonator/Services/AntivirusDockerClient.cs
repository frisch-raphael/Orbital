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

    public interface IAntivirusDockerClient
    {

    }

    public class AntivirusDockerClient : IAntivirusDockerClient
    {

        private readonly ImageBuilderService ImageBuilder;
        private readonly ContainerLauncherService ContainerLauncher;
        private readonly ScannerService Scanner;

        private List<string> ContainerIds;

        public void Scan()
        {

        }
    }
}

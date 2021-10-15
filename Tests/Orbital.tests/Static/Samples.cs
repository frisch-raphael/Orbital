using System.Collections.Generic;
using Shared.Dtos;
using Shared.Enums;

namespace Orbital.Tests.Static
{
    public static class Samples
    {

        public static readonly Payload SimpleX86Exe = new Payload()
        {
            FileName = "simplefunctionsx86.exe",
            StoragePath = "PayloadSamples\\Healthy\\SimpleFunctions_x86.exe",
            Id = 1,
            PayloadType = PayloadType.NativeExecutable
        };

        public static readonly Payload SimpleX64Exe = new Payload()
        {
            FileName = "simplefunctionsx64.exe",
            StoragePath = "PayloadSamples\\Healthy\\SimpleFunctions_x64.exe",
            Id = 2,
            PayloadType = PayloadType.NativeExecutable
        };

        public static readonly Payload TcpMeterpreterX86Exe = new Payload()
        {
            FileName = "meterpreter.exe",
            StoragePath = "PayloadSamples\\SimpleX86TcpMeterpreter.exe",
            Id = 3,
            PayloadType = PayloadType.NativeExecutable
        };

        public static readonly List<Payload> PayloadSamples = new List<Payload>()
        {
            SimpleX64Exe,
            SimpleX86Exe,
            TcpMeterpreterX86Exe
        };

    }


}

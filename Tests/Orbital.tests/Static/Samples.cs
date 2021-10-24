using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Shared.Dtos;
using Shared.Enums;

namespace Orbital.Tests.Static
{
    public static class Samples
    {

        public static readonly BackendPayload SimpleX86Exe = new()
        {
            FileName = "simplefunctionsx86.exe",
            StoragePath = "PayloadSamples\\Healthy\\SimpleFunctions_x86.exe",
            Id = 1,
            PayloadType = PayloadType.NativeExecutable,
            Functions = GetRandomFunctions(1)
        };

        public static readonly BackendPayload SimpleX64Exe = new()
        {
            FileName = "simplefunctionsx64.exe",
            StoragePath = "PayloadSamples\\Healthy\\SimpleFunctions_x64.exe",
            Id = 2,
            PayloadType = PayloadType.NativeExecutable,
            Functions = GetRandomFunctions(2)
        };

        public static readonly BackendPayload TcpMeterpreterX86Exe = new()
        {
            FileName = "meterpreter.exe",
            StoragePath = "PayloadSamples\\SimpleX86TcpMeterpreter.exe",
            Id = 3,
            PayloadType = PayloadType.NativeExecutable,
            Functions = GetRandomFunctions(3)
        };

        public static readonly List<BackendPayload> PayloadSamples = new()
        {
            SimpleX64Exe,
            SimpleX86Exe,
            TcpMeterpreterX86Exe
        };

        private static List<Function> GetRandomFunctions(int payloadId)
        {
            var fixture = new Fixture();
            
            var rand = new Random();
            var randomNumber = rand.Next(3, 12);
            var randomFunctions = fixture.Build<Function>()
                .With(f => f.BackendPayloadId, payloadId)
                .With(f => f.Id, 0)
                .CreateMany(randomNumber);
            return randomFunctions.ToList();
        }

    }


}

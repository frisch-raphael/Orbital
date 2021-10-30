using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Orbital.Services;
using Orbital.Shared.Static;
using Orbital.Tests.Static;
using Shared.Dtos;
using Xunit;

namespace Orbital.IntegrationTest
{
    public class HammerWrapperTests
    {
        [Fact]
        public void FetchFunctionsFromPdb_GetSimpleX64ExeFunctions()
        {
            var expectedFunctions = GetSampleFunctions();
            var hammerWrapper = new HammerWrapper();
            var functionsFromPayload = hammerWrapper.FetchFunctionsFromPdb(Samples.SimpleX64Exe.StoragePath);

            foreach (var expectedFunction in expectedFunctions)
            {
                functionsFromPayload
                    .Where(f => f.file == expectedFunction.file)
                    .Where(f => f.virtual_adress == expectedFunction.virtual_adress)
                    .Where(f => f.first_line == expectedFunction.first_line)
                    .Where(f => f.name == expectedFunction.name)
                    .Should()
                    .NotBeEmpty($" function \"{expectedFunction.name}\" should be in {Path.GetFileName(Samples.SimpleX64Exe.StoragePath)}");
            }

            // var areExpectedFunctionsFetched = marshalledFunctions
            //     .All(expectedFunction => actualFunctions.Contains<MarshalledFunction>(expectedFunction));
            // Assert.True(areExpectedFunctionsFetched, "Functions fetched from SimpleFunctions_x64.exe don't contain functions known to be in it");
        }

        private static List<MarshalledFunction> GetSampleFunctions()
        {
            var expectedFunctions = new List<MarshalledFunction>
            {
                new()
                {
                    name = "private: void __cdecl Najin::ExecuteNajin(void)",
                    file = "C:\\Users\\rafou\\source\\repos\\HammerTest\\SimpleFunctions\\najin.cpp",
                    first_line = 8,
                    virtual_adress = 71584,
                    length = 41
                },
                new()
                {
                    name = "void __cdecl inlineNajin(void)",
                    file = "C:\\Users\\rafou\\source\\repos\\HammerTest\\SimpleFunctions\\najin.cpp",
                    first_line = 12,
                    virtual_adress = 71648,
                    length = 37
                },
                new()
                {
                    name = "private: __cdecl Najin::Najin(void)",
                    file = "C:\\Users\\rafou\\source\\repos\\HammerTest\\SimpleFunctions\\najin.cpp",
                    first_line = 4,
                    virtual_adress = 71520,
                    length = 48
                }
            };

            return expectedFunctions;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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
            var hammerWrapper = new HammerWrapper(new FunctionService());
            var actualFunctions = hammerWrapper.FetchFunctionsFromPdb(Samples.SimpleX64Exe.StoragePath);

            var areExpectedFunctionsFetched = expectedFunctions
                .All(expectedFunction => actualFunctions.Contains<Function>(expectedFunction));
            Assert.True(areExpectedFunctionsFetched, "Functions fetched from SimpleFunctions_x64.exe don't contain functions known to be in it");
        }

        private static IEnumerable<Function> GetSampleFunctions()
        {
            var expectedFunctions = new List<Function>
            {
                new()
                {
                    Id = 0,
                    Name = "private: void __cdecl Najin::ExecuteNajin(void)",
                    File = "C:\\Users\\rafou\\source\\repos\\HammerTest\\SimpleFunctions\\najin.cpp",
                    FirstLine = 8,
                    Offset = 71584,
                    Length = 41,
                    BackendPayloadId = 0
                },
                new()
                {
                    Id = 0,
                    Name = "void __cdecl inlineNajin(void)",
                    File = "C:\\Users\\rafou\\source\\repos\\HammerTest\\SimpleFunctions\\najin.cpp",
                    FirstLine = 12,
                    Offset = 71648,
                    Length = 37,
                    BackendPayloadId = 0
                },
                new()
                {
                    Id = 0,
                    Name = "private: __cdecl Najin::Najin(void)",
                    File = "C:\\Users\\rafou\\source\\repos\\HammerTest\\SimpleFunctions\\najin.cpp",
                    FirstLine = 4,
                    Offset = 71520,
                    Length = 48,
                    BackendPayloadId = 0
                }
            };

            return expectedFunctions;
        }
    }
}

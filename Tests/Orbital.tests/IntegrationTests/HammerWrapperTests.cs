﻿using System;
using System.Collections.Generic;
using System.Linq;
using Orbital.Services;
using Orbital.Shared.Static;
using Orbital.Tests.Static;
using Shared.Dtos;
using Xunit;

namespace Orbital.Shared
{
    public class HammerWrapperTests
    {
        [Fact]
        public void FetchFunctionsFromPdb_GetSimpleX86ExeFunctions()
        {
            var expectedFunctions = GetSampleFunctions();
            var hammerWrapper = new HammerWrapper(new FunctionService());
            var actualFunctions = hammerWrapper.FetchFunctionsFromPdb(Samples.SimpleX64Exe.StoragePath);

            bool areExpectedFunctionsFetched = expectedFunctions
                .All(expectedFunction => actualFunctions.Contains<Function>(expectedFunction));
            Assert.True(areExpectedFunctionsFetched, "Functions fetched from SimpleFunctions_x64.exe don't contain functions known to be in it");
        }

        private List<Function> GetSampleFunctions()
        {
            var expectedFunctions = new List<Function>();
            expectedFunctions.Add(new Function
            {
                Id = 0,
                Name = "private: void __cdecl Najin::ExecuteNajin(void)",
                File = "C:\\Users\\rafou\\source\\repos\\HammerTest\\SimpleFunctions\\najin.cpp",
                FirstLine = 8,
                AdressSection = 2,
                AdressOffset = 2016,
                Length = 41,
            });
            expectedFunctions.Add(new Function
            {
                Id = 0,
                Name = "void __cdecl inlineNajin(void)",
                File = "C:\\Users\\rafou\\source\\repos\\HammerTest\\SimpleFunctions\\najin.cpp",
                FirstLine = 12,
                AdressSection = 2,
                AdressOffset = 2080,
                Length = 37,
            });
            expectedFunctions.Add(new Function
            {
                Id = 0,
                Name = "private: __cdecl Najin::Najin(void)",
                File = "C:\\Users\\rafou\\source\\repos\\HammerTest\\SimpleFunctions\\najin.cpp",
                FirstLine = 4,
                AdressSection = 2,
                AdressOffset = 1952,
                Length = 48,
            });

            return expectedFunctions;
        }
    }
}
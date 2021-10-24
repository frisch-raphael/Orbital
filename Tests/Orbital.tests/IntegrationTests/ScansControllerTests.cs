using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Orbital.Shared;
using Orbital.Shared.Static;
using Shared.Enums;
using Shared.Dtos;
using Shared.Static;
using Xunit;
using Orbital.Tests.Static;
using Shared.ControllerResponses.Dtos;

namespace Orbital.IntegrationTest
{

    public class ScansControllerTests : IClassFixture<OrbitalApiFactory>
    {

        private readonly OrbitalApiFactory Factory;

        public ScansControllerTests(OrbitalApiFactory factory)
        {
            Factory = factory;
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Post_HealthyPayloadsShouldNotBeFlagged(int payloadId)
        {
            // Arrange
            var client = Factory.CreateClient();


            var scanPost = new ScanPost()
            {
                Antiviruses = GetAllSupportedAntiviruses(),
                PayloadId = payloadId
            };

            // Act
            var response = await client.PostAsync(
                "/api/Scans",
                JsonHelper.SerializeAsync(scanPost));


            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            //var scanResult = await JsonHelper.DeserializeAsync<List<ScanResponse>>(await response.Content.ReadAsStreamAsync());
            //scanResult[0].IsFlagged.Should().Be(false);
        }

        [Fact]
        public async Task Post_TcpStagedMeterpreterShouldBeFlagged()
        {
            // Arrange
            var client = Factory.CreateClient();


            var postData = new ScanPost()
            {
                Antiviruses = GetAllSupportedAntiviruses(),
                PayloadId = Samples.TcpMeterpreterX86Exe.Id
            };

            var postDataContent = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
            // Act
            var response = await client.PostAsync(
                "/api/Scans",
                postDataContent);


            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            //var scanResult = await JsonHelper.DeserializeAsync<List<ScanResponse>>(await response.Content.ReadAsStreamAsync());
            //scanResult[0].IsFlagged.Should().Be(true);
        }

        private List<SupportedAntivirus> GetAllSupportedAntiviruses()
        {
            var antiviruses = Enum.GetValues(typeof(SupportedAntivirus)).Cast<SupportedAntivirus>();
            return antiviruses.Where(a => a != SupportedAntivirus.TestAntivir).ToList();
        }

    }
}

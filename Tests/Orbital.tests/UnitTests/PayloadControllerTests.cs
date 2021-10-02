using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using AutoFixture;
using Docker.DotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Orbital.Controllers;
using Orbital.Factories;
using Orbital.Model;
using Orbital.Services;
using Orbital.Tests.Services;
using Shared.Dtos;
using Xunit;

namespace Orbital.UniTests
{

    public class PayloadControllerTests : DbTestService
    {
        public PayloadControllerTests() : base(
            new DbContextOptionsBuilder<OrbitalContext>()
                .UseSqlite("Filename=Test.db")
                .Options)
        {
        }

        [Fact]
        public async Task PayloadPost_PostingSimpleExeShouldReturnSuccess()
        {
            using var orbitalContext = new OrbitalContext(ContextOptions);
            using (var mock = AutoMock.GetLoose(cfg => cfg.RegisterInstance(orbitalContext).As<OrbitalContext>()))
            {
                var payloadFileStorerFactoryMock = mock.Mock<IPayloadFileStorerFactory>();
                var payloadFileStorerMock = mock.Mock<IPayloadFileStorer>();
                var testPayload = GetTestPayload();

                payloadFileStorerMock
                    .Setup<Task>(service => service.StoreFile())
                    .Verifiable();
                payloadFileStorerMock
                    .Setup<Payload>(service => service.StorePayloadDataInDb())
                    .Returns(testPayload);
                payloadFileStorerFactoryMock
                    .Setup<IPayloadFileStorer>(factory => factory.Create(It.IsAny<UploadedFile>()))
                    .Returns(() => payloadFileStorerMock.Object);

                var cls = mock.Create<PayloadsController>();
                cls.ControllerContext = ControllerContextService.GetFakeControllerContext();

                //Act
                var actionResult = await cls.Post(GetTestFormFile());

                //Assert
                var returnedObject = Assert.IsType<CreatedResult>(actionResult.Result);
            }
        }

        private IFormFile GetTestFormFile()
        {
            byte[] bytes = Encoding.UTF8.GetBytes("This is a fake file");

            return new FormFile(
                baseStream: new MemoryStream(bytes),
                baseStreamOffset: 0,
                length: bytes.Length,
                name: "Data",
                fileName: "fakefile"
            );
        }

        private Payload GetTestPayload()
        {
            var fixture = new Fixture();
            var fakePayload = fixture.Create<Payload>();
            return fakePayload;
        }
    }
}

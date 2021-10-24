using System.IO;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Orbital.Controllers;
using Orbital.Factories;
using Orbital.Model;
using Orbital.Pocos;
using Orbital.Services;
using Orbital.Tests.Services;
using Orbital.Tests.Static;
using Shared.Api.ApiErrors;
using Shared.Dtos;
using Xunit;

namespace Orbital.UniTests
{

    public class PayloadControllerDeleteTests : DbTestService
    {
        public PayloadControllerDeleteTests() : base(
            new DbContextOptionsBuilder<OrbitalContext>()
                .UseSqlite("Filename=Test.db")
                .Options)
        {
        }

        [Fact]
        public async Task PayloadDelete_DeletingPayloadShouldReturnSuccess()
        {
            await using var orbitalContext = new OrbitalContext(ContextOptions);
            using var mock = AutoMock.GetLoose(cfg => cfg.RegisterInstance(orbitalContext).As<OrbitalContext>());


            var cls = mock.Create<PayloadsController>();
            cls.ControllerContext = ControllerContextService.GetFakeControllerContext();

            //Act
            var actionResult = cls.Delete(Samples.SimpleX86Exe.Id);

            //Assert
            var returnedObject = Assert.IsType<OkResult>(actionResult);
        }

        [Fact]
        public async Task PayloadDelete_DeletingNotExistingPayloadShouldFail()
        {
            await using var orbitalContext = new OrbitalContext(ContextOptions);
            using var mock = AutoMock.GetLoose(cfg => cfg.RegisterInstance(orbitalContext).As<OrbitalContext>());


            var cls = mock.Create<PayloadsController>();
            cls.ControllerContext = ControllerContextService.GetFakeControllerContext();

            //Act
            var actionResult = cls.Delete(1000000);

            //Assert
            var returnedObject = Assert.IsType<NotFoundObjectResult>(actionResult);
        }

    }
}

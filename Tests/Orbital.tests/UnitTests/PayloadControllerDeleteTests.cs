using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orbital.Controllers;
using Orbital.Model;
using Orbital.Tests.Services;
using Orbital.Tests.Static;
using Xunit;

namespace Orbital.UniTests
{

    public class PayloadControllerDeleteTests : DbTestService
    {
        public PayloadControllerDeleteTests() : base(
            new DbContextOptionsBuilder<OrbitalContext>()
                .UseSqlite("Filename=Payload.db")
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
            Assert.IsType<OkResult>(actionResult);
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
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

    }
}

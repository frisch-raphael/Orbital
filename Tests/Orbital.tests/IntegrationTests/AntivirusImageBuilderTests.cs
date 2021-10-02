using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using Orbital.Services.Antivirus;
using Shared.Enums;
using Xunit;

namespace Orbital.Shared
{

    public class AntivirusImageBuilderTests
    {
        private readonly DockerClient DockerClient = new DockerClientConfiguration().CreateClient();

        [Fact]
        public async Task Build_TestAntivirusDockerImageShouldBuild()
        {
            using (var mock = AutoMock.GetLoose(cfg => cfg.RegisterInstance(DockerClient).As<IDockerClient>()))
            {
                //Expected

                //Act
                await RemoveTestImage();
                var cls = mock.Create<AntivirusImageBuilder>();
                await cls.Build(SupportedAntivirus.TestAntivir);

                //Assert
                var images = await DockerClient.Images.ListImagesAsync(new ImagesListParameters() { }, default);
                //images.Where(i => i.RepoTags).Should().Equal()
                var createdImage = images.Where(i => i.RepoTags.Any(repo => repo.Contains(SupportedAntivirus.TestAntivir.ToString().ToLower())));
                createdImage.Should().HaveCount(1);
            }
        }

        private async Task RemoveTestImage()
        {
            try
            {
                await DockerClient.Images.DeleteImageAsync(
                    SupportedAntivirus.TestAntivir.ToString().ToLower(),
                    new ImageDeleteParameters() { Force = true });

            }
            catch (DockerImageNotFoundException)
            {
            }
        }



    }
}

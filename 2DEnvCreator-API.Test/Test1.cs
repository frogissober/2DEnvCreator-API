using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using _2DEnvCreator_API.Controllers;
using _2DEnvCreator_API.Models;
using _2DEnvCreator_API.Repositories;
using Microsoft.Extensions.Logging;

namespace _2DEnvCreator_API.Test
{
    [TestClass]
    public sealed class EnvironmentControllerTests
    {
        private const string TestUserId = "83430dee-3245-4559-8e41-22043eac0c7e";

        [TestMethod]
        public async Task GetEnvironments_ReturnsOk_WithEnvironments()
        {
            var mockRepository = new Mock<IEnvironmentRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockLogger = new Mock<ILogger<EnvironmentController>>();

            var controller = new EnvironmentController(mockRepository.Object, mockAuthService.Object, mockLogger.Object);

            var environments = new List<Environment2D>
            {
                new Environment2D { Id = 1, Name = "TestEnv1" },
                new Environment2D { Id = 2, Name = "TestEnv2" }
            };

            mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(TestUserId);
            mockRepository.Setup(repo => repo.GetEnvironmentsByUserId(TestUserId)).ReturnsAsync(environments);

            var result = await controller.GetEnvironments();

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedEnvironments = okResult.Value as List<Environment2D>;
            Assert.IsNotNull(returnedEnvironments);
            Assert.AreEqual(2, returnedEnvironments.Count);
        }

        [TestMethod]
        public async Task GetEnvironment_ReturnsNotFound_WhenEnvironmentDoesNotExist()
        {
            var mockRepository = new Mock<IEnvironmentRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockLogger = new Mock<ILogger<EnvironmentController>>();

            var controller = new EnvironmentController(mockRepository.Object, mockAuthService.Object, mockLogger.Object);

            var environmentId = 1;

            mockRepository.Setup(repo => repo.GetEnvironmentById(environmentId)).ReturnsAsync((Environment2D?)null);

            var result = await controller.GetEnvironment(environmentId);

            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public async Task CreateEnvironment_ReturnsCreated_WithEnvironment()
        {
            var mockRepository = new Mock<IEnvironmentRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockLogger = new Mock<ILogger<EnvironmentController>>();

            var controller = new EnvironmentController(mockRepository.Object, mockAuthService.Object, mockLogger.Object);

            var environment = new Environment2D { Name = "NewEnv", Height = 100, Width = 100 };
            var createdEnvironment = new Environment2D { Id = 1, Name = "NewEnv", Height = 100, Width = 100 };

            mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(TestUserId);
            mockRepository.Setup(repo => repo.CreateEnvironment(environment, TestUserId)).ReturnsAsync(createdEnvironment);

            var result = await controller.CreateEnvironment(environment);

            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            var returnedEnvironment = createdAtActionResult.Value as Environment2D;
            Assert.IsNotNull(returnedEnvironment);
            Assert.AreEqual(createdEnvironment.Id, returnedEnvironment.Id);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2DEnvCreator_API.Models;
using _2DEnvCreator_API.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using _2DEnvCreator_API.Interfaces;
using Microsoft.Extensions.Logging;

namespace _2DEnvCreator_API.Test
{
    [TestClass]
    public class EnvironmentControllerTests
    {
        [TestMethod]
        public async Task GetEnvironments_ReturnsOk_WithEnvironments()
        {
            // Arrange
            var mockRepository = new Mock<IEnvironmentRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockLogger = new Mock<ILogger<EnvironmentController>>();

            var controller = new EnvironmentController(mockRepository.Object, mockAuthService.Object, mockLogger.Object);

            var userId = "c52b5576-4bc2-4302-a400-f13af1cd43af";
            var environments = new List<Environment2D>
            {
                new Environment2D { Name = "TestEnv1", Height = 100, Width = 100 },
                new Environment2D { Name = "TestEnv2", Height = 200, Width = 200 }
            };

            mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(userId);
            mockRepository.Setup(repo => repo.GetEnvironmentsByUserId(userId)).ReturnsAsync(environments);

            // Act
            var result = await controller.GetEnvironments();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedEnvironments = okResult.Value as List<Environment2D>;
            Assert.IsNotNull(returnedEnvironments);
            Assert.AreEqual(2, returnedEnvironments.Count);
        }

        [TestMethod]
        public async Task CreateEnvironment_ReturnsBadRequest_WhenExceedingMaxWorlds()
        {
            // Arrange
            var mockRepository = new Mock<IEnvironmentRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockLogger = new Mock<ILogger<EnvironmentController>>();

            var controller = new EnvironmentController(mockRepository.Object, mockAuthService.Object, mockLogger.Object);

            var userId = "c52b5576-4bc2-4302-a400-f13af1cd43af";
            var existingWorlds = new List<Environment2D>();
            for (int i = 0; i < 5; i++)
            {
                existingWorlds.Add(new Environment2D { Name = $"World{i}", Height = 100, Width = 100 });
            }

            mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(userId);
            mockRepository.Setup(repo => repo.GetEnvironmentsByUserId(userId)).ReturnsAsync(existingWorlds);

            var newWorld = new Environment2D { Name = "NewWorld", Height = 200, Width = 200 };

            // Act
            var result = await controller.CreateEnvironment(newWorld);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Maximum of 5 worlds reached", badRequestResult.Value);
        }

        [TestMethod]
        public async Task CreateEnvironment_ReturnsBadRequest_WhenNameAlreadyExists()
        {
            // Arrange
            var mockRepository = new Mock<IEnvironmentRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();
            var mockLogger = new Mock<ILogger<EnvironmentController>>();

            var controller = new EnvironmentController(mockRepository.Object, mockAuthService.Object, mockLogger.Object);

            var userId = "user1";
            var existingWorlds = new List<Environment2D>
            {
                new Environment2D { Name = "ExistingWorld", Height = 100, Width = 100 }
            };

            mockAuthService.Setup(auth => auth.GetCurrentAuthenticatedUserId()).Returns(userId);
            mockRepository.Setup(repo => repo.GetEnvironmentsByUserId(userId)).ReturnsAsync(existingWorlds);

            var newWorld = new Environment2D { Name = "ExistingWorld", Height = 200, Width = 200 };

            // Act
            var result = await controller.CreateEnvironment(newWorld);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("World name must be unique", badRequestResult.Value);
        }
    }
}

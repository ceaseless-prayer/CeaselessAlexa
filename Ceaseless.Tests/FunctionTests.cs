using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Amazon.Lambda.Core;

namespace Ceaseless.Tests
{
    [TestClass]
    public class FunctionTests
    {

        private Mock<IEngine> mockEngine;
        private Mock<ILambdaContext> mockLambdaContext;
        private Mock<ILambdaLogger> mockLambdaLogger;
        private Function function;

        [TestInitialize]
        public void Initialize()
        {
            mockEngine = new Mock<IEngine>();
            mockLambdaContext = new Mock<ILambdaContext>();
            mockLambdaLogger = new Mock<ILambdaLogger>();

            mockLambdaContext.Setup(mock => mock.Logger).Returns(mockLambdaLogger.Object);

            function = new Function(mockEngine.Object);
        }

        [TestMethod]
        public async Task HandleAddPersonToPrayIntent_ShouldCollectInput_CallAddPersonToPray_ReturnSkillResponse()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString("N");
            var name = Guid.NewGuid().ToString("N");

            var skillRequest = new SkillRequest
            {
                Session = new Session
                {
                    User = new User
                    {
                        UserId = userId
                    }
                }
            };

            var intentRequest = new IntentRequest
            {
                Intent = new Intent
                {
                    Slots = new Dictionary<string, Slot>
                    {
                        { "Name", new Slot { Value = name } }
                    }
                }
            };

            // Act
            var response = await function.HandleAddPersonToPrayIntent(skillRequest, intentRequest, mockLambdaContext.Object);

            // Assert
            mockEngine.Verify(mock => mock.AddPersonToPray(userId, name), Times.Once);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Response.OutputSpeech);
        }

        [TestMethod]
        public async Task HandleGetNextPersonToPrayIntent_ShouldCollectInput_CallGetPeopleToPray_ReturnSkillResponse()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString("N");
            var name = Guid.NewGuid().ToString("N");

            var skillRequest = new SkillRequest
            {
                Session = new Session
                {
                    User = new User
                    {
                        UserId = userId
                    }
                }
            };

            mockEngine.Setup(mock => mock.GetPeopleToPray(userId)).ReturnsAsync(new List<string> { name });

            // Act
            var response = await function.HandleGetNextPersonToPrayIntent(skillRequest, null, mockLambdaContext.Object);

            // Assert
            mockEngine.Verify(mock => mock.GetPeopleToPray(userId), Times.Once);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Response.OutputSpeech);
        }

        [TestMethod]
        public async Task HandleGetNextPersonToPrayIntent_ShouldCollectInput_CallGetPeopleToPray_ReturnSkillResponse_WhenEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString("N");

            var skillRequest = new SkillRequest
            {
                Session = new Session
                {
                    User = new User
                    {
                        UserId = userId
                    }
                }
            };

            mockEngine.Setup(mock => mock.GetPeopleToPray(userId)).ReturnsAsync(new List<string>());

            // Act
            var response = await function.HandleGetNextPersonToPrayIntent(skillRequest, null, mockLambdaContext.Object);

            // Assert
            mockEngine.Verify(mock => mock.GetPeopleToPray(userId), Times.Once);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Response.OutputSpeech);
        }

    }
}

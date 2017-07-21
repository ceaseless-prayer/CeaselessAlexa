using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ceaseless.Tests
{
    [TestClass]
    public class EngineTests
    {

        private Mock<IAmazonS3> mockS3Client;
        private Engine engine;

        [TestInitialize]
        public void Initialize()
        {
            mockS3Client = new Mock<IAmazonS3>();
            engine = new Engine(mockS3Client.Object);
        }

        [TestMethod]
        public async Task GetPeopleToPray_ShouldReturnEmptyListWhenDataNotAvailable()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString("N");

            mockS3Client
                .Setup(mock => mock.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonS3Exception(null, ErrorType.Unknown, "NotFound", null, HttpStatusCode.NotFound));

            // Act
            var people = await engine.GetPeopleToPray(userId);

            // Assert
            Assert.IsNotNull(people);
            Assert.AreEqual(0, people.Count);
        }

        [TestMethod]
        public async Task GetPeopleToPray_ShouldTryGetDataFromS3_ParseJson_ReturnList()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString("N");
            var peopleBytes = Encoding.UTF32.GetBytes(JsonConvert.SerializeObject(new List<string>()));

            mockS3Client
                .Setup(mock => mock.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetObjectResponse { ResponseStream = new MemoryStream(peopleBytes) });

            // Act
            var people = await engine.GetPeopleToPray(userId);

            // Assert
            Assert.IsNotNull(people);
            mockS3Client.Verify(mock => mock.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task AddPersonToPray_ShouldTryGetDataFromS3_ParseJson_CallPutDataToS3()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString("N");
            var name = Guid.NewGuid().ToString("N");
            var peopleBytes = Encoding.UTF32.GetBytes(JsonConvert.SerializeObject(new List<string>()));

            mockS3Client
                .Setup(mock => mock.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetObjectResponse { ResponseStream = new MemoryStream(peopleBytes) });

            // Act
            await engine.AddPersonToPray(userId, name);

            // Assert
            mockS3Client.Verify(mock => mock.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            mockS3Client.Verify(mock => mock.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

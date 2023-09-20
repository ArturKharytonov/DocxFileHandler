using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using DocxFileHandler.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace DocxFileHandler.Tests
{
    public class BlobStorageServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly BlobContainerClient _containerClient = Substitute.For<BlobContainerClient>();

        public BlobStorageServiceTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();
        }

        [Fact]
        public void GenerateSasUrl_WhenBlobExists_ReturnsSasUri()
        {
            // Arrange
            var blobName = "testBlob";
            var blobClient = Substitute.For<BlobClient>();
            blobClient.CanGenerateSasUri.Returns(true);
            

            var service = new BlobStorageService(_configuration);
            _containerClient.GetBlobClient(blobName).Returns(blobClient);

            // Act
            var result = service.GenerateSasUrl(blobName);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UploadFileAsync_WhenBlobExists_ReturnsFalse()
        {
            // Arrange
            var fileName = "test.docx";
            var formFile = Substitute.For<IFormFile>();
            formFile.FileName.Returns(fileName);

            var blobClient = Substitute.For<BlobClient>();

            _containerClient.GetBlobClient(fileName).Returns(blobClient);

            var service = new BlobStorageService(_configuration);

            // Act
            var result = await service.UploadFileAsync(formFile);

            // Assert
            Assert.False(result);
        }
    }
}

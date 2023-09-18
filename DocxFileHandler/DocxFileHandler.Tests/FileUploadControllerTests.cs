using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocxFileHandler.Controllers;
using DocxFileHandler.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

namespace DocxFileHandler.Tests
{
    public class FileUploadControllerTests
    {
        private readonly Mock<BlobStorageService> _blobServiceMock;
        private readonly FileUploadController _controller;

        public FileUploadControllerTests()
        {
            _blobServiceMock = new Mock<BlobStorageService>();
            _controller = new FileUploadController(_blobServiceMock.Object);
        }

        [Fact]
        public async Task PostFile_ValidFile_ReturnsOkResult()
        {
            _blobServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>())).ReturnsAsync(true);
            _blobServiceMock.Setup(s => s.SetBlobMetadataAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()));

            var result = await _controller.PostFileAsync(Mock.Of<IFormFile>(), "test@example.com");

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("File uploaded successfully!", ((OkObjectResult)result).Value);
        }

        [Fact]
        public async Task PostFile_InvalidFile_ReturnsBadRequest()
        {
            var result = await _controller.PostFileAsync(Mock.Of<IFormFile>(), "test@example.com");

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid file or format.", ((BadRequestObjectResult)result).Value);
        }

        [Fact]
        public async Task PostFile_FileAlreadyExists_ReturnsConflict()
        {
            _blobServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>())).ReturnsAsync(false);

            var result = await _controller.PostFileAsync(Mock.Of<IFormFile>(), "test@example.com");

            Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("File already exist in storage", ((ConflictObjectResult)result).Value);
        }

        [Fact]
        public async Task PostFile_ThrowsException_ReturnsStatusCode500()
        {
            _blobServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>())).ThrowsAsync(new Exception("Something went wrong"));

            var result = await _controller.PostFileAsync(Mock.Of<IFormFile>(), "test@example.com");

            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
        }
    }
}
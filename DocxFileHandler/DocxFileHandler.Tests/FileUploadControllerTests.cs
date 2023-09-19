using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DocxFileHandler.Controllers;
using DocxFileHandler.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Xunit;

namespace DocxFileHandler.Tests
{
    public class FileUploadControllerTests
    {
        private void SetupHttpContextWithFormFile(FileUploadController controller, IFormFile file)
        {
            var files = new FormFileCollection { file };
            var httpContext = new DefaultHttpContext { Request = { Form = new FormCollection(new Dictionary<string, StringValues>(), files) } };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }
        [Fact]
        public async Task PostFileAsync_ValidRequest_ReturnsOkResult()
        {
            var mockBlobService = Substitute.For<IBlobStorageService>();
            var controller = new FileUploadController(mockBlobService);

            var fileBytes = Encoding.UTF8.GetBytes("This is a dummy file");
            var file = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "Test", "test1.docx");

            // Set up HttpContext with form files
            SetupHttpContextWithFormFile(controller, file);

            mockBlobService.UploadFileAsync(Arg.Any<IFormFile>()).Returns(true);
            mockBlobService.GenerateSasUrl(Arg.Any<string>()).Returns(new Uri("http://example.com"));
            mockBlobService.SetBlobMetadataAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, string>>()).Returns(Task.CompletedTask);
            // Act
            var result = await controller.PostFileAsync("test@example.com");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("File uploaded successfully!", okResult.Value);
        }

        [Fact]
        public async Task PostFileAsync_InvalidFile_ReturnsBadRequest()
        {
            // Arrange
            var mockBlobService = Substitute.For<IBlobStorageService>();
            var controller = new FileUploadController(mockBlobService);

            var fileBytes = Encoding.UTF8.GetBytes("This is a dummy file");
            var file = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "Test", "test.txt");

            // Set up HttpContext with form files
            SetupHttpContextWithFormFile(controller, file);

            // Act
            var result = await controller.PostFileAsync("test@example.com");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid file or format.", badRequestResult.Value);
        }

        [Fact]
        public async Task PostFileAsync_FileAlreadyExists_ReturnsConflict()
        {
            // Arrange
            var mockBlobService = Substitute.For<IBlobStorageService>();
            var controller = new FileUploadController(mockBlobService);

            var fileBytes = Encoding.UTF8.GetBytes("This is a dummy file");
            var file = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "Test", "test.docx");

            // Set up HttpContext with form files
            SetupHttpContextWithFormFile(controller, file);

            mockBlobService.UploadFileAsync(Arg.Any<IFormFile>()).Returns(false);

            // Act
            var result = await controller.PostFileAsync("test@example.com");

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("File already exist in storage", conflictResult.Value);
        }
    }
}

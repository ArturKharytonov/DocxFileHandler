using System;
using System.Net.Mail;
using System.Threading.Tasks;
using AzureBlobTrigger;
using Microsoft.Azure.Storage.Blob;
using Moq;
using NSubstitute;
using Xunit;

namespace DocxFileHandler.Tests
{
    public class EmailHandlerTests
    {
        [Fact]
        public async Task RunAsync_InvalidMetadata_DoesNotSendEmail()
        {
            // Arrange
            var smtpClient = Substitute.For<SmtpClient>();
            var emailHandler = new EmailHandler(smtpClient);

            var blob = new CloudBlockBlob(new Uri("https://example.com/blob"));

            // Act
            var result = await emailHandler.RunAsync(blob);

            // Assert
            Assert.False(result);
        }
    }
}
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace DocxFileHandler.Services
{
    public class BlobStorageService
    {
        private readonly CloudBlobClient _blobClient;
        private const string _containerName = "taskcontainer";
        public BlobStorageService(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public string GetContainerName() => _containerName;
        public async Task UploadFileAsync(string containerName, IFormFile file)
        {
            var container = _blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(file.FileName);

            await using var stream = file.OpenReadStream();

            await blob.UploadFromStreamAsync(stream);
        }
    }
}

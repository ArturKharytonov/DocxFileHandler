using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace DocxFileHandler.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _containerClient;
        private const string _containerName = "taskcontainer";

        public BlobStorageService(string storageConnectionString)
        {
            _blobServiceClient = new BlobServiceClient(storageConnectionString);
            _containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        }

        public Uri? GenerateSasUrl(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            if (!blobClient.CanGenerateSasUri) return null;

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri;

        }
        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            var blob = _containerClient.GetBlobClient(file.FileName);

            if (await blob.ExistsAsync())
                return false;

            await using var stream = file.OpenReadStream();

            await blob.UploadAsync(stream);
            return true;
        }

        public async Task SetBlobMetadataAsync(string blobName,
            Dictionary<string, string> metadata)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.SetMetadataAsync(metadata);
        }
    }
}

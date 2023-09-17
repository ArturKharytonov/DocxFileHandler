using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace DocxFileHandler.Services
{
    public class BlobStorageService
    {
        private readonly CloudBlobClient _blobClient;
        private readonly CloudBlobContainer _container;
        private const string _containerName = "taskcontainer";

        public BlobStorageService(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
            _container = _blobClient.GetContainerReference(_containerName);
        }

        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            var blob = _container.GetBlockBlobReference(file.FileName);

            if (await blob.ExistsAsync())
                return false;

            await using var stream = file.OpenReadStream();

            await blob.UploadFromStreamAsync(stream);
            return true;
        }

        public async Task SetBlobMetadataAsync(string blobName,
            Dictionary<string, string> metadata)
        {
            var blockBlob = _container.GetBlockBlobReference(blobName);

            blockBlob.Metadata.Clear();
            foreach (var item in metadata)
                blockBlob.Metadata.Add(item);


            await blockBlob.SetMetadataAsync();
        }
    }
}

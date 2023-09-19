namespace DocxFileHandler.Services.Interfaces
{
    public interface IBlobStorageService
    {
        public Uri? GenerateSasUrl(string blobName);
        public Task<bool> UploadFileAsync(IFormFile file);
        public Task SetBlobMetadataAsync(string blobName,
            Dictionary<string, string> metadata);
    }
}

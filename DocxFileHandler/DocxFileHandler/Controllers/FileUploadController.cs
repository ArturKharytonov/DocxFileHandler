using DocxFileHandler.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Queue;

namespace DocxFileHandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly BlobStorageService _blobService;

        public FileUploadController(BlobStorageService blobService) 
            => _blobService = blobService;


        [HttpPost]
        public async Task<IActionResult> PostFile(IFormFile file, string email)
        {
            try
            {
                if (file.Length <= 0 || Path.GetExtension(file.FileName).ToLower() != ".docx")
                    return BadRequest("Invalid file or format.");

                var metadata = new Dictionary<string, string>
                {
                    { "Key1", email }
                };

                if (!await _blobService.UploadFileAsync(file)) return Conflict("File already exist in storage");

                await _blobService.SetBlobMetadataAsync(file.FileName, metadata);
                return Ok("File uploaded successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }
        }
    }
}

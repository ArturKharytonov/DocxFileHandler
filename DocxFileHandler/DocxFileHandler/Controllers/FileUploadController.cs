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

                await _blobService.UploadFileAsync($"{_blobService.GetContainerName()}", file);

                return Ok("File uploaded successfully!");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

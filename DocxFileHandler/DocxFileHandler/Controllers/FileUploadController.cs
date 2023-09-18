using DocxFileHandler.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace DocxFileHandler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAnyOrigin")]
    public class FileUploadController : ControllerBase
    {
        private readonly BlobStorageService _blobService;
        public FileUploadController(BlobStorageService blobService) 
            => (_blobService) = (blobService);

        [HttpPost]
        public async Task<IActionResult> PostFileAsync([FromQuery] string email)
        {
            try
            {
                var file = Request.Form.Files[0];

                if (file.Length <= 0 || Path.GetExtension(file.FileName).ToLower() != ".docx")
                    return BadRequest("Invalid file or format.");

                if (!await _blobService.UploadFileAsync(file)) return Conflict("File already exist in storage");

                var sasUri = _blobService.GenerateSasUrl(file.FileName);

                var metadata = new Dictionary<string, string>
                {
                    { "Key1", email },
                    { "Key2", sasUri.ToString()}
                };
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Workshop.Controllers
{
    [ApiController]
    [Route("api/userprofile")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        // Task 4: Upload profile image
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, file.FileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Ok(new { message = "Uploaded successfully", fileName = file.FileName });
        }

        // Task 4: Download profile image
        [HttpGet("download/{filename}")]
        public IActionResult Download(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filename);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/octet-stream", filename);
        }
    }
}
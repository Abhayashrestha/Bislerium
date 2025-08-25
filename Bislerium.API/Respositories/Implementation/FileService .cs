using Bislerium.API.Respositories.Respository;

namespace Bislerium.API.Respositories.Implementation
{
    public class FileService: IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            const long maxFileSize = 3 * 1024 * 1024; // 3 MB
            if (file.Length > maxFileSize)
            {
                throw new Exception("File size must not exceed 3 MB.");
            }

            var allowedExtensions = new List<string> { ".jpeg", ".jpg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new Exception("Only JPEG and PNG files are allowed.");
            }

            var savePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", file.FileName);
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", file.FileName);
        }
    }
}

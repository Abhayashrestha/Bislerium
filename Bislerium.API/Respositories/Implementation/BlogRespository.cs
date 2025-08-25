using Bislerium.API.Data;
using Bislerium.API.Model.Domains;
using Bislerium.API.Model.Dto;
using Bislerium.API.Respositories.Respository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Bislerium.API.Respositories.Implementation
{
    public class BlogRespository : IBlogRepository
    {
        private readonly AuthDbcontext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public BlogRespository(AuthDbcontext context, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }
/*
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
*/


        public async Task<Blogs> CreateBlogAsync(CreateBlogDto blogDto, string userId)
        {
            var blog = new Blogs
            {
                Id = Guid.NewGuid(),
                AuthorId = userId,
                Title = blogDto.Title,
                Body = blogDto.Body,
                ImageUrl = blogDto.ImagePath,
                PostedDate = DateTime.UtcNow
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<bool> DeleteBlogAsync(Guid blogId, string userId)
        {
            var blog = await _context.Blogs.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == blogId);
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Blogs>> GetAllBlogsAsync()
        {
            return await _context.Blogs.ToListAsync();
        }

        public async Task<Blogs> GetBlogByIdAsync(Guid blogId)
        {
            return await _context.Blogs.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == blogId);
        }


        public async Task<Blogs> UpdateBlogAsync(UpdateBlogDto blogDto, IFormFile imageFile, Guid userId)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == blogDto.Id && b.AuthorId == userId.ToString());
            if (blog == null)
                return null;

            if (imageFile != null)
            {
                if (imageFile.Length > 3 * 1024 * 1024)  // 3MB size limit
                {
                    throw new Exception("Image file size must not exceed 3 MB.");
                }

                var imagePath = await _fileService.SaveFileAsync(imageFile);
                blog.ImageUrl = imagePath;
            }

            // Update other fields
            blog.Title = blogDto.Title ?? blog.Title;
            blog.Body = blogDto.Body ?? blog.Body;
            blog.UpdatedOn = DateTime.UtcNow; // Update the timestamp

            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();

            return blog;
        }
    }
}

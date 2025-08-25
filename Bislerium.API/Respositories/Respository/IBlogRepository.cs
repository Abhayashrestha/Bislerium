using Bislerium.API.Model.Domains;
using Bislerium.API.Model.Dto;

namespace Bislerium.API.Respositories.Respository
{
    public interface IBlogRepository
    {
        Task<Blogs> CreateBlogAsync(CreateBlogDto blogDto, String userId);
        Task<Blogs> UpdateBlogAsync(UpdateBlogDto blogDto, IFormFile imageFile, Guid userId);
        Task<bool> DeleteBlogAsync(Guid blogId, String userId);
        Task<Blogs> GetBlogByIdAsync(Guid blogId);
        Task<IEnumerable<Blogs>> GetAllBlogsAsync();
    }
}

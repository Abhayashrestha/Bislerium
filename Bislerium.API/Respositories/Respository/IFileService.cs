namespace Bislerium.API.Respositories.Respository
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}

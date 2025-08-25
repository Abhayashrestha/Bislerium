using Bislerium.API.Model.Domains;

namespace Bislerium.API.Respositories.Respository
{
    public interface IReactionRespository
    {
        Task<bool> AddReactionAsync(Guid blogId, string userId, ReactionType type);
        Task<bool> RemoveReactionAsync(Guid blogId, string userId, ReactionType type);

    }
}

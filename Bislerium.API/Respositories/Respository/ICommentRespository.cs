using Bislerium.API.Model.Domains;
using Bislerium.API.Model.Dto;

namespace Bislerium.API.Respositories.Respository
{
    public interface ICommentRespository
    {
        Task<Comment> AddComment(Guid blogId, string id, string content);
        Task<bool> UpdateComment(Guid commentId, string id, string content);
        Task<bool> DeleteComment(Guid commentId, string id);
        Task<CommentReaction> AddReaction(Guid commentId, string id, ReactionType type);
        Task<bool> RemoveReaction(Guid reactionId, string id);
        Task<IEnumerable<CommentDisplayDto>> GetCommentsByBlogId(Guid blogId);
        Task<Comment> GetCommentById(Guid commentId);
    }
}

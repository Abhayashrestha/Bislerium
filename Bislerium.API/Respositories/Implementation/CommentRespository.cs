using Bislerium.API.Data;
using Bislerium.API.Model.Domains;
using Bislerium.API.Model.Dto;
using Bislerium.API.Respositories.Respository;
using Microsoft.EntityFrameworkCore;

namespace Bislerium.API.Respositories.Implementation
{
    public class CommentRespository : ICommentRespository
    {
        private readonly AuthDbcontext _context;

        public CommentRespository(AuthDbcontext context)
        {
            _context = context;
        }

        public async Task<Comment> AddComment(Guid blogId, string id, string content)
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                BlogId = blogId,
                AuthorId = id,
                Content = content,
                PostedOn = DateTime.UtcNow,
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<CommentReaction> AddReaction(Guid commentId, string id, ReactionType type)
        {
            var existingReaction = await _context.CommentReactions
       .FirstOrDefaultAsync(r => r.CommentId == commentId && r.AuthorId == id && r.Type == type);

            if (existingReaction != null)
            {
                return null;  // or handle as needed (e.g., toggle reaction)
            }

            var reaction = new CommentReaction
            {
                Id = Guid.NewGuid(),
                CommentId = commentId,
                AuthorId = id,
                Type = type,
                CreatedOn = DateTime.UtcNow,
            };
            _context.CommentReactions.Add(reaction);
            await _context.SaveChangesAsync();
            return reaction;
        }

        public async Task<bool> DeleteComment(Guid commentId, string id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.AuthorId == id);
            if (comment == null)
                return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Comment> GetCommentById(Guid commentId)
        {
            return await _context.Comments
           .Include(c => c.CommentReactions)
           .FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<IEnumerable<CommentDisplayDto>> GetCommentsByBlogId(Guid blogId)
        {
            return await _context.Comments
            .Where(c => c.BlogId == blogId)
            .Select(c => new CommentDisplayDto
            {
                Id = c.Id,
                AuthorId = c.AuthorId,
                Content = c.Content,
                PostedOn = c.PostedOn,
                Upvotes = c.CommentReactions.Count(cr => cr.Type == ReactionType.Upvote),
                Downvotes = c.CommentReactions.Count(cr => cr.Type == ReactionType.Downvote)
            })
            .ToListAsync();
        }

        public async Task<bool> RemoveReaction(Guid reactionId, string id)
        {
            var reaction = await _context.CommentReactions.FirstOrDefaultAsync(r => r.Id == reactionId && r.AuthorId == id);
            if (reaction == null)
                return false;

            _context.CommentReactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateComment(Guid commentId, string id, string content)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.AuthorId == id);
            if (comment == null)
                return false;

            comment.Content = content;
            comment.PostedOn = DateTime.UtcNow;  // Assuming you want to update the timestamp
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

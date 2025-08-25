using Bislerium.API.Data;
using Bislerium.API.Model.Domains;
using Bislerium.API.Respositories.Respository;
using Microsoft.EntityFrameworkCore;

namespace Bislerium.API.Respositories.Implementation
{
    public class ReactionRespository: IReactionRespository
    {
        private readonly AuthDbcontext _context;

        public ReactionRespository(AuthDbcontext context)
        {
            _context = context;
        }

        public async Task<bool> AddReactionAsync(Guid blogId, string userId, ReactionType type)
        {
            // Check if the reaction already exists
            var existingReaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.BlogId == blogId && r.AuthorId == userId);

            if (existingReaction != null)
            {
                if (existingReaction.Type == type)
                {
                   
                    return false;
                }
                existingReaction.Type = type;
                _context.Update(existingReaction);
            }
            else
            {
                // Create a new reaction
                var reaction = new Reaction
                {
                    Id = Guid.NewGuid(),
                    BlogId = blogId,
                    AuthorId = userId,
                    Type = type,
                    CreatedOn = DateTime.UtcNow
                };
                await _context.Reactions.AddAsync(reaction);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveReactionAsync(Guid blogId, string userId, ReactionType type)
        {
            // Find the reaction
            var reaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.BlogId == blogId && r.AuthorId == userId && r.Type == type);

            if (reaction == null)
            {
                // No such reaction exists
                return false;
            }

            // Remove the reaction
            _context.Reactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

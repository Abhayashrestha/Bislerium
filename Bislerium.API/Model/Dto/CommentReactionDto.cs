using Bislerium.API.Model.Domains;

namespace Bislerium.API.Model.Dto
{
    public class CommentReactionDto
    {
        public Guid CommentId { get; set; }
        public ReactionType Type { get; set; }
    }
}

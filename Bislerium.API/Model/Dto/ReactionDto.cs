using Bislerium.API.Model.Domains;

namespace Bislerium.API.Model.Dto
{
    public class ReactionDto
    {
        public Guid BlogId { get; set; }
        public ReactionType Type { get; set; }
    }
}

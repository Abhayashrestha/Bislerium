using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bislerium.API.Model.Domains
{
    public class CommentReaction
    {
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }
        public virtual Comment Comment { get; set; }
        [Required]
        [ForeignKey("Author")]
        public string AuthorId { get; set; }
        public IdentityUser Author { get; set; }
        public ReactionType Type { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

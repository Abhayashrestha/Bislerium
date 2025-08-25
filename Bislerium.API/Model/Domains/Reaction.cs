using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Bislerium.API.Model.Domains
{
    public class Reaction
    {
        public Guid Id { get; set; }
        public Guid BlogId { get; set; }
        public virtual Blogs Blog { get; set; }
        [Required]
        [ForeignKey("Author")]
        public string AuthorId { get; set; }
        public IdentityUser Author { get; set; }
        public ReactionType Type { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

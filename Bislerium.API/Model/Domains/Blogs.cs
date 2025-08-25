using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Bislerium.API.Model.Domains
{
    public class Blogs
    {
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Author")]
        public string AuthorId { get; set; }
        public String? Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime UpdatedOn { get; set;}
        public IdentityUser Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Reaction> Reactions { get; set; }
    }
}

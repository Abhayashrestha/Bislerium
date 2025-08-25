using System.ComponentModel.DataAnnotations;

namespace Bislerium.API.Model.Dto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

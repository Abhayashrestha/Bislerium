namespace Bislerium.API.Model.Dto
{
    public class UpdateBlogDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public IFormFile imageUrl { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}

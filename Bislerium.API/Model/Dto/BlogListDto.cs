namespace Bislerium.API.Model.Dto
{
    public class BlogListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; } // URL to access the image
        public DateTime PostedDate { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}

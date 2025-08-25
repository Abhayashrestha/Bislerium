namespace Bislerium.API.Model.Dto
{
    public class BlogDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public IFormFile imagefile { get; set; }
        public DateTime PostedDate { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public int CommentCount { get; set; }
        public double Popularity { get; set; }
    }
}

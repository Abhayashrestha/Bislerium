namespace Bislerium.API.Model.Dto
{
    public class CommentDisplayDto
    {
        public Guid Id { get; set; }
        public string AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime PostedOn { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
    }
}

namespace Bislerium.API.Model.Dto
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid BlogId { get; set; }
        public string AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime PostedOn { get; set; }
    }
}

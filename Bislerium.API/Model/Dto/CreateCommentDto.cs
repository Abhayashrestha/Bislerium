namespace Bislerium.API.Model.Dto
{
    public class CreateCommentDto
    {
        public Guid BlogId { get; set; }
        public string Content { get; set; }
    }
}

namespace Bislerium.API.Model.Dto
{
    public class DashBoardData
    {
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
        public int TotalUpvotes { get; set; }
        public int TotalDownvotes { get; set; }
        public List<BlogPostSummary> TopPosts { get; set; }
        public List<BloggerSummary> TopBloggers { get; set; }
    }
    public class BlogPostSummary
    {
        public string PostId { get; set; }
        public string Title { get; set; }
        public int Views { get; set; }
        public int ReactionCount { get; set; }
        public double Popularity { get; set; }
    }

    public class BloggerSummary
    {
        public string BloggerId { get; set; }
        public string BloggerName { get; set; }
        public int TotalPosts { get; set; }
        public int TotalReactions { get; set; }
    }
}

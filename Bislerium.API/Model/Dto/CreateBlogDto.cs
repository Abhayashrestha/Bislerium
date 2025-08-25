namespace Bislerium.API.Model.Dto
{
    public class CreateBlogDto
    {

        public string Title { get; set; }
        public string Body { get; set; }
        public string ImagePath { get; set; }
        public DateTime PostedDate { get; set; }
    }
}

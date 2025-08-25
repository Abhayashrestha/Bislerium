using Bislerium.API.Data;
using Bislerium.API.Model.Domains;
using Bislerium.API.Model.Dto;
using Bislerium.API.Respositories.Implementation;
using Bislerium.API.Respositories.Respository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bislerium.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : Controller
    {
        private readonly IBlogRepository blogRespository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly AuthDbcontext _context;
        private readonly IFileService _fileService;
        public BlogController(IBlogRepository blogRespository, UserManager<IdentityUser> userManager, AuthDbcontext context, IFileService fileService)
        {
            this.blogRespository = blogRespository;
            this.userManager = userManager;
            this._context = context;
            _fileService = fileService;
        }

        //For Getting Blof by Id
        [HttpGet("{blogId}")]
        [Authorize(Roles = "Blogger,Admin")]
        public async Task<ActionResult<Blogs>> GetBlogById(Guid blogId)
        {
            var blog = await blogRespository.GetBlogByIdAsync(blogId);
            if (blog == null)
            {
                return NotFound();
            }
            return blog;
        }

        [HttpPost("create/{userid}")]
        public async Task<IActionResult> CreateBlog([FromForm] CreateBlogDto blogDto, [FromForm] IFormFile imageFile, string userid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await userManager.FindByIdAsync(userid);
                if (user == null)
                {
                    return Unauthorized("User is not recognized.");
                }

                // Save the image file if it is not null
                if (imageFile != null)
                {
                    var imagePath = await _fileService.SaveFileAsync(imageFile); // Utilize the SaveFileAsync method shown previously
                    blogDto.ImagePath = imagePath;
                }

                // Create the blog post
                var blog = await blogRespository.CreateBlogAsync(blogDto, userid);
                return CreatedAtAction(nameof(GetBlogById), new { blogId = blog.Id }, blog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the blog: " + ex.Message);
            }
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> UpdateBlog(Guid id, [FromForm] UpdateBlogDto blogDto, [FromForm] IFormFile imageFile)
        {
            try
            {
                var userId = userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not recognized.");
                }

                var updatedBlog = await blogRespository.UpdateBlogAsync(blogDto, imageFile, Guid.Parse(userId));
                if (updatedBlog == null)
                {
                    return NotFound("Blog not found or not authorized to update this blog.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred while updating the blog: " + ex.Message);
            }
        }

        [HttpDelete("Delete/{id:guid}")]
        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> DeleteBlog(Guid id)
        {
            var userId = userManager.GetUserId(User);
            var success = await blogRespository.DeleteBlogAsync(id, userId);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet]
        [Route("List")]
        public async Task<ActionResult<IEnumerable<Blogs>>> GetAllBlogs()
        {
            var blogs = await blogRespository.GetAllBlogsAsync();
            return Ok(blogs);
        }

        [HttpGet("myblogs/{id}")]
        public async Task<IActionResult> GetMyBlogs(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var blogs = await _context.Blogs
            .Where(b => b.AuthorId == id)
            .Select(b => new BlogListDto
            {
                Id = b.Id,
                Title = b.Title,
                Body = b.Body,
                ImageUrl = b.ImageUrl, // Assuming ImageUrl is stored in the database
                PostedDate = b.PostedDate,
                UpdatedOn = b.UpdatedOn
            })
            .ToListAsync();

            if (blogs == null)
            {
                return NotFound($"No blogs found for user {user.UserName}.");
            }

            return Ok(new { User = user.UserName, Blogs = blogs });
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogs([FromQuery] int pageNumber = 1, [FromQuery] string sortBy = "recency")
        {
            const int pageSize = 10;

            IQueryable<Blogs> query = _context.Set<Blogs>().AsQueryable();

            switch (sortBy.ToLower())
            {
                case "popularity":
                    query = query.OrderByDescending(b => (2 * b.Reactions.Count(r => r.Type == ReactionType.Upvote)) -
                                                       b.Reactions.Count(r => r.Type == ReactionType.Downvote) +
                                                       b.Comments.Count);
                    break;
                case "random":
                    query = query.OrderBy(b => Guid.NewGuid());
                    break;
                case "recency":
                default:
                    query = query.OrderByDescending(b => b.PostedDate);
                    break;
            }

            var blogs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(blogs);
        }
    }
}

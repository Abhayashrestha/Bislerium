using Bislerium.API.Data;
using Bislerium.API.Model.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bislerium.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly AuthDbcontext _context;

        public DashBoardController(AuthDbcontext context)
        {
            _context = context;
        }

        [HttpGet("all-time-statistics")]
        public async Task<IActionResult> GetAllTimeStatistics()
        {
            var blogCount = await _context.Set<Blogs>().CountAsync();
            var upvoteCount = await _context.Set<Reaction>().CountAsync(r => r.Type == ReactionType.Upvote);
            var downvoteCount = await _context.Set<Reaction>().CountAsync(r => r.Type == ReactionType.Downvote);
            var commentCount = await _context.Set<Comment>().CountAsync();

            var stats = new
            {
                TotalBlogs = blogCount,
                TotalUpvotes = upvoteCount,
                TotalDownvotes = downvoteCount,
                TotalComments = commentCount
            };

            return Ok(stats);
        }

        [HttpGet("month-specific-statistics/{year}/{month}")]
        public async Task<IActionResult> GetMonthSpecificStatistics(int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var blogCount = await _context.Set<Blogs>()
                .CountAsync(b => b.PostedDate >= monthStart && b.PostedDate < monthEnd);
            var upvoteCount = await _context.Set<Reaction>()
                .CountAsync(r => r.CreatedOn >= monthStart && r.CreatedOn < monthEnd && r.Type == ReactionType.Upvote);
            var downvoteCount = await _context.Set<Reaction>()
                .CountAsync(r => r.CreatedOn >= monthStart && r.CreatedOn < monthEnd && r.Type == ReactionType.Downvote);
            var commentCount = await _context.Set<Comment>()
                .CountAsync(c => c.PostedOn >= monthStart && c.PostedOn < monthEnd);

            var stats = new
            {
                TotalBlogs = blogCount,
                TotalUpvotes = upvoteCount,
                TotalDownvotes = downvoteCount,
                TotalComments = commentCount
            };

            return Ok(stats);
        }

        [HttpGet("top-posts/all-time")]
        public async Task<IActionResult> GetTopPostsAllTime()
        {
            var posts = await _context.Set<Blogs>()
                .Select(b => new
                {
                    BlogId = b.Id,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    Popularity = 2 * b.Reactions.Count(r => r.Type == ReactionType.Upvote) -
                                 b.Reactions.Count(r => r.Type == ReactionType.Downvote) +
                                 b.Comments.Count
                })
                .OrderByDescending(b => b.Popularity)
                .Take(10)
                .ToListAsync();

            return Ok(posts);
        }

        // Get top 10 most popular blog posts for a specific month
        [HttpGet("top-posts/month-specific/{year}/{month}")]
        public async Task<IActionResult> GetTopPostsMonthSpecific(int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var posts = await _context.Set<Blogs>()
                .Where(b => b.PostedDate >= monthStart && b.PostedDate < monthEnd)
                .Select(b => new
                {
                    BlogId = b.Id,
                    Title = b.Title,
                    AuthorId = b.AuthorId,
                    Popularity = 2 * b.Reactions.Where(r => r.CreatedOn >= monthStart && r.CreatedOn < monthEnd && r.Type == ReactionType.Upvote).Count() -
                                 b.Reactions.Where(r => r.CreatedOn >= monthStart && r.CreatedOn < monthEnd && r.Type == ReactionType.Downvote).Count() +
                                 b.Comments.Where(c => c.PostedOn >= monthStart && c.PostedOn < monthEnd).Count()
                })
                .OrderByDescending(b => b.Popularity)
                .Take(10)
                .ToListAsync();

            return Ok(posts);
        }

        // Get top 10 bloggers based on cumulative popularity of their blog posts all-time
        [HttpGet("top-bloggers/all-time")]
        public async Task<IActionResult> GetTopBloggersAllTime()
        {
            var bloggers = await _context.Set<Blogs>()
                .GroupBy(b => b.AuthorId)
                .Select(group => new
                {
                    AuthorId = group.Key,
                    TotalPopularity = group.Sum(g => 2 * g.Reactions.Count(r => r.Type == ReactionType.Upvote) -
                                                    g.Reactions.Count(r => r.Type == ReactionType.Downvote) +
                                                    g.Comments.Count)
                })
                .OrderByDescending(b => b.TotalPopularity)
                .Take(10)
                .ToListAsync();

            return Ok(bloggers);
        }

        // Get top 10 bloggers based on cumulative popularity of their blog posts for a specific month
        [HttpGet("top-bloggers/month-specific/{year}/{month}")]
        public async Task<IActionResult> GetTopBloggersMonthSpecific(int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var bloggers = await _context.Set<Blogs>()
                .Where(b => b.PostedDate >= monthStart && b.PostedDate < monthEnd)
                .GroupBy(b => b.AuthorId)
                .Select(group => new
                {
                    AuthorId = group.Key,
                    TotalPopularity = group.Sum(g => 2 * g.Reactions.Where(r => r.CreatedOn >= monthStart && r.CreatedOn < monthEnd && r.Type == ReactionType.Upvote).Count() -
                                                    g.Reactions.Where(r => r.CreatedOn >= monthStart && r.CreatedOn < monthEnd && r.Type == ReactionType.Downvote).Count() +
                                                    g.Comments.Where(c => c.PostedOn >= monthStart && c.PostedOn < monthEnd).Count())
                })
                .OrderByDescending(b => b.TotalPopularity)
                .Take(10)
                .ToListAsync();

            return Ok(bloggers);
        }
    }
}

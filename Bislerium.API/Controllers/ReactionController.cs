using Bislerium.API.Model.Dto;
using Bislerium.API.Respositories.Respository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionRespository _reactionService;
        private readonly UserManager<IdentityUser> _userManager;

        public ReactionController(IReactionRespository reactionService, UserManager<IdentityUser> userManager)
        {
            _reactionService = reactionService;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> AddReaction([FromBody] ReactionDto reactionDto)
        {
            var userId = _userManager.GetUserId(User);  // Get user ID from the authenticated user's Identity
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User must be logged in.");
            }

            var result = await _reactionService.AddReactionAsync(reactionDto.BlogId, userId, reactionDto.Type);
            if (!result)
            {
                return BadRequest("Unable to add reaction or duplicate reaction detected.");
            }
            return Ok("Reaction added successfully.");
        }

        [HttpPost]
        [Route("Remove")]
        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> RemoveReaction([FromBody] ReactionDto reactionDto)
        {
            var userId = _userManager.GetUserId(User);  // Get user ID from the authenticated user's Identity
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User must be logged in.");
            }

            var result = await _reactionService.RemoveReactionAsync(reactionDto.BlogId, userId, reactionDto.Type);
            if (!result)
            {
                return BadRequest("Unable to remove reaction or reaction not found.");
            }
            return Ok("Reaction removed successfully.");
        }
    }
}

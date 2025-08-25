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
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRespository _commentRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentsController(ICommentRespository commentRepository, UserManager<IdentityUser> userManager)
        {
            _commentRepository = commentRepository;
            _userManager = userManager;
        }

        [HttpPost("create/{blogId}/{userId}")]
        [Authorize]
        public async Task<IActionResult> CreateComment(Guid blogId, string userId, [FromBody] CommentDto commentDto)
        {
            // Assuming user validation is handled here if needed
            var comment = await _commentRepository.AddComment(blogId, userId, commentDto.Content);
            if (comment == null)
            {
                return BadRequest("Failed to create comment.");
            }
            return CreatedAtAction(nameof(GetComment), new { commentId = comment.Id }, comment);
        }

        [HttpGet("{commentId}")]
        public async Task<IActionResult> GetComment(Guid commentId)
        {
            var comment = await _commentRepository.GetCommentById(commentId);
            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            return Ok(comment);
        }

        [HttpPut("update/{commentId}/{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(Guid commentId, string userId, [FromBody] CommentDto commentDto)
        {
            var updated = await _commentRepository.UpdateComment(commentId, userId, commentDto.Content);
            if (!updated)
            {
                return NotFound("Failed to update comment or not authorized.");
            }
            return NoContent();
        }

        [HttpDelete("delete/{commentId}/{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid commentId, string userId)
        {
            var deleted = await _commentRepository.DeleteComment(commentId, userId);
            if (!deleted)
            {
                return NotFound("Failed to delete comment or not authorized.");
            }
            return NoContent();
        }

        [HttpPost("react/{commentId}/{userId}")]
        [Authorize]
        public async Task<IActionResult> AddReaction(Guid commentId, string userId, [FromBody] CommentReactionDto reactionDto)
        {
            var reaction = await _commentRepository.AddReaction(commentId, userId, reactionDto.Type);
            if (reaction == null)
            {
                return BadRequest("Reaction already exists or failed to add.");
            }
            return Ok(reaction);
        }

        [HttpDelete("unreact/{reactionId}/{userId}")]
        [Authorize]
        public async Task<IActionResult> RemoveReaction(Guid reactionId, string userId)
        {
            var removed = await _commentRepository.RemoveReaction(reactionId, userId);
            if (!removed)
            {
                return NotFound("Failed to remove reaction or not authorized.");
            }
            return NoContent();
        }
    }
}

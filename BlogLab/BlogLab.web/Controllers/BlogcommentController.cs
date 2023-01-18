using BlogLab.Models.BlogComment;
using BlogLab.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BlogLab.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogcommentController : ControllerBase
    {

        private readonly IBlogCommentRepository _blogCommentRepository;

        public BlogcommentController(IBlogCommentRepository blogCommentRepository)
        {
            blogCommentRepository = _blogCommentRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BlogComment>> Create(BlogCommentCreate blogCommetCreate)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);
            var createdBlogComment = await _blogCommentRepository.UpsertAsync(blogCommetCreate, applicationUserId);
            return Ok(createdBlogComment);
        }

        [HttpGet("{blogId}")]
        public async Task<ActionResult<List<BlogComment>>> GetAll(int blogId)
        {
            var blogComments = await _blogCommentRepository.GetAllAsync(blogId);
            return Ok(blogComments);
        }

        [Authorize]
        [HttpDelete("{blogCommentId}")]
        public async Task<ActionResult<int>> Delete(int blogCommentId)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);
            var foundBlogComment = await _blogCommentRepository.GetAsync(blogCommentId);
            if (blogCommentId == null) return BadRequest("Comment does not exits");
            if (foundBlogComment.ApplicationUserId == applicationUserId)
            {
                var affectedRows = await _blogCommentRepository.DeleteAsync(blogCommentId);
                return Ok(affectedRows);
            }
            else
            {
                return BadRequest("This comment was not created by the current user.");
            }
        }
    }
}

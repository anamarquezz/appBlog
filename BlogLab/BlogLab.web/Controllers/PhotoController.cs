using BlogLab.Models.Photo;
using BlogLab.Repository;
using BlogLab.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BlogLab.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoRepository _photoRespository;
        private readonly IBlogRepository _blogRepository;
        private readonly IPhotoService _photoService;

        public PhotoController(
            IPhotoRepository photoRepository,
            IBlogRepository blogRepository,
            IPhotoService photoService
        )
        {
            _photoRespository = photoRepository;
            _blogRepository = blogRepository;
            photoService = _photoService;
        }

        //http://localhost:5000/api/Photo [Photo] token
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Photo>> UploadPhoto(IFormFile file)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);
            var uploadResult = await _photoService.AddPhotoAsync(file);

            if (uploadResult.Error != null) return BadRequest(uploadResult.Error.Message);

            var photoCreate = new PhotoCreate
            {
                PublicId = uploadResult.PublicId,
                ImageUrl = uploadResult.SecureUri.AbsoluteUri,
                Description = file.FileName
            };

            var photo = await _photoRespository.InsertAsync(photoCreate, applicationUserId);

            return Ok(photo);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Photo>>> GetByApplicationUserId()
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var photos = await _photoRespository.GetAllByUserIdAsync(applicationUserId);
            return Ok(photos);
        }

        [HttpGet("photoId")]
        public async Task<ActionResult<Photo>> Get(int photoId)
        {
            var photo = await _photoRespository.GetAsync(photoId);
            return Ok(photo);
        }

        [Authorize]
        [HttpDelte("{photoId}")]
        public async Task<ActionResult<int>> Delete(int photoId)
        {
            int applicationUserId = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);
            var foundPhoto = await _photoRespository.GetAsync(photoId);
            
            if (foundPhoto != null)
            {
                if (foundPhoto.ApplicationUserId == applicationUserId)
                {
                    var blogs = await _blogRepository.GetAllByUserIdAsync(applicationUserId);
                    var usedInBlog = blogs.Any(b => b.PhotoId == photoId);

                    if (usedInBlog) return BadRequest("Cannot remove photo as it is beign used in published blog(s)");

                    var deleteResult = await _photoService.DeletePhotoAsync(foundPhoto.PublicId);
                    if(deleteResult.Error != null) return BadRequest(deleteResult.Error.Message);
                    var affectedRows = await _photoRespository.DeleteAsync(foundPhoto.PhotoId);

                    return Ok(affectedRows);
                }
                else
                {
                    return BadRequest("Photo was not uploaded by the current user.");
                }
            }

            return BadRequest("Photo does not exist.")
        }
    }

}

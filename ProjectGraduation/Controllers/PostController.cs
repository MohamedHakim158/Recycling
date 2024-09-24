    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Repository;

namespace ProjectGraduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository postRepository;
        public PostController(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }
        
        [HttpPost("AddPost")]
        public async Task<IActionResult> AddPost([FromForm] PostModel model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
            if (!(model.Image != null && allowedExtensions.Contains(fileExtension)))
            {
                return BadRequest("Invalid file type. Please upload an image.");
            }
            var result = await postRepository.AddPost(model);
            if (result.IsSucceded)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }


        [HttpGet("GetPostwithId/{PostId}")]
        public async Task<IActionResult> GetPostById(string PostId)
        {
             var Post = await postRepository.GetPostById(PostId);
            if (Post == null)
                return BadRequest("There is no Post with id " + PostId);
            return Ok(Post);
        }


        [HttpPut("UpdatePost/{postId}")]
        public async Task<IActionResult> UpdatePost([FromForm] PostModel model , string postId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Post = await postRepository.GetPostForUpdate(postId);
            if (Post == null)
                return BadRequest("There is no Post wiht id: " + postId);
            var Process = await postRepository.UpdatePost(model, postId);
            if (Process.IsSucceded)
                return Ok(Process.Message);
            return BadRequest(Process.Message);
        }
        [HttpGet("PostWithCategory/{Category}")]
        public async Task<IActionResult> GetPostsWithCategory(string Category)
        {
            var result = await postRepository.GetPostsWithCategory(Category);
            if (result is null)
            {
                return Ok("There is no Offers yet for this Category");
            }
            return Ok(result);
        }
        [HttpGet("GetAllPosts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var result = await postRepository.GetAllPosts();
            if (result is null)
            {
                return Ok("There is no Offers yet ");
            }
            return Ok(result);
        }
        [HttpPut("AvailableOrNot")]
        public async Task<IActionResult> ConvertAvailability([FromForm] string PostId , [FromForm] bool isavailable)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await postRepository.ConvertAvailability(isavailable, PostId);
            if (result.IsSucceded)
                return Ok(result.Message);
            return BadRequest(result.Message);
        }

        [HttpDelete("DelPost/{PostId}")]
        public async Task<IActionResult> DeletePost([FromRoute] string PostId)
        {
            var Post = await postRepository.GetPostById(PostId);
            if (Post == null)
                return BadRequest("There is no Post wiht id: " + PostId);
            
            var process = await postRepository.DeletPost(PostId);
            if (process.IsSucceded)
                return Ok(process.Message);
            return BadRequest(process.Message);
        }


        [HttpGet("SearchWithTitle/{title}")]
        public async Task<IActionResult> SearchWithTitle(string title)
        {
            if (title is null)
                return BadRequest("Please enter title you want to search for");
            var strList = title.Split(' ');
            var list = new List<PostDto>();
            foreach (var item in strList)
            {
                list.AddRange(await postRepository.SearchWithTitle(title));
            }
            return Ok(list);
        }
    }
}

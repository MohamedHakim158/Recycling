using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using ProjectGraduation.Services;

namespace ProjectGraduation.Repository
{
    public interface IPostRepository
    {
        public Task<ProcessResult> AddPost(PostModel model);
        public Task<PostDto> GetPostById(string PostId);
        public Task<ProcessResult> UpdatePost(PostModel model, string PostId);
        public Task<ProcessResult> DeletPost(string PostId);
        public Task<List<PostDto>> SearchWithTitle(string title);
        public Task<Post> GetPostForUpdate(string PostId);
        public Task<List<PostDto>> GetPostsWithCategory(string Category);
        public Task<List<PostDto>> GetAllPosts();
        public Task<ProcessResult> ConvertAvailability(bool isavailable , string PostId);
    }
}

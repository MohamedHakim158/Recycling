using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using ProjectGraduation.Services;

namespace ProjectGraduation.Repository
{
    public class PostRepository:IPostRepository
    {
        private readonly GraduationProjectContext context;
        private readonly IOfferRepository repo;
        public PostRepository(GraduationProjectContext _context , IOfferRepository rep)
        {
            context = _context;
            this.repo = rep;
        }
        public async Task<ProcessResult> AddPost([FromForm] PostModel model)    
        {
            using var stream = new MemoryStream();
            await model.Image.CopyToAsync(stream);
            try
            {
                await context.Posts.AddAsync(new Post
                {
                    ClientId = model.ClientId,
                    Image = stream.ToArray(),
                    CreatedAt = DateTime.Now,
                    Description = model.Description,
                    Title = model.Title.ToUpper(),
                    ProfileId = model.ProfileId,
                    CategoryName = model.CategoryName
                });
                await context.SaveChangesAsync();
                return new ProcessResult { IsSucceded = true, Message = "Post Added Successfully" };
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;                
                else 
                    msg = ex.Message;
                return new ProcessResult { IsSucceded = false, Message = $"Error Adding Post Due to {msg}" };
            } 
           
        }
        
        public async Task<PostDto> GetPostById(string PostId)
        {
            var exist = await CheckForPost(PostId);
            if (exist)
            {
                var Post = await context.Posts.Include(p => p.Profile).Include(p=>p.Client).FirstOrDefaultAsync(p => p.Id == PostId);
                return new PostDto(Post);
            }
            return null;
        }
        public async Task<Post> GetPostForUpdate(string PostId)
        {
            var exist = await CheckForPost(PostId);
            if (exist)
            {
                var Post = await context.Posts.FirstOrDefaultAsync(p => p.Id == PostId);
                return  Post;
            }
            return null;
        }
        public async Task<ProcessResult> UpdatePost([FromForm] PostModel model , string PostId)
        {
            var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == PostId);
            using var stream = new MemoryStream();
            await model.Image.CopyToAsync(stream);
            try
            {
                post.Description = model.Description;
                post.Image = stream.ToArray();
                post.Title = model.Title.ToUpper();
                post.CreatedAt = DateTime.Now;
                var process = await repo.DeleteAllOffersOfPost(post.Id);
                if (!process.IsSucceded)
                    return process;
                await context.SaveChangesAsync();
                return new ProcessResult { IsSucceded = true, Message = "Post Updated Successfully" };
            }
            catch (Exception ex)
            {
                string msg;
                if(ex.InnerException != null)
                    msg = ex.InnerException.Message;
                msg = ex.Message;
                return new ProcessResult { IsSucceded = false , Message = " Error Updating Post Due to "+ msg };
            }

        }
        public async Task<ProcessResult> DeletPost(string PostId)
        {
            var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == PostId);
            try
            {
                var offers = await context.Offers.Where(o=>o.PostId == post.Id).ToListAsync();
                context.Offers.RemoveRange(offers);
                context.Posts.Remove(post);
                await context.SaveChangesAsync();
                return new ProcessResult { IsSucceded = true, Message = "Post Deleted Successfully" };
            }
            catch (Exception ex)
            {
                string msg;
                if(ex.InnerException != null)
                    msg = ex.InnerException.Message;
                msg = ex.Message;
                return new ProcessResult { IsSucceded = false, Message = "Error Deleting Post Due to " + msg };
            }

        }
        public async Task<List<PostDto>> SearchWithTitle(string title)
        {
            var items = title.ToUpper().Split(' ');
            var list = new List<PostDto>();
            foreach (string item in items)
            {
                if (item.Length <= 2)
                {
                    continue;
                }
                var result = await context.SearchPostWithTitle(item);
                list.AddRange(result);
            }
            list.OrderByDescending(p => p.CreatedAt);
            
            return list;
        }

        private async Task<bool> CheckForPost(string PostId)
        {
            var post = await context.Posts.FirstOrDefaultAsync(p=>p.Id == PostId);
            if(post == null) 
                return false;
            return true;
        }

        public async Task<List<PostDto>> GetPostsWithCategory(string Category)
        {
            var posts = await context.Posts.Include(p=>p.Client).Include(p=>p.Profile).Where(p => p.CategoryName == Category && p.isAvailable ==true).ToListAsync();
            var postdto = new List<PostDto>();
            foreach (var post in posts)
            {
                postdto.Add(new PostDto(post));
            }
            return postdto;
        }

        public async Task<List<PostDto>> GetAllPosts()
        {
            var posts = await context.Posts.Include(p=>p.Client).Include(p=>p.Profile).Where(p => p.isAvailable == true).ToListAsync();
            var postdto = new List<PostDto>();
            foreach (var post in posts)
            {
                postdto.Add(new PostDto(post));
            }
            return postdto;
        }
        public async Task<ProcessResult> ConvertAvailability(bool isavailable , string PostId)
        {
            var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == PostId);
            if (post is null)
            {
                return new ProcessResult { IsSucceded = false, Message = "There is no Post wiht id: " + PostId };
            }
            post.isAvailable = isavailable;
            await context.SaveChangesAsync();
            return new ProcessResult { IsSucceded = true, Message = "Succeded" };
        }
    }
}

using ProjectGraduation.Models;

namespace ProjectGraduation.DTO
{
    public class PostDto
    {
        public string Id { get; set; }
        public byte[] Image { get; set; } 
        public byte[] ForegroundImageOfClient { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public string CategoryName { get; set; }
        public bool isavailable { get; set; }
        public string ClientId { get; set; } 
        public string ProfileId { get; set; }
        public PostDto()
        {
            
        }
        public PostDto(Post Post)
        {
            Id = Post.Id;
            Image = Post.Image;
            Title = Post.Title;
            Description = Post.Description;
            ClientId = Post.ClientId;
            ProfileId = Post.ProfileId;
            CreatedAt = Post.CreatedAt;
            ForegroundImageOfClient = Post.Profile.ForegroundImage;
            UserName = Post.Client.UserName;
            FullName = Post.Client.Fname + " " + Post.Client.Lname;
            CategoryName = Post.CategoryName;
            isavailable = Post.isAvailable;
        }
        public PostDto(IQueryable<PostDto> queryable)
        {
              
        }
    }
}

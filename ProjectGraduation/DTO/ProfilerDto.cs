using ProjectGraduation.Models;

namespace ProjectGraduation.DTO
{
    public class ProfilerDto
    {
        public string Id { get; }

        public string? HintAboutMe { get;  }

        public byte[]? ForegroundImage { get;  }

        public byte[]? BackgroundImage { get;  }
        public string? Profession { get; set; }
        public int? Age { get;  }

        public string? Address { get;  }

        public string ClientId { get; }
        public List<PostDto> Posts { get; } = new List<PostDto>();

        public ProfilerDto(Profile profile, List<Post> postlist)
        {
            Id = profile.Id;
            Address = profile.Address;
            ForegroundImage = profile.ForegroundImage;
            BackgroundImage = profile.BackgroundImage;
            Age = profile.Age;
            ClientId = profile.ClientId;
            HintAboutMe = profile.HintAboutMe;
            Profession = profile.Profession;
            foreach (var post in postlist)
            {
                Posts.Add(new PostDto
                {
                    Id = post.Id,
                    ClientId = post.ClientId,
                    ForegroundImageOfClient = ForegroundImage,
                    ProfileId = Id,
                    CreatedAt = post.CreatedAt,
                    Description = post.Description,
                    FullName = post.Client.Fname + " " + post.Client.Lname,
                    Image = post.Image,
                    Title = post.Title,
                    UserName = post.Client.UserName
                });
            }

        }
    }
}

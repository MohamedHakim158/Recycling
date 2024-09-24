using Microsoft.AspNetCore.Identity;

namespace ProjectGraduation.Models
{
    public class Client:IdentityUser
    {
        public string Fname { get; set; } = null!;

        public string Lname { get; set; } = null!;

        public string? ConfirmEmailCode { get; set; }

        public string? ResetPasswordCode { get; set; }

        public string RegisteredAs { get; set; } = null!;
        public virtual List<Post> Posts { get; set; }
        public virtual List<Offer> Offers { get; set; }
        public virtual Profile Profile { get; set; }
    }
}

using System.IdentityModel.Tokens.Jwt;

namespace ProjectGraduation.DTO
{
    public class AuthDto
    {
        public AuthDto()
        {
            Roles = new List<string>();
        }
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
        public string Token { get; set; } 
        public string Email { get; set; }
        public string UserId { get; set; }
        public DateTime ExpireOn { get; set; }
        public List<string> Roles { get; set; }
        public string ProfileId { get; set; }
    }
}

using ProjectGraduation.Models;

namespace ProjectGraduation.DTO
{
    public class UserDto
    {
        public string ProfileId { get; set; }
        public string Fname { get; set; } 
        public string Lname { get; set; } 
        public string UserName { get; set; }
        public byte[] ForegrounImage { get; set; }
        
    }
}

using System.ComponentModel.DataAnnotations;

namespace ProjectGraduation.Helping_Models
{
    public class LoginRequest
    {
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "This is not Email Address")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
       public string Password { get; set; }
    }
}

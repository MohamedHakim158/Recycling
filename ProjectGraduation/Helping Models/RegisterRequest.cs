using System.ComponentModel.DataAnnotations;

namespace ProjectGraduation.Helping_Models
{
    public class RegisterRequest
    {

        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage ="This is not Email Address")]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        [DataType(DataType.Password,ErrorMessage ="at least 8 Character Length and has (UpperCase , LowerCase, digits , at least on special character such as'@$#^' ")]
        [MinLength(8)]
        public string Password { get; set; }
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        public string RegisteredAs { get; set; }
        public string? Profession { get; set; }
  
    }
}

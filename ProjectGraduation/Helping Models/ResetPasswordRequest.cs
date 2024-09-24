using System.ComponentModel.DataAnnotations;

namespace ProjectGraduation.Helping_Models
{
    public class ResetPasswordRequest
    {
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "This is not Email Address")]
        public string Email {  get; set; }
        public string Code { get; set; }
        [DataType(DataType.Password, ErrorMessage = "at least 8 Character Length and has (UpperCase , LowerCase, digits , at least on special character such as'@$#^' ")]
        [MinLength(8)]
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}

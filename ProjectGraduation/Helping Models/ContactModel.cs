using System.ComponentModel.DataAnnotations;

namespace ProjectGraduation.Helping_Models
{
    public class ContactModel
    {
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public string LinkedInAccount { get; set; }
        public string FacebookAccount { get; set; }
        public string Specialty { get; set; }
        public IFormFile Image { get; set; }
    }
}

using ProjectGraduation.Models;

namespace ProjectGraduation.DTO
{
    public class ContactDto
    {
        public string Id { get; }
        public string Name { get;  }
        public string? Email { get; }
        public string? LinkedInAccount { get;  }
        public string? FacebookAccount { get;  }
        public string Specialty { get;  }
        public byte[] Image { get;  }
        public ContactDto(Contact contact)
        {
            Id = contact.Id; 
            Name = contact.Name; 
            Email = contact.Email; 
            LinkedInAccount = contact.LinkedInAccount; 
            FacebookAccount = contact.FacebookAccount; 
            Specialty = contact.Specialty;
            Image = contact.Image;
        }
    }
}

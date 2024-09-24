using ProjectGraduation.Models;

namespace ProjectGraduation.DTO
{
    public class OfferDto
    {
        public string Id { get; }

        public string Description { get; }

        public DateTime AddedAt { get;  }

        public string ClientId { get;  }

        public string PostId { get;  }
        public string Email { get;  }
        public string PhoneNumber { get; }
        public OfferDto()
        {
            
        }
        public OfferDto(Offer offer)
        {
            Id = offer.Id; 
            Description = offer.Description;
            ClientId = offer.ClientId;
            PostId = offer.PostId;
            Email = offer.Client.Email;
            PhoneNumber = offer.Client.PhoneNumber;
            AddedAt = offer.AddedAt;
        }
    }
}

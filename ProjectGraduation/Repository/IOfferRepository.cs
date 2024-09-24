using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using ProjectGraduation.Services;

namespace ProjectGraduation.Repository
{
    public interface IOfferRepository
    {
        public Task<ProcessResult> AddOffer(OfferModel model);
        public Task<ProcessResult> UpdateOffer(OfferModel model, string OfferId);
        public Task<ProcessResult> DeleteOffer(string OfferId);
        public Task<Offer> GetOfferById(string OfferId);
        public Task<ProcessResult> DeleteAllOffersOfPost(string PostId);
        public Task<List<Offer>> GetAllOffersOfPost(string PostId);
        public Task<ProcessResult> AcceptOffer(string OfferId);
    }
}

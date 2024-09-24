using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using ProjectGraduation.Repository;
using System.Security.Claims;

namespace ProjectGraduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OfferController : ControllerBase
    {
        private readonly IOfferRepository offerRepository;
        private readonly GraduationProjectContext context;
        public OfferController(IOfferRepository repository , GraduationProjectContext context)
        {
            offerRepository = repository;
            this.context = context;
        }
        [HttpPost("AddOffer")]
        public async Task<IActionResult> Addoffer([FromForm]OfferModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var process = await offerRepository.AddOffer(model);
            if (process.IsSucceded)
                return Ok(process.Message);
            return BadRequest(process.Message);
        }


        [HttpDelete("DeleteOffer/{OfferId}")]
        public async Task<IActionResult> DeleteOffer(string OfferId)
        {
            var offer = await offerRepository.GetOfferById(OfferId);
            if (offer == null)
                return BadRequest("There is no Offer wiht id: " + OfferId);
            var process = await offerRepository.DeleteOffer(OfferId);
            if (process.IsSucceded)
                return Ok(process.Message);
            return BadRequest(process.Message);
        }


        [HttpPut("AcceptOffer/{OfferId}")]
        public async Task<IActionResult> AcceptOffer(string OfferId)
        {
            var offer = await context.Offers.FirstOrDefaultAsync(p => p.Id == OfferId);
            if (offer is null)
                return BadRequest("There is no Offers with id "+OfferId);
            var result =await offerRepository.AcceptOffer(OfferId);
            if(result.IsSucceded)
                return Ok(result.Message);
            return BadRequest(result.Message);
        }

        [HttpGet("GetAllOffersOfPost/{PostId}")]
        public async Task<IActionResult> GetAllOffersOfPost(string PostId)
        {
            var offers = await offerRepository.GetAllOffersOfPost(PostId);
            if (offers == null)
                return Ok("There is no offers yet on this post");
            var offerDto = new List<OfferDto>();
            foreach (var offer in offers)
            {
                offerDto.Add(new OfferDto(offer));
            }
            return Ok(offerDto);

        }


        [HttpPut("UpdateOffer/{OfferId}")]
        public async Task<IActionResult> UpdateOffer([FromForm] OfferModel model , string OfferId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var offer = await offerRepository.GetOfferById(OfferId);
            if (offer == null)
                return BadRequest("There is no Offer wiht id: " + OfferId);
            var process = await offerRepository.UpdateOffer(model, OfferId);
            if (process.IsSucceded)
                return Ok(process.Message);
            return BadRequest(process.Message);
        }
        [HttpGet("GetOfferWithId/{OfferId}")]
        public async Task<IActionResult> GetOfferById(string OfferId)
        {
            var result = await offerRepository.GetOfferById(OfferId);
            if (result == null)
                return BadRequest("There is no offer wiht id: " + OfferId);
            return Ok(new OfferDto(result));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using ProjectGraduation.Repository;
using System.Security.Claims;

namespace ProjectGraduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository profileRepository;
        public ProfileController(IProfileRepository profileRepository)
        {
            this.profileRepository = profileRepository;
        }

        [HttpGet("ProfileData/{ProfileId}")]
        public async Task<IActionResult> GetProfileData(string ProfileId)
        {
            var Data = await profileRepository.UploadData(ProfileId);
            if (Data is null)
                return BadRequest("There is no Profile with id " + ProfileId);
            return Ok(Data);
            }


        [HttpPut("EditProfileData/{ProfileId}")]
        public async Task<IActionResult> UpdateProfilData([FromForm]ProfileModel model, string ProfileId)
        {
            var profile = await profileRepository.GetProfile(ProfileId);
            if (profile == null)
                return BadRequest("There is no Offer wiht id: " + ProfileId);
            
            var process = await profileRepository.UpdateProfileData(model,ProfileId);
            if (process.IsSucceded)
                return Ok(process.Message);
            return BadRequest(process.Message);
        }


        [HttpPut("EditForegroundImage/{ProfileId}")]
        public async Task<IActionResult> AddOrUpdateForegroundImage([FromForm] IFormFile Image ,[FromRoute] string ProfileId)
        {
            var profile = await profileRepository.GetProfile(ProfileId);
            if (profile == null)
                return BadRequest("There is no Offer wiht id: " + ProfileId);
            var process = await profileRepository.AddOrUpdatForeGroundImage(Image, ProfileId);
            if (process.IsSucceded)
                return Ok(process.Message);
            return BadRequest(process.Message);
        }
        [HttpPut("EditBackgroundImage/{ProfileId}")]
        public async Task<IActionResult> AddOrUpdateBackgroundImage([FromForm] IFormFile Image, [FromRoute] string ProfileId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var profile = await profileRepository.GetProfile(ProfileId);
            if (profile == null)
                return BadRequest("There is no Offer wiht id: " + ProfileId);
            var process = await profileRepository.AddOrUpdatBackGroundImage(Image, ProfileId);
            if (process.IsSucceded)
                return Ok(process.Message);
            return BadRequest(process.Message);
        }
        [HttpDelete("DeleteForegroundImage/{ProfileId}")]
        public async Task<IActionResult> DeleteForegroundImage(string ProfileId)
        {
            var profile = await profileRepository.GetProfile(ProfileId);
            if (profile == null)
                return BadRequest("There is no Offer wiht id: " + ProfileId);
            
            var process = await profileRepository.DeleteForegroundImage(ProfileId);
            if (process.IsSucceded)
                return Ok(process.Message);
            return BadRequest(process.Message);
        }
        [HttpDelete("DeleteBackgroundImage/{ProfileId}")]
        public async Task<IActionResult> DeleteBackgroundImage(string ProfileId)
        {
            var profile = await profileRepository.GetProfile(ProfileId);
            if (profile == null)
                return BadRequest("There is no Offer wiht id: " + ProfileId);
            var process = await profileRepository.DeleteForegroundImage
                (ProfileId);
            if (process.IsSucceded)
                return Ok(process.Message);
            return BadRequest(process.Message);
        }
    }
}

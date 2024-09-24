using Microsoft.AspNetCore.Mvc;
using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using ProjectGraduation.Services;

namespace ProjectGraduation.Repository
{
    public interface IProfileRepository
    {
        public Task<ProfilerDto> UploadData(string ProfileId);
        public Task<ProcessResult> UpdateProfileData(ProfileModel model, string profileId);
        public Task<ProcessResult> AddOrUpdatForeGroundImage([FromForm] IFormFile Image, string ProfileId);
        public Task<ProcessResult> AddOrUpdatBackGroundImage([FromForm] IFormFile Image, string ProfileId);
        public Task<ProcessResult> DeleteForegroundImage(string ProfileId);
        public Task<ProcessResult> DeleteBackgroundImage(string ProfileId);
        public Task<Profile> GetProfile(string ProfileId);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using ProjectGraduation.Services;

namespace ProjectGraduation.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly GraduationProjectContext context;
        public ProfileRepository(GraduationProjectContext context)
        {
            this.context = context;
        }
        public async Task<ProfilerDto> UploadData(string ProfileId)
        {
            var exist = await CheckProfile(ProfileId);
            if (exist)
            {
                var Profile = await context.Profiles.FirstOrDefaultAsync(p => p.Id == ProfileId);
                var Posts = await context.Posts.Include(p=>p.Client).OrderByDescending(p => p.CreatedAt).Where(p => p.ClientId == Profile.ClientId).ToListAsync();
                var ProfileData = new ProfilerDto(Profile, Posts);
                return ProfileData;
            }
            return null;
        }
        public async Task<ProcessResult> UpdateProfileData(ProfileModel model, string profileId)
        {
            var exist = await CheckProfile(profileId);
            if (!exist)
                return new ProcessResult { IsSucceded = false, Message = "There is no Profile with id " + profileId };
            var prof = await context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
            prof.HintAboutMe = model.HintAboutMe;
            prof.Address = model.Address;
            prof.Age = model.Age;
            prof.Profession = model.Profession;
            try
            {
                await context.SaveChangesAsync();
                return new ProcessResult { IsSucceded = true, Message = "Data Updated Sccessfully" };
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return new ProcessResult { IsSucceded = false, Message = "ُError Updating Data due to " + msg };
            }
        }
        public async Task<ProcessResult> AddOrUpdatForeGroundImage([FromForm] IFormFile Image, string ProfileId)
        {
            var exist = await CheckProfile(ProfileId);
            if (!exist)
                return new ProcessResult { IsSucceded = false, Message = "There is no Profile with id " + ProfileId };
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(Image.FileName).ToLowerInvariant();
            if (!(Image != null && allowedExtensions.Contains(fileExtension)))
            {
                return new ProcessResult { Message = "Invalid file type. Please upload an image.", IsSucceded = false };
            }
            var profile = await context.Profiles.FirstOrDefaultAsync(p => p.Id == ProfileId);
            try
            {
                using var stream = new MemoryStream();
                await Image.CopyToAsync(stream);
                profile.ForegroundImage = stream.ToArray();
                await context.SaveChangesAsync();
                return new ProcessResult { IsSucceded = true, Message = "Image Updated Sccessfully" };
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return new ProcessResult { IsSucceded = false, Message = "ُError Updating Image due to " + msg };
            }
        }
        public async Task<ProcessResult> AddOrUpdatBackGroundImage([FromForm] IFormFile Image, string ProfileId)
        {
            var exist = await CheckProfile(ProfileId);
            if (!exist)
                return new ProcessResult { IsSucceded = false, Message = "There is no Profile with id " + ProfileId };
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(Image.FileName).ToLowerInvariant();
            if (!(Image != null && allowedExtensions.Contains(fileExtension)))
            {
                return new ProcessResult { Message = "Invalid file type. Please upload an image.", IsSucceded = false };
            }
            var profile = await context.Profiles.FirstOrDefaultAsync(p => p.Id == ProfileId);
            try
            {
                using var stream = new MemoryStream();
                await Image.CopyToAsync(stream);
                profile.BackgroundImage = stream.ToArray();
                await context.SaveChangesAsync();
                return new ProcessResult { IsSucceded = true, Message = "Image Updated Sccessfully" };
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return new ProcessResult { IsSucceded = false, Message = "ُError Updating Image due to " + msg };
            }

        }
        public async Task<ProcessResult> DeleteForegroundImage(string ProfileId)
        {
            var exist = await CheckProfile(ProfileId);
            if(!exist)
                return new ProcessResult { IsSucceded = false, Message = "There is no Profile with id " + ProfileId };
            var prof = await context.Profiles.FirstOrDefaultAsync(p => p.Id == ProfileId);
            prof.ForegroundImage = null;
            await context.SaveChangesAsync();
            return new ProcessResult { IsSucceded = true, Message = "Image Deleted Successfully" };

        }
        public async Task<ProcessResult> DeleteBackgroundImage(string ProfileId)
        {
            var exist = await CheckProfile(ProfileId);
            if (!exist)
                return new ProcessResult { IsSucceded = false, Message = "There is no Profile with id " + ProfileId };
            var prof = await context.Profiles.FirstOrDefaultAsync(p => p.Id == ProfileId);
            prof.BackgroundImage = null;
            await context.SaveChangesAsync();
            return new ProcessResult { IsSucceded = true, Message = "Image Deleted Successfully" };

        }

        public async Task<Profile> GetProfile(string ProfileId)
        {
            return await context.Profiles.FirstOrDefaultAsync(p => p.Id == ProfileId);
        }

        private async Task<bool> CheckProfile(string ProfileId)
        {
            var prof = await context.Profiles.FirstOrDefaultAsync(p => p.Id == ProfileId);
            if (prof is null)
                return false;
            return true;
        }
    }
}

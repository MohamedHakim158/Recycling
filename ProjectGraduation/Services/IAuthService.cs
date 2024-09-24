using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;

namespace ProjectGraduation.Services
{
    public interface IAuthService
    {
        public Task<ProcessResult> RegisterAsync(RegisterRequest request);
        public Task<ProcessResult> ConfirmEmailAsync(string email, string code);
        public Task<ProcessResult> GenerateConfirmEmailAsync(string email);
        public Task<AuthDto> LoginAsync(LoginRequest request);
        public Task<ProcessResult> ResendConfirmCodeAsync(string Email);
        public Task<ProcessResult> ForgetPasswordAsync(string Email);
        public Task<ProcessResult> ResetPasswordAsync(ResetPasswordRequest request);
        public Task<ProcessResult> ChangePasswordAsync(string Email , string NewPassword);
    }
}

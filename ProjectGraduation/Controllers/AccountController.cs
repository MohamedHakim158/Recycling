using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using ProjectGraduation.Services;

namespace ProjectGraduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // timer for Delete Account
        private readonly UserManager<Client> manager;
        private readonly IAuthService authservice;
        private readonly GraduationProjectContext context;
        public AccountController( UserManager<Client> _manager, IAuthService _authservice , GraduationProjectContext _context)
        {
            manager = _manager;
            authservice = _authservice;
            context = _context;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm]RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (manager.Users.Any(u => u.Email == model.Email))
                return BadRequest("There is Already an Account Created with this Email");
            if (manager.Users.Any(u => u.UserName == model.UserName))
                return BadRequest("UserName Already Taken");
           
            var result = await authservice.RegisterAsync(model);
            if (result.IsSucceded)
            {
                var process = await authservice.GenerateConfirmEmailAsync(model.Email);
                if (process.IsSucceded)
                    return Ok(process.Message);
                else
                    return BadRequest(process.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }


        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromForm] string code ,[FromForm] string Email)
        {
            var Process = await authservice.ConfirmEmailAsync(Email, code);
            if (Process.IsSucceded)
              return Ok(Process.Message);
            
            else
               return BadRequest(Process.Message);
            
        }


        [HttpGet("ResendConfirmEmail/{Email}")]
        public async Task<IActionResult> ResendConfirmEmail(string Email)
        {
            var process = await authservice.ResendConfirmCodeAsync(Email);
            if (process.IsSucceded)
                return Ok(process.Message);

            return BadRequest(process.Message);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var result = await authservice.LoginAsync(model);
            if (result.IsAuthenticated)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }


        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok("You Logout Succesfully");
        }


        [HttpGet("ForgetPassword/{Email}")]
        public async Task<IActionResult> ForgetPassword(string Email)
        {
            var result = await authservice.ForgetPasswordAsync(Email);
            if (result.IsSucceded)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var result = await authservice.ResetPasswordAsync(model);
            if (result.IsSucceded)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromForm] LoginRequest model)
        {
            var result = await authservice.ChangePasswordAsync(model.Email, model.Password);
            if(result.IsSucceded)
                return Ok(result.Message);
            return BadRequest(result.Message);
        }

        [HttpGet("AddAdmin/{UserName}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddAdmin(string UserName)
        {
            var user = await manager.FindByNameAsync(UserName);
            if (user == null)
                return BadRequest("There is no Account with UserName: " + UserName);
            await manager.AddToRoleAsync(user, "Admin");
            return Ok("Account with UserName " + UserName + " Is now an Admin");
        }


        [HttpGet("SearchForUser/{SearchContent}")]
        [Authorize]
        public async Task<IActionResult> SearchForUser(string SearchContent)
        {
            var listOfString = SearchContent.Split(' ');
            var Users = new List<UserDto>();
            for (int i = 0; i < listOfString.Length; i++)
            {
                Users.AddRange(await context.SearchForUser(SearchContent));
            }
            Users.OrderBy(p => new { p.Fname, p.Lname });
            
            if (Users is null)
                return Ok("No Results");
            return Ok(Users);
        }

    }
}

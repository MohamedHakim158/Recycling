using ProjectGraduation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using ProjectGraduation.DTO;
using ProjectGraduation.Helping_Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ProjectGraduation.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Client> manager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailSender sender;
        private readonly IConfiguration config;
        private readonly GraduationProjectContext context;
        Random random = new Random();
        public AuthService(UserManager<Client> _manager, IEmailSender _sender , IConfiguration _configuration, GraduationProjectContext context, RoleManager<IdentityRole> roleManager)
        {
            manager = _manager;
            sender = _sender;
            config = _configuration;
            this.context = context;
            this.roleManager = roleManager;
        }


        public async Task<ProcessResult> RegisterAsync(RegisterRequest model)
        {
            var user = new Client
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Fname = model.FName,
                Lname = model.LName,
                RegisteredAs = model.RegisteredAs,
            };
            var result = await manager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                try
                {
                    await context.Profiles.AddAsync(new Profile { ClientId = user.Id , Profession = model.Profession!=null?model.Profession:"" });
                    
                    await context.SaveChangesAsync();
                    if (!await roleManager.RoleExistsAsync(user.RegisteredAs))
                    {
                        await roleManager.CreateAsync(new IdentityRole
                        {
                            Name = user.RegisteredAs,
                            NormalizedName = user.RegisteredAs.ToUpper()
                        });
                    }
                    await manager.AddToRoleAsync(user,user.RegisteredAs);
                    user.ConfirmEmailCode = await GenerateCode();
                    await manager.UpdateAsync(user);
                    return new ProcessResult { IsSucceded = true, Message = "Added Succssfully" };
                }
                catch (Exception ex)
                {
                    await manager.DeleteAsync(user);
                    return new ProcessResult { IsSucceded = false, Message = $"Error Adding User Due to {ex.Message}" };
                }
            }
            string Error = string.Empty;
            foreach (var error in result.Errors)
                Error += $"\n{error.Description} , ";
            return new ProcessResult { IsSucceded = false, Message = $"{Error}" };

        }

        public async Task<ProcessResult> ConfirmEmailAsync(string Email, string code)
        {
            var user = await manager.FindByEmailAsync(Email);
            if (user == null)
                return new ProcessResult { IsSucceded = false, Message = $"Threre is no Account with Email:{Email}" };
            if(code == user.ConfirmEmailCode)
            {
                user.ConfirmEmailCode = null;
                user.EmailConfirmed = true;
                await manager.UpdateAsync(user);
                return new ProcessResult { IsSucceded = true, Message = "Email Confirmed Successfully" };
            }
            else
                return new ProcessResult { IsSucceded = false, Message = "Wrong Code Entered" };

        }
       
        public async Task<ProcessResult> GenerateConfirmEmailAsync(string Email)
        {
            var user = await manager.FindByEmailAsync(Email);
            if(user == null)
                return new ProcessResult { IsSucceded = false, Message = $"Threre is no Account with Email:{Email}" };
            try
            {
                if (user.ConfirmEmailCode == null)
                {
                    user.ConfirmEmailCode = await GenerateCode();
                    await manager.UpdateAsync(user);
                }
                
                await sender.SendEmailAsync(Email, "Confirmation Email", $"<center><h1>Please Confirm Your Email Using Code Below</h1>\n" +
                   $"<h1 style=\"color:blue\">{user.ConfirmEmailCode}</h1></center>");
                return new ProcessResult { IsSucceded = true, Message = "Check your email to get Confirm code" };
            }
            catch (Exception ex)
            {
                return new ProcessResult { IsSucceded = false, Message = "Error Sending Email Please check if Email Entered is Correct" };
            }
        }
        public async Task<AuthDto> LoginAsync(LoginRequest request)
        {
            var authDTo = new AuthDto();
            var user =await context.Clients.Include(c=>c.Profile).FirstOrDefaultAsync(c=>c.Email==request.Email);
            if (user == null)
            {
                authDTo.Message = $"There is no Account with Email {request.Email}";
                authDTo.IsAuthenticated = false;
                return authDTo;
            }
            if (!user.EmailConfirmed)
            {
                authDTo.Message = $"Email is not Confirmed, Please Cofnirm your Email!";
                authDTo.IsAuthenticated = false;
                return authDTo;
            }
            if (await manager.CheckPasswordAsync(user, request.Password))
            {
                try
                {
                    authDTo = await GenerateTokenAsync(user);
                    return authDTo;
                }
                catch (Exception ex)
                {
                    authDTo.Message = ex.Message;
                    authDTo.IsAuthenticated = false;
                    return authDTo;
                }
            }
            else
            {
                authDTo.Message = $"Wrong Password!";
                authDTo.IsAuthenticated = false;
                return authDTo;
            }


        }
        public async Task<ProcessResult> ResendConfirmCodeAsync(string Email)
        {
            var user = await manager.FindByEmailAsync(Email);
            if (user == null)
                return new ProcessResult { IsSucceded = false, Message = $"Threre is no Account with Email: {Email}" };
            user.ConfirmEmailCode = await GenerateCode();
            await manager.UpdateAsync(user);
            return await GenerateConfirmEmailAsync(Email);
        }
        public async Task<ProcessResult> ForgetPasswordAsync(string Email)
        {
            var user = await manager.FindByEmailAsync(Email);
            if (user == null)
            {
                return new ProcessResult { IsSucceded = false, Message = $"There is no Account with Email: {Email}" };
            }
            try
            {
                user.ResetPasswordCode = await GenerateCode();
                await manager.UpdateAsync(user);
                await sender.SendEmailAsync(Email, "Reset Password Code", $"<center><h1>You Have to Use Code Below to Reset Your password</h1>\n" +
                   $"<h1 style=\"color:blue\">{user.ResetPasswordCode}</h1></center>");
                return new ProcessResult { IsSucceded = true, Message = "Check your email to get Reset Your Password" };
            }
            catch (Exception ex)
            {
                return new ProcessResult { IsSucceded = false, Message = $"Error Sending Reset Password Email Due to {ex.Message}" };
            }
        }
        public async Task<ProcessResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await manager.FindByEmailAsync(request.Email);
            if (user == null)
                return new ProcessResult { IsSucceded = false, Message = $"There is no Account with Email {request.Email} " };
            if (request.Code != user.ResetPasswordCode)
                return new ProcessResult { IsSucceded = false, Message = "Error Code Entered" };

            var token = await manager.GeneratePasswordResetTokenAsync(user);
            var result = await manager.ResetPasswordAsync(user,token ,request.NewPassword);
            
            if (result.Succeeded)
            {
                user.ResetPasswordCode = null;
                await manager.UpdateAsync(user);
                return new ProcessResult { IsSucceded = true, Message = "Password Sat Successfully" };
            }
            string Error = string.Empty;
            foreach (var error in result.Errors)
                Error += $"\n{error.Description} , ";
            return new ProcessResult { IsSucceded = false, Message = $"Error Reseting Password Due to {Error}" };



        }
        public async Task<ProcessResult> ChangePasswordAsync(string Email , string NewPassword)
        {
            var User = await manager.FindByEmailAsync(Email);
            if (User != null)
            {
                var token = await manager.GeneratePasswordResetTokenAsync(User);
                var result = await manager.ResetPasswordAsync(User, token, NewPassword);
                if(result.Succeeded)
                {
                    return new ProcessResult { IsSucceded = true, Message = "Password resat successfully" };
                }
                string Error = string.Empty;
                foreach (var error in result.Errors)
                    Error += $"\n{error.Description} , ";
                return new ProcessResult { IsSucceded = false, Message = $"Error Reseting Password Due to {Error}" };
            }
            return new ProcessResult { IsSucceded = false, Message = "There is no Account with Email: " + Email };
        }





        private async Task<string> GenerateCode()
        {
            return random.Next(100000, 1000000).ToString();
        }
        private async Task<AuthDto> GenerateTokenAsync(Client client)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, client.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, client.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            var roles = await manager.GetRolesAsync(client);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));
            SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken
            (
                claims: claims,
                audience: config["JWT:Audience"],
                issuer: config["JWT:Issuer"],
                expires:DateTime.Now.AddDays(10),
                signingCredentials:cred
            );


            return new AuthDto
            {
                Email = client.Email,
                ExpireOn = token.ValidTo,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = client.Id,
                IsAuthenticated = true,
                Roles = roles.ToList(),
                ProfileId = client.Profile.Id

            };
        }

    }
}

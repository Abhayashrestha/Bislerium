using Bislerium.API.Model.Dto;
using Bislerium.API.Respositories.Implementation;
using Bislerium.API.Respositories.Repository;
using Bislerium.API.Respositories.Respository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRespository tokenRepository;
        private readonly IEmailServiceRespository emailserviceRespository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRespository tokenRepository, IEmailServiceRespository emailServiceRespository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.emailserviceRespository = emailServiceRespository;
        }
        //POST: {apibaseurl}/api/auth/login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var identityUser = await userManager.FindByEmailAsync(request.Email);
            if (identityUser is not null)
            {
                var CheckpasswordResults = await userManager.CheckPasswordAsync(identityUser, request.password);

                if (CheckpasswordResults)
                {
                    var roles = await userManager.GetRolesAsync(identityUser);
                    //Create a Token and Response
                    var JwtToken = tokenRepository.createJwtToken(identityUser, roles.ToList());

                    var response = new LoginResponseDto()
                    {
                        UserId = identityUser.Id,
                        Email = request.Email,
                        Roles = roles.ToList(),
                        Token = JwtToken

                    };
                    return Ok(response);
                }
            }
            ModelState.AddModelError("", "Email or Password is incorrect");

            return ValidationProblem(ModelState);
        }
        //POST: {apibaseurl}/api/auth/adduser
        [HttpPost]
        [Route("AddAdmin")]
        public async Task<IActionResult> AddAdmin([FromBody] AddUserRequestDto request)
        {
            //Create IdentityUser object
            var User = new IdentityUser
            {
                UserName = request.Name?.Trim(),
                Email = request.Email?.Trim(),
                PhoneNumber = request.Number?.Trim()
            };

            //Create User
            var identityResult = await userManager.CreateAsync(User, request.password);

            if (identityResult.Succeeded)
            {
                //Add Role to user(Reader)
                identityResult = await userManager.AddToRoleAsync(User, "Admin");

                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return ValidationProblem(ModelState);
        }
        //POST: {apibaseurl}/api/auth/adduser
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] AddUserRequestDto request)
        {
            //Create IdentityUser object
            var User = new IdentityUser
            {
                UserName = request.Name?.Trim(),
                Email = request.Email?.Trim(),
                PhoneNumber = request.Number?.Trim()
            };

            //Create User
            var identityResult = await userManager.CreateAsync(User, request.password);

            if (identityResult.Succeeded)
            {
                //Add Role to user(Reader)
                identityResult = await userManager.AddToRoleAsync(User, "Blogger");

                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return ValidationProblem(ModelState);
        }


        [HttpPost]
        [Route("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model, string id)
        {
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError("ConfirmNewPassword", "The new password and confirmation password do not match.");
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ModelState.AddModelError("Id", "User not found.");
                return NotFound(ModelState);
            }

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok(new { message = "Password successfully changed." });
        }
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found.");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = dto.Email }, Request.Scheme);

            await emailserviceRespository.SendEmailAsync(dto.Email, "Reset your password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return Ok("Reset password link sent.");
        }


        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found.");

            var result = await userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password has been reset successfully.");
        }
    }
}

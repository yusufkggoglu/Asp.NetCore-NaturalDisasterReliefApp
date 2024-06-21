using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NaturalDisasters.IdentityServer.ActionFilters;
using NaturalDisasters.IdentityServer.Dtos;
using NaturalDisasters.IdentityServer.Models;
using NaturalDisasters.IdentityServer.Services;
using Shared.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace NaturalDisasters.IdentityServer.Controllers
{
    [Authorize(LocalApi.PolicyName)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilter))]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();
                return BadRequest(new { errors });
            }

            // Kullanıcı adı ve e-posta kontrolü
            var existingUserByEmail = await _userManager.FindByEmailAsync(signUpDto.Email);
            if (existingUserByEmail != null)
            {
                return BadRequest(Response<NoContent>.Fail("Email zaten kayıtlı.", 400));
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(signUpDto.UserName);
            if (existingUserByUsername != null)
            {
                return BadRequest(Response<NoContent>.Fail("Kullanıcı adı zaten kayıtlı.", 400));
            }

            ServiceKpsPublic serviceKpsPublic = new ServiceKpsPublic();
            var serviceResult = await serviceKpsPublic.OnGetService(signUpDto.TC,signUpDto.Name,signUpDto.Surname,signUpDto.BirthYear);
            if (serviceResult)
            {
                var user = new ApplicationUser
                {
                    Name = signUpDto.Name,
                    Surname = signUpDto.Surname,
                    UserName = signUpDto.UserName,
                    Email = signUpDto.Email,
                    City = signUpDto.City,
                    PhoneNumber = signUpDto.PhoneNumber,
                };
                var result = await _userManager.CreateAsync(user, signUpDto.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));
                }

                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }

                await _userManager.AddToRoleAsync(user, "User");

                return NoContent();
            } 
            else if (!serviceResult)
            {
                return BadRequest(Response<NoContent>.Fail("Kimlik Bilgileriniz Doğrulanamadı !", 400));
            }
            else
            {
                return BadRequest(Response<NoContent>.Fail("Kimlik Doğrulama Servisine Bağlanılamadı !", 500));
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (user == null)
            {
                return BadRequest();
            }
            return Ok(new
            {
                Id = user.Id,
                UserName = user.UserName,
                Name=user.Name,
                Surname=user.Surname,
                Email = user.Email,
                City = user.City,
                PhoneNumber= user.PhoneNumber
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInfo(UpdateUserDto userDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            var user = await _userManager.FindByIdAsync(userIdClaim.Value);

            user.City = userDto.City;
            user.Email = userDto.Email;
            user.UserName = userDto.UserName;
            //user.Name= userDto.Name;
            //user.Surname = userDto.Surname;
            user.PhoneNumber = userDto.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDto userDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            var user = await _userManager.FindByIdAsync(userIdClaim.Value);
            if (!userDto.OldPassword.IsNullOrEmpty() && !userDto.NewPassword.IsNullOrEmpty())
            {
                var hashOldPassword =  _userManager.PasswordHasher.HashPassword(user, userDto.OldPassword);
                var status =_userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash,userDto.OldPassword );
                if (status == PasswordVerificationResult.Success)
                {
                    if (await _userManager.HasPasswordAsync(user))
                    {
                        await _userManager.RemovePasswordAsync(user);
                    }
                    var result = await _userManager.AddPasswordAsync(user, userDto.NewPassword);
                    return Ok(result.Succeeded);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmailAndSend([FromBody] CheckEmailDto checkEmailDto)
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(checkEmailDto.Email);
            if (existingUserByEmail != null)
            {
                PasswordResetService resetService = new PasswordResetService(_userManager);
                var status = await resetService.ResetPassword(checkEmailDto.Email);
                if (status)
                {
                    return Ok(Response<NoContent>.Success(200));
                }
            }
            return BadRequest(Response<NoContent>.Fail("Sistemde böyle bir email adresine ait kullanıcı bulunamadı.", 400));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode([FromBody] CheckResetCodeDto resetcode)
        {
            var user = await _userManager.FindByEmailAsync(resetcode.Email);
            if (user != null)
            {
                if (user.PasswordResetCode == resetcode.Code)
                {
                    return Ok(Response<NoContent>.Success(200));
                }
            }
            return BadRequest(Response<NoContent>.Fail("Sistemde böyle bir email adresine ait kullanıcı bulunamadı.", 400));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetCodeDto reset)
        {
            var user = await _userManager.FindByEmailAsync(reset.Email);
            if (user != null)
            {
                if (user.PasswordResetCode == reset.Code)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, reset.Password);

                    if (result.Succeeded)
                    {
                        return Ok(Response<NoContent>.Success(200));
                    }
                    return BadRequest(Response<NoContent>.Fail("Şifre değiştirirken bir hata oluştu, daha sonra tekrar deneyiniz.", 400));
                }
            }
            return BadRequest(Response<NoContent>.Fail("Sistemde böyle bir email adresine ait kullanıcı bulunamadı.", 400));
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Role assigned successfully");
        }
    }
}

public class AssignRoleModel
{
    public string Email { get; set; }
    public string Role { get; set; }
}

public class CheckEmailDto
{
    public string Email { get; set; }
}

public class CheckResetCodeDto
{
    public string Email { get; set; }
    public string Code { get; set; }
}

public class ResetCodeDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Code { get; set; }
}
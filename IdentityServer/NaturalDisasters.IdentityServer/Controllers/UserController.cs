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
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
            user.Name= userDto.Name;
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
    }
}

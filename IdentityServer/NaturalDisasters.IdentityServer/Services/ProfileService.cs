using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using NaturalDisasters.IdentityServer.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace NaturalDisasters.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            if (user == null)
            {
                throw new ArgumentException("Invalid user identifier");
            }

            var claims = new List<Claim>
        {
            new Claim(JwtClaimTypes.Email, user.Email)
        };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            context.IsActive = (user != null) && user.LockoutEnd == null;
        }
    }
}

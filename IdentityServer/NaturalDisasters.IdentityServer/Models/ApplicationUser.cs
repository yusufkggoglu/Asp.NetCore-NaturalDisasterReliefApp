using Microsoft.AspNetCore.Identity;

namespace NaturalDisasters.IdentityServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PasswordResetCode { get; set; }
    }
}

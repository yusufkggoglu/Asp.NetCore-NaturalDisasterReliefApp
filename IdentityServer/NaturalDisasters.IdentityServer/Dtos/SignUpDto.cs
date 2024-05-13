using System.ComponentModel.DataAnnotations;

namespace NaturalDisasters.IdentityServer.Dtos
{
    public class SignUpDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public long TC { get; set; }
        [Required]
        public int BirthYear { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}

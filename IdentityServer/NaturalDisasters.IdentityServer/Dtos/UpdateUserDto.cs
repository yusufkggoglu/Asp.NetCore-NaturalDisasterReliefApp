namespace NaturalDisasters.IdentityServer.Dtos
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        //public string Name { get; set; }
        //public string Surname { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
    }
}

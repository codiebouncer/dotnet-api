namespace PropMan.Models.Dto
{
    public class CreateTenant
    {

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber{ get; set; }
        public string NationalId { get; set; } = null!;
        
        


    }
}
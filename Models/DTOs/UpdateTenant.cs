namespace PropMan.Models.Dto
{
    public class UpdateTenant
    {
        public Guid TenantId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string NationalId { get; set; } = null!;
    }
}
namespace PropMan.Models.Dto
{
    public class TenantDto
{
    public Guid TenantId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string NationalId { get; set; }
    public bool IsActive { get; set; }
       public bool IsAssigned { get; set; }
    public string? PropertyName { get; set; }
    public string? UnitName { get; set; }
    public Guid? PropertyTenantId { get; set; }
}

}
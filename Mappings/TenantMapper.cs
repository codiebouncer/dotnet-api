using PropMan.Models;
using PropMan.Models.Dto;

namespace PropMan.Map
{
    public static class TenantMapper
{
    public static TenantDto ToDto(this Tenant t)
{
    var activeAssignment = t.PropertyTenants
        ?.Where(pt => pt.IsActive)
        .OrderByDescending(pt => pt.CreatedAt)
        .FirstOrDefault();

    return new TenantDto
    {
        TenantId = t.TenantId,
        FullName = t.FullName,
        Email = t.Email,
        PhoneNumber = t.PhoneNumber,
        NationalId = t.NationalId,
        IsActive = t.IsActive,

        IsAssigned = activeAssignment != null,
        PropertyTenantId = activeAssignment?.PropertyTenantId,
        PropertyName = activeAssignment?.Property?.PropertyName,
        UnitName = activeAssignment?.Unit?.UnitName
    };
}

}

}
using PropMan.Models;
using PropMan.Models.Dto;

namespace PropMan.Map
{
    public static class PropertyTenantMapper
{
    public static PropertyTenantDto ToDto(this PropertyTenant p)
    {
        return new PropertyTenantDto
        {
            PropertyTenantId = p.PropertyTenantId,
            UnitId = p.UnitId,
            PropertyId = p.PropertyId,
            TenantId = p.TenantId,
            CompanyId = p.CompanyId,
            PlanId = p.PlanId,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            InstallmentAmount = p.installmentAmount,
            TotalInstallments = p.totalInstallments,
            Status = p.Status,
            AmountDue = p.AmountDue,
            InitialDeposit = p.InitialDeposit,
            IsActive = p.IsActive,
            LastPaymentDate = p.LastPaymentDate,
            IsPrimaryTenant = p.IsPrimaryTenant,
            CreatedAt = p.CreatedAt
        };
    }
}

}
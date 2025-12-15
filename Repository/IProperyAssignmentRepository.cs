

using PropMan.Models;

namespace Propman.Repository
{
    public interface IPropertyAssignmentRepository
    {
        Task<bool> TenantActive(Guid tenantId);
        Task<bool> PropertyActive(Guid propertyId);
        Task<PaymentPlan?> PlanById(Guid planid);
        Task<PropertyTenant?> TenantById(Guid tenantId);
        Task<PropertyTenant?> GetById(Guid proptenantId);
        Task AddPropertyTenant(PropertyTenant propertyTenant);
        Task UpdatePropertyTenant(PropertyTenant propertyTenant);
        Task<IEnumerable<PropertyTenant>> GetActiveTenants();
        Task<List<PropertyTenant>> AllPropTens();
    }
}
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;

namespace Propman.Repository
{
    public interface ITenantRepository
    {
        Task AddTenant(Tenant tenant);
        Task<Tenant?> GetTenant(string tenantname);
        Task<bool> TenantExists(string national_id, string tenantname);
        Task<Tenant?> GetById(Guid Tenanid);
        Task<(List<TenantDto> Records, int TotalCount)> GetAllTenants(
    int page,
    int pageSize,
    string? search,
    DateTime? startDate,
    DateTime? endDate
);
        Task<bool> Delete(Guid id);
        Task<Tenant> UpdateTenant(Tenant tenant);
        Task AddNotification(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsByTenant(Guid tenantId);
        Task<IEnumerable<Notification>> GetAllNotifications(Guid companyId);
        Task<List<TenantDropdownDto>> GetTenants();
        Task<List<PlanDto>> AllPlans();
        Task<FetchDataRecords<Payment>> GetPaymentsByTenantId(Guid tenantId, int page, int pageSize,DateTime? startDate,DateTime? endDate);
        Task<FetchDataRecords<Invoice>> GetInvoicesByTenantId(Guid tenantId, int page, int pageSize,DateTime? startDate,DateTime? endDate);
        Task<FetchDataRecords<Payment>> GetAllPayments( int page, int pageSize,DateTime? startDate,DateTime? endDate);
    }
}
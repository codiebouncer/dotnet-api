using Microsoft.AspNetCore.Mvc;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;

namespace PropMan.Services
{
    public interface ITenantService
    {
        Task<ApiResponse<IActionResult>> CreateTenant(CreateTenant request);
        Task<ApiResponse<IActionResult>> DeleteTenant(Guid id);
        Task<ApiResponse<FetchDataRecords<TenantDto>>> GetAllTens(int page, int pageSize, string search,DateTime? startDate,DateTime? EndDate);
        Task<ApiResponse<Tenant>> UpdateTenant(UpdateTenant request);
        Task<ApiResponse<Invoice>> MakePayment(PaymentRequest request);
        Task<ApiResponse<List<PlanDto>>> AllPlans();
        Task<ApiResponse<List<PropertyTenant>>> AllPropTens();
        Task<ApiResponse<List<Invoice>>> InvoiceByPropTenId(Guid proptenantId);
        Task<ApiResponse<List<Payment>>> AllPaymentsById(Guid proptenantId);
        Task<ApiResponse<List<Payment>>> AllPayments();
        Task<ApiResponse<FetchDataRecords<AllPayments>>> GetTenantPayments(
   Guid tenantId, int page, int pageSize,DateTime? startDate,DateTime? endDate);
       Task<ApiResponse<FetchDataRecords<InvoiceDto>>> GetTenantInvoices(
    Guid tenantId, int page, int pageSize,DateTime? startDate,DateTime? endDate);
    Task<ApiResponse<List<TenantDropdownDto>>>TenantDropdown();
    }
}
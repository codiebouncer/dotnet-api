using PropMan.Models;

namespace Propman.Repository
{
    public interface IInvoiceRepository
    {
        Task<bool> InvoiceUnpaid(Guid tenantId);
        Task<bool> PropertyInvoiceUnpaid(Guid propertyId);
        Task<List<Invoice>> GetAllInvoices();
        Task AddInvoice(Invoice invoice);
        Task<Invoice?> GetById(Guid invoiceId);
        Task<Invoice> UpdateInvoice(Invoice invoice);
        Task AddPayment(Payment payment);
        Task<List<Invoice>> OverdueInvoice();
        Task<List<Invoice>> UpcomingInvoice();
        Task<List<PropertyTenant>> ActiveTenants();
        Task<bool> AlreadyInvoiced(PropertyTenant tenant);
        Task<Invoice?> GetLastInvoiceByTenantId(Guid propertyTenantId);
        Task<List<Invoice>> UnpaidInvoice(Guid proptenantId);
        Task CreateTenantCredit(Guid propertyTenantId, decimal amount);
        Task UpdateTenantCredit(TenantCredit credit);
        Task<TenantCredit?> TenantCreditById(Guid proptenantId);
        Task<Invoice?> GetInvoiceByTenantAndDueDate(Guid propertyTenantId, DateTime dueDate);
        Task<List<Invoice>> InvoiceByPropTenantId(Guid proptenantId);
        Task<List<Payment>> AllPaymentsById(Guid proptenantId);
        Task<List<Payment>> AllPayments();
    }
}
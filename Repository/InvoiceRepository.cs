using Microsoft.EntityFrameworkCore;
using Propman.Constants;
using Propman.Services.UserContext;
using PropMan.Models;

namespace Propman.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DataContext _context;
        private readonly IUserContext _userContext;

        public InvoiceRepository(DataContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        // ✅ Check if tenant has any unpaid invoices
        public async Task<bool> InvoiceUnpaid(Guid tenantId)
        {
            return await _context.Invoices
                .Include(i => i.PropertyTenant)
                .AnyAsync(i => i.PropertyTenant.TenantId == tenantId
                    && i.PropertyTenant.IsActive
                    && (i.Status == InvoiceStatus.UnPaid || i.Status == InvoiceStatus.InProgress));
        }

        // ✅ Check if a property has unpaid invoices
        public async Task<bool> PropertyInvoiceUnpaid(Guid propertyId)
        {
            return await _context.Invoices
                .Include(i => i.PropertyTenant)
                .AnyAsync(i => i.PropertyTenant.PropertyId == propertyId
                    && i.PropertyTenant.IsActive
                    && (i.Status == InvoiceStatus.UnPaid || i.Status == InvoiceStatus.InProgress));
        }

        public async Task<List<Invoice>> GetAllInvoices()
        {
            var compId = Guid.Parse(_userContext.CompanyId);
            return await _context.Invoices
                .Include(i => i.PropertyTenant)
                .ThenInclude(pt => pt.Tenant)
                .Where(i => i.CompanyId == compId && i.PropertyTenant.IsActive)
                .ToListAsync();
        }

        public async Task AddInvoice(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<Invoice?> GetById(Guid invoiceId)
        {
            return await _context.Invoices
                .Include(i => i.PropertyTenant)
                .ThenInclude(pt => pt.Tenant)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && i.PropertyTenant.IsActive);
        }

        public async Task<Invoice> UpdateInvoice(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task AddPayment(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        // ✅ Get overdue invoices only for active tenants
        public async Task<List<Invoice>> OverdueInvoice()
        {
            var today = DateTime.UtcNow;
            return await _context.Invoices
                .Include(i => i.PropertyTenant)
                .ThenInclude(pt => pt.Tenant)
                .Where(i => i.Status != InvoiceStatus.Paid 
                            && i.DueDate < today
                            && i.PropertyTenant.IsActive)
                .ToListAsync();
        }

        // ✅ Upcoming invoices with reminders, only for active tenants
        public async Task<List<Invoice>> UpcomingInvoice()
        {
            var today = DateTime.UtcNow;
            var threeDaysFromNow = today.AddDays(3);

            return await _context.Invoices
                .Include(i => i.PropertyTenant)
                .ThenInclude(pt => pt.Tenant)
                .Where(i => !i.ReminderSent
                            && i.DueDate.Date <= threeDaysFromNow
                            && i.DueDate.Date >= today
                            && i.PropertyTenant.IsActive)
                .ToListAsync();
        }

        public async Task<List<PropertyTenant>> ActiveTenants()
        {
            return await _context.PropertyTenants
                .Where(pt => pt.IsActive)
                .ToListAsync();
        }

        public async Task<bool> AlreadyInvoiced(PropertyTenant tenant)
        {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            return await _context.Invoices
                .Include(i => i.PropertyTenant)
                .AnyAsync(i => i.PropertyTenant.TenantId == tenant.TenantId 
                               && i.PropertyTenant.IsActive
                               && i.CreatedAt >= startOfMonth);
        }

        public async Task<Invoice?> GetLastInvoiceByTenantId(Guid propertyTenantId)
        {
            return await _context.Invoices
                .Where(i => i.PropertyTenantId == propertyTenantId && i.PropertyTenant.IsActive)
                .OrderByDescending(i => i.DueDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Invoice>> UnpaidInvoice(Guid propertyTenantId)
        {
            return await _context.Invoices
                .Where(i => i.PropertyTenantId == propertyTenantId
                            && i.Status != InvoiceStatus.Paid
                            && i.PropertyTenant.IsActive)
                .OrderBy(i => i.DueDate)
                .ToListAsync();
        }

        public async Task CreateTenantCredit(Guid propertyTenantId, decimal amount)
        {
            var credit = new TenantCredit
            {
                PropertyTenantId = propertyTenantId,
                Amount = amount,
                CreatedAt = DateTime.UtcNow
            };
            await _context.TenantCredits.AddAsync(credit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTenantCredit(TenantCredit credit)
        {
            _context.TenantCredits.Update(credit);
            await _context.SaveChangesAsync();
        }

        public async Task<TenantCredit?> TenantCreditById(Guid propertyTenantId)
        {
            return await _context.TenantCredits
                .FirstOrDefaultAsync(pt => pt.PropertyTenantId == propertyTenantId);
        }

        public async Task<Invoice?> GetInvoiceByTenantAndDueDate(Guid propertyTenantId, DateTime dueDate)
        {
            return await _context.Invoices
                .Where(i => i.PropertyTenantId == propertyTenantId
                            && i.DueDate.Date == dueDate.Date
                            && i.PropertyTenant.IsActive)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Invoice>> InvoiceByPropTenantId(Guid proptenantId)
        {
            return await _context.Invoices
            .Where(x => x.PropertyTenantId == proptenantId)
            .OrderByDescending(x=> x.CreatedAt)
            .ToListAsync();
        }
        public async Task<List<Payment>> AllPaymentsById(Guid proptenantId)
        {
            return await _context.Payments
            .Where(x => x.PropertyTenantId == proptenantId)
            .OrderByDescending(x=> x.CreatedAt)
            .ToListAsync();
        }
        public async Task<List<Payment>>AllPayments()
        {
            return await _context.Payments
            .OrderByDescending(x=> x.CreatedAt)
            .ToListAsync();
        }
    }
}

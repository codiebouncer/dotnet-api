using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Propman.Constants;
using Propman.Logic;
using Propman.Repository;
using Propman.Services;
using PropMan.Models;
using PropMan.Services;
using PropMan.Services.AuditLogService;
using PropMan.EmailTemplates; // <<< add this

public class InvoiceJobService : IInvoiceJobService
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IPropertyRepository _propRepo;
    private readonly IPropAssLogic _propassLogic;
    private readonly IPropertyAssignmentRepository _propassRepo;
    private readonly IEmailService _emailService;
    private readonly ITenantRepository _tenantRepo;
    private readonly IAuditLogService _auditLog;
    private readonly INotificationService _notify;
    private readonly ITransactionService _transaction;

    public InvoiceJobService(
        IInvoiceRepository invoiceRepo,
        IPropertyRepository propRepo,
        IPropAssLogic propAssLogic,
        IPropertyAssignmentRepository propassRepo,
        IEmailService emailService,
        ITenantRepository tenantRepo,
        IAuditLogService auditLog,
        INotificationService notify,
        ITransactionService transaction)
    {
        _invoiceRepo = invoiceRepo;
        _propRepo = propRepo;
        _propassLogic = propAssLogic;
        _propassRepo = propassRepo;
        _emailService = emailService;
        _tenantRepo = tenantRepo;
        _auditLog = auditLog;
        _notify = notify;
        _transaction = transaction;
    }

    public async Task CheckAndNotifyOverdueInvoices()
    {
        try
        {
            var overdueInvoices = await _invoiceRepo.OverdueInvoice();

            foreach (var invoice in overdueInvoices)
            {
                invoice.Status = InvoiceStatus.Defaulted;
                await _invoiceRepo.UpdateInvoice(invoice);

                var tenant = await _tenantRepo.GetById(invoice.PropertyTenant.TenantId);

                var subject = EmailTemplates.OverdueInvoiceSubject();
                var body = EmailTemplates.OverdueInvoiceBody(
                    tenant.FullName,
                    invoice.Balance,
                    invoice.DueDate
                );

                await _emailService.SendEmail(tenant.Email, subject, body);

                await _notify.AddNotification(
                    tenant.CompanyId,
                    tenant.PropertyTenants.FirstOrDefault()?.PropertyTenantId ?? Guid.Empty,
                    "Email",
                    body,
                    tenant.TenantId
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
    }

    public async Task SendPaymentReminders()
    {
        var upcominginvoice = await _invoiceRepo.UpcomingInvoice();

        foreach (var invoice in upcominginvoice)
        {
            var tenant = await _tenantRepo.GetById(invoice.PropertyTenant.TenantId);

            var subject = EmailTemplates.PaymentReminderSubject();
            var body = EmailTemplates.PaymentReminderBody(
                tenant.FullName,
                invoice.Balance,
                invoice.DueDate
            );

            await _emailService.SendEmail(tenant.Email, subject, body);

            await _notify.AddNotification(
                tenant.CompanyId,
                tenant.PropertyTenants.FirstOrDefault()?.PropertyTenantId ?? Guid.Empty,
                "Email",
                body,
                tenant.TenantId
            );

            invoice.ReminderSent = true;
            await _invoiceRepo.UpdateInvoice(invoice);
        }
    }

    public async Task GenerateInvoice()
    {
        var activeTenants = await _propassRepo.GetActiveTenants();

        foreach (var tenant in activeTenants)
        {
            await _transaction.ExecuteAsync(async () =>
            {
                if (tenant == null)
                    return;

                var plan = await _propassRepo.PlanById(tenant.PlanId);
                if (plan == null)
                    return;

                var lastInvoice = await _invoiceRepo.GetLastInvoiceByTenantId(tenant.PropertyTenantId);
                DateTime referenceDate = lastInvoice?.DueDate ?? tenant.StartDate;

                if (DateTime.UtcNow.Date <= referenceDate.Date)
                    return;

                DateTime nextDueDate = plan.Frequency switch
                {
                    "Weekly"    => referenceDate.AddDays(7),
                    "Monthly"   => referenceDate.AddMonths(1),
                    "Quarterly" => referenceDate.AddMonths(3),
                    "Yearly"    => referenceDate.AddYears(1),
                    _           => referenceDate.AddMonths(1)
                };

                if (nextDueDate > tenant.EndDate)
                    return;

                var existingInvoice = await _invoiceRepo
                    .GetInvoiceByTenantAndDueDate(tenant.PropertyTenantId, nextDueDate);

                if (existingInvoice != null)
                    return;

                decimal rentAmount = tenant.installmentAmount > 0
                    ? tenant.installmentAmount
                    : tenant.Unit?.RentPrice ?? 0;

                if (rentAmount <= 0)
                    return;

                decimal amountPaid = 0;
                decimal balance = rentAmount;

                var tenantCredit = await _invoiceRepo.TenantCreditById(tenant.PropertyTenantId);
                if (tenantCredit != null && tenantCredit.Amount > 0)
                {
                    decimal creditToApply = Math.Min(rentAmount, tenantCredit.Amount);

                    amountPaid = creditToApply;
                    balance = rentAmount - creditToApply;

                    tenantCredit.Amount -= creditToApply;
                    await _invoiceRepo.UpdateTenantCredit(tenantCredit);
                }

                var invoice = new Invoice
                {
                    PropertyTenantId = tenant.PropertyTenantId,
                    CompanyId = tenant.CompanyId,
                    DueDate = nextDueDate,
                    Amount = rentAmount,
                    AmountPaid = amountPaid,
                    Balance = balance,
                    Status = balance == 0 ? InvoiceStatus.Paid : InvoiceStatus.UnPaid,
                    CreatedAt = DateTime.UtcNow,
                    LastPaymentDate = DateTime.UtcNow
                };

                await _invoiceRepo.AddInvoice(invoice);

                if (tenant.Property?.PropertyType == "WorkAndPay" &&
                    invoice.Status == InvoiceStatus.Paid)
                {
                    tenant.AmountDue -= rentAmount;
                    tenant.totalInstallments = Math.Max(tenant.totalInstallments - 1, 0);

                    await _propassRepo.UpdatePropertyTenant(tenant);
                }

                var subject = EmailTemplates.InvoiceGeneratedSubject();
                var body = EmailTemplates.InvoiceGeneratedBody(
                    tenant.Tenant.FullName,
                    invoice.Amount,
                    invoice.Balance,
                    invoice.DueDate
                );

                await _emailService.SendEmail(
                    tenant.Tenant.Email,
                    subject,
                    body
                );

                await _notify.AddNotification(
                    tenant.CompanyId,
                    tenant.PropertyTenantId,
                    "Email",
                    body,
                    tenant.TenantId
                );

                Console.WriteLine(
                    $"Generated invoice for {tenant.Tenant.FullName} due {nextDueDate:d}");
            });
        }
    }
}

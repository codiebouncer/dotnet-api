using Azure.Identity;
using Microsoft.VisualBasic;
using Propman.Constants;
using Propman.Repository;
using Propman.Services;
using Propman.Services.UserContext;
using PropMan.Models;

namespace Propman.Logic
{
    public class PropAssLogic:IPropAssLogic{
        private readonly IPropertyAssignmentRepository _propAssRepo;
        private readonly ITenantRepository _tenantRepo;
        private readonly IPropertyRepository _propRepo;
        private readonly IUserContext _userContext;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IEmailService _emailservice;

        public PropAssLogic(IPropertyAssignmentRepository propAssRepo,ITenantRepository tenantRepo,IPropertyRepository propRepo,IUserContext userContext,IInvoiceRepository invoiceRepo,IEmailService emailService)
        {
            _propAssRepo = propAssRepo;
            _tenantRepo = tenantRepo;
            _propRepo = propRepo;
            _userContext = userContext;
            _invoiceRepo = invoiceRepo;
            _emailservice = emailService;
        }
        public async Task RentAssignment(Guid planId, Guid tenantId, Guid unitId,Guid proptenantId)
        {
            try
            {
                var plan = await _propAssRepo.PlanById(planId);
                if (plan == null)
                    return;
                {
                    

                }
            var tenant = await _propAssRepo.TenantById(proptenantId);
    
                
            var unit = await _propRepo.GetUnitById(unitId);
            var rentAmount = unit!.RentPrice;
            var dueDate = tenant!.StartDate;
            dueDate = plan.Frequency switch
            {
                "Weekly" => dueDate.AddDays(7),
                "Monthly" => dueDate.AddMonths(1),
                "Quarterly" => dueDate.AddMonths(3),
                "Yearly" => dueDate.AddYears(1),
                _ => dueDate.AddMonths(1)
            };

            var invoice = new Invoice
            {
                PropertyTenantId = proptenantId,
                CompanyId = tenant.CompanyId,
                DueDate = dueDate,
                Amount = rentAmount,
                AmountPaid = 0,
                Balance = rentAmount,
                Status = InvoiceStatus.UnPaid,
                LastPaymentDate = DateTime.UtcNow

            };
            await _invoiceRepo.AddInvoice(invoice);
            await _emailservice.SendEmail(tenant.Tenant.Email,  "Rent Assignment", $"<h2>Welcome to Business</h2><p>You have been assigned to {unit.UnitName} with a {plan.Frequency} rent of {unit.RentPrice}  </p>");
            }
            catch (Exception ex)
            {

                var message = ex.Message;
            }
            
        }

        public async Task<(decimal outstanding, int totalInstallments,decimal installmentAmount)> WorkAndPay(
            Guid propId,
            decimal initdeposit,
            Guid proptenantId,
            Guid planId,
            DateTime StartDate,
            DateTime EndDate


            )
        {
            var property = await _propRepo.GetPropertyById(propId);
            var totalPrice = property!.SellingPrice;
            var deposit = initdeposit;
            var outstanding = totalPrice - deposit;
            var plan = await _propAssRepo.PlanById(planId);
            var endDate = EndDate;
            var startDate = StartDate;
            int totalInstallments = plan!.Frequency switch
            {
                "Weekly" => (int)Math.Ceiling((endDate - startDate).TotalDays / 7),
                "Monthly" => ((endDate.Year - startDate.Year) * 12) + (endDate.Month - startDate.Month),
                "Quarterly" => (((endDate.Year - startDate.Year) * 12) + (endDate.Month - startDate.Month)) / 3,
                "Yearly" => Math.Max(endDate.Year - startDate.Year, 1),
                _ => ((endDate.Year - startDate.Year) * 12) + (endDate.Month - startDate.Month)
            };

            // 3️⃣ Prevent divide-by-zero
            if (totalInstallments <= 0)
                totalInstallments = 1;

            // 4️⃣ Calculate amount per installment
            decimal installmentAmount =Math.Round((outstanding ?? 0m) / totalInstallments, 2);

            // 5️⃣ Determine the *next due date* only (not all)
            DateTime nextDueDate = plan.Frequency switch
            {
                "Weekly" => startDate.AddDays(7),
                "Monthly" => startDate.AddMonths(1),
                "Quarterly" => startDate.AddMonths(3),
                "Yearly" => startDate.AddYears(1),
                _ => startDate.AddMonths(1)
            };

            // 6️⃣ Return tuple (for service logic to handle next generation)
            // return (totalInstallments, installmentAmount, nextDueDate);
            var invoice = new Invoice
            {
                PropertyTenantId = proptenantId,
                CompanyId = Guid.Parse(_userContext.CompanyId!),
                DueDate = nextDueDate,
                Amount = installmentAmount,
                AmountPaid = 0,
                Balance = installmentAmount,
                Status = InvoiceStatus.UnPaid,
                LastPaymentDate = DateTime.UtcNow

            };
            await _invoiceRepo.AddInvoice(invoice);

            return (outstanding ?? 0m, totalInstallments, installmentAmount);
            

        }
        
public async Task WorkAndPay2(
            Guid proptenantId,
            Guid planId,
            DateTime StartDate


            )
        {
            var plan = await _propAssRepo.PlanById(planId);
            var startDate = StartDate;
            var proptenant = await _propAssRepo.GetById(proptenantId);

        // 5️⃣ Determine the *next due date* only (not all)
        DateTime nextDueDate = plan!.Frequency switch
        {
            "Weekly" => startDate.AddDays(7),
            "Monthly" => startDate.AddMonths(1),
            "Quarterly" => startDate.AddMonths(3),
            "Yearly" => startDate.AddYears(1),
            _ => startDate.AddMonths(1)
        };

            // 6️⃣ Return tuple (for service logic to handle next generation)
            // return (totalInstallments, installmentAmount, nextDueDate);
            var invoice = new Invoice
            {
                PropertyTenantId = proptenantId,
                CompanyId = Guid.Parse(_userContext.CompanyId!),
                DueDate = nextDueDate,
                Amount = proptenant!.installmentAmount,
                AmountPaid = 0,
                Balance = proptenant.installmentAmount,
                Status = InvoiceStatus.UnPaid,
                LastPaymentDate = DateTime.MinValue

            };
            await _invoiceRepo.AddInvoice(invoice);

        }

    }
    }
    
    



using Azure;
using Documan.Services;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Pqc.Crypto.Ntru;
using Propman.Constants;
using Propman.Repository;
using Propman.Services;
using Propman.Services.UserContext;
using PropMan.Map;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;
using PropMan.Services.AuditLogService;

namespace PropMan.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepo;
        private readonly IUserContext _userContext;
        private readonly IAuditLogService _auditLog;
        private readonly IUserRepository _userRepo;
        private readonly IPropertyAssignmentRepository _propassRepo;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IPropertyRepository _propRepo;
        private readonly IEmailService _email;
        private readonly INotificationService _notify;
        private readonly ITransactionService _transaction;
        public TenantService(
            ITenantRepository tenantRepo,
            IEmailService email,
            IUserContext userContext,
            IAuditLogService auditLog,
            IUserRepository userRepo,
            IPropertyAssignmentRepository propassRepo,
            IInvoiceRepository invoiceRepo,
            IPropertyRepository propRepo,
            INotificationService notify,
            ITransactionService transaction
            )
        {
            _tenantRepo = tenantRepo;
            _userContext = userContext;
            _auditLog = auditLog;
            _userRepo = userRepo;
            _propassRepo = propassRepo;
            _invoiceRepo = invoiceRepo;
            _propRepo = propRepo;
            _email = email;
            _notify = notify;
            _transaction = transaction;
        }
        public async Task<ApiResponse<IActionResult>> CreateTenant(CreateTenant request)
        {
            try
            {
                if (await _tenantRepo.TenantExists(request.NationalId, request.FullName))
                    return new ApiResponse<IActionResult>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.tenexists
                    };






                var compId = Guid.Parse(_userContext.CompanyId);
                var user = await _userRepo.GetUser(_userContext.UserName);

                var tenant = new Tenant
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = true,
                    CompanyId = compId,
                    NationalId = request.NationalId

                };
                await _tenantRepo.AddTenant(tenant);
                await _auditLog.Log(
                    tenant.TenantId,
                    $"{user.Name} created a tenant"

                );
                return new ApiResponse<IActionResult>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.tencreate
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IActionResult>
                {
                    Status = (int)StatusCode.Success,
                    Message = ex.Message,
                };

            }




        }
        public async Task<ApiResponse<IActionResult>> DeleteTenant(Guid id)
        {
            try
            {
                var payinvoice = await _invoiceRepo.InvoiceUnpaid(id);
                if (payinvoice)
                {
                    return new ApiResponse<IActionResult>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.invoiceunpaid
                    };
                }
                var tenant = await _tenantRepo.GetById(id);
                var compid = Guid.Parse(_userContext.CompanyId);
                if (tenant.CompanyId != compid)
                {
                    return new ApiResponse<IActionResult>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.tennotfound
                    };
                }
                var delete = await _tenantRepo.Delete(id);

                var activeassignment = await _propassRepo.TenantActive(id);
                if (activeassignment)
                {
                    return new ApiResponse<IActionResult>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.tenactive
                    };
                }

                var user = await _userRepo.GetUser(_userContext.UserName);
                if (!delete)
                {
                    return new ApiResponse<IActionResult>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.tennotfound
                    };
                }
                await _auditLog.Log(
                    id,
                    $"{user.Name} deleted a tenant"
                );
                return new ApiResponse<IActionResult>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.delsuccess
                };

            }
            catch (Exception ex)
            {
                return new ApiResponse<IActionResult>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };


            }

        }
        public async Task<ApiResponse<FetchDataRecords<TenantDto>>> GetAllTens(int page, int pageSize,string? search,DateTime? startDate,
    DateTime? endDate)
        {
            try
            {
                var result = await _tenantRepo.GetAllTenants(page, pageSize,search,startDate,endDate);


                var pagresult = new FetchDataRecords<TenantDto>
                {
                    Data = result.Records,
                    TotalCount = result.TotalCount,
                    Page = page,
                    PageSize = pageSize
                };
                return new ApiResponse<FetchDataRecords<TenantDto>>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.tenretsuccess,
                    Data = pagresult

                };
        

            }
            catch (Exception ex)
            {

                return new ApiResponse<FetchDataRecords<TenantDto>>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message

                };
            }



        }
        public async Task<ApiResponse<Tenant>> UpdateTenant(UpdateTenant request)
        {
            try
            {
                var tenant = await _tenantRepo.GetById(request.TenantId);
                if (tenant == null)
                {
                    return new ApiResponse<Tenant>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.tennotfound
                    };
                }
                tenant.FullName = request.FullName ?? tenant.FullName;
                tenant.PhoneNumber = request.PhoneNumber ?? tenant.PhoneNumber;
                tenant.Email = request.Email ?? tenant.Email;
                tenant.NationalId = request.NationalId ?? tenant.NationalId;
                var user = await _userRepo.GetUser(_userContext.UserName);

                await _tenantRepo.UpdateTenant(tenant);
                await _auditLog.Log(
                    tenant.TenantId,
                    $"{user.Name} has updated a tenant"

                );
                return new ApiResponse<Tenant>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.tenupdatesuccess
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Tenant>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message
                };
            }

        }
       public async Task<ApiResponse<Invoice>> MakePayment(PaymentRequest request)
{
    try
    {
        var invoices = await _invoiceRepo.GetById(request.InvoiceId);
        var proptenant = await _propassRepo.GetById(request.PropertyTenantId);
        var plan = await _propassRepo.PlanById(proptenant!.PlanId);

        if (invoices.Status == InvoiceStatus.Paid)
        {
            return new ApiResponse<Invoice>
            {
                Status = (int)StatusCode.ValidationError,
                Message = ConstantVariable.invoicepaid
            };
        }

        if (request.PaymentAmount <= 0)
        {
            return new ApiResponse<Invoice>
            {
                Status = (int)StatusCode.ValidationError,
                Message = ConstantVariable.invalidamount
            };
        }

        var property = await _propRepo.GetPropertyById(proptenant.PropertyId);
        var tenant = await _tenantRepo.GetById(request.TenantId);

       
        await _transaction.ExecuteAsync(async () =>
        {
            decimal amountToApply = Math.Min(request.PaymentAmount, invoices.Balance);

            invoices.AmountPaid += amountToApply;
            invoices.Balance -= amountToApply;
            invoices.LastPaymentDate = DateTime.UtcNow;

          
            if (invoices.Balance <= 0)
                invoices.Status = InvoiceStatus.Paid;
            else if (invoices.AmountPaid == 0)
                invoices.Status = InvoiceStatus.UnPaid;
            else
                invoices.Status = InvoiceStatus.InProgress;

            await _invoiceRepo.UpdateInvoice(invoices);

            // Add payment record
            var payment = new Payment
            {
                InvoiceId = invoices.InvoiceId,
                PropertyTenantId = request.PropertyTenantId,
                CompanyId = Guid.Parse(_userContext.CompanyId),
                AmountPaid = request.PaymentAmount,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod
            };

            await _invoiceRepo.AddPayment(payment);

            // 🔄 Overpayment logic
            decimal overpaid = request.PaymentAmount - amountToApply;

            if (overpaid > 0)
            {
                await _invoiceRepo.CreateTenantCredit(request.PropertyTenantId, overpaid);
            }

            // 🔄 Work & Pay Logic
            if (property?.PropertyType == ConstantVariable.wp && invoices.Status == InvoiceStatus.UnPaid)
            {
                proptenant.AmountDue -= invoices.Amount;
                proptenant.totalInstallments = Math.Max(proptenant.totalInstallments - 1, 0);

                await _propassRepo.UpdatePropertyTenant(proptenant);

                await _auditLog.Log(
                    invoices.InvoiceId,
                    $"WorkAndPay update: {tenant.FullName}'s AmountDue reduced by {invoices.Amount:C}. Remaining: {proptenant.AmountDue:C}"
                );
            }
        });
        // 🔥 TRANSACTION COMPLETED HERE

        // These occur AFTER transaction (so failures here don’t rollback payment)
        await _email.SendEmail(
            tenant.Email,
            "Payment Receipt",
            $"A payment of {request.PaymentAmount} has been received from {tenant.FullName}"
        );

        await _notify.AddNotification(
            tenant.CompanyId,
            proptenant.PropertyTenantId,
            "Email",
            $"A payment of {request.PaymentAmount} has been received from {tenant.FullName}",
            tenant.TenantId
        );

        return new ApiResponse<Invoice>
        {
            Status = (int)StatusCode.Success,
            Message = ConstantVariable.paysuccess,
            Data = invoices
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<Invoice>
        {
            Status = (int)StatusCode.SystemError,
            Message = ex.Message
        };
    }
}

        public async Task<ApiResponse<List<PlanDto>>> AllPlans()
        {
            try
            {
                var result = await _tenantRepo.AllPlans();
                return new ApiResponse<List<PlanDto>>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.planret,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<PlanDto>>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message,
                };
            }
        }
        public async Task<ApiResponse<List<PropertyTenant>>> AllPropTens()
        {
            try
            {
                var result = await _propassRepo.AllPropTens();
                return new ApiResponse<List<PropertyTenant>>
                {
                    Status = (int)StatusCode.ValidationError,
                    Message = "Successful",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<PropertyTenant>>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message,
                };
            }
        }
        public async Task<ApiResponse<List<Invoice>>> InvoiceByPropTenId(Guid proptenantId)
        {
            try
            {
                var result = await _invoiceRepo.InvoiceByPropTenantId(proptenantId);
                return new ApiResponse<List<Invoice>>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.invoiceretrieved,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Invoice>>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message,
                };
            }
        }
        public async Task<ApiResponse<List<Payment>>> AllPaymentsById(Guid proptenantId)
        {
            try
            {
                var result = await _invoiceRepo.AllPaymentsById(proptenantId);
                return new ApiResponse<List<Payment>>
                {
                    Status = (int)StatusCode.ValidationError,
                    Message = ConstantVariable.payret,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Payment>>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message,
                };
            }
        }
        public async Task<ApiResponse<List<Payment>>> AllPayments()
        {
            try
            {
                var result = await _invoiceRepo.AllPayments();
                return new ApiResponse<List<Payment>>
                {
                    Status = (int)StatusCode.ValidationError,
                    Message = ConstantVariable.payret,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Payment>>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ApiResponse<FetchDataRecords<AllPayments>>> GetTenantPayments(
    Guid tenantId, int page,int pageSize,DateTime? startDate,DateTime? endDate)
{
    try
    {
        var fetched = await _tenantRepo.GetPaymentsByTenantId(
            tenantId,
            page,
            pageSize,
            startDate,
            endDate
        );

        if (fetched.TotalCount == 0)
        {
            return new ApiResponse<FetchDataRecords<AllPayments>>
            {
                Status = (int)StatusCode.ValidationError,
                Message = "No payments made yet."
            };
        }

        var mapped = fetched.Data.Select(p => p.ToDto()).ToList();

        var result = new FetchDataRecords<AllPayments>
        {
            TotalCount = fetched.TotalCount,
            Page = fetched.Page,
            PageSize = fetched.PageSize,
            Data = mapped
        };

        return new ApiResponse<FetchDataRecords<AllPayments>>
        {
            Status = (int)StatusCode.Success,
            Message = "Payments retrieved successfully",
            Data = result
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<FetchDataRecords<AllPayments>>
        {
            Status = (int)StatusCode.SystemError,
            Message = ex.Message
        };
    }
}

        public async Task<ApiResponse<FetchDataRecords<InvoiceDto>>> GetTenantInvoices(
    Guid tenantId,int page,int pageSize,DateTime? startDate,DateTime? endDate)
{
    try
    {
        var fetched = await _tenantRepo.GetInvoicesByTenantId(
            tenantId,
            page,
            pageSize,
            startDate,
            endDate
        );

        if (fetched.TotalCount == 0)
        {
            return new ApiResponse<FetchDataRecords<InvoiceDto>>
            {
                Status = (int)StatusCode.ValidationError,
                Message = "No invoices found."
            };
        }

        var mapped = fetched.Data.Select(i => i.ToDto()).ToList();

        var result = new FetchDataRecords<InvoiceDto>
        {
            TotalCount = fetched.TotalCount,
            Page = fetched.Page,
            PageSize = fetched.PageSize,
            Data = mapped
        };

        return new ApiResponse<FetchDataRecords<InvoiceDto>>
        {
            Status = (int)StatusCode.Success,
            Message = "Success",
            Data = result
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<FetchDataRecords<InvoiceDto>>
        {
            Status = (int)StatusCode.SystemError,
            Message = ex.Message
        };
    }
}
public async Task<ApiResponse<List<TenantDropdownDto>>>TenantDropdown()
        {
            var res = await _tenantRepo.GetTenants();

            return new ApiResponse<List<TenantDropdownDto>>
            {
                Status = (int)StatusCode.Success,
                Message ="Retrieved",
                Data = res
            };


        }

            
        }
            
        }
    
    

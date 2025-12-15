using Documan.Services;
using Microsoft.Identity.Client;
using Propman.Constants;
using Propman.Repository;
using PropMan.Models;
using PropMan.Payloads;

namespace Propman.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly ITenantRepository _tenant;

        public PaymentService(IInvoiceRepository invoiceRepo,ITenantRepository tenant)
        {
            _invoiceRepo = invoiceRepo;
            _tenant = tenant;
        }

        public async Task<ApiResponse<List<Invoice>>> GetAllInvoices()
        {
            try
            {
                var result = await _invoiceRepo.GetAllInvoices();
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
                Message = ex.Message
            };
            }
            
        }
        public async Task<ApiResponse<FetchDataRecords<Payment>>> AllPayments(int page,int pageSize,DateTime? startDate,DateTime? endDate)
        {
            var res = await _tenant.GetAllPayments(page,pageSize,startDate,endDate);

            if (res.TotalCount == 0)
        {
            return new ApiResponse<FetchDataRecords<Payment>>
            {
                Status = (int)StatusCode.ValidationError,
                Message = "No payments found."
            };
        }
         var result = new FetchDataRecords<Payment>
        {
            TotalCount = res.TotalCount,
            Page = res.Page,
            PageSize = res.PageSize,
            Data = res.Data
        };

        return new ApiResponse<FetchDataRecords<Payment>>
        {
            Status = (int)StatusCode.Success,
            Message = "Success",
            Data = result
        };
        }
    }
}
using PropMan.Models;
using PropMan.Payloads;

namespace Propman.Services
{
    public interface IPaymentService
    {
        Task<ApiResponse<FetchDataRecords<Payment>>> AllPayments(int page,int pageSize,DateTime? startDate,DateTime? endDate);
    }
}
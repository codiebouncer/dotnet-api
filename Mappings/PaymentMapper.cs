using PropMan.Models;
using PropMan.Models.Dto;

namespace PropMan.Map
{
    public static class PaymentMapper
{
    public static AllPayments ToDto(this Payment i)
    {
        return new AllPayments
        {
            InvoiceId = i.InvoiceId,
            PropertyTenantId = i.PropertyTenantId,
            CompanyId = i.CompanyId,
            PaymentId = i.PaymentId,
            AmountPaid = i.AmountPaid,
            PaymentDate = i.PaymentDate,
            PaymentMethod = i.PaymentMethod,
            CreatedAt = i.CreatedAt


        };
    }
}

}
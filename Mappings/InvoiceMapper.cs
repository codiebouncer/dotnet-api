using PropMan.Models;
using PropMan.Models.Dto;

namespace PropMan.Map
{
    public static class InvoiceMapper
{
    public static InvoiceDto ToDto(this Invoice i)
    {
        return new InvoiceDto
        {
            InvoiceId = i.InvoiceId,
            PropertyTenantId = i.PropertyTenantId,
            CompanyId = i.CompanyId,
            InvoiceDate = i.InvoiceDate,
            DueDate = i.DueDate,
            Amount = i.Amount,
            AmountPaid = i.AmountPaid,
            Balance = i.Balance,
            Status = i.Status,
            ReminderSent = i.ReminderSent,
            LastPaymentDate = i.LastPaymentDate,
            CreatedAt = i.CreatedAt
        };
    }
}

}
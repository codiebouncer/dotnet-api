namespace PropMan.Models.Dto
{
    public class AllPayments
    {
         public Guid PaymentId { get; set; } = Guid.NewGuid();

    public Guid InvoiceId { get; set; }

    public Guid PropertyTenantId { get; set; }

    public Guid CompanyId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
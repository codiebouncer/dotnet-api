namespace PropMan.Models.Dto
{
    public class CreativeInvoice
    {
    public Guid PropertyTenantId { get; set; }

    public Guid CompanyId { get; set; }

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }

    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }

    public string Status { get; set; } = null!;

    public DateTime LastPaymentDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual PropertyTenant PropertyTenant { get; set; } = null!;
    }
}
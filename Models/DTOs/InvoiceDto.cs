namespace PropMan.Models.Dto
{
    public class InvoiceDto
{
    public Guid InvoiceId { get; set; }
    public Guid PropertyTenantId { get; set; }
    public Guid CompanyId { get; set; }

    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }

    public string Status { get; set; }
    public bool ReminderSent { get; set; }
    public DateTime? LastPaymentDate { get; set; }

    public DateTime CreatedAt { get; set; }
}

}
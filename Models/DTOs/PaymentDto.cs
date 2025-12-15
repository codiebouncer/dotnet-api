namespace PropMan.Models.Dto
{
     public class PaymentDto
{
    public Guid PaymentId { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid CompanyId { get; set; }

    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Method { get; set; }

    public DateTime CreatedAt { get; set; }
}

}
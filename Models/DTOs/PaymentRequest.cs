namespace PropMan.Models.Dto
{
    public class PaymentRequest
    {
        public Guid InvoiceId { get; set; }
        public Guid TenantId { get; set; }
        public Guid PropertyTenantId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
    }
}
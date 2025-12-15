namespace PropMan.Models.Dto
{
    public class RepairComplete
    {
        public Guid RepairId { get; set; }
        public Guid PropertyTenantId{ get; set; }
        public string Status { get; set; }
        public DateTime CompletedDate{ get; set; }
    }
}
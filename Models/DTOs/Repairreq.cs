namespace PropMan.Models.Dto
{
    public class Repairreq
    {
        public Guid PropertyId { get; set; }
        
    public string Description { get; set; } = null!;

    public decimal? Cost { get; set; }

    

    }
}
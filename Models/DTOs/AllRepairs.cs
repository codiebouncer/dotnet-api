namespace PropMan.Models.Dto
{
     public class AllRepairs
    {
        public string? Name {get; set;}
            public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal? Cost { get; set; }
    public DateTime CompletedDate { get; set; }
    }
}
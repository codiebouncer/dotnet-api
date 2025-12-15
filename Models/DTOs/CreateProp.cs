namespace PropMan.Models.Dto
{
    public class CreateProperty
    {
        public string? PropertyName { get; set; }
        public string? PropertyType { get; set; } 
        public string? Location { get; set; }
        public int TotalUnits { get; set; }
        public decimal CostPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? OccupancyStatus { get; set; }
        public List<IFormFile>? Pictures { get; set; }

        


    }
}
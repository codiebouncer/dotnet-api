namespace PropMan.Models.Dto
{
    public class UpdateProperty
    {
        public Guid PropertyId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyType { get; set; }
        public string? Location { get; set; }
        public int TotalUnits { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? OccupancyStatus { get; set; }
        public List<IFormFile>? NewPictures { get; set; } = new();
        public List<Guid>? PicturesToRemove { get; set; } = new();
    }
}
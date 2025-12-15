namespace PropMan.Models.Dto
{
    public class UpdateUnit
    {
        public Guid UnitId{ get; set; }
        public string UnitName { get; set; } = null!;

    public string Description { get; set; } = null!;

        public string Status { get; set; } = null!;

    public decimal RentPrice { get; set; }
    }
}
namespace PropMan.Models.Dto
{
    public class PropTen
    {

    public Guid? UnitId { get; set; }

    public Guid PropertyId { get; set; }

    public Guid TenantId { get; set; }

    public Guid PlanId { get; set; }

    public DateTime StartDate { get; set; }

    public int DurationMonths { get; set; }

    public decimal? installmentAmount { get; set; }
    public int InitialDeposit{ get; set; }



    public bool IsPrimaryTenant { get; set; }    
    }
}
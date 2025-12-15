namespace PropMan.Models.Dto
{
    public class PropertyTenantDto
{
    public Guid PropertyTenantId { get; set; }
    public Guid? UnitId { get; set; }
    public Guid PropertyId { get; set; }
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid PlanId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public decimal InstallmentAmount { get; set; }
    public int TotalInstallments { get; set; }

    public string Status { get; set; }
    public decimal? AmountDue { get; set; }
    public decimal InitialDeposit { get; set; }
    public bool IsActive { get; set; }

    public DateTime? LastPaymentDate { get; set; }
    public bool IsPrimaryTenant { get; set; }

    public DateTime CreatedAt { get; set; }
}

}
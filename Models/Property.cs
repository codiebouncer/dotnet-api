using System;
using System.Collections.Generic;

namespace PropMan.Models;

public partial class Property
{
    public Guid PropertyId { get; set; } = Guid.NewGuid();

    public Guid CompanyId { get; set; }

    public string PropertyName { get; set; } = null!;

    public string PropertyType { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int TotalUnits { get; set; }

    public int OccuppiedUnits { get; set; }

    public int VacantUnits { get; set; }

    public decimal CostPrice { get; set; }

    public decimal? SellingPrice { get; set; }

    public string OccupancyStatus { get; set; } = null!;

    public bool IsActive { get; set; }

    public  Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } 

    public virtual Company Company { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Picture> Pictures { get; set; } = new List<Picture>();

    public virtual ICollection<PropertyTenant> PropertyTenants { get; set; } = new List<PropertyTenant>();

    public virtual ICollection<PropertyUnit> PropertyUnits { get; set; } = new List<PropertyUnit>();
    public virtual ICollection<Repair> Repairs {get; set; } = new List<Repair>();
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PropMan.Models;

public partial class PropertyUnit
{
    public Guid UnitId { get; set; } = Guid.NewGuid();

    public Guid PropertyId { get; set; }

    public Guid CompanyId { get; set; }

    public string UnitName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal RentPrice { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Company Company { get; set; } = null!;
    [JsonIgnore]
    public virtual Property Property { get; set; } = null!;

    public virtual ICollection<PropertyTenant> PropertyTenants { get; set; } = new List<PropertyTenant>();
}

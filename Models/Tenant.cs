using System;
using System.Collections.Generic;

namespace PropMan.Models;

public partial class Tenant
{
    public Guid TenantId { get; set; }

    public Guid CompanyId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsActive { get; set; }

    public string NationalId { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<PropertyTenant> PropertyTenants { get; set; } = new List<PropertyTenant>();
    
}

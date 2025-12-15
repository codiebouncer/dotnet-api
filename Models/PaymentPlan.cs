using System;
using System.Collections.Generic;

namespace PropMan.Models;

public partial class PaymentPlan
{
    public Guid PlanId { get; set; }


    public string Frequency { get; set; } = null!;


    public virtual ICollection<PropertyTenant> PropertyTenants { get; set; } = new List<PropertyTenant>();
}

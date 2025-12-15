using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PropMan.Models;

public partial class PropertyTenant
{
    public Guid PropertyTenantId { get; set; } = Guid.NewGuid();

    public Guid? UnitId { get; set; }

    public Guid PropertyId { get; set; }

    public Guid TenantId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid PlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
    public decimal installmentAmount { get; set; }
    public int totalInstallments{ get; set; }

    public string Status { get; set; } = null!;

    public decimal? AmountDue { get; set; }
    public decimal InitialDeposit{ get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime? LastPaymentDate { get; set; } = DateTime.Now;

    public bool IsPrimaryTenant { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Company Company { get; set; } = null!;
  
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual PaymentPlan Plan { get; set; } = null!;

    public virtual Property Property { get; set; } = null!;

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual PropertyUnit? Unit { get; set; } 
}

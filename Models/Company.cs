using System;
using System.Collections.Generic;

namespace PropMan.Models;

public partial class Company
{
    public Guid CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PaymentPlan> PaymentPlans { get; set; } = new List<PaymentPlan>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<PropertyTenant> PropertyTenants { get; set; } = new List<PropertyTenant>();

    public virtual ICollection<PropertyUnit> PropertyUnits { get; set; } = new List<PropertyUnit>();

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();

    public virtual ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

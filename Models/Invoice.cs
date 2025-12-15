using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropMan.Models;

public partial class Invoice
{
    public Guid InvoiceId { get; set; } = Guid.NewGuid();

    public Guid PropertyTenantId { get; set; }

    public Guid CompanyId { get; set; }

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }

    public decimal AmountPaid { get; set; }
    public decimal Balance { get; set; }

    public string Status { get; set; } = null!;
    public bool ReminderSent { get; set; } = false;

    public DateTime LastPaymentDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey(nameof(PropertyTenantId))]
    public virtual PropertyTenant PropertyTenant { get; set; } 
}

using System;
using System.Collections.Generic;

namespace PropMan.Models;

public partial class Payment
{
    public Guid PaymentId { get; set; } = Guid.NewGuid();

    public Guid InvoiceId { get; set; }

    public Guid PropertyTenantId { get; set; }

    public Guid CompanyId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Company Company { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual PropertyTenant PropertyTenant { get; set; } = null!;
}

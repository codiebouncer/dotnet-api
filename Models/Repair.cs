using System;
using System.Collections.Generic;

namespace PropMan.Models;

public partial class Repair
{
    public Guid RepairId { get; set; } = Guid.NewGuid();

    public Guid PropertyId { get; set; }

    public Guid CompanyId { get; set; }

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal? Cost { get; set; }

    public DateTime CompletedDate { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Company Company { get; set; } = null!;
    public virtual Property Property { get; set; } = null!;

}

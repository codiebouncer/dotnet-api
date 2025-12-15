using System;
using System.Collections.Generic;

namespace PropMan.Models;

public partial class AuditLog
{
    public Guid LogId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid? UserId { get; set; }

    public Guid EntityId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Company Company { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

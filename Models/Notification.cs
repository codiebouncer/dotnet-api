using System;
using System.Collections.Generic;

namespace PropMan.Models;

public partial class Notification
{
    public Guid NotificationId { get; set; } = Guid.NewGuid();

    public Guid CompanyId { get; set; }

    public Guid PropertyTenantId { get; set; }

    public string MessageType { get; set; } = null!;

    public string MessageContent { get; set; } = null!;

    public Guid SentTo { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public virtual Company Company { get; set; } = null!;

    public virtual PropertyTenant PropertyTenant { get; set; } = null!;
}

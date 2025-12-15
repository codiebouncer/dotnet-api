using System;
using System.Collections.Generic;

namespace PropMan.Models;

public partial class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();

    public Guid CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public string Role { get; set; } = null!;

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}

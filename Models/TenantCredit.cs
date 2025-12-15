using System;

namespace PropMan.Models
{
    public class TenantCredit
    {
        public Guid TenantCreditId { get; set; } = Guid.NewGuid();

        public Guid PropertyTenantId { get; set; }
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual PropertyTenant PropertyTenant { get; set; } = null!;
    }
}

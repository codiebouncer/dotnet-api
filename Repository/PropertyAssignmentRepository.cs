using Microsoft.EntityFrameworkCore;
using PropMan.Models;

namespace Propman.Repository
{
    public class PropertyAssignmentRepository : IPropertyAssignmentRepository
    {
        private readonly DataContext _context;
        public PropertyAssignmentRepository(DataContext context)
        {
            _context = context;
        }

        // ✅ Returns true if the tenant has any active property assignments
        public async Task<bool> TenantActive(Guid tenantId)
        {
            return await _context.PropertyTenants
                .AnyAsync(pt => pt.TenantId == tenantId && pt.IsActive);
        }

        // ✅ Returns true if the property has any active assignments
        public async Task<bool> PropertyActive(Guid propertyId)
        {
            return await _context.PropertyTenants
                .AnyAsync(pt => pt.PropertyId == propertyId && pt.IsActive);
        }

        public async Task<PaymentPlan?> PlanById(Guid planId)
        {
            return await _context.PaymentPlans
                .FirstOrDefaultAsync(p => p.PlanId == planId);
        }

        public async Task<PropertyTenant?> TenantById(Guid tenantId)
        {
            return await _context.PropertyTenants
                .Where(pt => pt.IsActive)
                .FirstOrDefaultAsync(pt => pt.PropertyTenantId == tenantId);
        }

        public async Task<PropertyTenant?> GetById(Guid propertyTenantId)
        {
            return await _context.PropertyTenants
                .Include(pt => pt.Unit)
                .Include(pt => pt.Tenant)
                .Include(pt => pt.Plan)
                .Where(pt => pt.IsActive)
                .FirstOrDefaultAsync(pt => pt.PropertyTenantId == propertyTenantId);
        }

        public async Task AddPropertyTenant(PropertyTenant propertyTenant)
        {
            propertyTenant.IsActive = true; // ensure new assignment is active
            await _context.PropertyTenants.AddAsync(propertyTenant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePropertyTenant(PropertyTenant propertyTenant)
        {

            _context.PropertyTenants.Update(propertyTenant);
            await _context.SaveChangesAsync();
        }

        // ✅ Get all active tenants with their related entities
        public async Task<IEnumerable<PropertyTenant>> GetActiveTenants()
        {
            return await _context.PropertyTenants
                .Include(pt => pt.Unit)
                .Include(pt => pt.Plan)
                .Include(pt => pt.Tenant)
                .Where(pt => pt.IsActive && pt.EndDate > DateTime.UtcNow)
                .ToListAsync();
        }
        public async Task<List<PropertyTenant>> AllPropTens()
        {
            return await _context.PropertyTenants
            .Where(x => x.IsActive == true)
            .Include(x=> x.Invoices)
            .Include(x=> x.Payments)
            .ToListAsync();
        }
    }
}

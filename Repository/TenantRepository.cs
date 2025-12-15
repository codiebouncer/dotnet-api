using System.Formats.Tar;
using Microsoft.EntityFrameworkCore;
using Propman.Services.UserContext;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;
using PropMan.Services.UserContext;

namespace Propman.Repository
{
    public class TenantRepository : ITenantRepository
    {
        private readonly DataContext _context;
        private readonly IUserContext _userContext;

        public TenantRepository(DataContext context,IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task AddTenant(Tenant tenant)
        {
            tenant.TenantId = Guid.NewGuid();
            tenant.IsActive = true; // ensure new tenants are active
            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();
        }

        public async Task<Tenant?> GetTenant(string tenantName)
        {
            return await _context.Tenants
                .Where(t => t.IsActive) // only active tenants
                .FirstOrDefaultAsync(u => u.FullName == tenantName);
        }

        public async Task<bool> TenantExists(string nationalId, string tenantName)
        {
            return await _context.Tenants
                .Where(t => t.IsActive)
                .AnyAsync(u => u.NationalId == nationalId || u.FullName == tenantName);
        }

        public async Task<Tenant?> GetById(Guid tenantId)
        {
            return await _context.Tenants
                .Where(t => t.IsActive)
                .Include(t => t.PropertyTenants.Where(pt => pt.IsActive == true)) 
                    .ThenInclude(pt => pt.Property)    
                .FirstOrDefaultAsync(t => t.TenantId == tenantId);
        }

      public async Task<(List<TenantDto> Records, int TotalCount)> GetAllTenants(
    int page,
    int pageSize,
    string? search,
    DateTime? startDate,
    DateTime? endDate
)
{
    var compId = Guid.Parse(_userContext.CompanyId!);

    var query = _context.Tenants
        .Where(u => u.CompanyId == compId && u.IsActive)
        .Include(t => t.PropertyTenants)
            .ThenInclude(pt => pt.Property)
        .Include(t => t.PropertyTenants)
            .ThenInclude(pt => pt.Unit)
        .AsQueryable();

    // 🔍 SEARCH — NAME
    if (!string.IsNullOrWhiteSpace(search))
    {
        string normalized = search.Trim().ToLower();
        query = query.Where(t => t.FullName.ToLower().Contains(normalized));
    }

    // 📅 DATE RANGE FILTER (APPLIED ON ACTIVE ASSIGNMENTS)
   if (startDate.HasValue)
{
    query = query.Where(t =>
        t.CreatedAt.Date >= startDate.Value.Date
    );
}

if (endDate.HasValue)
{
    query = query.Where(t =>
        t.CreatedAt.Date <= endDate.Value.Date
    );
}


    int totalCount = await query.CountAsync();

    var tenants = await query
        .OrderByDescending(t => t.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    // ✨ MAP TO DTO
    var tenantDtos = tenants.Select(t =>
    {
        var activeAssignment = t.PropertyTenants
            .Where(pt => pt.IsActive)
            .OrderByDescending(pt => pt.CreatedAt)
            .FirstOrDefault();

        return new TenantDto
        {
            TenantId = t.TenantId,
            FullName = t.FullName,
            Email = t.Email,
            PhoneNumber = t.PhoneNumber,
            IsActive = t.IsActive,
            NationalId = t.NationalId,

            IsAssigned = activeAssignment != null,
            PropertyTenantId = activeAssignment?.PropertyTenantId,
            PropertyName = activeAssignment?.Property?.PropertyName,
            UnitName = activeAssignment?.Unit?.UnitName
        };
    }).ToList();

    return (tenantDtos, totalCount);
}






        public async Task<bool> Delete(Guid id)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.TenantId == id && t.IsActive);
            if (tenant == null)
                return false;

            tenant.IsActive = false;
            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Tenant> UpdateTenant(Tenant tenant)
        {
            if (!tenant.IsActive)
                throw new InvalidOperationException("Cannot update an inactive tenant.");

            _context.Tenants.Update(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }

        public async Task AddNotification(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        // ✅ Get all notifications for an active tenant
        public async Task<IEnumerable<Notification>> GetNotificationsByTenant(Guid tenantId)
        {
            return await _context.Notifications
                .Where(n => n.SentTo == tenantId)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }

        // ✅ Get all notifications by company
        public async Task<IEnumerable<Notification>> GetAllNotifications(Guid companyId)
        {
            return await _context.Notifications
                .Where(n => n.CompanyId == companyId)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();
        }
        public async Task<List<PlanDto>> AllPlans()
        {
            return await _context.PaymentPlans
            .AsNoTracking()
            .Select(pp=> new PlanDto
            {
                PlanId = pp.PlanId,
                Name = pp.Frequency
            } )
            .ToListAsync();
        }
       public async Task<FetchDataRecords<Payment>> GetPaymentsByTenantId(Guid tenantId, int page, int pageSize,DateTime? startDate,DateTime? endDate)
{
    var query = _context.Payments
        .Include(p => p.Invoice)
        .Include(p => p.PropertyTenant)
        .Where(p => p.PropertyTenant.TenantId == tenantId);
         if (startDate.HasValue)
{
    query = query.Where(t =>
        t.CreatedAt.Date >= startDate.Value.Date
    );
}

if (endDate.HasValue)
{
    query = query.Where(t =>
        t.CreatedAt.Date <= endDate.Value.Date
    );
}

    int totalCount = await query.CountAsync();

    var payments = await query
        .OrderByDescending(p => p.PaymentDate)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new FetchDataRecords<Payment>
    {
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        Data = payments
    };
}

        public async Task<FetchDataRecords<Invoice>> GetInvoicesByTenantId(Guid tenantId, int page, int pageSize,DateTime? startDate,DateTime? endDate)
{
    var query = _context.Invoices
        .Include(i => i.PropertyTenant)
        .Where(i => i.PropertyTenant.TenantId == tenantId);
        if (startDate.HasValue)
{
    query = query.Where(t =>
        t.CreatedAt.Date >= startDate.Value.Date
    );
}

if (endDate.HasValue)
{
    query = query.Where(t =>
        t.CreatedAt.Date <= endDate.Value.Date
    );
}
    int totalCount = await query.CountAsync();

    var invoices = await query
        .OrderByDescending(i => i.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new FetchDataRecords<Invoice>
    {
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        Data = invoices
    };
}
    public async Task<FetchDataRecords<Payment>> GetAllPayments( int page, int pageSize,DateTime? startDate,DateTime? endDate)
        {
            var compId = Guid.Parse(_userContext.CompanyId!);
            var query = _context.Payments
                                .Where(p=> p.CompanyId==compId);

            if (startDate.HasValue)
{
    query = query.Where(t =>
        t.CreatedAt.Date >= startDate.Value.Date
    );
}

if (endDate.HasValue)
{
    query = query.Where(t =>
        t.CreatedAt.Date <= endDate.Value.Date
    );
}      
    
        int totalCount = await query.CountAsync(); 
        var payments= await query
                        .OrderByDescending(i => i.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(); 

        return new FetchDataRecords<Payment>
    {
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        Data = payments
    };             
        }
        public async Task<List<TenantDropdownDto>> GetTenants()
{
    return await _context.Tenants
        .AsNoTracking()
        .Select(t => new TenantDropdownDto
        {
            TenantId = t.TenantId,
            FullName = t.FullName
        })
        .ToListAsync();
}




    }

}


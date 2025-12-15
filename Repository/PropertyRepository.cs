using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using Propman.Services.UserContext;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;

namespace Propman.Repository
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly DataContext _context;
        private readonly IUserContext _userContext;
        public PropertyRepository(DataContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }
        public async Task<Property?> GetPropertyById(Guid propid)
        {
            return await _context.Properties.FirstOrDefaultAsync(p => p.PropertyId == propid);
        }
        public async Task<Property> UpdateProperty(Property property)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();
            return property;
        }
        public async Task<List<Property>> GetAllProperties()
        {
            var compId = Guid.Parse(_userContext.CompanyId!);
            return await _context.Properties
            .Where(p => p.CompanyId == compId && p.IsActive == true)
            .Include(p => p.Pictures)
            .ToListAsync();
        }
        public async Task<bool> PropertyExists(string propname)
        {
            var compId = Guid.Parse(_userContext.CompanyId!);
            return await _context.Properties.AnyAsync(u => u.PropertyName == propname && u.IsActive == true && u.CompanyId == compId);

        }

        public async Task AddProperty(Property property)
        {
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();
        }

        public async Task AddPicture(IEnumerable<Picture> pictures)
        {
            foreach (var picture in pictures)
            {


                _context.Pictures.Add(picture);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(Property property)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();



        }

        public async Task RemovePicturesAsync(IEnumerable<Guid> pictureIds)
        {
            var pics = await _context.Pictures
                .Where(p => pictureIds.Contains(p.PictureId))
                .ToListAsync();

            _context.Pictures.RemoveRange(pics);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UnitExists(string unitname)
        {
            return await _context.PropertyUnits.AnyAsync(u => u.UnitName == unitname);
        }
        public async Task<PropertyUnit?> GetUnitById(Guid propId)
        {
            return await _context.PropertyUnits.FirstOrDefaultAsync(p => p.UnitId == propId && p.IsActive==true);
        }
        public async Task AddUnit(PropertyUnit propertyunit)
        {
            await _context.PropertyUnits.AddAsync(propertyunit);
            await _context.SaveChangesAsync();
        }
        public async Task AddRepair(Repair repair)
        {
            await _context.Repairs.AddAsync(repair);
            await _context.SaveChangesAsync();
        }
        public async Task<Repair> UpdateRepair(Repair repair)
        {
            _context.Repairs.Update(repair);
            await _context.SaveChangesAsync();
            return repair;
        }
        public async Task<Repair?> GetRepairById(Guid repairId)
        {
            return await _context.Repairs.FirstOrDefaultAsync(r => r.RepairId == repairId);
        }
        public async Task<bool> UnitsFull(Guid propertyId)
        {

            var property = await _context.Properties
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId);


            var totalUnitsInDb = await _context.PropertyUnits
                .CountAsync(u => u.PropertyId == propertyId);


            return totalUnitsInDb == property!.TotalUnits;
        }
        public async Task<PropertyUnit> UpdateUnit(PropertyUnit propertyunit)
        {
            _context.PropertyUnits.Update(propertyunit);
            await _context.SaveChangesAsync();
            return propertyunit;
        }
        public async Task DeleteUnit(PropertyUnit propertyunit)
        {
            _context.PropertyUnits.Update(propertyunit);
            await _context.SaveChangesAsync();


        }
        public async Task<DashboardSummary> GetDashboardSummary()
        {
            var companyId = Guid.Parse(_userContext.CompanyId!);
            var summary = new DashboardSummary
            {
                TotalProperties = await _context.Properties
                    .CountAsync(p => p.CompanyId == companyId && p.IsActive),

                VacantUnits = await _context.PropertyUnits
                    .CountAsync(u => u.CompanyId == companyId && u.Status == "Vacant"),

                OccupiedUnits = await _context.PropertyUnits
                    .CountAsync(u => u.CompanyId == companyId && u.Status == "Occupied"),

                PendingRepairs = await _context.Repairs
                    .CountAsync(r => r.CompanyId == companyId && r.Status == "Pending")
            };
            summary.RecentProperties = await _context.Properties
        .Where(p => p.CompanyId == companyId && p.IsActive)
        .OrderByDescending(p => p.CreatedAt)
        .Take(3)
        .Select(p => new RecentProperty
        {
            PropertyId = p.PropertyId,
            PropertyName = p.PropertyName,
            UnitCount = _context.PropertyUnits.Count(u => u.PropertyId == p.PropertyId && u.IsActive == true)
        })
        .ToListAsync();
            summary.RecentTenants = await _context.Tenants
           .Where(t => t.CompanyId == companyId)
           .OrderByDescending(t => t.CreatedAt)
           .Take(3)
           .Select(t => new RecentTenant
           {
               TenantId = t.TenantId,
               FullName = t.FullName,
               PropertyName = t.PropertyTenants
               .OrderByDescending(pt => pt.StartDate)
               .Select(pt => pt.Property.PropertyName)
               .FirstOrDefault() ?? "No Property Assigned"
           })
           .ToListAsync();


            return summary;
        }
        public async Task<FetchDataRecords<TenantPropertyDto>> GetPropertiesByTenantId(Guid tenantId, int page, int pageSize)
        {
            var query = _context.PropertyTenants
                .Where(pt => pt.TenantId == tenantId && pt.IsActive== true) 
                .Include(pt => pt.Property)
                .Include(pt => pt.Unit);

            int totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(pt => pt.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(pt => new TenantPropertyDto
                {
                    PropertyName = pt.Property.PropertyName,
                    PropertyUnit = pt.Unit != null ? pt.Unit.UnitName : "N/A",
                    Status = pt.Status,
                    PropertyType = pt.Property.PropertyType
                })
                .ToListAsync();

            return new FetchDataRecords<TenantPropertyDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };
        }
        public async Task<FetchDataRecords<AllUnits>>GetUnitsByPropertyId(Guid propId,int page, int pageSize)
        {
            var compId = Guid.Parse(_userContext.CompanyId!);
            var query = _context.PropertyUnits
            .Where(pt=>pt.PropertyId== propId && pt.CompanyId== compId && pt.IsActive== true);

            int totalCount = await query.CountAsync();
            var data = await query 
            .OrderByDescending(pt=> pt.CreatedAt)
            .Skip((page-1) * pageSize)
            .Take(pageSize)
            .Select(pt=> new AllUnits
            {
                UnitId = pt.UnitId,
                UnitName = pt.UnitName,
                Status = pt.Status,
                RentPrice = pt.RentPrice
                
            })
            .ToListAsync();
            return new FetchDataRecords<AllUnits>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };
            
        }
       public async Task<List<UnitDto>> GetUnitsByPropertyId(Guid propId)
{
    return await _context.PropertyUnits
        .Where(pu => pu.PropertyId == propId)
        .AsNoTracking()
        .Include(pu => pu.Property)
        .Select(pu => new UnitDto
        {
            UnitId = pu.UnitId,
            UnitName = pu.UnitName,
            PropertyType = pu.Property.PropertyType
        })
        .ToListAsync();
}


        public async Task<List<PropertyDto>> PropertiesByPropertyId()
{
    var compId = Guid.Parse(_userContext.CompanyId!);
    return await _context.Properties
        .Where(pd=> pd.CompanyId == compId)
        .AsNoTracking()
        .Select(p => new PropertyDto
        {
            PropertyId = p.PropertyId,
            PropertyName = p.PropertyName
        })
        .ToListAsync();
}
public async Task<FetchDataRecords<RepairDto>> RepairsByPropertyId(Guid propId, int page, int pageSize,DateTime? startDate,DateTime? endDate)
{
    var query = _context.Repairs
        .Where(r => r.PropertyId == propId);

    // COUNT
    int totalCount = await query.CountAsync();
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

    // PAGINATED DATA
    var data = await query
        .OrderByDescending(r => r.CompletedDate)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(r => new RepairDto
        {
            Description = r.Description,
            Status = r.Status,
            Cost = r.Cost,
            CompletedDate = r.CompletedDate
        })
        .ToListAsync();

    return new FetchDataRecords<RepairDto>
    {
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        Data = data
    };
}
public async Task<FetchDataRecords<AllRepairs>> AllRepairs(
    int page,
    int pageSize,
    DateTime? startDate,
    DateTime? endDate,
    string? status
    )
{
    var compId = Guid.Parse(_userContext.CompanyId!);

    var query = _context.Repairs
        .Where(r => r.CompanyId == compId);
         

    // DATE FILTERS
    if (startDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue){
        query = query.Where(r => r.CreatedAt.Date <= endDate.Value.Date);
            }
            if (!string.IsNullOrWhiteSpace(status) && status != "All")
        query = query.Where(r => r.Status == status);
    // TOTAL COUNT BEFORE PAGINATION
    int totalCount = await query.CountAsync();
    query= query.Include(r=> r.Property);

    // PAGINATED RESULTS
    var data = await query
        .OrderByDescending(r => r.CompletedDate)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(r => new AllRepairs
        {
            Name  = r.Property.PropertyName,    
            Description   = r.Description,
            Status        = r.Status,
            Cost          = r.Cost,
            CompletedDate = r.CompletedDate
        })
        .ToListAsync();

    return new FetchDataRecords<AllRepairs>
    {
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        Data = data
    };
}












    }
}
    

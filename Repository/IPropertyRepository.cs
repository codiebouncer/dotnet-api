using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;

namespace Propman.Repository
{
    public interface IPropertyRepository
    {
        
        Task<bool> PropertyExists(string propname);
        Task<Property?> GetPropertyById(Guid propid);
        Task AddProperty(Property property);
        Task<Property> UpdateProperty(Property property);
        Task<List<Property>> GetAllProperties();
        Task AddPicture(IEnumerable<Picture> pictures);
        Task Delete(Property property);
        Task RemovePicturesAsync(IEnumerable<Guid> pictureIds);
        Task<bool> UnitExists(string unitname);
        Task<PropertyUnit?> GetUnitById(Guid propId);
        Task<List<UnitDto>> GetUnitsByPropertyId(Guid propId);
        Task AddUnit(PropertyUnit propertyunit);
        Task AddRepair(Repair repair);
        Task<Repair> UpdateRepair(Repair repair);
        Task<Repair?> GetRepairById(Guid repairId);
        Task<bool> UnitsFull(Guid propertyId);
        Task<PropertyUnit> UpdateUnit(PropertyUnit propertyunit);
        Task DeleteUnit(PropertyUnit propertyunit);
        Task<DashboardSummary> GetDashboardSummary();
        Task<FetchDataRecords<TenantPropertyDto>> GetPropertiesByTenantId(Guid tenantId, int page, int pageSize);
        Task<FetchDataRecords<AllUnits>>GetUnitsByPropertyId(Guid propId,int page, int pageSize);
        Task<List<PropertyDto>> PropertiesByPropertyId();
         Task<FetchDataRecords<RepairDto>> RepairsByPropertyId(Guid propId, int page, int pageSize,DateTime? startDate,DateTime? endDate);
         Task<FetchDataRecords<AllRepairs>> AllRepairs(
    int page,
    int pageSize,
    DateTime? startDate,
    DateTime? endDate,
    string? status
    );



        
    }
}

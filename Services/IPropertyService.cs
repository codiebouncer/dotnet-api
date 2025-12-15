using Microsoft.AspNetCore.Mvc;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;

namespace PropMan.Services
{
    public interface IPropertyService
    {
        Task<ApiResponse<IActionResult>> CreateProperty(CreateProperty request);
        Task<ApiResponse<List<Property>>> GetAllProps();
        Task<ApiResponse<Property>> DeleteProperty(Guid propid);
        Task<ApiResponse<Property>> UpdateProperty(UpdateProperty request);
        Task<ApiResponse<PropertyUnit>> AddUnit(CreateUnit request);
        Task<ApiResponse<Repair>> AddRepair(Repairreq request);
        Task<ApiResponse<Repair>> CompleteRepair(RepairComplete request);
        Task<ApiResponse<PropertyTenant>> AssignProperty(PropTen request);
        Task<ApiResponse<PropertyTenant>> UnassignProperty(Guid propertyTenantId);
        Task<ApiResponse<PropertyUnit>> UpdateUnit(UpdateUnit request);
        Task<ApiResponse<PropertyTenant>> AssignUnit(AssignUnit request);
        Task<ApiResponse<DashboardSummary>> GetDashboardSummary();
        Task<ApiResponse<FetchDataRecords<TenantPropertyDto>>> GetTenantProperties(Guid tenantId, int page, int pageSize);
        Task<ApiResponse<FetchDataRecords<AllUnits>>>GetUnitsById(Guid propId,int page,int pageSize);
        Task<ApiResponse<List<UnitDto>>>GetUnitsByPropertyId(Guid propId);
        Task<ApiResponse<List<PropertyDto>>>PropertiesdByPropertyId();
        Task<ApiResponse<FetchDataRecords<RepairDto>>>RepairsByPropertyId(Guid propId,int page,int pageSize,DateTime? startDate,DateTime? endDate);
        Task<ApiResponse<FetchDataRecords<AllRepairs>>>AllRepairs(int page,int pageSize,DateTime? startDate,DateTime? endDate,string? status);
        
    }
}

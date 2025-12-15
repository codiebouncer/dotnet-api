using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropMan.Models.Dto;
using PropMan.Services;



namespace PropMan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propService;

        public PropertyController( IPropertyService propService)
        {
            _propService = propService;
            
            
        }
        [HttpPost("create-property")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>> CreateCompany(CreateProperty request)
        {
            var result = await _propService.CreateProperty(request);
            return Ok(result);
        }
        [HttpGet("all-properties")]
        public async Task<ActionResult<string>> GetAllProperties()
        {
            var result = await _propService.GetAllProps();
            return Ok(result);
        }
        
        
        [HttpPut("update-property")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>>UpdateProperty(UpdateProperty request)
        {
            var result = await _propService.UpdateProperty(request);
            return Ok(result);
        }
        [HttpPost("add-unit")]
        public async Task<ActionResult<string>> AddUnit(CreateUnit request)
        {
            var result = await _propService.AddUnit(request);
            return Ok(result);
        }
        [HttpPost("delete-property")]
        public async Task<ActionResult<string>> DeleteProperty(Guid propid)
        {
            var result = await _propService.DeleteProperty(propid);
            return Ok(result);
        }
        [HttpPost("add-repair")]
        public async Task<ActionResult<string>> AddRepair(Repairreq request)
        {
            var result = await _propService.AddRepair(request);
            return Ok(result);
        }
        [HttpPost("complete-repair")]
        public async Task<ActionResult<string>> CompleteRepair(RepairComplete request)
        {
            var result = await _propService.CompleteRepair(request);
            return Ok(result);
        }
        [HttpPost("assign-property")]
        public async Task<ActionResult<string>> AssignProperty(PropTen request)
        {
            var result = await _propService.AssignProperty(request);
            return Ok(result);
        }
        [HttpPost("unassign-property")]
        public async Task<ActionResult<string>> UnassignProperty(Guid propertyId)
        {
            var result = await _propService.UnassignProperty(propertyId);
            return Ok(result);
        }
        [HttpPost("update-unit")]
        public async Task<ActionResult<string>> UpdateUnit(Guid propertyId)
        {
            var result = await _propService.UnassignProperty(propertyId);
            return Ok(result);
        }
        [HttpPost("dashboard-summary")]
        public async Task<ActionResult<string>> DashboardSummary()
        {
            var result = await _propService.GetDashboardSummary();
            return Ok(result);
        }
        [HttpGet("property/{tenantId}")]
        public async Task<ActionResult<string>> GetAllProperties(Guid tenantId,int page,int pageSize)
        {
            var result = await _propService.GetTenantProperties(tenantId, page, pageSize);
            return Ok(result);
        }
        [HttpGet("all-units-by-property/{propId}")]
        public async Task<ActionResult<string>> GetUnitsByProperty(Guid propId,int page,int pageSize)
        {
            var result = await _propService.GetUnitsById(propId, page, pageSize);
            return Ok(result);
        }
        [HttpGet("dropdown-unit/{propId}")]
        public async Task<ActionResult<string>> GetUnitsByPropertyId(Guid propId)
        {
            var result = await _propService.GetUnitsByPropertyId(propId);
            return Ok(result);
        }
        [HttpGet("dropdown-properties")]
         public async Task<ActionResult<string>> PropertiesByPropertyId()
        {
            var result = await _propService.PropertiesdByPropertyId();
            return Ok(result);
        }
        [HttpGet("repairs/{propertyId}")]
public async Task<IActionResult> GetRepairs(Guid propertyId, int page, int pageSize ,DateTime? startDate,DateTime? endDate)
{
    var result = await _propService.RepairsByPropertyId(propertyId, page, pageSize,startDate,endDate);
    return Ok( result);
}
[HttpGet("all-repairs")]
public async Task<IActionResult> AllRepairs( int page, int pageSize ,DateTime? startDate,DateTime? endDate,string? status)
{
    var result = await _propService.AllRepairs(page,pageSize,startDate,endDate,status);
    return Ok( result);
}

    }
} 

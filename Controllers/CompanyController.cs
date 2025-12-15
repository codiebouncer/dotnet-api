using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Propman.Services;
using PropMan.Models.Dto;



namespace Propman.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _compService;

        public CompanyController( ICompanyService compService)
        {
            _compService = compService;
            
            
        }
        [HttpPost("create-company")]
        public async Task<ActionResult<string>> CreateCompany(CreateCompany request)
        {
            var result = await _compService.CreateCompany(request);
            return Ok(result);
        }
        
    }
} 

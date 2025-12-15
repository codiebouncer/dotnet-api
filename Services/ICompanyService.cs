using Microsoft.AspNetCore.Mvc;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;

namespace Propman.Services
{
    public interface ICompanyService
    {
        Task<ApiResponse<IActionResult>> CreateCompany(CreateCompany request);
    }
}

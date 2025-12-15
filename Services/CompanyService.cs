using Documan.Services;
using Microsoft.AspNetCore.Mvc;
using Propman.Constants;
using Propman.Repository;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;



namespace Propman.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _compRepo;
        

        public CompanyService(ICompanyRepository compRepo)
        {
            _compRepo = compRepo;
        }

        public async Task<ApiResponse<IActionResult>> CreateCompany(CreateCompany request)
        {
            try
            {
                if (await _compRepo.CompanyExists(request.Name))
                {
                    return new ApiResponse<IActionResult>
                {
                    Status = (int)StatusCode.ValidationError,
                    Message = ConstantVariable.compexists,
                };
                }




                var company = new Company
                {
                    Name = request.Name,
                    Email = request.Email,
                    IsActive = true
                };
                await _compRepo.AddCompany(company);





                return new ApiResponse<IActionResult>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.compsuccess,
                };
            }
            catch (Exception ex)
            {
                
                return new ApiResponse<IActionResult>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message,
                };
            }

            
                
            

        }
        
    





    }
}


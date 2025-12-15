using PropMan.Models;

namespace Propman.Repository
{
    public interface ICompanyRepository
    {
        
        Task<bool> CompanyExists(string username);

        Task AddCompany(Company company);
        
    }
}

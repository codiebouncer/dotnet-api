using PropMan.Models;
using Microsoft.EntityFrameworkCore;


namespace Propman.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DataContext _context;  

        public CompanyRepository(DataContext context)
        {
            _context = context;
        }
        public async Task AddCompany(Company company)
        {
            company.CompanyId = Guid.NewGuid();
            
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        

        public async Task<User?> GetUser(string compname)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == compname);
        }

        public async Task<bool> CompanyExists(string compname)
        {
            return await _context.Companies.AnyAsync(u => u.Name == compname);
        }

        

        

        

    }
}

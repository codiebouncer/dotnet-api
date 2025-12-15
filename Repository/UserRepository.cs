using PropMan.Models;
using Microsoft.EntityFrameworkCore;


namespace Propman.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;  

        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public async Task AddUser(User user)
        {
            user.UserId = Guid.NewGuid();
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        

        public async Task<User?> GetUser(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Name == username);
        }

        

        

        

    }
}

using PropMan.Models;

namespace Propman.Repository
{
    public interface IUserRepository
    {
        
        Task<bool> UserExists(string username);
        Task<User?> GetUser(string username);
        Task AddUser(User user);
        
    }
}

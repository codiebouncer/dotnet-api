
using PropMan.Models;

namespace PropMan.Services.Token
{
    public interface ITokenService
    {
        string CreateToken(User user);
        //  Task<string> GenerateRefreshToken( User user);
    }
}

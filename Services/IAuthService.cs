using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;

namespace Propman.Services
{
    public interface ILoginService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(Login request);
        Task<ApiResponse<User>> RegisterUser(RegisterUser request);
        
    }
}

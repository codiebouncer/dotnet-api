using Microsoft.AspNetCore.Http;
using Propman.Services.UserContext;
using System.Security.Claims;

namespace PropMan.Services.UserContext
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }



        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst("UserName")?.Value;

        public string? CompanyId =>
           _httpContextAccessor.HttpContext?.User?.FindFirst("CompanyId"!)?.Value;    

        public string? Role => 
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    }
}

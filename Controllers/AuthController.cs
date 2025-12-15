using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Propman.Services;
using PropMan.Models.Dto;



namespace PropMan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public AuthController( ILoginService loginService)
        {
            _loginService = loginService;
            
            
        }

        // [HttpPost("register")]
        // public async Task<ActionResult<User>> Register([FromBody] UserDto request)
        // {
        //     var result = await _registerService.RegisterAsync(request);
        //     return result.Status==ConstantVariable.Success ? Ok(result) : BadRequest(result);
        // }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(Login request)
        {
            var result = await _loginService.LoginAsync(request);
            return Ok(result);
        }
        [HttpPost("register")]
        [Authorize]
        public async Task<ActionResult<string>> Register(RegisterUser request)
        {
            var result = await _loginService.RegisterUser(request);
            return Ok(result);
        }
        
    }
} 

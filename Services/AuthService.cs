using Documan.Services;
using Propman.Constants;
using Propman.Repository;
using Propman.Services.UserContext;
using PropMan.Models;
using PropMan.Models.Dto;
using PropMan.Payloads;
using PropMan.Services.AuditLogService;
using PropMan.Services.Token;


namespace Propman.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _token;
        private readonly IAuditLogService _auditLog;
        private readonly IUserContext _userContext;

        public LoginService(IUserRepository userRepo, ITokenService token,IAuditLogService auditLog,IUserContext userContext)
        {
            _userRepo = userRepo;
            _token = token;
            _auditLog = auditLog;
            _userContext = userContext;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(Login request)
        {
            try
            {
                var user = await _userRepo.GetUser(request.Username);
                if (user?.Name == null)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.userdne
                    };
                }
                string inputPasswordHash = MainFunction.ComputeSha256Hash(request.Password);




                if (user.PasswordHash != inputPasswordHash)
                {

                    return new ApiResponse<LoginResponse>
                    {
                        Status = (int)StatusCode.ValidationError,
                        Message = ConstantVariable.invalidLogin
                    };
                }

                var response = new LoginResponse
                {
                    accessToken = _token.CreateToken(user),
                    Role = user.Role
                };





                return new ApiResponse<LoginResponse>
                {
                    Status = (int)StatusCode.Success,
                    Message = ConstantVariable.validLogin,
                    Data = response
                };

            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponse>
                {
                    Status = (int)StatusCode.SystemError,
                    Message = ex.Message,

                };




            }





        }
        public async Task<ApiResponse<User>> RegisterUser(RegisterUser request)
{
    try
    {
        // Check if email already exists
        var existingUser = await _userRepo.UserExists(request.Name);
        if (existingUser )
        {
            return new ApiResponse<User>
            {
                Status = (int)StatusCode.ValidationError,
                Message = "Email is already registered."
            };
        }

        // Create password hash
        var passwordHash = MainFunction.ComputeSha256Hash(request.Password);

        var user = new User
        {
            CompanyId = Guid.Parse(_userContext.CompanyId),
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = passwordHash,
            Role = request.Role,
            IsActive = true
        };

        await _userRepo.AddUser(user);

        await _auditLog.Log(user.UserId, $"{user.Name} registered a new account.");

        return new ApiResponse<User>
        {
            Status = (int)StatusCode.Success,
            Message = "User registered successfully.",
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<User>
        {
            Status = (int)StatusCode.SystemError,
            Message = ex.Message
        };
    }
}

    }
}



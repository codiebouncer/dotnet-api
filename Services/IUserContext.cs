namespace Propman.Services.UserContext
{
    public interface IUserContext
    {
        public string? CompanyId { get; }
        public string? UserName { get; }
        public string? Role { get; }
    }
}

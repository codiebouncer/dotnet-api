using Propman.Constants;
using Propman.Repository;
using Propman.Services.UserContext;
using PropMan.Models;
using PropMan.Services.AuditLogService;


namespace Documan.Services.AuditLogService
{
    public class AuditLogService : IAuditLogService
    {
        private readonly DataContext _context;
        private readonly IUserContext _userContext;
        private readonly IUserRepository _userRepo;

        public AuditLogService(DataContext context,IUserContext userContext,IUserRepository userRepo)
        {
            _context = context;
            _userContext = userContext;
            _userRepo = userRepo;
        }

        public async Task Log(Guid entityId,string description)
        {
           
                var compId = Guid.Parse(_userContext.CompanyId);
            var user = await _userRepo.GetUser(_userContext.UserName);
            var auditLog = new AuditLog
            {
                LogId = Guid.NewGuid(),
                CompanyId = compId,
                UserId = user.UserId,
                EntityId = entityId,
                Description = description



            };
            

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
            
            
        }
    }
}


using System.Threading.Tasks;

namespace PropMan.Services.AuditLogService
{
    public interface IAuditLogService
    {
         Task Log(Guid entityId, string description);
    }
}

using Propman.Repository;
using Propman.Services.UserContext;
using PropMan.Models;

namespace PropMan.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ITenantRepository _tenantRepo;
        private readonly IUserContext _userContext;
       

        public NotificationService(ITenantRepository tenantRepo,IUserContext userContext)
        {
            _tenantRepo = tenantRepo;
            _userContext=userContext;
        }

        public async Task AddNotification(Guid companyId, Guid propertyTenantId, string messageType, string messageContent, Guid sentTo)
        {
            var notification = new Notification
            {
                CompanyId = companyId,
                PropertyTenantId = propertyTenantId,
                MessageType = messageType,
                MessageContent = messageContent,
                SentTo = sentTo
            };

            await _tenantRepo.AddNotification(notification);

        }

        public async Task<IEnumerable<Notification>> GetNotificationsByTenant(Guid tenantId)
        {
            return await _tenantRepo.GetNotificationsByTenant(tenantId);
        }

        public async Task<IEnumerable<Notification>> GetAllNotifications()
        {
            var companyId = Guid.Parse(_userContext.CompanyId);
            return await _tenantRepo.GetAllNotifications(companyId);
        }
    }
}
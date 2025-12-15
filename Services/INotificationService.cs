using PropMan.Models;

namespace PropMan.Services
{
    public interface INotificationService
    {
        Task AddNotification(Guid companyId, Guid propertyTenantId, string messageType, string messageContent, Guid sentTo);
        Task<IEnumerable<Notification>> GetNotificationsByTenant(Guid tenantId);
        Task<IEnumerable<Notification>> GetAllNotifications();
    }
}
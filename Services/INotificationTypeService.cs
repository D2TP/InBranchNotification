using InBranchNotification.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public interface INotificationTypeService
    {
        Task AddNotificationTypeAsync(NotificationTypeDTO command);
        Task<List<NotificationTypeDTO>> GetNotificationTypesAsync();
        Task<NotificationTypeDTO> GetNotificationTypeByIdAsync(NotificationTypeDTO id);
        Task UpdateNotificationTypeAsync(NotificationTypeDTO command);
        Task ApproveNotificationAsync(Approve command);
    }
}
using InBranchNotification.Domain;
using InBranchNotification.DTOs;
using InBranchNotification.Helpers;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public interface IServiceRequestHistory
    {
        Task AddServiceRequestHistoryAsync(ServiceHistory command);
        Task<PagedList<ServiceRequestHistoryDto>> SearchAllServiceRequestHistoryAsync(ServiceHistroySearchDto query);
    }
}
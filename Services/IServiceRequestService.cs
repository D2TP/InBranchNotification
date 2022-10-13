using InBranchNotification.Domain;
using InBranchNotification.DTOs;
using InBranchNotification.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public interface IServiceRequestService
    {
        Task AddServiceRequestAsync(ServiceRequestCreateDto command);
        Task<List<NotificationTypeDTO>> GetServiceRequestAsync();
        Task<ServiceRequestDetail> GetServiceRequestByIdAsync(string id);
        Task UpdateServiceRequestAsync(NotificationTypeDTO command);
        Task<PagedList<ServiceRequestDetail>> GetAllServiceRequestAsync(ServiceRequestSearch query);
        Task ReviewServiceRequestAsync(ReviewServiceRequest command);
        Task ApproveServiceRequestAsync(ApproveServiceRequest command);
        Task<ServiceRequest> GetServiceRequestByIdNoJoinsAsync(string id);
    }
}
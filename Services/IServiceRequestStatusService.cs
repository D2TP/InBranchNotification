using InBranchNotification.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public interface IServiceRequestStatusService
    {
        Task AddServiceRequestStatusAsync(ServiceRequestStatusDTO command);
        Task<ServiceRequestStatusDTO> GetServiceRequestStatusByIdAsync(ServiceRequestStatusDTO command);
        Task<List<ServiceRequestStatusDTO>> GetServiceRequestStatussAsync();
        Task UpdateServiceRequestStatusAsync(ServiceRequestStatusDTO command);
    }
}
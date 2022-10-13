using InBranchNotification.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public interface IServiiceRequestTypeService
    {
        Task AddServiceRequestTypeAsync(ServiceRequestTypeDto command);
        Task<ServiceRequestTypeDto> GetServiceRequestTypeByIdAsync(ServiceRequestTypeDto command);
        Task<List<ServiceRequestTypeDto>> GetServiceRequestTypesAsync();
        Task UpdateServiceRequestTypeAsync(ServiceRequestTypeDto command);
    }
}
using AutoMapper;
using InBranchNotification.Commands.AdUser;
using InBranchNotification.DTOs;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchNotification.Services
{
    public class ServiceRequestProfile : Profile
    {
     
        public ServiceRequestProfile()
        {
            CreateMap<ServiceRequestDTO, ServiceRequestCreateDto>();
            
        }
    }
}
//ServiceRequestCreateDto : ServiceRequestDTO
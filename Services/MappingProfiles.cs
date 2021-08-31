using AutoMapper;
using InBranchDashboard.Commands.AdUser;
using InBranchDashboard.DTOs;
using InBranchMgt.Commands.AdUser.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InBranchDashboard.Services
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ADUserInsertDTO, CreateADUserCommand>()
             .ForMember(d => d.Active, o => o.MapFrom(s => s.active))
                .ForMember(d => d.BranchId, o => o.MapFrom(s => s.branch_Id))
             .ForMember(d => d.Email, o => o.MapFrom(s => s.email))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.first_name))
                 .ForMember(d => d.LastName, o => o.MapFrom(s => s.last_name))
                 .ForMember(d => d.RoleId, o => o.MapFrom(s => s.role_id))
                 .ForMember(d => d.UserName, o => o.MapFrom(s => s.user_name));
            //CreateMap<Address, AddressDto>().ReverseMap();
            //CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            //CreateMap<BasketItemDto, BasketItem>().ReverseMap();
        }
    }
}

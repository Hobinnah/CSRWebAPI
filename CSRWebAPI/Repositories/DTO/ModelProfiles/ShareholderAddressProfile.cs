using AutoMapper;
using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO.ModelProfiles
{
    public class ShareholderAddressProfile : Profile
    {
        public ShareholderAddressProfile()
        {
            CreateMap<ShareholderAddress, ShareholderAddressDto>()
                .ForMember(d => d.ID, opt => opt.MapFrom(s => s.ShareholderAddressID)).ReverseMap();
        }

    }
}

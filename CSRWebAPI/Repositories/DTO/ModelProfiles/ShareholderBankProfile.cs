using AutoMapper;
using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO.ModelProfiles
{
    public class ShareholderBankProfile : Profile
    {
        public ShareholderBankProfile()
        {
            CreateMap<ShareholderBank, ShareholderBankDto>()
                .ForMember(d => d.ID, opt => opt.MapFrom(s => s.ShareholderBankID)).ReverseMap();
        }
    }
}

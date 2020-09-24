using AutoMapper;
using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO.ModelProfiles
{
    public class ShareholderProfile : Profile
    {
        public ShareholderProfile()
        {
            CreateMap<Shareholder, ShareholderDto>()
                .ForMember(d => d.ID, opt => opt.MapFrom(s => s.ShareholderID)).ReverseMap();
                //.ForMember(d => d.ShareholderAddress.ShareholderAddressID, opt => opt.MapFrom(s => s.ShareholderAddress.ID))
                //.ForMember(d => d.ShareholderBank.ShareholderBankID, opt => opt.MapFrom(s => s.ShareholderBank.ID));
        }
    }
}

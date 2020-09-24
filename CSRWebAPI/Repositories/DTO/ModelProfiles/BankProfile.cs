using AutoMapper;
using CSRWebAPI.Repositories.DTO;
using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.DTO.ModelProfiles
{
    public class BankProfile: Profile
    {
        public BankProfile()
        {
            CreateMap<Bank, BankDto>()
                .ForMember(d => d.BankName, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.ID, opt => opt.MapFrom(s => s.BankID));
        }
    }
}

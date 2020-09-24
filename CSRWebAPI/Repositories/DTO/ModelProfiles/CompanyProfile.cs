using AutoMapper;
using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO.ModelProfiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.ID, opt => opt.MapFrom(s => s.CompanyID));
        }
    }
}

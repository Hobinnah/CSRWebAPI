using AutoMapper;
using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO.ModelProfiles
{
    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            CreateMap<Country, CountryDto>()
                .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.ID, opt => opt.MapFrom(s => s.CountryID));
        }
    }
}

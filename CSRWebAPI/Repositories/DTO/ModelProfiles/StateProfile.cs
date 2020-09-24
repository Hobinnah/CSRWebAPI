using AutoMapper;
using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO.ModelProfiles
{
    public class StateProfile : Profile
    {
        public StateProfile()
        {
            CreateMap<State, StateDto>()
                .ForMember(d => d.StateName, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.ID, opt => opt.MapFrom(s => s.StateID));
        }
    }
}

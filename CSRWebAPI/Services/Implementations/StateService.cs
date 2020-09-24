using AutoMapper;
using CSRWebAPI.Models;
using CSRWebAPI.Repositories.DTO;
using CSRWebAPI.Repositories.Interfaces;
using CSRWebAPI.Repositories.Models;
using CSRWebAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Implementations
{
    public class StateService : IStateService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<StateService> logger;
        private readonly IConfiguration configuration;
        private readonly IStateRepository stateRepository;
        public StateService(IMapper mapper, IMemoryCache cache, IConfiguration configuration, ILogger<StateService> logger, IStateRepository stateRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.stateRepository = stateRepository;
        }

        public async Task<IEnumerable<StateDto>> GetAll()
        {
            IEnumerable<State> states = new List<State>();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                states = cache.Get<IEnumerable<State>>(string.Format("{0}", CacheEnum.STATES.ToString()));

                if (states == null || !states.Any())
                {
                    states = await this.stateRepository.GetAll();
                    if (states != null && states.Any())
                        cache.Set<IEnumerable<State>>(string.Format("{0}", CacheEnum.STATES.ToString()), states, DateTime.Now.AddHours(CacheTimeOutInHours));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<IEnumerable<StateDto>>(states);
        }

        public async Task<StateDto> GetID(int ID)
        {
            StateDto stateDto = new StateDto();
            State state = new State();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                IEnumerable<State> states = new List<State>();
                states = cache.Get<IEnumerable<State>>(string.Format("{0}", CacheEnum.STATES.ToString()));

                if (states == null || !states.Any())
                {
                    state = await this.stateRepository.GetByID(ID);
                    return this.mapper.Map<StateDto>(state);
                }

                stateDto = this.mapper.Map<StateDto>(states.FirstOrDefault(x => x.StateID == ID));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return stateDto;
        }

        public async Task<StateDto> CreateState(StateDto stateDto)
        {
            State state = new State();
            IEnumerable<State> checkState = new List<State>();

            try
            {
                checkState = await this.stateRepository.Find(x => x.Name.ToLower().Trim() == stateDto.StateName.ToLower().Trim());

                if (checkState == null || checkState.Any())
                {
                    state = this.mapper.Map<State>(stateDto);
                    state = await stateRepository.Create(state);
                    await stateRepository.Save();

                    cache.Remove(string.Format("{0}", CacheEnum.STATES.ToString()));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<StateDto>(state);
        }

        public async Task<StateDto> UpdateState(int id, StateDto stateDto)
        {
            try
            {
                State state = new State();

                state = this.mapper.Map<State>(stateDto);
                state = await stateRepository.Update(state);
                await stateRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.STATES.ToString()));

                stateDto = this.mapper.Map<StateDto>(state);

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return stateDto;
        }

        public async Task DeleteState(int stateID)
        {
            try
            {
                State state = new State();

                state = await this.stateRepository.GetByID(stateID);
                await stateRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.STATES.ToString()));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

        }
    }
}

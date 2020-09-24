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
    public class CountryService : ICountryService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<CountryService> logger;
        private readonly IConfiguration configuration;
        private readonly ICountryRepository countryRepository;
        public CountryService(IMapper mapper, IMemoryCache cache, IConfiguration configuration, ILogger<CountryService> logger, ICountryRepository countryRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.countryRepository = countryRepository;
        }

        public async Task<IEnumerable<CountryDto>> GetAll()
        {
            IEnumerable<Country> countries = new List<Country>();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                countries = cache.Get<IEnumerable<Country>>(string.Format("{0}", CacheEnum.COUNTRIES.ToString()));

                if (countries == null || !countries.Any())
                {
                    countries = await this.countryRepository.GetAll();
                    if (countries != null && countries.Any())
                        cache.Set<IEnumerable<Country>>(string.Format("{0}", CacheEnum.COUNTRIES.ToString()), countries, DateTime.Now.AddHours(CacheTimeOutInHours));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<IEnumerable<CountryDto>>(countries);
        }

        public async Task<CountryDto> GetID(int ID)
        {
            CountryDto CountryDto = new CountryDto();
            Country Country = new Country();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                IEnumerable<Country> countries = new List<Country>();
                countries = cache.Get<IEnumerable<Country>>(string.Format("{0}", CacheEnum.COUNTRIES.ToString()));

                if (countries == null || !countries.Any())
                {
                    Country = await this.countryRepository.GetByID(ID);
                    return this.mapper.Map<CountryDto>(Country);
                }

                CountryDto = this.mapper.Map<CountryDto>(countries.FirstOrDefault(x => x.CountryID == ID));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return CountryDto;
        }

        public async Task<CountryDto> CreateCountry(CountryDto countryDto)
        {
            Country country = new Country();
            IEnumerable<Country> checkCountry = new List<Country>();

            try
            {
                checkCountry = await this.countryRepository.Find(x => x.Name.ToLower().Trim() == countryDto.CountryName.ToLower().Trim());

                if (checkCountry == null || checkCountry.Any())
                {
                    country = this.mapper.Map<Country>(countryDto);
                    country = await countryRepository.Create(country);
                    await countryRepository.Save();

                    cache.Remove(string.Format("{0}", CacheEnum.COUNTRIES.ToString()));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<CountryDto>(country);
        }

        public async Task<CountryDto> UpdateCountry(int id, CountryDto countryDto)
        {
            try
            {
                Country country = new Country();

                country = this.mapper.Map<Country>(countryDto);
                country = await countryRepository.Update(country);
                await countryRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.COUNTRIES.ToString()));

                countryDto = this.mapper.Map<CountryDto>(country);

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return countryDto;
        }

        public async Task DeleteCountry(int CountryID)
        {
            try
            {
                Country Country = new Country();

                Country = await this.countryRepository.GetByID(CountryID);
                await countryRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.COUNTRIES.ToString()));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

        }
    }
}

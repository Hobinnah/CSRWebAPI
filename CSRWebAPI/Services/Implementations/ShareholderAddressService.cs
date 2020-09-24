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
    public class ShareholderAddressService : IShareholderAddressService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ShareholderAddressService> logger;
        private readonly IConfiguration configuration;
        private readonly IShareholderAddressRepository shareholderAddressRepository;
        public ShareholderAddressService(IMapper mapper, IMemoryCache cache, IConfiguration configuration, ILogger<ShareholderAddressService> logger, IShareholderAddressRepository shareholderAddressRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.shareholderAddressRepository = shareholderAddressRepository;
        }

        public async Task<IEnumerable<ShareholderAddressDto>> GetAll()
        {
            IEnumerable<ShareholderAddress> ShareholderAddresses = new List<ShareholderAddress>();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                ShareholderAddresses = cache.Get<IEnumerable<ShareholderAddress>>(string.Format("{0}", CacheEnum.SHAREHOLDERADDRESS.ToString()));

                if (ShareholderAddresses == null || !ShareholderAddresses.Any())
                {
                    ShareholderAddresses = await this.shareholderAddressRepository.GetAll();
                    if (ShareholderAddresses != null && ShareholderAddresses.Any())
                        cache.Set<IEnumerable<ShareholderAddress>>(string.Format("{0}", CacheEnum.SHAREHOLDERADDRESS.ToString()), ShareholderAddresses, DateTime.Now.AddHours(CacheTimeOutInHours));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<IEnumerable<ShareholderAddressDto>>(ShareholderAddresses);
        }

        public async Task<ShareholderAddressDto> GetID(int ID)
        {
            ShareholderAddressDto ShareholderAddressDto = new ShareholderAddressDto();
            ShareholderAddress ShareholderAddress = new ShareholderAddress();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                IEnumerable<ShareholderAddress> ShareholderAddresses = new List<ShareholderAddress>();
                ShareholderAddresses = cache.Get<IEnumerable<ShareholderAddress>>(string.Format("{0}", CacheEnum.SHAREHOLDERADDRESS.ToString()));

                if (ShareholderAddresses == null || !ShareholderAddresses.Any())
                {
                    ShareholderAddress = await this.shareholderAddressRepository.GetByID(ID);
                    return this.mapper.Map<ShareholderAddressDto>(ShareholderAddress);
                }

                ShareholderAddressDto = this.mapper.Map<ShareholderAddressDto>(ShareholderAddresses.FirstOrDefault(x => x.ShareholderAddressID == ID));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return ShareholderAddressDto;
        }

        public async Task<ShareholderAddressDto> CreateShareholderAddress(ShareholderAddressDto shareholderAddressDto)
        {
            ShareholderAddress shareholderAddress = new ShareholderAddress();
            IEnumerable<ShareholderAddress> checkShareholderAddress = new List<ShareholderAddress>();

            try
            {
                shareholderAddress = this.mapper.Map<ShareholderAddress>(shareholderAddressDto);
                shareholderAddress = await shareholderAddressRepository.Create(shareholderAddress);
                await shareholderAddressRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.SHAREHOLDERADDRESS.ToString()));
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<ShareholderAddressDto>(shareholderAddress);
        }

        public async Task<ShareholderAddressDto> UpdateShareholderAddress(int id, ShareholderAddressDto shareholderAddressDto)
        {
            try
            {
                ShareholderAddress shareholderAddress = new ShareholderAddress();

                shareholderAddress = this.mapper.Map<ShareholderAddress>(shareholderAddressDto);
                shareholderAddress = await shareholderAddressRepository.Update(shareholderAddress);
                await shareholderAddressRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.SHAREHOLDERADDRESS.ToString()));

                shareholderAddressDto = this.mapper.Map<ShareholderAddressDto>(shareholderAddress);

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return shareholderAddressDto;
        }

        public async Task DeleteShareholderAddress(int ShareholderAddressID)
        {
            try
            {
                ShareholderAddress ShareholderAddress = new ShareholderAddress();

                ShareholderAddress = await this.shareholderAddressRepository.GetByID(ShareholderAddressID);
                await shareholderAddressRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.SHAREHOLDERADDRESS.ToString()));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

        }
    }
}

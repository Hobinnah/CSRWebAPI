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
    public class ShareholderBankService : IShareholderBankService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ShareholderBankService> logger;
        private readonly IConfiguration configuration;
        private readonly IShareholderBankRepository shareholderBankRepository;
        public ShareholderBankService(IMapper mapper, IMemoryCache cache, IConfiguration configuration, ILogger<ShareholderBankService> logger, IShareholderBankRepository shareholderBankRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.shareholderBankRepository = shareholderBankRepository;
        }

        public async Task<IEnumerable<ShareholderBankDto>> GetAll()
        {
            IEnumerable<ShareholderBank> shareholderBanks = new List<ShareholderBank>();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                shareholderBanks = cache.Get<IEnumerable<ShareholderBank>>(string.Format("{0}", CacheEnum.SHAREHOLDERBANKS.ToString()));

                if (shareholderBanks == null || !shareholderBanks.Any())
                {
                    shareholderBanks = await this.shareholderBankRepository.GetAll();
                    if (shareholderBanks != null && shareholderBanks.Any())
                        cache.Set<IEnumerable<ShareholderBank>>(string.Format("{0}", CacheEnum.SHAREHOLDERBANKS.ToString()), shareholderBanks, DateTime.Now.AddHours(CacheTimeOutInHours));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<IEnumerable<ShareholderBankDto>>(shareholderBanks);
        }

        public async Task<ShareholderBankDto> GetID(int ID)
        {
            ShareholderBankDto shareholderBankDto = new ShareholderBankDto();
            ShareholderBank shareholderBank = new ShareholderBank();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                IEnumerable<ShareholderBank> shareholderBanks = new List<ShareholderBank>();
                shareholderBanks = cache.Get<IEnumerable<ShareholderBank>>(string.Format("{0}", CacheEnum.SHAREHOLDERBANKS.ToString()));

                if (shareholderBanks == null || !shareholderBanks.Any())
                {
                    shareholderBank = await this.shareholderBankRepository.GetByID(ID);
                    return this.mapper.Map<ShareholderBankDto>(shareholderBank);
                }

                shareholderBankDto = this.mapper.Map<ShareholderBankDto>(shareholderBanks.FirstOrDefault(x => x.ShareholderBankID == ID));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return shareholderBankDto;
        }

        public async Task<ShareholderBankDto> CreateShareholderBank(ShareholderBankDto shareholderBankDto)
        {
            ShareholderBank shareholderBank = new ShareholderBank();
            IEnumerable<ShareholderBank> checkShareholderBank = new List<ShareholderBank>();

            try
            {
                checkShareholderBank = await this.shareholderBankRepository.Find(x => x.BankID == shareholderBankDto.BankID && x.ShareholderID == shareholderBankDto.ShareholderID);

                if (checkShareholderBank == null || checkShareholderBank.Any())
                {
                    shareholderBank = this.mapper.Map<ShareholderBank>(shareholderBankDto);
                    shareholderBank = await shareholderBankRepository.Create(shareholderBank);
                    await shareholderBankRepository.Save();

                    cache.Remove(string.Format("{0}", CacheEnum.SHAREHOLDERBANKS.ToString()));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<ShareholderBankDto>(shareholderBank);
        }

        public async Task<ShareholderBankDto> UpdateShareholderBank(int id, ShareholderBankDto shareholderBankDto)
        {
            try
            {
                ShareholderBank shareholderBank = new ShareholderBank();

                shareholderBank = this.mapper.Map<ShareholderBank>(shareholderBankDto);
                shareholderBank = await shareholderBankRepository.Update(shareholderBank);
                await shareholderBankRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.SHAREHOLDERBANKS.ToString()));

                shareholderBankDto = this.mapper.Map<ShareholderBankDto>(shareholderBank);

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return shareholderBankDto;
        }

        public async Task DeleteShareholderBank(int bankID)
        {
            try
            {
                ShareholderBank shareholderBank = new ShareholderBank();

                shareholderBank = await this.shareholderBankRepository.GetByID(bankID);
                await shareholderBankRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.SHAREHOLDERBANKS.ToString()));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

        }
    }
}

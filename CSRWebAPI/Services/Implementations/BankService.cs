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
    public class BankService: IBankService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<BankService> logger;
        private readonly IConfiguration configuration;
        private readonly IBankRepository bankRepository;
        public BankService(IMapper mapper, IMemoryCache cache, IConfiguration configuration, ILogger<BankService> logger, IBankRepository bankRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.bankRepository = bankRepository;
        }

        public async Task<IEnumerable<BankDto>> GetAll()
        {
            IEnumerable<Bank> banks = new List<Bank>();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;
                
                banks = cache.Get<IEnumerable<Bank>>(string.Format("{0}", CacheEnum.BANKS.ToString()));

                if (banks == null || !banks.Any())
                {
                    banks = await this.bankRepository.GetAll();
                    if (banks != null && banks.Any())
                        cache.Set<IEnumerable<Bank>>(string.Format("{0}", CacheEnum.BANKS.ToString()), banks, DateTime.Now.AddHours(CacheTimeOutInHours));
                }
            } catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<IEnumerable<BankDto>>(banks);
        }

        public async Task<BankDto> GetID(int ID)
        {
            BankDto bankDto = new BankDto();
            Bank bank = new Bank();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                IEnumerable<Bank> banks = new List<Bank>();
                banks = cache.Get<IEnumerable<Bank>>(string.Format("{0}", CacheEnum.BANKS.ToString()));

                if (banks == null || !banks.Any())
                {
                    bank = await this.bankRepository.GetByID(ID);
                    return this.mapper.Map<BankDto>(bank);
                }

                bankDto = this.mapper.Map<BankDto>(banks.FirstOrDefault(x => x.BankID == ID));

            } catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return bankDto;
        }

        public async Task<BankDto> CreateBank(BankDto bankDto)
        {
            Bank bank = new Bank();
            IEnumerable<Bank> checkBank = new List<Bank>();

            try
            {
                checkBank = await this.bankRepository.Find(x => x.Name.ToLower().Trim() == bankDto.BankName.ToLower().Trim());

                if (checkBank == null || checkBank.Any())
                {
                    bank = this.mapper.Map<Bank>(bankDto);
                    bank = await bankRepository.Create(bank);
                    await bankRepository.Save();

                    cache.Remove(string.Format("{0}", CacheEnum.BANKS.ToString()));
                }
            } catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<BankDto>(bank);
        }

        public async Task<BankDto> UpdateBank(int id, BankDto bankDto)
        {
            try
            {
                Bank bank = new Bank();

                bank = this.mapper.Map<Bank>(bankDto);
                bank = await bankRepository.Update(bank);
                await bankRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.BANKS.ToString()));

                bankDto = this.mapper.Map<BankDto>(bank);

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return bankDto;
        }

        public async Task DeleteBank(int bankID)
        {
            try
            {
                Bank bank = new Bank();

                bank = await this.bankRepository.GetByID(bankID);
                await bankRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.BANKS.ToString()));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

        }

    }
}

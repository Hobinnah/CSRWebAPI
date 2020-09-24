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
    public class ShareholderService : IShareholderService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ShareholderService> logger;
        private readonly IFileManager fileManager;
        private readonly IConfiguration configuration;
        private readonly IShareholderRepository shareholderRepository;
        private readonly IShareholderAddressRepository shareholderAddressRepository;
        private readonly IShareholderBankRepository shareholderBankRepository;
        public ShareholderService(IMapper mapper, IMemoryCache cache, IConfiguration configuration, ILogger<ShareholderService> logger, IShareholderRepository shareholderRepository,
            IShareholderAddressRepository shareholderAddressRepository, IShareholderBankRepository shareholderBankRepository, IFileManager fileManager)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.fileManager = fileManager;
            this.configuration = configuration;
            this.shareholderRepository = shareholderRepository;
            this.shareholderBankRepository = shareholderBankRepository;
            this.shareholderAddressRepository = shareholderAddressRepository;
        }

        public async Task<IEnumerable<ShareholderDto>> GetAll()
        {
            IEnumerable<Shareholder> shareholders = new List<Shareholder>();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                shareholders = cache.Get<IEnumerable<Shareholder>>(string.Format("{0}", CacheEnum.SHAREHOLDERS.ToString()));

                if (shareholders == null || !shareholders.Any())
                {
                    shareholders = this.shareholderRepository.Shareholders;
                    if (shareholders != null && shareholders.Any())
                        cache.Set<IEnumerable<Shareholder>>(string.Format("{0}", CacheEnum.SHAREHOLDERS.ToString()), shareholders, DateTime.Now.AddHours(CacheTimeOutInHours));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<IEnumerable<ShareholderDto>>(shareholders);
        }

        public async Task<ShareholderDto> GetID(int ID)
        {
            ShareholderDto shareholderDto = new ShareholderDto();
            Shareholder shareholder = new Shareholder();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                IEnumerable<Shareholder> shareholders = new List<Shareholder>();
                shareholders = cache.Get<IEnumerable<Shareholder>>(string.Format("{0}", CacheEnum.SHAREHOLDERS.ToString()));

                if (shareholders == null || !shareholders.Any())
                {
                    shareholder = await this.shareholderRepository.GetByID(ID);
                    return this.mapper.Map<ShareholderDto>(shareholder);
                }

                shareholderDto = this.mapper.Map<ShareholderDto>(shareholders.FirstOrDefault(x => x.ShareholderID == ID));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return shareholderDto;
        }

        public async Task<ShareholderDto> CreateShareholder(ShareholderDto shareholderDto)
        {
            Shareholder shareholder = new Shareholder();
            ShareholderAddress shareholderAddress = new ShareholderAddress();
            ShareholderAddressDto shareholderAddressDto = new ShareholderAddressDto();
            ShareholderBank shareholderBank = new ShareholderBank();
            ShareholderBankDto shareholderBankDto = new ShareholderBankDto();
            IEnumerable<Shareholder> checkShareholder = new List<Shareholder>();
            string base64Image = string.Empty;

            try
            {
                if (shareholder.Company)
                {
                    checkShareholder = await this.shareholderRepository.Find(x => x.GSM01 == shareholderDto.GSM01 && x.CompanyName.ToLower().Trim() == shareholderDto.CompanyName.ToLower().Trim());
                } else
                {
                    checkShareholder = await this.shareholderRepository.Find(x => x.GSM01 == shareholderDto.GSM01 && x.Lastname.ToLower().Trim() == shareholderDto.Lastname.ToLower().Trim()
                                        && x.Firstname.ToLower().Trim() == shareholderDto.Firstname.ToLower().Trim() && x.Middlename.ToLower().Trim() == shareholderDto.Middlename.ToLower().Trim());
                }
                if (checkShareholder == null || !checkShareholder.Any())
                {
                    //if (!string.IsNullOrEmpty(shareholderDto.ImageString))
                    //{
                    //    string url = this.cache.Get<string>(shareholderDto.ImageString);
                    //    if (!string.IsNullOrEmpty(url))
                    //    {
                    //        base64Image = fileManager.ConvertImageToByte_FromByteToBase64String(url);
                    //        if (!string.IsNullOrEmpty(base64Image))
                    //        {
                    //            shareholderDto.ImageString = base64Image;
                    //            this.fileManager.DeleteFIle(url);
                    //        }
                    //    }
                    //}


                    //shareholderAddressDto = shareholderDto.ShareholderAddress;
                    //shareholderAddressDto.CapturedDate = DateTime.Now;
                    //shareholderDto.ShareholderAddress = new ShareholderAddressDto();
                    //shareholderBankDto = shareholderDto.ShareholderBank;
                    //shareholderBankDto.CapturedDate = DateTime.Now;
                    //shareholderDto.ShareholderBank = new ShareholderBankDto();

                    //shareholderAddress = this.mapper.Map<ShareholderAddress>(shareholderAddressDto);
                    //shareholderBank = this.mapper.Map<ShareholderBank>(shareholderBankDto);
                    shareholder = this.mapper.Map<Shareholder>(shareholderDto);

                    shareholder.ShareholderAddress.CapturedDate = DateTime.Now;
                    shareholder.ShareholderBank.CapturedDate = DateTime.Now;

                    shareholderAddress = await this.shareholderAddressRepository.Create(shareholder.ShareholderAddress);
                    shareholderBank = await this.shareholderBankRepository.Create(shareholder.ShareholderBank);                    
                    shareholder = await shareholderRepository.Create(shareholder);
                    await shareholderRepository.Save();

                    shareholder.ShareholderAddressID = shareholderAddress.ShareholderAddressID;
                    shareholderAddress.ShareholderID = shareholder.ShareholderID;
                    shareholder.ShareholderBankID = shareholderBank.ShareholderBankID;
                    shareholderBank.ShareholderID = shareholder.ShareholderID;

                    await shareholderRepository.Update(shareholder);
                    await this.shareholderAddressRepository.Update(shareholder.ShareholderAddress);
                    await this.shareholderBankRepository.Update(shareholder.ShareholderBank);
                    await shareholderRepository.Save();

                    cache.Remove(string.Format("{0}", CacheEnum.SHAREHOLDERS.ToString()));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<ShareholderDto>(shareholder);
        }

        public async Task<ShareholderDto> UpdateShareholder(int id, ShareholderDto shareholderDto)
        {
            try
            {
                Shareholder shareholder = new Shareholder();
                
                shareholder = this.mapper.Map<Shareholder>(shareholderDto);
                await this.shareholderAddressRepository.Update(shareholder.ShareholderAddress);
                await this.shareholderBankRepository.Update(shareholder.ShareholderBank);
                await shareholderRepository.Update(shareholder);

                await shareholderRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.SHAREHOLDERS.ToString()));

                shareholderDto = this.mapper.Map<ShareholderDto>(shareholder);

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return shareholderDto;
        }

        public async Task DeleteShareholder(int shareholderID)
        {
            try
            {
                Shareholder shareholder = new Shareholder();

                shareholder = await this.shareholderRepository.GetByID(shareholderID);
                await shareholderRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.SHAREHOLDERS.ToString()));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

        }
    }
}

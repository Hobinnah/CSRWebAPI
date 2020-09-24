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
    public class CompanyService : ICompanyService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<CompanyService> logger;
        private readonly IConfiguration configuration;
        private readonly ICompanyRepository companyRepository;
        public CompanyService(IMapper mapper, IMemoryCache cache, IConfiguration configuration, ILogger<CompanyService> logger, ICompanyRepository companyRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.configuration = configuration;
            this.companyRepository = companyRepository;
        }

        public async Task<IEnumerable<CompanyDto>> GetAll()
        {
            IEnumerable<Company> companies = new List<Company>();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                companies = cache.Get<IEnumerable<Company>>(string.Format("{0}", CacheEnum.COMPANIES.ToString()));

                if (companies == null || !companies.Any())
                {
                    companies = await this.companyRepository.GetAll();
                    if (companies != null && companies.Any())
                        cache.Set<IEnumerable<Company>>(string.Format("{0}", CacheEnum.COMPANIES.ToString()), companies, DateTime.Now.AddHours(CacheTimeOutInHours));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<CompanyDto> GetID(int ID)
        {
            CompanyDto CompanyDto = new CompanyDto();
            Company Company = new Company();
            try
            {
                int CacheTimeOutInHours = this.configuration.GetValue<int>("MemoryCache:CacheTimeOutInHours");

                if (CacheTimeOutInHours <= 0)
                    CacheTimeOutInHours = 1;

                IEnumerable<Company> companies = new List<Company>();
                companies = cache.Get<IEnumerable<Company>>(string.Format("{0}", CacheEnum.COMPANIES.ToString()));

                if (companies == null || !companies.Any())
                {
                    Company = await this.companyRepository.GetByID(ID);
                    return this.mapper.Map<CompanyDto>(Company);
                }

                CompanyDto = this.mapper.Map<CompanyDto>(companies.FirstOrDefault(x => x.CompanyID == ID));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return CompanyDto;
        }

        public async Task<CompanyDto> CreateCompany(CompanyDto companyDto)
        {
            Company company = new Company();
            IEnumerable<Company> checkCompany = new List<Company>();

            try
            {
                checkCompany = await this.companyRepository.Find(x => x.Name.ToLower().Trim() == companyDto.CompanyName.ToLower().Trim());

                if (checkCompany == null || checkCompany.Any())
                {
                    company = this.mapper.Map<Company>(companyDto);
                    company = await companyRepository.Create(company);
                    await companyRepository.Save();

                    cache.Remove(string.Format("{0}", CacheEnum.COMPANIES.ToString()));
                }
            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return this.mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> UpdateCompany(int id, CompanyDto companyDto)
        {
            try
            {
                Company company = new Company();

                company = this.mapper.Map<Company>(companyDto);
                company = await companyRepository.Update(company);
                await companyRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.COMPANIES.ToString()));

                companyDto = this.mapper.Map<CompanyDto>(company);

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

            return companyDto;
        }

        public async Task DeleteCompany(int CompanyID)
        {
            try
            {
                Company Company = new Company();

                Company = await this.companyRepository.GetByID(CompanyID);
                await companyRepository.Save();

                cache.Remove(string.Format("{0}", CacheEnum.COMPANIES.ToString()));

            }
            catch (Exception er) { logger.LogError(string.Format("{0}===================={1}====================\n", DateTime.Now.ToString(), er.ToString())); }

        }
    }
}

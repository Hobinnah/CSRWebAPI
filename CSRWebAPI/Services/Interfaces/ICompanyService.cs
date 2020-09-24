using CSRWebAPI.Repositories.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAll();
        Task<CompanyDto> GetID(int ID);
        Task<CompanyDto> CreateCompany(CompanyDto Company);
        Task<CompanyDto> UpdateCompany(int id, CompanyDto Company);
        Task DeleteCompany(int CompanyID);
    }
}

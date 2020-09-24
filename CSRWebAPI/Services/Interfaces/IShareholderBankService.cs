using CSRWebAPI.Repositories.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Interfaces
{
    public interface IShareholderBankService
    {
        Task<IEnumerable<ShareholderBankDto>> GetAll();
        Task<ShareholderBankDto> GetID(int ID);
        Task<ShareholderBankDto> CreateShareholderBank(ShareholderBankDto ShareholderBank);
        Task<ShareholderBankDto> UpdateShareholderBank(int id, ShareholderBankDto ShareholderBank);
        Task DeleteShareholderBank(int ShareholderBankID);
    }
}

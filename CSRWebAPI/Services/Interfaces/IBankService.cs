using CSRWebAPI.Repositories.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Interfaces
{
    public interface IBankService
    {
        Task<IEnumerable<BankDto>> GetAll();
        Task<BankDto> GetID(int ID);
        Task<BankDto> CreateBank(BankDto bank);
        Task<BankDto> UpdateBank(int id, BankDto bank);
        Task DeleteBank(int bankID);
    }
}

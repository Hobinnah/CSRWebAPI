using CSRWebAPI.Repositories.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Interfaces
{
    public interface IShareholderService
    {
        Task<IEnumerable<ShareholderDto>> GetAll();
        Task<ShareholderDto> GetID(int ID);
        Task<ShareholderDto> CreateShareholder(ShareholderDto Shareholder);
        Task<ShareholderDto> UpdateShareholder(int id, ShareholderDto Shareholder);
        Task DeleteShareholder(int ShareholderID);
    }
}

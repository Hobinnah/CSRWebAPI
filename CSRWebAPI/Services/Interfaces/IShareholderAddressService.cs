using CSRWebAPI.Repositories.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Interfaces
{
    public interface IShareholderAddressService
    {
        Task<IEnumerable<ShareholderAddressDto>> GetAll();
        Task<ShareholderAddressDto> GetID(int ID);
        Task<ShareholderAddressDto> CreateShareholderAddress(ShareholderAddressDto ShareholderAddress);
        Task<ShareholderAddressDto> UpdateShareholderAddress(int id, ShareholderAddressDto ShareholderAddress);
        Task DeleteShareholderAddress(int ShareholderAddressID);
    }
}

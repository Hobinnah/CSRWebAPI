using CSRWebAPI.Repositories.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryDto>> GetAll();
        Task<CountryDto> GetID(int ID);
        Task<CountryDto> CreateCountry(CountryDto Country);
        Task<CountryDto> UpdateCountry(int id, CountryDto Country);
        Task DeleteCountry(int CountryID);
    }
}

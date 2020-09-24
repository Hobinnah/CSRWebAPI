using CSRWebAPI.Repositories.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Services.Interfaces
{
    public interface IStateService
    {
        Task<IEnumerable<StateDto>> GetAll();
        Task<StateDto> GetID(int ID);
        Task<StateDto> CreateState(StateDto State);
        Task<StateDto> UpdateState(int id, StateDto State);
        Task DeleteState(int StateID);
    }
}

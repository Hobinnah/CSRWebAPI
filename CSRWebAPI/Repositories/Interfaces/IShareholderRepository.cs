using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Interfaces
{
    public interface IShareholderRepository : IRepository<Shareholder>
    {
        IEnumerable<Shareholder> Shareholders { get; }
    }
}

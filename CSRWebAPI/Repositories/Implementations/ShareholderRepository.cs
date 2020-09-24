using CSRWebAPI.Repositories.Interfaces;
using CSRWebAPI.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Implementations
{
    public class ShareholderRepository : Repository<Shareholder>, IShareholderRepository, IDisposable
    {
        private readonly CSRDBContext context;

        public ShareholderRepository(CSRDBContext context) : base(context)
        {
            this.context = context;
        }

        public IEnumerable<Shareholder> Shareholders => context.Shareholders.OrderByDescending(x => x.ShareholderID).Include(x => x.ShareholderAddress).Include(x => x.ShareholderBank)
                                                        .Include(x => x.ShareholderAddress.State).Include(x => x.ShareholderBank.Bank).Include(x => x.ShareholderAddress.Country).ToList();
       
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._context != null)
                {
                    this._context.Dispose();
                    this._context = null;
                }
            }
        }
    }
}

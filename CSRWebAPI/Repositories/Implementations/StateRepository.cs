using CSRWebAPI.Repositories.Interfaces;
using CSRWebAPI.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Implementations
{
    public class StateRepository : Repository<State>, IStateRepository, IDisposable
    {

        private readonly CSRDBContext context;

        public StateRepository(CSRDBContext context) : base(context)
        {
            this.context = context;
        }

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

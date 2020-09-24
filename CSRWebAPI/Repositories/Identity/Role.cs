using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Identity
{
    public class Role : IdentityRole<long>
    {
        public int UserType { get; set; }
    }
}

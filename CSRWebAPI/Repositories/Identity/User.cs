using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Identity
{
    public class User : IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PreferenceID { get; set; }
        public bool Admin { get; set; }
        public bool IsBlackListed { get; set; }
        public string CapturedBy { get; set; }
        public DateTime CapturedDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }
        public string Comment { get; set; }
    }

}

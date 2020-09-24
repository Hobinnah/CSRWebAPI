using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO
{
   // [AutoMapTo(typeof(Bank))]
    public class BankDto
    {
        public int ID { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Kindly indicate a bank's name.")]
        public string BankName { get; set; }
    }
}

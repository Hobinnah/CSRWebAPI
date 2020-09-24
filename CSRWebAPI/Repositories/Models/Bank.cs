using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Models
{
    public class Bank
    {
        [Key]
        public int BankID { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Kindly indicate a bank's name.")]
        public string Name { get; set; }
    }
}

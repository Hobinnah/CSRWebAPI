using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO
{
    public class CompanyDto
    {
        public int ID { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Kindly indicate a company's name.")]
        public string CompanyName { get; set; }
    }
}

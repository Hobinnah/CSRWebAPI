using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Models
{
    public class Country
    {
        [Key]
        public int CountryID { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Kindly indicate a country's name.")]
        public string Name { get; set; }
    }
}

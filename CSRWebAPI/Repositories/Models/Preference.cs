using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Models
{
    public class Preference
    {
        [Key]
        public int PreferenceID { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Kindly indicate Company Name.")]
        public String Description { get; set; }
    }
}

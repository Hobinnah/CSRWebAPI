using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Models
{
    public class State
    {
        [Key]
        public int StateID { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Kindly indicate a state.")]
        public string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO
{
    public class StateDto
    {
        public int ID { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Kindly indicate a state.")]
        public string StateName { get; set; }
    }
}

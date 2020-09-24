using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.Models
{
    public class ShareholderAddress
    {
        [Key]
        public int ShareholderAddressID { get; set; }
        public int ShareholderID { get; set; }
        [StringLength(100)]
        [Required(ErrorMessage = "Kindly indicate an address.")]
        public string Address01 { get; set; }
        public string Address02 { get; set; }
        [Required(ErrorMessage = "Kindly indicate a city.")]
        public string City { get; set; }
        [Required]
        public int StateID { get; set; }
        [Required]
        public int CountryID { get; set; }
        public DateTime? CapturedDate { get; set; }
        public virtual State State { get; set; }
        public virtual Country Country { get; set; }
    }
}

using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO
{
    public class ShareholderBankDto
    {
        public int ID { get; set; }
        [Required]
        public int ShareholderID { get; set; }
        [Required(ErrorMessage = "Please indicate a bank.")]
        public int BankID { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please indicate the account opening date.")]
        public DateTime? AccountOpeningDate { get; set; }
        [Required(ErrorMessage = "Kindly indicate your BVN.")]
        [StringLength(15)]
        public string BVN { get; set; }
        [Required(ErrorMessage = "Kindly indicate an account number.")]
        [StringLength(10)]
        public string AccountNumber { get; set; }
        public string CapturedBy { get; set; }
        public DateTime? CapturedDate { get; set; }
        public virtual Bank Bank { get; set; }
    }
}

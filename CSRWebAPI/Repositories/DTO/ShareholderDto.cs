using CSRWebAPI.Repositories.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSRWebAPI.Repositories.DTO
{
    public class ShareholderDto
    {
        public int ID { get; set; }
        [StringLength(50)]
        public string CompanyName { get; set; }
        [StringLength(50)]
        public string Lastname { get; set; }
        [StringLength(50)]
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public bool Company { get; set; } = false;
        public int ShareholderAddressID { get; set; }
        public int ShareholderBankID { get; set; }
        public string CHN { get; set; }
        [Required]
        [StringLength(20)]
        public string GSM01 { get; set; }
        public string GSM02 { get; set; }
        public string Image { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Emailaddress { get; set; }
        public string Field01 { get; set; }
        public string Field02 { get; set; }
        public string Field03 { get; set; }
        public string Field04 { get; set; }
        public string Field05 { get; set; }
        public ShareholderAddressDto ShareholderAddress { get; set; }
        public ShareholderBankDto ShareholderBank { get; set; }
    }
}

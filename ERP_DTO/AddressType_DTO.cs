using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class AddressType_DTO
    {
        public Int64 ADTP_Number { get; set; }

        [Display(Name = "Address Type")]
        [Required(ErrorMessage = "Address Type is Required")]
        [MaxLength(50, ErrorMessage = "Address Type be longer than 50 characters.")]
        public String? ADTP_Name { get; set; }

        [Display(Name = "Notes")]
        [MaxLength(250, ErrorMessage = "Notes be longer than 250 characters.")]
        [DataType(DataType.MultilineText)]
        public String? ADTP_Notes { get; set; }

        public String? ADTP_DeleteNumbers { get; set; }

        public Int64 ADTP_CreatorCode { get; set; }

        public Int16 ADTP_Id { get; set; }

        public void Reset()
        {
            this.ADTP_Number = 0;
            this.ADTP_Name = string.Empty;
            this.ADTP_Notes = string.Empty;
            this.ADTP_DeleteNumbers = string.Empty;
            this.ADTP_CreatorCode = 0;
            this.ADTP_Id = 0;
        }
    }
}

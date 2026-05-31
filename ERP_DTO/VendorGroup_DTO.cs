using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class VendorGroup_DTO
    {
        public Int64 VendorGroupNumber { get; set; }

        [Display(Name = "Vendor Group")]
        [Required(ErrorMessage = "Vendor Group is Required")]
        [MaxLength(30, ErrorMessage = "Vendor Group be longer than 30 characters.")]
        public String? VendorGroup { get; set; }

        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        [DataType(DataType.MultilineText)]
        public String? VG_Description { get; set; }

        [Display(Name = "Under")]
        public String? UnderVGroup { get; set; }
        public String? DeleteNumbers { get; set; }

        public Int32 CreatorCode { get; set; }

        public Int32 Id { get; set; }

        public void Reset()
        {
            this.VendorGroupNumber = 0;
            this.VG_Description = string.Empty;
            this.UnderVGroup = string.Empty;
            this.DeleteNumbers = string.Empty;
        }
    }
}

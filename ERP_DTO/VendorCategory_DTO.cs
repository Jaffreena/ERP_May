using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class VendorCategory_DTO
    {
        public Int64 VendorCategoryNumber { get; set; }

        [Display(Name = "Vendor Category")]
        [Required(ErrorMessage = "Vendor Category is Required")]
        [MaxLength(30, ErrorMessage = "Vendor Category be longer than 30 characters.")]
        public String? VendorCategory { get; set; }

        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        [DataType(DataType.MultilineText)]
        public String? VC_Description { get; set; }

        [Display(Name = "Under")]
        public String? UnderVCategory { get; set; }
        public String? DeleteNumbers { get; set; }

        public Int32 CreatorCode { get; set; }

        public Int32 Id { get; set; }

        public void Reset()
        {
            this.VendorCategoryNumber = 0;
            this.VC_Description = string.Empty;
            this.UnderVCategory = string.Empty;
            this.DeleteNumbers = string.Empty;
        }
    }
}

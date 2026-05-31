using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class WithholdTax_DTO
    {
        public Int64 WH_Number { get; set; }

        [Display(Name = "WH Tax Code")]
        [Required(ErrorMessage = "WH Tax Code is Required")]
        [MaxLength(25, ErrorMessage = "WH Tax Code be longer than 25 characters.")]
        public String? WH_TaxCode { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is Required")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        public String? WH_TaxDescription { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is Required")]
        public String? WH_TaxCategory { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessage = "Type is Required")]
        public String? WH_TaxType { get; set; }

        [Display(Name = "Impact")]
        [Required(ErrorMessage = "Impact is Required")]
        public String? WH_TaxImpact { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class ItemSubcategory_DTO
    {
        public Int64 ItemSubcategoryNumber { get; set; }

        [Display(Name = "Item Sub-category")]
        [Required(ErrorMessage = "Item Sub-category is Required")]
        [MaxLength(30, ErrorMessage = "Item Sub-category be longer than 30 characters.")]
        public String? ItemSubcategory { get; set; }

        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        [DataType(DataType.MultilineText)]
        public String? ISC_Description { get; set; }

        [Display(Name = "Under")]
        public String? UnderISubcategory { get; set; }
        public String? DeleteNumbers { get; set; }

        public Int32 CreatorCode { get; set; }

        public Int32 Id { get; set; }

        public void Reset()
        {
            this.ItemSubcategoryNumber = 0;
            this.ItemSubcategory = string.Empty;
            this.ISC_Description = string.Empty;
            this.UnderISubcategory = string.Empty;
            this.DeleteNumbers = string.Empty;
        }
    }
}

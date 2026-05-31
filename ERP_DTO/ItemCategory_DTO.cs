using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class ItemCategory_DTO
    {
        public Int64 ItemCategoryNumber { get; set; }

        [Display(Name = "Item Category")]
        [Required(ErrorMessage = "Item Category is Required")]
        [MaxLength(30, ErrorMessage = "Item Category be longer than 30 characters.")]
        public String? ItemCategory { get; set; }

        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        [DataType(DataType.MultilineText)]
        public String? IC_Description { get; set; }

        [Display(Name = "Under")]
        public String? UnderICategory { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }

        public Int32 Id { get; set; }

        public void Reset()
        {
            this.ItemCategoryNumber = 0;
            this.ItemCategory = string.Empty;
            this.IC_Description = string.Empty;
            this.UnderICategory = string.Empty;
            this.DeleteNumbers = string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class ItemGroup_DTO
    {
        public Int64 ItemGroupNumber { get; set; }

        [Display(Name = "Item Group")]
        [Required(ErrorMessage = "Item Group is Required")]
        [MaxLength(30, ErrorMessage = "Item Group be longer than 30 characters.")]
        public String? ItemGroup { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is Required")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        public String? IG_Description { get; set; }

        [Display(Name = "Under")]
        [Required(ErrorMessage = "Under is Required")]
        public String? UnderIGroup { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public String? MaterialSegregation { get; set; }

        [Display(Name = "Material Ownership")]
        [Required(ErrorMessage = "Material Ownership is Required")]
        public String? MaterialOwnership { get; set; }

        [Display(Name = "Purchase Warehouse")]
        [Required(ErrorMessage = "Purchase Warehouse is Required")]
        public String? PurchaseWarehouse { get; set; }

        [Display(Name = "Sale Warehouse")]
        [Required(ErrorMessage = "Sale Warehouse is Required")]
        public String? SaleWarehouse { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.ItemGroupNumber = 0;
            this.ItemGroup = string.Empty;
            this.IG_Description = string.Empty;
            this.UnderIGroup = string.Empty;
            this.MaterialSegregation = string.Empty;
            this.MaterialOwnership = string.Empty;
            this.PurchaseWarehouse = string.Empty;
            this.SaleWarehouse = string.Empty;
        }
    }
}

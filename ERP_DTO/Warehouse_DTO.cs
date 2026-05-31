using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class Warehouse_DTO
    {
        public Int64 WarehouseNumber { get; set; }

        [Display(Name = "Warehouse Code")]
        [Required(ErrorMessage = "Code is Required")]
        [MaxLength(25, ErrorMessage = "Code be longer than 25 characters.")]
        public String? WarehouseCode { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is Required")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        public String? WarehouseDescription { get; set; }

        [Display(Name = "Under")]
        [Required(ErrorMessage = "Under is Required")]
        public String? WarehouseGroup { get; set; }

        [Display(Name = "Warehouse Category")]
        [Required(ErrorMessage = "Category is Required")]
        public String? WarehouseCategory { get; set; }


        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.WarehouseNumber = 0;
            this.WarehouseCode = string.Empty;
            this.WarehouseDescription = string.Empty;
            this.WarehouseGroup = string.Empty;
        }
    }


}

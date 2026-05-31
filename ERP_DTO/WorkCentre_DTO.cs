using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class WorkCentre_DTO
    {
        public Int64 WC_Number { get; set; }

        [Display(Name = "Work Centre")]
        [Required(ErrorMessage = "Work Centre is Required")]
        [MaxLength(25, ErrorMessage = "Work Centre cannot be longer than 25 characters.")]
        public String? WC_WorkCentre { get; set; }

        [Display(Name = "Description")]
        [MaxLength(100, ErrorMessage = "Description cannot be longer than 100 characters.")]
        public String? WC_Description { get; set; }

        [Display(Name = "Work Centre Group")]
        [Required(ErrorMessage = "Work Centre Group is Required")]
        public Int64 WC_WCG_Number { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public Int64 WC_Warehouse_Number { get; set; }

        [Display(Name = "Process")]
        [Required(ErrorMessage = "Process is Required")]
        public Int64 WC_PRS_Number { get; set; }
        public String? WC_WarehouseName { get; set; }

        public String? WC_WorkcentreGroup { get; set; }

        public String? WC_ProcessName { get; set; }

        public String? WC_DeleteNumbers { get; set; }

        public Int64 WC_CreatorCode { get; set; }

        public Int16 WC_Id { get; set; }

        public void Reset()
        {
            this.WC_Number = 0;
            this.WC_WorkCentre = string.Empty;
            this.WC_Description = string.Empty;
            this.WC_WCG_Number = 0;
            this.WC_Warehouse_Number = 0;
            this.WC_PRS_Number = 0;
            this.WC_DeleteNumbers = string.Empty;
            this.WC_CreatorCode = 0;
            this.WC_Id = 0;
        }
    }
}

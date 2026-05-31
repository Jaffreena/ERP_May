using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class UoM_DTO
    {
        public Int64 UnitNumber { get; set; }

        [Display(Name = "Unit Code")]
        [Required(ErrorMessage = "Unit Code is Required")]
        [MaxLength(15, ErrorMessage = "Unit Code be longer than 15 characters.")]
        public String? UnitCode { get; set; }

        [Display(Name = "Unit Description")]
        [MaxLength(25, ErrorMessage = "Unit Description be longer than 25 characters.")]
        [Required(ErrorMessage = "Unit Description is Required")]
        [DataType(DataType.MultilineText)]
        public String? UnitDescription { get; set; }

        [Display(Name = "Number of decimal places")]
        [Required(ErrorMessage = "Number of decimal places is Required")]
        [Range(0, 9, ErrorMessage = "Number of decimal places be longer than 1 characters.")]
        [MaxLength(1, ErrorMessage = "Number of decimal be longer than 1 characters.")]
        [MinLength(1, ErrorMessage = "Number of decimal places be more than 1 characters.")]
        public String? DecimalPlaces { get; set; }

        public String? DeleteNumbers { get; set; }

        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.UnitNumber = 0;
            this.UnitCode = string.Empty;
            this.UnitDescription = string.Empty;
            this.DecimalPlaces = string.Empty;
            this.DeleteNumbers = string.Empty;
        }
    }
}

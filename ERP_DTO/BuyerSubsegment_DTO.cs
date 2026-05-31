using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class BuyerSubsegment_DTO
    {
        public Int64 BSS_Number { get; set; }

        [Display(Name = "Buyer Subsegment")]
        [Required(ErrorMessage = "Buyer Subsegment is Required")]
        [MaxLength(30, ErrorMessage = "Buyer Subsegment be longer than 30 characters.")]
        public String? BSS_SubSegment { get; set; }

        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        [DataType(DataType.MultilineText)]
        public String? BSS_Description { get; set; }
        [Display(Name = "Under")]
        public String? BSS_Under_BSS_Number { get; set; }
        public String? BSS_DeleteNumbers { get; set; }

        public Int64 BSS_CreatorCode { get; set; }

        public Int16 BSS_Id { get; set; }

        public void Reset()
        {
            this.BSS_Number = 0;
            this.BSS_SubSegment = string.Empty;
            this.BSS_Description = string.Empty;
            this.BSS_Under_BSS_Number = string.Empty;
            this.BSS_DeleteNumbers = string.Empty;
            this.BSS_CreatorCode = 0;
            this.BSS_Id = 0;
        }
    }
}


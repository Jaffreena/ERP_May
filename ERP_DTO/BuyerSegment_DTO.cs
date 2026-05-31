using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class BuyerSegment_DTO
    {
        public Int64 BYS_Number { get; set; }

        [Display(Name = "Buyer Segment")]
        [Required(ErrorMessage = "Buyer Segment is Required")]
        [MaxLength(30, ErrorMessage = "Buyer Segment be longer than 30 characters.")]
        public String? BYS_Segment { get; set; }

        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        [DataType(DataType.MultilineText)]
        public String? BYS_Description { get; set; }

        [Display(Name = "Under")]
        public String? BYS_Under_BYS_Number { get; set; }

        public String? BYS_DeleteNumbers { get; set; }

        public Int64 BYS_CreatorCode { get; set; }

        public Int16 BYS_Id { get; set; }

        public void Reset()
        {
            this.BYS_Number = 0;
            this.BYS_Segment = string.Empty;
            this.BYS_Description = string.Empty;
            this.BYS_Under_BYS_Number = string.Empty;
            this.BYS_DeleteNumbers = string.Empty;
            this.BYS_CreatorCode = 0;
            this.BYS_Id = 0;
        }
    }
}

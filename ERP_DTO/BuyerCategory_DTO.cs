using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class BuyerCategory_DTO
    {
        public Int64 BYC_Number { get; set; }

        [Display(Name = "Buyer Category")]
        [Required(ErrorMessage = "Buyer Category is Required")]
        [MaxLength(30, ErrorMessage = "Buyer Category be longer than 30 characters.")]
        public String? BYC_Category { get; set; }

        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        [DataType(DataType.MultilineText)]
        public String? BYC_Description { get; set; }

        [Display(Name = "Under")]
        public String? BYC_Under_BYC_Number { get; set; }

        public String? BYC_DeleteNumbers { get; set; }

        public Int64 BYC_CreatorCode { get; set; }

        public Int16 BYC_Id { get; set; }

        public void Reset()
        {
            this.BYC_Number = 0;
            this.BYC_Category = string.Empty;
            this.BYC_Description = string.Empty;
            this.BYC_Under_BYC_Number = string.Empty;
            this.BYC_DeleteNumbers = string.Empty;
            this.BYC_CreatorCode = 0;
            this.BYC_Id = 0;
        }
    }
}

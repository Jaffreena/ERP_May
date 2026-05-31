using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class CustomerCategory_DTO
    {
        public Int64 JCC_Number { get; set; }

        [Display(Name = "JW Customer Category")]
        [Required(ErrorMessage = "JW Customer Category is Required")]
        [MaxLength(50, ErrorMessage = "JW Customer Category should not  be longer than 50 characters.")]
        public String? JCC_JW_CustomerCategory { get; set; }

        [Display(Name = "Description")]
        [MaxLength(250, ErrorMessage = "Description should not  be longer than 250 characters.")]
        [DataType(DataType.MultilineText)]
        public String? JCC_Description { get; set; }

        [Display(Name = "Under")]
        public String? JCC_Under_JCC_Number { get; set; }

        public String? JCC_DeleteNumbers { get; set; }

        public Int64 JCC_CreatorCode { get; set; }

        public Int16 JCC_Id { get; set; }
        public String? JCC_Status { get; set; }

        public void Reset()
        {
            this.JCC_Number = 0;
            this.JCC_JW_CustomerCategory = string.Empty;
            this.JCC_Description = string.Empty;
            this.JCC_Under_JCC_Number = string.Empty;
            this.JCC_DeleteNumbers = string.Empty;
            this.JCC_CreatorCode = 0;
            this.JCC_Id = 0;
            this.JCC_JW_CustomerCategory = string.Empty;
        }
    }
}

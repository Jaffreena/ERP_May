using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{

    public class CustomerGroup_DTO
    {
        public Int64 JCG_Number { get; set; }

        [Display(Name = "JW Customer Group")]
        [Required(ErrorMessage = "JW Customer Group is Required")]
        [MaxLength(50, ErrorMessage = "JW Customer Group be longer than 50 characters.")]
        public String? JCG_JW_CustomerGroup { get; set; }

        [Display(Name = "Description")]
        [MaxLength(250, ErrorMessage = "Description be longer than 250 characters.")]
        public String? JCG_Description { get; set; }

        [Display(Name = "Under")]
        public String? JCG_Under_JCG_Number { get; set; }

        public String? JCG_DeleteNumbers { get; set; }

        public Int64 JCG_CreatorCode { get; set; }

        public Int16 JCG_Id { get; set; }

        public void Reset()
        {
            this.JCG_Number = 0;
            this.JCG_JW_CustomerGroup = string.Empty;
            this.JCG_Description = string.Empty;
            this.JCG_Under_JCG_Number = string.Empty;
            this.JCG_DeleteNumbers = string.Empty;
            this.JCG_CreatorCode = 0;
            this.JCG_Id = 0;

        }
    }


}

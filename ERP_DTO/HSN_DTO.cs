using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class HSN_DTO
    {
        public Int64 HSN_Number { get; set; }

        [Display(Name = "HSN Code")]
        [Required(ErrorMessage = "HSN Code is Required")]
        [MaxLength(8, ErrorMessage = "HSN code requires 8 characters")]
        [MinLength(8, ErrorMessage = "HSN code requires 8 characters")]
        public String? HSN_Code { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is Required")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        public String? Description { get; set; }
        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }
    }
}

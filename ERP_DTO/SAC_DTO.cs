using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class SAC_DTO
    {
        public Int64 SAC_Number { get; set; }

        [Display(Name = "SAC")]
        [Required(ErrorMessage = "SAC is Required")]
        [MaxLength(6, ErrorMessage = "SAC requires 6 characters")]
        [MinLength(6, ErrorMessage = "SAC requires 6 characters")]
        public String? SAC_Code { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is Required")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        public String? Description { get; set; }
        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }
    }
}

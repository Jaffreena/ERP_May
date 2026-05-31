using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class TaxationSubmaster_DTO
    {
        public Int64 Number { get; set; }

        [Display(Name = "Location")]
        public String? Location { get; set; }

        [Required(ErrorMessage = "Title is Required")]
        [MaxLength(50, ErrorMessage = "Title be longer than 50 characters.")]
        public String? Title { get; set; }

        [Display(Name = "Notes")]
        [MaxLength(250, ErrorMessage = "Notes be longer than 250 characters.")]
        [DataType(DataType.MultilineText)]
        public String? Notes { get; set; }
        public String? DeleteNumbers { get; set; }

        public Int32 CreatorCode { get; set; }

        public Int32 Id { get; set; }
    }
}

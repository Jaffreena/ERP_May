using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class Shift_DTO
    {
        public Int64 Number { get; set; }

        [Required(ErrorMessage = "Shift Name is Required")]
        [MaxLength(25, ErrorMessage = "Shift Name should not  be longer than 25 characters.")]
        public String? ShiftName { get; set; }

        [Display(Name = "Description")]
        [MaxLength(100, ErrorMessage = "Description should not be be longer than 100 characters.")]
        [DataType(DataType.MultilineText)]
        public String? Description { get; set; }
        public String? DeleteNumbers { get; set; }

        public Int32 CreatorCode { get; set; }

        public Int32 Id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class BuyerGroup_DTO
    {
        public Int64 BYG_Number { get; set; }

        [Display(Name = "Buyer Group")]
        [Required(ErrorMessage = "Buyer Group is Required")]
        [MaxLength(30, ErrorMessage = "Buyer Group be longer than 30 characters.")]
        public String? BYG_Group { get; set; }

        [Display(Name = "Description")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        public String? BYG_Description { get; set; }

        [Display(Name = "Under")]
        public String? BYG_Under_BYG_Number { get; set; }

        public String? BYG_DeleteNumbers { get; set; }

        public Int64 BYG_CreatorCode { get; set; }

        public Int16 BYG_Id { get; set; }

        public void Reset()
        {
            this.BYG_Number = 0;
            this.BYG_Group = string.Empty;
            this.BYG_Description = string.Empty;
            this.BYG_Under_BYG_Number = string.Empty;
            this.BYG_DeleteNumbers = string.Empty;
            this.BYG_CreatorCode = 0; 
            this.BYG_Id = 0;
        }
    }
}

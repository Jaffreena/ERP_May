using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
  


    public class WorkCentreGroup_DTO
    {
        public Int64 WCG_Number { get; set; }

        [Display(Name = "Work Centre Group")]
        [Required(ErrorMessage = "Work Centre Group is Required")]
        [MaxLength(25, ErrorMessage = "Work Centre Group should not be longer than 25 characters.")]
        public String? WCG_WorkCentreGroup { get; set; }

        [Display(Name = "Description")]
        [MaxLength(100, ErrorMessage = "Description should not be longer than 100 characters.")]
        public String? WCG_Description { get; set; }

        [Display(Name = "Under")]
        public String? WCG_Under_WCG_Number { get; set; }

        public String? WCG_DeleteNumbers { get; set; }

        public Int64 WCG_CreatorCode { get; set; }

        public Int16 WCG_Id { get; set; }

        public void Reset()
        {
            this.WCG_Number = 0;
            this.WCG_WorkCentreGroup = string.Empty;
            this.WCG_Description = string.Empty;
            this.WCG_Under_WCG_Number = string.Empty;
            this.WCG_DeleteNumbers = string.Empty;
            this.WCG_CreatorCode = 0;
            this.WCG_Id = 0;

        }
    }

}

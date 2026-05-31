using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class IncomeCode_DTO
    {
        public Int64 MIC_Number { get; set; }

        [Display(Name = "Income Code")]
        [Required(ErrorMessage = "Income Code is Required")]
        [MaxLength(30, ErrorMessage = "Income Code be longer than 30 characters.")]
        public String? MIC_Code { get; set; }

        [Display(Name = "Description")]
        //[Required(ErrorMessage = "Description is Required")]
        [MaxLength(50, ErrorMessage = "Description be longer than 50 characters.")]
        public String? MIC_Description { get; set; }

        [Display(Name = "Occurrence")]
        [Required(ErrorMessage = "Occurrence is Required")]
        public String? MIC_OCRN_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? MIC_CM_Number { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? MIC_ALCT_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? MIC_LA_Number { get; set; }

        [Display(Name = "SAC")]
        [Required(ErrorMessage = "SAC is Required")]
        public String? MIC_SAC_Number { get; set; }

        public String? MIC_DeleteNumbers { get; set; }
        public Int64 MIC_CreatorCode { get; set; }
        public Int16 MIC_Id { get; set; }

        public void Reset()
        {
            this.MIC_Number = 0;
            this.MIC_Code = string.Empty;
            this.MIC_Description = string.Empty;
            this.MIC_OCRN_Number = string.Empty;
            this.MIC_CM_Number = string.Empty;
            this.MIC_ALCT_Number = string.Empty;
            this.MIC_LA_Number = string.Empty;
            this.MIC_SAC_Number = string.Empty;
            this.MIC_DeleteNumbers = string.Empty;
            this.MIC_CreatorCode = 0;
            this.MIC_Id = 0;

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class WH_TaxAssign_DTO
    {
        public Int64 WH_TaxNumber { get; set; }

        public String? WH_TaxCode { get; set; }

        public String? WH_TaxDescription { get; set; }

        public Int64? WH_TaxAssignNumber { get; set; }

        public Int64? AssesseeNature { get; set; }

        public String? FromDate { get; set; }

        public String? ToDate { get; set; }

        public Double? SingleTransLimit { get; set; }

        public Double? AggregateTransLimit { get; set; }

        public Int64? IncludeTax { get; set; }

        public Double? PAN_TaxPercent { get; set; }

        public Double? NON_PAN_TaxPercent { get; set; }

        public Int64? COA { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.WH_TaxNumber = 0;
            this.WH_TaxCode = "0";
            this.WH_TaxDescription = string.Empty;
            this.AssesseeNature = 0; 
            this.FromDate = "0";
            this.ToDate = "0";
            this.SingleTransLimit = 0;
            this.AggregateTransLimit = 0;
            this.IncludeTax = 0;
            this.PAN_TaxPercent = 0;
            this.NON_PAN_TaxPercent = 0;
            this.COA = 0;
        }
    }

    public class WH_TaxAssignDetail_DTO
    {
        public Int64 WH_TaxAssignNumber { get; set; }

        [Display(Name = "Nature of assessee")]
        [Required(ErrorMessage = "Nature of assessee is Required")]
        [MaxLength(25, ErrorMessage = "Nature of assessee be longer than 25 characters.")]
        public String? AssesseeNature { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is Required")]
        [MaxLength(25, ErrorMessage = "From Date be longer than 25 characters.")]
        public String? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is Required")]
        [MaxLength(25, ErrorMessage = "To Date be longer than 25 characters.")]
        public String? ToDate { get; set; }

        [Display(Name = "Single Transaction")]
        [Required(ErrorMessage = "Single Transaction is Required")]
        [MaxLength(25, ErrorMessage = "Single Transaction be longer than 25 characters.")]
        public String? SingleTransLimit { get; set; }

        [Display(Name = "Aggregate Transaction")]
        [Required(ErrorMessage = "Aggregate Transaction is Required")]
        [MaxLength(25, ErrorMessage = "Aggregate Transaction be longer than 25 characters.")]
        public String? AggregateTransLimit { get; set; }

        [Display(Name = "Include Tax")]
        [Required(ErrorMessage = "Include Tax is Required")]
        [MaxLength(25, ErrorMessage = "Include Tax be longer than 25 characters.")]
        public String? IncludeTax { get; set; }

        [Display(Name = "Tax Percentage")]
        [Required(ErrorMessage = "Tax Percentage is Required")]
        [MaxLength(25, ErrorMessage = "Tax Percentage be longer than 25 characters.")]
        public String? PAN_TaxPercent { get; set; }

        [Display(Name = "PAN non-availability %")]
        [Required(ErrorMessage = "PAN non-availability % is Required")]
        [MaxLength(25, ErrorMessage = "PAN non-availability % be longer than 25 characters.")]
        public String? NON_PAN_TaxPercent { get; set; }

        [Display(Name = "Assign COA")]
        [Required(ErrorMessage = "Assign COA is Required")]
        [MaxLength(25, ErrorMessage = "Assign COA be longer than 25 characters.")]
        public String? COA { get; set; }
        public Boolean IsDeleted { get; set; }


        public void Reset()
        {
            this.WH_TaxAssignNumber = 0;
            this.AssesseeNature = string.Empty;
            this.FromDate = string.Empty;
            this.ToDate = string.Empty;
            this.SingleTransLimit = string.Empty;
            this.AggregateTransLimit = string.Empty;
            this.IncludeTax = string.Empty;
            this.PAN_TaxPercent = string.Empty;
            this.NON_PAN_TaxPercent = string.Empty;
            this.COA = string.Empty;
        }
    }
    public class WH_TaxAssignHead_DTO
    {
        public Int64 WH_TaxNumber { get; set; }

        [Display(Name = "WH Tax Code")]
        [Required(ErrorMessage = "WH Tax Code is Required")]
        [MaxLength(9, ErrorMessage = "WH Tax Code be longer than 9 characters.")]
        public String? WH_TaxCode { get; set; }

        [Display(Name = "WH Tax Description")]
        public String? WH_TaxDescription { get; set; }

        public List<WH_TaxAssignDetail_DTO>? WH_TaxDetail { get; set; } = new List<WH_TaxAssignDetail_DTO>();

        public PaginatedList_DTO<WH_TaxAssign_DTO>? WH_TaxAssign_List { get; set; }


        public void Reset()
        {
            this.WH_TaxNumber = 0;
            this.WH_TaxCode = string.Empty;
            this.WH_TaxDescription = string.Empty;
            this.WH_TaxDetail = null;
            this.WH_TaxAssign_List = null;
        }
    }
}

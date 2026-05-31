using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class TaxElement_DTO
    {
        public Int64 TaxElementNumber { get; set; }
        public Int64 TaxNumber { get; set; }
        public String? TaxElement { get; set; }
        public String? ElementDescription { get; set; }
        public String? TaxCategory { get; set; }
        public String? TaxType { get; set; }

        public Boolean LoadonInventory { get; set; }
        public String? LoadonInventoryPercent { get; set; }
        public String? COA_LedgerAccount { get; set; }
        public String? GST_Abatement { get; set; }
        public String? GST_TaxNature { get; set; }

        public String? FromDate { get; set; }
        public String? ToDate { get; set; }
        public String? FixedPercent { get; set; }

        public String? HSN { get; set; }
        public String? HSNPercent { get; set; }
        public String? SAC { get; set; }
        public String? SACPercent { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.TaxElementNumber = 0;
            this.TaxElement = "";
            this.ElementDescription = string.Empty;
            this.TaxCategory = "0";
            this.TaxType = "0";
            this.LoadonInventory = false;
            this.LoadonInventoryPercent = "0";
            this.COA_LedgerAccount = "0";
            this.GST_Abatement = "0";
            this.COA_LedgerAccount = "0";
            this.GST_TaxNature = "0";
            this.FromDate = "0";
            this.ToDate = "0";
            this.FixedPercent = "0";
            this.GST_TaxNature = "0";
            this.HSN = "0";
            this.HSNPercent = "0";
            this.SAC = "0";
            this.SACPercent = "0";
        }
    }
    public class TaxElementHead_DTO
    {
        public Int64 TaxElementNumber { get; set; }

        [Display(Name = "Tax Element")]
        [Required(ErrorMessage = "Tax Element is Required")]
        [MaxLength(25, ErrorMessage = "Tax Element be longer than 25 characters.")]
        public String? TaxElement { get; set; }

        [Display(Name = "Element Description")]
        [Required(ErrorMessage = "Element Description is Required")]
        [MaxLength(50, ErrorMessage = "Element Description be longer than 50 characters.")]
        public String? ElementDescription { get; set; }

        [Display(Name = "Tax Category")]
        [Required(ErrorMessage = "Tax Category is Required")]
        [MaxLength(25, ErrorMessage = "Tax Category be longer than 25 characters.")]
        public String? TaxCategory { get; set; }

        [Display(Name = "Tax Type")]
        [Required(ErrorMessage = "Tax Type is Required")]
        [MaxLength(25, ErrorMessage = "Tax Type be longer than 25 characters.")]
        public String? TaxType { get; set; }

        [Display(Name = "Load on Inventory")]
        public Boolean LoadonInventory { get; set; }

        [Display(Name = "Percent")]
        public String? LoadonInventoryPercent { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        [MaxLength(25, ErrorMessage = "Ledger Account be longer than 25 characters.")]
        public String? COA_LedgerAccount { get; set; }

        [Display(Name = "Abatement")]
        [Required(ErrorMessage = "Abatement is Required")]
        [MaxLength(25, ErrorMessage = "Abatement be longer than 25 characters.")]
        public String? GST_Abatement { get; set; }

        [Display(Name = "Tax Nature")]
        [Required(ErrorMessage = "Tax Nature is Required")]
        [MaxLength(25, ErrorMessage = "Tax Nature be longer than 25 characters.")]
        public String? GST_TaxNature { get; set; }
        public PaginatedList_DTO<TaxElement_DTO>? TaxElement_List { get; set; }
        public List<TaxElementDetail_DTO>? Abatement_List { get; set; }
        public List<TaxElementDetail_DTO>? Fixed_Nature_List { get; set; }
        public List<TaxElementVariableDetail_DTO>? Variable_Nature_List { get; set; }

        public void Reset()
        {
            this.TaxElementNumber = 0;
            this.TaxElement = "";
            this.ElementDescription = string.Empty;
            this.TaxCategory = "0";
            this.TaxType = "0";
            this.LoadonInventory = false;
            this.LoadonInventoryPercent = "0";
            this.COA_LedgerAccount = "0";
            this.GST_Abatement = "0";
            this.COA_LedgerAccount = "0";
            this.GST_TaxNature = "0";
            this.TaxElement_List = null;
            this.Abatement_List = null;
            this.Fixed_Nature_List = null;
            this.Variable_Nature_List = null;
        }
    }

    public class TaxElementDetail_DTO
    {
        public Int64 TaxNumber { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is Required")]
        public String? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is Required")]
        public String? ToDate { get; set; }

        [Display(Name = "Fixed Percent")]
        [Required(ErrorMessage = "Fixed Percent is Required")]
        public String? FixedPercent { get; set; }
        public Boolean IsDeleted { get; set; }

        public void Reset()
        {
            this.TaxNumber = 0;
            this.FromDate = "0";
            this.FixedPercent = "0";
        }
    }

    public class TaxElementVariableDetail_DTO
    {
        public Int64 TaxNumber { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "Variable Fixed From Date is Required")]
        public String? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "Variable Fixed To Date is Required")]
        public String? ToDate { get; set; }

        [Display(Name = "HSN")]
        public String? HSN { get; set; }

        [Display(Name = "HSN Percent")]
        public String? HSNPercent { get; set; }

        [Display(Name = "SAC")]
        public String? SAC { get; set; }

        [Display(Name = "SAC Percent")]
        public String? SACPercent { get; set; }
        public Boolean IsDeleted { get; set; }

        public void Reset()
        {
            this.TaxNumber = 0;
            this.FromDate = "0";
            this.HSN = "0";
            this.HSNPercent = "0";
            this.SAC = "0";
            this.SACPercent = "0";
        }
    }
}

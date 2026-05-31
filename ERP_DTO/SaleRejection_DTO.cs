using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class SaleRejection_DTO
    {
        public Int64 SR_Number { get; set; }
        public String? SR_RejectionNo { get; set; }
        public Int32 SR_RejectionDate { get; set; }
        public String? SR_ExchangeRate { get; set; }
        public Int64 SR_BUY_Number { get; set; }
        public Int64 SR_BUY_LOC_Number { get; set; }
        public Int64 SR_ExportOrder { get; set; }
        public Int64 SR_CUR_Number { get; set; }
        public Int32 SR_CUR_DecimalPlaces { get; set; }
        public Int64 SR_TCT_Number { get; set; }
        public Int64 SR_WHT_Number { get; set; }
        public Int64 SR_MS_Number { get; set; }
        public Double SR_WHT_Tax { get; set; }
        public Double SR_WHT_Percent { get; set; }

        public Double SR_MaterialCost { get; set; }
        public Double SR_ItemMiscIncome { get; set; }
        public Double SR_HeaderMiscIncome { get; set; }
        public Double SR_GST_Amount { get; set; }
        public Double SR_RejectionAmount { get; set; }
        public Double SR_WHT_Amount { get; set; }
        public Double SR_RoundOff { get; set; }
        public Double SR_BuyerReceivable { get; set; }


        public Int64 SR_I_Number { get; set; }
        public Int64 SR_WH_Number { get; set; }
        public Int64 SR_ITM_Number { get; set; }
        public String? SR_ITM_Code { get; set; }
        public Int64 SR_UoM_Number { get; set; }
        public Double SR_Qty { get; set; }
        public Double SR_UnitPrice { get; set; }
        public Double SR_Amount { get; set; }
        public Double SR_IncomeValue { get; set; }
        public Int64 SR_HSN_Number { get; set; }


        public Int64 SR_INC_Number { get; set; }
        public Int64 SR_INC_MIC_Number { get; set; }
        public String? SR_INC_Remarks { get; set; }
        public Int64 SR_INC_OCRN_Number { get; set; }
        public Int64 SR_INC_CM_Number { get; set; }
        public Double SR_INC_IncomeBase { get; set; }
        public Double SR_INC_IncomeValue { get; set; }
        public Int64 SR_INC_ALCT_Number { get; set; }
        public Int64 SR_INC_LA_Number { get; set; }
        public Int64 SR_INC_CalculateGST { get; set; }
        public Double SR_INC_GST_Amount { get; set; }
        public Int64 SR_INC_SAC_Number { get; set; }
        public Double SR_INC_WHT_Percent { get; set; }
        public Double SR_INC_WHT_Amount { get; set; }

        public Int64 SR_BCH_Number { get; set; }
        public Int64 SR_BCH_BCH_Number { get; set; }
        public String? SR_BCH_Date { get; set; }
        public String? SR_BCH_No { get; set; }
        public Double SR_BCH_Qty { get; set; }
        public Double SR_BCH_UnitPrice { get; set; }
        public Double SR_BCH_Value { get; set; }
        public Int32 SR_BCH_Mode { get; set; }
        public Int32 SR_BCH_Index { get; set; }

        public Int64 SR_ADD_Number { get; set; }
        public Int64 SR_ADD_ADTP_Number { get; set; }
        public String? SR_ADD_AddressID { get; set; }
        public String? SR_ADD_Address { get; set; }
        public String? SR_ADD_City { get; set; }
        public String? SR_ADD_State { get; set; }
        public String? SR_ADD_Country { get; set; }
        public String? SR_ADD_Pin { get; set; }
        public String? SR_ADD_GSTIN { get; set; }

        public String? SR_Search { get; set; }
        public String? SR_DeleteNumbers { get; set; }
        public Int64 SR_CreatorCode { get; set; }
        public Int16 SR_Id { get; set; }

        public void Reset()
        {
            this.SR_Number = 0;
            this.SR_RejectionNo = "";
            this.SR_RejectionDate = 0;
            this.SR_ExchangeRate = "";
            this.SR_BUY_Number = 0;
            this.SR_BUY_LOC_Number = 0;
            this.SR_ExportOrder = 0;
            this.SR_CUR_Number = 0;
            this.SR_CUR_DecimalPlaces = 0;
            this.SR_TCT_Number = 0;
            this.SR_WHT_Number = 0;
            this.SR_MS_Number = 0;
            this.SR_WHT_Tax = 0;
            this.SR_WHT_Percent = 0;

            this.SR_MaterialCost = 0;
            this.SR_ItemMiscIncome = 0;
            this.SR_HeaderMiscIncome = 0;
            this.SR_GST_Amount = 0;
            this.SR_RejectionAmount = 0;
            this.SR_WHT_Amount = 0;
            this.SR_RoundOff = 0;
            this.SR_BuyerReceivable = 0;


            this.SR_WH_Number = 0;
            this.SR_ITM_Number = 0;
            this.SR_UoM_Number = 0;
            this.SR_Qty = 0;
            this.SR_UnitPrice = 0;
            this.SR_Amount = 0;
            this.SR_IncomeValue = 0;
            this.SR_HSN_Number = 0;

            this.SR_INC_MIC_Number = 0;
            this.SR_INC_Remarks = "";
            this.SR_INC_OCRN_Number = 0;
            this.SR_INC_CM_Number = 0;
            this.SR_INC_IncomeBase = 0;
            this.SR_INC_IncomeValue = 0;
            this.SR_INC_ALCT_Number = 0;
            this.SR_INC_LA_Number = 0;
            this.SR_INC_CalculateGST = 0;
            this.SR_INC_GST_Amount = 0;
            this.SR_INC_SAC_Number = 0;

            this.SR_BCH_Number = 0;
            this.SR_BCH_BCH_Number = 0;
            this.SR_BCH_Date = "";
            this.SR_BCH_No = "";
            this.SR_BCH_Qty = 0;
            this.SR_BCH_UnitPrice = 0;
            this.SR_BCH_Value = 0;

            this.SR_Search = "";
            this.SR_DeleteNumbers = "";
            this.SR_CreatorCode = 0;
            this.SR_Id = 0;
        }
    }


    public class SaleRejectionRegister_DTO
    {
        public Int64 SR_Number { get; set; }
        public String? SR_RejectionNo { get; set; }
        public String? SR_RejectionDate { get; set; }
        public String? SR_SOH_OrderNo { get; set; }
        public String? SR_SOH_OrderDate { get; set; }
        public Int32 SR_SOH_OrderCount { get; set; }
        public String? SR_SIH_InvoiceNo { get; set; }
        public String? SR_SIH_InvoiceDate { get; set; }
        public Int32 SR_SIH_InvoiceCount { get; set; }
        public String? SR_ExportOrder { get; set; }
        public String? SR_BYC_Category { get; set; }
        public String? SR_BYG_Group { get; set; }
        public String? SR_BUY_Name { get; set; }
        public String? SR_MS_Name { get; set; }
        public String? SR_ITM_Group { get; set; }
        public String? SR_ITM_Code { get; set; }
        public String? SR_ITM_Description { get; set; }
        public String? SR_ITM_OuterDia { get; set; }
        public String? SR_ITM_Thickness { get; set; }
        public String? SR_ITM_Length { get; set; }
        public String? SR_ITM_MaterialGrade { get; set; }
        public String? SR_WH_Name { get; set; }
        public String? SR_Qty { get; set; }
        public String? SR_UoM_Name { get; set; }
        public String? SR_NoOfItem { get; set; }
        public String? SR_CUR_Name { get; set; }
        public String? SR_UnitPrice { get; set; }
        public String? SR_MaterialValue { get; set; }
        public String? SR_TotalItemIncome { get; set; }
        public String? SR_TotalHeadIncome { get; set; }
        public String? SR_TotalAmount { get; set; }
        public String? SR_ExchangeRate { get; set; }
        public String? SR_HeadGST_Amount { get; set; }
        public String? SR_ItemGST_Amount { get; set; }
        public String? SR_RejectionAmount { get; set; }
        public String? SR_HeadWHT_Amount { get; set; }
        public String? SR_ItemWHT_Amount { get; set; }
        public String? SR_RoundOff { get; set; }
        public String? SR_BuyerReceivable { get; set; }
    }


    public class SaleRejectionHead_DTO
    {
        public Int64 SRH_Number { get; set; }

        [Display(Name = "Sale Rejection No")]
        [Required(ErrorMessage = "Sale Rejection No is Required")]
        public String? SRH_RejectionNo { get; set; }

        [Display(Name = "Date")]
        [MaxLength(15, ErrorMessage = "Sale Rejection Date be longer than 15 characters.")]
        [Required(ErrorMessage = "Sale Rejection Date is Required")]
        public String? SRH_RejectionDate { get; set; }

        [Display(Name = "Exchange Rate")]
        public String? SRH_ExchangeRate { get; set; }

        [Display(Name = "Buyer Name")]
        [Required(ErrorMessage = "Buyer Name is Required")]
        public String? SRH_BUY_Number { get; set; }
        public String? SRH_BUY_Name { get; set; }
        public String? SRH_BUY_LOC_Number { get; set; }

        [Display(Name = "Export Order")]
        [Required(ErrorMessage = "Import Order is Required")]
        public String? SRH_ExportOrder { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public String? SRH_CUR_Number { get; set; }
        public String? SRH_CUR_Name { get; set; }
        public String? SRH_CUR_DecimalPlaces { get; set; }

        [Display(Name = "Tax Cluster")]
        [Required(ErrorMessage = "Tax Cluster is Required")]
        public String? SRH_TCT_Number { get; set; }
        public String? SRH_HIDE_TCT_Number { get; set; }

        [Display(Name = "WH Tax Code")]
        [Required(ErrorMessage = "WH Tax Code is Required")]
        public String? SRH_WHT_Number { get; set; }
        public String? SRH_WHT_Code { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public String? SRH_MS_Number { get; set; }

        public String? SRH_WHT_Tax { get; set; }
        public String? SRH_WHT_Percent { get; set; }


        [Display(Name = "Material Cost")]
        public String? SRH_MaterialCost { get; set; }

        [Display(Name = "Item Misc.Income")]
        public String? SRH_ItemMiscIncome { get; set; }

        [Display(Name = "Header Misc.Income")]
        public String? SRH_HeaderMiscIncome { get; set; }

        [Display(Name = "GST Amount")]
        public String? SRH_GST_Amount { get; set; }

        [Display(Name = "Rejection Amount")]
        public String? SRH_RejectionAmount { get; set; }

        [Display(Name = "WHT Amount")]
        public String? SRH_WHT_Amount { get; set; }

        [Display(Name = "Round Off")]
        public String? SRH_RoundOff { get; set; }

        [Display(Name = "Buyer Receivable")]
        public String? SRH_BuyerReceivable { get; set; }

        public List<SaleRejectionItem_DTO>? RejectionItem { get; set; }
        public List<SaleRejectionIncome_DTO>? Income { get; set; }
        public List<SaleRejectionIIncome_DTO>? ItemIncome { get; set; }
        public List<SaleRejectionBatch_DTO>? ItemBatch { get; set; }
        public List<SaleRejectionAddress_DTO>? BuyerAddress { get; set; }

        public Int32 SRH_Mode { get; set; }

        public void Reset()
        {
            this.SRH_Number = 0;
            this.SRH_RejectionNo = null;
            this.SRH_RejectionDate = "";
            this.SRH_RejectionDate = "";
            this.SRH_BUY_Number = null;
            this.SRH_BUY_Name = null;
            this.SRH_BUY_LOC_Number = null;
            this.SRH_ExportOrder = null;
            this.SRH_CUR_Number = null;
            this.SRH_CUR_Name = null;
            this.SRH_CUR_DecimalPlaces = null;
            this.SRH_HIDE_TCT_Number = null;
            this.SRH_TCT_Number = null;
            this.SRH_WHT_Number = "";
            this.SRH_WHT_Tax = "";
            this.SRH_WHT_Percent = "";
            this.SRH_MS_Number = "";

            this.RejectionItem = null;
            this.Income = null;
            this.ItemIncome = null;
            this.ItemBatch = null;

            this.SRH_MaterialCost = "";
            this.SRH_ItemMiscIncome = "";
            this.SRH_HeaderMiscIncome = "";
            this.SRH_GST_Amount = "";
            this.SRH_RejectionAmount = "";
            this.SRH_WHT_Amount = "";
            this.SRH_RoundOff = "";
            this.SRH_BuyerReceivable = "";
        }

    }
    public class SaleRejectionItem_DTO
    {
        public Int64 SRI_Number { get; set; }
        public Int64 SRI_MS_Number { get; set; }
        public Int64 SRI_Index { get; set; }
        public String? SRI_SIH_InvoiceNo { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public Int64 SRI_WH_Number { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public Int64 SRI_ITM_Number { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? SRI_ITM_Code { get; set; }

        [Display(Name = "Item Group")]
        public String? SRI_ITM_Group { get; set; }

        [Display(Name = "Outer Dia")]
        public String? SRI_ITM_OuterDia { get; set; }

        [Display(Name = "Thickness")]
        public String? SRI_ITM_Thickness { get; set; }

        [Display(Name = "Length")]
        public String? SRI_ITM_Length { get; set; }

        [Display(Name = "Description")]
        public String? SRI_ITM_Description { get; set; }

        [Display(Name = "Material Grade")]
        public String? SRI_ITM_MaterialGrade { get; set; }

        [Display(Name = "UoM")]
        [Required(ErrorMessage = "UoM is Required")]
        public Int64 SRI_UoM_Number { get; set; }
        public Int64 SRI_HIDE_UoM_Number { get; set; }
        public String? SRI_UoM_DecimalPlaces { get; set; }

        [Display(Name = "SO Qty")]
        public String? SRI_SOI_Qty { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Qty is Required")]
        public String? SRI_Qty { get; set; }

        [Display(Name = "SI Qty")]
        public String? SRI_OLD_Qty { get; set; }

        [Display(Name = "Returned Qty")]
        public String? SRI_SRI_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public String? SRI_UnitPrice { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        public String? SRI_Amount { get; set; }

        [Display(Name = "Misc. Income Value")]
        [Required(ErrorMessage = "Misc. Income Value is Required")]
        public String? SRI_IncomeValue { get; set; }

        [Display(Name = "HSN")]
        [Required(ErrorMessage = "HSN is Required")]
        public Int64 SRI_HSN_Number { get; set; }

        [Display(Name = "GST Amount")]
        [Required(ErrorMessage = "GST Amount is Required")]
        public String? SRI_GST_Amount { get; set; }

        [Display(Name = "WHT Percent")]
        [Required(ErrorMessage = "WHT Percent is Required")]
        public String? SRI_WHT_Percent { get; set; }

        [Display(Name = "WH_Tax Amount")]
        [Required(ErrorMessage = "WH_Tax Amount is Required")]
        public String? SRI_WHT_Amount { get; set; }

        public Int16 SRI_IsDeleted { get; set; }

        public void Reset()
        {
            this.SRI_Number = 0;
            this.SRI_ITM_Number = 0;
            this.SRI_ITM_Code = "";
            this.SRI_UoM_DecimalPlaces = "";
            this.SRI_ITM_Group = "";
            this.SRI_ITM_Description = "";
            this.SRI_ITM_OuterDia = "";
            this.SRI_ITM_Thickness = "";
            this.SRI_ITM_Length = "";
            this.SRI_ITM_MaterialGrade = "";
            this.SRI_UoM_Number = 0;
            this.SRI_HIDE_UoM_Number = 0;
            this.SRI_Qty = "";
            this.SRI_UnitPrice = "";
            this.SRI_Amount = "";
            this.SRI_IncomeValue = "";
            this.SRI_HSN_Number = 0;
            this.SRI_GST_Amount = "";
            this.SRI_WHT_Percent = "";
            this.SRI_WHT_Amount = "";
            this.SRI_IsDeleted = 0;
        }
    }
    public class SaleRejectionIncome_DTO
    {
        public Int64 SRH_INC_Number { get; set; }
        public Int64 SRH_INC_SRH_Number { get; set; }
        public String? SRH_INC_SIH_InvoiceNo { get; set; }

        [Display(Name = "Income Code")]
        [Required(ErrorMessage = "Income Code is Required")]
        public String? SRH_INC_MIC_Number { get; set; }

        [Display(Name = "Description")]
        public String? SRH_INC_MIC_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? SRH_INC_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? SRH_INC_OCRN_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? SRH_INC_CM_Number { get; set; }

        [Display(Name = "Income Base")]
        [Required(ErrorMessage = "Income Base is Required")]
        public String? SRH_INC_IncomeBase { get; set; }

        [Display(Name = "Income Value")]
        [Required(ErrorMessage = "Income Value is Required")]
        public String? SRH_INC_IncomeValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? SRH_INC_ALCT_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? SRH_INC_LA_Number { get; set; }

        [Display(Name = "Calculate GST")]
        [Required(ErrorMessage = "Calculate GST is Required")]
        public String? SRH_INC_CalculateGST { get; set; }

        [Display(Name = "GST Amount")]
        [Required(ErrorMessage = "GST Amount is Required")]
        public String? SRH_INC_GST_Amount { get; set; }

        [Display(Name = "WHT Percent")]
        [Required(ErrorMessage = "WHT Percent is Required")]
        public String? SRH_INC_WHT_Percent { get; set; }

        [Display(Name = "WHT Amount")]
        [Required(ErrorMessage = "WHT Amount is Required")]
        public String? SRH_INC_WHT_Amount { get; set; }
        public String? SRH_INC_SAC_Number { get; set; }
        public Int16 SRH_INC_IsDeleted { get; set; }
        public void Reset()
        {
            this.SRH_INC_Number = 0;
            this.SRH_INC_MIC_Number = "";
            this.SRH_INC_MIC_Description = "";
            this.SRH_INC_Remarks = "";
            this.SRH_INC_OCRN_Number = "";
            this.SRH_INC_CM_Number = "";
            this.SRH_INC_IncomeBase = "";
            this.SRH_INC_IncomeValue = "";
            this.SRH_INC_ALCT_Number = "";
            this.SRH_INC_CalculateGST = "";
            this.SRH_INC_GST_Amount = "";
            this.SRH_INC_WHT_Percent = "";
            this.SRH_INC_WHT_Amount = "";
            this.SRH_INC_LA_Number = "";
            this.SRH_INC_SAC_Number = "";
            this.SRH_INC_IsDeleted = 0;
        }
    }
    public class SaleRejectionIIncome_DTO
    {
        public Int64 SRI_INC_Number { get; set; }
        public Int64 SRI_INC_SRI_Number { get; set; }
        public String? SRI_INC_ITM_Number { get; set; }
        public Int64 SRI_INC_ITM_Index { get; set; }

        [Display(Name = "Income Code")]
        [Required(ErrorMessage = "Income Code is Required")]
        public String? SRI_INC_MIC_Number { get; set; }

        [Display(Name = "Description")]
        public String? SRI_INC_MIC_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? SRI_INC_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? SRI_INC_OCRN_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? SRI_INC_CM_Number { get; set; }

        [Display(Name = "Income Base")]
        [Required(ErrorMessage = "Income Base is Required")]
        public String? SRI_INC_IncomeBase { get; set; }

        [Display(Name = "Income Value")]
        [Required(ErrorMessage = "Income Value is Required")]
        public String? SRI_INC_IncomeValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? SRI_INC_ALCT_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? SRI_INC_LA_Number { get; set; }
        public Int16 SRI_INC_IsDeleted { get; set; }

        public void Reset()
        {
            this.SRI_INC_Number = 0;
            this.SRI_INC_ITM_Number = "";
            this.SRI_INC_ITM_Index = 0;
            this.SRI_INC_MIC_Number = "";
            this.SRI_INC_MIC_Description = "";
            this.SRI_INC_Remarks = "";
            this.SRI_INC_OCRN_Number = "";
            this.SRI_INC_CM_Number = "";
            this.SRI_INC_IncomeBase = "";
            this.SRI_INC_IncomeValue = "";
            this.SRI_INC_ALCT_Number = "";
            this.SRI_INC_LA_Number = "";
            this.SRI_INC_IsDeleted = 0;
        }
    }
    public class SaleRejectionBatch_DTO
    {
        public Int64 SRI_BCH_Number { get; set; }
        public Int64 SRI_BCH_SRI_Number { get; set; }
        public Int64 SRI_BCH_ITM_Index { get; set; }
        public Int64 SRI_BCH_ITM_Number { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Batch Date is Required")]
        public String? SRI_BCH_Date { get; set; }

        [Display(Name = "Batch Number")]
        [Required(ErrorMessage = "Batch Number is Required")]
        public String? SRI_BCH_No { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Batch Qty is Required")]
        public String? SRI_BCH_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Batch Unit Price is Required")]
        public String? SRI_BCH_UnitPrice { get; set; }

        [Display(Name = "Batch Value")]
        [Required(ErrorMessage = "Batch Qty is Required")]
        public String? SRI_BCH_Value { get; set; }
        public Int16 SRI_BCH_IsDeleted { get; set; }
        public void Reset()
        {
            this.SRI_BCH_Number = 0;
            this.SRI_BCH_ITM_Number = 0;
            this.SRI_BCH_Date = "";
            this.SRI_BCH_No = "";
            this.SRI_BCH_Qty = "";
            this.SRI_BCH_UnitPrice = "";
            this.SRI_BCH_Value = "";
            this.SRI_BCH_IsDeleted = 0;
        }
    }
    public class SaleRejectionAddress_DTO
    {
        public Int64 SRH_ADD_Number { get; set; }
        public Int64 SRH_ADD_SRH_Number { get; set; }
        public Int64 SRH_ADD_BUY_ADD_Number { get; set; }

        [Display(Name = "Address Type")]
        public Int64 SRH_ADD_ADTP_Number { get; set; }

        [Display(Name = "Address ID")]
        public String? SRH_ADD_AddressID { get; set; }

        [Display(Name = "Address")]
        public String? SRH_ADD_Address { get; set; }

        [Display(Name = "City")]
        public String? SRH_ADD_City { get; set; }

        [Display(Name = "State")]
        public String? SRH_ADD_State { get; set; }

        [Display(Name = "Country")]
        public String? SRH_ADD_Country { get; set; }

        [Display(Name = "PIN Code")]
        public String? SRH_ADD_Pin { get; set; }

        [Display(Name = "GSTIN")]
        public String? SRH_ADD_GSTIN { get; set; }

        public Int16 SRH_ADD_IsDeleted { get; set; }

        public void Reset()
        {
            this.SRH_ADD_Number = 0;
            this.SRH_ADD_SRH_Number = 0;
            this.SRH_ADD_Number = 0;
            this.SRH_ADD_ADTP_Number = 0;
            this.SRH_ADD_AddressID = "";
            this.SRH_ADD_Address = "";
            this.SRH_ADD_City = "";
            this.SRH_ADD_State = "";
            this.SRH_ADD_Country = "";
            this.SRH_ADD_Pin = "";
            this.SRH_ADD_GSTIN = "";
            this.SRH_ADD_IsDeleted = 0;
        }
    }


    public class SaleRejectionGst
    {
        public Int64 TaxIndex { get; set; }
        public String? TaxCategory { get; set; }
        public String? TaxType { get; set; }
        public String? TaxElement { get; set; }
        public String? TaxElementName { get; set; }
        public String? Chargeable { get; set; }
        public String? LoadonInventory { get; set; }
        public String? LoadonInventoryPercent { get; set; }
        public Int64 Calculation { get; set; }
        public Double? Percentage { get; set; }
        public Double AssessableValue { get; set; }
        public Double Amount { get; set; }
    }
    public class SaleRejectionWHT
    {
        public Int64 TaxNumber { get; set; }
        public String? TaxCode { get; set; }
        public Int64 Tax { get; set; }
        public Double Percentage { get; set; }
    }


    public class SaleRejectionAscii
    {
        public Int64 SRH_Number { get; set; }

        [Display(Name = "Sale Rejection Date")]
        public String? SRH_RejectionDate { get; set; }

        [Display(Name = "Sale Rejection Number")]
        public String? SRH_RejectionNo { get; set; }

        [Display(Name = "Supplier Rejection Number")]
        public String? SRH_SupplierRejectionNo { get; set; }

        [Display(Name = "Supplier Rejection Date")]
        public String? SRH_SupplierRejectionDate { get; set; }

        [Display(Name = "Export Order")]
        public String? SRH_ExportOrder { get; set; }

        [Display(Name = "Buyer Name")]
        public String? SRH_Buyer_Number { get; set; }

        [Display(Name = "Currency")]
        public String? SRH_Currency_Number { get; set; }

        [Display(Name = "Tax Cluster")]
        public String? SRH_TaxCluster_Number { get; set; }

        [Display(Name = "WH Tax Code")]
        public String? SRH_WHT_Number { get; set; }

        [Display(Name = "Material Segregation")]
        public String? SRH_MS_Number { get; set; }

        [Display(Name = "No. of Line Item")]
        public String? SRH_TotalItem { get; set; }

        [Display(Name = "Qty")]
        public String? SRI_Qty { get; set; }

        [Display(Name = "Material Value")]
        public String? SRH_MaterialCost { get; set; }

        [Display(Name = "Item Misc.Income Value")]
        public String? SRH_ItemMiscIncome { get; set; }

        [Display(Name = "Header Misc.Income Value")]
        public String? SRH_HeaderMiscIncome { get; set; }

        [Display(Name = "GST")]
        public String? SRH_GST_Amount { get; set; }

        [Display(Name = "Rejection Amount")]
        public String? SRH_RejectionAmount { get; set; }

        [Display(Name = "Withhold Tax")]
        public String? SRH_WHT_Amount { get; set; }

        [Display(Name = "Round Off")]
        public String? SRH_RoundOff { get; set; }

        [Display(Name = "Buyer Payable")]
        public String? SRH_BuyerReceivable { get; set; }
    }
    public class SaleRejectionDetailAscii
    {
        public Int64 SRH_Number { get; set; }

        [Display(Name = "Sale Rejection Date")]
        public String? SRH_RejectionDate { get; set; }

        [Display(Name = "Sale Rejection Number")]
        public String? SRH_RejectionNo { get; set; }

        [Display(Name = "Supplier Rejection Number")]
        public String? SRH_SupplierRejectionNo { get; set; }

        [Display(Name = "Supplier Rejection Date")]
        public String? SRH_SupplierRejectionDate { get; set; }

        [Display(Name = "Export Order")]
        public String? SRH_ExportOrder { get; set; }

        [Display(Name = "Buyer Name")]
        public String? SRH_Buyer_Number { get; set; }

        [Display(Name = "Currency")]
        public String? SRH_Currency_Number { get; set; }

        [Display(Name = "Material Segregation")]
        public String? SRI_MS_Number { get; set; }

        [Display(Name = "Sale Order Number")]
        public String? SRI_SRH_OrderNo { get; set; }

        [Display(Name = "Item Group")]
        public String? SRI_ItemGroup { get; set; }

        [Display(Name = "Item Code")]
        public String? SRI_ItemCode { get; set; }

        [Display(Name = "Description")]
        public String? SRI_Description { get; set; }

        [Display(Name = "Warehouse")]
        public String? SRI_Warehouse_Number { get; set; }

        [Display(Name = "UoM")]
        public String? SRI_UoM { get; set; }

        [Display(Name = "Qty")]
        public String? SRI_Qty { get; set; }

        [Display(Name = "Unit Price")]
        public String? SRI_UnitPrice { get; set; }

        [Display(Name = "Material Value")]
        public String? SRI_Amount { get; set; }

        [Display(Name = "Income Code")]
        public String? SRH_INC_Income { get; set; }

        [Display(Name = "Item Misc.Income Value")]
        public String? SRH_ItemMiscIncome { get; set; }

        [Display(Name = "Header Misc.Income Value")]
        public String? SRH_HeaderMiscIncome { get; set; }

        [Display(Name = "HSN")]
        public String? SRI_HSN_Number { get; set; }

        [Display(Name = "GST")]
        public String? SRI_GST_Amount { get; set; }

        [Display(Name = "WH_Tax Code")]
        public String? SRI_WHT_Code { get; set; }

        [Display(Name = "Percent")]
        public String? SRI_WHT_Percent { get; set; }

        [Display(Name = "WH_Tax Amount")]
        public String? SRI_WHT_Amount { get; set; }
    }


    public class SaleRejectionAddress
    {
        public List<BuyerAdd_DTO>? BuyerAddressId { get; set; }
        public BuyerAdd_DTO? BuyerAddress { get; set; }
    }
}

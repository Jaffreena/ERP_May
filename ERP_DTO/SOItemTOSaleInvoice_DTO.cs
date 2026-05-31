using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class SOItemToSaleInvoice_DTO
    {
        public Int64 SI_Number { get; set; }
        public Int64 SI_SOH_Number { get; set; }
        public Int64 SI_SOI_Number { get; set; }
        public String? SI_InvoiceNo { get; set; }
        public Int32 SI_InvoiceDate { get; set; }
        public String? SI_ExchangeRate { get; set; }
        public Int64 SI_BUY_Number { get; set; }
        public Int64 SI_BUY_LOC_Number { get; set; }
        public Int64 SI_ExportOrder { get; set; }
        public Int64 SI_CUR_Number { get; set; }
        public Int32 SI_CUR_DecimalPlaces { get; set; }
        public Int64 SI_TCT_Number { get; set; }
        public Int64 SI_WHT_Number { get; set; }
        public Int64 SI_MS_Number { get; set; }
        public Double SI_WHT_Tax { get; set; }
        public Double SI_WHT_Percent { get; set; }

        public Double SI_MaterialCost { get; set; }
        public Double SI_ItemMiscIncome { get; set; }
        public Double SI_HeaderMiscIncome { get; set; }
        public Double SI_GST_Amount { get; set; }
        public Double SI_InvoiceAmount { get; set; }
        public Double SI_WHT_Amount { get; set; }
        public Double SI_RoundOff { get; set; }
        public Double SI_BuyerReceivable { get; set; }


        public Int64 SI_I_Number { get; set; }
        public Int64 SI_WH_Number { get; set; }
        public Int64 SI_ITM_Number { get; set; }
        public String? SI_ITM_Code { get; set; }
        public Int64 SI_UoM_Number { get; set; }
        public Double SI_Qty { get; set; }
        public Double SI_UnitPrice { get; set; }
        public Double SI_Amount { get; set; }
        public Double SI_IncomeValue { get; set; }
        public Int64 SI_HSN_Number { get; set; }


        public Int64 SI_INC_Number { get; set; }
        public Int64 SI_SOI_INC_Number { get; set; }
        public Int64 SI_SOH_INC_Number { get; set; }
        public Int64 SI_INC_MIC_Number { get; set; }
        public String? SI_INC_Remarks { get; set; }
        public Int64 SI_INC_OCRN_Number { get; set; }
        public Int64 SI_INC_CM_Number { get; set; }
        public Double SI_INC_IncomeBase { get; set; }
        public Double SI_INC_IncomeValue { get; set; }
        public Int64 SI_INC_ALCT_Number { get; set; }
        public Int64 SI_INC_LA_Number { get; set; }
        public Int64 SI_INC_CalculateGST { get; set; }
        public Double SI_INC_GST_Amount { get; set; }
        public Int64 SI_INC_SAC_Number { get; set; }
        public Double SI_INC_WHT_Percent { get; set; }
        public Double SI_INC_WHT_Amount { get; set; }

        public Int64 SI_BCH_Number { get; set; }
        public Int64 SI_BCH_BCH_Number { get; set; }
        public String? SI_BCH_Date { get; set; }
        public String? SI_BCH_No { get; set; }
        public Double SI_BCH_Qty { get; set; }
        public Double SI_BCH_UnitPrice { get; set; }
        public Double SI_BCH_Value { get; set; }
        public Int32 SI_BCH_Mode { get; set; }
        public Int32 SI_BCH_Index { get; set; }

        public Int64 SI_ADD_Number { get; set; }
        public Int64 SI_ADD_ADTP_Number { get; set; }
        public String? SI_ADD_AddressID { get; set; }
        public String? SI_ADD_Address { get; set; }
        public String? SI_ADD_City { get; set; }
        public String? SI_ADD_State { get; set; }
        public String? SI_ADD_Country { get; set; }
        public String? SI_ADD_Pin { get; set; }
        public String? SI_ADD_GSTIN { get; set; }

        public String? SI_Search { get; set; }
        public String? SI_DeleteNumbers { get; set; }
        public Int64 SI_CreatorCode { get; set; }
        public Int16 SI_Id { get; set; }

        public void Reset()
        {
            this.SI_Number = 0;
            this.SI_InvoiceNo = "";
            this.SI_InvoiceDate = 0;
            this.SI_ExchangeRate = "";
            this.SI_BUY_Number = 0;
            this.SI_BUY_LOC_Number = 0;
            this.SI_ExportOrder = 0;
            this.SI_CUR_Number = 0;
            this.SI_CUR_DecimalPlaces = 0;
            this.SI_TCT_Number = 0;
            this.SI_WHT_Number = 0;
            this.SI_MS_Number = 0;
            this.SI_WHT_Tax = 0;
            this.SI_WHT_Percent = 0;

            this.SI_MaterialCost = 0;
            this.SI_ItemMiscIncome = 0;
            this.SI_HeaderMiscIncome = 0;
            this.SI_GST_Amount = 0;
            this.SI_InvoiceAmount = 0;
            this.SI_WHT_Amount = 0;
            this.SI_RoundOff = 0;
            this.SI_BuyerReceivable = 0;


            this.SI_WH_Number = 0;
            this.SI_ITM_Number = 0;
            this.SI_UoM_Number = 0;
            this.SI_Qty = 0;
            this.SI_UnitPrice = 0;
            this.SI_Amount = 0;
            this.SI_IncomeValue = 0;
            this.SI_HSN_Number = 0;

            this.SI_INC_MIC_Number = 0;
            this.SI_INC_Remarks = "";
            this.SI_INC_OCRN_Number = 0;
            this.SI_INC_CM_Number = 0;
            this.SI_INC_IncomeBase = 0;
            this.SI_INC_IncomeValue = 0;
            this.SI_INC_ALCT_Number = 0;
            this.SI_INC_LA_Number = 0;
            this.SI_INC_CalculateGST = 0;
            this.SI_INC_GST_Amount = 0;
            this.SI_INC_SAC_Number = 0;

            this.SI_BCH_Number = 0;
            this.SI_BCH_BCH_Number = 0;
            this.SI_BCH_Date = "";
            this.SI_BCH_No = "";
            this.SI_BCH_Qty = 0;
            this.SI_BCH_UnitPrice = 0;
            this.SI_BCH_Value = 0;

            this.SI_Search = "";
            this.SI_DeleteNumbers = "";
            this.SI_CreatorCode = 0;
            this.SI_Id = 0;
        }
    }

    public class SOItemToSaleInvoiceHead_DTO
    {
        public Int64 SIH_Number { get; set; }

        [Display(Name = "Sale Invoice No")]
        [Required(ErrorMessage = "Sale Invoice No is Required")]
        public String? SIH_InvoiceNo { get; set; }

        [Display(Name = "Date")]
        [MaxLength(15, ErrorMessage = "Sale Invoice Date be longer than 15 characters.")]
        [Required(ErrorMessage = "Sale Invoice Date is Required")]
        public String? SIH_InvoiceDate { get; set; }

        [Display(Name = "Exchange Rate")]
        public String? SIH_ExchangeRate { get; set; }

        [Display(Name = "Buyer Name")]
        [Required(ErrorMessage = "Buyer Name is Required")]
        public String? SIH_BUY_Number { get; set; }
        public String? SIH_BUY_Name { get; set; }
        public String? SIH_BUY_LOC_Number { get; set; }

        [Display(Name = "Export Order")]
        [Required(ErrorMessage = "Import Order is Required")]
        public String? SIH_ExportOrder { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public String? SIH_CUR_Number { get; set; }
        public String? SIH_CUR_Name { get; set; }
        public String? SIH_CUR_DecimalPlaces { get; set; }

        [Display(Name = "Tax Cluster")]
        [Required(ErrorMessage = "Tax Cluster is Required")]
        public String? SIH_TCT_Number { get; set; }
        public String? SIH_HIDE_TCT_Number { get; set; }

        [Display(Name = "WH Tax Code")]
        [Required(ErrorMessage = "WH Tax Code is Required")]
        public String? SIH_WHT_Number { get; set; }
        public String? SIH_WHT_Code { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public String? SIH_MS_Number { get; set; }

        public String? SIH_WHT_Tax { get; set; }
        public String? SIH_WHT_Percent { get; set; }


        [Display(Name = "Material Cost")]
        public String? SIH_MaterialCost { get; set; }

        [Display(Name = "Item Misc.Income")]
        public String? SIH_ItemMiscIncome { get; set; }

        [Display(Name = "Header Misc.Income")]
        public String? SIH_HeaderMiscIncome { get; set; }

        [Display(Name = "GST Amount")]
        public String? SIH_GST_Amount { get; set; }

        [Display(Name = "Invoice Amount")]
        public String? SIH_InvoiceAmount { get; set; }

        [Display(Name = "WHT Amount")]
        public String? SIH_WHT_Amount { get; set; }

        [Display(Name = "Round Off")]
        public String? SIH_RoundOff { get; set; }

        [Display(Name = "Buyer Receivable")]
        public String? SIH_BuyerReceivable { get; set; }

        public List<SOItemToSaleInvoiceItem_DTO>? InvoiceItem { get; set; }
        public List<SOItemToSaleInvoiceIncome_DTO>? Income { get; set; }
        public List<SOItemToSaleInvoiceIIncome_DTO>? ItemIncome { get; set; }
        public List<SOItemToSaleInvoiceBatch_DTO>? ItemBatch { get; set; }
        public List<SOItemToSaleInvoiceAddress_DTO>? BuyerAddress { get; set; }

        public Int32 SIH_Mode { get; set; }

        public void Reset()
        {
            this.SIH_Number = 0;
            this.SIH_InvoiceNo = null;
            this.SIH_InvoiceDate = "";
            this.SIH_InvoiceDate = "";
            this.SIH_BUY_Number = null;
            this.SIH_BUY_Name = null;
            this.SIH_BUY_LOC_Number = null;
            this.SIH_ExportOrder = null;
            this.SIH_CUR_Number = null;
            this.SIH_CUR_Name = null;
            this.SIH_CUR_DecimalPlaces = null;
            this.SIH_HIDE_TCT_Number = null;
            this.SIH_TCT_Number = null;
            this.SIH_WHT_Number = "";
            this.SIH_WHT_Tax = "";
            this.SIH_WHT_Percent = "";
            this.SIH_MS_Number = "";

            this.InvoiceItem = null;
            this.Income = null;
            this.ItemIncome = null;
            this.ItemBatch = null;

            this.SIH_MaterialCost = "";
            this.SIH_ItemMiscIncome = "";
            this.SIH_HeaderMiscIncome = "";
            this.SIH_GST_Amount = "";
            this.SIH_InvoiceAmount = "";
            this.SIH_WHT_Amount = "";
            this.SIH_RoundOff = "";
            this.SIH_BuyerReceivable = "";
        }

    }
    public class SOItemToSaleInvoiceItem_DTO
    {
        public Int64 SII_SOI_Number { get; set; }
        public Int64 SII_SOH_Number { get; set; }
        public Int64 SII_Number { get; set; }
        public Int64 SII_MS_Number { get; set; }
        public Int64 SII_Index { get; set; }
        public String? SII_SOH_OrderNo { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public Int64 SII_WH_Number { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public Int64 SII_ITM_Number { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? SII_ITM_Code { get; set; }

        [Display(Name = "Item Group")]
        public String? SII_ITM_Group { get; set; }

        [Display(Name = "Outer Dia")]
        public String? SII_ITM_OuterDia { get; set; }

        [Display(Name = "Thickness")]
        public String? SII_ITM_Thickness { get; set; }

        [Display(Name = "Length")]
        public String? SII_ITM_Length { get; set; }

        [Display(Name = "Description")]
        public String? SII_ITM_Description { get; set; }

        [Display(Name = "Material Grade")]
        public String? SII_ITM_MaterialGrade { get; set; }

        [Display(Name = "UoM")]
        [Required(ErrorMessage = "UoM is Required")]
        public Int64 SII_UoM_Number { get; set; }
        public Int64 SII_HIDE_UoM_Number { get; set; }
        public String? SII_UoM_DecimalPlaces { get; set; }

        [Display(Name = "Invoice Qty")]
        [Required(ErrorMessage = "Qty is Required")]
        public String? SII_Qty { get; set; }

        [Display(Name = "SI Qty")]
        public String? SII_OLD_Qty { get; set; }

        [Display(Name = "Returned Qty")]
        public String? SII_SRI_Qty { get; set; }

        [Display(Name = "SO Pending Qty")]
        public String? SII_SOI_Qty { get; set; }

        [Display(Name = "SO Qty")]
        public String? SII_SOI_Origin_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public String? SII_UnitPrice { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        public String? SII_Amount { get; set; }

        [Display(Name = "Misc. Income Value")]
        [Required(ErrorMessage = "Misc. Income Value is Required")]
        public String? SII_IncomeValue { get; set; }

        [Display(Name = "HSN")]
        [Required(ErrorMessage = "HSN is Required")]
        public Int64 SII_HSN_Number { get; set; }

        [Display(Name = "GST Amount")]
        [Required(ErrorMessage = "GST Amount is Required")]
        public String? SII_GST_Amount { get; set; }

        [Display(Name = "WHT Percent")]
        [Required(ErrorMessage = "WHT Percent is Required")]
        public String? SII_WHT_Percent { get; set; }

        [Display(Name = "WH_Tax Amount")]
        [Required(ErrorMessage = "WH_Tax Amount is Required")]
        public String? SII_WHT_Amount { get; set; }

        public Int16 SII_IsDeleted { get; set; }

        public void Reset()
        {
            this.SII_Number = 0;
            this.SII_ITM_Number = 0;
            this.SII_ITM_Code = "";
            this.SII_UoM_DecimalPlaces = "";
            this.SII_ITM_Group = "";
            this.SII_ITM_Description = "";
            this.SII_ITM_OuterDia = "";
            this.SII_ITM_Thickness = "";
            this.SII_ITM_Length = "";
            this.SII_ITM_MaterialGrade = "";
            this.SII_UoM_Number = 0;
            this.SII_HIDE_UoM_Number = 0;
            this.SII_Qty = "";
            this.SII_UnitPrice = "";
            this.SII_Amount = "";
            this.SII_IncomeValue = "";
            this.SII_HSN_Number = 0;
            this.SII_GST_Amount = "";
            this.SII_WHT_Percent = "";
            this.SII_WHT_Amount = "";
            this.SII_IsDeleted = 0;
        }
    }
    public class SOItemToSaleInvoiceIncome_DTO
    {
        public Int64 SIH_INC_Number { get; set; }
        public Int64 SIH_INC_SOH_INC_Number { get; set; }
        public Int64 SIH_INC_SOH_Number { get; set; }
        public String? SIH_INC_SOH_OrderNo { get; set; }

        [Display(Name = "Income Code")]
        [Required(ErrorMessage = "Income Code is Required")]
        public String? SIH_INC_MIC_Number { get; set; }

        [Display(Name = "Description")]
        public String? SIH_INC_MIC_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? SIH_INC_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? SIH_INC_OCRN_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? SIH_INC_CM_Number { get; set; }

        [Display(Name = "Income Base")]
        [Required(ErrorMessage = "Income Base is Required")]
        public String? SIH_INC_IncomeBase { get; set; }

        [Display(Name = "Income Value")]
        [Required(ErrorMessage = "Income Value is Required")]
        public String? SIH_INC_IncomeValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? SIH_INC_ALCT_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? SIH_INC_LA_Number { get; set; }

        [Display(Name = "Calculate GST")]
        [Required(ErrorMessage = "Calculate GST is Required")]
        public String? SIH_INC_CalculateGST { get; set; }

        [Display(Name = "GST Amount")]
        [Required(ErrorMessage = "GST Amount is Required")]
        public String? SIH_INC_GST_Amount { get; set; }

        [Display(Name = "WHT Percent")]
        [Required(ErrorMessage = "WHT Percent is Required")]
        public String? SIH_INC_WHT_Percent { get; set; }

        [Display(Name = "WHT Amount")]
        [Required(ErrorMessage = "WHT Amount is Required")]
        public String? SIH_INC_WHT_Amount { get; set; }
        public String? SIH_INC_SAC_Number { get; set; }
        public Int16 SIH_INC_IsDeleted { get; set; }
        public void Reset()
        {
            this.SIH_INC_Number = 0;
            this.SIH_INC_MIC_Number = "";
            this.SIH_INC_MIC_Description = "";
            this.SIH_INC_Remarks = "";
            this.SIH_INC_OCRN_Number = "";
            this.SIH_INC_CM_Number = "";
            this.SIH_INC_IncomeBase = "";
            this.SIH_INC_IncomeValue = "";
            this.SIH_INC_ALCT_Number = "";
            this.SIH_INC_CalculateGST = "";
            this.SIH_INC_GST_Amount = "";
            this.SIH_INC_WHT_Percent = "";
            this.SIH_INC_WHT_Amount = "";
            this.SIH_INC_LA_Number = "";
            this.SIH_INC_SAC_Number = "";
            this.SIH_INC_IsDeleted = 0;
        }
    }
    public class SOItemToSaleInvoiceIIncome_DTO
    {
        public Int64 SII_INC_Number { get; set; }
        public Int64 SII_INC_SOI_INC_Number { get; set; }
        public Int64 SII_INC_SOI_Number { get; set; }
        public Int64 SII_INC_SOH_Number { get; set; }
        public Int64 SII_INC_SII_Number { get; set; }
        public String? SII_INC_ITM_Number { get; set; }
        public Int64 SII_INC_ITM_Index { get; set; }

        [Display(Name = "Item Income Code")]
        [Required(ErrorMessage = "Item Income Code is Required")]
        public String? SII_INC_MIC_Number { get; set; }

        [Display(Name = "Description")]
        public String? SII_INC_MIC_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? SII_INC_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? SII_INC_OCRN_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? SII_INC_CM_Number { get; set; }

        [Display(Name = "Income Base")]
        [Required(ErrorMessage = "Income Base is Required")]
        public String? SII_INC_IncomeBase { get; set; }

        [Display(Name = "Income Value")]
        [Required(ErrorMessage = "Income Value is Required")]
        public String? SII_INC_IncomeValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? SII_INC_ALCT_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? SII_INC_LA_Number { get; set; }
        public Int16 SII_INC_IsDeleted { get; set; }

        public void Reset()
        {
            this.SII_INC_Number = 0;
            this.SII_INC_ITM_Number = "";
            this.SII_INC_ITM_Index = 0;
            this.SII_INC_MIC_Number = "";
            this.SII_INC_MIC_Description = "";
            this.SII_INC_Remarks = "";
            this.SII_INC_OCRN_Number = "";
            this.SII_INC_CM_Number = "";
            this.SII_INC_IncomeBase = "";
            this.SII_INC_IncomeValue = "";
            this.SII_INC_ALCT_Number = "";
            this.SII_INC_LA_Number = "";
            this.SII_INC_IsDeleted = 0;
        }
    }
    public class SOItemToSaleInvoiceBatch_DTO
    {
        public Int64 SII_BCH_WH_Number { get; set; }
        public String? SII_BCH_WH_Name { get; set; }
        public Int64 SII_BCH_BCH_Number { get; set; }
        public Int64 SII_BCH_Number { get; set; }
        public Int64 SII_BCH_SII_Number { get; set; }
        public Int64 SII_BCH_ITM_Index { get; set; }
        public String? SII_BCH_ITM_Number { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Batch Date is Required")]
        public String? SII_BCH_Date { get; set; }

        [Display(Name = "Batch Number")]
        [Required(ErrorMessage = "Batch Number is Required")]
        public String? SII_BCH_No { get; set; }

        [Display(Name = "Item Qty")]
        [Required(ErrorMessage = "Item Qty is Required")]
        public String? SII_BCH_ITM_Qty { get; set; }

        [Display(Name = "Hold Qty")]
        public String? SII_BCH_HOLD_Qty { get; set; }

        [Display(Name = " Qty")]
        [Required(ErrorMessage = "Batch Qty is Required")]
        public String? SII_BCH_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Batch Unit Price is Required")]
        public String? SII_BCH_UnitPrice { get; set; }

        [Display(Name = "Batch Value")]
        [Required(ErrorMessage = "Batch Value is Required")]
        public String? SII_BCH_Value { get; set; }

        public String? SII_BCH_IsDeleted { get; set; }

        public void Reset()
        {
            this.SII_BCH_Number = 0;
            this.SII_BCH_ITM_Number = "";
            this.SII_BCH_Date = "";
            this.SII_BCH_No = "";
            this.SII_BCH_Qty = "";
            this.SII_BCH_ITM_Qty = "";
            this.SII_BCH_HOLD_Qty = "";
            this.SII_BCH_UnitPrice = "";
            this.SII_BCH_Value = "";
            this.SII_BCH_IsDeleted = "false";
        }
    }
    public class SOItemToSaleInvoiceAddress_DTO
    {
        public Int64 SIH_ADD_Number { get; set; }
        public Int64 SIH_ADD_SIH_Number { get; set; }
        public Int64 SIH_ADD_BUY_ADD_Number { get; set; }

        [Display(Name = "Address Type")]
        public Int64 SIH_ADD_ADTP_Number { get; set; }

        [Display(Name = "Address ID")]
        public String? SIH_ADD_AddressID { get; set; }

        [Display(Name = "Address")]
        public String? SIH_ADD_Address { get; set; }

        [Display(Name = "City")]
        public String? SIH_ADD_City { get; set; }

        [Display(Name = "State")]
        public String? SIH_ADD_State { get; set; }

        [Display(Name = "Country")]
        public String? SIH_ADD_Country { get; set; }

        [Display(Name = "PIN Code")]
        public String? SIH_ADD_Pin { get; set; }

        [Display(Name = "GSTIN")]
        public String? SIH_ADD_GSTIN { get; set; }

        public Int16 SIH_ADD_IsDeleted { get; set; }

        public void Reset()
        {
            this.SIH_ADD_Number = 0;
            this.SIH_ADD_SIH_Number = 0;
            this.SIH_ADD_Number = 0;
            this.SIH_ADD_ADTP_Number = 0;
            this.SIH_ADD_AddressID = "";
            this.SIH_ADD_Address = "";
            this.SIH_ADD_City = "";
            this.SIH_ADD_State = "";
            this.SIH_ADD_Country = "";
            this.SIH_ADD_Pin = "";
            this.SIH_ADD_GSTIN = "";
            this.SIH_ADD_IsDeleted = 0;
        }
    }


    public class SOItemToSaleInvoiceGst
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
    public class SOItemToSaleInvoiceWHT
    {
        public Int64 TaxNumber { get; set; }
        public String? TaxCode { get; set; }
        public Int64 Tax { get; set; }
        public Double Percentage { get; set; }
    }


    public class SOItemToSIBatchItem_DTO
    {
        public List<SIBatch_DTO>? SIBatch { get; set; }
        public List<SIBatch_DTO>? SIView { get; set; }
    }
    public class SOItemToSaleInvoiceItemBatch
    {
        public List<SIBatchJson_DTO> ItemBatch { get; set; }
    }


    public class SOItemToSIBatchJson_DTO
    {
        public String? SII_BCH_Number { get; set; }
        public String? SII_BCH_BCH_Number { get; set; }
        public String? SII_BCH_ITM_Number { get; set; }
        public String? SII_BCH_ITM_Index { get; set; }
        public String? SII_BCH_WH_Number { get; set; }

        public String? SII_BCH_Date { get; set; }
        public String? SII_BCH_No { get; set; }

        public Double SII_BCH_ITM_Qty { get; set; }
        public Double SII_BCH_Hold_Qty { get; set; }
        public Double SII_BCH_Qty { get; set; }
        public Double SII_BCH_UnitPrice { get; set; }
        public Double SII_BCH_Value { get; set; }
    }
    public class SOItemToSIBatch_DTO
    {
        public Int64 SII_BCH_Number { get; set; }
        public Int64 SII_BCH_BCH_Number { get; set; }
        public Int64 SII_BCH_SII_Number { get; set; }
        public Int64 SII_BCH_Item_Index { get; set; }
        public Int64 SII_BCH_WH_Number { get; set; }
        public String? SII_BCH_WH_Name { get; set; }
        public String? SII_BCH_ITM_Number { get; set; }
        public String? SII_BCH_Date { get; set; }
        public String? SII_BCH_No { get; set; }
        public String? SII_BCH_ITM_Qty { get; set; }
        public String? SII_BCH_HOLD_Qty { get; set; }
        public String? SII_BCH_Qty { get; set; }
        public String? SII_BCH_UnitPrice { get; set; }
        public String? SII_BCH_Value { get; set; }
    }


    public class SOItemToSI_DTO
    {
        public List<SIItemSaleOrderItem_DTO>? SOItems { get; set; }
        public List<SIItemSaleOrderIncome_DTO>? SOIncomes { get; set; }
        public List<SIItemSaleOrderIIncome_DTO>? SOItemIncomes { get; set; }
    }
    public class SIItemSaleOrderItem_DTO
    {
        public Int64 SOI_Number { get; set; }
        public Int64 SOH_Number { get; set; }
        public String? SOH_OrderNo { get; set; }
        public Int64 SOI_MS_Number { get; set; }
        public Int64 SOI_WH_Number { get; set; }
        public String? SOI_ITM_Number { get; set; }
        public String? SOI_ITM_Code { get; set; }
        public String? SOI_ITM_Group { get; set; }
        public String? SOI_ITM_OuterDia { get; set; }
        public String? SOI_ITM_MaterialGrade { get; set; }
        public String? SOI_ITM_Length { get; set; }
        public String? SOI_ITM_Thickness { get; set; }
        public String? SOI_HSN { get; set; }
        public String? SOI_ITM_Description { get; set; }
        public String? SOI_UoM_Number { get; set; }
        public String? soI_Origin_Qty { get; set; }
        public String? SOI_Qty { get; set; }
        public String? SOI_UnitPrice { get; set; }
        public String? SOI_Amount { get; set; }
        public String? SOI_UoM_DecimalPlaces { get; set; }
    }
    public class SIItemSaleOrderIncome_DTO
    {
        public Int64 SOH_INC_SOH_Number { get; set; }
        public Int64 SOH_INC_Number { get; set; }
        public String? SOH_INC_SOH_OrderNo { get; set; }
        public String? SOH_INC_MIC_Number { get; set; }
        public String? SOH_INC_Description { get; set; }
        public String? SOH_INC_Remarks { get; set; }
        public String? SOH_INC_OCRN_Number { get; set; }
        public String? SOH_INC_CM_Number { get; set; }
        public String? SOH_INC_IncomeBase { get; set; }
        public String? SOH_INC_IncomeValue { get; set; }
        public String? SOH_INC_ALCT_Number { get; set; }
        public String? SOH_INC_LA_Number { get; set; }
        public String? SOH_INC_SAC_Number { get; set; }
    }
    public class SIItemSaleOrderIIncome_DTO
    {
        public Int64 SOI_INC_Number { get; set; }
        public Int64 SOI_INC_SOI_Number { get; set; }
        public Int64 SOI_INC_SOH_Number { get; set; }
        public String? SOI_INC_ITM_Number { get; set; }
        public String? SOI_INC_MIC_Number { get; set; }
        public String? SOI_INC_Description { get; set; }
        public String? SOI_INC_Remarks { get; set; }
        public String? SOI_INC_OCRN_Number { get; set; }
        public String? SOI_INC_CM_Number { get; set; }
        public String? SOI_INC_IncomeBase { get; set; }
        public String? SOI_INC_IncomeValue { get; set; }
        public String? SOI_INC_ALCT_Number { get; set; }
        public String? SOI_INC_LA_Number { get; set; }
    }


    public class SOItemToSaleInvoiceAddress
    {
        public List<BuyerAdd_DTO>? BuyerAddressId { get; set; }
        public BuyerAdd_DTO? BuyerAddress { get; set; }
    }
}

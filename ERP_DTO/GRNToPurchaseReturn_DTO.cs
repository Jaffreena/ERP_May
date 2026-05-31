using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class GRNToPurchaseReturn_DTO
    {
        public Int64 PRH_Number { get; set; }
        public String? PRH_ReturnNo { get; set; }
        public String? PRH_ReturnDate { get; set; }
        public String? PRH_Vendor_Number { get; set; }
        public String? PRH_Currency_Number { get; set; }
        public Int32 PRH_ImportOrder { get; set; }
        public Double PRH_ExchangeRate { get; set; }
        public String? PRH_Currency { get; set; }
        public String? PRH_TaxCluster_Number { get; set; }
        public String? PRH_WHT_Number { get; set; }
        public String? PRH_MS_Number { get; set; }
        public String? PRH_DueDate { get; set; }

        public String? PRH_DeliveryTerms { get; set; }
        public String? PRH_DeliveryMode { get; set; }



        public String? PRH_TotalItem { get; set; }
        public String? PRH_MaterialCost { get; set; }
        public String? PRH_ItemMiscExpense { get; set; }
        public String? PRH_EXP_Expense { get; set; }
        public String? PRH_HeaderMiscExpense { get; set; }
        public String? PRH_GST_Amount { get; set; }
        public String? PRH_ReturnAmount { get; set; }
        public String? PRH_WHT_Amount { get; set; }
        public String? PRH_RoundOff { get; set; }
        public String? PRH_VendorReceivable { get; set; }


        public Int64 EXP_Number { get; set; }
        public Int64 EXP_Expense_Number { get; set; }
        public String? EXP_Description { get; set; }
        public String? EXP_Remarks { get; set; }
        public Int64 EXP_Occurrence_Number { get; set; }
        public Int64 EXP_CM_Number { get; set; }
        public Double EXP_ExpenseBase { get; set; }
        public Double EXP_ExpenseValue { get; set; }
        public Int64 EXP_Allocate_Number { get; set; }
        public Int64 EXP_LA_Number { get; set; }
        public Int64 EXP_TaxCalculate { get; set; }
        public Double EXP_TaxValue { get; set; }
        public Int64 EXP_SAC_Number { get; set; }

        public Int64 PRI_PII_Number { get; set; }
        public Int64 PRI_PIH_Number { get; set; }
        public Int64 PRI_Number { get; set; }
        public String? PRI_MS_Number { get; set; }
        public Int64 PRI_Index { get; set; }
        public String? PRI_Item_Number { get; set; }
        public String? PRI_Warehouse_Number { get; set; }
        public String? PRI_DecimalPlaces { get; set; }
        public String? PRI_ItemCode { get; set; }
        public String? PRI_ItemGroup { get; set; }
        public String? PRI_Description { get; set; }
        public String? PRI_UoM_Number { get; set; }
        public String? PRI_UoM { get; set; }
        public String? PRI_Qty { get; set; }
        public String? PRI_UnitPrice { get; set; }
        public String? PRI_Amount { get; set; }
        public Double PRI_ExpenseValue { get; set; }
        public String? PRI_HSN_Number { get; set; }
        public String? PRI_GST_Amount { get; set; }
        public String? PRI_WHT_Percent { get; set; }
        public String? PRI_WHT_Amount { get; set; }

        public Int64 BCH_Number { get; set; }
        public Int64 PRI_BCH_Number { get; set; }
        public String? PRI_BCH_Date { get; set; }
        public String? PRI_BCH_No { get; set; }
        public Double PRI_BCH_Qty { get; set; }
        public Double PRI_BCH_UnitPrice { get; set; }
        public Double PRI_BCH_Value { get; set; }

        public String? PRI_Op1 { get; set; }
        public String? PRI_Op2 { get; set; }

        public String? Search { get; set; }
        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.PRH_Number = 0;
            this.PRH_ReturnNo = null;
            this.PRH_ReturnDate = "";
            this.PRH_Vendor_Number = "";
            this.PRH_Currency_Number = "";
            this.PRH_ImportOrder = 0;
            this.PRH_ExchangeRate = 0;
            this.PRH_Currency = "";
            this.PRH_TaxCluster_Number = "";
            this.PRH_WHT_Number = "";
            this.PRH_MS_Number = "";
            this.PRH_DeliveryTerms = "";
            this.PRH_DeliveryMode = "";

            this.PRH_MaterialCost = "";
            this.PRH_ItemMiscExpense = "";
            this.PRH_HeaderMiscExpense = "";
            this.PRH_GST_Amount = "";
            this.PRH_ReturnAmount = "";
            this.PRH_WHT_Amount = "";
            this.PRH_RoundOff = "";
            this.PRH_VendorReceivable = "";


            this.EXP_Number = 0;
            this.EXP_Expense_Number = 0;
            this.EXP_Description = "";
            this.EXP_Remarks = "";
            this.EXP_Occurrence_Number = 0;
            this.EXP_CM_Number = 0;
            this.EXP_ExpenseBase = 0;
            this.EXP_ExpenseValue = 0;
            this.EXP_Allocate_Number = 0;
            this.EXP_LA_Number = 0;

            this.PRI_Number = 0;
            this.PRI_MS_Number = "";
            this.PRI_Index = 0;
            this.PRI_Item_Number = "";
            this.PRI_Warehouse_Number = "";
            this.PRI_DecimalPlaces = "";
            this.PRI_ItemCode = "";
            this.PRI_ItemGroup = "";
            this.PRI_Description = "";
            this.PRI_UoM_Number = "";
            this.PRI_UoM = "";
            this.PRI_Qty = "";
            this.PRI_UnitPrice = "";
            this.PRI_Amount = "";
            this.PRI_ExpenseValue = 0;
            this.PRI_HSN_Number = "";
            this.PRI_GST_Amount = "";
            this.PRI_WHT_Percent = "";
            this.PRI_WHT_Amount = "";

            this.PRI_BCH_Number = 0;
            this.PRI_BCH_Date = "";
            this.PRI_BCH_No = "";
            this.PRI_BCH_Qty = 0;
            this.PRI_BCH_Value = 0;

            this.Search = "";
            this.DeleteNumbers = "";
        }

    }

    public class GRNToPurchaseReturnHead_DTO
    {
        public Int64 PRH_Number { get; set; }

        [Display(Name = "Purchase Return No.")]
        [Required(ErrorMessage = "Purchase Return No. is Required")]
        public String? PRH_ReturnNo { get; set; }

        [Display(Name = "Date")]
        [MaxLength(15, ErrorMessage = "GRN Date be longer than 15 characters.")]
        [Required(ErrorMessage = "GRN Date is Required")]
        public String? PRH_ReturnDate { get; set; }
        public String? PRHReturnDate { get; set; }

        [Display(Name = "Due Date")]
        [MaxLength(15, ErrorMessage = "Due Date be longer than 15 characters.")]
        [Required(ErrorMessage = "Due Date is Required")]
        public String? PRH_DueDate { get; set; }

        [Display(Name = "Exchange Rate")]
        public String? PRH_ExchangeRate { get; set; }

        [Display(Name = "Vendor Name")]
        [Required(ErrorMessage = "Vendor Name is Required")]
        public String? PRH_Vendor_Number { get; set; }
        public String? PRH_Vendor { get; set; }
        public String? PRH_VendorLocation { get; set; }
        public String? PRH_CreditDays { get; set; }
        public String? PRH_PaymentBase { get; set; }
        public String? PRH_DecimalPlaces { get; set; }
        public String? PRH_TaxCluster { get; set; }

        [Display(Name = "Import Order")]
        [Required(ErrorMessage = "Import Order is Required")]
        public String? PRH_ImportOrder { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public String? PRH_Currency_Number { get; set; }
        public String? PRH_Currency { get; set; }
        public String? PRH_TaxClusterNumber { get; set; }

        [Display(Name = "Tax Cluster")]
        [Required(ErrorMessage = "Tax Cluster is Required")]
        public String? PRH_TaxCluster_Number { get; set; }
        public String? PRH_Cluster { get; set; }

        [Display(Name = "WH Tax Code")]
        [Required(ErrorMessage = "WH Tax Code is Required")]
        public String? PRH_WHT_Number { get; set; }
        public String? PRH_WHT_Code { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public String? PRH_MS_Number { get; set; }

        public String? PRH_WHT_Tax { get; set; }
        public String? PRH_WHT_Percent { get; set; }

        [Display(Name = "Terms of Delivery")]
        public String? PRH_DeliveryTerms { get; set; }

        [Display(Name = "Mode of delivery")]
        public String? PRH_DeliveryMode { get; set; }





        [Display(Name = "Material Cost")]
        public String? PRH_MaterialCost { get; set; }

        [Display(Name = "Item Misc.Expense")]
        public String? PRH_ItemMiscExpense { get; set; }

        [Display(Name = "Header Misc.Expense")]
        public String? PRH_HeaderMiscExpense { get; set; }

        [Display(Name = "GST Amount")]
        public String? PRH_GST_Amount { get; set; }

        [Display(Name = "Rejection Invoice Amount")]
        public String? PRH_ReturnAmount { get; set; }

        [Display(Name = "WHT Amount")]
        public String? PRH_WHT_Amount { get; set; }

        [Display(Name = "Round Off")]
        public String? PRH_RoundOff { get; set; }

        [Display(Name = "Vendor Receivable")]
        public String? PRH_VendorReceivable { get; set; }

        public List<GRNToPurchaseReturnItem_DTO>? ReturnItems { get; set; }
        public List<GRNToPurchaseReturnExpense_DTO>? Expenses { get; set; }
        public List<GRNToPurchaseReturnIExpense_DTO>? ItemExpenses { get; set; }
        public List<GRNToPurchaseReturnBatch_DTO>? ItemBatch { get; set; }
        public void Reset()
        {
            this.PRH_Number = 0;
            this.PRH_ReturnNo = null;
            this.PRH_ReturnDate = "";
            this.PRH_Vendor_Number = "";
            this.PRH_Vendor = "";
            this.PRH_VendorLocation = "";
            this.PRH_DecimalPlaces = "";
            this.PRH_Currency_Number = "";
            this.PRH_Currency = "";
            this.PRH_TaxCluster_Number = "";
            this.PRH_WHT_Number = "";
            this.PRH_WHT_Tax = "";
            this.PRH_WHT_Percent = "";
            this.PRH_MS_Number = "";

            this.ReturnItems = null;
            this.Expenses = null;
            this.ItemExpenses = null;
            this.ItemBatch = null;

            this.PRH_MaterialCost = "";
            this.PRH_ItemMiscExpense = "";
            this.PRH_HeaderMiscExpense = "";
            this.PRH_GST_Amount = "";
            this.PRH_ReturnAmount = "";
            this.PRH_WHT_Amount = "";
            this.PRH_RoundOff = "";
            this.PRH_VendorReceivable = "";
        }

    }

    public class GRNToPurchaseReturnItem_DTO
    {
        public Int64 PRI_Number { get; set; }
        public Int64 PRI_Index { get; set; }
        public Int64 PRI_PII_Number { get; set; }
        public Int64 PRI_PIH_Number { get; set; }
        public Int64 PRI_POH_Number { get; set; }

        [Display(Name = "PO Number")]
        public String? PRI_POH_Order { get; set; }
        public String? PRI_PI_Date { get; set; }
        public Int64 PRI_MS_Number { get; set; }

        [Display(Name = "Supplier Invoice No.")]
        public String? PRI_SupplierInvoiceNo { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? PRI_Item_Number { get; set; }
        public String? PRI_DecimalPlaces { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? PRI_ItemCode { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public String? PRI_Warehouse_Number { get; set; }
        public String? PRI_Warehouse { get; set; }

        [Display(Name = "Item Group")]
        public String? PRI_ItemGroup { get; set; }

        [Display(Name = "Description")]
        public String? PRI_Description { get; set; }

        [Display(Name = "UoM")]
        [Required(ErrorMessage = "UoM is Required")]
        public String? PRI_UoM_Number { get; set; }

        public String? PRI_UoM { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Qty is Required")]
        public String? PRI_Qty { get; set; }
        public String? PRI_PII_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public String? PRI_UnitPrice { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        public String? PRI_Amount { get; set; }

        [Display(Name = "Expense value")]
        [Required(ErrorMessage = "Expense value is Required")]
        public String? PRI_ExpenseValue { get; set; }

        [Display(Name = "HSN")]
        [Required(ErrorMessage = "HSN is Required")]
        public String? PRI_HSN_Number { get; set; }

        public String? PRI_HSN { get; set; }

        [Display(Name = "GST Amount")]
        [Required(ErrorMessage = "GST Amount is Required")]
        public String? PRI_GST_Amount { get; set; }

        [Display(Name = "Percent")]
        [Required(ErrorMessage = "Percent is Required")]
        public String? PRI_WHT_Percent { get; set; }

        [Display(Name = "WH_Tax Amount")]
        [Required(ErrorMessage = "WH_Tax Amount is Required")]
        public String? PRI_WHT_Amount { get; set; }

        public String? PRI_IsDeleted { get; set; }

        public void Reset()
        {
            this.PRI_Number = 0;
            this.PRI_Item_Number = "";
            this.PRI_ItemCode = "";
            this.PRI_DecimalPlaces = "";
            this.PRI_ItemGroup = "";
            this.PRI_Description = "";
            this.PRI_UoM_Number = "";
            this.PRI_Qty = "";
            this.PRI_UnitPrice = "";
            this.PRI_Amount = "";
            this.PRI_ExpenseValue = "";
            this.PRI_HSN_Number = "";
            this.PRI_GST_Amount = "";
            this.PRI_WHT_Percent = "";
            this.PRI_WHT_Amount = "";
            this.PRI_IsDeleted = "false";
        }
    }
    public class GRNToPurchaseReturnExpense_DTO
    {
        public Int64 PRH_EXP_Number { get; set; }
        public Int64 PRH_EXP_PIH_Number { get; set; }
        public String? PRH_EXP_PIH_OrderNo { get; set; }
        public String? PRH_EXP_POH_OrderNo { get; set; }
        public String? PRH_EXP_POH_Number { get; set; }

        [Display(Name = "Expense Code")]
        [Required(ErrorMessage = "Expense Code is Required")]
        public String? PRH_EXP_Expense_Number { get; set; }

        [Display(Name = "Description")]
        public String? PRH_EXP_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? PRH_EXP_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? PRH_EXP_Occurrence_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? PRH_EXP_CM_Number { get; set; }

        [Display(Name = "Expense Base")]
        [Required(ErrorMessage = "Expense Base is Required")]
        public String? PRH_EXP_ExpenseBase { get; set; }

        [Display(Name = "Expense value")]
        [Required(ErrorMessage = "Expense value is Required")]
        public String? PRH_EXP_ExpenseValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? PRH_EXP_Allocate_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? PRH_EXP_LA_Number { get; set; }


        [Display(Name = "Tax calculating")]
        [Required(ErrorMessage = "Tax calculating is Required")]
        public String? PRH_EXP_TaxCalculate { get; set; }

        [Display(Name = "Tax Value")]
        [Required(ErrorMessage = "Tax Value is Required")]
        public String? PRH_EXP_TaxValue { get; set; }
        public String? PRH_EXP_SAC_Number { get; set; }
        public String? PRH_EXP_IsDeleted { get; set; }

        public void Reset()
        {
            this.PRH_EXP_Number = 0;
            this.PRH_EXP_Expense_Number = "";
            this.PRH_EXP_PIH_Number = 0;
            this.PRH_EXP_POH_OrderNo = "";
            this.PRH_EXP_POH_OrderNo = "";
            this.PRH_EXP_POH_Number = "";
            this.PRH_EXP_Description = "";
            this.PRH_EXP_Remarks = "";
            this.PRH_EXP_Occurrence_Number = "";
            this.PRH_EXP_CM_Number = "";
            this.PRH_EXP_ExpenseBase = "";
            this.PRH_EXP_Allocate_Number = "";
            this.PRH_EXP_TaxCalculate = "";
            this.PRH_EXP_TaxValue = "";
            this.PRH_EXP_LA_Number = "";
            this.PRH_EXP_SAC_Number = "";
            this.PRH_EXP_IsDeleted = "false";
        }
    }
    public class GRNToPurchaseReturnIExpense_DTO
    {
        public Int64 PRI_EXP_Number { get; set; }
        public Int64 PRI_EXP_PII_Number { get; set; }
        public Int64 PRI_EXP_PIH_Number { get; set; }

        [Display(Name = "Expense Code")]
        [Required(ErrorMessage = "Expense Code is Required")]
        public String? PRI_EXP_Expense_Number { get; set; }

        [Display(Name = "Description")]
        public String? PRI_EXP_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? PRI_EXP_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? PRI_EXP_Occurrence_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? PRI_EXP_CM_Number { get; set; }

        [Display(Name = "Expense Base")]
        [Required(ErrorMessage = "Expense Base is Required")]
        public String? PRI_EXP_ExpenseBase { get; set; }

        [Display(Name = "Expense value")]
        [Required(ErrorMessage = "Expense value is Required")]
        public String? PRI_EXP_ExpenseValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? PRI_EXP_Allocate_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? PRI_EXP_LA_Number { get; set; }
        public String? PRI_EXP_IsDeleted { get; set; }

        public void Reset()
        {
            this.PRI_EXP_Number = 0;
            this.PRI_EXP_PII_Number = 0;
            this.PRI_EXP_PIH_Number = 0;
            this.PRI_EXP_Expense_Number = "";
            this.PRI_EXP_Description = "";
            this.PRI_EXP_Remarks = "";
            this.PRI_EXP_Occurrence_Number = "";
            this.PRI_EXP_CM_Number = "";
            this.PRI_EXP_ExpenseBase = "";
            this.PRI_EXP_Allocate_Number = "";
            this.PRI_EXP_LA_Number = "";
            this.PRI_EXP_IsDeleted = "false";
        }
    }
    public class GRNToPurchaseReturnBatch_DTO
    {
        public Int64 PRI_BCH_Warehouse_Number { get; set; }
        public String? PRI_BCH_Warehouse { get; set; }
        public Int64 PRI_BCH_BCH_Number { get; set; }
        public Int64 PRI_BCH_Number { get; set; }
        public Int64 PRI_BCH_PRI_Number { get; set; }
        public Int64 PRI_BCH_Item_Index { get; set; }
        public String? PRI_BCH_Item_Number { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Batch Date is Required")]
        public String? PRI_BCH_Date { get; set; }

        [Display(Name = "Batch Number")]
        [Required(ErrorMessage = "Batch Number is Required")]
        public String? PRI_BCH_No { get; set; }

        [Display(Name = "Item Qty")]
        [Required(ErrorMessage = "Item Qty is Required")]
        public String? PRI_BCH_ItemQty { get; set; }

        [Display(Name = "Hold Qty")]
        [Required(ErrorMessage = "Hold Qty is Required")]
        public String? PRI_BCH_HoldQty { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Batch Qty is Required")]
        public String? PRI_BCH_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Batch Unit Price is Required")]
        public String? PRI_BCH_UnitPrice { get; set; }

        [Display(Name = "Batch Value")]
        [Required(ErrorMessage = "Batch Value is Required")]
        public String? PRI_BCH_Value { get; set; }

        public String? PRI_BCH_IsDeleted { get; set; }

        public void Reset()
        {
            this.PRI_BCH_Number = 0;
            this.PRI_BCH_Item_Number = "";
            this.PRI_BCH_Date = "";
            this.PRI_BCH_No = "";
            this.PRI_BCH_Qty = "";
            this.PRI_BCH_ItemQty = "";
            this.PRI_BCH_HoldQty = "";
            this.PRI_BCH_UnitPrice = "";
            this.PRI_BCH_Value = "";
            this.PRI_BCH_IsDeleted = "false";
        }
    }


    public class GRNToPurchaseReturnGst
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
    public class GRNToPurchaseReturnWHT
    {
        public Int64 TaxNumber { get; set; }
        public String? TaxCode { get; set; }
        public Int64 Tax { get; set; }
        public Double Percentage { get; set; }
    }


    public class GRNToPurchaseReturnAscii
    {
        public Int64 PRH_Number { get; set; }

        [Display(Name = "Date")]
        public String? PRH_ReturnDate { get; set; }

        [Display(Name = "Purchase Return No")]
        public String? PRH_ReturnNo { get; set; }

        [Display(Name = "Import Order")]
        public String? PRH_ImportOrder { get; set; }

        [Display(Name = "Vendor Name")]
        public String? PRH_Vendor_Number { get; set; }

        [Display(Name = "Currency")]
        public String? PRH_Currency_Number { get; set; }

        [Display(Name = "Tax Cluster")]
        public String? PRH_TaxCluster_Number { get; set; }

        [Display(Name = "WH_Tax Code")]
        public String? PRH_WHT_Number { get; set; }

        [Display(Name = "Material Segregation")]
        public String? PRH_MS_Number { get; set; }

        [Display(Name = "No. of Line Item")]
        public String? PRH_TotalItem { get; set; }
        
        [Display(Name = "Qty")]
        public String? PRI_Qty { get; set; }

        [Display(Name = "Material Value")]
        public String? PRH_MaterialCost { get; set; }

        [Display(Name = "Item Misc.Expense Value")]
        public String? PRH_ItemMiscExpense { get; set; }

        [Display(Name = "Header Misc.Expense Value")]
        public String? PRH_HeaderMiscExpense { get; set; }

        [Display(Name = "GST Amount")]
        public String? PRH_GST_Amount { get; set; }

        [Display(Name = "Rejection Invoice Amount")]
        public String? PRH_ReturnAmount { get; set; }

        [Display(Name = "WH_Tax Amount")]
        public String? PRH_WHT_Amount { get; set; }

        [Display(Name = "Round Off")]
        public String? PRH_RoundOff { get; set; }

        [Display(Name = "Vendor Receivable")]
        public String? PRH_VendorReceivable { get; set; }
    }
    public class GRNToPurchaseReturnDetailAscii
    {
        public Int64 PRH_Number { get; set; }

        [Display(Name = "Date")]
        public String? PRH_ReturnDate { get; set; }

        [Display(Name = "Purchase Return No")]
        public String? PRH_ReturnNo { get; set; }

        [Display(Name = "Import Order")]
        public String? PRH_ImportOrder { get; set; }

        [Display(Name = "Vendor Name")]
        public String? PRH_Vendor_Number { get; set; }

        [Display(Name = "Currency")]
        public String? PRH_Currency_Number { get; set; }

        [Display(Name = "Supplier Invoice No.")]
        public String? PRH_SupplierInvoiceNo { get; set; }

        [Display(Name = "Purchase Order Number")]
        public String? PRI_POH_OrderNo { get; set; }

        [Display(Name = "Material Segregation")]
        public String? PRI_MS_Number { get; set; }

        [Display(Name = "Item Group")]
        public String? PRI_ItemGroup { get; set; }

        [Display(Name = "Item Code")]
        public String? PRI_ItemCode { get; set; }

        [Display(Name = "Description")]
        public String? PRI_Description { get; set; }

        [Display(Name = "Warehouse")]
        public String? PRI_Warehouse_Number { get; set; }

        [Display(Name = "UoM")]
        public String? PRI_UoM { get; set; }

        [Display(Name = "Qty")]
        public String? PRI_Qty { get; set; }

        [Display(Name = "Unit Price")]
        public String? PRI_UnitPrice { get; set; }

        [Display(Name = "Material Value")]
        public String? PRI_Amount { get; set; }

        [Display(Name = "Expense Code")]
        public String? PRH_EXP_Expense { get; set; }

        [Display(Name = "Item Misc.Expense Value")]
        public String? PRH_ItemMiscExpense { get; set; }

        [Display(Name = "Header Misc.Expense Value")]
        public String? PRH_HeaderMiscExpense { get; set; }

        [Display(Name = "HSN")]
        public String? PRI_HSN_Number { get; set; }

        [Display(Name = "GST Amount")]
        public String? PRI_GST_Amount { get; set; }

        [Display(Name = "WH_Tax Code")]
        public String? PRI_WHT_Code { get; set; }

        [Display(Name = "WH_Tax Percent")]
        public String? PRI_WHT_Percent { get; set; }

        [Display(Name = "WH_Tax Amount")]
        public String? PRI_WHT_Amount { get; set; }
    }




    public class PIToPR_DTO
    {
        public List<PurchaseInvoicePRItem_DTO>? PRItems { get; set; }
        public List<PurchaseInvoicePRExpense_DTO>? PRExpenses { get; set; }
        public List<PurchaseInvoicePRIExpense_DTO>? PRItemExpenses { get; set; }
    }
    public class PurchaseInvoicePRItem_DTO
    {
        public Int64 PII_Number { get; set; }
        public Int64 PIH_Number { get; set; }
        public String? PIH_InvoiceNo { get; set; }
        public String? PIH_InvoiceDate { get; set; }
        public String? PIH_POH_OrderNo { get; set; }
        public String? PIH_POH_Number { get; set; }
        public String? PIH_SupplierInvoiceNo { get; set; }
        public String? PIH_SupplierInvoiceDate { get; set; }
        public String? PII_MS_Number { get; set; }
        public String? PII_Item_Number { get; set; }
        public String? PII_ItemGroup { get; set; }
        public String? PII_ItemCode { get; set; }
        public String? PII_Description { get; set; }
        public String? PII_Warehouse_Number { get; set; }
        public String? PII_Warehouse { get; set; }
        public String? PII_UoM_Number { get; set; }
        public String? PII_DecimalPlaces { get; set; }
        public String? PII_UoM { get; set; }
        public String? PII_Qty { get; set; }
        public String? PII_UnitPrice { get; set; }
        public String? PII_Amount { get; set; }
        public String? PII_HSN_Number { get; set; }
        public String? PII_SAC_Number { get; set; }
        public String? PII_GST_Amount { get; set; }
        public String? PIH_WHT_Number { get; set; }
        public String? PII_WHT_Percent { get; set; }
        public String? PII_WHT_Amount { get; set; }
    }
    public class PurchaseInvoicePRExpense_DTO
    {
        public Int64 PIH_EXP_Number { get; set; }
        public Int64 PIH_EXP_PIH_Number { get; set; }
        public String? PIH_EXP_PIH_InvoiceNo { get; set; }
        public String? PIH_EXP_POH_OrderNo { get; set; }
        public String? PIH_EXP_POH_Number { get; set; }
        public String? PIH_EXP_Expense_Number { get; set; }
        public String? PIH_EXP_Description { get; set; }
        public String? PIH_EXP_Remarks { get; set; }
        public String? PIH_EXP_Occurrence_Number { get; set; }
        public String? PIH_EXP_CM_Number { get; set; }
        public String? PIH_EXP_ExpenseBase { get; set; }
        public String? PIH_EXP_ExpenseValue { get; set; }
        public String? PIH_EXP_Allocate_Number { get; set; }
        public String? PIH_EXP_TaxCalculate { get; set; }
        public String? PIH_EXP_TaxValue { get; set; }
        public String? PIH_EXP_LA_Number { get; set; }
        public String? PIH_EXP_SAC_Number { get; set; }
    }
    public class PurchaseInvoicePRIExpense_DTO
    {
        public Int64 PII_EXP_Number { get; set; }
        public Int64 PII_EXP_PII_Number { get; set; }
        public Int64 PII_EXP_PIH_Number { get; set; }
        public String? PII_EXP_Expense_Number { get; set; }
        public String? PII_EXP_Description { get; set; }
        public String? PII_EXP_Remarks { get; set; }
        public String? PII_EXP_Occurrence_Number { get; set; }
        public String? PII_EXP_CM_Number { get; set; }
        public String? PII_EXP_ExpenseBase { get; set; }
        public String? PII_EXP_ExpenseValue { get; set; }
        public String? PII_EXP_Allocate_Number { get; set; }
        public String? PII_EXP_LA_Number { get; set; }
    }
}

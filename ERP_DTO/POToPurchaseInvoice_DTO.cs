using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class POToPurchaseInvoice_DTO
    {
        public Int64 PIH_Number { get; set; }
        public String? PIH_InvoiceNo { get; set; }
        public String? PIH_DueDate { get; set; }
        public String? PIH_InvoiceDate { get; set; }
        public String? PIH_SupplierInvoiceNo { get; set; }
        public String? PIH_SupplierInvoiceDate { get; set; }
        public String? PIH_Vendor_Number { get; set; }
        public String? PIH_Currency_Number { get; set; }
        public Int32 PIH_ImportOrder { get; set; }
        public Double PIH_ExchangeRate { get; set; }
        public String? PIH_Currency { get; set; }
        public String? PIH_TaxCluster_Number { get; set; }
        public String? PIH_WHT_Number { get; set; }
        public String? PIH_MS_Number { get; set; }


        public String? PIH_TotalItem { get; set; }
        public String? PIH_MaterialCost { get; set; }
        public String? PIH_ItemMiscExpense { get; set; }
        public String? PIH_EXP_Expense { get; set; }
        public String? PIH_HeaderMiscExpense { get; set; }
        public String? PIH_GST_Amount { get; set; }
        public String? PIH_InvoiceAmount { get; set; }
        public String? PIH_WHT_Amount { get; set; }
        public String? PIH_RoundOff { get; set; }
        public String? PIH_VendorPayable { get; set; }


        public Int64 EXP_Number { get; set; }
        public Int64 EXP_Expense_Number { get; set; }
        public String? EXP_Description { get; set; }
        public String? EXP_Remarks { get; set; }
        public Int64 EXP_Occurrence_Number { get; set; }
        public Int64 EXP_CM_Number { get; set; }
        public Double EXP_ExpenseBase { get; set; }
        public Double EXP_ExpenseValue { get; set; }
        public Int64 EXP_Allocate_Number { get; set; }
        public Int64 EXP_GST_Calculate { get; set; }
        public Int64 EXP_LA_Number { get; set; }
        public Int64 EXP_SAC_Number { get; set; }
        public Int64 EXP_TaxCalculate { get; set; }
        public Double EXP_TaxValue { get; set; }

        public String? PII_POH_OrderNo { get; set; }
        public Int64 PII_POH_Number { get; set; }
        public Int64 PII_POI_Number { get; set; }
        public Int64 PII_Number { get; set; }
        public String? PII_MS_Number { get; set; }
        public Int64 PII_Index { get; set; }
        public String? PII_Item_Number { get; set; }
        public String? PII_Warehouse_Number { get; set; }
        public String? PII_Warehouse { get; set; }
        public String? PII_DecimalPlaces { get; set; }
        public String? PII_ItemCode { get; set; }
        public String? PII_ItemGroup { get; set; }
        public String? PII_Description { get; set; }
        public String? PII_UoM_Number { get; set; }
        public String? PII_UoM { get; set; }
        public String? PII_Qty { get; set; }
        public String? PII_UnitPrice { get; set; }
        public String? PII_Amount { get; set; }
        public Double PII_ExpenseValue { get; set; }
        public String? PII_HSN_Number { get; set; }
        public String? PII_GST_Amount { get; set; }
        public String? PII_WHT_Percent { get; set; }
        public String? PII_WHT_Amount { get; set; }

        public Int64 PII_BCH_Number { get; set; }
        public String? PII_BCH_Date { get; set; }
        public String? PII_BCH_No { get; set; }
        public Double PII_BCH_Qty { get; set; }
        public Double PII_BCH_UnitPrice { get; set; }
        public Double PII_BCH_Value { get; set; }

        public String? Search { get; set; }
        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.PIH_Number = 0;
            this.PIH_InvoiceNo = "";
            this.PIH_DueDate = "";
            this.PIH_InvoiceDate = "";
            this.PIH_SupplierInvoiceNo = "";
            this.PIH_SupplierInvoiceDate = "";
            this.PIH_Vendor_Number = "";
            this.PIH_Currency_Number = "";
            this.PIH_ImportOrder = 0;
            this.PIH_ExchangeRate = 0;
            this.PIH_Currency = "";
            this.PIH_TaxCluster_Number = "";
            this.PIH_WHT_Number = "";
            this.PIH_MS_Number = "";

            this.PIH_TotalItem = "";
            this.PIH_MaterialCost = "";
            this.PIH_ItemMiscExpense = "";
            this.PIH_HeaderMiscExpense = "";
            this.PIH_GST_Amount = "";
            this.PIH_InvoiceAmount = "";
            this.PIH_WHT_Amount = "";
            this.PIH_RoundOff = "";
            this.PIH_VendorPayable = "";


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
            this.EXP_SAC_Number = 0;
            this.EXP_TaxCalculate = 0;
            this.EXP_TaxValue = 0;

            this.PII_Number = 0;
            this.PII_POH_Number = 0;
            this.PII_POI_Number = 0;
            this.PII_MS_Number = "";
            this.PII_Index = 0;
            this.PII_Item_Number = "";
            this.PII_Warehouse_Number = "";
            this.PII_DecimalPlaces = "";
            this.PII_ItemCode = "";
            this.PII_ItemGroup = "";
            this.PII_Description = "";
            this.PII_UoM_Number = "";
            this.PII_UoM = "";
            this.PII_Qty = "";
            this.PII_UnitPrice = "";
            this.PII_Amount = "";
            this.PII_ExpenseValue = 0;
            this.PII_HSN_Number = "";
            this.PII_GST_Amount = "";
            this.PII_WHT_Percent = "";
            this.PII_WHT_Amount = "";

            this.PII_BCH_Number = 0;
            this.PII_BCH_Date = "";
            this.PII_BCH_No = "";
            this.PII_BCH_Qty = 0;
            this.PII_BCH_UnitPrice = 0;
            this.PII_BCH_Value = 0;

            this.Search = "";
            this.DeleteNumbers = "";
        }

    }

    public class POToPurchaseInvoiceHead_DTO
    {
        public Int64 PIH_Number { get; set; }

        [Display(Name = "GRN No")]
        [Required(ErrorMessage = "GRN No is Required")]
        public String? PIH_InvoiceNo { get; set; }

        [Display(Name = "Date")]
        [MaxLength(15, ErrorMessage = "GRN Date be longer than 15 characters.")]
        [Required(ErrorMessage = "GRN Date is Required")]
        public String? PIH_InvoiceDate { get; set; }

        [Display(Name = "Due Date")]
        [MaxLength(15, ErrorMessage = "Due Date be longer than 15 characters.")]
        [Required(ErrorMessage = "Due Date is Required")]
        public String? PIH_DueDate { get; set; }

        [Display(Name = "Supplier Invoice No.")]
        [Required(ErrorMessage = "Supplier Invoice No. is Required")]
        public String? PIH_SupplierInvoiceNo { get; set; }

        [Display(Name = "Exchange Rate")]
        public String? PIH_ExchangeRate { get; set; }

        [Display(Name = "Date")]
        [MaxLength(15, ErrorMessage = "Supplier Invoice Date be longer than 15 characters.")]
        [Required(ErrorMessage = "Supplier Invoice Date is Required")]
        public String? PIH_SupplierInvoiceDate { get; set; }

        [Display(Name = "Vendor Name")]
        [Required(ErrorMessage = "Vendor Name is Required")]
        public String? PIH_Vendor_Number { get; set; }
        public String? PIH_Vendor { get; set; }
        public String? PIH_VendorLocation { get; set; }
        public String? PIH_CreditDays { get; set; }
        public String? PIH_PaymentBase { get; set; }
        public String? PIH_DecimalPlaces { get; set; }
        public String? PIH_TaxCluster { get; set; }

        [Display(Name = "Import Order")]
        [Required(ErrorMessage = "Import Order is Required")]
        public String? PIH_ImportOrder { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public String? PIH_Currency_Number { get; set; }
        public String? PIH_Currency { get; set; }

        [Display(Name = "Tax Cluster")]
        [Required(ErrorMessage = "Tax Cluster is Required")]
        public String? PIH_TaxCluster_Number { get; set; }
        public String? PIH_Cluster { get; set; }
        public String? PIH_TaxClusterNumber { get; set; }

        [Display(Name = "WH Tax Code")]
        [Required(ErrorMessage = "WH Tax Code is Required")]
        public String? PIH_WHT_Number { get; set; }
        public String? PIH_WHT_Code { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public String? PIH_MS_Number { get; set; }

        public String? PIH_WHT_Tax { get; set; }
        public String? PIH_WHT_Percent { get; set; }


        [Display(Name = "Material Cost")]
        public String? PIH_MaterialCost { get; set; }

        [Display(Name = "Item Misc.Expense")]
        public String? PIH_ItemMiscExpense { get; set; }

        [Display(Name = "Header Misc.Expense")]
        public String? PIH_HeaderMiscExpense { get; set; }

        [Display(Name = "GST Amount")]
        public String? PIH_GST_Amount { get; set; }

        [Display(Name = "Invoice Amount")]
        public String? PIH_InvoiceAmount { get; set; }

        [Display(Name = "WHT Amount")]
        public String? PIH_WHT_Amount { get; set; }

        [Display(Name = "Round Off")]
        public String? PIH_RoundOff { get; set; }

        [Display(Name = "Payable")]
        public String? PIH_VendorPayable { get; set; }

        public Int32 PIH_Mode { get; set; }

        public List<POToPurchaseInvoiceItem_DTO>? InvoiceItems { get; set; }
        public List<POToPurchaseInvoiceExpense_DTO>? Expenses { get; set; }
        public List<POToPurchaseInvoiceIExpense_DTO>? ItemExpenses { get; set; }
        public List<POToPurchaseInvoiceBatch_DTO>? ItemBatch { get; set; }
        public void Reset()
        {
            this.PIH_Number = 0;
            this.PIH_InvoiceNo = null;
            this.PIH_InvoiceDate = "";
            this.PIH_SupplierInvoiceNo = "";
            this.PIH_SupplierInvoiceDate = "";
            this.PIH_InvoiceDate = "";
            this.PIH_DueDate = "";
            this.PIH_Vendor_Number = "";
            this.PIH_Vendor = "";
            this.PIH_VendorLocation = "";
            this.PIH_DecimalPlaces = "";
            this.PIH_Currency_Number = "";
            this.PIH_Currency = "";
            this.PIH_TaxCluster_Number = "";
            this.PIH_WHT_Number = "";
            this.PIH_WHT_Tax = "";
            this.PIH_WHT_Percent = "";
            this.PIH_MS_Number = "";

            this.InvoiceItems = null;
            this.Expenses = null;
            this.ItemExpenses = null;
            this.ItemBatch = null;

            this.PIH_MaterialCost = "";
            this.PIH_ItemMiscExpense = "";
            this.PIH_HeaderMiscExpense = "";
            this.PIH_GST_Amount = "";
            this.PIH_InvoiceAmount = "";
            this.PIH_WHT_Amount = "";
            this.PIH_RoundOff = "";
            this.PIH_VendorPayable = "";
        }

    }
    public class POToPurchaseInvoiceItem_DTO
    {
        public Int64 PII_Number { get; set; }
        public Int64 PII_MS_Number { get; set; }

        public Int64 PII_POH_Number { get; set; }
        public Int64 PII_POI_Number { get; set; }

        [Display(Name = "Purchase Order")]
        public String? PII_POH_OrderNo { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? PII_Item_Number { get; set; }
        public String? PII_DecimalPlaces { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? PII_ItemCode { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public String? PII_Warehouse_Number { get; set; }

        [Display(Name = "Item Group")]
        public String? PII_ItemGroup { get; set; }

        [Display(Name = "Description")]
        public String? PII_Description { get; set; }

        [Display(Name = "UoM")]
        [Required(ErrorMessage = "UoM is Required")]
        public String? PII_UoM_Number { get; set; }

        public String? PII_UoM { get; set; }

        [Display(Name = "PO Qty")]
        public String? PII_POQty { get; set; }

        [Display(Name = "Invoice Qty")]
        [Required(ErrorMessage = "Invoice Qty is Required")]
        public String? PII_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public String? PII_UnitPrice { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        public String? PII_Amount { get; set; }

        [Display(Name = "Expense value")]
        [Required(ErrorMessage = "Expense value is Required")]
        public String? PII_ExpenseValue { get; set; }

        [Display(Name = "HSN")]
        [Required(ErrorMessage = "HSN is Required")]
        public String? PII_HSN_Number { get; set; }

        public String? PII_HSN { get; set; }

        [Display(Name = "GST Amount")]
        [Required(ErrorMessage = "GST Amount is Required")]
        public String? PII_GST_Amount { get; set; }

        [Display(Name = "Percent")]
        [Required(ErrorMessage = "Percent is Required")]
        public String? PII_WHT_Percent { get; set; }

        [Display(Name = "WH_Tax Amount")]
        [Required(ErrorMessage = "WH_Tax Amount is Required")]
        public String? PII_WHT_Amount { get; set; }

        public String? PII_IsDeleted { get; set; }

        public void Reset()
        {
            this.PII_Number = 0;
            this.PII_Item_Number = "";
            this.PII_ItemCode = "";
            this.PII_DecimalPlaces = "";
            this.PII_ItemGroup = "";
            this.PII_Description = "";
            this.PII_UoM_Number = "";
            this.PII_Qty = "";
            this.PII_UnitPrice = "";
            this.PII_Amount = "";
            this.PII_ExpenseValue = "";
            this.PII_HSN_Number = "";
            this.PII_GST_Amount = "";
            this.PII_WHT_Percent = "";
            this.PII_WHT_Amount = "";
            this.PII_IsDeleted = "";
        }
    }

    public class POToPurchaseInvoiceExpense_DTO
    {
        public Int64 PIH_EXP_Number { get; set; }
        public Int64 PIH_EXP_POH_EXP_Number { get; set; }
        public String? PIH_EXP_POH_OrderNo { get; set; }
        public Int64 PIH_EXP_POH_Number { get; set; }

        [Display(Name = "Expense Code")]
        [Required(ErrorMessage = "Expense Code is Required")]
        public String? PIH_EXP_Expense_Number { get; set; }

        [Display(Name = "Description")]
        public String? PIH_EXP_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? PIH_EXP_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? PIH_EXP_Occurrence_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? PIH_EXP_CM_Number { get; set; }

        [Display(Name = "Expense Base")]
        [Required(ErrorMessage = "Expense Base is Required")]
        public String? PIH_EXP_ExpenseBase { get; set; }

        [Display(Name = "Expense Value")]
        [Required(ErrorMessage = "Expense Value is Required")]
        public String? PIH_EXP_ExpenseValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? PIH_EXP_Allocate_Number { get; set; }

        [Display(Name = "Tax calculating")]
        [Required(ErrorMessage = "Tax calculating is Required")]
        public String? PIH_EXP_TaxCalculate { get; set; }

        [Display(Name = "Tax Value")]
        [Required(ErrorMessage = "Tax Value is Required")]
        public String? PIH_EXP_TaxValue { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? PIH_EXP_LA_Number { get; set; }
        public String? PIH_EXP_SAC_Number { get; set; }
        public String? PIH_EXP_IsDeleted { get; set; }

        public void Reset()
        {
            this.PIH_EXP_Number = 0;
            this.PIH_EXP_Expense_Number = "";
            this.PIH_EXP_Description = "";
            this.PIH_EXP_Remarks = "";
            this.PIH_EXP_Occurrence_Number = "";
            this.PIH_EXP_CM_Number = "";
            this.PIH_EXP_ExpenseBase = "";
            this.PIH_EXP_ExpenseValue = "";
            this.PIH_EXP_Allocate_Number = "";
            this.PIH_EXP_TaxCalculate = "";
            this.PIH_EXP_TaxValue = "";
            this.PIH_EXP_LA_Number = "";
            this.PIH_EXP_SAC_Number = "";
            this.PIH_EXP_IsDeleted = "false";
        }
    }
    public class POToPurchaseInvoiceIExpense_DTO
    {
        public Int64 PII_EXP_Number { get; set; }
        public Int64 PII_EXP_POI_EXP_Number { get; set; }
        public Int64 PII_EXP_PII_Number { get; set; }
        public Int64 PII_EXP_POH_Number { get; set; }
        public Int64 PII_EXP_POI_Number { get; set; }

        [Display(Name = "Expense Code")]
        [Required(ErrorMessage = "Expense Code is Required")]
        public String? PII_EXP_Expense_Number { get; set; }

        [Display(Name = "Description")]
        public String? PII_EXP_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? PII_EXP_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? PII_EXP_Occurrence_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? PII_EXP_CM_Number { get; set; }

        [Display(Name = "Expense Base")]
        [Required(ErrorMessage = "Expense Base is Required")]
        public String? PII_EXP_ExpenseBase { get; set; }

        [Display(Name = "Expense Value")]
        [Required(ErrorMessage = "Expense Value is Required")]
        public String? PII_EXP_ExpenseValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? PII_EXP_Allocate_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? PII_EXP_LA_Number { get; set; }
        public String? PII_EXP_IsDeleted { get; set; }

        public void Reset()
        {
            this.PII_EXP_Number = 0;
            this.PII_EXP_POH_Number = 0;
            this.PII_EXP_POI_Number = 0;
            this.PII_EXP_Expense_Number = "";
            this.PII_EXP_Description = "";
            this.PII_EXP_Remarks = "";
            this.PII_EXP_Occurrence_Number = "";
            this.PII_EXP_CM_Number = "";
            this.PII_EXP_ExpenseBase = "";
            this.PII_EXP_ExpenseValue = "";
            this.PII_EXP_Allocate_Number = "";
            this.PII_EXP_LA_Number = "";
            this.PII_EXP_IsDeleted = "false";
        }
    }
    public class POToPurchaseInvoiceBatch_DTO
    {
        public Int64 PII_BCH_Number { get; set; }
        public Int64 PII_BCH_PII_Number { get; set; }
        public Int64 PII_BCH_POI_Number { get; set; }
        public Int64 PII_BCH_POH_Number { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Batch Date is Required")]
        public String? PII_BCH_Date { get; set; }

        [Display(Name = "Batch Number")]
        [Required(ErrorMessage = "Batch Number is Required")]
        public String? PII_BCH_No { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Batch Qty is Required")]
        public String? PII_BCH_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Batch Unit Price is Required")]
        public String? PII_BCH_UnitPrice { get; set; }

        [Display(Name = "Batch Value")]
        [Required(ErrorMessage = "Batch Qty is Required")]
        public String? PII_BCH_Value { get; set; }
        public String? PII_BCH_IsDeleted { get; set; }

        public void Reset()
        {
            this.PII_BCH_Number = 0;
            this.PII_BCH_PII_Number = 0;
            this.PII_BCH_POI_Number = 0;
            this.PII_BCH_POH_Number = 0;
            this.PII_BCH_Date = "";
            this.PII_BCH_No = "";
            this.PII_BCH_Qty = "";
            this.PII_BCH_UnitPrice = "";
            this.PII_BCH_Value = "";
            this.PII_BCH_IsDeleted = "false";
        }
    }


    public class POToPurchaseInvoiceGst
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
    public class POToPurchaseInvoiceWHT
    {
        public Int64 TaxNumber { get; set; }
        public String? TaxCode { get; set; }
        public Int64 Tax { get; set; }
        public Double Percentage { get; set; }
    }


    public class POToPurchaseInvoiceAscii
    {
        public Int64 PIH_Number { get; set; }

        [Display(Name = "GRN Date")]
        public String? PIH_InvoiceDate { get; set; }

        [Display(Name = "GRN Number")]
        public String? PIH_InvoiceNo { get; set; }

        [Display(Name = "Supplier Invoice Number")]
        public String? PIH_SupplierInvoiceNo { get; set; }

        [Display(Name = "Supplier Invoice Date")]
        public String? PIH_SupplierInvoiceDate { get; set; }

        [Display(Name = "Import Order")]
        public String? PIH_ImportOrder { get; set; }

        [Display(Name = "Vendor Name")]
        public String? PIH_Vendor_Number { get; set; }

        [Display(Name = "Currency")]
        public String? PIH_Currency_Number { get; set; }

        [Display(Name = "Tax Cluster")]
        public String? PIH_TaxCluster_Number { get; set; }

        [Display(Name = "WH Tax Code")]
        public String? PIH_WHT_Number { get; set; }

        [Display(Name = "Material Segregation")]
        public String? PIH_MS_Number { get; set; }

        [Display(Name = "No. of Line Item")]
        public String? PIH_TotalItem { get; set; }
        
        [Display(Name = "Qty")]
        public String? PII_Qty { get; set; }

        [Display(Name = "Material Value")]
        public String? PIH_MaterialCost { get; set; }

        [Display(Name = "Item Misc.Expense Value")]
        public String? PIH_ItemMiscExpense { get; set; }

        [Display(Name = "Header Misc.Expense Value")]
        public String? PIH_HeaderMiscExpense { get; set; }

        [Display(Name = "GST")]
        public String? PIH_GST_Amount { get; set; }

        [Display(Name = "Invoice Amount")]
        public String? PIH_InvoiceAmount { get; set; }

        [Display(Name = "Withhold Tax")]
        public String? PIH_WHT_Amount { get; set; }

        [Display(Name = "Round Off")]
        public String? PIH_RoundOff { get; set; }

        [Display(Name = "Vendor Payable")]
        public String? PIH_VendorPayable { get; set; }
    }
    public class POToPurchaseInvoiceDetailAscii
    {
        public Int64 PIH_Number { get; set; }

        [Display(Name = "GRN Date")]
        public String? PIH_InvoiceDate { get; set; }

        [Display(Name = "GRN Number")]
        public String? PIH_InvoiceNo { get; set; }

        [Display(Name = "Supplier Invoice Number")]
        public String? PIH_SupplierInvoiceNo { get; set; }

        [Display(Name = "Supplier Invoice Date")]
        public String? PIH_SupplierInvoiceDate { get; set; }

        [Display(Name = "Import Order")]
        public String? PIH_ImportOrder { get; set; }

        [Display(Name = "Vendor Name")]
        public String? PIH_Vendor_Number { get; set; }

        [Display(Name = "Currency")]
        public String? PIH_Currency_Number { get; set; }

        [Display(Name = "Material Segregation")]
        public String? PII_MS_Number { get; set; }

        [Display(Name = "Purchase Order Number")]
        public String? PII_POH_OrderNo { get; set; }

        [Display(Name = "Item Group")]
        public String? PII_ItemGroup { get; set; }

        [Display(Name = "Item Code")]
        public String? PII_ItemCode { get; set; }

        [Display(Name = "Description")]
        public String? PII_Description { get; set; }

        [Display(Name = "Warehouse")]
        public String? PII_Warehouse_Number { get; set; }

        [Display(Name = "UoM")]
        public String? PII_UoM { get; set; }

        [Display(Name = "Qty")]
        public String? PII_Qty { get; set; }

        [Display(Name = "Unit Price")]
        public String? PII_UnitPrice { get; set; }

        [Display(Name = "Material Value")]
        public String? PII_Amount { get; set; }

        [Display(Name = "Expense Code")]
        public String? PIH_EXP_Expense { get; set; }

        [Display(Name = "Item Misc.Expense Value")]
        public String? PIH_ItemMiscExpense { get; set; }

        [Display(Name = "Header Misc.Expense Value")]
        public String? PIH_HeaderMiscExpense { get; set; }

        [Display(Name = "HSN")]
        public String? PII_HSN_Number { get; set; }

        [Display(Name = "GST")]
        public String? PII_GST_Amount { get; set; }

        [Display(Name = "WH_Tax Code")]
        public String? PII_WHT_Code { get; set; }

        [Display(Name = "Percent")]
        public String? PII_WHT_Percent { get; set; }

        [Display(Name = "WH_Tax Amount")]
        public String? PII_WHT_Amount { get; set; }
    }


    public class POToPI_DTO
    {
        public List<PurchaseOrderPIItem_DTO>? POItems { get; set; }
        public List<PurchaseOrderPIExpense_DTO>? POExpenses { get; set; }
        public List<PurchaseOrderPIIExpense_DTO>? POItemExpenses { get; set; }
    }
    public class PurchaseOrderPIItem_DTO
    {
        public Int64 POI_Number { get; set; }
        public Int64 POH_Number { get; set; }
        public String? POH_OrderNo { get; set; }
        public Int64 POI_MS_Number { get; set; }
        public Int64 POI_Warehouse_Number { get; set; }
        public String? POI_Item_Number { get; set; }
        public String? POI_ItemCode { get; set; }
        public String? POI_ItemGroup { get; set; }
        public String? POI_HSN { get; set; }
        public String? POI_ItemDescription { get; set; }
        public String? POI_UoM_Number { get; set; }
        public String? POI_Qty { get; set; }
        public String? POI_UnitPrice { get; set; }
        public String? POI_Amount { get; set; }
        public String? POI_DecimalPlaces { get; set; }
    }
    public class PurchaseOrderPIExpense_DTO
    {
        public Int64 POH_EXP_POH_Number { get; set; }
        public Int64 POH_EXP_Number { get; set; }
        public String? POH_EXP_POH_OrderNo { get; set; }
        public String? POH_EXP_Expense_Number { get; set; }
        public String? POH_EXP_Description { get; set; }
        public String? POH_EXP_Remarks { get; set; }
        public String? POH_EXP_Occurrence_Number { get; set; }
        public String? POH_EXP_CM_Number { get; set; }
        public String? POH_EXP_ExpenseBase { get; set; }
        public String? POH_EXP_ExpenseValue { get; set; }
        public String? POH_EXP_Allocate_Number { get; set; }
        public String? POH_EXP_LA_Number { get; set; }
        public String? POH_EXP_SAC_Number { get; set; }
    }
    public class PurchaseOrderPIIExpense_DTO
    {
        public Int64 POI_EXP_Number { get; set; }
        public Int64 POI_EXP_POI_Number { get; set; }
        public Int64 POI_EXP_POH_Number { get; set; }
        public String? POI_EXP_Item_Number { get; set; }
        public String? POI_EXP_Expense_Number { get; set; }
        public String? POI_EXP_Description { get; set; }
        public String? POI_EXP_Remarks { get; set; }
        public String? POI_EXP_Occurrence_Number { get; set; }
        public String? POI_EXP_CM_Number { get; set; }
        public String? POI_EXP_ExpenseBase { get; set; }
        public String? POI_EXP_ExpenseValue { get; set; }
        public String? POI_EXP_Allocate_Number { get; set; }
        public String? POI_EXP_LA_Number { get; set; }
    }
}

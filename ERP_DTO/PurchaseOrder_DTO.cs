using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class PurchaseOrder_DTO
    {
        public Int64 POH_Number { get; set; }
        public String? POH_OrderNo { get; set; }
        public String? POH_Date { get; set; }

        public String? POH_Vendor_Number { get; set; }
        public String? POH_Vendor_Category { get; set; }
        public String? POH_Vendor_Group { get; set; }
        public String? POH_Vendor_Name { get; set; }

        public Int64 POH_ImportOrder { get; set; }
        public String? POH_Currency_Number { get; set; }
        public String? POH_ExchangeRate { get; set; }
        public String? POH_MS_Number { get; set; }
        public String? POH_PaymentTerms { get; set; }
        public String? POH_MOP { get; set; }
        public String? POH_DeliveryTerms { get; set; }
        public String? POH_MOD { get; set; }
        public String? POH_Tax { get; set; }
        public String? POH_Inspection { get; set; }
        public String? POH_TDC { get; set; }
        public String? POH_Remarks { get; set; }

        public String? POH_MaterialValue { get; set; }
        public String? POH_ItemMiscExpense { get; set; }
        public String? POH_HeadMiscExpense { get; set; }
        public String? POH_OrderSum { get; set; }
        public String? POH_OrderValue { get; set; }

        public String? POI_UoM { get; set; }
        public String? TotalItem { get; set; }
        public String? TotalQty { get; set; }
        public String? MaterialValue { get; set; }
        public String? ItemMiscExpense { get; set; }
        public String? HeadMiscExpense { get; set; }
        public String? POH_TotalValue { get; set; }

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

        public Int64 POI_Number { get; set; }
        public Int64 POI_Item_Number { get; set; }
        public String? POI_Item_Code { get; set; }
        public String? POI_Item_Group { get; set; }
        public String? POI_Item_Description { get; set; }
        public String? POI_Item_MaterialGrade { get; set; }
        public String? POI_Item_Thickness { get; set; }
        public String? POI_Item_Width { get; set; }
        public Int64 POI_Warehouse_Number { get; set; }
        public Int64 POI_UoM_Number { get; set; }
        public String? POI_DecimalPlaces { get; set; }
        public Double POI_Qty { get; set; }
        public Double POI_UnitPrice { get; set; }
        public Double POI_Amount { get; set; }
        public Double POI_ExpenseValue { get; set; }
        public String? POI_DeliveryDate { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.POH_Number = 0;
            this.POH_OrderNo = "";
            this.POH_Date = "";
            this.POH_Vendor_Number = "";
            this.POH_ImportOrder = 0;
            this.POH_Currency_Number = "";
            this.POH_MS_Number = "";
            this.POH_PaymentTerms = "";
            this.POH_MOP = "";
            this.POH_DeliveryTerms = "";
            this.POH_MOD = "";
            this.POH_Tax = "";
            this.POH_Inspection = "";
            this.POH_TDC = "";
            this.POH_Remarks = "";

            this.EXP_Number = 0;
            this.EXP_Expense_Number = 0;
            this.EXP_Description = "";
            this.EXP_Remarks = "";
            this.EXP_Occurrence_Number = 0;
            this.EXP_CM_Number = 0;
            this.EXP_ExpenseBase = 0;
            this.EXP_Allocate_Number = 0;
            this.EXP_LA_Number = 0;

            this.POI_Number = 0;
            this.POI_Item_Number = 0;
            this.POI_UoM_Number = 0;
            this.POI_Qty = 0;
            this.POI_UnitPrice = 0;
            this.POI_Amount = 0;
            this.POI_DeliveryDate = "";

            this.DeleteNumbers = "";
        }

    }


    public class PurchaseOrderHead_DTO
    {
        public Int64 POH_Number { get; set; }

        [Display(Name = "Purchase Order No")]
        [Required(ErrorMessage = "Purchase Order No is Required")]
        public String? POH_OrderNo { get; set; }

        [Display(Name = "Date")]
        [MaxLength(15, ErrorMessage = "PO Date be longer than 15 characters.")]
        [Required(ErrorMessage = "PO Date is Required")]
        public String? POH_Date { get; set; }

        [Display(Name = "Vendor Name")]
        [Required(ErrorMessage = "Vendor is Required")]
        public String? POH_Vendor_Number { get; set; }
        public String? POH_Vendor { get; set; }
        public String? POH_VendorLocation { get; set; }
        public String? POH_DecimalPlaces { get; set; }

        [Display(Name = "Import Order")]
        [Required(ErrorMessage = "Import Order is Required")]
        public String? POH_ImportOrder { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public String? POH_Currency_Number { get; set; }

        [Display(Name = "Exchange Rate")]
        public String? POH_ExchangeRate { get; set; }

        public String? POH_Currency { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public String? POH_MS_Number { get; set; }

        [Display(Name = "Payment Terms")]
        [MaxLength(50, ErrorMessage = "Payment terms be longer than 50 characters.")]
        public String? POH_PaymentTerms { get; set; }

        [Display(Name = "Method of Payment")]
        [MaxLength(50, ErrorMessage = "Method of payment be longer than 50 characters.")]
        public String? POH_MOP { get; set; }

        [Display(Name = "Delivery Terms")]
        [MaxLength(50, ErrorMessage = "Delivery terms be longer than 50 characters.")]
        public String? POH_DeliveryTerms { get; set; }

        [Display(Name = "Mode of Delivery")]
        [MaxLength(50, ErrorMessage = "Mode of delivery be longer than 50 characters.")]
        public String? POH_MOD { get; set; }

        [Display(Name = "Tax")]
        [MaxLength(250, ErrorMessage = "Tax be longer than 250 characters.")]
        public String? POH_Tax { get; set; }

        [Display(Name = "Inspection")]
        [MaxLength(250, ErrorMessage = "Inspection be longer than 250 characters.")]
        public String? POH_Inspection { get; set; }

        [Display(Name = "Technical Delivery Conditions")]
        [MaxLength(250, ErrorMessage = "Technical Delivery Conditions be longer than 250 characters.")]
        public String? POH_TDC { get; set; }


        public String? TotalItem { get; set; }
        public String? TotalQty { get; set; }
        public String? MaterialValue { get; set; }
        public String? MiscExpense { get; set; }

        [Display(Name = "Material Cost")]
        public String? POH_MaterialValue { get; set; }

        [Display(Name = "Item Misc.Expense")]
        public String? POH_ItemMiscExpense { get; set; }

        [Display(Name = "Header Misc.Expense")]
        public String? POH_HeadMiscExpense { get; set; }

        [Display(Name = "Purchase  Order Value")]
        public String? POH_OrderValue { get; set; }


        [Display(Name = "Remarks")]
        [MaxLength(250, ErrorMessage = "Remarks be longer than 250 characters.")]
        public String? POH_Remarks { get; set; }
        public List<PurchaseOrderItem_DTO>? PurchaseItems { get; set; }
        public List<PurchaseOrderExpense_DTO>? Expenses { get; set; }
        public List<PurchaseOrderIExpense_DTO>? ItemExpenses { get; set; }
        public void Reset()
        {
            this.POH_Number = 0;
            this.POH_OrderNo = null;
            this.POH_Date = "";
            this.POH_Vendor_Number = "";
            this.POH_Vendor = "";
            this.POH_VendorLocation = "";
            this.POH_DecimalPlaces = "";
            this.POH_ImportOrder = "false";
            this.POH_Currency_Number = "";
            this.POH_MS_Number = "";
            this.POH_PaymentTerms = "";
            this.POH_MOP = "";
            this.POH_DeliveryTerms = "";
            this.POH_MOD = "";
            this.POH_Tax = "";
            this.POH_Inspection = "";
            this.POH_TDC = "";
            this.POH_Remarks = "";
        }

    }
    public class PurchaseOrderItem_DTO
    {
        public Int64 POI_Number { get; set; }
        public Int64 POI_MS_Number { get; set; }
        public Int64 POI_Index { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? POI_Item_Number { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? POI_ItemCode { get; set; }

        [Display(Name = "Thickness")]
        public String? POI_ITM_Thickness { get; set; }

        [Display(Name = "Width")]
        public String? POI_ITM_Width { get; set; }

        [Display(Name = "Material Grade")]
        public String? POI_ITM_MaterialGrade { get; set; }

        [Display(Name = "Item Group")]
        public String? POI_ItemGroup { get; set; }

        [Display(Name = "Description")]
        public String? POI_Description { get; set; }

        [Display(Name = "UoM")]
        [Required(ErrorMessage = "UoM is Required")]
        public String? POI_UoM_Number { get; set; }

        public String? POI_UoM { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Qty is Required")]
        public String? POI_Qty { get; set; }

        [Display(Name = "Invoice Qty")]
        public String? POI_PII_Qty { get; set; }

        [Display(Name = "PO Qty")]
        public String? POI_OLD_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public String? POI_UnitPrice { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        public String? POI_Amount { get; set; }

        [Display(Name = "Expense Value")]
        [Required(ErrorMessage = "Expense value is Required")]
        public String? POI_ExpenseValue { get; set; }

        [Display(Name = "Delivery Date")]
        [Required(ErrorMessage = "Delivery Date is Required")]
        public String? POI_DeliveryDate { get; set; }
        public String? POI_DecimalPlaces { get; set; }
        public String? IsDeleted { get; set; }

        public void Reset()
        {
            this.POI_Number = 0;
            this.POI_Item_Number = "";
            this.POI_ItemCode = "";
            this.POI_ItemGroup = "";
            this.POI_Description = "";
            this.POI_UoM_Number = "";
            this.POI_Qty = "";
            this.POI_UnitPrice = "";
            this.POI_Amount = "";
            this.POI_DeliveryDate = "";
            this.POI_DecimalPlaces = "";
            this.IsDeleted = "false";
        }
    }
    public class PurchaseOrderExpense_DTO
    {
        public Int64 POH_EXP_Number { get; set; }

        [Display(Name = "Expense Code")]
        [Required(ErrorMessage = "Expense Code is Required")]
        public String? POH_EXP_Expense_Number { get; set; }

        [Display(Name = "Description")]
        public String? POH_EXP_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? POH_EXP_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? POH_EXP_Occurrence_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? POH_EXP_CM_Number { get; set; }

        [Display(Name = "Expense Base")]
        [Required(ErrorMessage = "Expense Base is Required")]
        public String? POH_EXP_ExpenseBase { get; set; }

        [Display(Name = "Expense Value")]
        [Required(ErrorMessage = "Expense Value is Required")]
        public String? POH_EXP_ExpenseValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? POH_EXP_Allocate_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? POH_EXP_LA_Number { get; set; }
        public String? IsDeleted { get; set; }

        public void Reset()
        {
            this.POH_EXP_Number = 0;
            this.POH_EXP_Expense_Number = "";
            this.POH_EXP_Description = "";
            this.POH_EXP_Remarks = "";
            this.POH_EXP_Occurrence_Number = "";
            this.POH_EXP_CM_Number = "";
            this.POH_EXP_ExpenseBase = "";
            this.POH_EXP_ExpenseValue = "";
            this.POH_EXP_Allocate_Number = "";
            this.POH_EXP_LA_Number = "";
            this.IsDeleted = "false";
        }
    }
    public class PurchaseOrderIExpense_DTO
    {
        public Int64 POI_EXP_Number { get; set; }
        public Int64 POI_EXP_POI_Number { get; set; }
        public String? POI_EXP_Item_Number { get; set; }
        public Int64 POI_EXP_Item_Index { get; set; }

        [Display(Name = "Expense Code")]
        [Required(ErrorMessage = "Expense Code is Required")]
        public String? POI_EXP_Expense_Number { get; set; }

        [Display(Name = "Description")]
        public String? POI_EXP_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? POI_EXP_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? POI_EXP_Occurrence_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? POI_EXP_CM_Number { get; set; }

        [Display(Name = "Expense Base")]
        [Required(ErrorMessage = "Expense Base is Required")]
        public String? POI_EXP_ExpenseBase { get; set; }

        [Display(Name = "Expense Value")]
        [Required(ErrorMessage = "Expense Value is Required")]
        public String? POI_EXP_ExpenseValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? POI_EXP_Allocate_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? POI_EXP_LA_Number { get; set; }
        public String? IsDeleted { get; set; }

        public void Reset()
        {
            this.POI_EXP_Number = 0;
            this.POI_EXP_Item_Number = "";
            this.POI_EXP_Item_Index = 0;
            this.POI_EXP_Expense_Number = "";
            this.POI_EXP_Description = "";
            this.POI_EXP_Remarks = "";
            this.POI_EXP_Occurrence_Number = "";
            this.POI_EXP_CM_Number = "";
            this.POI_EXP_ExpenseBase = "";
            this.POI_EXP_ExpenseValue = "";
            this.POI_EXP_Allocate_Number = "";
            this.POI_EXP_LA_Number = "";
            this.IsDeleted = "false";
        }
    }



    public class PurchaseOrderAscii
    {
        public Int64 POH_Number { get; set; }

        [Display(Name = "Date")]
        public String? POH_Date { get; set; }

        [Display(Name = "Order Number")]
        public String? POH_OrderNo { get; set; }

        [Display(Name = "Vendor Name")]
        public String? POH_Vendor { get; set; }

        [Display(Name = "Currency")]
        public String? POH_Currency { get; set; }

        [Display(Name = "Import Order")]
        public String? POH_ImportOrder { get; set; }

        [Display(Name = "Material Segregation")]
        public String? POH_MS_Number { get; set; }

        [Display(Name = "No. of Item")]
        public String? TotalItem { get; set; }

        [Display(Name = "Qty")]
        public String? TotalQty { get; set; }

        [Display(Name = "Material Value")]
        public String? MaterialValue { get; set; }

        [Display(Name = "Item Misc Expense Value")]
        public String? ItemMiscExpense { get; set; }

        [Display(Name = "Head Misc Expense Value")]
        public String? HeadMiscExpense { get; set; }
    }
    public class PurchaseOrderDetailAscii
    {
        public Int64 POH_Number { get; set; }

        [Display(Name = "Date")]
        public String? POH_Date { get; set; }

        [Display(Name = "Order Number")]
        public String? POH_OrderNo { get; set; }

        [Display(Name = "Vendor Name")]
        public String? POH_Vendor { get; set; }

        [Display(Name = "Currency")]
        public String? POH_Currency { get; set; }

        [Display(Name = "Import Order")]
        public String? POH_ImportOrder { get; set; }

        [Display(Name = "Material Segregation")]
        public String? POH_MS_Number { get; set; }

        [Display(Name = "Item Group")]
        public String? POI_Item_Group { get; set; }

        [Display(Name = "Item Code")]
        public String? POI_Item_Code { get; set; }

        [Display(Name = "Item Description")]
        public String? POI_Item_Description { get; set; }

        [Display(Name = "UoM")]
        public String? POI_UoM { get; set; }

        [Display(Name = "Qty")]
        public String? POI_Qty { get; set; }

        [Display(Name = "Unit Price")]
        public String? POI_UnitPrice { get; set; }

        [Display(Name = "Material Value")]
        public String? POI_Amount { get; set; }

        [Display(Name = "Item Misc Expense Value")]
        public String? ItemMiscExpense { get; set; }

        [Display(Name = "Head Misc Expense Value")]
        public String? HeadMiscExpense { get; set; }

        [Display(Name = "Delivery Date")]
        public String? POI_DeliveryDate { get; set; }
    }
}

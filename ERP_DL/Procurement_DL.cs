using ERP_DTO;
using System;
using System.Data;
using System.Globalization;

namespace ERP.DataList
{
    public class Procurement_DL
    {
        CultureInfo India = new CultureInfo("hi-IN");
        MethodHelp MH = new MethodHelp();

        public List<Item_DTO> IList(DataTable Dt)
        {
            List<Item_DTO> IList = new List<Item_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new Item_DTO
                    {
                        ItemNumber = Convert.ToInt64(dr["ItemNumber"]),
                        ItemCode = Convert.ToString(dr["ItemCode"]),
                        Thickness = Convert.ToString(dr["Thickness"]),
                        ITM_Width = Convert.ToString(dr["ITM_Width"]),
                        MaterialGrade = Convert.ToString(dr["MaterialGrade"]),
                        ItemDescription = Convert.ToString(dr["ItemDescription"]),
                        ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        UoM = Convert.ToString(dr["Uom"]),
                        DecimalPlaces = Convert.ToInt32(dr["DecimalPlaces"]),
                        MaterialSegregation = Convert.ToString(dr["MaterialSegregation"]),
                    });
            }
            return IList;
        }
        public List<Vendor_DTO> VList(DataTable Dt)
        {
            List<Vendor_DTO> VList = new List<Vendor_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VList.Add(
                    new Vendor_DTO
                    {
                        VendorNumber = Convert.ToInt64(dr["VendorNumber"]),
                        VendorCode = Convert.ToString(dr["VendorCode"]),
                        VendorName = Convert.ToString(dr["VendorName"]),
                        VendorLocation = Convert.ToString(dr["VendorLocation"]),
                        Currency = Convert.ToString(dr["CurrencyNumber"]),
                        CurrencyCode = Convert.ToString(dr["CurrencyCode"]),
                        DecimalPlaces = Convert.ToString(dr["DecimalPlaces"]),
                    });
            }
            return VList;
        }
        public List<Item_DTO> PIIList(DataTable Dt)
        {
            List<Item_DTO> IList = new List<Item_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new Item_DTO
                    {
                        ItemNumber = Convert.ToInt64(dr["ItemNumber"]),
                        ItemCode = Convert.ToString(dr["ItemCode"]),
                        ItemDescription = Convert.ToString(dr["ItemDescription"]),
                        ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        UoM = Convert.ToString(dr["Uom"]),
                        DecimalPlaces = Convert.ToInt32(dr["DecimalPlaces"]),
                        MaterialSegregation = Convert.ToString(dr["MaterialSegregation"]),
                        HSN_Code = Convert.ToString(dr["HSN_Code"]),
                        PurchaseWarehouse = Convert.ToInt32(dr["PurchaseWarehouse"]),
                    });
            }
            return IList;
        }
        public List<Vendor_DTO> PIVList(DataTable Dt)
        {
            List<Vendor_DTO> VList = new List<Vendor_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VList.Add(
                    new Vendor_DTO
                    {
                        VendorNumber = Convert.ToInt64(dr["VendorNumber"]),
                        VendorCode = Convert.ToString(dr["VendorCode"]),
                        VendorName = Convert.ToString(dr["VendorName"]),
                        VendorLocation = Convert.ToString(dr["VendorLocation"]),
                        Currency = Convert.ToString(dr["CurrencyNumber"]),
                        CurrencyCode = Convert.ToString(dr["CurrencyCode"]),
                        DecimalPlaces = Convert.ToString(dr["DecimalPlaces"]),
                        Cluster = Convert.ToString(dr["TaxCluster"]),
                        TaxNumber = Convert.ToInt64(dr["TaxClusterNumber"]),
                        WithholdTax = Convert.ToInt64(dr["TaxCode"]),
                        CreditDays = Convert.ToInt32(dr["CreditDays"]),
                        PaymentBase = Convert.ToInt64(dr["PaymentBase"]),
                    });
            }
            return VList;
        }
        public List<Currency_DTO> CurencyList(DataTable Dt)
        {
            List<Currency_DTO> CurList = new List<Currency_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CurList.Add(
                    new Currency_DTO
                    {
                        CurrencyCode = Convert.ToString(dr["CurrencyCode"]),
                        DecimalPlaces = Convert.ToString(dr["DecimalPlaces"]),
                    });
            }
            return CurList;
        }
        public List<PurchaseOrder_DTO> POList(DataTable Dt)
        {
            List<PurchaseOrder_DTO> POList = new List<PurchaseOrder_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrder_DTO
                    {
                        POH_Number = Convert.ToInt64(dr["POH_Number"]),
                        POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_ImportOrder = Convert.ToInt32(dr["POH_ImportOrder"]),
                        POH_Date = Convert.ToString(dr["POH_Date"]),
                        POH_Vendor_Group = Convert.ToString(dr["POH_Vendor_Group"]),
                        POH_Vendor_Category = Convert.ToString(dr["POH_Vendor_Category"]),
                        POH_Vendor_Name = Convert.ToString(dr["POH_Vendor_Name"]),
                        POH_ExchangeRate = Convert.ToString(dr["POH_ExchangeRate"]),
                        POH_Currency_Number = Convert.ToString(dr["POH_Currency_Number"]),
                        POH_MS_Number = Convert.ToString(dr["POH_MS_Number"]),
                        TotalItem = Convert.ToString(dr["TotalItem"]),
                        TotalQty =  DecimalConvertQty(Convert.ToDouble(dr["TotalQty"])),
                        POH_MaterialValue = Convert.ToString(dr["POH_MaterialValue"]),
                        POH_ItemMiscExpense = Convert.ToString(dr["POH_ItemMiscExpense"]),
                        POH_HeadMiscExpense = Convert.ToString(dr["POH_HeadMiscExpense"]),
                        POH_OrderValue = Convert.ToString(dr["POH_OrderValue"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrderAscii> POAscii(DataTable Dt)
        {
            List<PurchaseOrderAscii> POList = new List<PurchaseOrderAscii>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderAscii
                    {
                        POH_Number = Convert.ToInt64(dr["POH_Number"]),
                        POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_ImportOrder = Convert.ToString(dr["POH_ImportOrder"]) == "1"?"Yes":"No",
                        POH_Date = Convert.ToString(dr["POH_Date"]),
                        POH_Vendor = Convert.ToString(dr["POH_Vendor_Number"]),
                        POH_Currency = Convert.ToString(dr["POH_Currency_Number"]),
                        POH_MS_Number = Convert.ToString(dr["POH_MS_Number"]),
                        TotalItem = Convert.ToString(dr["TotalItem"]),
                        TotalQty = Convert.ToString(dr["TotalQty"]),
                        MaterialValue = Convert.ToString(dr["MaterialValue"]),
                        ItemMiscExpense = Convert.ToString(dr["ItemMiscExpense"]),
                        HeadMiscExpense = Convert.ToString(dr["HeadMiscExpense"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrder_DTO> PODetailList(DataTable Dt)
        {
            List<PurchaseOrder_DTO> POList = new List<PurchaseOrder_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrder_DTO
                    {
                        POH_Number = Convert.ToInt64(dr["POH_Number"]),
                        POI_Number = Convert.ToInt64(dr["POI_Number"]),
                        POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_ImportOrder = Convert.ToInt32(dr["POH_ImportOrder"]),
                        POH_Date = Convert.ToString(dr["POH_Date"]),
                        POH_Vendor_Group = Convert.ToString(dr["POH_Vendor_Group"]),
                        POH_Vendor_Category = Convert.ToString(dr["POH_Vendor_Category"]),
                        POH_Vendor_Name = Convert.ToString(dr["POH_Vendor_Name"]),
                        POH_ExchangeRate = Convert.ToString(dr["POH_ExchangeRate"]),
                        POH_Currency_Number = Convert.ToString(dr["POH_Currency_Number"]),
                        POH_MS_Number = Convert.ToString(dr["POH_MS_Number"]),

                        POI_Item_MaterialGrade = Convert.ToString(dr["POI_Item_MaterialGrade"]),
                        POI_Item_Thickness = Convert.ToString(dr["POI_Item_Thickness"]),
                        POI_Item_Width = Convert.ToString(dr["POI_Item_Width"]),

                        POI_Item_Code = Convert.ToString(dr["ItemCode"]),
                        POI_Item_Group = Convert.ToString(dr["ItemGroup"]),
                        POI_Item_Description = Convert.ToString(dr["ItemDescription"]),
                        POI_UoM = Convert.ToString(dr["POI_UoM_Number"]),

                        POI_Qty = Convert.ToDouble(dr["POI_Qty"]),
                        POI_UnitPrice = Convert.ToDouble(dr["POI_UnitPrice"]),
                        POI_Amount = Convert.ToDouble(dr["POI_Amount"]),
                        POI_ExpenseValue = Convert.ToDouble(dr["POI_ExpenseValue"]),
                        HeadMiscExpense = Convert.ToString(dr["HeadMiscExpense"]),
                        POH_TotalValue = Convert.ToString(dr["POH_TotalValue"]),
                        POI_DeliveryDate = Convert.ToString(dr["POI_DeliveryDate"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrderDetailAscii> PODetailAscii(DataTable Dt)
        {
            List<PurchaseOrderDetailAscii> POList = new List<PurchaseOrderDetailAscii>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderDetailAscii
                    {
                        POH_Number = Convert.ToInt64(dr["POI_Number"]),
                        POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_ImportOrder = Convert.ToString(dr["POH_ImportOrder"]) == "1" ? "Yes" : "No",
                        POH_Date = Convert.ToString(dr["POH_Date"]),
                        POH_Vendor = Convert.ToString(dr["POH_Vendor_Number"]),
                        POH_Currency = Convert.ToString(dr["POH_Currency_Number"]),
                        POH_MS_Number = Convert.ToString(dr["POH_MS_Number"]),

                        POI_Item_Group = Convert.ToString(dr["ItemGroup"]),
                        POI_Item_Code = Convert.ToString(dr["ItemCode"]),
                        POI_Item_Description = Convert.ToString(dr["ItemDescription"]),
                        POI_UoM = Convert.ToString(dr["POI_UoM_Number"]),

                        POI_Qty = Convert.ToString(dr["POI_Qty"]),
                        POI_UnitPrice = Convert.ToString(dr["POI_UnitPrice"]),
                        POI_Amount = Convert.ToString(dr["POI_Amount"]),
                        ItemMiscExpense = Convert.ToString(dr["ItemMiscExpense"]),
                        HeadMiscExpense = Convert.ToString(dr["HeadMiscExpense"]),
                        POI_DeliveryDate = Convert.ToString(dr["POI_DeliveryDate"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrderExpense_DTO> ExList(DataTable Dt)
        {
            List<PurchaseOrderExpense_DTO> IList = new List<PurchaseOrderExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseOrderExpense_DTO
                    {
                        POH_EXP_Description = Convert.ToString(dr["EC_Description"]),
                        POH_EXP_LA_Number = Convert.ToString(dr["LedgerAccount"])
                    });
            }
            return IList;
        }
        public List<PurchaseOrderHead_DTO> POHeadList(DataTable Dt)
        {
            List<PurchaseOrderHead_DTO> POList = new List<PurchaseOrderHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderHead_DTO
                    {
                        POH_Number = Convert.ToInt64(dr["POH_Number"]),
                        POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_ImportOrder = Convert.ToString(dr["POH_ImportOrder"]),
                        POH_Date = Convert.ToString(dr["POH_Date"]),
                        POH_Vendor_Number = Convert.ToString(dr["POH_Vendor_Number"]),
                        POH_Currency = Convert.ToString(dr["POH_Currency"]),
                        POH_MS_Number = Convert.ToString(dr["POH_MS_Number"]),
                        POH_PaymentTerms = Convert.ToString(dr["POH_PaymentTerms"]),
                        POH_MOP = Convert.ToString(dr["POH_MOP"]),
                        POH_DeliveryTerms = Convert.ToString(dr["POH_DeliveryTerms"]),
                        POH_MOD = Convert.ToString(dr["POH_MOD"]),
                        TotalQty = Convert.ToString(dr["TotalQty"]),
                        MaterialValue = Convert.ToString(dr["MaterialValue"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrderItem_DTO> POItemList(DataTable Dt)
        {
            List<PurchaseOrderItem_DTO> POList = new List<PurchaseOrderItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderItem_DTO
                    {
                        POI_ItemCode = Convert.ToString(dr["POI_ItemCode"]),
                        POI_Description = Convert.ToString(dr["POI_Description"]),
                        POI_UoM = Convert.ToString(dr["POI_UoM"]),
                        POI_Qty = Convert.ToString(dr["POI_Qty"]),
                        POI_UnitPrice = Convert.ToString(dr["POI_UnitPrice"]),
                        POI_Amount = Convert.ToString(dr["POI_Amount"]),
                    });
            }
            return POList;
        }
        public List<Vendor_DTO> VendorList(DataTable Dt)
        {
            List<Vendor_DTO> VList = new List<Vendor_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VList.Add(
                    new Vendor_DTO
                    {
                        VendorNumber = Convert.ToInt64(dr["VendorNumber"]),
                        VendorCode = Convert.ToString(dr["VendorCode"]),
                        VendorName = Convert.ToString(dr["VendorName"]),
                        VendorLocation = Convert.ToString(dr["VendorLocation"]),
                        Address = Convert.ToString(dr["Address"]),
                        City = Convert.ToString(dr["City"]),
                        State = Convert.ToString(dr["State"]),
                        Pincode = Convert.ToString(dr["Pincode"]),
                        GSTIN = Convert.ToString(dr["GSTIN"]),
                        PAN = Convert.ToString(dr["PAN"]),
                    });
            }
            return VList;
        }
        public List<PurchaseOrderHead_DTO> POHeadEditList(DataTable Dt)
        {
            List<PurchaseOrderHead_DTO> POList = new List<PurchaseOrderHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderHead_DTO
                    {
                        POH_Number = Convert.ToInt64(dr["POH_Number"]),
                        POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_ImportOrder = Convert.ToString(dr["POH_ImportOrder"]) == "1" ? "true" : "false",
                        POH_Date = Convert.ToString(dr["POH_Date"]),
                        POH_Vendor_Number = Convert.ToString(dr["POH_Vendor_Number"]),
                        POH_Vendor = Convert.ToString(dr["POH_Vendor"]),
                        POH_VendorLocation = Convert.ToString(dr["POH_VendorLocation"]),
                        POH_Currency_Number = Convert.ToString(dr["POH_Currency_Number"]),
                        POH_Currency = Convert.ToString(dr["POH_Currency"]),
                        POH_DecimalPlaces = Convert.ToString(dr["POH_DecimalPlaces"]),
                        POH_MS_Number = Convert.ToString(dr["POH_MS_Number"]),
                        POH_PaymentTerms = Convert.ToString(dr["POH_PaymentTerms"]),
                        POH_MOP = Convert.ToString(dr["POH_MOP"]),
                        POH_DeliveryTerms = Convert.ToString(dr["POH_DeliveryTerms"]),
                        POH_MOD = Convert.ToString(dr["POH_MOD"]),
                        POH_Tax = Convert.ToString(dr["POH_Tax"]),
                        POH_ExchangeRate = Convert.ToString(dr["POH_ExchangeRate"]),
                        POH_Inspection = Convert.ToString(dr["POH_Inspection"]),
                        POH_TDC = Convert.ToString(dr["POH_TDC"]),
                        POH_Remarks = Convert.ToString(dr["POH_Remarks"]),
                        POH_MaterialValue = Convert.ToString(dr["POH_MaterialValue"]),
                        POH_ItemMiscExpense = Convert.ToString(dr["POH_ItemMiscExpense"]),
                        POH_HeadMiscExpense = Convert.ToString(dr["POH_HeadMiscExpense"]),
                        POH_OrderValue = Convert.ToString(dr["POH_OrderValue"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrderItem_DTO> POItemEditList(DataTable Dt)
        {
            List<PurchaseOrderItem_DTO> POList = new List<PurchaseOrderItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderItem_DTO
                    {
                        POI_Number = Convert.ToInt64(dr["POI_Number"]),
                        POI_MS_Number = Convert.ToInt64(dr["POI_MS_Number"]),
                        POI_Item_Number = Convert.ToString(dr["POI_Item_Number"]),
                        POI_ItemCode = Convert.ToString(dr["POI_ItemCode"]),
                        POI_ItemGroup = Convert.ToString(dr["POI_ItemGroup"]),
                        POI_ITM_Thickness = Convert.ToString(dr["Thickness"]),
                        POI_ITM_Width = Convert.ToString(dr["ITM_Width"]),
                        POI_ITM_MaterialGrade = Convert.ToString(dr["MaterialGrade"]),
                        POI_Description = Convert.ToString(dr["POI_Description"]),
                        POI_UoM = Convert.ToString(dr["POI_UoM"]),
                        POI_UoM_Number = Convert.ToString(dr["POI_UoM_Number"]),
                        POI_Qty = DecimalConvertQty(Convert.ToDouble(dr["POI_Qty"])),
                        POI_PII_Qty = DecimalConvertQty(Convert.ToDouble(dr["POI_PII_Qty"])),
                        POI_UnitPrice = DecimalConvertUnit(Convert.ToDouble(dr["POI_UnitPrice"])),
                        POI_Amount = Convert.ToString(dr["POI_Amount"]),
                        POI_ExpenseValue = Convert.ToString(dr["POI_ExpenseValue"]),
                        POI_DeliveryDate = Convert.ToString(dr["POI_DeliveryDate"]),
                        POI_DecimalPlaces = Convert.ToString(dr["POI_DecimalPlaces"]),
                        IsDeleted = "false"
                    });
            }
            return POList;
        }
        public List<PurchaseOrderExpense_DTO> POExpenseEditList(DataTable Dt)
        {
            List<PurchaseOrderExpense_DTO> POList = new List<PurchaseOrderExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderExpense_DTO
                    {
                        POH_EXP_Number = Convert.ToInt64(dr["POH_EXP_Number"]),
                        POH_EXP_Expense_Number = Convert.ToString(dr["POH_EXP_Expense_Number"]),
                        POH_EXP_Description = Convert.ToString(dr["POH_EXP_Description"]),
                        POH_EXP_Remarks = Convert.ToString(dr["POH_EXP_Remarks"]),
                        POH_EXP_Occurrence_Number = Convert.ToString(dr["POH_EXP_Occurrence_Number"]),
                        POH_EXP_CM_Number = Convert.ToString(dr["POH_EXP_CM_Number"]),
                        POH_EXP_ExpenseBase = Convert.ToString(dr["POH_EXP_ExpenseBase"]),
                        POH_EXP_ExpenseValue = Convert.ToString(dr["POH_EXP_ExpenseValue"]),
                        POH_EXP_Allocate_Number = Convert.ToString(dr["POH_EXP_Allocate_Number"]),
                        POH_EXP_LA_Number = Convert.ToString(dr["POH_EXP_LA_Number"]),
                        IsDeleted = "false"
                    });
            }
            return POList;
        }
        public List<PurchaseOrderIExpense_DTO> POIExpenseEditList(DataTable Dt)
        {
            List<PurchaseOrderIExpense_DTO> POList = new List<PurchaseOrderIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderIExpense_DTO
                    {
                        POI_EXP_Number = Convert.ToInt64(dr["POI_EXP_Number"]),
                        POI_EXP_POI_Number = Convert.ToInt64(dr["POI_EXP_POI_Number"]),
                        POI_EXP_Expense_Number = Convert.ToString(dr["POI_EXP_Expense_Number"]),
                        POI_EXP_Description = Convert.ToString(dr["POI_EXP_Description"]),
                        POI_EXP_Remarks = Convert.ToString(dr["POI_EXP_Remarks"]),
                        POI_EXP_Occurrence_Number = Convert.ToString(dr["POI_EXP_Occurrence_Number"]),
                        POI_EXP_CM_Number = Convert.ToString(dr["POI_EXP_CM_Number"]),
                        POI_EXP_ExpenseBase = Convert.ToString(dr["POI_EXP_ExpenseBase"]),
                        POI_EXP_ExpenseValue = Convert.ToString(dr["POI_EXP_ExpenseValue"]),
                        POI_EXP_Allocate_Number = Convert.ToString(dr["POI_EXP_Allocate_Number"]),
                        POI_EXP_LA_Number = Convert.ToString(dr["POI_EXP_LA_Number"]),
                        IsDeleted = "false"
                    });
            }
            return POList;
        }






        //INVOICE
        public List<PurchaseInvoiceExpense_DTO> ExInvoiceList(DataTable Dt)
        {
            List<PurchaseInvoiceExpense_DTO> IList = new List<PurchaseInvoiceExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceExpense_DTO
                    {
                        PIH_EXP_Description = Convert.ToString(dr["EC_Description"]),
                        PIH_EXP_LA_Number = Convert.ToString(dr["LedgerAccount"]),
                        PIH_EXP_SAC_Number = Convert.ToString(dr["EC_SAC_Number"])
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceGst> PurInvGst(DataTable Dt)
        {
            List<PurchaseInvoiceGst> IList = new List<PurchaseInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceGst
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        Chargeable = Convert.ToString(dr["Chargeable"]),
                        Calculation = Convert.ToInt64(dr["Calculation"]),
                        Percentage = Convert.ToDouble(dr["Percentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceGst> PurInvGstView(DataTable Dt)
        {
            List<PurchaseInvoiceGst> IList = new List<PurchaseInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceGst
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        TaxCategory = Convert.ToString(dr["TaxCategory"]),
                        TaxType = Convert.ToString(dr["TaxType"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        TaxElementName = Convert.ToString(dr["TaxElementName"]),
                        LoadonInventory = Convert.ToString(dr["LoadonInventory"]),
                        LoadonInventoryPercent = Convert.ToString(dr["LoadonInventoryPercent"]),
                        Chargeable = Convert.ToString(dr["Chargeable"]),
                        Calculation = Convert.ToInt64(dr["Calculation"]),
                        Percentage = Convert.ToDouble(dr["Percentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceWHT> PurInvWHT(DataTable Dt)
        {
            List<PurchaseInvoiceWHT> IList = new List<PurchaseInvoiceWHT>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceWHT
                    {
                        TaxNumber = Convert.ToInt64(dr["TaxNumber"]),
                        TaxCode = Convert.ToString(dr["TaxCode"]),
                        Tax = Convert.ToInt64(dr["IncludeTax"]),
                        Percentage = Convert.ToDouble(dr["TaxPercentage"])
                    });
            }
            return IList;
        }
        public List<TaxCluster_DTO> PurCluster(DataTable Dt)
        {
            List<TaxCluster_DTO> IList = new List<TaxCluster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new TaxCluster_DTO
                    {
                        TaxClusterNumber = Convert.ToInt64(dr["TaxClusterNumber"]),
                        TaxCluster = Convert.ToString(dr["TaxCluster"]),
                    });
            }
            return IList;
        }
        public List<PurchaseInvoice_DTO> PIList(DataTable Dt)
        {
            List<PurchaseInvoice_DTO> POList = new List<PurchaseInvoice_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseInvoice_DTO
                    {
                        PIH_Number = Convert.ToInt64(dr["PIH_Number"]),
                        PIH_POH_OrderNo = Convert.ToString(dr["PIH_POH_OrderNo"]),
                        //PIH_POH_Date = Convert.ToString(dr["PIH_POH_Date"]),
                        PIH_POH_OrderCount = Convert.ToInt32(dr["PIH_POH_OrderCount"]),

                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PIH_ImportOrder = Convert.ToInt32(dr["PIH_ImportOrder"]),
                        PIH_Vendor_Category = Convert.ToString(dr["PIH_Vendor_Category"]),
                        PIH_Vendor_Group = Convert.ToString(dr["PIH_Vendor_Group"]),
                        PIH_Vendor_Name = Convert.ToString(dr["PIH_Vendor_Name"]),
                        PIH_ExchangeRate = Convert.ToDouble(dr["PIH_ExchangeRate"]),
                        PIH_Currency_Number = Convert.ToString(dr["PIH_Currency_Number"]),
                        PIH_TaxCluster_Number = Convert.ToString(dr["PIH_TaxCluster_Number"]),
                        PIH_WHT_Number = Convert.ToString(dr["PIH_WHT_Number"]),
                        PIH_MS_Number = Convert.ToString(dr["PIH_MS_Number"]),

                        PII_Qty = DecimalConvertQty(Convert.ToDouble(dr["PII_Qty"])),
                        PIH_MaterialCost = Convert.ToString(dr["PIH_MaterialCost"]),
                        PIH_ItemMiscExpense = Convert.ToString(dr["PIH_ItemMiscExpense"]),
                        PIH_HeaderMiscExpense = Convert.ToString(dr["PIH_HeaderMiscExpense"]),
                        PIH_Amount = Convert.ToString(dr["PIH_Amount"]),

                        PIH_GST_Amount = Convert.ToString(dr["PIH_GST_Amount"]),
                        PII_GST_Amount = Convert.ToString(dr["PII_GST_Amount"]),
                        PIH_InvoiceAmount = Convert.ToString(dr["PIH_InvoiceAmount"]),
                        PIH_WHT_Amount = Convert.ToString(dr["PIH_WHT_Amount"]),
                        PIH_RoundOff = Convert.ToString(dr["PIH_RoundOff"]),
                        PIH_VendorPayable = Convert.ToString(dr["PIH_VendorPayable"]),

                        PIH_TotalItem = Convert.ToString(dr["PIH_TotalItem"]),
                    });
            }
            return POList;
        }

        public List<PurchaseInvoiceAscii> PIAscii(DataTable Dt)
        {
            List<PurchaseInvoiceAscii> POList = new List<PurchaseInvoiceAscii>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseInvoiceAscii
                    {
                        PIH_Number = Convert.ToInt64(dr["PIH_Number"]),
                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PIH_ImportOrder = Convert.ToString(dr["PIH_ImportOrder"]) == "1" ? "Yes" : "No",
                        PIH_Vendor_Number = Convert.ToString(dr["PIH_Vendor_Number"]),
                        PIH_Currency_Number = Convert.ToString(dr["PIH_Currency_Number"]),
                        PIH_TaxCluster_Number = Convert.ToString(dr["PIH_TaxCluster_Number"]),
                        PIH_WHT_Number = Convert.ToString(dr["PIH_WHT_Number"]),
                        PIH_MS_Number = Convert.ToString(dr["PIH_MS_Number"]),

                        PII_Qty = Convert.ToString(dr["PII_Qty"]),
                        PIH_MaterialCost = Convert.ToString(dr["PIH_MaterialCost"]),
                        PIH_ItemMiscExpense = Convert.ToString(dr["PIH_ItemMiscExpense"]),
                        PIH_HeaderMiscExpense = Convert.ToString(dr["PIH_HeaderMiscExpense"]),

                        PIH_GST_Amount = Convert.ToString(dr["PIH_GST_Amount"]),
                        PIH_InvoiceAmount = Convert.ToString(dr["PIH_InvoiceAmount"]),
                        PIH_WHT_Amount = Convert.ToString(dr["PIH_WHT_Amount"]),
                        PIH_RoundOff = Convert.ToString(dr["PIH_RoundOff"]),
                        PIH_VendorPayable = Convert.ToString(dr["PIH_VendorPayable"]),

                        PIH_TotalItem = Convert.ToString(dr["PIH_TotalItem"])
                    });
            }
            return POList;
        }
        public List<PurchaseInvoice_DTO> PIDetailList(DataTable Dt)
        {
            List<PurchaseInvoice_DTO> POList = new List<PurchaseInvoice_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseInvoice_DTO
                    {
                        PIH_Number = Convert.ToInt64(dr["PIH_Number"]),
                        PII_Number = Convert.ToInt64(dr["PII_Number"]),
                        PIH_POH_OrderNo = Convert.ToString(dr["PIH_POH_OrderNo"]),
                        PIH_POH_Date = Convert.ToString(dr["PIH_POH_Date"]),
                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PIH_ImportOrder = Convert.ToInt32(dr["PIH_ImportOrder"]),
                        PIH_Vendor_Category = Convert.ToString(dr["PIH_Vendor_Category"]),
                        PIH_Vendor_Group = Convert.ToString(dr["PIH_Vendor_Group"]),
                        PIH_Vendor_Name = Convert.ToString(dr["PIH_Vendor_Name"]),
                        PIH_Currency_Number = Convert.ToString(dr["PIH_Currency_Number"]),
                        PII_MS_Number = Convert.ToString(dr["PII_MS_Number"]),
                        PII_ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        PII_ItemCode = Convert.ToString(dr["ItemCode"]),
                        PII_Description = Convert.ToString(dr["ItemDescription"]),
                        PII_Item_MaterialGrade = Convert.ToString(dr["PII_Item_MaterialGrade"]),
                        PII_Item_Thickness = Convert.ToString(dr["PII_Item_Thickness"]),
                        PII_Item_Width = Convert.ToString(dr["PII_Item_Width"]),
                        PII_Warehouse_Number = Convert.ToString(dr["PII_Warehouse_Number"]),
                        PII_UoM = Convert.ToString(dr["PII_UoM_Number"]),
                        PII_Qty = DecimalConvertQty(Convert.ToDouble(dr["PII_Qty"])),
                        PII_UnitPrice = DecimalConvertUnit(Convert.ToDouble(dr["PII_UnitPrice"])),
                        PII_Amount = Convert.ToString(dr["PII_Amount"]),
                        PIH_EXP_Expense = Convert.ToString(dr["PIH_EXP_Expense"]),
                        PIH_ItemMiscExpense = Convert.ToString(dr["PII_ItemMiscExpense"]),
                        PIH_HeaderMiscExpense = Convert.ToString(dr["PIH_HeaderMiscExpense"]),
                        PIH_Amount = Convert.ToString(dr["PIH_Amount"]),
                        PIH_ExchangeRate = Convert.ToDouble(dr["PIH_ExchangeRate"]),
                        PII_HSN_Number = Convert.ToString(dr["PII_HSN_Number"]),
                        PII_HSN_Description = Convert.ToString(dr["PII_HSN_Description"]),
                        PII_GST_Amount = Convert.ToString(dr["PII_GST_Amount"]),
                        PIH_GST_Amount = Convert.ToString(dr["PIH_GST_Amount"]),
                        PIH_InvoiceAmount = Convert.ToString(dr["PIH_InvoiceAmount"]),
                        PII_WHT_Amount = Convert.ToString(dr["PII_WHT_Amount"]),
                        PII_WHT_Percent = Convert.ToString(dr["PII_WHT_Percent"]),
                        PIH_RoundOff = Convert.ToString(dr["PIH_RoundOff"]),
                        PIH_VendorPayable = Convert.ToString(dr["PIH_VendorPayable"])
                    });
            }
            return POList;
        }
        public List<PurchaseInvoiceDetailAscii> PIDAscii(DataTable Dt)
        {
            List<PurchaseInvoiceDetailAscii> POList = new List<PurchaseInvoiceDetailAscii>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseInvoiceDetailAscii
                    {
                        PIH_Number = Convert.ToInt64(dr["PII_Number"]),
                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PIH_ImportOrder = Convert.ToString(dr["PIH_ImportOrder"]) == "1" ? "Yes" : "No",
                        PIH_Vendor_Number = Convert.ToString(dr["PIH_Vendor_Number"]),
                        PIH_Currency_Number = Convert.ToString(dr["PIH_Currency_Number"]),
                        PII_MS_Number = Convert.ToString(dr["PII_MS_Number"]),
                        PII_ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        PII_ItemCode = Convert.ToString(dr["ItemCode"]),
                        PII_Description = Convert.ToString(dr["ItemDescription"]),
                        PII_Warehouse_Number = Convert.ToString(dr["PII_Warehouse_Number"]),
                        PII_UoM = Convert.ToString(dr["PII_UoM_Number"]),
                        PII_Qty = Convert.ToString(dr["PII_Qty"]),
                        PII_UnitPrice = Convert.ToString(dr["PII_UnitPrice"]),
                        PII_Amount = Convert.ToString(dr["PII_Amount"]),
                        PIH_EXP_Expense = Convert.ToString(dr["PIH_EXP_Expense"]),
                        PIH_ItemMiscExpense = Convert.ToString(dr["PII_ItemMiscExpense"]),
                        PIH_HeaderMiscExpense = Convert.ToString(dr["PIH_HeaderMiscExpense"]),
                        PII_HSN_Number = Convert.ToString(dr["PII_HSN_Number"]),
                        PII_GST_Amount = Convert.ToString(dr["PII_GST_Amount"]),
                        PII_WHT_Amount = Convert.ToString(dr["PII_WHT_Amount"]),
                        PII_WHT_Code = Convert.ToString(dr["PIH_WHT_Number"]),
                        PII_WHT_Percent = Convert.ToString(dr["PII_WHT_Percent"])
                    });
            }
            return POList;
        }
        public List<PurchaseInvoiceHead_DTO> PIHeadList(DataTable Dt)
        {
            List<PurchaseInvoiceHead_DTO> PIList = new List<PurchaseInvoiceHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoiceHead_DTO
                    {
                        PIH_Number = Convert.ToInt64(dr["PIH_Number"]),
                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PIH_DueDate = Convert.ToString(dr["PIH_DueDate"]),
                        PIH_ImportOrder = Convert.ToString(dr["PIH_ImportOrder"]) == "1" ? "true" : "false",
                        PIH_Vendor_Number = Convert.ToString(dr["PIH_Vendor_Number"]),
                        PIH_Vendor = Convert.ToString(dr["PIH_Vendor"]),
                        PIH_VendorLocation = Convert.ToString(dr["PIH_VendorLocation"]),
                        PIH_Currency_Number = Convert.ToString(dr["PIH_Currency_Number"]),
                        PIH_TaxCluster = Convert.ToString(dr["PIH_TaxCluster"]),
                        PIH_TaxCluster_Number = Convert.ToString(dr["PIH_TaxCluster_Number"]),
                        PIH_TaxClusterNumber = Convert.ToString(dr["PIH_TaxCluster_Number"]),
                        PIH_WHT_Number = Convert.ToString(dr["PIH_WHT_Number"]),
                        //PIH_WHT_Percent = Convert.ToString(dr["PIH_WHT_Percent"]),
                        //PIH_WHT_Tax = Convert.ToString(dr["PIH_WHT_Tax"]),
                        PIH_MS_Number = Convert.ToString(dr["PIH_MS_Number"]),
                        PIH_Currency = Convert.ToString(dr["PIH_Currency"]),
                        PIH_DecimalPlaces = Convert.ToString(dr["PIH_DecimalPlaces"]),
                        PIH_ExchangeRate = Convert.ToString(dr["PIH_ExchangeRate"]),

                        PIH_MaterialCost = Convert.ToString(dr["PIH_MaterialCost"]),
                        PIH_ItemMiscExpense = Convert.ToString(dr["PIH_ItemMiscExpense"]),
                        PIH_HeaderMiscExpense = Convert.ToString(dr["PIH_HeaderMiscExpense"]),
                        PIH_GST_Amount = Convert.ToString(dr["PIH_GST_Amount"]),
                        PIH_InvoiceAmount = Convert.ToString(dr["PIH_InvoiceAmount"]),
                        PIH_WHT_Amount = Convert.ToString(dr["PIH_WHT_Amount"]),
                        PIH_RoundOff = Convert.ToString(dr["PIH_RoundOff"]),
                        PIH_VendorPayable = Convert.ToString(dr["PIH_VendorPayable"]),

                        PIH_Mode = Convert.ToInt32(dr["PIH_Mode"]),

                        PIH_WHT_Code = Convert.ToString(dr["PIH_WHT"]),
                    });
            }
            return PIList;
        }
        public List<PurchaseInvoiceItem_DTO> PIItemList(DataTable Dt)
        {
            List<PurchaseInvoiceItem_DTO> PIList = new List<PurchaseInvoiceItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoiceItem_DTO
                    {
                        PII_Number = Convert.ToInt64(dr["PII_Number"]),
                        PII_POH_OrderNo = Convert.ToString(dr["PII_POH_OrderNo"]),
                        PII_ItemCode = Convert.ToString(dr["PII_ItemCode"]),
                        PII_Item_Number = Convert.ToString(dr["PII_Item_Number"]),
                        PII_ItemGroup = Convert.ToString(dr["PII_ItemGroup"]),
                        PII_Thickness = Convert.ToString(dr["Thickness"]),
                        PII_Width = Convert.ToString(dr["ITM_Width"]),
                        PII_MaterialGrade = Convert.ToString(dr["MaterialGrade"]),
                        PII_Description = Convert.ToString(dr["PII_Description"]),
                        PII_Warehouse_Number = Convert.ToString(dr["PII_Warehouse_Number"]),
                        PII_MS_Number = Convert.ToInt64(dr["PII_MS_Number"]),
                        PII_UoM_Number = Convert.ToString(dr["PII_UoM_Number"]),
                        PII_UoM = Convert.ToString(dr["PII_UoM"]),
                        PII_DecimalPlaces = Convert.ToString(dr["PII_DecimalPlaces"]),
                        PII_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["PII_Qty"]))),
                        PII_UnitPrice = Convert.ToString(MH.DecimalConvertUnit(Convert.ToDouble(dr["PII_UnitPrice"]))),
                        PII_Amount = Convert.ToString(dr["PII_Amount"]),
                        PII_ExpenseValue = Convert.ToString(dr["PII_ExpenseValue"]),
                        PII_HSN_Number = Convert.ToString(dr["PII_HSN_Number"]),
                        PII_HSN = Convert.ToString(dr["PII_HSN"]),
                        PII_GST_Amount = Convert.ToString(dr["PII_GST_Amount"]),
                        PII_WHT_Percent = Convert.ToString(dr["PII_WHT_Percent"]),
                        PII_WHT_Amount = Convert.ToString(dr["PII_WHT_Amount"]),
                        IsDeleted = "false"
                    });
            }
            return PIList;
        }
        public List<PurchaseInvoiceExpense_DTO> PIExpenseEditList(DataTable Dt)
        {
            List<PurchaseInvoiceExpense_DTO> PIList = new List<PurchaseInvoiceExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoiceExpense_DTO
                    {
                        PIH_EXP_Number = Convert.ToInt64(dr["PIH_EXP_Number"]),
                        PIH_EXP_POH_OrderNo = Convert.ToString(dr["PIH_EXP_POH_OrderNo"]),
                        PIH_EXP_Expense_Number = Convert.ToString(dr["PIH_EXP_Expense_Number"]),
                        PIH_EXP_Description = Convert.ToString(dr["PIH_EXP_Description"]),
                        PIH_EXP_Remarks = Convert.ToString(dr["PIH_EXP_Remarks"]),
                        PIH_EXP_Occurrence_Number = Convert.ToString(dr["PIH_EXP_Occurrence_Number"]),
                        PIH_EXP_CM_Number = Convert.ToString(dr["PIH_EXP_CM_Number"]),
                        PIH_EXP_ExpenseBase = Convert.ToString(dr["PIH_EXP_ExpenseBase"]),
                        PIH_EXP_ExpenseValue = Convert.ToString(dr["PIH_EXP_ExpenseValue"]),
                        PIH_EXP_Allocate_Number = Convert.ToString(dr["PIH_EXP_Allocate_Number"]),
                        PIH_EXP_LA_Number = Convert.ToString(dr["PIH_EXP_LA_Number"]),
                        PIH_EXP_SAC_Number = Convert.ToString(dr["PIH_EXP_SAC_Number"]),
                        PIH_EXP_TaxCalculate = Convert.ToString(dr["PIH_EXP_TaxCalculate"]),
                        PIH_EXP_TaxValue = Convert.ToString(dr["PIH_EXP_TaxValue"]),
                        PIH_EXP_IsDeleted = "false"
                    }); ;
            }
            return PIList;
        }
        public List<PurchaseInvoiceIExpense_DTO> PIIExpenseEditList(DataTable Dt)
        {
            List<PurchaseInvoiceIExpense_DTO> PIList = new List<PurchaseInvoiceIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoiceIExpense_DTO
                    {
                        PII_EXP_Number = Convert.ToInt64(dr["PII_EXP_Number"]),
                        PII_EXP_PII_Number = Convert.ToInt64(dr["PII_EXP_PII_Number"]),
                        PII_EXP_Expense_Number = Convert.ToString(dr["PII_EXP_Expense_Number"]),
                        PII_EXP_Description = Convert.ToString(dr["PII_EXP_Description"]),
                        PII_EXP_Remarks = Convert.ToString(dr["PII_EXP_Remarks"]),
                        PII_EXP_Occurrence_Number = Convert.ToString(dr["PII_EXP_Occurrence_Number"]),
                        PII_EXP_CM_Number = Convert.ToString(dr["PII_EXP_CM_Number"]),
                        PII_EXP_ExpenseBase = Convert.ToString(dr["PII_EXP_ExpenseBase"]),
                        PII_EXP_ExpenseValue = Convert.ToString(dr["PII_EXP_ExpenseValue"]),
                        PII_EXP_Allocate_Number = Convert.ToString(dr["PII_EXP_Allocate_Number"]),
                        PII_EXP_LA_Number = Convert.ToString(dr["PII_EXP_LA_Number"]),
                        PII_EXP_IsDeleted = "false"
                    });
            }
            return PIList;
        }
        public List<PurchaseInvoiceBatch_DTO> PIIBatchEditList(DataTable Dt)
        {
            List<PurchaseInvoiceBatch_DTO> PIList = new List<PurchaseInvoiceBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoiceBatch_DTO
                    {
                        PII_BCH_Number = Convert.ToInt64(dr["PII_BCH_Number"]),
                        PII_BCH_PII_Number = Convert.ToInt64(dr["PII_BCH_PII_Number"]),
                        PII_BCH_Date = Convert.ToString(dr["PII_BCH_Date"]),
                        PII_BCH_No = Convert.ToString(dr["PII_BCH_No"]),
                        PII_BCH_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["PII_BCH_Qty"]))),
                        PII_BCH_UnitPrice = Convert.ToString(MH.DecimalConvertUnit(Convert.ToDouble(dr["PII_BCH_UnitPrice"]))),
                        PII_BCH_Value = Convert.ToString(dr["PII_BCH_Value"]),
                        PII_BCH_IsDeleted = "false"
                    });
            }
            return PIList;
        }







        //PO TO ITEM INVOICE
        public List<TaxCluster_DTO> POtoPICluster(DataTable Dt)
        {
            List<TaxCluster_DTO> IList = new List<TaxCluster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new TaxCluster_DTO
                    {
                        TaxClusterNumber = Convert.ToInt64(dr["TaxClusterNumber"]),
                        TaxCluster = Convert.ToString(dr["TaxCluster"]),
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceGst> POtoPIGstView(DataTable Dt)
        {
            List<PurchaseInvoiceGst> IList = new List<PurchaseInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceGst
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        TaxCategory = Convert.ToString(dr["TaxCategory"]),
                        TaxType = Convert.ToString(dr["TaxType"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        TaxElementName = Convert.ToString(dr["TaxElementName"]),
                        LoadonInventory = Convert.ToString(dr["LoadonInventory"]),
                        LoadonInventoryPercent = Convert.ToString(dr["LoadonInventoryPercent"]),
                        Chargeable = Convert.ToString(dr["Chargeable"]),
                        Calculation = Convert.ToInt64(dr["Calculation"]),
                        Percentage = Convert.ToDouble(dr["Percentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceWHT> POtoPIWHT(DataTable Dt)
        {
            List<PurchaseInvoiceWHT> IList = new List<PurchaseInvoiceWHT>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceWHT
                    {
                        TaxNumber = Convert.ToInt64(dr["TaxNumber"]),
                        TaxCode = Convert.ToString(dr["TaxCode"]),
                        Tax = Convert.ToInt64(dr["IncludeTax"]),
                        Percentage = Convert.ToDouble(dr["TaxPercentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceGst> POtoPIGst(DataTable Dt)
        {
            List<PurchaseInvoiceGst> IList = new List<PurchaseInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceGst
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        Chargeable = Convert.ToString(dr["Chargeable"]),
                        Calculation = Convert.ToInt64(dr["Calculation"]),
                        Percentage = Convert.ToDouble(dr["Percentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceExpense_DTO> POtoPIList(DataTable Dt)
        {
            List<PurchaseInvoiceExpense_DTO> IList = new List<PurchaseInvoiceExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceExpense_DTO
                    {
                        PIH_EXP_Description = Convert.ToString(dr["EC_Description"]),
                        PIH_EXP_LA_Number = Convert.ToString(dr["LedgerAccount"])
                    });
            }
            return IList;
        }
        public List<PurchaseOrder_DTO> POToPIOrder(DataTable Dt)
        {
            List<PurchaseOrder_DTO> POList = new List<PurchaseOrder_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrder_DTO
                    {
                        POH_Number = Convert.ToInt64(dr["POH_Number"]),
                        POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_Date = Convert.ToString(dr["POH_Date"]),
                        POH_MaterialValue = Convert.ToString(dr["POH_MaterialValue"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrderPIItem_DTO> POToPIOrderItem(DataTable Dt)
        {
            List<PurchaseOrderPIItem_DTO> POList = new List<PurchaseOrderPIItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderPIItem_DTO
                    {
                        POI_Number = Convert.ToInt64(dr["POI_Number"]),
                        POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_Number = Convert.ToInt64(dr["POH_Number"]),
                        POI_MS_Number = Convert.ToInt64(dr["POH_MS_Number"]),
                        POI_Item_Number = Convert.ToString(dr["POI_Item_Number"]),
                        POI_UoM_Number = Convert.ToString(dr["POI_UoM_Number"]),
                        POI_Warehouse_Number = Convert.ToInt64(dr["POI_Warehouse_Number"]),
                        POI_DecimalPlaces = Convert.ToString(dr["POI_DecimalPlaces"]),
                        POI_Qty = DecimalConvertQty(Convert.ToDouble(dr["POI_Qty"])),
                        POI_HSN = Convert.ToString(dr["POI_HSN"]),
                        POI_UnitPrice = DecimalConvertUnit(Convert.ToDouble(dr["POI_UnitPrice"])),
                        POI_Amount = Convert.ToString(dr["POI_Amount"]),
                        POI_ItemGroup = Convert.ToString(dr["POI_Item_Group"]),
                        POI_ItemCode = Convert.ToString(dr["POI_Item_Code"]),
                        POI_ItemDescription = Convert.ToString(dr["POI_Item_Description"])
                    });
            }
            return POList;
        }
        public List<PurchaseOrderPIExpense_DTO> POToPIExpense(DataTable Dt)
        {
            List<PurchaseOrderPIExpense_DTO> POList = new List<PurchaseOrderPIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderPIExpense_DTO
                    {
                        POH_EXP_POH_Number = Convert.ToInt64(dr["POH_EXP_POH_Number"]),
                        POH_EXP_Number = Convert.ToInt64(dr["POH_EXP_Number"]),
                        POH_EXP_POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_EXP_Expense_Number = Convert.ToString(dr["POH_EXP_Expense_Number"]),
                        POH_EXP_Description = Convert.ToString(dr["POH_EXP_Description"]),
                        POH_EXP_Remarks = Convert.ToString(dr["POH_EXP_Remarks"]),
                        POH_EXP_Occurrence_Number = Convert.ToString(dr["POH_EXP_Occurrence_Number"]),
                        POH_EXP_CM_Number = Convert.ToString(dr["POH_EXP_CM_Number"]),
                        POH_EXP_ExpenseBase = Convert.ToString(dr["POH_EXP_ExpenseBase"]),
                        POH_EXP_ExpenseValue = Convert.ToString(dr["POH_EXP_ExpenseValue"]),
                        POH_EXP_Allocate_Number = Convert.ToString(dr["POH_EXP_Allocate_Number"]),
                        POH_EXP_LA_Number = Convert.ToString(dr["POH_EXP_LA_Number"]),
                        POH_EXP_SAC_Number = Convert.ToString(dr["POH_EXP_SAC_Number"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrderPIIExpense_DTO> POToPIIExpense(DataTable Dt)
        {
            List<PurchaseOrderPIIExpense_DTO> POList = new List<PurchaseOrderPIIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderPIIExpense_DTO
                    {
                        POI_EXP_Number = Convert.ToInt64(dr["POI_EXP_Number"]),
                        POI_EXP_POI_Number = Convert.ToInt64(dr["POI_EXP_POI_Number"]),
                        POI_EXP_POH_Number = Convert.ToInt64(dr["POI_EXP_POH_Number"]),
                        POI_EXP_Expense_Number = Convert.ToString(dr["POI_EXP_Expense_Number"]),
                        POI_EXP_Description = Convert.ToString(dr["POI_EXP_Description"]),
                        POI_EXP_Remarks = Convert.ToString(dr["POI_EXP_Remarks"]),
                        POI_EXP_Occurrence_Number = Convert.ToString(dr["POI_EXP_Occurrence_Number"]),
                        POI_EXP_CM_Number = Convert.ToString(dr["POI_EXP_CM_Number"]),
                        POI_EXP_ExpenseBase = Convert.ToString(dr["POI_EXP_ExpenseBase"]),
                        POI_EXP_ExpenseValue = Convert.ToString(dr["POI_EXP_ExpenseValue"]),
                        POI_EXP_Allocate_Number = Convert.ToString(dr["POI_EXP_Allocate_Number"]),
                        POI_EXP_LA_Number = Convert.ToString(dr["POI_EXP_LA_Number"]),
                    });
            }
            return POList;
        }

        public List<POToPurchaseInvoiceHead_DTO> POToPIHeadList(DataTable Dt)
        {
            List<POToPurchaseInvoiceHead_DTO> PIList = new List<POToPurchaseInvoiceHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POToPurchaseInvoiceHead_DTO
                    {
                        PIH_Number = Convert.ToInt64(dr["PIH_Number"]),
                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PIH_DueDate = Convert.ToString(dr["PIH_DueDate"]),
                        PIH_ImportOrder = Convert.ToString(dr["PIH_ImportOrder"]) == "1" ? "true" : "false",
                        PIH_Vendor_Number = Convert.ToString(dr["PIH_Vendor_Number"]),
                        PIH_Vendor = Convert.ToString(dr["PIH_Vendor"]),
                        PIH_VendorLocation = Convert.ToString(dr["PIH_VendorLocation"]),
                        PIH_Currency_Number = Convert.ToString(dr["PIH_Currency_Number"]),
                        PIH_TaxCluster = Convert.ToString(dr["PIH_TaxCluster"]),
                        PIH_TaxCluster_Number = Convert.ToString(dr["PIH_TaxCluster_Number"]),
                        PIH_TaxClusterNumber = Convert.ToString(dr["PIH_TaxCluster_Number"]),
                        PIH_WHT_Number = Convert.ToString(dr["PIH_WHT_Number"]),
                        //PIH_WHT_Percent = Convert.ToString(dr["PIH_WHT_Percent"]),
                        //PIH_WHT_Tax = Convert.ToString(dr["PIH_WHT_Tax"]),
                        PIH_MS_Number = Convert.ToString(dr["PIH_MS_Number"]),
                        PIH_Currency = Convert.ToString(dr["PIH_Currency"]),
                        PIH_DecimalPlaces = Convert.ToString(dr["PIH_DecimalPlaces"]),
                        PIH_ExchangeRate = Convert.ToString(dr["PIH_ExchangeRate"]),

                        PIH_MaterialCost = Convert.ToString(dr["PIH_MaterialCost"]),
                        PIH_ItemMiscExpense = Convert.ToString(dr["PIH_ItemMiscExpense"]),
                        PIH_HeaderMiscExpense = Convert.ToString(dr["PIH_HeaderMiscExpense"]),
                        PIH_GST_Amount = Convert.ToString(dr["PIH_GST_Amount"]),
                        PIH_InvoiceAmount = Convert.ToString(dr["PIH_InvoiceAmount"]),
                        PIH_WHT_Amount = Convert.ToString(dr["PIH_WHT_Amount"]),
                        PIH_RoundOff = Convert.ToString(dr["PIH_RoundOff"]),
                        PIH_VendorPayable = Convert.ToString(dr["PIH_VendorPayable"]),

                        PIH_Mode = Convert.ToInt32(dr["PIH_Mode"]),

                        PIH_WHT_Code = Convert.ToString(dr["PIH_WHT"]),
                    });
            }
            return PIList;
        }
        public List<POToPurchaseInvoiceItem_DTO> POToPIItemList(DataTable Dt)
        {
            List<POToPurchaseInvoiceItem_DTO> PIList = new List<POToPurchaseInvoiceItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POToPurchaseInvoiceItem_DTO
                    {
                        PII_Number = Convert.ToInt64(dr["PII_Number"]),
                        PII_POI_Number = Convert.ToInt64(dr["PII_POI_Number"]),
                        PII_POH_Number = Convert.ToInt64(dr["PII_POH_Number"]),
                        PII_POH_OrderNo = Convert.ToString(dr["PII_POH_OrderNo"]),
                        PII_ItemCode = Convert.ToString(dr["PII_ItemCode"]),
                        PII_Item_Number = Convert.ToString(dr["PII_Item_Number"]),
                        PII_ItemGroup = Convert.ToString(dr["PII_ItemGroup"]),
                        PII_Description = Convert.ToString(dr["PII_Description"]),
                        PII_Warehouse_Number = Convert.ToString(dr["PII_Warehouse_Number"]),
                        PII_MS_Number = Convert.ToInt64(dr["PII_MS_Number"]),
                        PII_UoM_Number = Convert.ToString(dr["PII_UoM_Number"]),
                        PII_UoM = Convert.ToString(dr["PII_UoM"]),
                        PII_DecimalPlaces = Convert.ToString(dr["PII_DecimalPlaces"]),
                        PII_POQty = Convert.ToString(dr["POI_Qty"]),
                        PII_Qty = Convert.ToString(dr["PII_Qty"]),
                        PII_UnitPrice = Convert.ToString(dr["PII_UnitPrice"]),
                        PII_Amount = Convert.ToString(dr["PII_Amount"]),
                        PII_ExpenseValue = Convert.ToString(dr["PII_ExpenseValue"]),
                        PII_HSN_Number = Convert.ToString(dr["PII_HSN_Number"]),
                        PII_HSN = Convert.ToString(dr["PII_HSN"]),
                        PII_GST_Amount = Convert.ToString(dr["PII_GST_Amount"]),
                        PII_WHT_Percent = Convert.ToString(dr["PII_WHT_Percent"]),
                        PII_WHT_Amount = Convert.ToString(dr["PII_WHT_Amount"]),
                        PII_IsDeleted = "false"
                    });
            }
            return PIList;
        }
        public List<POToPurchaseInvoiceExpense_DTO> POToPIExpenseEditList(DataTable Dt)
        {
            List<POToPurchaseInvoiceExpense_DTO> PIList = new List<POToPurchaseInvoiceExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POToPurchaseInvoiceExpense_DTO
                    {
                        PIH_EXP_Number = Convert.ToInt64(dr["PIH_EXP_Number"]),
                        PIH_EXP_POH_EXP_Number = Convert.ToInt64(dr["PIH_EXP_POH_EXP_Number"]),
                        PIH_EXP_POH_Number = Convert.ToInt64(dr["PIH_EXP_POH_Number"]),
                        PIH_EXP_POH_OrderNo = Convert.ToString(dr["PIH_EXP_POH_OrderNo"]),
                        PIH_EXP_Expense_Number = Convert.ToString(dr["PIH_EXP_Expense_Number"]),
                        PIH_EXP_Description = Convert.ToString(dr["PIH_EXP_Description"]),
                        PIH_EXP_Remarks = Convert.ToString(dr["PIH_EXP_Remarks"]),
                        PIH_EXP_Occurrence_Number = Convert.ToString(dr["PIH_EXP_Occurrence_Number"]),
                        PIH_EXP_CM_Number = Convert.ToString(dr["PIH_EXP_CM_Number"]),
                        PIH_EXP_ExpenseBase = Convert.ToString(dr["PIH_EXP_ExpenseBase"]),
                        PIH_EXP_ExpenseValue = Convert.ToString(dr["PIH_EXP_ExpenseValue"]),
                        PIH_EXP_Allocate_Number = Convert.ToString(dr["PIH_EXP_Allocate_Number"]),
                        PIH_EXP_LA_Number = Convert.ToString(dr["PIH_EXP_LA_Number"]),
                        PIH_EXP_SAC_Number = Convert.ToString(dr["PIH_EXP_SAC_Number"]),
                        PIH_EXP_TaxCalculate = Convert.ToString(dr["PIH_EXP_TaxCalculate"]),
                        PIH_EXP_TaxValue = Convert.ToString(dr["PIH_EXP_TaxValue"]),
                        PIH_EXP_IsDeleted = "false"
                    }); ;
            }
            return PIList;
        }
        public List<POToPurchaseInvoiceIExpense_DTO> POToPIIExpenseEditList(DataTable Dt)
        {
            List<POToPurchaseInvoiceIExpense_DTO> PIList = new List<POToPurchaseInvoiceIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POToPurchaseInvoiceIExpense_DTO
                    {
                        PII_EXP_Number = Convert.ToInt64(dr["PII_EXP_Number"]),
                        PII_EXP_POI_EXP_Number = Convert.ToInt64(dr["PII_EXP_POI_EXP_Number"]),
                        PII_EXP_PII_Number = Convert.ToInt64(dr["PII_EXP_PII_Number"]),

                        PII_EXP_POI_Number = Convert.ToInt64(dr["PII_EXP_POI_Number"]),
                        PII_EXP_POH_Number = Convert.ToInt64(dr["PII_EXP_POH_Number"]),

                        PII_EXP_Expense_Number = Convert.ToString(dr["PII_EXP_Expense_Number"]),
                        PII_EXP_Description = Convert.ToString(dr["PII_EXP_Description"]),
                        PII_EXP_Remarks = Convert.ToString(dr["PII_EXP_Remarks"]),
                        PII_EXP_Occurrence_Number = Convert.ToString(dr["PII_EXP_Occurrence_Number"]),
                        PII_EXP_CM_Number = Convert.ToString(dr["PII_EXP_CM_Number"]),
                        PII_EXP_ExpenseBase = Convert.ToString(dr["PII_EXP_ExpenseBase"]),
                        PII_EXP_ExpenseValue = Convert.ToString(dr["PII_EXP_ExpenseValue"]),
                        PII_EXP_Allocate_Number = Convert.ToString(dr["PII_EXP_Allocate_Number"]),
                        PII_EXP_LA_Number = Convert.ToString(dr["PII_EXP_LA_Number"]),
                        PII_EXP_IsDeleted = "false"
                    });
            }
            return PIList;
        }
        public List<POToPurchaseInvoiceBatch_DTO> POToPIIBatchEditList(DataTable Dt)
        {
            List<POToPurchaseInvoiceBatch_DTO> PIList = new List<POToPurchaseInvoiceBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POToPurchaseInvoiceBatch_DTO
                    {
                        PII_BCH_Number = Convert.ToInt64(dr["PII_BCH_Number"]),
                        PII_BCH_POH_Number = Convert.ToInt64(dr["PII_POH_Number"]),
                        PII_BCH_POI_Number = Convert.ToInt64(dr["PII_POI_Number"]),
                        PII_BCH_PII_Number = Convert.ToInt64(dr["PII_BCH_PII_Number"]),
                        PII_BCH_Date = Convert.ToString(dr["PII_BCH_Date"]),
                        PII_BCH_No = Convert.ToString(dr["PII_BCH_No"]),
                        PII_BCH_Qty = Convert.ToString(dr["PII_BCH_Qty"]),
                        PII_BCH_UnitPrice = Convert.ToString(dr["PII_BCH_UnitPrice"]),
                        PII_BCH_Value = Convert.ToString(dr["PII_BCH_Value"]),
                        PII_BCH_IsDeleted = "false"
                    });
            }
            return PIList;
        }






        //PO ITEM TO ITEM INVOICE
        public List<TaxCluster_DTO> POItemTOPICluster(DataTable Dt)
        {
            List<TaxCluster_DTO> IList = new List<TaxCluster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new TaxCluster_DTO
                    {
                        TaxClusterNumber = Convert.ToInt64(dr["TaxClusterNumber"]),
                        TaxCluster = Convert.ToString(dr["TaxCluster"]),
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceGst> POItemTOPIGstView(DataTable Dt)
        {
            List<PurchaseInvoiceGst> IList = new List<PurchaseInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceGst
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        TaxCategory = Convert.ToString(dr["TaxCategory"]),
                        TaxType = Convert.ToString(dr["TaxType"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        TaxElementName = Convert.ToString(dr["TaxElementName"]),
                        LoadonInventory = Convert.ToString(dr["LoadonInventory"]),
                        LoadonInventoryPercent = Convert.ToString(dr["LoadonInventoryPercent"]),
                        Chargeable = Convert.ToString(dr["Chargeable"]),
                        Calculation = Convert.ToInt64(dr["Calculation"]),
                        Percentage = Convert.ToDouble(dr["Percentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceWHT> POItemTOPIWHT(DataTable Dt)
        {
            List<PurchaseInvoiceWHT> IList = new List<PurchaseInvoiceWHT>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceWHT
                    {
                        TaxNumber = Convert.ToInt64(dr["TaxNumber"]),
                        TaxCode = Convert.ToString(dr["TaxCode"]),
                        Tax = Convert.ToInt64(dr["IncludeTax"]),
                        Percentage = Convert.ToDouble(dr["TaxPercentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceGst> POItemTOPIGst(DataTable Dt)
        {
            List<PurchaseInvoiceGst> IList = new List<PurchaseInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceGst
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        Chargeable = Convert.ToString(dr["Chargeable"]),
                        Calculation = Convert.ToInt64(dr["Calculation"]),
                        Percentage = Convert.ToDouble(dr["Percentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseInvoiceExpense_DTO> POItemTOPIList(DataTable Dt)
        {
            List<PurchaseInvoiceExpense_DTO> IList = new List<PurchaseInvoiceExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseInvoiceExpense_DTO
                    {
                        PIH_EXP_Description = Convert.ToString(dr["EC_Description"]),
                        PIH_EXP_LA_Number = Convert.ToString(dr["LedgerAccount"])
                    });
            }
            return IList;
        }
        public List<PurchaseOrderPIHItem_DTO> POItemTOPIOrder(DataTable Dt)
        {
            List<PurchaseOrderPIHItem_DTO> POList = new List<PurchaseOrderPIHItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderPIHItem_DTO
                    {
                        POI_Item_Number = Convert.ToString(dr["ItemNumber"]),
                        POI_ItemCode = Convert.ToString(dr["ItemCode"]),
                        POI_ItemDescription = Convert.ToString(dr["ItemDescription"]),
                        POI_ItemGroup = Convert.ToString(dr["ItemGroup"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrderPIHItem_DTO> POItemTOPIOrderItem(DataTable Dt)
        {
            List<PurchaseOrderPIHItem_DTO> POList = new List<PurchaseOrderPIHItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderPIHItem_DTO
                    {
                        POI_Number = Convert.ToInt64(dr["POI_Number"]),
                        POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_Number = Convert.ToInt64(dr["POH_Number"]),
                        POH_PODate = Convert.ToString(dr["POH_PODate"]),
                        POI_MS_Number = Convert.ToInt64(dr["POH_MS_Number"]),
                        POI_Item_Number = Convert.ToString(dr["POI_Item_Number"]),
                        POI_UoM_Number = Convert.ToString(dr["POI_UoM_Number"]),
                        POI_Warehouse_Number = Convert.ToInt64(dr["POI_Warehouse_Number"]),
                        POI_DecimalPlaces = Convert.ToString(dr["POI_DecimalPlaces"]),
                        POI_Qty = DecimalConvertQty(Convert.ToDouble(dr["POI_Qty"])),
                        POI_HSN = Convert.ToString(dr["POI_HSN"]),
                        POI_UnitPrice = DecimalConvertUnit(Convert.ToDouble(dr["POI_UnitPrice"])),
                        POI_Amount = Convert.ToString(dr["POI_Amount"]),
                        POI_ItemGroup = Convert.ToString(dr["POI_Item_Group"]),
                        POI_ItemCode = Convert.ToString(dr["POI_Item_Code"]),
                        POI_ItemDescription = Convert.ToString(dr["POI_Item_Description"])
                    });
            }
            return POList;
        }
        public List<PurchaseOrderPIHExpense_DTO> POItemTOPIExpense(DataTable Dt)
        {
            List<PurchaseOrderPIHExpense_DTO> POList = new List<PurchaseOrderPIHExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderPIHExpense_DTO
                    {
                        POH_EXP_POH_Number = Convert.ToInt64(dr["POH_EXP_POH_Number"]),
                        POH_EXP_Number = Convert.ToInt64(dr["POH_EXP_Number"]),
                        POH_EXP_POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        POH_EXP_Expense_Number = Convert.ToString(dr["POH_EXP_Expense_Number"]),
                        POH_EXP_Description = Convert.ToString(dr["POH_EXP_Description"]),
                        POH_EXP_Remarks = Convert.ToString(dr["POH_EXP_Remarks"]),
                        POH_EXP_Occurrence_Number = Convert.ToString(dr["POH_EXP_Occurrence_Number"]),
                        POH_EXP_CM_Number = Convert.ToString(dr["POH_EXP_CM_Number"]),
                        POH_EXP_ExpenseBase = Convert.ToString(dr["POH_EXP_ExpenseBase"]),
                        POH_EXP_ExpenseValue = Convert.ToString(dr["POH_EXP_ExpenseValue"]),
                        POH_EXP_Allocate_Number = Convert.ToString(dr["POH_EXP_Allocate_Number"]),
                        POH_EXP_LA_Number = Convert.ToString(dr["POH_EXP_LA_Number"]),
                        POH_EXP_SAC_Number = Convert.ToString(dr["POH_EXP_SAC_Number"]),
                    });
            }
            return POList;
        }
        public List<PurchaseOrderPIHIExpense_DTO> POItemTOPIIExpense(DataTable Dt)
        {
            List<PurchaseOrderPIHIExpense_DTO> POList = new List<PurchaseOrderPIHIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseOrderPIHIExpense_DTO
                    {
                        POI_EXP_Number = Convert.ToInt64(dr["POI_EXP_Number"]),
                        POI_EXP_POI_Number = Convert.ToInt64(dr["POI_EXP_POI_Number"]),
                        POI_EXP_POH_Number = Convert.ToInt64(dr["POI_EXP_POH_Number"]),
                        POI_EXP_Expense_Number = Convert.ToString(dr["POI_EXP_Expense_Number"]),
                        POI_EXP_Description = Convert.ToString(dr["POI_EXP_Description"]),
                        POI_EXP_Remarks = Convert.ToString(dr["POI_EXP_Remarks"]),
                        POI_EXP_Occurrence_Number = Convert.ToString(dr["POI_EXP_Occurrence_Number"]),
                        POI_EXP_CM_Number = Convert.ToString(dr["POI_EXP_CM_Number"]),
                        POI_EXP_ExpenseBase = Convert.ToString(dr["POI_EXP_ExpenseBase"]),
                        POI_EXP_ExpenseValue = Convert.ToString(dr["POI_EXP_ExpenseValue"]),
                        POI_EXP_Allocate_Number = Convert.ToString(dr["POI_EXP_Allocate_Number"]),
                        POI_EXP_LA_Number = Convert.ToString(dr["POI_EXP_LA_Number"]),
                    });
            }
            return POList;
        }


        public List<POItemTOPurchaseInvoiceHead_DTO> POItemTOPIHeadList(DataTable Dt)
        {
            List<POItemTOPurchaseInvoiceHead_DTO> PIList = new List<POItemTOPurchaseInvoiceHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POItemTOPurchaseInvoiceHead_DTO
                    {
                        PIH_Number = Convert.ToInt64(dr["PIH_Number"]),
                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PIH_DueDate = Convert.ToString(dr["PIH_DueDate"]),
                        PIH_ImportOrder = Convert.ToString(dr["PIH_ImportOrder"]) == "1" ? "true" : "false",
                        PIH_Vendor_Number = Convert.ToString(dr["PIH_Vendor_Number"]),
                        PIH_Vendor = Convert.ToString(dr["PIH_Vendor"]),
                        PIH_VendorLocation = Convert.ToString(dr["PIH_VendorLocation"]),
                        PIH_Currency_Number = Convert.ToString(dr["PIH_Currency_Number"]),
                        PIH_TaxCluster = Convert.ToString(dr["PIH_TaxCluster"]),
                        PIH_TaxCluster_Number = Convert.ToString(dr["PIH_TaxCluster_Number"]),
                        PIH_TaxClusterNumber = Convert.ToString(dr["PIH_TaxCluster_Number"]),
                        PIH_WHT_Number = Convert.ToString(dr["PIH_WHT_Number"]),
                        //PIH_WHT_Percent = Convert.ToString(dr["PIH_WHT_Percent"]),
                        //PIH_WHT_Tax = Convert.ToString(dr["PIH_WHT_Tax"]),
                        PIH_MS_Number = Convert.ToString(dr["PIH_MS_Number"]),
                        PIH_Currency = Convert.ToString(dr["PIH_Currency"]),
                        PIH_DecimalPlaces = Convert.ToString(dr["PIH_DecimalPlaces"]),
                        PIH_ExchangeRate = Convert.ToString(dr["PIH_ExchangeRate"]),

                        PIH_MaterialCost = Convert.ToString(dr["PIH_MaterialCost"]),
                        PIH_ItemMiscExpense = Convert.ToString(dr["PIH_ItemMiscExpense"]),
                        PIH_HeaderMiscExpense = Convert.ToString(dr["PIH_HeaderMiscExpense"]),
                        PIH_GST_Amount = Convert.ToString(dr["PIH_GST_Amount"]),
                        PIH_InvoiceAmount = Convert.ToString(dr["PIH_InvoiceAmount"]),
                        PIH_WHT_Amount = Convert.ToString(dr["PIH_WHT_Amount"]),
                        PIH_RoundOff = Convert.ToString(dr["PIH_RoundOff"]),
                        PIH_VendorPayable = Convert.ToString(dr["PIH_VendorPayable"]),

                        PIH_Mode = Convert.ToInt32(dr["PIH_Mode"]),

                        PIH_WHT_Code = Convert.ToString(dr["PIH_WHT"]),
                    });
            }
            return PIList;
        }
        public List<POItemTOPurchaseInvoiceItem_DTO> POItemTOPIItemList(DataTable Dt)
        {
            List<POItemTOPurchaseInvoiceItem_DTO> PIList = new List<POItemTOPurchaseInvoiceItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POItemTOPurchaseInvoiceItem_DTO
                    {
                        PII_Number = Convert.ToInt64(dr["PII_Number"]),
                        PII_POI_Number = Convert.ToInt64(dr["PII_POI_Number"]),
                        PII_POH_Number = Convert.ToInt64(dr["PII_POH_Number"]),
                        PII_POH_OrderNo = Convert.ToString(dr["PII_POH_OrderNo"]),
                        PII_ItemCode = Convert.ToString(dr["PII_ItemCode"]),
                        PII_Item_Number = Convert.ToString(dr["PII_Item_Number"]),
                        PII_ItemGroup = Convert.ToString(dr["PII_ItemGroup"]),
                        PII_Description = Convert.ToString(dr["PII_Description"]),
                        PII_Warehouse_Number = Convert.ToString(dr["PII_Warehouse_Number"]),
                        PII_MS_Number = Convert.ToInt64(dr["PII_MS_Number"]),
                        PII_UoM_Number = Convert.ToString(dr["PII_UoM_Number"]),
                        PII_UoM = Convert.ToString(dr["PII_UoM"]),
                        PII_DecimalPlaces = Convert.ToString(dr["PII_DecimalPlaces"]),
                        PII_POQty = Convert.ToString(dr["POI_Qty"]),
                        PII_Qty = Convert.ToString(dr["PII_Qty"]),
                        PII_UnitPrice = Convert.ToString(dr["PII_UnitPrice"]),
                        PII_Amount = Convert.ToString(dr["PII_Amount"]),
                        PII_ExpenseValue = Convert.ToString(dr["PII_ExpenseValue"]),
                        PII_HSN_Number = Convert.ToString(dr["PII_HSN_Number"]),
                        PII_HSN = Convert.ToString(dr["PII_HSN"]),
                        PII_GST_Amount = Convert.ToString(dr["PII_GST_Amount"]),
                        PII_WHT_Percent = Convert.ToString(dr["PII_WHT_Percent"]),
                        PII_WHT_Amount = Convert.ToString(dr["PII_WHT_Amount"]),
                        PII_IsDeleted = "false"
                    });
            }
            return PIList;
        }
        public List<POItemTOPurchaseInvoiceExpense_DTO> POItemTOPIExpenseEditList(DataTable Dt)
        {
            List<POItemTOPurchaseInvoiceExpense_DTO> PIList = new List<POItemTOPurchaseInvoiceExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POItemTOPurchaseInvoiceExpense_DTO
                    {
                        PIH_EXP_Number = Convert.ToInt64(dr["PIH_EXP_Number"]),
                        PIH_EXP_POH_EXP_Number = Convert.ToInt64(dr["PIH_EXP_POH_EXP_Number"]),
                        PIH_EXP_POH_Number = Convert.ToInt64(dr["PIH_EXP_POH_Number"]),
                        PIH_EXP_POH_OrderNo = Convert.ToString(dr["PIH_EXP_POH_OrderNo"]),
                        PIH_EXP_Expense_Number = Convert.ToString(dr["PIH_EXP_Expense_Number"]),
                        PIH_EXP_Description = Convert.ToString(dr["PIH_EXP_Description"]),
                        PIH_EXP_Remarks = Convert.ToString(dr["PIH_EXP_Remarks"]),
                        PIH_EXP_Occurrence_Number = Convert.ToString(dr["PIH_EXP_Occurrence_Number"]),
                        PIH_EXP_CM_Number = Convert.ToString(dr["PIH_EXP_CM_Number"]),
                        PIH_EXP_ExpenseBase = Convert.ToString(dr["PIH_EXP_ExpenseBase"]),
                        PIH_EXP_ExpenseValue = Convert.ToString(dr["PIH_EXP_ExpenseValue"]),
                        PIH_EXP_Allocate_Number = Convert.ToString(dr["PIH_EXP_Allocate_Number"]),
                        PIH_EXP_LA_Number = Convert.ToString(dr["PIH_EXP_LA_Number"]),
                        PIH_EXP_SAC_Number = Convert.ToString(dr["PIH_EXP_SAC_Number"]),
                        PIH_EXP_TaxCalculate = Convert.ToString(dr["PIH_EXP_TaxCalculate"]),
                        PIH_EXP_TaxValue = Convert.ToString(dr["PIH_EXP_TaxValue"]),
                        PIH_EXP_IsDeleted = "false"
                    }); ;
            }
            return PIList;
        }
        public List<POItemTOPurchaseInvoiceIExpense_DTO> POItemTOPIIExpenseEditList(DataTable Dt)
        {
            List<POItemTOPurchaseInvoiceIExpense_DTO> PIList = new List<POItemTOPurchaseInvoiceIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POItemTOPurchaseInvoiceIExpense_DTO
                    {
                        PII_EXP_Number = Convert.ToInt64(dr["PII_EXP_Number"]),
                        PII_EXP_POI_EXP_Number = Convert.ToInt64(dr["PII_EXP_POI_EXP_Number"]),
                        PII_EXP_PII_Number = Convert.ToInt64(dr["PII_EXP_PII_Number"]),

                        PII_EXP_POI_Number = Convert.ToInt64(dr["PII_EXP_POI_Number"]),
                        PII_EXP_POH_Number = Convert.ToInt64(dr["PII_EXP_POH_Number"]),

                        PII_EXP_Expense_Number = Convert.ToString(dr["PII_EXP_Expense_Number"]),
                        PII_EXP_Description = Convert.ToString(dr["PII_EXP_Description"]),
                        PII_EXP_Remarks = Convert.ToString(dr["PII_EXP_Remarks"]),
                        PII_EXP_Occurrence_Number = Convert.ToString(dr["PII_EXP_Occurrence_Number"]),
                        PII_EXP_CM_Number = Convert.ToString(dr["PII_EXP_CM_Number"]),
                        PII_EXP_ExpenseBase = Convert.ToString(dr["PII_EXP_ExpenseBase"]),
                        PII_EXP_ExpenseValue = Convert.ToString(dr["PII_EXP_ExpenseValue"]),
                        PII_EXP_Allocate_Number = Convert.ToString(dr["PII_EXP_Allocate_Number"]),
                        PII_EXP_LA_Number = Convert.ToString(dr["PII_EXP_LA_Number"]),
                        PII_EXP_IsDeleted = "false"
                    });
            }
            return PIList;
        }
        public List<POItemTOPurchaseInvoiceBatch_DTO> POItemTOPIIBatchEditList(DataTable Dt)
        {
            List<POItemTOPurchaseInvoiceBatch_DTO> PIList = new List<POItemTOPurchaseInvoiceBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new POItemTOPurchaseInvoiceBatch_DTO
                    {
                        PII_BCH_Number = Convert.ToInt64(dr["PII_BCH_Number"]),
                        PII_BCH_POH_Number = Convert.ToInt64(dr["PII_POH_Number"]),
                        PII_BCH_POI_Number = Convert.ToInt64(dr["PII_POI_Number"]),
                        PII_BCH_PII_Number = Convert.ToInt64(dr["PII_BCH_PII_Number"]),
                        PII_BCH_Date = Convert.ToString(dr["PII_BCH_Date"]),
                        PII_BCH_No = Convert.ToString(dr["PII_BCH_No"]),
                        PII_BCH_Qty = Convert.ToString(dr["PII_BCH_Qty"]),
                        PII_BCH_UnitPrice = Convert.ToString(dr["PII_BCH_UnitPrice"]),
                        PII_BCH_Value = Convert.ToString(dr["PII_BCH_Value"]),
                        PII_BCH_IsDeleted = "false"
                    });
            }
            return PIList;
        }








        //Return
        public List<Item_DTO> PRIList(DataTable Dt)
        {
            List<Item_DTO> IList = new List<Item_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new Item_DTO
                    {
                        ItemNumber = Convert.ToInt64(dr["ItemNumber"]),
                        ItemCode = Convert.ToString(dr["ItemCode"]),
                        ItemDescription = Convert.ToString(dr["ItemDescription"]),
                        ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        UoM = Convert.ToString(dr["Uom"]),
                        DecimalPlaces = Convert.ToInt32(dr["DecimalPlaces"]),
                        MaterialSegregation = Convert.ToString(dr["MaterialSegregation"]),
                        HSN_Code = Convert.ToString(dr["HSN_Code"]),
                        PurchaseWarehouse = Convert.ToInt32(dr["PurchaseWarehouse"]),
                    });
            }
            return IList;
        }
        public List<PurchaseReturnExpense_DTO> ExReturnList(DataTable Dt)
        {
            List<PurchaseReturnExpense_DTO> IList = new List<PurchaseReturnExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseReturnExpense_DTO
                    {
                        PRH_EXP_Description = Convert.ToString(dr["EC_Description"]),
                        PRH_EXP_LA_Number = Convert.ToString(dr["LedgerAccount"]),
                        PRH_EXP_SAC_Number = Convert.ToString(dr["EC_SAC_Number"]),
                    });
            }
            return IList;
        }
        public List<PurchaseReturnGst> PurRerGst(DataTable Dt)
        {
            List<PurchaseReturnGst> IList = new List<PurchaseReturnGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseReturnGst
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        Chargeable = Convert.ToString(dr["Chargeable"]),
                        Calculation = Convert.ToInt64(dr["Calculation"]),
                        Percentage = Convert.ToDouble(dr["Percentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseReturnGst> PurRerGstView(DataTable Dt)
        {
            List<PurchaseReturnGst> IList = new List<PurchaseReturnGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseReturnGst
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        TaxCategory = Convert.ToString(dr["TaxCategory"]),
                        TaxType = Convert.ToString(dr["TaxType"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        TaxElementName = Convert.ToString(dr["TaxElementName"]),
                        LoadonInventory = Convert.ToString(dr["LoadonInventory"]),
                        LoadonInventoryPercent = Convert.ToString(dr["LoadonInventoryPercent"]),
                        Chargeable = Convert.ToString(dr["Chargeable"]),
                        Calculation = Convert.ToInt64(dr["Calculation"]),
                        Percentage = Convert.ToDouble(dr["Percentage"])
                    });
            }
            return IList;
        }
        public List<PurchaseReturnWHT> PurRerWHT(DataTable Dt)
        {
            List<PurchaseReturnWHT> IList = new List<PurchaseReturnWHT>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new PurchaseReturnWHT
                    {
                        TaxNumber = Convert.ToInt64(dr["TaxNumber"]),
                        TaxCode = Convert.ToString(dr["TaxCode"]),
                        Tax = Convert.ToInt64(dr["IncludeTax"]),
                        Percentage = Convert.ToDouble(dr["TaxPercentage"])
                    });
            }
            return IList;
        }
        public List<TaxCluster_DTO> PurRerCluster(DataTable Dt)
        {
            List<TaxCluster_DTO> IList = new List<TaxCluster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new TaxCluster_DTO
                    {
                        TaxClusterNumber = Convert.ToInt64(dr["TaxClusterNumber"]),
                        TaxCluster = Convert.ToString(dr["TaxCluster"]),
                    });
            }
            return IList;
        }
        public List<PurchaseReturn_DTO> PRList(DataTable Dt)
        {
            List<PurchaseReturn_DTO> POList = new List<PurchaseReturn_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseReturn_DTO
                    {
                        PRH_Number = Convert.ToInt64(dr["PRH_Number"]),
                        PRH_ReturnNo = Convert.ToString(dr["PRH_ReturnNo"]),
                        PRH_ReturnDate = Convert.ToString(dr["PRH_ReturnDate"]),
                        PRH_ImportOrder = Convert.ToInt32(dr["PRH_ImportOrder"]),
                        PRH_Vendor_Name = Convert.ToString(dr["PRH_Vendor_Name"]),
                        PRH_Vendor_Group = Convert.ToString(dr["PRH_Vendor_Group"]),
                        PRH_Vendor_Category = Convert.ToString(dr["PRH_Vendor_Category"]),
                        PRH_Currency_Number = Convert.ToString(dr["PRH_Currency_Number"]),
                        PRH_TaxCluster_Number = Convert.ToString(dr["PRH_TaxCluster_Number"]),
                        PRH_WHT_Number = Convert.ToString(dr["PRH_WHT_Number"]),
                        PRH_MS_Number = Convert.ToString(dr["PRH_MS_Number"]),
                        PRH_ExchangeRate = Convert.ToDouble(dr["PRH_ExchangeRate"]),

                        PRI_Qty = DecimalConvertQty(Convert.ToDouble(dr["PRI_Qty"])),
                        PRH_MaterialCost = Convert.ToString(dr["PRH_MaterialCost"]),
                        PRH_ItemMiscExpense = Convert.ToString(dr["PRH_ItemMiscExpense"]),
                        PRH_HeaderMiscExpense = Convert.ToString(dr["PRH_HeaderMiscExpense"]),
                        PRH_Amount = Convert.ToString(dr["PRH_Amount"]),

                        PRH_GST_Amount = Convert.ToString(dr["PRH_GST_Amount"]),
                        PRI_GST_Amount = Convert.ToString(dr["PRI_GST_Amount"]),
                        PRH_ReturnAmount = Convert.ToString(dr["PRH_ReturnAmount"]),
                        PRH_WHT_Amount = Convert.ToString(dr["PRH_WHT_Amount"]),
                        PRH_RoundOff = Convert.ToString(dr["PRH_RoundOff"]),
                        PRH_VendorReceivable = Convert.ToString(dr["PRH_VendorReceivable"]),

                        PRH_TotalItem = Convert.ToString(dr["PRH_TotalItem"]),

                        PRH_POH_OrderNo = Convert.ToString(dr["PRH_POH_OrderNo"]),
                        PRH_POH_OrderCount = Convert.ToInt32(dr["PRH_POH_OrderCount"]),

                        PRH_PIH_InvoiceNo = Convert.ToString(dr["PRH_PIH_InvoiceNo"]),
                        PRH_PIH_InvoiceCount = Convert.ToInt32(dr["PRH_PIH_InvoiceCount"]),

                        PRH_PIH_SupplierInvoiceNo = Convert.ToString(dr["PRH_PIH_SupplierInvoiceNo"]),
                        PRH_SupplierCount = Convert.ToInt32(dr["PRH_SupplierCount"]),
                    });
            }
            return POList;
        }
        public List<PurchaseReturnAscii> PRAscii(DataTable Dt)
        {
            List<PurchaseReturnAscii> POList = new List<PurchaseReturnAscii>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseReturnAscii
                    {
                        PRH_Number = Convert.ToInt64(dr["PRH_Number"]),
                        PRH_ReturnNo = Convert.ToString(dr["PRH_ReturnNo"]),
                        PRH_ReturnDate = Convert.ToString(dr["PRH_ReturnDate"]),
                        PRH_ImportOrder = Convert.ToString(dr["PRH_ImportOrder"]) == "1" ? "Yes" : "No",
                        PRH_Vendor_Number = Convert.ToString(dr["PRH_Vendor_Number"]),
                        PRH_Currency_Number = Convert.ToString(dr["PRH_Currency_Number"]),
                        PRH_TaxCluster_Number = Convert.ToString(dr["PRH_TaxCluster_Number"]),
                        PRH_WHT_Number = Convert.ToString(dr["PRH_WHT_Number"]),
                        PRH_MS_Number = Convert.ToString(dr["PRH_MS_Number"]),

                        PRI_Qty = Convert.ToString(dr["PRI_Qty"]),
                        PRH_MaterialCost = Convert.ToString(dr["PRH_MaterialCost"]),
                        PRH_ItemMiscExpense = Convert.ToString(dr["PRH_ItemMiscExpense"]),
                        PRH_HeaderMiscExpense = Convert.ToString(dr["PRH_HeaderMiscExpense"]),

                        PRH_GST_Amount = Convert.ToString(dr["PRH_GST_Amount"]),
                        PRH_ReturnAmount = Convert.ToString(dr["PRH_ReturnAmount"]),
                        PRH_WHT_Amount = Convert.ToString(dr["PRH_WHT_Amount"]),
                        PRH_RoundOff = Convert.ToString(dr["PRH_RoundOff"]),
                        PRH_VendorReceivable = Convert.ToString(dr["PRH_VendorReceivable"]),

                        PRH_TotalItem = Convert.ToString(dr["PRH_TotalItem"])
                    });
            }
            return POList;
        }
        public List<PurchaseReturn_DTO> PRDetailList(DataTable Dt)
        {
            List<PurchaseReturn_DTO> POList = new List<PurchaseReturn_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseReturn_DTO
                    {
                        PRH_Number = Convert.ToInt64(dr["PRH_Number"]),
                        PRI_Number = Convert.ToInt64(dr["PRI_Number"]),
                        PRH_ReturnNo = Convert.ToString(dr["PRH_ReturnNo"]),
                        PRH_ReturnDate = Convert.ToString(dr["PRH_ReturnDate"]),
                        PRH_POH_OrderNo = Convert.ToString(dr["PRH_POH_OrderNo"]),
                        PRH_POH_OrderDate = Convert.ToString(dr["PRH_POH_OrderDate"]),
                        PRH_PIH_InvoiceNo = Convert.ToString(dr["PRH_PIH_InvoiceNo"]),
                        PRH_PIH_InvoiceDate = Convert.ToString(dr["PRH_PIH_InvoiceDate"]),
                        PRH_PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PRH_PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PRH_ImportOrder = Convert.ToInt32(dr["PRH_ImportOrder"]),
                        PRH_Vendor_Name = Convert.ToString(dr["PRH_Vendor_Name"]),
                        PRH_Vendor_Group = Convert.ToString(dr["PRH_Vendor_Group"]),
                        PRH_Vendor_Category = Convert.ToString(dr["PRH_Vendor_Category"]),
                        PRH_Currency_Number = Convert.ToString(dr["PRH_Currency_Number"]),
                        PRI_MS_Number = Convert.ToString(dr["PRI_MS_Number"]),
                        PRI_ItemGroup = Convert.ToString(dr["PRI_ItemGroup"]),
                        PRI_ItemCode = Convert.ToString(dr["PRI_ItemCode"]),
                        PRI_ItemMaterialGrade = Convert.ToString(dr["PRI_ItemMaterialGrade"]),
                        PRI_ItemThickness = Convert.ToString(dr["PRI_ItemThickness"]),
                        PRI_Description = Convert.ToString(dr["PRI_ItemDescription"]),
                        PRI_Warehouse_Number = Convert.ToString(dr["PRI_Warehouse_Number"]),
                        PRI_UoM = Convert.ToString(dr["PRI_UoM_Number"]),
                        PRI_Qty = DecimalConvertQty(Convert.ToDouble(dr["PRI_Qty"])),
                        PRI_UnitPrice = DecimalConvertUnit(Convert.ToDouble(dr["PRI_UnitPrice"])),
                        PRI_MaterialCost = Convert.ToString(dr["PRI_MaterialCost"]),
                        PRI_Amount = Convert.ToString(dr["PRI_Amount"]),
                        //PRH_EXP_Expense = Convert.ToString(dr["PRH_EXP_Expense"]),
                        PRH_ExchangeRate = Convert.ToDouble(dr["PRH_ExchangeRate"]),
                        PRH_ItemMiscExpense = Convert.ToString(dr["PRI_ItemMiscExpense"]),
                        PRH_HeaderMiscExpense = Convert.ToString(dr["PRH_HeaderMiscExpense"]),
                        PRI_HSN_Number = Convert.ToString(dr["PRI_HSN_Number"]),
                        PRI_HSN_Description = Convert.ToString(dr["PRI_HSN_Description"]),
                        PRI_GST_Amount = Convert.ToString(dr["PRI_GST_Amount"]),
                        PRH_GST_Amount = Convert.ToString(dr["PRH_GST_Amount"]),
                        PRI_WHT_Amount = Convert.ToString(dr["PRI_WHT_Amount"]),
                        PRH_WHT_Number = Convert.ToString(dr["PRH_WHT_Number"]),
                        PRI_WHT_Percent = Convert.ToString(dr["PRI_WHT_Percent"]),
                        PRH_ReturnAmount = Convert.ToString(dr["PRH_ReturnAmount"]),
                        PRH_RoundOff = Convert.ToString(dr["PRH_RoundOff"]),
                        PRH_VendorReceivable = Convert.ToString(dr["PRH_VendorReceivable"])
                    });
            }
            return POList;
        }
        public List<PurchaseReturnDetailAscii> PRDAscii(DataTable Dt)
        {
            List<PurchaseReturnDetailAscii> POList = new List<PurchaseReturnDetailAscii>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseReturnDetailAscii
                    {
                        PRH_Number = Convert.ToInt64(dr["PRI_Number"]),
                        PRH_ReturnNo = Convert.ToString(dr["PRH_ReturnNo"]),
                        PRH_ReturnDate = Convert.ToString(dr["PRH_ReturnDate"]),
                        PRH_ImportOrder = Convert.ToString(dr["PRH_ImportOrder"]) == "1" ? "Yes" : "No",
                        PRH_Vendor_Number = Convert.ToString(dr["PRH_Vendor_Number"]),
                        PRH_Currency_Number = Convert.ToString(dr["PRH_Currency_Number"]),
                        PRI_MS_Number = Convert.ToString(dr["PRI_MS_Number"]),
                        PRI_ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        PRI_ItemCode = Convert.ToString(dr["ItemCode"]),
                        PRI_Description = Convert.ToString(dr["ItemDescription"]),
                        PRI_Warehouse_Number = Convert.ToString(dr["PRI_Warehouse_Number"]),
                        PRI_UoM = Convert.ToString(dr["PRI_UoM_Number"]),
                        PRI_Qty = Convert.ToString(dr["PRI_Qty"]),
                        PRI_UnitPrice = Convert.ToString(dr["PRI_UnitPrice"]),
                        PRI_Amount = Convert.ToString(dr["PRI_Amount"]),
                        PRH_EXP_Expense = Convert.ToString(dr["PRH_EXP_Expense"]),
                        PRH_ItemMiscExpense = Convert.ToString(dr["PRI_ItemMiscExpense"]),
                        PRH_HeaderMiscExpense = Convert.ToString(dr["PRH_HeaderMiscExpense"]),
                        PRI_HSN_Number = Convert.ToString(dr["PRI_HSN_Number"]),
                        PRI_GST_Amount = Convert.ToString(dr["PRI_GST_Amount"]),
                        PRI_WHT_Amount = Convert.ToString(dr["PRI_WHT_Amount"]),
                        PRI_WHT_Code = Convert.ToString(dr["PRH_WHT_Number"]),
                        PRI_WHT_Percent = Convert.ToString(dr["PRI_WHT_Percent"])
                    });
            }
            return POList;
        }
        public List<PurchaseReturnHead_DTO> PRHeadList(DataTable Dt)
        {
            List<PurchaseReturnHead_DTO> PRList = new List<PurchaseReturnHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRList.Add(
                    new PurchaseReturnHead_DTO
                    {
                        PRH_Number = Convert.ToInt64(dr["PRH_Number"]),
                        PRH_ReturnNo = Convert.ToString(dr["PRH_ReturnNo"]),
                        PRH_ReturnDate = Convert.ToString(dr["PRH_ReturnDate"]),
                        PRH_ImportOrder = Convert.ToString(dr["PRH_ImportOrder"]) == "1" ? "true" : "false",
                        PRH_Vendor_Number = Convert.ToString(dr["PRH_Vendor_Number"]),
                        PRH_Vendor = Convert.ToString(dr["PRH_Vendor"]),
                        PRH_VendorLocation = Convert.ToString(dr["PRH_VendorLocation"]),
                        PRH_Currency_Number = Convert.ToString(dr["PRH_Currency_Number"]),
                        PRH_TaxCluster = Convert.ToString(dr["PRH_TaxCluster"]),
                        PRH_TaxCluster_Number = Convert.ToString(dr["PRH_TaxCluster_Number"]),
                        PRH_WHT_Number = Convert.ToString(dr["PRH_WHT_Number"]),
                        PRH_MS_Number = Convert.ToString(dr["PRH_MS_Number"]),
                        PRH_Currency = Convert.ToString(dr["PRH_Currency"]),
                        PRH_DecimalPlaces = Convert.ToString(dr["PRH_DecimalPlaces"]),
                        PRH_ExchangeRate = Convert.ToString(dr["PRH_ExchangeRate"]),
                        PRH_DueDate = Convert.ToString(dr["PRH_DueDate"]),

                        PRH_MaterialCost = Convert.ToString(dr["PRH_MaterialCost"]),
                        PRH_ItemMiscExpense = Convert.ToString(dr["PRH_ItemMiscExpense"]),
                        PRH_HeaderMiscExpense = Convert.ToString(dr["PRH_HeaderMiscExpense"]),
                        PRH_GST_Amount = Convert.ToString(dr["PRH_GST_Amount"]),
                        PRH_ReturnAmount = Convert.ToString(dr["PRH_ReturnAmount"]),
                        PRH_WHT_Amount = Convert.ToString(dr["PRH_WHT_Amount"]),
                        PRH_RoundOff = Convert.ToString(dr["PRH_RoundOff"]),
                        PRH_VendorReceivable = Convert.ToString(dr["PRH_VendorReceivable"]),

                        PRH_DeliveryTerms = Convert.ToString(dr["PRH_DeliveryTerms"]),
                        PRH_DeliveryMode = Convert.ToString(dr["PRH_DeliveryMode"]),

                        PRH_WHT_Code = Convert.ToString(dr["PRH_WHT"]),
                        PRH_Mode = Convert.ToInt32(dr["PRH_Mode"]),
                    });
            }
            return PRList;
        }
        public List<PurchaseReturnItem_DTO> PRItemList(DataTable Dt)
        {
            List<PurchaseReturnItem_DTO> PRList = new List<PurchaseReturnItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRList.Add(
                    new PurchaseReturnItem_DTO
                    {
                        PRI_SupplierInvoiceNo = Convert.ToString(dr["PRI_SupplierInvoiceNo"]),
                        PRI_POH_Order = Convert.ToString(dr["PRI_POH_Order"]),
                        PRI_Number = Convert.ToInt64(dr["PRI_Number"]),
                        PRI_ItemCode = Convert.ToString(dr["PRI_ItemCode"]),
                        PRI_Item_Number = Convert.ToString(dr["PRI_Item_Number"]),
                        PRI_ItemGroup = Convert.ToString(dr["PRI_ItemGroup"]),
                        PRI_Description = Convert.ToString(dr["PRI_Description"]),
                        PRI_Warehouse_Number = Convert.ToString(dr["PRI_Warehouse_Number"]),
                        PRI_MS_Number = Convert.ToInt64(dr["PRI_MS_Number"]),
                        PRI_UoM_Number = Convert.ToString(dr["PRI_UoM_Number"]),
                        PRI_UoM = Convert.ToString(dr["PRI_UoM"]),
                        PRI_DecimalPlaces = Convert.ToString(dr["PRI_DecimalPlaces"]),
                        PRI_Qty = Convert.ToString(dr["PRI_Qty"]),
                        PRI_UnitPrice = Convert.ToString(dr["PRI_UnitPrice"]),
                        PRI_Amount = Convert.ToString(dr["PRI_Amount"]),
                        PRI_ExpenseValue = Convert.ToString(dr["PRI_ExpenseValue"]),
                        PRI_HSN_Number = Convert.ToString(dr["PRI_HSN_Number"]),
                        PRI_HSN = Convert.ToString(dr["PRI_HSN"]),
                        PRI_GST_Amount = Convert.ToString(dr["PRI_GST_Amount"]),
                        PRI_WHT_Percent = Convert.ToString(dr["PRI_WHT_Percent"]),
                        PRI_WHT_Amount = Convert.ToString(dr["PRI_WHT_Amount"]),
                    });
            }
            return PRList;
        }
        public List<PurchaseReturnExpense_DTO> PRExpenseEditList(DataTable Dt)
        {
            List<PurchaseReturnExpense_DTO> PRList = new List<PurchaseReturnExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRList.Add(
                    new PurchaseReturnExpense_DTO
                    {
                        PRH_EXP_SupplierInvoiceNo = Convert.ToString(dr["PRI_SupplierInvoiceNo"]),
                        PRH_EXP_POH_OrderNo = Convert.ToString(dr["PRI_POH_Order"]),
                        PRH_EXP_Number = Convert.ToInt64(dr["PRH_EXP_Number"]),
                        PRH_EXP_Expense_Number = Convert.ToString(dr["PRH_EXP_Expense_Number"]),
                        PRH_EXP_Description = Convert.ToString(dr["PRH_EXP_Description"]),
                        PRH_EXP_Remarks = Convert.ToString(dr["PRH_EXP_Remarks"]),
                        PRH_EXP_Occurrence_Number = Convert.ToString(dr["PRH_EXP_Occurrence_Number"]),
                        PRH_EXP_CM_Number = Convert.ToString(dr["PRH_EXP_CM_Number"]),
                        PRH_EXP_ExpenseBase = Convert.ToString(dr["PRH_EXP_ExpenseBase"]),
                        PRH_EXP_ExpenseValue = Convert.ToString(dr["PRH_EXP_ExpenseValue"]),
                        PRH_EXP_Allocate_Number = Convert.ToString(dr["PRH_EXP_Allocate_Number"]),
                        PRH_EXP_LA_Number = Convert.ToString(dr["PRH_EXP_LA_Number"]),
                        PRH_EXP_SAC_Number = Convert.ToString(dr["PRH_EXP_SAC_Number"]),
                        PRH_EXP_TaxCalculate = Convert.ToString(dr["PRH_EXP_TaxCalculate"]),
                        PRH_EXP_TaxValue = Convert.ToString(dr["PRH_EXP_TaxValue"]),
                    });
            }
            return PRList;
        }
        public List<PurchaseReturnIExpense_DTO> PRIExpenseEditList(DataTable Dt)
        {
            List<PurchaseReturnIExpense_DTO> PRList = new List<PurchaseReturnIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRList.Add(
                    new PurchaseReturnIExpense_DTO
                    {
                        PRI_EXP_Number = Convert.ToInt64(dr["PRI_EXP_Number"]),
                        PRI_EXP_PRI_Number = Convert.ToInt64(dr["PRI_EXP_PRI_Number"]),
                        PRI_EXP_Expense_Number = Convert.ToString(dr["PRI_EXP_Expense_Number"]),
                        PRI_EXP_Description = Convert.ToString(dr["PRI_EXP_Description"]),
                        PRI_EXP_Remarks = Convert.ToString(dr["PRI_EXP_Remarks"]),
                        PRI_EXP_Occurrence_Number = Convert.ToString(dr["PRI_EXP_Occurrence_Number"]),
                        PRI_EXP_CM_Number = Convert.ToString(dr["PRI_EXP_CM_Number"]),
                        PRI_EXP_ExpenseBase = Convert.ToString(dr["PRI_EXP_ExpenseBase"]),
                        PRI_EXP_ExpenseValue = Convert.ToString(dr["PRI_EXP_ExpenseValue"]),
                        PRI_EXP_Allocate_Number = Convert.ToString(dr["PRI_EXP_Allocate_Number"]),
                        PRI_EXP_LA_Number = Convert.ToString(dr["PRI_EXP_LA_Number"]),
                    });
            }
            return PRList;
        }
        public List<PurchaseReturnBatch_DTO> PRIBatchEditList(DataTable Dt)
        {
            List<PurchaseReturnBatch_DTO> PRList = new List<PurchaseReturnBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRList.Add(
                    new PurchaseReturnBatch_DTO
                    {
                        PRI_BCH_Number = Convert.ToInt64(dr["PRI_BCH_Number"]),
                        PRI_BCH_PRI_Number = Convert.ToInt64(dr["PRI_BCH_PRI_Number"]),
                        PRI_BCH_Date = Convert.ToString(dr["PRI_BCH_Date"]),
                        PRI_BCH_No = Convert.ToString(dr["PRI_BCH_No"]),
                        PRI_BCH_Qty = Convert.ToString(dr["PRI_BCH_Qty"]),
                        PRI_BCH_UnitPrice = Convert.ToString(dr["PRI_BCH_UnitPrice"]),
                        PRI_BCH_Value = Convert.ToString(dr["PRI_BCH_Value"])
                    });
            }
            return PRList;
        }





        //return GRN
        public List<Vendor_DTO> PRVList(DataTable Dt)
        {
            List<Vendor_DTO> VList = new List<Vendor_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VList.Add(
                    new Vendor_DTO
                    {
                        VendorNumber = Convert.ToInt64(dr["VendorNumber"]),
                        VendorCode = Convert.ToString(dr["VendorCode"]),
                        VendorName = Convert.ToString(dr["VendorName"]),
                        VendorLocation = Convert.ToString(dr["VendorLocation"]),
                        Currency = Convert.ToString(dr["CurrencyNumber"]),
                        CurrencyCode = Convert.ToString(dr["CurrencyCode"]),
                        DecimalPlaces = Convert.ToString(dr["DecimalPlaces"]),
                        Cluster = Convert.ToString(dr["TaxCluster"]),
                        TaxNumber = Convert.ToInt64(dr["TaxClusterNumber"]),
                        WithholdTax = Convert.ToInt64(dr["TaxCode"]),
                        CreditDays = Convert.ToInt32(dr["CreditDays"]),
                        PaymentBase = Convert.ToInt64(dr["PaymentBase"]),
                    });
            }
            return VList;
        }
        public List<TaxCluster_DTO> PRCluster(DataTable Dt)
        {
            List<TaxCluster_DTO> IList = new List<TaxCluster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new TaxCluster_DTO
                    {
                        TaxClusterNumber = Convert.ToInt64(dr["TaxClusterNumber"]),
                        TaxCluster = Convert.ToString(dr["TaxCluster"]),
                    });
            }
            return IList;
        }
        public List<PurchaseInvoice_DTO> PRInvoice(DataTable Dt)
        {
            List<PurchaseInvoice_DTO> POList = new List<PurchaseInvoice_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseInvoice_DTO
                    {
                        PIH_Number = Convert.ToInt64(dr["PIH_Number"]),
                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PIH_MaterialCost = Convert.ToString(dr["PIH_MaterialCost"]),
                    });
            }
            return POList;
        }

        public List<PurchaseInvoicePRItem_DTO> PRInvoiceGet(DataTable Dt)
        {
            List<PurchaseInvoicePRItem_DTO> PIList = new List<PurchaseInvoicePRItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoicePRItem_DTO
                    {
                        PIH_Number = Convert.ToInt64(dr["PIH_Number"]),
                        PII_Number = Convert.ToInt64(dr["PII_Number"]),
                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        PIH_POH_Number = Convert.ToString(dr["POH_Number"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PII_MS_Number = Convert.ToString(dr["PIH_MS_Number"]),
                        PII_Item_Number = Convert.ToString(dr["PII_Item_Number"]),
                        PII_ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        PII_ItemCode = Convert.ToString(dr["ItemCode"]),
                        PII_Description = Convert.ToString(dr["ItemDescription"]),
                        PII_Warehouse_Number = Convert.ToString(dr["PII_Warehouse_Number"]),
                        PII_Warehouse = Convert.ToString(dr["PII_Warehouse"]),
                        PII_UoM_Number = Convert.ToString(dr["PII_UoM_Number"]),
                        PII_DecimalPlaces = Convert.ToString(dr["PII_DecimalPlaces"]),
                        PII_UoM = Convert.ToString(dr["PII_UoM"]),
                        PII_Qty = Convert.ToString(dr["PII_Qty"]),
                        PII_UnitPrice = Convert.ToString(dr["PII_UnitPrice"]),
                        PII_Amount = Convert.ToString(dr["PII_Amount"]),
                        PII_HSN_Number = Convert.ToString(dr["PII_HSN_Number"]),
                        PII_GST_Amount = Convert.ToString(dr["PII_GST_Amount"]),
                        PII_WHT_Amount = Convert.ToString(dr["PII_WHT_Amount"]),
                        PIH_WHT_Number = Convert.ToString(dr["PIH_WHT_Number"]),
                        PII_WHT_Percent = Convert.ToString(dr["PII_WHT_Percent"])
                    });
            }
            return PIList;
        }
        public List<PurchaseInvoicePRExpense_DTO> PRExpenseGet(DataTable Dt)
        {
            List<PurchaseInvoicePRExpense_DTO> PIList = new List<PurchaseInvoicePRExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoicePRExpense_DTO
                    {
                        PIH_EXP_Number = Convert.ToInt64(dr["PIH_EXP_Number"]),
                        PIH_EXP_PIH_Number = Convert.ToInt64(dr["PIH_EXP_PIH_Number"]),
                        PIH_EXP_PIH_InvoiceNo = Convert.ToString(dr["PIH_EXP_PIH_InvoiceNo"]),
                        PIH_EXP_POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        PIH_EXP_POH_Number = Convert.ToString(dr["POH_Number"]),
                        PIH_EXP_Expense_Number = Convert.ToString(dr["PIH_EXP_Expense_Number"]),
                        PIH_EXP_Description = Convert.ToString(dr["PIH_EXP_Description"]),
                        PIH_EXP_Remarks = Convert.ToString(dr["PIH_EXP_Remarks"]),
                        PIH_EXP_Occurrence_Number = Convert.ToString(dr["PIH_EXP_Occurrence_Number"]),
                        PIH_EXP_CM_Number = Convert.ToString(dr["PIH_EXP_CM_Number"]),
                        PIH_EXP_ExpenseBase = Convert.ToString(dr["PIH_EXP_ExpenseBase"]),
                        PIH_EXP_ExpenseValue = Convert.ToString(dr["PIH_EXP_ExpenseValue"]),
                        PIH_EXP_Allocate_Number = Convert.ToString(dr["PIH_EXP_Allocate_Number"]),
                        PIH_EXP_LA_Number = Convert.ToString(dr["PIH_EXP_LA_Number"]),
                        PIH_EXP_TaxCalculate = Convert.ToString(dr["PIH_EXP_TaxCalculate"]),
                        PIH_EXP_TaxValue = Convert.ToString(dr["PIH_EXP_TaxValue"]),
                        PIH_EXP_SAC_Number = Convert.ToString(dr["PIH_EXP_SAC_Number"]),
                    });
            }
            return PIList;
        }
        public List<PurchaseInvoicePRIExpense_DTO> PRIExpenseGet(DataTable Dt)
        {
            List<PurchaseInvoicePRIExpense_DTO> PIList = new List<PurchaseInvoicePRIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoicePRIExpense_DTO
                    {
                        PII_EXP_Number = Convert.ToInt64(dr["PII_EXP_Number"]),
                        PII_EXP_PII_Number = Convert.ToInt64(dr["PII_EXP_PII_Number"]),
                        PII_EXP_PIH_Number = Convert.ToInt64(dr["PII_EXP_PIH_Number"]),
                        PII_EXP_Expense_Number = Convert.ToString(dr["PII_EXP_Expense_Number"]),
                        PII_EXP_Description = Convert.ToString(dr["PII_EXP_Description"]),
                        PII_EXP_Remarks = Convert.ToString(dr["PII_EXP_Remarks"]),
                        PII_EXP_Occurrence_Number = Convert.ToString(dr["PII_EXP_Occurrence_Number"]),
                        PII_EXP_CM_Number = Convert.ToString(dr["PII_EXP_CM_Number"]),
                        PII_EXP_ExpenseBase = Convert.ToString(dr["PII_EXP_ExpenseBase"]),
                        PII_EXP_ExpenseValue = Convert.ToString(dr["PII_EXP_ExpenseValue"]),
                        PII_EXP_Allocate_Number = Convert.ToString(dr["PII_EXP_Allocate_Number"]),
                        PII_EXP_LA_Number = Convert.ToString(dr["PII_EXP_LA_Number"]),
                    });
            }
            return PIList;
        }

        public List<PurchaseInvoiceItemPRItem_DTO> ItemPRInvoiceGet(DataTable Dt)
        {
            List<PurchaseInvoiceItemPRItem_DTO> PIList = new List<PurchaseInvoiceItemPRItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoiceItemPRItem_DTO
                    {
                        PIH_Number = Convert.ToInt64(dr["PIH_Number"]),
                        PII_Number = Convert.ToInt64(dr["PII_Number"]),
                        PIH_InvoiceNo = Convert.ToString(dr["PIH_InvoiceNo"]),
                        PIH_InvoiceDate = Convert.ToString(dr["PIH_InvoiceDate"]),
                        PIH_POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        PIH_POH_Number = Convert.ToString(dr["POH_Number"]),
                        PIH_SupplierInvoiceNo = Convert.ToString(dr["PIH_SupplierInvoiceNo"]),
                        PIH_SupplierInvoiceDate = Convert.ToString(dr["PIH_SupplierInvoiceDate"]),
                        PII_MS_Number = Convert.ToString(dr["PIH_MS_Number"]),
                        PII_Item_Number = Convert.ToString(dr["PII_Item_Number"]),
                        PII_ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        PII_ItemCode = Convert.ToString(dr["ItemCode"]),
                        PII_Description = Convert.ToString(dr["ItemDescription"]),
                        PII_Warehouse_Number = Convert.ToString(dr["PII_Warehouse_Number"]),
                        PII_Warehouse = Convert.ToString(dr["PII_Warehouse"]),
                        PII_UoM_Number = Convert.ToString(dr["PII_UoM_Number"]),
                        PII_DecimalPlaces = Convert.ToString(dr["PII_DecimalPlaces"]),
                        PII_UoM = Convert.ToString(dr["PII_UoM"]),
                        PII_Qty = Convert.ToString(dr["PII_Qty"]),
                        PII_UnitPrice = Convert.ToString(dr["PII_UnitPrice"]),
                        PII_Amount = Convert.ToString(dr["PII_Amount"]),
                        PII_HSN_Number = Convert.ToString(dr["PII_HSN_Number"]),
                        PII_GST_Amount = Convert.ToString(dr["PII_GST_Amount"]),
                        PII_WHT_Amount = Convert.ToString(dr["PII_WHT_Amount"]),
                        PIH_WHT_Number = Convert.ToString(dr["PIH_WHT_Number"]),
                        PII_WHT_Percent = Convert.ToString(dr["PII_WHT_Percent"])
                    });
            }
            return PIList;
        }
        public List<PurchaseInvoiceItemPRExpense_DTO> ItemPRExpenseGet(DataTable Dt)
        {
            List<PurchaseInvoiceItemPRExpense_DTO> PIList = new List<PurchaseInvoiceItemPRExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoiceItemPRExpense_DTO
                    {
                        PIH_EXP_Number = Convert.ToInt64(dr["PIH_EXP_Number"]),
                        PIH_EXP_PIH_Number = Convert.ToInt64(dr["PIH_EXP_PIH_Number"]),
                        PIH_EXP_PIH_InvoiceNo = Convert.ToString(dr["PIH_EXP_PIH_InvoiceNo"]),
                        PIH_EXP_POH_OrderNo = Convert.ToString(dr["POH_OrderNo"]),
                        PIH_EXP_POH_Number = Convert.ToString(dr["POH_Number"]),
                        PIH_EXP_Expense_Number = Convert.ToString(dr["PIH_EXP_Expense_Number"]),
                        PIH_EXP_Description = Convert.ToString(dr["PIH_EXP_Description"]),
                        PIH_EXP_Remarks = Convert.ToString(dr["PIH_EXP_Remarks"]),
                        PIH_EXP_Occurrence_Number = Convert.ToString(dr["PIH_EXP_Occurrence_Number"]),
                        PIH_EXP_CM_Number = Convert.ToString(dr["PIH_EXP_CM_Number"]),
                        PIH_EXP_ExpenseBase = Convert.ToString(dr["PIH_EXP_ExpenseBase"]),
                        PIH_EXP_ExpenseValue = Convert.ToString(dr["PIH_EXP_ExpenseValue"]),
                        PIH_EXP_Allocate_Number = Convert.ToString(dr["PIH_EXP_Allocate_Number"]),
                        PIH_EXP_LA_Number = Convert.ToString(dr["PIH_EXP_LA_Number"]),
                        PIH_EXP_TaxCalculate = Convert.ToString(dr["PIH_EXP_TaxCalculate"]),
                        PIH_EXP_TaxValue = Convert.ToString(dr["PIH_EXP_TaxValue"]),
                        PIH_EXP_SAC_Number = Convert.ToString(dr["PIH_EXP_SAC_Number"]),
                    });
            }
            return PIList;
        }
        public List<PurchaseInvoiceItemPRIExpense_DTO> ItemPRIExpenseGet(DataTable Dt)
        {
            List<PurchaseInvoiceItemPRIExpense_DTO> PIList = new List<PurchaseInvoiceItemPRIExpense_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIList.Add(
                    new PurchaseInvoiceItemPRIExpense_DTO
                    {
                        PII_EXP_Number = Convert.ToInt64(dr["PII_EXP_Number"]),
                        PII_EXP_PII_Number = Convert.ToInt64(dr["PII_EXP_PII_Number"]),
                        PII_EXP_PIH_Number = Convert.ToInt64(dr["PII_EXP_PIH_Number"]),
                        PII_EXP_Expense_Number = Convert.ToString(dr["PII_EXP_Expense_Number"]),
                        PII_EXP_Description = Convert.ToString(dr["PII_EXP_Description"]),
                        PII_EXP_Remarks = Convert.ToString(dr["PII_EXP_Remarks"]),
                        PII_EXP_Occurrence_Number = Convert.ToString(dr["PII_EXP_Occurrence_Number"]),
                        PII_EXP_CM_Number = Convert.ToString(dr["PII_EXP_CM_Number"]),
                        PII_EXP_ExpenseBase = Convert.ToString(dr["PII_EXP_ExpenseBase"]),
                        PII_EXP_ExpenseValue = Convert.ToString(dr["PII_EXP_ExpenseValue"]),
                        PII_EXP_Allocate_Number = Convert.ToString(dr["PII_EXP_Allocate_Number"]),
                        PII_EXP_LA_Number = Convert.ToString(dr["PII_EXP_LA_Number"]),
                    });
            }
            return PIList;
        }
        public List<PurchaseInvoice_DTO> PRInvoiceItem(DataTable Dt)
        {
            List<PurchaseInvoice_DTO> POList = new List<PurchaseInvoice_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new PurchaseInvoice_DTO
                    {
                        PII_Item_Number = Convert.ToString(dr["ItemNumber"]),
                        PII_ItemCode = Convert.ToString(dr["ItemCode"]),
                        PII_Description = Convert.ToString(dr["ItemDescription"]),
                        PII_ItemGroup = Convert.ToString(dr["ItemGroup"]),
                    });
            }
            return POList;
        }

        public List<PRBatch_DTO> PRBatchList(DataTable Dt)
        {
            List<PRBatch_DTO> BList = new List<PRBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new PRBatch_DTO
                    {
                        PRI_BCH_Number = Convert.ToInt64(dr["ICB_Number"]),
                        PRI_BCH_Warehoue = Convert.ToString(dr["PII_BCH_Warehoue"]),
                        PRI_BCH_BCH_Number = Convert.ToInt64(dr["ICB_Batch_Number"]),
                        PRI_BCH_Date = Convert.ToString(dr["ICB_Batch_Date"]),
                        PRI_BCH_No = Convert.ToString(dr["ICB_Batch_No"]),
                        PRI_BCH_ItemQty = Convert.ToString(dr["ICB_ItemQty"]),
                        PRI_BCH_HoldQty = Convert.ToString(dr["ICB_HoldQty"]),
                        PRI_BCH_Qty = Convert.ToString(dr["ICB_Qty"]),
                        PRI_BCH_UnitPrice = Convert.ToString(dr["ICB_UnitPrice"]),
                        PRI_BCH_Value = Convert.ToString(dr["ICB_Value"]),
                    });
            }
            return BList;
        }
        public List<PRBatch_DTO> PRBatchOverallList(DataTable Dt)
        {
            List<PRBatch_DTO> BList = new List<PRBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new PRBatch_DTO
                    {
                        PRI_BCH_Warehoue = Convert.ToString(dr["PII_BCH_Warehoue"]),
                        PRI_BCH_Date = Convert.ToString(dr["ICB_Batch_Date"]),
                        PRI_BCH_No = Convert.ToString(dr["ICB_Batch_No"]),
                        PRI_BCH_Qty = Convert.ToString(dr["ICB_Qty"]),
                        PRI_BCH_UnitPrice = Convert.ToString(dr["ICB_UnitPrice"]),
                        PRI_BCH_Value = Convert.ToString(dr["ICB_Value"]),
                    });
            }
            return BList;
        }




        //Common
        public String DecimalConvertQty(Double Qty)
        {
            return String.Format(India, "{0:#,0.##}", Qty);
        }
        public String DecimalConvertUnit(Double UnitPrice)
        {
            return String.Format(India, "{0:#,0.00##}", UnitPrice);
        }
    }
}

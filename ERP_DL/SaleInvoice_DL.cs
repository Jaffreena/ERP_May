using ERP_DAO;
using ERP_DTO;
using System;
using System.Data;
using System.Globalization;

namespace ERP.DataList
{
    public class SaleInvoice_DL
    {
        MethodHelp MH = new MethodHelp();

        public List<ItemOrder_DTO> ItemList(DataTable Dt)
        {
            List<ItemOrder_DTO> I_List = new List<ItemOrder_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                I_List.Add(
                    new ItemOrder_DTO
                    {
                        ItemNumber = Convert.ToInt64(dr["ItemNumber"]),
                        ItemCode = Convert.ToString(dr["ItemCode"]),
                        ItemDescription = Convert.ToString(dr["ItemDescription"]),
                        ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        OuterDia = Convert.ToString(dr["OuterDia"]),
                        Thickness = Convert.ToString(dr["Thickness"]),
                        Length = Convert.ToString(dr["Length"]),
                        MaterialGrade = Convert.ToString(dr["MaterialGrade"]),
                        UoM = Convert.ToString(dr["Uom"]),
                        DecimalPlaces = Convert.ToInt32(dr["DecimalPlaces"]),
                        MaterialSegregation = Convert.ToString(dr["MaterialSegregation"]),
                        HSN_Code = Convert.ToString(dr["HSN_Code"]),
                        SaleWarehouse = Convert.ToInt32(dr["SaleWarehouse"]),
                    });
            }
            return I_List;
        }
        public List<IncomeCode_DTO> IncomeList(DataTable Dt)
        {
            List<IncomeCode_DTO> I_List = new List<IncomeCode_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                I_List.Add(
                    new IncomeCode_DTO
                    {
                        MIC_Number = Convert.ToInt64(dr["MIC_Number"]),
                        MIC_Code = Convert.ToString(dr["MIC_Code"]),
                        MIC_Description = Convert.ToString(dr["MIC_Description"]),
                        MIC_OCRN_Number = Convert.ToString(dr["MIC_OCRN_Number"]),
                        MIC_CM_Number = Convert.ToString(dr["MIC_CM_Number"]),
                        MIC_ALCT_Number = Convert.ToString(dr["MIC_ALCT_Number"]),
                        MIC_LA_Number = Convert.ToString(dr["MIC_LA_Number"]),
                        MIC_SAC_Number = Convert.ToString(dr["MIC_SAC_Number"])
                    });
            }
            return I_List;
        }
        public List<BuyerOrderList_DTO> BuyerList(DataTable Dt)
        {
            List<BuyerOrderList_DTO> BList = new List<BuyerOrderList_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new BuyerOrderList_DTO
                    {
                        BUY_Number = Convert.ToInt64(dr["BUY_Number"]),
                        BUY_Name = Convert.ToString(dr["BUY_Name"]),
                        BUY_CUR_Number = Convert.ToString(dr["BUY_CUR_Number"]),
                        BUY_CUR_Name = Convert.ToString(dr["BUY_CUR_Name"]),
                        BUY_LOC_Number = Convert.ToString(dr["BUY_LOC_Number"]),
                        BUY_CUR_DecimalPlaces = Convert.ToString(dr["BUY_CUR_DecimalPlaces"]),
                        BUY_TCT_Number = Convert.ToInt64(dr["BUY_GST_TCT_Number"]),
                        BUY_WHT_Number = Convert.ToInt64(dr["BUY_WHT_WHT_Number"]),
                    });
            }
            return BList;
        }
        public List<SaleInvoiceGst> SaleInvGst(DataTable Dt)
        {
            List<SaleInvoiceGst> IList = new List<SaleInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new SaleInvoiceGst
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
        public List<SaleInvoiceGst> SaleInvGstView(DataTable Dt)
        {
            List<SaleInvoiceGst> IList = new List<SaleInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new SaleInvoiceGst
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
        public List<SaleInvoiceWHT> SaleInvWHT(DataTable Dt)
        {
            List<SaleInvoiceWHT> IList = new List<SaleInvoiceWHT>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new SaleInvoiceWHT
                    {
                        TaxNumber = Convert.ToInt64(dr["TaxNumber"]),
                        TaxCode = Convert.ToString(dr["TaxCode"]),
                        Tax = Convert.ToInt64(dr["IncludeTax"]),
                        Percentage = Convert.ToDouble(dr["TaxPercentage"])
                    });
            }
            return IList;
        }
        public List<TaxCluster_DTO> SaleCluster(DataTable Dt)
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
        public List<BuyerAdd_DTO> BuyerAddress(DataTable Dt)
        {
            List<BuyerAdd_DTO> BuyerAddList = new List<BuyerAdd_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BuyerAddList.Add(
                    new BuyerAdd_DTO
                    {
                        BUY_ADD_Number = Convert.ToInt64(dr["BUY_ADD_Number"]),
                        BUY_ADD_ADTP_Number = Convert.ToInt64(dr["BUY_ADD_ADTP_Number"]),
                        BUY_ADD_AddressID = Convert.ToString(dr["BUY_ADD_AddressID"]),
                        BUY_ADD_Address = Convert.ToString(dr["BUY_ADD_Address"]),
                        BUY_ADD_City = Convert.ToString(dr["BUY_ADD_City"]),
                        BUY_ADD_State = Convert.ToString(dr["BUY_ADD_State"]),
                        BUY_ADD_Country = Convert.ToString(dr["BUY_ADD_Country"]),
                        BUY_ADD_Pin = Convert.ToString(dr["BUY_ADD_Pin"]),
                        BUY_ADD_GSTIN = Convert.ToString(dr["BUY_ADD_GSTIN"]),
                        BUY_ADD_Primary = Convert.ToBoolean(dr["BUY_ADD_Primary"]),
                    });
            }
            return BuyerAddList;
        }
        public List<BuyerAdd_DTO> JWCCustAddress(DataTable Dt)
        {
            List<BuyerAdd_DTO> BuyerAddList = new List<BuyerAdd_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BuyerAddList.Add(
                    new BuyerAdd_DTO
                    {
                        BUY_ADD_Number = Convert.ToInt64(dr["JWC_ADD_Number"]),
                        BUY_ADD_ADTP_Number = Convert.ToInt64(dr["JWC_ADD_ADTP_Number"]),
                        BUY_ADD_AddressID = Convert.ToString(dr["JWC_ADD_Address_ID"]),
                        BUY_ADD_Address = Convert.ToString(dr["JWC_ADD_Address"]),
                        BUY_ADD_City = Convert.ToString(dr["JWC_ADD_City"]),
                        BUY_ADD_State = Convert.ToString(dr["JWC_ADD_State"]),
                        BUY_ADD_Country = Convert.ToString(dr["JWC_ADD_Country"]),
                        BUY_ADD_Pin = Convert.ToString(dr["JWC_ADD_Pin"]),
                        BUY_ADD_GSTIN = Convert.ToString(dr["JWC_ADD_GSTIN"])
                        
                    });
            }
            return BuyerAddList;
        }
        public List<BuyerAdd_DTO> BuyerAddressID(DataTable Dt)
        {
            List<BuyerAdd_DTO> BuyerAddList = new List<BuyerAdd_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BuyerAddList.Add(
                    new BuyerAdd_DTO
                    {
                        BUY_ADD_Number = Convert.ToInt64(dr["BUY_ADD_Number"]),
                        BUY_ADD_AddressID = Convert.ToString(dr["BUY_ADD_AddressID"]),
                    });
            }
            return BuyerAddList;
        }

        public List<BuyerAdd_DTO> JWCCustAddressID(DataTable Dt)
        {
            List<BuyerAdd_DTO> BuyerAddList = new List<BuyerAdd_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BuyerAddList.Add(
                    new BuyerAdd_DTO
                    {
                        BUY_ADD_Number = Convert.ToInt64(dr["JWC_ADD_Number"]),
                        BUY_ADD_AddressID = Convert.ToString(dr["JWC_ADD_Address_ID"]),
                    });
            }
            return BuyerAddList;
        }


        public List<SaleInvoiceRegister_DTO> SISummaryList(DataTable Dt)
        {
            List<SaleInvoiceRegister_DTO> SIList = new List<SaleInvoiceRegister_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleInvoiceRegister_DTO
                    {
                        SI_Number = Convert.ToInt64(dr["SIH_Number"]),
                        SI_InvoiceNo = Convert.ToString(dr["SIH_InvoiceNo"]),
                        SI_InvoiceDate = Convert.ToString(dr["SIH_InvoiceDate"]),
                        SI_SOH_OrderNo = Convert.ToString(dr["SIH_SOH_OrderNo"]),
                        SI_SOH_OrderCount = Convert.ToInt32(dr["SIH_SOH_OrderCount"]),
                        SI_ExportOrder = Convert.ToString(dr["SIH_ExportOrder"]) == "1"?"Yes":"No",
                        SI_BYC_Category = Convert.ToString(dr["SIH_BYC_Category"]),
                        SI_BYG_Group = Convert.ToString(dr["SIH_BYG_Group"]),
                        SI_BUY_Name = Convert.ToString(dr["SIH_BUY_Name"]),
                        SI_MS_Name = Convert.ToString(dr["SIH_MS_Name"]),
                        SI_NoOfItem = Convert.ToString(dr["SIH_NoOfItem"]),
                        SI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SI_CUR_Name = Convert.ToString(dr["SIH_CUR_Name"]),
                        SI_MaterialValue = Convert.ToString(dr["SIH_MaterialCost"]),
                        SI_TotalItemIncome = Convert.ToString(dr["SIH_ItemMiscIncome"]),
                        SI_TotalHeadIncome = Convert.ToString(dr["SIH_HeaderMiscIncome"]),
                        SI_TotalAmount = Convert.ToString(dr["SIH_Amount"]),
                        SI_ExchangeRate = Convert.ToString(dr["SIH_ExchangeRate"]),
                        SI_HeadGST_Amount = Convert.ToString(dr["SIH_GST_Amount"]),
                        SI_ItemGST_Amount = Convert.ToString(dr["SII_GST_Amount"]),
                        SI_InvoiceAmount = Convert.ToString(dr["SIH_InvoiceAmount"]),
                        SI_HeadWHT_Amount = Convert.ToString(dr["SIH_WHT_Amount"]),
                        SI_ItemWHT_Amount = Convert.ToString(dr["SII_WHT_Amount"]),
                        SI_RoundOff = Convert.ToString(dr["SIH_RoundOff"]),
                        SI_BuyerReceivable = Convert.ToString(dr["SIH_BuyerReceivable"])
                    });
            }
            return SIList;
        }
        public List<SaleInvoiceRegister_DTO> SIDetailedList(DataTable Dt)
        {
            List<SaleInvoiceRegister_DTO> SIList = new List<SaleInvoiceRegister_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleInvoiceRegister_DTO
                    {
                        SI_Number = Convert.ToInt64(dr["SIH_Number"]),
                        SI_InvoiceNo = Convert.ToString(dr["SIH_InvoiceNo"]),
                        SI_InvoiceDate = Convert.ToString(dr["SIH_InvoiceDate"]),
                        SI_SOH_OrderNo = Convert.ToString(dr["SIH_SOH_OrderNo"]),
                        SI_SOH_OrderDate = Convert.ToString(dr["SIH_SOH_OrderDate"]),
                        SI_ExportOrder = Convert.ToString(dr["SIH_ExportOrder"]) == "1" ? "Yes" : "No",
                        SI_BYC_Category = Convert.ToString(dr["SIH_BYC_Category"]),
                        SI_BYG_Group = Convert.ToString(dr["SIH_BYG_Group"]),
                        SI_BUY_Name = Convert.ToString(dr["SIH_BUY_Name"]),
                        SI_MS_Name = Convert.ToString(dr["SIH_MS_Name"]),
                        SI_ITM_Group = Convert.ToString(dr["SII_ITM_Group"]),
                        SI_ITM_Code = Convert.ToString(dr["SII_ITM_Code"]),
                        SI_ITM_Description = Convert.ToString(dr["SII_ITM_Description"]),
                        SI_ITM_OuterDia = Convert.ToString(dr["SII_ITM_OuterDia"]),
                        SI_ITM_Thickness = Convert.ToString(dr["SII_ITM_Thickness"]),
                        SI_ITM_Length = Convert.ToString(dr["SII_ITM_Length"]),
                        SI_ITM_MaterialGrade = Convert.ToString(dr["SII_ITM_MaterialGrade"]),
                        SI_WH_Name = Convert.ToString(dr["SII_WH_Name"]),
                        SI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SI_UoM_Name = Convert.ToString(dr["SII_UOM_Name"]),
                        SI_CUR_Name = Convert.ToString(dr["SIH_CUR_Name"]),
                        SI_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SII_UnitPrice"])),
                        SI_MaterialValue = Convert.ToString(dr["SII_Amount"]),
                        SI_TotalItemIncome = Convert.ToString(dr["SII_ItemMiscIncome"]),
                        SI_TotalHeadIncome = Convert.ToString(dr["SIH_HeaderMiscIncome"]),
                        SI_TotalAmount = Convert.ToString(dr["SIH_Amount"]),
                        SI_ExchangeRate = Convert.ToString(dr["SIH_ExchangeRate"]),
                        SI_HeadGST_Amount = Convert.ToString(dr["SIH_GST_Amount"]),
                        SI_ItemGST_Amount = Convert.ToString(dr["SII_GST_Amount"]),
                        SI_InvoiceAmount = Convert.ToString(dr["SIH_InvoiceAmount"]),
                        SI_HeadWHT_Amount = Convert.ToString(dr["SIH_WHT_Amount"]),
                        SI_ItemWHT_Amount = Convert.ToString(dr["SII_WHT_Amount"]),
                        SI_RoundOff = Convert.ToString(dr["SIH_RoundOff"]),
                        SI_BuyerReceivable = Convert.ToString(dr["SIH_BuyerReceivable"])
                    });
            }
            return SIList;
        }




        public List<SIBatch_DTO> SIBatchList(DataTable Dt)
        {
            List<SIBatch_DTO> BList = new List<SIBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new SIBatch_DTO
                    {
                        SII_BCH_Number = Convert.ToInt64(dr["ICB_Number"]),
                        SII_BCH_WH_Number = Convert.ToInt64(dr["ICB_Warehouse_Number"]),
                        SII_BCH_WH_Name = Convert.ToString(dr["SII_BCH_Warehoue"]),
                        SII_BCH_BCH_Number = Convert.ToInt64(dr["ICB_Batch_Number"]),
                        SII_BCH_Date = Convert.ToString(dr["ICB_Batch_Date"]),
                        SII_BCH_No = Convert.ToString(dr["ICB_Batch_No"]),
                        SII_BCH_ITM_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["ICB_ItemQty"]))),
                        SII_BCH_HOLD_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["ICB_HoldQty"]))),
                        SII_BCH_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["ICB_Qty"]))),
                        SII_BCH_UnitPrice = Convert.ToString(MH.DecimalConvertUnit(Convert.ToDouble(dr["ICB_UnitPrice"]))),
                        SII_BCH_Value = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["ICB_Value"]))),
                    });
            }
            return BList;
        }
        public List<SIBatch_DTO> SIBatchOverallList(DataTable Dt)
        {
            List<SIBatch_DTO> BList = new List<SIBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new SIBatch_DTO
                    {
                        SII_BCH_WH_Name = Convert.ToString(dr["SII_BCH_Warehoue"]),
                        SII_BCH_Date = Convert.ToString(dr["ICB_Batch_Date"]),
                        SII_BCH_No = Convert.ToString(dr["ICB_Batch_No"]),
                        SII_BCH_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["ICB_Qty"]))),
                        SII_BCH_UnitPrice = Convert.ToString(MH.DecimalConvertUnit(Convert.ToDouble(dr["ICB_UnitPrice"]))),
                        SII_BCH_Value = Convert.ToString(dr["ICB_Value"]),
                    });
            }
            return BList;
        }
        public List<SIBatch_DTO> SIBatchViewList(DataTable Dt)
        {
            List<SIBatch_DTO> BList = new List<SIBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new SIBatch_DTO
                    {
                        SII_BCH_Date = Convert.ToString(dr["SII_BCH_Date"]),
                        SII_BCH_No = Convert.ToString(dr["SII_BCH_No"]),
                        SII_BCH_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["SII_BCH_Qty"]))),
                        SII_BCH_UnitPrice = Convert.ToString(MH.DecimalConvertUnit(Convert.ToDouble(dr["SII_BCH_UnitPrice"]))),
                        SII_BCH_Value = Convert.ToString(dr["SII_BCH_Value"]),
                    });
            }
            return BList;
        }


        public List<SaleInvoiceHead_DTO> SIHeadEditList(DataTable Dt)
        {
            List<SaleInvoiceHead_DTO> SIList = new List<SaleInvoiceHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleInvoiceHead_DTO
                    {
                        SIH_Number = Convert.ToInt64(dr["SIH_Number"]),
                        SIH_InvoiceNo = Convert.ToString(dr["SIH_InvoiceNo"]),
                        SIH_InvoiceDate = Convert.ToString(dr["SIH_InvoiceDate"]),
                        SIH_ExportOrder = Convert.ToString(dr["SIH_ExportOrder"]) == "1" ? "true" : "false",
                        SIH_BUY_Number = Convert.ToString(dr["SIH_BUY_Number"]),
                        SIH_BUY_Name = Convert.ToString(dr["SIH_BUY_Name"]),
                        SIH_BUY_LOC_Number = Convert.ToString(dr["SIH_BUY_LOC_Number"]),
                        SIH_CUR_Number = Convert.ToString(dr["SIH_CUR_Number"]),
                        SIH_CUR_Name = Convert.ToString(dr["SIH_CUR_Name"]),
                        SIH_CUR_DecimalPlaces = Convert.ToString(dr["SIH_CUR_DecimalPlaces"]),
                        SIH_MS_Number = Convert.ToString(dr["SIH_MS_Number"]),
                        SIH_ExchangeRate = Convert.ToString(dr["SIH_ExchangeRate"]),
                        SIH_TCT_Number = Convert.ToString(dr["SIH_TCT_Number"]),
                        SIH_WHT_Number = Convert.ToString(dr["SIH_WHT_Number"]),
                        SIH_MaterialCost = Convert.ToString(dr["SIH_MaterialCost"]),
                        SIH_ItemMiscIncome = Convert.ToString(dr["SIH_ItemMiscIncome"]),
                        SIH_HeaderMiscIncome = Convert.ToString(dr["SIH_HeaderMiscIncome"]),
                        SIH_InvoiceAmount = Convert.ToString(dr["SIH_InvoiceAmount"]),
                        SIH_GST_Amount = Convert.ToString(dr["SIH_GST_Amount"]),
                        SIH_WHT_Amount = Convert.ToString(dr["SIH_WHT_Amount"]),
                        SIH_RoundOff = Convert.ToString(dr["SIH_RoundOff"]),
                        SIH_BuyerReceivable = Convert.ToString(dr["SIH_BuyerReceivable"]),
                        SIH_Mode = Convert.ToInt32(dr["SIH_Mode"]),
                    });
            }
            return SIList;
        }
        public List<SaleInvoiceItem_DTO> SIItemEditList(DataTable Dt)
        {
            List<SaleInvoiceItem_DTO> SIList = new List<SaleInvoiceItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleInvoiceItem_DTO
                    {
                        SII_Number = Convert.ToInt64(dr["SII_Number"]),
                        SII_SOH_OrderNo = Convert.ToString(dr["SII_SOH_OrderNo"]),
                        SII_MS_Number = Convert.ToInt64(dr["SII_MS_Number"]),
                        SII_ITM_Number = Convert.ToInt64(dr["SII_ITM_Number"]),
                        SII_ITM_Code = Convert.ToString(dr["SII_ITM_Code"]),
                        SII_ITM_Group = Convert.ToString(dr["SII_ITM_Group"]),
                        SII_ITM_Thickness = Convert.ToString(dr["SII_ITM_Thickness"]),
                        SII_ITM_OuterDia = Convert.ToString(dr["SII_ITM_OuterDia"]),
                        SII_ITM_Length = Convert.ToString(dr["SII_ITM_Length"]),
                        SII_ITM_MaterialGrade = Convert.ToString(dr["SII_ITM_MaterialGrade"]),
                        SII_ITM_Description = Convert.ToString(dr["SII_ITM_Description"]),
                        SII_WH_Number = Convert.ToInt64(dr["SII_WH_Number"]),
                        SII_UoM_Number = Convert.ToInt64(dr["SII_UoM_Number"]),
                        SII_OLD_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SII_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SII_SRI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_SRI_Qty"])),
                        SII_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SII_UnitPrice"])),
                        SII_Amount = Convert.ToString(dr["SII_Amount"]),
                        SII_IncomeValue = Convert.ToString(dr["SII_IncomeValue"]),
                        SII_UoM_DecimalPlaces = Convert.ToString(dr["SII_UoM_DecimalPlaces"]),
                        SII_HSN_Number = Convert.ToInt64(dr["SII_HSN_Number"]),
                        SII_GST_Amount = Convert.ToString(dr["SII_GST_Amount"]),
                        SII_WHT_Percent = Convert.ToString(dr["SII_WHT_Percent"]),
                        SII_WHT_Amount = Convert.ToString(dr["SII_WHT_Amount"]),
                        SII_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SaleInvoiceIncome_DTO> SIIncomeEditList(DataTable Dt)
        {
            List<SaleInvoiceIncome_DTO> SIList = new List<SaleInvoiceIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleInvoiceIncome_DTO
                    {
                        SIH_INC_SOH_OrderNo = Convert.ToString(dr["SIH_INC_SOH_OrderNo"]),
                        SIH_INC_Number = Convert.ToInt64(dr["SIH_INC_Number"]),
                        SIH_INC_MIC_Number = Convert.ToString(dr["SIH_INC_MIC_Number"]),
                        SIH_INC_MIC_Description = Convert.ToString(dr["SIH_INC_MIC_Description"]),
                        SIH_INC_Remarks = Convert.ToString(dr["SIH_INC_Remarks"]),
                        SIH_INC_OCRN_Number = Convert.ToString(dr["SIH_INC_OCRN_Number"]),
                        SIH_INC_CM_Number = Convert.ToString(dr["SIH_INC_CM_Number"]),
                        SIH_INC_IncomeBase = Convert.ToString(dr["SIH_INC_IncomeBase"]),
                        SIH_INC_IncomeValue = Convert.ToString(dr["SIH_INC_IncomeValue"]),
                        SIH_INC_ALCT_Number = Convert.ToString(dr["SIH_INC_ALCT_Number"]),
                        SIH_INC_LA_Number = Convert.ToString(dr["SIH_INC_LA_Number"]),
                        SIH_INC_SAC_Number = Convert.ToString(dr["SIH_INC_SAC_Number"]),
                        SIH_INC_CalculateGST = Convert.ToString(dr["SIH_INC_CalculateGST"]),
                        SIH_INC_GST_Amount = Convert.ToString(dr["SIH_INC_GST_Amount"]),
                        SIH_INC_WHT_Percent = Convert.ToString(dr["SIH_INC_WHT_Percent"]),
                        SIH_INC_WHT_Amount = Convert.ToString(dr["SIH_INC_WHT_Amount"]),
                        SIH_INC_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SaleInvoiceIIncome_DTO> SIIIncomeEditList(DataTable Dt)
        {
            List<SaleInvoiceIIncome_DTO> SIList = new List<SaleInvoiceIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleInvoiceIIncome_DTO
                    {
                        SII_INC_Number = Convert.ToInt64(dr["SII_INC_Number"]),
                        SII_INC_SII_Number = Convert.ToInt64(dr["SII_INC_SII_Number"]),
                        SII_INC_MIC_Number = Convert.ToString(dr["SII_INC_MIC_Number"]),
                        SII_INC_MIC_Description = Convert.ToString(dr["SII_INC_MIC_Description"]),
                        SII_INC_Remarks = Convert.ToString(dr["SII_INC_Remarks"]),
                        SII_INC_OCRN_Number = Convert.ToString(dr["SII_INC_OCRN_Number"]),
                        SII_INC_CM_Number = Convert.ToString(dr["SII_INC_CM_Number"]),
                        SII_INC_IncomeBase = Convert.ToString(dr["SII_INC_IncomeBase"]),
                        SII_INC_IncomeValue = Convert.ToString(dr["SII_INC_IncomeValue"]),
                        SII_INC_ALCT_Number = Convert.ToString(dr["SII_INC_ALCT_Number"]),
                        SII_INC_LA_Number = Convert.ToString(dr["SII_INC_LA_Number"]),
                        SII_INC_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SaleInvoiceAddress_DTO> SIHAddressEditList(DataTable Dt)
        {
            List<SaleInvoiceAddress_DTO> SRList = new List<SaleInvoiceAddress_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SaleInvoiceAddress_DTO
                    {
                        SIH_ADD_Number = Convert.ToInt64(dr["SIH_ADD_Number"]),
                        SIH_ADD_ADTP_Number = Convert.ToInt64(dr["SIH_ADD_ADTP_Number"]),
                        SIH_ADD_AddressID = Convert.ToString(dr["SIH_ADD_AddressID"]),
                        SIH_ADD_Address = Convert.ToString(dr["SIH_ADD_Address"]),
                        SIH_ADD_City = Convert.ToString(dr["SIH_ADD_City"]),
                        SIH_ADD_State = Convert.ToString(dr["SIH_ADD_State"]),
                        SIH_ADD_Country = Convert.ToString(dr["SIH_ADD_Country"]),
                        SIH_ADD_Pin = Convert.ToString(dr["SIH_ADD_Pin"]),
                        SIH_ADD_GSTIN = Convert.ToString(dr["SIH_ADD_GSTIN"]),
                        SIH_ADD_IsDeleted = 0
                    });
            }
            return SRList;
        }



        //SO TO Sale Invoice
        public List<SaleOrder_DTO> SOToSIOrder(DataTable Dt)
        {
            List<SaleOrder_DTO> SOList = new List<SaleOrder_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new SaleOrder_DTO
                    {
                        SO_Number = Convert.ToInt64(dr["SOH_Number"]),
                        SO_OrderNo = Convert.ToString(dr["SOH_OrderNo"]),
                        SO_OrderDate = Convert.ToString(dr["SOH_OrderDate"]),
                        SO_TotalAmount = Convert.ToDouble(dr["SOH_TotalAmount"]),
                    });
            }
            return SOList;
        }
        public List<SaleOrderSIItem_DTO> SOToSIOrderItem(DataTable Dt)
        {
            List<SaleOrderSIItem_DTO> POList = new List<SaleOrderSIItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SaleOrderSIItem_DTO
                    {
                        SOI_Number = Convert.ToInt64(dr["SOI_Number"]),
                        SOH_OrderNo = Convert.ToString(dr["SOH_OrderNo"]),
                        SOH_Number = Convert.ToInt64(dr["SOH_Number"]),
                        SOI_MS_Number = Convert.ToInt64(dr["SOI_MS_Number"]),
                        SOI_ITM_Number = Convert.ToString(dr["SOI_ITM_Number"]),
                        SOI_UoM_Number = Convert.ToString(dr["SOI_UoM_Number"]),
                        SOI_WH_Number = Convert.ToInt64(dr["SOI_WH_Number"]),
                        SOI_UoM_DecimalPlaces = Convert.ToString(dr["SOI_DecimalPlaces"]),
                        soI_Origin_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_Origin_Qty"])),
                        SOI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_Qty"])),
                        SOI_HSN = Convert.ToString(dr["SOI_HSN"]),
                        SOI_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SOI_UnitPrice"])),
                        SOI_Amount = Convert.ToString(dr["SOI_Amount"]),
                        SOI_ITM_Group = Convert.ToString(dr["SOI_ITM_Group"]),
                        SOI_ITM_Code = Convert.ToString(dr["SOI_ITM_Code"]),
                        SOI_ITM_Description = Convert.ToString(dr["SOI_ITM_Description"]),
                        SOI_ITM_OuterDia = Convert.ToString(dr["SOI_ITM_OuterDia"]),
                        SOI_ITM_MaterialGrade = Convert.ToString(dr["SOI_ITM_MaterialGrade"]),
                        SOI_ITM_Length = Convert.ToString(dr["SOI_ITM_Length"]),
                        SOI_ITM_Thickness = Convert.ToString(dr["SOI_ITM_Thickness"]),
                    });
            }
            return POList;
        }
        public List<SaleOrderSIIncome_DTO> SOToSIIncome(DataTable Dt)
        {
            List<SaleOrderSIIncome_DTO> POList = new List<SaleOrderSIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SaleOrderSIIncome_DTO
                    {
                        SOH_INC_SOH_Number = Convert.ToInt64(dr["SOH_INC_SOH_Number"]),
                        SOH_INC_Number = Convert.ToInt64(dr["SOH_INC_Number"]),
                        SOH_INC_SOH_OrderNo = Convert.ToString(dr["SOH_INC_SOH_OrderNo"]),
                        SOH_INC_MIC_Number = Convert.ToString(dr["SOH_INC_MIC_Number"]),
                        SOH_INC_Description = Convert.ToString(dr["SOH_INC_Description"]),
                        SOH_INC_Remarks = Convert.ToString(dr["SOH_INC_Remarks"]),
                        SOH_INC_OCRN_Number = Convert.ToString(dr["SOH_INC_OCRN_Number"]),
                        SOH_INC_CM_Number = Convert.ToString(dr["SOH_INC_CM_Number"]),
                        SOH_INC_IncomeBase = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SOH_INC_IncomeBase"]))),
                        SOH_INC_IncomeValue = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SOH_INC_IncomeValue"]))),
                        SOH_INC_ALCT_Number = Convert.ToString(dr["SOH_INC_ALCT_Number"]),
                        SOH_INC_LA_Number = Convert.ToString(dr["SOH_INC_LA_Number"]),
                        SOH_INC_SAC_Number = Convert.ToString(dr["SOH_INC_SAC_Number"]),
                    });
            }
            return POList;
        }
        public List<SaleOrderSIIIncome_DTO> SOToSIIIncome(DataTable Dt)
        {
            List<SaleOrderSIIIncome_DTO> POList = new List<SaleOrderSIIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SaleOrderSIIIncome_DTO
                    {
                        SOI_INC_Number = Convert.ToInt64(dr["SOI_INC_Number"]),
                        SOI_INC_SOI_Number = Convert.ToInt64(dr["SOI_INC_SOI_Number"]),
                        SOI_INC_SOH_Number = Convert.ToInt64(dr["SOI_INC_SOH_Number"]),
                        SOI_INC_MIC_Number = Convert.ToString(dr["SOI_INC_MIC_Number"]),
                        SOI_INC_Description = Convert.ToString(dr["SOI_INC_Description"]),
                        SOI_INC_Remarks = Convert.ToString(dr["SOI_INC_Remarks"]),
                        SOI_INC_OCRN_Number = Convert.ToString(dr["SOI_INC_OCRN_Number"]),
                        SOI_INC_CM_Number = Convert.ToString(dr["SOI_INC_CM_Number"]),
                        SOI_INC_IncomeBase = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SOI_INC_IncomeBase"]))),
                        SOI_INC_IncomeValue = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SOI_INC_IncomeValue"]))),
                        SOI_INC_ALCT_Number = Convert.ToString(dr["SOI_INC_ALCT_Number"]),
                        SOI_INC_LA_Number = Convert.ToString(dr["SOI_INC_LA_Number"]),
                    });
            }
            return POList;
        }


        public List<SOToSaleInvoiceHead_DTO> SOToSIHeadEditList(DataTable Dt)
        {
            List<SOToSaleInvoiceHead_DTO> SIList = new List<SOToSaleInvoiceHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SOToSaleInvoiceHead_DTO
                    {
                        SIH_Number = Convert.ToInt64(dr["SIH_Number"]),
                        SIH_InvoiceNo = Convert.ToString(dr["SIH_InvoiceNo"]),
                        SIH_InvoiceDate = Convert.ToString(dr["SIH_InvoiceDate"]),
                        SIH_ExportOrder = Convert.ToString(dr["SIH_ExportOrder"]) == "1" ? "true" : "false",
                        SIH_BUY_Number = Convert.ToString(dr["SIH_BUY_Number"]),
                        SIH_BUY_Name = Convert.ToString(dr["SIH_BUY_Name"]),
                        SIH_BUY_LOC_Number = Convert.ToString(dr["SIH_BUY_LOC_Number"]),
                        SIH_CUR_Number = Convert.ToString(dr["SIH_CUR_Number"]),
                        SIH_CUR_Name = Convert.ToString(dr["SIH_CUR_Name"]),
                        SIH_CUR_DecimalPlaces = Convert.ToString(dr["SIH_CUR_DecimalPlaces"]),
                        SIH_MS_Number = Convert.ToString(dr["SIH_MS_Number"]),
                        SIH_ExchangeRate = Convert.ToString(dr["SIH_ExchangeRate"]),
                        SIH_TCT_Number = Convert.ToString(dr["SIH_TCT_Number"]),
                        SIH_WHT_Number = Convert.ToString(dr["SIH_WHT_Number"]),
                        SIH_MaterialCost = Convert.ToString(dr["SIH_MaterialCost"]),
                        SIH_ItemMiscIncome = Convert.ToString(dr["SIH_ItemMiscIncome"]),
                        SIH_HeaderMiscIncome = Convert.ToString(dr["SIH_HeaderMiscIncome"]),
                        SIH_InvoiceAmount = Convert.ToString(dr["SIH_InvoiceAmount"]),
                        SIH_GST_Amount = Convert.ToString(dr["SIH_GST_Amount"]),
                        SIH_WHT_Amount = Convert.ToString(dr["SIH_WHT_Amount"]),
                        SIH_RoundOff = Convert.ToString(dr["SIH_RoundOff"]),
                        SIH_BuyerReceivable = Convert.ToString(dr["SIH_BuyerReceivable"]),
                        SIH_Mode = Convert.ToInt32(dr["SIH_Mode"]),
                    });
            }
            return SIList;
        }
        public List<SOToSaleInvoiceItem_DTO> SOToSIItemEditList(DataTable Dt)
        {
            List<SOToSaleInvoiceItem_DTO> SIList = new List<SOToSaleInvoiceItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SOToSaleInvoiceItem_DTO
                    {
                        SII_Number = Convert.ToInt64(dr["SII_Number"]),
                        SII_SOH_Number = Convert.ToInt64(dr["SII_SOH_Number"]),
                        SII_SOI_Number = Convert.ToInt64(dr["SII_SOI_Number"]),
                        SII_SOH_OrderNo = Convert.ToString(dr["SII_SOH_OrderNo"]),
                        SII_MS_Number = Convert.ToInt64(dr["SII_MS_Number"]),
                        SII_ITM_Number = Convert.ToInt64(dr["SII_ITM_Number"]),
                        SII_ITM_Code = Convert.ToString(dr["SII_ITM_Code"]),
                        SII_ITM_Group = Convert.ToString(dr["SII_ITM_Group"]),
                        SII_ITM_Thickness = Convert.ToString(dr["SII_ITM_Thickness"]),
                        SII_ITM_OuterDia = Convert.ToString(dr["SII_ITM_OuterDia"]),
                        SII_ITM_Length = Convert.ToString(dr["SII_ITM_Length"]),
                        SII_ITM_MaterialGrade = Convert.ToString(dr["SII_ITM_MaterialGrade"]),
                        SII_ITM_Description = Convert.ToString(dr["SII_ITM_Description"]),
                        SII_WH_Number = Convert.ToInt64(dr["SII_WH_Number"]),
                        SII_UoM_Number = Convert.ToInt64(dr["SII_UoM_Number"]),
                        SII_SOI_Origin_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_SOI_Origin_Qty"])),
                        SII_SOI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_SOI_Qty"])),
                        SII_OLD_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SII_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SII_SRI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_SRI_Qty"])),
                        SII_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SII_UnitPrice"])),
                        SII_Amount = Convert.ToString(dr["SII_Amount"]),
                        SII_IncomeValue = Convert.ToString(dr["SII_IncomeValue"]),
                        SII_UoM_DecimalPlaces = Convert.ToString(dr["SII_UoM_DecimalPlaces"]),
                        SII_HSN_Number = Convert.ToInt64(dr["SII_HSN_Number"]),
                        SII_GST_Amount = Convert.ToString(dr["SII_GST_Amount"]),
                        SII_WHT_Percent = Convert.ToString(dr["SII_WHT_Percent"]),
                        SII_WHT_Amount = Convert.ToString(dr["SII_WHT_Amount"]),
                        SII_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SOToSaleInvoiceIncome_DTO> SOToSIIncomeEditList(DataTable Dt)
        {
            List<SOToSaleInvoiceIncome_DTO> SIList = new List<SOToSaleInvoiceIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SOToSaleInvoiceIncome_DTO
                    {
                        SIH_INC_SOH_OrderNo = Convert.ToString(dr["SIH_INC_SOH_OrderNo"]),
                        SIH_INC_Number = Convert.ToInt64(dr["SIH_INC_Number"]),
                        SIH_INC_SOH_INC_Number = Convert.ToInt64(dr["SIH_INC_SOH_INC_Number"]),
                        SIH_INC_SOH_Number = Convert.ToInt64(dr["SIH_INC_SOH_Number"]),
                        SIH_INC_MIC_Number = Convert.ToString(dr["SIH_INC_MIC_Number"]),
                        SIH_INC_MIC_Description = Convert.ToString(dr["SIH_INC_MIC_Description"]),
                        SIH_INC_Remarks = Convert.ToString(dr["SIH_INC_Remarks"]),
                        SIH_INC_OCRN_Number = Convert.ToString(dr["SIH_INC_OCRN_Number"]),
                        SIH_INC_CM_Number = Convert.ToString(dr["SIH_INC_CM_Number"]),
                        SIH_INC_IncomeBase = Convert.ToString(dr["SIH_INC_IncomeBase"]),
                        SIH_INC_IncomeValue = Convert.ToString(dr["SIH_INC_IncomeValue"]),
                        SIH_INC_ALCT_Number = Convert.ToString(dr["SIH_INC_ALCT_Number"]),
                        SIH_INC_LA_Number = Convert.ToString(dr["SIH_INC_LA_Number"]),
                        SIH_INC_SAC_Number = Convert.ToString(dr["SIH_INC_SAC_Number"]),
                        SIH_INC_CalculateGST = Convert.ToString(dr["SIH_INC_CalculateGST"]),
                        SIH_INC_GST_Amount = Convert.ToString(dr["SIH_INC_GST_Amount"]),
                        SIH_INC_WHT_Percent = Convert.ToString(dr["SIH_INC_WHT_Percent"]),
                        SIH_INC_WHT_Amount = Convert.ToString(dr["SIH_INC_WHT_Amount"]),
                        SIH_INC_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SOToSaleInvoiceIIncome_DTO> SOToSIIIncomeEditList(DataTable Dt)
        {
            List<SOToSaleInvoiceIIncome_DTO> SIList = new List<SOToSaleInvoiceIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SOToSaleInvoiceIIncome_DTO
                    {
                        SII_INC_Number = Convert.ToInt64(dr["SII_INC_Number"]),
                        SII_INC_SOI_Number = Convert.ToInt64(dr["SII_INC_SOI_Number"]),
                        SII_INC_SOH_Number = Convert.ToInt64(dr["SII_INC_SOH_Number"]),
                        SII_INC_SOI_INC_Number = Convert.ToInt64(dr["SII_INC_SOI_INC_Number"]),
                        SII_INC_SII_Number = Convert.ToInt64(dr["SII_INC_SII_Number"]),
                        SII_INC_MIC_Number = Convert.ToString(dr["SII_INC_MIC_Number"]),
                        SII_INC_MIC_Description = Convert.ToString(dr["SII_INC_MIC_Description"]),
                        SII_INC_Remarks = Convert.ToString(dr["SII_INC_Remarks"]),
                        SII_INC_OCRN_Number = Convert.ToString(dr["SII_INC_OCRN_Number"]),
                        SII_INC_CM_Number = Convert.ToString(dr["SII_INC_CM_Number"]),
                        SII_INC_IncomeBase = Convert.ToString(dr["SII_INC_IncomeBase"]),
                        SII_INC_IncomeValue = Convert.ToString(dr["SII_INC_IncomeValue"]),
                        SII_INC_ALCT_Number = Convert.ToString(dr["SII_INC_ALCT_Number"]),
                        SII_INC_LA_Number = Convert.ToString(dr["SII_INC_LA_Number"]),
                        SII_INC_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SOToSaleInvoiceAddress_DTO> SOToSIHAddressEditList(DataTable Dt)
        {
            List<SOToSaleInvoiceAddress_DTO> SRList = new List<SOToSaleInvoiceAddress_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SOToSaleInvoiceAddress_DTO
                    {
                        SIH_ADD_Number = Convert.ToInt64(dr["SIH_ADD_Number"]),
                        SIH_ADD_ADTP_Number = Convert.ToInt64(dr["SIH_ADD_ADTP_Number"]),
                        SIH_ADD_AddressID = Convert.ToString(dr["SIH_ADD_AddressID"]),
                        SIH_ADD_Address = Convert.ToString(dr["SIH_ADD_Address"]),
                        SIH_ADD_City = Convert.ToString(dr["SIH_ADD_City"]),
                        SIH_ADD_State = Convert.ToString(dr["SIH_ADD_State"]),
                        SIH_ADD_Country = Convert.ToString(dr["SIH_ADD_Country"]),
                        SIH_ADD_Pin = Convert.ToString(dr["SIH_ADD_Pin"]),
                        SIH_ADD_GSTIN = Convert.ToString(dr["SIH_ADD_GSTIN"]),
                        SIH_ADD_IsDeleted = 0
                    });
            }
            return SRList;
        }




        //SO Item TO Sale Invoice
        public List<SIItemSaleOrderItem_DTO> SOItemToSIOrder(DataTable Dt)
        {
            List<SIItemSaleOrderItem_DTO> POList = new List<SIItemSaleOrderItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SIItemSaleOrderItem_DTO
                    {
                        SOI_ITM_Number = Convert.ToString(dr["ItemNumber"]),
                        SOI_ITM_Code = Convert.ToString(dr["ItemCode"]),
                        SOI_ITM_Description = Convert.ToString(dr["ItemDescription"]),
                        SOI_ITM_Group = Convert.ToString(dr["ItemGroup"]),
                    });
            }
            return POList;
        }
        public List<SIItemSaleOrderItem_DTO> SOItemToSIOrderItem(DataTable Dt)
        {
            List<SIItemSaleOrderItem_DTO> POList = new List<SIItemSaleOrderItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SIItemSaleOrderItem_DTO
                    {
                        SOI_Number = Convert.ToInt64(dr["SOI_Number"]),
                        SOH_OrderNo = Convert.ToString(dr["SOH_OrderNo"]),
                        SOH_Number = Convert.ToInt64(dr["SOH_Number"]),
                        SOI_MS_Number = Convert.ToInt64(dr["SOI_MS_Number"]),
                        SOI_ITM_Number = Convert.ToString(dr["SOI_ITM_Number"]),
                        SOI_UoM_Number = Convert.ToString(dr["SOI_UoM_Number"]),
                        SOI_WH_Number = Convert.ToInt64(dr["SOI_WH_Number"]),
                        SOI_UoM_DecimalPlaces = Convert.ToString(dr["SOI_DecimalPlaces"]),
                        soI_Origin_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_Origin_Qty"])),
                        SOI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_Qty"])),
                        SOI_HSN = Convert.ToString(dr["SOI_HSN"]),
                        SOI_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SOI_UnitPrice"])),
                        SOI_Amount = Convert.ToString(dr["SOI_Amount"]),
                        SOI_ITM_Group = Convert.ToString(dr["SOI_ITM_Group"]),
                        SOI_ITM_Code = Convert.ToString(dr["SOI_ITM_Code"]),
                        SOI_ITM_Description = Convert.ToString(dr["SOI_ITM_Description"]),
                        SOI_ITM_OuterDia = Convert.ToString(dr["SOI_ITM_OuterDia"]),
                        SOI_ITM_MaterialGrade = Convert.ToString(dr["SOI_ITM_MaterialGrade"]),
                        SOI_ITM_Length = Convert.ToString(dr["SOI_ITM_Length"]),
                        SOI_ITM_Thickness = Convert.ToString(dr["SOI_ITM_Thickness"]),
                    });
            }
            return POList;
        }
        public List<SIItemSaleOrderIncome_DTO> SOItemToSIIncome(DataTable Dt)
        {
            List<SIItemSaleOrderIncome_DTO> POList = new List<SIItemSaleOrderIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SIItemSaleOrderIncome_DTO
                    {
                        SOH_INC_SOH_Number = Convert.ToInt64(dr["SOH_INC_SOH_Number"]),
                        SOH_INC_Number = Convert.ToInt64(dr["SOH_INC_Number"]),
                        SOH_INC_SOH_OrderNo = Convert.ToString(dr["SOH_INC_SOH_OrderNo"]),
                        SOH_INC_MIC_Number = Convert.ToString(dr["SOH_INC_MIC_Number"]),
                        SOH_INC_Description = Convert.ToString(dr["SOH_INC_Description"]),
                        SOH_INC_Remarks = Convert.ToString(dr["SOH_INC_Remarks"]),
                        SOH_INC_OCRN_Number = Convert.ToString(dr["SOH_INC_OCRN_Number"]),
                        SOH_INC_CM_Number = Convert.ToString(dr["SOH_INC_CM_Number"]),
                        SOH_INC_IncomeBase = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SOH_INC_IncomeBase"]))),
                        SOH_INC_IncomeValue = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SOH_INC_IncomeValue"]))),
                        SOH_INC_ALCT_Number = Convert.ToString(dr["SOH_INC_ALCT_Number"]),
                        SOH_INC_LA_Number = Convert.ToString(dr["SOH_INC_LA_Number"]),
                        SOH_INC_SAC_Number = Convert.ToString(dr["SOH_INC_SAC_Number"]),
                    });
            }
            return POList;
        }
        public List<SIItemSaleOrderIIncome_DTO> SOItemToSIIIncome(DataTable Dt)
        {
            List<SIItemSaleOrderIIncome_DTO> POList = new List<SIItemSaleOrderIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SIItemSaleOrderIIncome_DTO
                    {
                        SOI_INC_Number = Convert.ToInt64(dr["SOI_INC_Number"]),
                        SOI_INC_SOI_Number = Convert.ToInt64(dr["SOI_INC_SOI_Number"]),
                        SOI_INC_SOH_Number = Convert.ToInt64(dr["SOI_INC_SOH_Number"]),
                        SOI_INC_MIC_Number = Convert.ToString(dr["SOI_INC_MIC_Number"]),
                        SOI_INC_Description = Convert.ToString(dr["SOI_INC_Description"]),
                        SOI_INC_Remarks = Convert.ToString(dr["SOI_INC_Remarks"]),
                        SOI_INC_OCRN_Number = Convert.ToString(dr["SOI_INC_OCRN_Number"]),
                        SOI_INC_CM_Number = Convert.ToString(dr["SOI_INC_CM_Number"]),
                        SOI_INC_IncomeBase = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SOI_INC_IncomeBase"]))),
                        SOI_INC_IncomeValue = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SOI_INC_IncomeValue"]))),
                        SOI_INC_ALCT_Number = Convert.ToString(dr["SOI_INC_ALCT_Number"]),
                        SOI_INC_LA_Number = Convert.ToString(dr["SOI_INC_LA_Number"]),
                    });
            }
            return POList;
        }


        public List<SOItemToSaleInvoiceHead_DTO> SOItemToSIHeadEditList(DataTable Dt)
        {
            List<SOItemToSaleInvoiceHead_DTO> SIList = new List<SOItemToSaleInvoiceHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SOItemToSaleInvoiceHead_DTO
                    {
                        SIH_Number = Convert.ToInt64(dr["SIH_Number"]),
                        SIH_InvoiceNo = Convert.ToString(dr["SIH_InvoiceNo"]),
                        SIH_InvoiceDate = Convert.ToString(dr["SIH_InvoiceDate"]),
                        SIH_ExportOrder = Convert.ToString(dr["SIH_ExportOrder"]) == "1" ? "true" : "false",
                        SIH_BUY_Number = Convert.ToString(dr["SIH_BUY_Number"]),
                        SIH_BUY_Name = Convert.ToString(dr["SIH_BUY_Name"]),
                        SIH_BUY_LOC_Number = Convert.ToString(dr["SIH_BUY_LOC_Number"]),
                        SIH_CUR_Number = Convert.ToString(dr["SIH_CUR_Number"]),
                        SIH_CUR_Name = Convert.ToString(dr["SIH_CUR_Name"]),
                        SIH_CUR_DecimalPlaces = Convert.ToString(dr["SIH_CUR_DecimalPlaces"]),
                        SIH_MS_Number = Convert.ToString(dr["SIH_MS_Number"]),
                        SIH_ExchangeRate = Convert.ToString(dr["SIH_ExchangeRate"]),
                        SIH_TCT_Number = Convert.ToString(dr["SIH_TCT_Number"]),
                        SIH_WHT_Number = Convert.ToString(dr["SIH_WHT_Number"]),
                        SIH_MaterialCost = Convert.ToString(dr["SIH_MaterialCost"]),
                        SIH_ItemMiscIncome = Convert.ToString(dr["SIH_ItemMiscIncome"]),
                        SIH_HeaderMiscIncome = Convert.ToString(dr["SIH_HeaderMiscIncome"]),
                        SIH_InvoiceAmount = Convert.ToString(dr["SIH_InvoiceAmount"]),
                        SIH_GST_Amount = Convert.ToString(dr["SIH_GST_Amount"]),
                        SIH_WHT_Amount = Convert.ToString(dr["SIH_WHT_Amount"]),
                        SIH_RoundOff = Convert.ToString(dr["SIH_RoundOff"]),
                        SIH_BuyerReceivable = Convert.ToString(dr["SIH_BuyerReceivable"]),
                        SIH_Mode = Convert.ToInt32(dr["SIH_Mode"]),
                    });
            }
            return SIList;
        }
        public List<SOItemToSaleInvoiceItem_DTO> SOItemToSIItemEditList(DataTable Dt)
        {
            List<SOItemToSaleInvoiceItem_DTO> SIList = new List<SOItemToSaleInvoiceItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SOItemToSaleInvoiceItem_DTO
                    {
                        SII_Number = Convert.ToInt64(dr["SII_Number"]),
                        SII_SOH_Number = Convert.ToInt64(dr["SII_SOH_Number"]),
                        SII_SOI_Number = Convert.ToInt64(dr["SII_SOI_Number"]),
                        SII_SOH_OrderNo = Convert.ToString(dr["SII_SOH_OrderNo"]),
                        SII_MS_Number = Convert.ToInt64(dr["SII_MS_Number"]),
                        SII_ITM_Number = Convert.ToInt64(dr["SII_ITM_Number"]),
                        SII_ITM_Code = Convert.ToString(dr["SII_ITM_Code"]),
                        SII_ITM_Group = Convert.ToString(dr["SII_ITM_Group"]),
                        SII_ITM_Thickness = Convert.ToString(dr["SII_ITM_Thickness"]),
                        SII_ITM_OuterDia = Convert.ToString(dr["SII_ITM_OuterDia"]),
                        SII_ITM_Length = Convert.ToString(dr["SII_ITM_Length"]),
                        SII_ITM_MaterialGrade = Convert.ToString(dr["SII_ITM_MaterialGrade"]),
                        SII_ITM_Description = Convert.ToString(dr["SII_ITM_Description"]),
                        SII_WH_Number = Convert.ToInt64(dr["SII_WH_Number"]),
                        SII_UoM_Number = Convert.ToInt64(dr["SII_UoM_Number"]),
                        SII_SOI_Origin_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_SOI_Origin_Qty"])),
                        SII_SOI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_SOI_Qty"])),
                        SII_OLD_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SII_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SII_SRI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_SRI_Qty"])),
                        SII_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SII_UnitPrice"])),
                        SII_Amount = Convert.ToString(dr["SII_Amount"]),
                        SII_IncomeValue = Convert.ToString(dr["SII_IncomeValue"]),
                        SII_UoM_DecimalPlaces = Convert.ToString(dr["SII_UoM_DecimalPlaces"]),
                        SII_HSN_Number = Convert.ToInt64(dr["SII_HSN_Number"]),
                        SII_GST_Amount = Convert.ToString(dr["SII_GST_Amount"]),
                        SII_WHT_Percent = Convert.ToString(dr["SII_WHT_Percent"]),
                        SII_WHT_Amount = Convert.ToString(dr["SII_WHT_Amount"]),
                        SII_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SOItemToSaleInvoiceIncome_DTO> SOItemToSIIncomeEditList(DataTable Dt)
        {
            List<SOItemToSaleInvoiceIncome_DTO> SIList = new List<SOItemToSaleInvoiceIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SOItemToSaleInvoiceIncome_DTO
                    {
                        SIH_INC_SOH_OrderNo = Convert.ToString(dr["SIH_INC_SOH_OrderNo"]),
                        SIH_INC_Number = Convert.ToInt64(dr["SIH_INC_Number"]),
                        SIH_INC_SOH_INC_Number = Convert.ToInt64(dr["SIH_INC_SOH_INC_Number"]),
                        SIH_INC_SOH_Number = Convert.ToInt64(dr["SIH_INC_SOH_Number"]),
                        SIH_INC_MIC_Number = Convert.ToString(dr["SIH_INC_MIC_Number"]),
                        SIH_INC_MIC_Description = Convert.ToString(dr["SIH_INC_MIC_Description"]),
                        SIH_INC_Remarks = Convert.ToString(dr["SIH_INC_Remarks"]),
                        SIH_INC_OCRN_Number = Convert.ToString(dr["SIH_INC_OCRN_Number"]),
                        SIH_INC_CM_Number = Convert.ToString(dr["SIH_INC_CM_Number"]),
                        SIH_INC_IncomeBase = Convert.ToString(dr["SIH_INC_IncomeBase"]),
                        SIH_INC_IncomeValue = Convert.ToString(dr["SIH_INC_IncomeValue"]),
                        SIH_INC_ALCT_Number = Convert.ToString(dr["SIH_INC_ALCT_Number"]),
                        SIH_INC_LA_Number = Convert.ToString(dr["SIH_INC_LA_Number"]),
                        SIH_INC_SAC_Number = Convert.ToString(dr["SIH_INC_SAC_Number"]),
                        SIH_INC_CalculateGST = Convert.ToString(dr["SIH_INC_CalculateGST"]),
                        SIH_INC_GST_Amount = Convert.ToString(dr["SIH_INC_GST_Amount"]),
                        SIH_INC_WHT_Percent = Convert.ToString(dr["SIH_INC_WHT_Percent"]),
                        SIH_INC_WHT_Amount = Convert.ToString(dr["SIH_INC_WHT_Amount"]),
                        SIH_INC_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SOItemToSaleInvoiceIIncome_DTO> SOItemToSIIIncomeEditList(DataTable Dt)
        {
            List<SOItemToSaleInvoiceIIncome_DTO> SIList = new List<SOItemToSaleInvoiceIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SOItemToSaleInvoiceIIncome_DTO
                    {
                        SII_INC_Number = Convert.ToInt64(dr["SII_INC_Number"]),
                        SII_INC_SOI_Number = Convert.ToInt64(dr["SII_INC_SOI_Number"]),
                        SII_INC_SOH_Number = Convert.ToInt64(dr["SII_INC_SOH_Number"]),
                        SII_INC_SOI_INC_Number = Convert.ToInt64(dr["SII_INC_SOI_INC_Number"]),
                        SII_INC_SII_Number = Convert.ToInt64(dr["SII_INC_SII_Number"]),
                        SII_INC_MIC_Number = Convert.ToString(dr["SII_INC_MIC_Number"]),
                        SII_INC_MIC_Description = Convert.ToString(dr["SII_INC_MIC_Description"]),
                        SII_INC_Remarks = Convert.ToString(dr["SII_INC_Remarks"]),
                        SII_INC_OCRN_Number = Convert.ToString(dr["SII_INC_OCRN_Number"]),
                        SII_INC_CM_Number = Convert.ToString(dr["SII_INC_CM_Number"]),
                        SII_INC_IncomeBase = Convert.ToString(dr["SII_INC_IncomeBase"]),
                        SII_INC_IncomeValue = Convert.ToString(dr["SII_INC_IncomeValue"]),
                        SII_INC_ALCT_Number = Convert.ToString(dr["SII_INC_ALCT_Number"]),
                        SII_INC_LA_Number = Convert.ToString(dr["SII_INC_LA_Number"]),
                        SII_INC_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SOItemToSaleInvoiceAddress_DTO> SOItemToSIHAddressEditList(DataTable Dt)
        {
            List<SOItemToSaleInvoiceAddress_DTO> SRList = new List<SOItemToSaleInvoiceAddress_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SOItemToSaleInvoiceAddress_DTO
                    {
                        SIH_ADD_Number = Convert.ToInt64(dr["SIH_ADD_Number"]),
                        SIH_ADD_ADTP_Number = Convert.ToInt64(dr["SIH_ADD_ADTP_Number"]),
                        SIH_ADD_AddressID = Convert.ToString(dr["SIH_ADD_AddressID"]),
                        SIH_ADD_Address = Convert.ToString(dr["SIH_ADD_Address"]),
                        SIH_ADD_City = Convert.ToString(dr["SIH_ADD_City"]),
                        SIH_ADD_State = Convert.ToString(dr["SIH_ADD_State"]),
                        SIH_ADD_Country = Convert.ToString(dr["SIH_ADD_Country"]),
                        SIH_ADD_Pin = Convert.ToString(dr["SIH_ADD_Pin"]),
                        SIH_ADD_GSTIN = Convert.ToString(dr["SIH_ADD_GSTIN"]),
                        SIH_ADD_IsDeleted = 0
                    });
            }
            return SRList;
        }
    }
}

using ERP_DAO;
using ERP_DTO;
using System;
using System.Data;
using System.Globalization;

namespace ERP.DataList
{
    public class SaleRejection_DL
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
        public List<SaleRejectionGst> SaleInvGst(DataTable Dt)
        {
            List<SaleRejectionGst> IList = new List<SaleRejectionGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new SaleRejectionGst
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
        public List<SaleRejectionGst> SaleInvGstView(DataTable Dt)
        {
            List<SaleRejectionGst> IList = new List<SaleRejectionGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new SaleRejectionGst
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
        public List<SaleRejectionWHT> SaleInvWHT(DataTable Dt)
        {
            List<SaleRejectionWHT> IList = new List<SaleRejectionWHT>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new SaleRejectionWHT
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


        public List<SaleRejectionRegister_DTO> SRSummaryList(DataTable Dt)
        {
            List<SaleRejectionRegister_DTO> SIList = new List<SaleRejectionRegister_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleRejectionRegister_DTO
                    {
                        SR_Number = Convert.ToInt64(dr["SRH_Number"]),
                        SR_RejectionNo = Convert.ToString(dr["SRH_RejectionNo"]),
                        SR_RejectionDate = Convert.ToString(dr["SRH_RejectionDate"]),
                        SR_SIH_InvoiceNo = Convert.ToString(dr["SRH_SIH_InvoiceNo"]),
                        SR_SIH_InvoiceCount = Convert.ToInt32(dr["SRH_SIH_InvoiceCount"]),
                        SR_ExportOrder = Convert.ToString(dr["SRH_ExportOrder"]) == "1"?"Yes":"No",
                        SR_BYC_Category = Convert.ToString(dr["SRH_BYC_Category"]),
                        SR_BYG_Group = Convert.ToString(dr["SRH_BYG_Group"]),
                        SR_BUY_Name = Convert.ToString(dr["SRH_BUY_Name"]),
                        SR_MS_Name = Convert.ToString(dr["SRH_MS_Name"]),
                        SR_NoOfItem = Convert.ToString(dr["SRH_NoOfItem"]),
                        SR_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_Qty"])),
                        SR_CUR_Name = Convert.ToString(dr["SRH_CUR_Name"]),
                        SR_MaterialValue = Convert.ToString(dr["SRH_MaterialCost"]),
                        SR_TotalItemIncome = Convert.ToString(dr["SRH_ItemMiscIncome"]),
                        SR_TotalHeadIncome = Convert.ToString(dr["SRH_HeaderMiscIncome"]),
                        SR_TotalAmount = Convert.ToString(dr["SRH_Amount"]),
                        SR_ExchangeRate = Convert.ToString(dr["SRH_ExchangeRate"]),
                        SR_HeadGST_Amount = Convert.ToString(dr["SRH_GST_Amount"]),
                        SR_ItemGST_Amount = Convert.ToString(dr["SRI_GST_Amount"]),
                        SR_RejectionAmount = Convert.ToString(dr["SRH_RejectionAmount"]),
                        SR_HeadWHT_Amount = Convert.ToString(dr["SRH_WHT_Amount"]),
                        SR_ItemWHT_Amount = Convert.ToString(dr["SRI_WHT_Amount"]),
                        SR_RoundOff = Convert.ToString(dr["SRH_RoundOff"]),
                        SR_BuyerReceivable = Convert.ToString(dr["SRH_BuyerReceivable"])
                    });
            }
            return SIList;
        }
        public List<SaleRejectionRegister_DTO> SRDetailedList(DataTable Dt)
        {
            List<SaleRejectionRegister_DTO> SIList = new List<SaleRejectionRegister_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleRejectionRegister_DTO
                    {
                        SR_Number = Convert.ToInt64(dr["SRH_Number"]),
                        SR_RejectionNo = Convert.ToString(dr["SRH_RejectionNo"]),
                        SR_RejectionDate = Convert.ToString(dr["SRH_RejectionDate"]),
                        //SR_SOH_OrderNo = Convert.ToString(dr["SRH_SOH_OrderNo"]),
                        //SR_SOH_OrderDate = Convert.ToString(dr["SRH_SOH_OrderDate"]),
                        SR_SIH_InvoiceNo = Convert.ToString(dr["SRH_SIH_InvoiceNo"]),
                        SR_SIH_InvoiceDate = Convert.ToString(dr["SRH_SIH_InvoiceDate"]),
                        SR_ExportOrder = Convert.ToString(dr["SRH_ExportOrder"]) == "1" ? "Yes" : "No",
                        SR_BYC_Category = Convert.ToString(dr["SRH_BYC_Category"]),
                        SR_BYG_Group = Convert.ToString(dr["SRH_BYG_Group"]),
                        SR_BUY_Name = Convert.ToString(dr["SRH_BUY_Name"]),
                        SR_MS_Name = Convert.ToString(dr["SRH_MS_Name"]),
                        SR_ITM_Group = Convert.ToString(dr["SRI_ITM_Group"]),
                        SR_ITM_Code = Convert.ToString(dr["SRI_ITM_Code"]),
                        SR_ITM_Description = Convert.ToString(dr["SRI_ITM_Description"]),
                        SR_ITM_OuterDia = Convert.ToString(dr["SRI_ITM_OuterDia"]),
                        SR_ITM_Thickness = Convert.ToString(dr["SRI_ITM_Thickness"]),
                        SR_ITM_Length = Convert.ToString(dr["SRI_ITM_Length"]),
                        SR_ITM_MaterialGrade = Convert.ToString(dr["SRI_ITM_MaterialGrade"]),
                        SR_WH_Name = Convert.ToString(dr["SRI_WH_Name"]),
                        SR_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_Qty"])),
                        SR_UoM_Name = Convert.ToString(dr["SRI_UOM_Name"]),
                        SR_CUR_Name = Convert.ToString(dr["SRH_CUR_Name"]),
                        SR_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SRI_UnitPrice"])),
                        SR_MaterialValue = Convert.ToString(dr["SRI_Amount"]),
                        SR_TotalItemIncome = Convert.ToString(dr["SRI_ItemMiscIncome"]),
                        SR_TotalHeadIncome = Convert.ToString(dr["SRH_HeaderMiscIncome"]),
                        SR_TotalAmount = Convert.ToString(dr["SRH_Amount"]),
                        SR_ExchangeRate = Convert.ToString(dr["SRH_ExchangeRate"]),
                        SR_HeadGST_Amount = Convert.ToString(dr["SRH_GST_Amount"]),
                        SR_ItemGST_Amount = Convert.ToString(dr["SRI_GST_Amount"]),
                        SR_RejectionAmount = Convert.ToString(dr["SRH_RejectionAmount"]),
                        SR_HeadWHT_Amount = Convert.ToString(dr["SRH_WHT_Amount"]),
                        SR_ItemWHT_Amount = Convert.ToString(dr["SRI_WHT_Amount"]),
                        SR_RoundOff = Convert.ToString(dr["SRH_RoundOff"]),
                        SR_BuyerReceivable = Convert.ToString(dr["SRH_BuyerReceivable"])
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


        public List<SaleRejectionHead_DTO> SRHeadEditList(DataTable Dt)
        {
            List<SaleRejectionHead_DTO> SIList = new List<SaleRejectionHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleRejectionHead_DTO
                    {
                        SRH_Number = Convert.ToInt64(dr["SRH_Number"]),
                        SRH_RejectionNo = Convert.ToString(dr["SRH_RejectionNo"]),
                        SRH_RejectionDate = Convert.ToString(dr["SRH_RejectionDate"]),
                        SRH_ExportOrder = Convert.ToString(dr["SRH_ExportOrder"]) == "1" ? "true" : "false",
                        SRH_BUY_Number = Convert.ToString(dr["SRH_BUY_Number"]),
                        SRH_BUY_Name = Convert.ToString(dr["SRH_BUY_Name"]),
                        SRH_BUY_LOC_Number = Convert.ToString(dr["SRH_BUY_LOC_Number"]),
                        SRH_CUR_Number = Convert.ToString(dr["SRH_CUR_Number"]),
                        SRH_CUR_Name = Convert.ToString(dr["SRH_CUR_Name"]),
                        SRH_CUR_DecimalPlaces = Convert.ToString(dr["SRH_CUR_DecimalPlaces"]),
                        SRH_MS_Number = Convert.ToString(dr["SRH_MS_Number"]),
                        SRH_ExchangeRate = Convert.ToString(dr["SRH_ExchangeRate"]),
                        SRH_TCT_Number = Convert.ToString(dr["SRH_TCT_Number"]),
                        SRH_WHT_Number = Convert.ToString(dr["SRH_WHT_Number"]),
                        SRH_MaterialCost = Convert.ToString(dr["SRH_MaterialCost"]),
                        SRH_ItemMiscIncome = Convert.ToString(dr["SRH_ItemMiscIncome"]),
                        SRH_HeaderMiscIncome = Convert.ToString(dr["SRH_HeaderMiscIncome"]),
                        SRH_RejectionAmount = Convert.ToString(dr["SRH_RejectionAmount"]),
                        SRH_GST_Amount = Convert.ToString(dr["SRH_GST_Amount"]),
                        SRH_WHT_Amount = Convert.ToString(dr["SRH_WHT_Amount"]),
                        SRH_RoundOff = Convert.ToString(dr["SRH_RoundOff"]),
                        SRH_BuyerReceivable = Convert.ToString(dr["SRH_BuyerReceivable"]),
                        SRH_Mode = Convert.ToInt32(dr["SRH_Mode"]),
                    });
            }
            return SIList;
        }
        public List<SaleRejectionItem_DTO> SRItemEditList(DataTable Dt)
        {
            List<SaleRejectionItem_DTO> SIList = new List<SaleRejectionItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleRejectionItem_DTO
                    {
                        SRI_Number = Convert.ToInt64(dr["SRI_Number"]),
                        SRI_SIH_InvoiceNo = Convert.ToString(dr["SRI_SIH_InvoiceNo"]),
                        SRI_MS_Number = Convert.ToInt64(dr["SRI_MS_Number"]),
                        SRI_ITM_Number = Convert.ToInt64(dr["SRI_ITM_Number"]),
                        SRI_ITM_Code = Convert.ToString(dr["SRI_ITM_Code"]),
                        SRI_ITM_Group = Convert.ToString(dr["SRI_ITM_Group"]),
                        SRI_ITM_Thickness = Convert.ToString(dr["SRI_ITM_Thickness"]),
                        SRI_ITM_OuterDia = Convert.ToString(dr["SRI_ITM_OuterDia"]),
                        SRI_ITM_Length = Convert.ToString(dr["SRI_ITM_Length"]),
                        SRI_ITM_MaterialGrade = Convert.ToString(dr["SRI_ITM_MaterialGrade"]),
                        SRI_ITM_Description = Convert.ToString(dr["SRI_ITM_Description"]),
                        SRI_WH_Number = Convert.ToInt64(dr["SRI_WH_Number"]),
                        SRI_UoM_Number = Convert.ToInt64(dr["SRI_UoM_Number"]),
                        SRI_OLD_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_Qty"])),
                        SRI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_Qty"])),
                        SRI_SRI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_SRI_Qty"])),
                        SRI_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SRI_UnitPrice"])),
                        SRI_Amount = Convert.ToString(dr["SRI_Amount"]),
                        SRI_IncomeValue = Convert.ToString(dr["SRI_IncomeValue"]),
                        SRI_UoM_DecimalPlaces = Convert.ToString(dr["SRI_UoM_DecimalPlaces"]),
                        SRI_HSN_Number = Convert.ToInt64(dr["SRI_HSN_Number"]),
                        SRI_GST_Amount = Convert.ToString(dr["SRI_GST_Amount"]),
                        SRI_WHT_Percent = Convert.ToString(dr["SRI_WHT_Percent"]),
                        SRI_WHT_Amount = Convert.ToString(dr["SRI_WHT_Amount"]),
                        SRI_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SaleRejectionIncome_DTO> SRIncomeEditList(DataTable Dt)
        {
            List<SaleRejectionIncome_DTO> SIList = new List<SaleRejectionIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleRejectionIncome_DTO
                    {
                        SRH_INC_SIH_InvoiceNo = Convert.ToString(dr["SRH_INC_SIH_InvoiceNo"]),
                        SRH_INC_Number = Convert.ToInt64(dr["SRH_INC_Number"]),
                        SRH_INC_MIC_Number = Convert.ToString(dr["SRH_INC_MIC_Number"]),
                        SRH_INC_MIC_Description = Convert.ToString(dr["SRH_INC_MIC_Description"]),
                        SRH_INC_Remarks = Convert.ToString(dr["SRH_INC_Remarks"]),
                        SRH_INC_OCRN_Number = Convert.ToString(dr["SRH_INC_OCRN_Number"]),
                        SRH_INC_CM_Number = Convert.ToString(dr["SRH_INC_CM_Number"]),
                        SRH_INC_IncomeBase = Convert.ToString(dr["SRH_INC_IncomeBase"]),
                        SRH_INC_IncomeValue = Convert.ToString(dr["SRH_INC_IncomeValue"]),
                        SRH_INC_ALCT_Number = Convert.ToString(dr["SRH_INC_ALCT_Number"]),
                        SRH_INC_LA_Number = Convert.ToString(dr["SRH_INC_LA_Number"]),
                        SRH_INC_SAC_Number = Convert.ToString(dr["SRH_INC_SAC_Number"]),
                        SRH_INC_CalculateGST = Convert.ToString(dr["SRH_INC_CalculateGST"]),
                        SRH_INC_GST_Amount = Convert.ToString(dr["SRH_INC_GST_Amount"]),
                        SRH_INC_WHT_Percent = Convert.ToString(dr["SRH_INC_WHT_Percent"]),
                        SRH_INC_WHT_Amount = Convert.ToString(dr["SRH_INC_WHT_Amount"]),
                        SRH_INC_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SaleRejectionIIncome_DTO> SRIIncomeEditList(DataTable Dt)
        {
            List<SaleRejectionIIncome_DTO> SIList = new List<SaleRejectionIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new SaleRejectionIIncome_DTO
                    {
                        SRI_INC_Number = Convert.ToInt64(dr["SRI_INC_Number"]),
                        SRI_INC_SRI_Number = Convert.ToInt64(dr["SRI_INC_SRI_Number"]),
                        SRI_INC_MIC_Number = Convert.ToString(dr["SRI_INC_MIC_Number"]),
                        SRI_INC_MIC_Description = Convert.ToString(dr["SRI_INC_MIC_Description"]),
                        SRI_INC_Remarks = Convert.ToString(dr["SRI_INC_Remarks"]),
                        SRI_INC_OCRN_Number = Convert.ToString(dr["SRI_INC_OCRN_Number"]),
                        SRI_INC_CM_Number = Convert.ToString(dr["SRI_INC_CM_Number"]),
                        SRI_INC_IncomeBase = Convert.ToString(dr["SRI_INC_IncomeBase"]),
                        SRI_INC_IncomeValue = Convert.ToString(dr["SRI_INC_IncomeValue"]),
                        SRI_INC_ALCT_Number = Convert.ToString(dr["SRI_INC_ALCT_Number"]),
                        SRI_INC_LA_Number = Convert.ToString(dr["SRI_INC_LA_Number"]),
                        SRI_INC_IsDeleted = 0
                    });
            }
            return SIList;
        }
        public List<SaleRejectionBatch_DTO> SRIBatchEditList(DataTable Dt)
        {
            List<SaleRejectionBatch_DTO> SRList = new List<SaleRejectionBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SaleRejectionBatch_DTO
                    {
                        SRI_BCH_Number = Convert.ToInt64(dr["SRI_BCH_Number"]),
                        SRI_BCH_SRI_Number = Convert.ToInt64(dr["SRI_BCH_SRI_Number"]),
                        SRI_BCH_Date = Convert.ToString(dr["SRI_BCH_Date"]),
                        SRI_BCH_No = Convert.ToString(dr["SRI_BCH_No"]),
                        SRI_BCH_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_BCH_Qty"]))),
                        SRI_BCH_UnitPrice = Convert.ToString(MH.DecimalConvertUnit(Convert.ToDouble(dr["SRI_BCH_UnitPrice"]))),
                        SRI_BCH_Value = Convert.ToString(dr["SRI_BCH_Value"]),
                        SRI_BCH_IsDeleted = 0
                    });
            }
            return SRList;
        }
        public List<SaleRejectionAddress_DTO> SRHAddressEditList(DataTable Dt)
        {
            List<SaleRejectionAddress_DTO> SRList = new List<SaleRejectionAddress_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SaleRejectionAddress_DTO
                    {
                        SRH_ADD_Number = Convert.ToInt64(dr["SRH_ADD_Number"]),
                        SRH_ADD_ADTP_Number = Convert.ToInt64(dr["SRH_ADD_ADTP_Number"]),
                        SRH_ADD_AddressID = Convert.ToString(dr["SRH_ADD_AddressID"]),
                        SRH_ADD_Address = Convert.ToString(dr["SRH_ADD_Address"]),
                        SRH_ADD_City = Convert.ToString(dr["SRH_ADD_City"]),
                        SRH_ADD_State = Convert.ToString(dr["SRH_ADD_State"]),
                        SRH_ADD_Country = Convert.ToString(dr["SRH_ADD_Country"]),
                        SRH_ADD_Pin = Convert.ToString(dr["SRH_ADD_Pin"]),
                        SRH_ADD_GSTIN = Convert.ToString(dr["SRH_ADD_GSTIN"]),
                        SRH_ADD_IsDeleted = 0
                    });
            }
            return SRList;
        }





        //SI TO Sale Rejection
        public List<SaleInvoiceHead_DTO> SIToSRInvoice(DataTable Dt)
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
                        SIH_BuyerReceivable = Convert.ToString(dr["SIH_BuyerReceivable"]),
                    });
            }
            return SIList;
        }
        public List<SaleInvoiceSRItem_DTO> SIToSRInvoiceItem(DataTable Dt)
        {
            List<SaleInvoiceSRItem_DTO> POList = new List<SaleInvoiceSRItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SaleInvoiceSRItem_DTO
                    {
                        SII_Number = Convert.ToInt64(dr["SII_Number"]),
                        SIH_InvoiceNo = Convert.ToString(dr["SIH_InvoiceNo"]),
                        SIH_Number = Convert.ToInt64(dr["SIH_Number"]),
                        SII_MS_Number = Convert.ToInt64(dr["SII_MS_Number"]),
                        SII_ITM_Number = Convert.ToString(dr["SII_ITM_Number"]),
                        SII_UoM_Number = Convert.ToString(dr["SII_UoM_Number"]),
                        SII_WH_Number = Convert.ToInt64(dr["SII_WH_Number"]),
                        SII_UoM_DecimalPlaces = Convert.ToString(dr["SII_DecimalPlaces"]),
                        SII_Origin_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Origin_Qty"])),
                        SII_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SII_HSN = Convert.ToString(dr["SII_HSN"]),
                        SII_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SII_UnitPrice"])),
                        SII_Amount = Convert.ToString(dr["SII_Amount"]),
                        SII_ITM_Group = Convert.ToString(dr["SII_ITM_Group"]),
                        SII_ITM_Code = Convert.ToString(dr["SII_ITM_Code"]),
                        SII_ITM_Description = Convert.ToString(dr["SII_ITM_Description"]),
                        SII_ITM_OuterDia = Convert.ToString(dr["SII_ITM_OuterDia"]),
                        SII_ITM_MaterialGrade = Convert.ToString(dr["SII_ITM_MaterialGrade"]),
                        SII_ITM_Length = Convert.ToString(dr["SII_ITM_Length"]),
                        SII_ITM_Thickness = Convert.ToString(dr["SII_ITM_Thickness"]),
                    });
            }
            return POList;
        }
        public List<SaleInvoiceSRIncome_DTO> SIToSRIncome(DataTable Dt)
        {
            List<SaleInvoiceSRIncome_DTO> POList = new List<SaleInvoiceSRIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SaleInvoiceSRIncome_DTO
                    {
                        SIH_INC_SIH_Number = Convert.ToInt64(dr["SIH_INC_SIH_Number"]),
                        SIH_INC_Number = Convert.ToInt64(dr["SIH_INC_Number"]),
                        SIH_INC_SIH_InvoiceNo = Convert.ToString(dr["SIH_INC_SIH_InvoiceNo"]),
                        SIH_INC_MIC_Number = Convert.ToString(dr["SIH_INC_MIC_Number"]),
                        SIH_INC_Description = Convert.ToString(dr["SIH_INC_Description"]),
                        SIH_INC_Remarks = Convert.ToString(dr["SIH_INC_Remarks"]),
                        SIH_INC_OCRN_Number = Convert.ToString(dr["SIH_INC_OCRN_Number"]),
                        SIH_INC_CM_Number = Convert.ToString(dr["SIH_INC_CM_Number"]),
                        SIH_INC_IncomeBase = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SIH_INC_IncomeBase"]))),
                        SIH_INC_IncomeValue = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SIH_INC_IncomeValue"]))),
                        SIH_INC_ALCT_Number = Convert.ToString(dr["SIH_INC_ALCT_Number"]),
                        SIH_INC_LA_Number = Convert.ToString(dr["SIH_INC_LA_Number"]),
                        SIH_INC_SAC_Number = Convert.ToString(dr["SIH_INC_SAC_Number"]),
                    });
            }
            return POList;
        }
        public List<SaleInvoiceSRIIncome_DTO> SIToSRIIncome(DataTable Dt)
        {
            List<SaleInvoiceSRIIncome_DTO> POList = new List<SaleInvoiceSRIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SaleInvoiceSRIIncome_DTO
                    {
                        SII_INC_Number = Convert.ToInt64(dr["SII_INC_Number"]),
                        SII_INC_SII_Number = Convert.ToInt64(dr["SII_INC_SII_Number"]),
                        SII_INC_SIH_Number = Convert.ToInt64(dr["SII_INC_SIH_Number"]),
                        SII_INC_MIC_Number = Convert.ToString(dr["SII_INC_MIC_Number"]),
                        SII_INC_Description = Convert.ToString(dr["SII_INC_Description"]),
                        SII_INC_Remarks = Convert.ToString(dr["SII_INC_Remarks"]),
                        SII_INC_OCRN_Number = Convert.ToString(dr["SII_INC_OCRN_Number"]),
                        SII_INC_CM_Number = Convert.ToString(dr["SII_INC_CM_Number"]),
                        SII_INC_IncomeBase = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SII_INC_IncomeBase"]))),
                        SII_INC_IncomeValue = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SII_INC_IncomeValue"]))),
                        SII_INC_ALCT_Number = Convert.ToString(dr["SII_INC_ALCT_Number"]),
                        SII_INC_LA_Number = Convert.ToString(dr["SII_INC_LA_Number"]),
                    });
            }
            return POList;
        }

        public List<SIToSaleRejectionHead_DTO> SIToSRHeadEditList(DataTable Dt)
        {
            List<SIToSaleRejectionHead_DTO> SRList = new List<SIToSaleRejectionHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIToSaleRejectionHead_DTO
                    {
                        SRH_Number = Convert.ToInt64(dr["SRH_Number"]),
                        SRH_RejectionNo = Convert.ToString(dr["SRH_RejectionNo"]),
                        SRH_RejectionDate = Convert.ToString(dr["SRH_RejectionDate"]),
                        SRH_ExportOrder = Convert.ToString(dr["SRH_ExportOrder"]) == "1" ? "true" : "false",
                        SRH_BUY_Number = Convert.ToString(dr["SRH_BUY_Number"]),
                        SRH_BUY_Name = Convert.ToString(dr["SRH_BUY_Name"]),
                        SRH_BUY_LOC_Number = Convert.ToString(dr["SRH_BUY_LOC_Number"]),
                        SRH_CUR_Number = Convert.ToString(dr["SRH_CUR_Number"]),
                        SRH_CUR_Name = Convert.ToString(dr["SRH_CUR_Name"]),
                        SRH_CUR_DecimalPlaces = Convert.ToString(dr["SRH_CUR_DecimalPlaces"]),
                        SRH_MS_Number = Convert.ToString(dr["SRH_MS_Number"]),
                        SRH_ExchangeRate = Convert.ToString(dr["SRH_ExchangeRate"]),
                        SRH_TCT_Number = Convert.ToString(dr["SRH_TCT_Number"]),
                        SRH_WHT_Number = Convert.ToString(dr["SRH_WHT_Number"]),
                        SRH_MaterialCost = Convert.ToString(dr["SRH_MaterialCost"]),
                        SRH_ItemMiscIncome = Convert.ToString(dr["SRH_ItemMiscIncome"]),
                        SRH_HeaderMiscIncome = Convert.ToString(dr["SRH_HeaderMiscIncome"]),
                        SRH_RejectionAmount = Convert.ToString(dr["SRH_RejectionAmount"]),
                        SRH_GST_Amount = Convert.ToString(dr["SRH_GST_Amount"]),
                        SRH_WHT_Amount = Convert.ToString(dr["SRH_WHT_Amount"]),
                        SRH_RoundOff = Convert.ToString(dr["SRH_RoundOff"]),
                        SRH_BuyerReceivable = Convert.ToString(dr["SRH_BuyerReceivable"]),
                        SRH_Mode = Convert.ToInt32(dr["SRH_Mode"]),
                    });
            }
            return SRList;
        }
        public List<SIToSaleRejectionItem_DTO> SIToSRItemEditList(DataTable Dt)
        {
            List<SIToSaleRejectionItem_DTO> SRList = new List<SIToSaleRejectionItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIToSaleRejectionItem_DTO
                    {
                        SRI_Number = Convert.ToInt64(dr["SRI_Number"]),
                        SRI_SIH_Number = Convert.ToInt64(dr["SRI_SIH_Number"]),
                        SRI_SII_Number = Convert.ToInt64(dr["SRI_SII_Number"]),
                        SRI_SIH_InvoiceNo = Convert.ToString(dr["SRI_SIH_InvoiceNo"]),
                        SRI_MS_Number = Convert.ToInt64(dr["SRI_MS_Number"]),
                        SRI_ITM_Number = Convert.ToInt64(dr["SRI_ITM_Number"]),
                        SRI_ITM_Code = Convert.ToString(dr["SRI_ITM_Code"]),
                        SRI_ITM_Group = Convert.ToString(dr["SRI_ITM_Group"]),
                        SRI_ITM_Thickness = Convert.ToString(dr["SRI_ITM_Thickness"]),
                        SRI_ITM_OuterDia = Convert.ToString(dr["SRI_ITM_OuterDia"]),
                        SRI_ITM_Length = Convert.ToString(dr["SRI_ITM_Length"]),
                        SRI_ITM_MaterialGrade = Convert.ToString(dr["SRI_ITM_MaterialGrade"]),
                        SRI_ITM_Description = Convert.ToString(dr["SRI_ITM_Description"]),
                        SRI_WH_Number = Convert.ToInt64(dr["SRI_WH_Number"]),
                        SRI_UoM_Number = Convert.ToInt64(dr["SRI_UoM_Number"]),
                        SRI_SII_Origin_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_SII_Origin_Qty"])),
                        SRI_SII_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_SII_Qty"])),
                        SRI_OLD_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_Qty"])),
                        SRI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_Qty"])),
                        //SRI_SRI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_SRI_Qty"])),
                        SRI_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SRI_UnitPrice"])),
                        SRI_Amount = Convert.ToString(dr["SRI_Amount"]),
                        SRI_IncomeValue = Convert.ToString(dr["SRI_IncomeValue"]),
                        SRI_UoM_DecimalPlaces = Convert.ToString(dr["SRI_UoM_DecimalPlaces"]),
                        SRI_HSN_Number = Convert.ToInt64(dr["SRI_HSN_Number"]),
                        SRI_GST_Amount = Convert.ToString(dr["SRI_GST_Amount"]),
                        SRI_WHT_Percent = Convert.ToString(dr["SRI_WHT_Percent"]),
                        SRI_WHT_Amount = Convert.ToString(dr["SRI_WHT_Amount"]),
                        SRI_IsDeleted = 0
                    });
            }
            return SRList;
        }
        public List<SIToSaleRejectionIncome_DTO> SIToSRIncomeEditList(DataTable Dt)
        {
            List<SIToSaleRejectionIncome_DTO> SRList = new List<SIToSaleRejectionIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIToSaleRejectionIncome_DTO
                    {
                        SRH_INC_SIH_InvoiceNo = Convert.ToString(dr["SRH_INC_SIH_InvoiceNo"]),
                        SRH_INC_Number = Convert.ToInt64(dr["SRH_INC_Number"]),
                        SRH_INC_SIH_INC_Number = Convert.ToInt64(dr["SRH_INC_SIH_INC_Number"]),
                        SRH_INC_SIH_Number = Convert.ToInt64(dr["SRH_INC_SIH_Number"]),
                        SRH_INC_MIC_Number = Convert.ToString(dr["SRH_INC_MIC_Number"]),
                        SRH_INC_MIC_Description = Convert.ToString(dr["SRH_INC_MIC_Description"]),
                        SRH_INC_Remarks = Convert.ToString(dr["SRH_INC_Remarks"]),
                        SRH_INC_OCRN_Number = Convert.ToString(dr["SRH_INC_OCRN_Number"]),
                        SRH_INC_CM_Number = Convert.ToString(dr["SRH_INC_CM_Number"]),
                        SRH_INC_IncomeBase = Convert.ToString(dr["SRH_INC_IncomeBase"]),
                        SRH_INC_IncomeValue = Convert.ToString(dr["SRH_INC_IncomeValue"]),
                        SRH_INC_ALCT_Number = Convert.ToString(dr["SRH_INC_ALCT_Number"]),
                        SRH_INC_LA_Number = Convert.ToString(dr["SRH_INC_LA_Number"]),
                        SRH_INC_SAC_Number = Convert.ToString(dr["SRH_INC_SAC_Number"]),
                        SRH_INC_CalculateGST = Convert.ToString(dr["SRH_INC_CalculateGST"]),
                        SRH_INC_GST_Amount = Convert.ToString(dr["SRH_INC_GST_Amount"]),
                        SRH_INC_WHT_Percent = Convert.ToString(dr["SRH_INC_WHT_Percent"]),
                        SRH_INC_WHT_Amount = Convert.ToString(dr["SRH_INC_WHT_Amount"]),
                        SRH_INC_IsDeleted = 0
                    });
            }
            return SRList;
        }
        public List<SIToSaleRejectionIIncome_DTO> SIToSRIIncomeEditList(DataTable Dt)
        {
            List<SIToSaleRejectionIIncome_DTO> SRList = new List<SIToSaleRejectionIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIToSaleRejectionIIncome_DTO
                    {
                        SRI_INC_Number = Convert.ToInt64(dr["SRI_INC_Number"]),
                        SRI_INC_SII_Number = Convert.ToInt64(dr["SRI_INC_SII_Number"]),
                        SRI_INC_SIH_Number = Convert.ToInt64(dr["SRI_INC_SIH_Number"]),
                        SRI_INC_SII_INC_Number = Convert.ToInt64(dr["SRI_INC_SII_INC_Number"]),
                        SRI_INC_SRI_Number = Convert.ToInt64(dr["SRI_INC_SRI_Number"]),
                        SRI_INC_MIC_Number = Convert.ToString(dr["SRI_INC_MIC_Number"]),
                        SRI_INC_MIC_Description = Convert.ToString(dr["SRI_INC_MIC_Description"]),
                        SRI_INC_Remarks = Convert.ToString(dr["SRI_INC_Remarks"]),
                        SRI_INC_OCRN_Number = Convert.ToString(dr["SRI_INC_OCRN_Number"]),
                        SRI_INC_CM_Number = Convert.ToString(dr["SRI_INC_CM_Number"]),
                        SRI_INC_IncomeBase = Convert.ToString(dr["SRI_INC_IncomeBase"]),
                        SRI_INC_IncomeValue = Convert.ToString(dr["SRI_INC_IncomeValue"]),
                        SRI_INC_ALCT_Number = Convert.ToString(dr["SRI_INC_ALCT_Number"]),
                        SRI_INC_LA_Number = Convert.ToString(dr["SRI_INC_LA_Number"]),
                        SRI_INC_IsDeleted = 0
                    });
            }
            return SRList;
        }
        public List<SIToSaleRejectionBatch_DTO> SIToSRIBatchEditList(DataTable Dt)
        {
            List<SIToSaleRejectionBatch_DTO> SRList = new List<SIToSaleRejectionBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIToSaleRejectionBatch_DTO
                    {
                        SRI_BCH_Number = Convert.ToInt64(dr["SRI_BCH_Number"]),
                        SRI_BCH_SRI_Number = Convert.ToInt64(dr["SRI_BCH_SRI_Number"]),
                        SRI_BCH_Date = Convert.ToString(dr["SRI_BCH_Date"]),
                        SRI_BCH_No = Convert.ToString(dr["SRI_BCH_No"]),
                        SRI_BCH_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_BCH_Qty"]))),
                        SRI_BCH_UnitPrice = Convert.ToString(MH.DecimalConvertUnit(Convert.ToDouble(dr["SRI_BCH_UnitPrice"]))),
                        SRI_BCH_Value = Convert.ToString(dr["SRI_BCH_Value"]),
                        SRI_BCH_IsDeleted = 0
                    });
            }
            return SRList;
        }
        public List<SIToSaleRejectionAddress_DTO> SIToSRHAddressEditList(DataTable Dt)
        {
            List<SIToSaleRejectionAddress_DTO> SRList = new List<SIToSaleRejectionAddress_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIToSaleRejectionAddress_DTO
                    {
                        SRH_ADD_Number = Convert.ToInt64(dr["SRH_ADD_Number"]),
                        SRH_ADD_ADTP_Number = Convert.ToInt64(dr["SRH_ADD_ADTP_Number"]),
                        SRH_ADD_AddressID = Convert.ToString(dr["SRH_ADD_AddressID"]),
                        SRH_ADD_Address = Convert.ToString(dr["SRH_ADD_Address"]),
                        SRH_ADD_City = Convert.ToString(dr["SRH_ADD_City"]),
                        SRH_ADD_State = Convert.ToString(dr["SRH_ADD_State"]),
                        SRH_ADD_Country = Convert.ToString(dr["SRH_ADD_Country"]),
                        SRH_ADD_Pin = Convert.ToString(dr["SRH_ADD_Pin"]),
                        SRH_ADD_GSTIN = Convert.ToString(dr["SRH_ADD_GSTIN"]),
                        SRH_ADD_IsDeleted = 0
                    });
            }
            return SRList;
        }





        //SI Item TO Sale Rejection
        public List<SRItemSaleInvoiceItem_DTO> SIItemToSRInvoice(DataTable Dt)
        {
            List<SRItemSaleInvoiceItem_DTO> POList = new List<SRItemSaleInvoiceItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SRItemSaleInvoiceItem_DTO
                    {
                        SII_ITM_Number = Convert.ToString(dr["ItemNumber"]),
                        SII_ITM_Code = Convert.ToString(dr["ItemCode"]),
                        SII_ITM_Description = Convert.ToString(dr["ItemDescription"]),
                        SII_ITM_Group = Convert.ToString(dr["ItemGroup"]),
                    });
            }
            return POList;
        }
        public List<SRItemSaleInvoiceItem_DTO> SIItemToSRInvoiceItem(DataTable Dt)
        {
            List<SRItemSaleInvoiceItem_DTO> POList = new List<SRItemSaleInvoiceItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SRItemSaleInvoiceItem_DTO
                    {
                        SII_Number = Convert.ToInt64(dr["SII_Number"]),
                        SIH_InvoiceNo = Convert.ToString(dr["SIH_InvoiceNo"]),
                        SIH_Number = Convert.ToInt64(dr["SIH_Number"]),
                        SII_MS_Number = Convert.ToInt64(dr["SII_MS_Number"]),
                        SII_ITM_Number = Convert.ToString(dr["SII_ITM_Number"]),
                        SII_UoM_Number = Convert.ToString(dr["SII_UoM_Number"]),
                        SII_WH_Number = Convert.ToInt64(dr["SII_WH_Number"]),
                        SII_UoM_DecimalPlaces = Convert.ToString(dr["SII_DecimalPlaces"]),
                        SII_Origin_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Origin_Qty"])),
                        SII_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SII_Qty"])),
                        SII_HSN = Convert.ToString(dr["SII_HSN"]),
                        SII_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SII_UnitPrice"])),
                        SII_Amount = Convert.ToString(dr["SII_Amount"]),
                        SII_ITM_Group = Convert.ToString(dr["SII_ITM_Group"]),
                        SII_ITM_Code = Convert.ToString(dr["SII_ITM_Code"]),
                        SII_ITM_Description = Convert.ToString(dr["SII_ITM_Description"]),
                        SII_ITM_OuterDia = Convert.ToString(dr["SII_ITM_OuterDia"]),
                        SII_ITM_MaterialGrade = Convert.ToString(dr["SII_ITM_MaterialGrade"]),
                        SII_ITM_Length = Convert.ToString(dr["SII_ITM_Length"]),
                        SII_ITM_Thickness = Convert.ToString(dr["SII_ITM_Thickness"]),
                    });
            }
            return POList;
        }
        public List<SRItemSaleInvoiceIncome_DTO> SIItemToSRIncome(DataTable Dt)
        {
            List<SRItemSaleInvoiceIncome_DTO> POList = new List<SRItemSaleInvoiceIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SRItemSaleInvoiceIncome_DTO
                    {
                        SIH_INC_SIH_Number = Convert.ToInt64(dr["SIH_INC_SIH_Number"]),
                        SIH_INC_Number = Convert.ToInt64(dr["SIH_INC_Number"]),
                        SIH_INC_SIH_InvoiceNo = Convert.ToString(dr["SIH_INC_SIH_InvoiceNo"]),
                        SIH_INC_MIC_Number = Convert.ToString(dr["SIH_INC_MIC_Number"]),
                        SIH_INC_Description = Convert.ToString(dr["SIH_INC_Description"]),
                        SIH_INC_Remarks = Convert.ToString(dr["SIH_INC_Remarks"]),
                        SIH_INC_OCRN_Number = Convert.ToString(dr["SIH_INC_OCRN_Number"]),
                        SIH_INC_CM_Number = Convert.ToString(dr["SIH_INC_CM_Number"]),
                        SIH_INC_IncomeBase = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SIH_INC_IncomeBase"]))),
                        SIH_INC_IncomeValue = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SIH_INC_IncomeValue"]))),
                        SIH_INC_ALCT_Number = Convert.ToString(dr["SIH_INC_ALCT_Number"]),
                        SIH_INC_LA_Number = Convert.ToString(dr["SIH_INC_LA_Number"]),
                        SIH_INC_SAC_Number = Convert.ToString(dr["SIH_INC_SAC_Number"]),
                    });
            }
            return POList;
        }
        public List<SRItemSaleInvoiceIIncome_DTO> SIItemToSRIIncome(DataTable Dt)
        {
            List<SRItemSaleInvoiceIIncome_DTO> POList = new List<SRItemSaleInvoiceIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                POList.Add(
                    new SRItemSaleInvoiceIIncome_DTO
                    {
                        SII_INC_Number = Convert.ToInt64(dr["SII_INC_Number"]),
                        SII_INC_SII_Number = Convert.ToInt64(dr["SII_INC_SII_Number"]),
                        SII_INC_SIH_Number = Convert.ToInt64(dr["SII_INC_SIH_Number"]),
                        SII_INC_MIC_Number = Convert.ToString(dr["SII_INC_MIC_Number"]),
                        SII_INC_Description = Convert.ToString(dr["SII_INC_Description"]),
                        SII_INC_Remarks = Convert.ToString(dr["SII_INC_Remarks"]),
                        SII_INC_OCRN_Number = Convert.ToString(dr["SII_INC_OCRN_Number"]),
                        SII_INC_CM_Number = Convert.ToString(dr["SII_INC_CM_Number"]),
                        SII_INC_IncomeBase = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SII_INC_IncomeBase"]))),
                        SII_INC_IncomeValue = Convert.ToString(MH.DecimalConvertFixed(Convert.ToDouble(dr["SII_INC_IncomeValue"]))),
                        SII_INC_ALCT_Number = Convert.ToString(dr["SII_INC_ALCT_Number"]),
                        SII_INC_LA_Number = Convert.ToString(dr["SII_INC_LA_Number"]),
                    });
            }
            return POList;
        }


        public List<SIItemToSaleRejectionHead_DTO> SIItemToSRHeadEditList(DataTable Dt)
        {
            List<SIItemToSaleRejectionHead_DTO> SRList = new List<SIItemToSaleRejectionHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIItemToSaleRejectionHead_DTO
                    {
                        SRH_Number = Convert.ToInt64(dr["SRH_Number"]),
                        SRH_RejectionNo = Convert.ToString(dr["SRH_RejectionNo"]),
                        SRH_RejectionDate = Convert.ToString(dr["SRH_RejectionDate"]),
                        SRH_ExportOrder = Convert.ToString(dr["SRH_ExportOrder"]) == "1" ? "true" : "false",
                        SRH_BUY_Number = Convert.ToString(dr["SRH_BUY_Number"]),
                        SRH_BUY_Name = Convert.ToString(dr["SRH_BUY_Name"]),
                        SRH_BUY_LOC_Number = Convert.ToString(dr["SRH_BUY_LOC_Number"]),
                        SRH_CUR_Number = Convert.ToString(dr["SRH_CUR_Number"]),
                        SRH_CUR_Name = Convert.ToString(dr["SRH_CUR_Name"]),
                        SRH_CUR_DecimalPlaces = Convert.ToString(dr["SRH_CUR_DecimalPlaces"]),
                        SRH_MS_Number = Convert.ToString(dr["SRH_MS_Number"]),
                        SRH_ExchangeRate = Convert.ToString(dr["SRH_ExchangeRate"]),
                        SRH_TCT_Number = Convert.ToString(dr["SRH_TCT_Number"]),
                        SRH_WHT_Number = Convert.ToString(dr["SRH_WHT_Number"]),
                        SRH_MaterialCost = Convert.ToString(dr["SRH_MaterialCost"]),
                        SRH_ItemMiscIncome = Convert.ToString(dr["SRH_ItemMiscIncome"]),
                        SRH_HeaderMiscIncome = Convert.ToString(dr["SRH_HeaderMiscIncome"]),
                        SRH_RejectionAmount = Convert.ToString(dr["SRH_RejectionAmount"]),
                        SRH_GST_Amount = Convert.ToString(dr["SRH_GST_Amount"]),
                        SRH_WHT_Amount = Convert.ToString(dr["SRH_WHT_Amount"]),
                        SRH_RoundOff = Convert.ToString(dr["SRH_RoundOff"]),
                        SRH_BuyerReceivable = Convert.ToString(dr["SRH_BuyerReceivable"]),
                        SRH_Mode = Convert.ToInt32(dr["SRH_Mode"]),
                    });
            }
            return SRList;
        }
        public List<SIItemToSaleRejectionItem_DTO> SIItemToSRItemEditList(DataTable Dt)
        {
            List<SIItemToSaleRejectionItem_DTO> SRList = new List<SIItemToSaleRejectionItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIItemToSaleRejectionItem_DTO
                    {
                        SRI_Number = Convert.ToInt64(dr["SRI_Number"]),
                        SRI_SIH_Number = Convert.ToInt64(dr["SRI_SIH_Number"]),
                        SRI_SII_Number = Convert.ToInt64(dr["SRI_SII_Number"]),
                        SRI_SIH_InvoiceNo = Convert.ToString(dr["SRI_SIH_InvoiceNo"]),
                        SRI_MS_Number = Convert.ToInt64(dr["SRI_MS_Number"]),
                        SRI_ITM_Number = Convert.ToInt64(dr["SRI_ITM_Number"]),
                        SRI_ITM_Code = Convert.ToString(dr["SRI_ITM_Code"]),
                        SRI_ITM_Group = Convert.ToString(dr["SRI_ITM_Group"]),
                        SRI_ITM_Thickness = Convert.ToString(dr["SRI_ITM_Thickness"]),
                        SRI_ITM_OuterDia = Convert.ToString(dr["SRI_ITM_OuterDia"]),
                        SRI_ITM_Length = Convert.ToString(dr["SRI_ITM_Length"]),
                        SRI_ITM_MaterialGrade = Convert.ToString(dr["SRI_ITM_MaterialGrade"]),
                        SRI_ITM_Description = Convert.ToString(dr["SRI_ITM_Description"]),
                        SRI_WH_Number = Convert.ToInt64(dr["SRI_WH_Number"]),
                        SRI_UoM_Number = Convert.ToInt64(dr["SRI_UoM_Number"]),
                        SRI_SII_Origin_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_SII_Origin_Qty"])),
                        SRI_SII_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_SII_Qty"])),
                        SRI_OLD_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_Qty"])),
                        SRI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_Qty"])),
                        SRI_SRI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_SRI_Qty"])),
                        SRI_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SRI_UnitPrice"])),
                        SRI_Amount = Convert.ToString(dr["SRI_Amount"]),
                        SRI_IncomeValue = Convert.ToString(dr["SRI_IncomeValue"]),
                        SRI_UoM_DecimalPlaces = Convert.ToString(dr["SRI_UoM_DecimalPlaces"]),
                        SRI_HSN_Number = Convert.ToInt64(dr["SRI_HSN_Number"]),
                        SRI_GST_Amount = Convert.ToString(dr["SRI_GST_Amount"]),
                        SRI_WHT_Percent = Convert.ToString(dr["SRI_WHT_Percent"]),
                        SRI_WHT_Amount = Convert.ToString(dr["SRI_WHT_Amount"]),
                        SRI_IsDeleted = 0
                    });
            }
            return SRList;
        }
        public List<SIItemToSaleRejectionIncome_DTO> SIItemToSRIncomeEditList(DataTable Dt)
        {
            List<SIItemToSaleRejectionIncome_DTO> SRList = new List<SIItemToSaleRejectionIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIItemToSaleRejectionIncome_DTO
                    {
                        SRH_INC_SIH_InvoiceNo = Convert.ToString(dr["SRH_INC_SIH_InvoiceNo"]),
                        SRH_INC_Number = Convert.ToInt64(dr["SRH_INC_Number"]),
                        SRH_INC_SIH_INC_Number = Convert.ToInt64(dr["SRH_INC_SIH_INC_Number"]),
                        SRH_INC_SIH_Number = Convert.ToInt64(dr["SRH_INC_SIH_Number"]),
                        SRH_INC_MIC_Number = Convert.ToString(dr["SRH_INC_MIC_Number"]),
                        SRH_INC_MIC_Description = Convert.ToString(dr["SRH_INC_MIC_Description"]),
                        SRH_INC_Remarks = Convert.ToString(dr["SRH_INC_Remarks"]),
                        SRH_INC_OCRN_Number = Convert.ToString(dr["SRH_INC_OCRN_Number"]),
                        SRH_INC_CM_Number = Convert.ToString(dr["SRH_INC_CM_Number"]),
                        SRH_INC_IncomeBase = Convert.ToString(dr["SRH_INC_IncomeBase"]),
                        SRH_INC_IncomeValue = Convert.ToString(dr["SRH_INC_IncomeValue"]),
                        SRH_INC_ALCT_Number = Convert.ToString(dr["SRH_INC_ALCT_Number"]),
                        SRH_INC_LA_Number = Convert.ToString(dr["SRH_INC_LA_Number"]),
                        SRH_INC_SAC_Number = Convert.ToString(dr["SRH_INC_SAC_Number"]),
                        SRH_INC_CalculateGST = Convert.ToString(dr["SRH_INC_CalculateGST"]),
                        SRH_INC_GST_Amount = Convert.ToString(dr["SRH_INC_GST_Amount"]),
                        SRH_INC_WHT_Percent = Convert.ToString(dr["SRH_INC_WHT_Percent"]),
                        SRH_INC_WHT_Amount = Convert.ToString(dr["SRH_INC_WHT_Amount"]),
                        SRH_INC_IsDeleted = 0
                    });
            }
            return SRList;
        }
        public List<SIItemToSaleRejectionIIncome_DTO> SIItemToSRIIncomeEditList(DataTable Dt)
        {
            List<SIItemToSaleRejectionIIncome_DTO> SRList = new List<SIItemToSaleRejectionIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIItemToSaleRejectionIIncome_DTO
                    {
                        SRI_INC_Number = Convert.ToInt64(dr["SRI_INC_Number"]),
                        SRI_INC_SII_Number = Convert.ToInt64(dr["SRI_INC_SII_Number"]),
                        SRI_INC_SIH_Number = Convert.ToInt64(dr["SRI_INC_SIH_Number"]),
                        SRI_INC_SII_INC_Number = Convert.ToInt64(dr["SRI_INC_SII_INC_Number"]),
                        SRI_INC_SRI_Number = Convert.ToInt64(dr["SRI_INC_SRI_Number"]),
                        SRI_INC_MIC_Number = Convert.ToString(dr["SRI_INC_MIC_Number"]),
                        SRI_INC_MIC_Description = Convert.ToString(dr["SRI_INC_MIC_Description"]),
                        SRI_INC_Remarks = Convert.ToString(dr["SRI_INC_Remarks"]),
                        SRI_INC_OCRN_Number = Convert.ToString(dr["SRI_INC_OCRN_Number"]),
                        SRI_INC_CM_Number = Convert.ToString(dr["SRI_INC_CM_Number"]),
                        SRI_INC_IncomeBase = Convert.ToString(dr["SRI_INC_IncomeBase"]),
                        SRI_INC_IncomeValue = Convert.ToString(dr["SRI_INC_IncomeValue"]),
                        SRI_INC_ALCT_Number = Convert.ToString(dr["SRI_INC_ALCT_Number"]),
                        SRI_INC_LA_Number = Convert.ToString(dr["SRI_INC_LA_Number"]),
                        SRI_INC_IsDeleted = 0
                    });
            }
            return SRList;
        }
        public List<SIItemToSaleRejectionBatch_DTO> SIItemToSRIBatchEditList(DataTable Dt)
        {
            List<SIItemToSaleRejectionBatch_DTO> SRList = new List<SIItemToSaleRejectionBatch_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIItemToSaleRejectionBatch_DTO
                    {
                        SRI_BCH_Number = Convert.ToInt64(dr["SRI_BCH_Number"]),
                        SRI_BCH_SRI_Number = Convert.ToInt64(dr["SRI_BCH_SRI_Number"]),
                        SRI_BCH_Date = Convert.ToString(dr["SRI_BCH_Date"]),
                        SRI_BCH_No = Convert.ToString(dr["SRI_BCH_No"]),
                        SRI_BCH_Qty = Convert.ToString(MH.DecimalConvertQty(Convert.ToDouble(dr["SRI_BCH_Qty"]))),
                        SRI_BCH_UnitPrice = Convert.ToString(MH.DecimalConvertUnit(Convert.ToDouble(dr["SRI_BCH_UnitPrice"]))),
                        SRI_BCH_Value = Convert.ToString(dr["SRI_BCH_Value"]),
                        SRI_BCH_IsDeleted = 0
                    });
            }
            return SRList;
        }
        public List<SIItemToSaleRejectionAddress_DTO> SIItemToSRHAddressEditList(DataTable Dt)
        {
            List<SIItemToSaleRejectionAddress_DTO> SRList = new List<SIItemToSaleRejectionAddress_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SRList.Add(
                    new SIItemToSaleRejectionAddress_DTO
                    {
                        SRH_ADD_Number = Convert.ToInt64(dr["SRH_ADD_Number"]),
                        SRH_ADD_ADTP_Number = Convert.ToInt64(dr["SRH_ADD_ADTP_Number"]),
                        SRH_ADD_AddressID = Convert.ToString(dr["SRH_ADD_AddressID"]),
                        SRH_ADD_Address = Convert.ToString(dr["SRH_ADD_Address"]),
                        SRH_ADD_City = Convert.ToString(dr["SRH_ADD_City"]),
                        SRH_ADD_State = Convert.ToString(dr["SRH_ADD_State"]),
                        SRH_ADD_Country = Convert.ToString(dr["SRH_ADD_Country"]),
                        SRH_ADD_Pin = Convert.ToString(dr["SRH_ADD_Pin"]),
                        SRH_ADD_GSTIN = Convert.ToString(dr["SRH_ADD_GSTIN"]),
                        SRH_ADD_IsDeleted = 0
                    });
            }
            return SRList;
        }
    }
}

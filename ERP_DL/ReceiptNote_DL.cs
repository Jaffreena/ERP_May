using ERP_DTO;
using ERP_DTO.JobInwardTransaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DL
{
    public class ReceiptNote_DL
    {
        MethodHelp MH = new MethodHelp();
        public List<CustomerOrderList_DTO> CustomerList(DataTable Dt)
        {
            List<CustomerOrderList_DTO> BList = new List<CustomerOrderList_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new CustomerOrderList_DTO
                    {
                        CUS_Number = Convert.ToInt64(dr["CUS_Number"]),
                        CUS_Name = Convert.ToString(dr["CUS_Name"]),
                        CUS_CUR_Number = Convert.ToString(dr["CUS_CUR_Number"]),
                        CUS_CUR_Name = Convert.ToString(dr["CUS_CUR_Name"]),
                        CUS_LOC_Number = Convert.ToString(dr["CUS_LOC_Number"]),
                        CUS_CUR_DecimalPlaces = Convert.ToString(dr["CUS_CUR_DecimalPlaces"]),
                        CUS_TCT_Number = Convert.ToInt64(dr["CUS_TCT_Number"]),
                        CUS_WHT_Number = Convert.ToInt64(dr["CUS_WHT_Number"]),
                        CUS_WH_Number = Convert.ToInt64(dr["JWC_WH_Number"]),


                    });
            }
            return BList;
        }

        public List<CustomerOrderList_DTO> CustomerListData(DataTable Dt)
        {
            List<CustomerOrderList_DTO> BList = new List<CustomerOrderList_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new CustomerOrderList_DTO
                    {
                        CUS_Number = Convert.ToInt64(dr["CUS_Number"]),
                        CUS_Name = Convert.ToString(dr["CUS_Name"]),
                        CUS_CUR_Number = Convert.ToString(dr["CUS_CUR_Number"]),
                        CUS_CUR_Name = Convert.ToString(dr["CUS_CUR_Name"]),
                        CUS_LOC_Number = Convert.ToString(dr["CUS_LOC_Number"]),
                        CUS_CUR_DecimalPlaces = Convert.ToString(dr["CUS_CUR_DecimalPlaces"]),
                        CUS_TCT_Number = Convert.ToInt64(dr["CUS_TCT_Number"]),
                        CUS_WHT_Number = Convert.ToInt64(dr["CUS_WHT_Number"]),
                        CUS_WH_Number = Convert.ToInt64(dr["JWC_WH_Number"])
                    });
            }
            return BList;
        }
        public List<JW_Process_DTO> ProcessList(DataTable Dt)
        {
            List<JW_Process_DTO> P_List = new List<JW_Process_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                P_List.Add(
                    new JW_Process_DTO
                    {
                        ProcessNumber = Convert.ToInt64(dr["PRS_Number"]),
                        ProcessName = Convert.ToString(dr["PRS_ProcessName"]),
                        Description = Convert.ToString(dr["PRS_Description"])
                    });
            }

            return P_List;
        }
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


        public List<ReceiptNoteSummary_DTO> RNDetailedList(DataTable Dt)
        {
            List<ReceiptNoteSummary_DTO> SIList = new List<ReceiptNoteSummary_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIList.Add(
                    new ReceiptNoteSummary_DTO
                    {
                        RN_Number = Convert.ToInt64(dr["JIRNH_Number"]),
                        RN_No = Convert.ToString(dr["JIRNH_RN_No"]),
                        RN_Date = Convert.ToString(dr["JIRNH_RN_Date"]),
                       RN_Process = Convert.ToString(dr["RN_Process"]),
                        RN_JWC_WH_Name = Convert.ToString(dr["RN_JWC_WH_Name"]),
                    //    SI_ExportOrder = Convert.ToString(dr["SIH_ExportOrder"]) == "1" ? "Yes" : "No",
                        RN_ITM_Category = Convert.ToString(dr["RN_ITM_Category"]),
                        RN_ITM_GroupInfo = Convert.ToString(dr["RN_ITM_GroupInfo"]),
                        RN_JWC_Name = Convert.ToString(dr["RN_JWC_Name"]),
                        RN_MS_Number = Convert.ToString(dr["RN_MS_Number"]),
                        RN_ITM_Group = Convert.ToString(dr["RN_ITM_Group"]),
                        RN_ITM_Code = Convert.ToString(dr["RN_ITM_Code"]),
                        RN_ITM_Description = Convert.ToString(dr["RN_ITM_Description"]),
                        RN_ITM_OuterDia = Convert.ToString(dr["RN_ITM_OuterDia"]),
                        RN_ITM_Thickness = Convert.ToString(dr["RN_ITM_Thickness"]),
                        RN_ITM_Length = Convert.ToString(dr["RN_ITM_Length"]),
                        RN_ITM_Width = Convert.ToString(dr["RN_ITM_Width"]),
                        RN_ITM_MaterialGrade = Convert.ToString(dr["RN_ITM_MaterialGrade"]),
                        RN_WH_Name = Convert.ToString(dr["RN_WH_Name"]),
                        RN_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["JIRNI_Qty"])),
                        RN_UoM_Name = Convert.ToString(dr["RN_UoM_Name"]),
                        RN_CUR_Name = Convert.ToString(dr["RN_CUR_Name"]),
                        RN_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["RN_UnitPrice"])),
                        RN_TotalAmount = Convert.ToString(dr["JIRNI_Amount"]),
                        RN_JW_CustomerDC_No = Convert.ToString(dr["RN_JW_CustomerDC_No"]),
                        RN_JW_CustomerDC_Date = Convert.ToString(dr["RN_JW_CustomerDC_Date"]),
                     //   SI_TotalAmount = Convert.ToString(dr["JIRNI_Amount"]),
                        //SI_ExchangeRate = Convert.ToString(dr["SIH_ExchangeRate"]),
                        //SI_HeadGST_Amount = Convert.ToString(dr["SIH_GST_Amount"]),
                        //SI_ItemGST_Amount = Convert.ToString(dr["SII_GST_Amount"]),
                        //SI_InvoiceAmount = Convert.ToString(dr["SIH_InvoiceAmount"]),
                        //SI_HeadWHT_Amount = Convert.ToString(dr["SIH_WHT_Amount"]),
                        //SI_ItemWHT_Amount = Convert.ToString(dr["SII_WHT_Amount"]),
                        //SI_RoundOff = Convert.ToString(dr["SIH_RoundOff"]),
                        //SI_BuyerReceivable = Convert.ToString(dr["SIH_BuyerReceivable"])
                    });
            }
            return SIList;
        }


        public List<ReceiptNoteSummary_DTO> RNSummaryList(DataTable Dt)
        {
            List<ReceiptNoteSummary_DTO> RNList = new List<ReceiptNoteSummary_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                RNList.Add(
                    new ReceiptNoteSummary_DTO
                    {
                        RN_Number = Convert.ToInt64(dr["JIRNH_Number"]),

                        RN_No = Convert.ToString(dr["JIRNH_RN_No"]),
                        RN_Date = Convert.ToString(dr["JIRNH_RN_Date"]),

                        RN_JW_CustomerDC_No = Convert.ToString(dr["JIRNH_JW_CustomerDC_No"]),
                        RN_JW_CustomerDC_Date = Convert.ToString(dr["JIRNH_JW_CustomerDC_Date"]),
                        //RN_JW_CustomerDC_No ="0",
                        //RN_JW_CustomerDC_Date ="01-01-2007",

                        RN_MS_Number = Convert.ToString(dr["SIH_MS_Name"]),

                        RN_JWC_Name = Convert.ToString(dr["JWC_Name"]),

                        RN_CUR_Name = Convert.ToString(dr["CUR_Name"]),

                        RN_WH_Name = Convert.ToString(dr["WH_Name"]),

                        RN_ITM_Group = Convert.ToString(dr["ITM_Group"]),
                        RN_ITM_Category = Convert.ToString(dr["ITM_Category"]),
                        RN_ITM_Code = Convert.ToString(dr["ITM_Code"]),
                        RN_ITM_Description = Convert.ToString(dr["ITM_Description"]),

                        RN_ITM_OuterDia = Convert.ToString(dr["ITM_OuterDia"]),
                        RN_ITM_Thickness = Convert.ToString(dr["ITM_Thickness"]),
                        RN_ITM_Length = Convert.ToString(dr["ITM_Length"]),
                        RN_ITM_MaterialGrade = Convert.ToString(dr["ITM_MaterialGrade"]),

                           RN_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["JIRNI_Qty"])),
                       // RN_Qty="0",
                        RN_UoM_Name = Convert.ToString(dr["UOM_Name"]),

                        RN_NoOfItem = Convert.ToString(dr["JIRNH_NoOfItem"]),

                        RN_UnitPrice = Convert.ToString(dr["JIRNI_UnitPrice"]),

                        RN_MaterialValue = Convert.ToString(dr["JIRNH_MaterialValue"]),

                        //RN_TotalItemExpense = Convert.ToString(dr["JIRNH_ItemMiscExpense"]),

                        //RN_TotalHeadExpense = Convert.ToString(dr["JIRNH_HeaderMiscExpense"]),

                        //RN_TotalAmount = Convert.ToString(dr["JIRNH_Amount"]),

                        //RN_ExchangeRate = Convert.ToString(dr["JIRNH_ExchangeRate"]),

                        //RN_HeadGST_Amount = Convert.ToString(dr["JIRNH_GST_Amount"]),

                        //RN_ItemGST_Amount = Convert.ToString(dr["JIRNI_GST_Amount"]),

                        //RN_InvoiceAmount = Convert.ToString(dr["JIRNH_InvoiceAmount"]),

                        //RN_HeadWHT_Amount = Convert.ToString(dr["JIRNH_WHT_Amount"]),

                        //RN_ItemWHT_Amount = Convert.ToString(dr["JIRNI_WHT_Amount"]),

                        //RN_RoundOff = Convert.ToString(dr["JIRNH_RoundOff"]),

                        //RN_SupplierPayable = Convert.ToString(dr["JIRNH_SupplierPayable"]),

                        RN_Remarks = Convert.ToString(dr["JIRNH_Remarks"])
                    });
            }

            return RNList;
        }

        #region receiptnote Edit
        public List<ReceiptNoteHeadEdit_DTO> ReceiptNoteHeadEditSelfList(DataTable Dt)
        {
            List<ReceiptNoteHeadEdit_DTO> RNList = new List<ReceiptNoteHeadEdit_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                RNList.Add(
                    new ReceiptNoteHeadEdit_DTO
                    {
                        JIRNH_Number = Convert.ToInt64(dr["JIRNH_Number"]),

                        RN_No = Convert.ToString(dr["JIRNH_RN_No"]),

                        RN_Date = dr["JIRNH_RN_Date"] != DBNull.Value
                                    ? Convert.ToDateTime(dr["JIRNH_RN_Date"]).ToString("yyyy-MM-dd")
                                    : "",

                        JW_CustomerDC_No = Convert.ToString(dr["JIRNH_JW_CustomerDC_No"]),

                        JW_CustomerDC_Date = dr["JIRNH_JW_CustomerDC_Date"] != DBNull.Value
                                                ? Convert.ToDateTime(dr["JIRNH_JW_CustomerDC_Date"]).ToString("yyyy-MM-dd")
                                                : "",

                        MS_Number = Convert.ToString(dr["JIRNH_MS_Number"]),

                        JWC_Number = Convert.ToString(dr["JIRNH_JWC_Number"]),

                        JWC_Name = Convert.ToString(dr["CUS_Name"]),

                        Currency_Number = Convert.ToString(dr["JIRNH_Currency_Number"]),

                        Currency_Name = Convert.ToString(dr["CurrencyCode"]),

                        WH_Number = Convert.ToString(dr["JIRNH_WH_Number"]),

                        Remarks = Convert.ToString(dr["JIRNH_Remarks"])


                    });
            }

            return RNList;
        }
        public List<ReceiptNoteBatchEdit_DTO> ReceiptNoteBatchSelfList(DataTable Dt)
        {
            List<ReceiptNoteBatchEdit_DTO> BatchList = new List<ReceiptNoteBatchEdit_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                BatchList.Add(
                    new ReceiptNoteBatchEdit_DTO
                    {
                        RNI_BCH_No = Convert.ToInt64(dr["JIRNI_BCH_Number"]),

                        JIRNI_BCH_JIRNH_Number = Convert.ToInt64(dr["JIRNI_BCH_JIRNH_Number"]),

                        JIRNI_BCH_JIRNI_Number = Convert.ToInt64(dr["JIRNI_BCH_JIRNI_Number"]),

                        //  RNI_BCH_WH_Name = Convert.ToString(dr["RNI_BCH_WH_Name"]),

                        //   WH_Number = Convert.ToString(dr["WH_Number"]),

                        RNI_BCH_Date = Convert.ToString(dr["JIRNI_BCH_BatchDate"]),

                        RNI_BCH_WH_Number = Convert.ToString(dr["JIRNI_BCH_WH_Number"]),

                        RNI_BCH_OriginalQty = Convert.ToString(dr["JIRNI_BCH_BatchQty"]),

                        //RNI_BCH_Qty = Convert.ToString(dr["JIRNI_BCH_BatchQty"]),

                        RNI_BCH_UnitPrice = Convert.ToString(dr["JIRNI_BCH_BatchUnitPrice"]),

                        RNI_BCH_Value = Convert.ToString(dr["JIRNI_BCH_BatchValue"]),
                        RNI_BCH_Number = Convert.ToString(dr["JIRNI_BCH_BatchNo"]),


                        //  RNI_BCH_IsDeleted = Convert.ToString(dr["RNI_BCH_IsDeleted"]),

                        RNI_BCH_Item_Number = Convert.ToString(dr["JIRNI_BCH_JIRNI_Number"])
                    });
            }

            return BatchList;
        }

        public List<ReceiptNoteItemEdit_DTO> ReceiptNoteItemSelfList(DataTable Dt)
        {
            List<ReceiptNoteItemEdit_DTO> ItemList = new List<ReceiptNoteItemEdit_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                ItemList.Add(
                    new ReceiptNoteItemEdit_DTO
                    {
                        JIRNI_Number = Convert.ToInt64(dr["JIRNI_Number"]),

                        JIRNI_JIRNH_Number = Convert.ToInt64(dr["JIRNI_JIRNH_Number"]),

                        PRS_Number = Convert.ToString(dr["JIRNI_Number"]),

                        Item_Number = Convert.ToString(dr["JIRNI_Item_Number"]),

                        PRS_Name = Convert.ToString(dr["PRS_ProcessName"]),

                        //Description = Convert.ToString(dr["Description"]),

                        //OuterDia = Convert.ToString(dr["OuterDia"]),

                        //Thickness = Convert.ToString(dr["Thickness"]),

                        //Length = Convert.ToString(dr["Length"]),

                        //Width = Convert.ToString(dr["Width"]),

                        //MaterialGrade = Convert.ToString(dr["MaterialGrade"]),

                        //ItemGroup = Convert.ToString(dr["ItemGroup"]),

                        //WH_Number = Convert.ToString(dr["WH_Number"]),

                        //UoM_Number = Convert.ToString(dr["UoM_Number"]),

                        OriginalQty = Convert.ToString(dr["JIRNI_Qty"]),

                        UnitPrice = Convert.ToString(dr["JIRNI_UnitPrice"]),

                        Amount = Convert.ToString(dr["JIRNI_Amount"])


                    });
            }

            return ItemList;
        }
        #endregion
        #region receiptnote view
        public List<ReceiptNoteHead_DTO> ReceiptNoteHeadEditList(DataTable Dt)
        {
            List<ReceiptNoteHead_DTO> RNList = new List<ReceiptNoteHead_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                RNList.Add(
                    new ReceiptNoteHead_DTO
                    {
                        JIRNH_Number = Convert.ToInt64(dr["JIRNH_Number"]),

                        RN_No = Convert.ToString(dr["JIRNH_RN_No"]),

                        RN_Date = dr["JIRNH_RN_Date"] != DBNull.Value
                                    ? Convert.ToDateTime(dr["JIRNH_RN_Date"]).ToString("yyyy-MM-dd")
                                    : "",

                        JW_CustomerDC_No = Convert.ToString(dr["JIRNH_JW_CustomerDC_No"]),

                        JW_CustomerDC_Date = dr["JIRNH_JW_CustomerDC_Date"] != DBNull.Value
                                                ? Convert.ToDateTime(dr["JIRNH_JW_CustomerDC_Date"]).ToString("yyyy-MM-dd")
                                                : "",

                        MS_Number = Convert.ToString(dr["JIRNH_MS_Number"]),

                        JWC_Number = Convert.ToString(dr["JIRNH_JWC_Number"]),

                        JWC_Name = Convert.ToString(dr["CUS_Name"]),

                        Currency_Number = Convert.ToString(dr["JIRNH_Currency_Number"]),

                        Currency_Name = Convert.ToString(dr["CurrencyCode"]),

                        WH_Number = Convert.ToString(dr["JIRNH_WH_Number"]),

                        Remarks = Convert.ToString(dr["JIRNH_Remarks"])
 
                       
                    });
            }

            return RNList;
        }
        public List<ReceiptNoteBatch_DTO> ReceiptNoteBatchList(DataTable Dt)
        {
            List<ReceiptNoteBatch_DTO> BatchList = new List<ReceiptNoteBatch_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                BatchList.Add(
                    new ReceiptNoteBatch_DTO
                    {
                        RNI_BCH_No = Convert.ToInt64(dr["JIRNI_BCH_Number"]),

                        JIRNI_BCH_JIRNH_Number = Convert.ToInt64(dr["JIRNI_BCH_JIRNH_Number"]),

                        JIRNI_BCH_JIRNI_Number = Convert.ToInt64(dr["JIRNI_BCH_JIRNI_Number"]),

                      //  RNI_BCH_WH_Name = Convert.ToString(dr["RNI_BCH_WH_Name"]),

                     //   WH_Number = Convert.ToString(dr["WH_Number"]),

                        RNI_BCH_Date = Convert.ToString(dr["JIRNI_BCH_BatchDate"]),

                        RNI_BCH_WH_Number = Convert.ToString(dr["JIRNI_BCH_WH_Number"]),

                        RNI_BCH_Qty = Convert.ToString(dr["JIRNI_BCH_BatchQty"]),

                        RNI_BCH_UnitPrice = Convert.ToString(dr["JIRNI_BCH_BatchUnitPrice"]),

                        RNI_BCH_Value = Convert.ToString(dr["JIRNI_BCH_BatchValue"]),
                        RNI_BCH_Number = Convert.ToString(dr["JIRNI_BCH_BatchNo"]),
                       

                        //  RNI_BCH_IsDeleted = Convert.ToString(dr["RNI_BCH_IsDeleted"]),

                         RNI_BCH_Item_Number = Convert.ToString(dr["JIRNI_BCH_JIRNI_Number"])
                    });
            }

            return BatchList;
        }

        public List<ReceiptNoteItem_DTO> ReceiptNoteItemList(DataTable Dt)
        {
            List<ReceiptNoteItem_DTO> ItemList = new List<ReceiptNoteItem_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                ItemList.Add(
                    new ReceiptNoteItem_DTO
                    {
                        JIRNI_Number = Convert.ToInt64(dr["JIRNI_Number"]),

                        JIRNI_JIRNH_Number = Convert.ToInt64(dr["JIRNI_JIRNH_Number"]),

                        PRS_Number = Convert.ToString(dr["JIRNI_Number"]),

                        Item_Number = Convert.ToString(dr["JIRNI_Item_Number"]),

                        PRS_Name = Convert.ToString(dr["PRS_ProcessName"]),
                        
                        //Description = Convert.ToString(dr["Description"]),

                        //OuterDia = Convert.ToString(dr["OuterDia"]),

                        //Thickness = Convert.ToString(dr["Thickness"]),

                        //Length = Convert.ToString(dr["Length"]),

                        //Width = Convert.ToString(dr["Width"]),

                        //MaterialGrade = Convert.ToString(dr["MaterialGrade"]),

                        //ItemGroup = Convert.ToString(dr["ItemGroup"]),

                        //WH_Number = Convert.ToString(dr["WH_Number"]),

                        //UoM_Number = Convert.ToString(dr["UoM_Number"]),

                        Qty = Convert.ToString(dr["JIRNI_Qty"]),

                        UnitPrice = Convert.ToString(dr["JIRNI_UnitPrice"]),

                        Amount = Convert.ToString(dr["JIRNI_Amount"])

                      
                    });
            }

            return ItemList;
        }
        #endregion

    }
}

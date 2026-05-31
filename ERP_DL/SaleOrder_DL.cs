using ERP_DAO;
using ERP_DTO;
using System;
using System.Data;
using System.Globalization;

namespace ERP.DataList
{
    public class SaleOrder_DL
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
                    });
            }
            return BList;
        }
        public List<SalesOrderRegister_DTO> SOSummaryList(DataTable Dt)
        {
            List<SalesOrderRegister_DTO> SOList = new List<SalesOrderRegister_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new SalesOrderRegister_DTO
                    {
                        SO_Number = Convert.ToInt64(dr["SOH_Number"]),
                        SO_RegNo = Convert.ToString(dr["SOH_RegNo"]),
                        SO_RegDate = Convert.ToString(dr["SOH_RegDate"]),
                        SO_OrderNo = Convert.ToString(dr["SOH_OrderNo"]),
                        SO_OrderDate = Convert.ToString(dr["SOH_OrderDate"]),
                        SO_ExportOrder = Convert.ToString(dr["SOH_ExportOrder"]) == "1"?"Yes":"No",
                        SO_BYC_Category = Convert.ToString(dr["SOH_BYC_Category"]),
                        SO_BYG_Group = Convert.ToString(dr["SOH_BYG_Group"]),
                        SO_BUY_Name = Convert.ToString(dr["SOH_BUY_Name"]),
                        SO_MS_Name = Convert.ToString(dr["SOH_MS_Name"]),
                        SO_NoOfItem = Convert.ToString(dr["SOI_NoOfItem"]),
                        SO_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_Qty"])),
                        SO_CUR_Name = Convert.ToString(dr["SOH_CUR_Name"]),
                        SO_TotalAmount = Convert.ToString(dr["SOH_TotalAmount"]),
                        SO_TotalItemIncome = Convert.ToString(dr["SOH_TotalItemIncome"]),
                        SO_TotalHeadIncome = Convert.ToString(dr["SOH_TotalHeadIncome"]),
                        SO_OrderValue = Convert.ToString(dr["SOH_OrderValue"]),
                        SO_ExchangeRate = Convert.ToString(dr["SOH_ExchangeRate"])
                    });
            }
            return SOList;
        }
        public List<SalesOrderRegister_DTO> SODetailedList(DataTable Dt)
        {
            List<SalesOrderRegister_DTO> SOList = new List<SalesOrderRegister_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new SalesOrderRegister_DTO
                    {
                        SO_Number = Convert.ToInt64(dr["SOH_Number"]),
                        SO_RegNo = Convert.ToString(dr["SOH_RegNo"]),
                        SO_RegDate = Convert.ToString(dr["SOH_RegDate"]),
                        SO_OrderNo = Convert.ToString(dr["SOH_OrderNo"]),
                        SO_OrderDate = Convert.ToString(dr["SOH_OrderDate"]),
                        SO_ExportOrder = Convert.ToString(dr["SOH_ExportOrder"]) == "1" ? "Yes" : "No",
                        SO_BYC_Category = Convert.ToString(dr["SOH_BYC_Category"]),
                        SO_BYG_Group = Convert.ToString(dr["SOH_BYG_Group"]),
                        SO_BUY_Name = Convert.ToString(dr["SOH_BUY_Name"]),
                        SO_MS_Name = Convert.ToString(dr["SOH_MS_Name"]),
                        SO_ITM_Group = Convert.ToString(dr["SOH_ITM_Group"]),
                        SO_ITM_Code = Convert.ToString(dr["SOI_ITM_Code"]),
                        SO_ITM_Description = Convert.ToString(dr["SOI_ITM_Description"]),
                        SO_ITM_OuterDia = Convert.ToString(dr["SOI_ITM_OuterDia"]),
                        SO_ITM_MaterialGrade = Convert.ToString(dr["SOI_ITM_MaterialGrade"]),
                        SO_ITM_Thickness = Convert.ToString(dr["SOI_ITM_Thickness"]),
                        SO_ITM_Length = Convert.ToString(dr["SOI_ITM_Length"]),
                        SO_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_Qty"])),
                        SO_UOM_Name = Convert.ToString(dr["SOI_UOM_Name"]),
                        SO_DeliveryDate = Convert.ToString(dr["SOI_DeliveryDate"]),
                        SO_CUR_Name = Convert.ToString(dr["SOH_CUR_Name"]),
                        SO_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SOI_UnitPrice"])),
                        SO_TotalAmount = Convert.ToString(dr["SOI_Amount"]),
                        SO_TotalItemIncome = Convert.ToString(dr["SOI_IncomeValue"]),
                        SO_TotalHeadIncome = Convert.ToString(dr["SOH_TotalHeadIncome"]),
                        SO_OrderValue = Convert.ToString(dr["SOI_TotalValue"]),
                        SO_ExchangeRate = Convert.ToString(dr["SOH_ExchangeRate"])
                    });
            }
            return SOList;
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


        ////PREVIEW
        //public List<SaleOrderHead_DTO> SOHeadList(DataTable Dt)
        //{
        //    List<SaleOrderHead_DTO> SOList = new List<SaleOrderHead_DTO>();
        //    foreach (DataRow dr in Dt.Rows)
        //    {
        //        SOList.Add(
        //            new SaleOrderHead_DTO
        //            {
        //                SOH_Number = Convert.ToInt64(dr["SOH_Number"]),
        //                SOH_OrderNo = Convert.ToString(dr["SOH_OrderNo"]),
        //                SOH_ExportOrder = Convert.ToString(dr["SOH_ExportOrder"]),
        //                SOH_OrderDate = Convert.ToString(dr["SOH_OrderDate"]),
        //                SOH_BUY_Number = Convert.ToString(dr["SOH_BUY_Number"]),
        //                SOH_CUR_Name = Convert.ToString(dr["SOH_Currency"]),
        //                SOH_MS_Number = Convert.ToString(dr["SOH_MS_Number"]),
        //                SOH_PaymentTerms = Convert.ToString(dr["SOH_PaymentTerms"]),
        //                SOH_MOP = Convert.ToString(dr["SOH_MOP"]),
        //                SOH_DeliveryTerms = Convert.ToString(dr["SOH_DeliveryTerms"]),
        //                SOH_MOD = Convert.ToString(dr["SOH_MOD"]),
        //                TotalQty = Convert.ToString(dr["TotalQty"]),
        //                MaterialValue = Convert.ToString(dr["MaterialValue"]),
        //            });
        //    }
        //    return SOList;
        //}
        public List<SaleOrderItem_DTO> SOItemPreviewList(DataTable Dt)
        {
            List<SaleOrderItem_DTO> SOList = new List<SaleOrderItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new SaleOrderItem_DTO
                    {
                        SOI_Number = Convert.ToInt64(dr["SOI_Number"]),
                        SOI_MS_Number = Convert.ToInt64(dr["SOI_MS_Number"]),
                        SOI_ITM_Number = Convert.ToInt64(dr["SOI_ITM_Number"]),
                        SOI_ITM_Code = Convert.ToString(dr["SOI_ITM_Code"]),
                        SOI_ITM_Group = Convert.ToString(dr["SOI_ITM_Group"]),
                        SOI_ITM_Thickness = Convert.ToString(dr["SOI_ITM_Thickness"]),
                        SOI_ITM_OuterDia = Convert.ToString(dr["SOI_ITM_OuterDia"]),
                        SOI_ITM_Length = Convert.ToString(dr["SOI_ITM_Length"]),
                        SOI_ITM_MaterialGrade = Convert.ToString(dr["SOI_ITM_MaterialGrade"]),
                        SOI_ITM_Description = Convert.ToString(dr["SOI_ITM_Description"]),
                        SOI_UoM_Name = Convert.ToString(dr["SOI_UoM_Name"]),
                        SOI_UoM_Number = Convert.ToString(dr["SOI_UoM_Number"]),
                        SOI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_Qty"])),
                        SOI_SII_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_SII_Qty"])),
                        SOI_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SOI_UnitPrice"])),
                        SOI_Amount = Convert.ToString(dr["SOI_Amount"]),
                        SOI_IncomeValue = Convert.ToString(dr["SOI_IncomeValue"]),
                        SOI_DeliveryDate = Convert.ToString(dr["SOI_DeliveryDate"]),
                        SOI_UoM_DecimalPlaces = Convert.ToString(dr["SOI_UoM_DecimalPlaces"]),
                        SOI_IsDeleted = 0
                    });
            }
            return SOList;
        }


        public List<SaleOrderHead_DTO> SOHeadEditList(DataTable Dt)
        {
            List<SaleOrderHead_DTO> SOList = new List<SaleOrderHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new SaleOrderHead_DTO
                    {
                        SOH_Number = Convert.ToInt64(dr["SOH_Number"]),
                        SOH_OrderNo = Convert.ToString(dr["SOH_OrderNo"]),
                        SOH_OrderDate = Convert.ToString(dr["SOH_OrderDate"]),
                        SOH_RegNo = Convert.ToString(dr["SOH_RegNo"]),
                        SOH_RegDate = Convert.ToString(dr["SOH_RegDate"]),
                        SOH_ExportOrder = Convert.ToString(dr["SOH_ExportOrder"]) == "1" ? "true" : "false",
                        SOH_BUY_Number = Convert.ToString(dr["SOH_BUY_Number"]),
                        SOH_BUY_Name = Convert.ToString(dr["SOH_BUY_Name"]),
                        SOH_BUY_LOC_Number = Convert.ToString(dr["SOH_BUY_LOC_Number"]),
                        SOH_CUR_Number = Convert.ToString(dr["SOH_CUR_Number"]),
                        SOH_CUR_Name = Convert.ToString(dr["SOH_CUR_Name"]),
                        SOH_CUR_DecimalPlaces = Convert.ToString(dr["SOH_CUR_DecimalPlaces"]),
                        SOH_MS_Number = Convert.ToString(dr["SOH_MS_Number"]),
                        SOH_PaymentTerms = Convert.ToString(dr["SOH_PaymentTerms"]),
                        SOH_PaymentMethod = Convert.ToString(dr["SOH_PaymentMethod"]),
                        SOH_DeliveryTerms = Convert.ToString(dr["SOH_DeliveryTerms"]),
                        SOH_DeliveryMethod = Convert.ToString(dr["SOH_DeliveryMethod"]),
                        SOH_QCR = Convert.ToString(dr["SOH_QCR"]),
                        SOH_ExchangeRate = Convert.ToString(dr["SOH_ExchangeRate"]),
                        SOH_TDC = Convert.ToString(dr["SOH_TDC"]),
                        SOH_OtherRemarks = Convert.ToString(dr["SOH_OtherRemarks"]),
                        SOH_TotalAmount = Convert.ToString(dr["SOH_TotalAmount"]),
                        SOH_TotalItemIncome = Convert.ToString(dr["SOH_TotalItemIncome"]),
                        SOH_TotalHeadIncome = Convert.ToString(dr["SOH_TotalHeadIncome"]),
                        SOH_OrderValue = Convert.ToString(dr["SOH_OrderValue"]),
                    });
            }
            return SOList;
        }
        public List<SaleOrderItem_DTO> SOItemEditList(DataTable Dt)
        {
            List<SaleOrderItem_DTO> SOList = new List<SaleOrderItem_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new SaleOrderItem_DTO
                    {
                        SOI_Number = Convert.ToInt64(dr["SOI_Number"]),
                        SOI_MS_Number = Convert.ToInt64(dr["SOI_MS_Number"]),
                        SOI_ITM_Number = Convert.ToInt64(dr["SOI_ITM_Number"]),
                        SOI_ITM_Code = Convert.ToString(dr["SOI_ITM_Code"]),
                        SOI_ITM_Group = Convert.ToString(dr["SOI_ITM_Group"]),
                        SOI_ITM_Thickness = Convert.ToString(dr["SOI_ITM_Thickness"]),
                        SOI_ITM_OuterDia = Convert.ToString(dr["SOI_ITM_OuterDia"]),
                        SOI_ITM_Length = Convert.ToString(dr["SOI_ITM_Length"]),
                        SOI_ITM_MaterialGrade = Convert.ToString(dr["SOI_ITM_MaterialGrade"]),
                        SOI_ITM_Description = Convert.ToString(dr["SOI_ITM_Description"]),
                        SOI_UoM_Number = Convert.ToString(dr["SOI_UoM_Number"]),
                        SOI_OLD_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_Qty"])),
                        SOI_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_Qty"])),
                        SOI_SII_Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["SOI_SII_Qty"])),
                        SOI_UnitPrice = MH.DecimalConvertUnit(Convert.ToDouble(dr["SOI_UnitPrice"])),
                        SOI_Amount = Convert.ToString(dr["SOI_Amount"]),
                        SOI_IncomeValue = Convert.ToString(dr["SOI_IncomeValue"]),
                        SOI_DeliveryDate = Convert.ToString(dr["SOI_DeliveryDate"]),
                        SOI_UoM_DecimalPlaces = Convert.ToString(dr["SOI_UoM_DecimalPlaces"]),
                        SOI_IsDeleted = 0
                    });
            }
            return SOList;
        }
        public List<SaleOrderIncome_DTO> SOIncomeEditList(DataTable Dt)
        {
            List<SaleOrderIncome_DTO> SOList = new List<SaleOrderIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new SaleOrderIncome_DTO
                    {
                        SOH_INC_Number = Convert.ToInt64(dr["SOH_INC_Number"]),
                        SOH_INC_MIC_Number = Convert.ToString(dr["SOH_INC_MIC_Number"]),
                        SOH_INC_MIC_Description = Convert.ToString(dr["SOH_INC_MIC_Description"]),
                        SOH_INC_Remarks = Convert.ToString(dr["SOH_INC_Remarks"]),
                        SOH_INC_OCRN_Number = Convert.ToString(dr["SOH_INC_OCRN_Number"]),
                        SOH_INC_CM_Number = Convert.ToString(dr["SOH_INC_CM_Number"]),
                        SOH_INC_IncomeBase = Convert.ToString(dr["SOH_INC_IncomeBase"]),
                        SOH_INC_IncomeValue = Convert.ToString(dr["SOH_INC_IncomeValue"]),
                        SOH_INC_ALCT_Number = Convert.ToString(dr["SOH_INC_ALCT_Number"]),
                        SOH_INC_LA_Number = Convert.ToString(dr["SOH_INC_LA_Number"]),
                        SOH_INC_IsDeleted = 0
                    });
            }
            return SOList;
        }
        public List<SaleOrderIIncome_DTO> SOIIncomeEditList(DataTable Dt)
        {
            List<SaleOrderIIncome_DTO> SOList = new List<SaleOrderIIncome_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new SaleOrderIIncome_DTO
                    {
                        SOI_INC_Number = Convert.ToInt64(dr["SOI_INC_Number"]),
                        SOI_INC_SOI_Number = Convert.ToInt64(dr["SOI_INC_SOI_Number"]),
                        SOI_INC_MIC_Number = Convert.ToString(dr["SOI_INC_MIC_Number"]),
                        SOI_INC_MIC_Description = Convert.ToString(dr["SOI_INC_MIC_Description"]),
                        SOI_INC_Remarks = Convert.ToString(dr["SOI_INC_Remarks"]),
                        SOI_INC_OCRN_Number = Convert.ToString(dr["SOI_INC_OCRN_Number"]),
                        SOI_INC_CM_Number = Convert.ToString(dr["SOI_INC_CM_Number"]),
                        SOI_INC_IncomeBase = Convert.ToString(dr["SOI_INC_IncomeBase"]),
                        SOI_INC_IncomeValue = Convert.ToString(dr["SOI_INC_IncomeValue"]),
                        SOI_INC_ALCT_Number = Convert.ToString(dr["SOI_INC_ALCT_Number"]),
                        SOI_INC_LA_Number = Convert.ToString(dr["SOI_INC_LA_Number"]),
                        SOI_INC_IsDeleted = 0
                    });
            }
            return SOList;
        }
        public List<SaleOrderAddress_DTO> SOHAddressEditList(DataTable Dt)
        {
            List<SaleOrderAddress_DTO> SOList = new List<SaleOrderAddress_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new SaleOrderAddress_DTO
                    {
                        SOH_ADD_Number = Convert.ToInt64(dr["SOH_ADD_Number"]),
                        SOH_ADD_ADTP_Number = Convert.ToInt64(dr["SOH_ADD_ADTP_Number"]),
                        SOH_ADD_AddressID = Convert.ToString(dr["SOH_ADD_AddressID"]),
                        SOH_ADD_Address = Convert.ToString(dr["SOH_ADD_Address"]),
                        SOH_ADD_City = Convert.ToString(dr["SOH_ADD_City"]),
                        SOH_ADD_State = Convert.ToString(dr["SOH_ADD_State"]),
                        SOH_ADD_Country = Convert.ToString(dr["SOH_ADD_Country"]),
                        SOH_ADD_Pin = Convert.ToString(dr["SOH_ADD_Pin"]),
                        SOH_ADD_GSTIN = Convert.ToString(dr["SOH_ADD_GSTIN"]),
                        SOH_ADD_IsDeleted = 0
                    });
            }
            return SOList;
        }
    }
}

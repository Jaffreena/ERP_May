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
    public class DeliveryNote_DL
    {
        MethodHelp MH = new MethodHelp();
        public List<DeliveryNoteSummary_DTO> DNSummaryList(DataTable Dt)
        {
            List<DeliveryNoteSummary_DTO> DNList =
                new List<DeliveryNoteSummary_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                DNList.Add(
                    new DeliveryNoteSummary_DTO
                    {
                        JIDNH_Number =
                            dr["JIDNH_Number"] == DBNull.Value ||
                            string.IsNullOrWhiteSpace(dr["JIDNH_Number"].ToString())
                            ? 0
                            : Convert.ToInt64(dr["JIDNH_Number"]),

                        JIDNH_DN_No =
                            Convert.ToString(dr["JIDNH_DN_No"]),

                        JIDNH_DN_Date =
                            dr["JIDNH_DN_Date"] == DBNull.Value
                            ? DateTime.MinValue
                            : Convert.ToDateTime(dr["JIDNH_DN_Date"]),

                        CUS_Name =
                            Convert.ToString(dr["CUS_Name"]),

                        CurrencyCode =
                            Convert.ToString(dr["CurrencyCode"]),

                        WarehouseCode =
                            Convert.ToString(dr["WarehouseCode"]),

                        JIDNH_MS_Number =
                            dr["JIDNH_MS_Number"] == DBNull.Value ||
                            string.IsNullOrWhiteSpace(dr["JIDNH_MS_Number"].ToString())
                            ? 0
                            : Convert.ToInt64(dr["JIDNH_MS_Number"]),

                        Segregation =
                            Convert.ToString(dr["Segregation"]),

                        NoOfLineItems =
                            dr["NoOfLineItems"] == DBNull.Value ||
                            string.IsNullOrWhiteSpace(dr["NoOfLineItems"].ToString())
                            ? 0
                            : Convert.ToInt32(dr["NoOfLineItems"]),

                        Qty =
                            MH.DecimalConvertQty(
                                dr["Qty"] == DBNull.Value ||
                                string.IsNullOrWhiteSpace(dr["Qty"].ToString())
                                ? 0
                                : Convert.ToDouble(dr["Qty"])),

                        Amount =
                            dr["Amount"] == DBNull.Value ||
                            string.IsNullOrWhiteSpace(dr["Amount"].ToString())
                            ? 0
                            : Convert.ToDouble(dr["Amount"]),

                        JCG_JW_CustomerGroup =
                            Convert.ToString(dr["JCG_JW_CustomerGroup"]),

                        JCC_JW_CustomerCategory =
                            Convert.ToString(dr["JCC_JW_CustomerCategory"]),

                        CustomerWareHouse =
                            Convert.ToString(dr["CustomerWareHouse"]),

                        ItemWareHouse =
                            Convert.ToString(dr["ItemWareHouse"])
                    });
            }

            return DNList;
        }
        public List<DeliveryNoteCreate_DTO> DeliveryNoteViewList(DataSet DS)
        {
            List<DeliveryNoteCreate_DTO> DNList =
                new List<DeliveryNoteCreate_DTO>();
            if (DS != null && DS.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    DeliveryNoteCreate_DTO dto =
                        new DeliveryNoteCreate_DTO();

                    //-----------------------------------------
                    // HEADER
                    //-----------------------------------------
                    dto.Header = new DeliveryNoteHeader_DTO
                    {
                        JIDNH_Number =
                            Convert.ToInt64(dr["JIDNH_Number"]),

                        JIDNH_DN_No =
                            Convert.ToString(dr["JIDNH_DN_No"]),

                        JIDNH_DN_Date =
                            Convert.ToDateTime(dr["JIDNH_DN_Date"]),

                        JIDNH_MS_Number =
                            Convert.ToInt64(dr["JIDNH_MS_Number"]),

                        JIDNH_JW_Customer_Number =
                            Convert.ToInt64(dr["JIDNH_JW_Customer_Number"]),

                        JIDNH_Currency_Number =
                            Convert.ToInt64(dr["JIDNH_Currency_Number"]),

                        JIDNH_WH_Number =
                            Convert.ToInt64(dr["JIDNH_WH_Number"]),

                        JIDNH_PaymentTerms =
                            Convert.ToString(dr["JIDNH_PaymentTerms"]),

                        JIDNH_DeliveryTerms =
                            Convert.ToString(dr["JIDNH_DeliveryTerms"]),

                        JIDNH_DeliveryMode =
                            Convert.ToString(dr["JIDNH_DeliveryMode"]),

                        JIDNH_DespatchDocumentNo =
                            Convert.ToString(dr["JIDNH_DespatchDocumentNo"]),

                        JIDNH_DespatchedThrough =
                            Convert.ToString(dr["JIDNH_DespatchedThrough"]),

                        JIDNH_Remarks =
                            Convert.ToString(dr["JIDNH_Remarks"]),
                        JIDNH_Warehouse =
                            Convert.ToString(dr["JIDNH_Warehouse"]),
                        JIDNH_CustomerName =
                            Convert.ToString(dr["JIDNH_CustomerName"]),
                        JIDNH_CurrencyCode =
                            Convert.ToString(dr["JIDNH_CurrencyCode"]),
                        JIDNH_Segregation =
                            Convert.ToString(dr["JIDNH_Segregation"])
                    };

                    //-----------------------------------------
                    // ITEM LIST
                    //-----------------------------------------
                    dto.Items = new List<DeliveryNoteItem_DTO>();

                    foreach (DataRow item in DS.Tables[1].Rows)
                    {
                        dto.Items.Add(
                            new DeliveryNoteItem_DTO
                            {
                                JIDNI_Number =
                                    Convert.ToInt64(item["JIDNI_Number"]),

                                JIDNI_JIDNH_Number =
                                    Convert.ToInt64(item["JIDNI_JIDNH_Number"]),

                                JIDNI_PRS_Number =
                                    Convert.ToInt64(item["JIDNI_PRS_Number"]),

                                JIDNI_Item_Number =
                                    Convert.ToInt64(item["JIDNI_Item_Number"]),

                                JIDNI_WH_Number =
                                    Convert.ToInt64(item["JIDNI_WH_Number"]),

                                JIDNI_UoM_Number =
                                    Convert.ToInt64(item["JIDNI_UoM_Number"]),

                                JIDNI_Qty =
                                    Convert.ToDouble(item["JIDNI_Qty"]),

                                JIDNI_UnitPrice =
                                    Convert.ToDouble(item["JIDNI_UnitPrice"]),

                                JIDNI_Amount =
                                    Convert.ToDouble(item["JIDNI_Amount"]),

                                JIDNI_JW_InvoiceTracking =
                                    Convert.ToString(item["JIDNI_JW_InvoiceTracking"]),
                                JIDNI_JW_ProcessName =
                                Convert.ToString(item["JIDNI_JW_ProcessName"]),
                                JIDNI_JW_ItemName =
                                Convert.ToString(item["JIDNI_JW_ItemName"]),

                                JIDNI_JW_ItemDescription =
        Convert.ToString(item["JIDNI_JW_ItemDescription"]),

                                JIDNI_JW_OuterDia =
        Convert.ToString(item["JIDNI_JW_OuterDia"]),

                                JIDNI_JW_Thickness =
        Convert.ToString(item["JIDNI_JW_Thickness"]),

                                JIDNI_JW_Length =
        Convert.ToString(item["JIDNI_JW_Length"]),

                                JIDNI_JW_ITM_Width =
        Convert.ToString(item["JIDNI_JW_ITM_Width"]),

                                JIDNI_JW_MaterialGrade =
        Convert.ToString(item["JIDNI_JW_MaterialGrade"]),
                                JIDNI_UOM =
        Convert.ToString(item["JIDNI_UOM"]),
                                JIDNI_Warehouse =
        Convert.ToString(item["JIDNI_Warehouse"])
                            });
                    }

                    //-----------------------------------------
                    // ADDRESS LIST
                    //-----------------------------------------
                    dto.Addresses = new List<DeliveryNoteAddress_DTO>();

                    foreach (DataRow add in DS.Tables[2].Rows)
                    {
                        dto.Addresses.Add(
                            new DeliveryNoteAddress_DTO
                            {
                                JIDNA_Number =
                                    Convert.ToInt64(add["JIDNA_Number"]),

                                JIDNA_JIDNH_Number =
                                    Convert.ToInt64(add["JIDNA_JIDNH_Number"]),

                                JIDNA_ADTP_Number =
                                    Convert.ToInt64(add["JIDNA_ADTP_Number"]),

                                JIDNA_Address_ID =
                                    Convert.ToString(add["JIDNA_Address_ID"]),

                                JIDNA_Address =
                                    Convert.ToString(add["JIDNA_Address"]),

                                JIDNA_City =
                                    Convert.ToString(add["JIDNA_City"]),

                                JIDNA_State =
                                    Convert.ToString(add["JIDNA_State"]),

                                JIDNA_Country =
                                    Convert.ToString(add["JIDNA_Country"]),

                                JIDNA_PIN =
                                    Convert.ToString(add["JIDNA_PIN"]),

                                JIDNA_GSTIN =
                                    Convert.ToString(add["JIDNA_GSTIN"])
                            });
                    }

                    DNList.Add(dto);
                }
            }
            return DNList;
        }
        public List<DeliveryNoteDetailed_DTO> DNDetailedList(DataTable Dt)
        {
            List<DeliveryNoteDetailed_DTO> DNList =
                new List<DeliveryNoteDetailed_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                DNList.Add(
                    new DeliveryNoteDetailed_DTO
                    {
                        JIDNH_Number =
                            dr["JIDNH_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["JIDNH_Number"]),

                        JIDNH_DN_No =
                            dr["JIDNH_DN_No"] == DBNull.Value ? "" : Convert.ToString(dr["JIDNH_DN_No"]),

                        JIDNH_DN_Date =
                            dr["JIDNH_DN_Date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["JIDNH_DN_Date"]),

                        JIDNH_JW_Customer_Number =
                            dr["JIDNH_JW_Customer_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["JIDNH_JW_Customer_Number"]),

                        CUS_JCG_Number =
                            dr["CUS_JCG_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["CUS_JCG_Number"]),

                        JCG_JW_CustomerGroup =
                            dr["JCG_JW_CustomerGroup"] == DBNull.Value ? "" : Convert.ToString(dr["JCG_JW_CustomerGroup"]),

                        JCC_JW_CustomerCategory =
                            dr["JCC_JW_CustomerCategory"] == DBNull.Value ? "" : Convert.ToString(dr["JCC_JW_CustomerCategory"]),

                        CUS_Name =
                            dr["CUS_Name"] == DBNull.Value ? "" : Convert.ToString(dr["CUS_Name"]),

                        CurrencyCode =
                            dr["CurrencyCode"] == DBNull.Value ? "" : Convert.ToString(dr["CurrencyCode"]),

                        WarehouseCode =
                            dr["WarehouseCode"] == DBNull.Value ? "" : Convert.ToString(dr["WarehouseCode"]),

                        JIDNH_MS_Number =
                            dr["JIDNH_MS_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["JIDNH_MS_Number"]),

                        CustomerWareHouse =
                            dr["CustomerWareHouse"] == DBNull.Value ? "" : Convert.ToString(dr["CustomerWareHouse"]),

                        ItemWareHouse =
                            dr["ItemWareHouse"] == DBNull.Value ? "" : Convert.ToString(dr["ItemWareHouse"]),

                        Segregation =
                            dr["Segregation"] == DBNull.Value ? "" : Convert.ToString(dr["Segregation"]),

                        JIDNI_PRS_Number =
                            dr["JIDNI_PRS_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["JIDNI_PRS_Number"]),

                        JIDNI_Item_Number =
                            dr["JIDNI_Item_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["JIDNI_Item_Number"]),

                        Process =
                            dr["Process"] == DBNull.Value ? "" : Convert.ToString(dr["Process"]),

                        ItemGroup =
                            dr["ItemGroup"] == DBNull.Value ? "" : Convert.ToString(dr["ItemGroup"]),

                        ItemCode =
                            dr["ItemCode"] == DBNull.Value ? "" : Convert.ToString(dr["ItemCode"]),

                        ItemDescription =
                            dr["ItemDescription"] == DBNull.Value ? "" : Convert.ToString(dr["ItemDescription"]),

                        OuterDia =
                            dr["OuterDia"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["OuterDia"]),

                        Thickness =
                            dr["Thickness"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Thickness"]),

                        ItemLength =
                            dr["ItemLength"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ItemLength"]),

                        ITM_Width =
                            dr["ITM_Width"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["ITM_Width"]),

                        MaterialGrade =
                            dr["MaterialGrade"] == DBNull.Value ? "" : Convert.ToString(dr["MaterialGrade"]),

                        Warehouse =
                            dr["Warehouse"] == DBNull.Value ? "" : Convert.ToString(dr["Warehouse"]),

                        UOM =
                            dr["UOM"] == DBNull.Value ? "" : Convert.ToString(dr["UOM"]),

                        JIDNI_Qty =
                            dr["JIDNI_Qty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["JIDNI_Qty"]),

                        JIDNI_Amount =
                            dr["JIDNI_Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["JIDNI_Amount"]),

                        JIDNI_JW_InvoiceTracking =
                            dr["JIDNI_JW_InvoiceTracking"] == DBNull.Value ? "" : Convert.ToString(dr["JIDNI_JW_InvoiceTracking"])
                    });
            }

            return DNList;
        }

    }
}

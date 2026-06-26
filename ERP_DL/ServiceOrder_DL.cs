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
    public class ServiceOrder_DL
    {
        MethodHelp MH = new MethodHelp();
        public List<ServiceOrderDetailed_DTO> SODetailedList(DataTable Dt)
        {
            List<ServiceOrderDetailed_DTO> SOList =
                new List<ServiceOrderDetailed_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(
                    new ServiceOrderDetailed_DTO
                    {
                        JISVOH_Number =
                            dr["JISVOH_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["JISVOH_Number"]),

                        JISVOH_RegNo =
                            dr["JISVOH_RegNo"] == DBNull.Value ? "" : Convert.ToString(dr["JISVOH_RegNo"]),

                        JISVOH_RegDate =
                            dr["JISVOH_RegDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["JISVOH_RegDate"]),

                        JISVOH_ServiceOrderNo =
                            dr["JISVOH_ServiceOrderNo"] == DBNull.Value ? "" : Convert.ToString(dr["JISVOH_ServiceOrderNo"]),

                        JISVOH_ServiceOrderDate =
                            dr["JISVOH_ServiceOrderDate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["JISVOH_ServiceOrderDate"]),

                        JISVOH_JW_Customer_Number =
                            dr["JISVOH_JW_Customer_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["JISVOH_JW_Customer_Number"]),

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

                        JISVOI_PRS_Number =
                            dr["JISVOI_PRS_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["JISVOI_PRS_Number"]),

                        JISVOI_Item_Number =
                            dr["JISVOI_Item_Number"] == DBNull.Value ? 0 : Convert.ToInt64(dr["JISVOI_Item_Number"]),

                        PRS_ProcessName =
                            dr["Process"] == DBNull.Value ? "" : Convert.ToString(dr["Process"]),

                        Segregation =
                            dr.Table.Columns.Contains("Segregation") && dr["Segregation"] != DBNull.Value ? Convert.ToString(dr["Segregation"]) : "",

                        ItemGroup =
                            dr["ItemGroup"] == DBNull.Value ? "" : Convert.ToString(dr["ItemGroup"]),

                        ItemCode =
                            dr["ItemCode"] == DBNull.Value ? "" : Convert.ToString(dr["ItemCode"]),

                        ItemDescription =
                            dr["ItemDescription"] == DBNull.Value ? "" : Convert.ToString(dr["ItemDescription"]),

                        OuterDia =
                            dr["OuterDia"] == DBNull.Value ? "" : Convert.ToString(dr["OuterDia"]),

                        Thickness =
                            dr["Thickness"] == DBNull.Value ? "" : Convert.ToString(dr["Thickness"]),

                        ItemLength =
                            dr["ItemLength"] == DBNull.Value ? "" : Convert.ToString(dr["ItemLength"]),

                        ITM_Width =
                            dr["ITM_Width"] == DBNull.Value ? "" : Convert.ToString(dr["ITM_Width"]),

                        MaterialGrade =
                            dr["MaterialGrade"] == DBNull.Value ? "" : Convert.ToString(dr["MaterialGrade"]),

                        WarehouseCode =
                            dr.Table.Columns.Contains("Warehouse") && dr["Warehouse"] != DBNull.Value ? Convert.ToString(dr["Warehouse"]) : "",

                        UOM =
                            dr["UOM"] == DBNull.Value ? "" : Convert.ToString(dr["UOM"]),

                        Qty =
                            dr["JISVOI_Qty"] == DBNull.Value ? "0" : MH.DecimalConvertQty(Convert.ToDouble(dr["JISVOI_Qty"])),

                        UnitPrice =
                            dr.Table.Columns.Contains("JISVOI_UnitPrice") && dr["JISVOI_UnitPrice"] != DBNull.Value ? Convert.ToDouble(dr["JISVOI_UnitPrice"]) : 0,

                        Amount =
                            dr["JISVOI_Amount"] == DBNull.Value ? 0 : Convert.ToDouble(dr["JISVOI_Amount"]),

                        DeliveryDate =
                            dr["JISVOI_DeliveryDate"] == DBNull.Value ? null : Convert.ToDateTime(dr["JISVOI_DeliveryDate"])
                    });
            }

            return SOList;
        }
        public List<ServiceOrderSummary_DTO> SOSummaryList(DataTable Dt)
        {
            List<ServiceOrderSummary_DTO> SOList = new List<ServiceOrderSummary_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                SOList.Add(new ServiceOrderSummary_DTO
                {
                    JISVOH_Number = Convert.ToInt64(dr["JISVOH_Number"]),
                    JISVOH_RegNo = Convert.ToString(dr["JISVOH_RegNo"]),
                    JISVOH_RegDate = Convert.ToDateTime(dr["JISVOH_RegDate"]),
                    JISVOH_ServiceOrderNo = Convert.ToString(dr["JISVOH_ServiceOrderNo"]),
                    JISVOH_ServiceOrderDate = Convert.ToDateTime(dr["JISVOH_ServiceOrderDate"]),
                    CUS_Name = Convert.ToString(dr["CUS_Name"]),
                    CurrencyCode = Convert.ToString(dr["CurrencyCode"]),
                    PRS_ProcessName = Convert.ToString(dr["Process"]),
                    NoOfLineItems = Convert.ToInt32(dr["NoOfLineItems"]),
                    Qty = MH.DecimalConvertQty(Convert.ToDouble(dr["Qty"])),
                    Amount = Convert.ToDouble(dr["Amount"]),
                    JCG_JW_CustomerGroup = Convert.ToString(dr["JCG_JW_CustomerGroup"]),
                    JCC_JW_CustomerCategory = Convert.ToString(dr["JCC_JW_CustomerCategory"])
                });
            }

            return SOList;
        }
    }
}

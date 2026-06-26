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
    public class JW_Invoice_DL
    {
        MethodHelp MH = new MethodHelp();
        public List<JobInwardInvoiceGst> SaleInvGstView(DataTable Dt)
        {
            List<JobInwardInvoiceGst> IList = new List<JobInwardInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new JobInwardInvoiceGst
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
        public List<JobInwardInvoiceGst> SaleInvGst(DataTable Dt)
        {
            List<JobInwardInvoiceGst> IList = new List<JobInwardInvoiceGst>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new JobInwardInvoiceGst
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

        public List<JobworkInvoiceSummary_DTO> JobworkInvoiceSummaryList(DataTable Dt)
        {
            List<JobworkInvoiceSummary_DTO> InvoiceList =
                new List<JobworkInvoiceSummary_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                InvoiceList.Add(
                    new JobworkInvoiceSummary_DTO
                    {
                        JISVIH_Number =
                            Convert.ToInt64(dr["JISVIH_Number"]),

                        JISVIH_InvoiceNo =
                            Convert.ToString(dr["JISVIH_InvoiceNo"]),

                        JISVIH_InvoiceDate =
                            Convert.ToString(dr["JISVIH_InvoiceDate"]),

                        //JIDNH_DN_No =
                        //    Convert.ToString(dr["JIDNH_DN_No"]),

                        //JIDNH_DN_Date =
                        //    Convert.ToString(dr["JIDNH_DN_Date"]),

                        CUS_Name =
                            Convert.ToString(dr["CUS_Name"]),

                        // ✅ NEW
                        CustomerGroup =
                            Convert.ToString(dr["CustomerGroup"]),

                        // ✅ NEW
                        CustomerCategory =
                            Convert.ToString(dr["CustomerCategory"]),

                        CurrencyCode =
                            Convert.ToString(dr["CurrencyCode"]),

                        TaxCluster =
                            Convert.ToString(dr["TaxCluster"]),

                        TotalQty = Convert.ToDecimal(
                            dr["TotalQty"] == DBNull.Value ? 0 : dr["TotalQty"]),

                        Amount = Convert.ToDecimal(
                            dr["Amount"] == DBNull.Value ? 0 : dr["Amount"]),

                        GST_Amount = Convert.ToDecimal(
                            dr["GST_Amount"] == DBNull.Value ? 0 : dr["GST_Amount"]),

                        // ✅ NEW
                        Segregation =
                            Convert.ToString(dr["Segregation"]),

                        // ✅ NEW
                        WarehouseCode =
                            Convert.ToString(dr["WarehouseCode"]),
                        DN_Count =
                            Convert.ToString(dr["DN_Count"]),
                        DN_List =
                            Convert.ToString(dr["DN_List"]),

                    });
            }

            return InvoiceList;
        }
        public List<JobworkInvoiceDetail_DTO> JobworkInvoiceDetailList(DataTable Dt)
        {
            List<JobworkInvoiceDetail_DTO> InvoiceList =
                new List<JobworkInvoiceDetail_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                InvoiceList.Add(
     new JobworkInvoiceDetail_DTO
     {
         JIDNH_MS_Number =
             dr["JIDNH_MS_Number"] == DBNull.Value
                 ? 0
                 : Convert.ToInt64(dr["JIDNH_MS_Number"]),

         JIDNH_WH_Number =
             dr["JIDNH_WH_Number"] == DBNull.Value
                 ? 0
                 : Convert.ToInt64(dr["JIDNH_WH_Number"]),

         JISVIH_Number =
             dr["JISVIH_Number"] == DBNull.Value
                 ? 0
                 : Convert.ToInt64(dr["JISVIH_Number"]),

         JISVIH_InvoiceNo =
             dr["JISVIH_InvoiceNo"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["JISVIH_InvoiceNo"]),

         JISVIH_InvoiceDate =
             dr["JISVIH_InvoiceDate"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["JISVIH_InvoiceDate"]),

         JIDNH_DN_No =
             dr["JIDNH_DN_No"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["JIDNH_DN_No"]),

         JIDNH_DN_Date =
             dr["JIDNH_DN_Date"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["JIDNH_DN_Date"]),

         CUS_Name =
             dr["CUS_Name"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["CUS_Name"]),

         CustomerGroup =
             dr["CustomerGroup"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["CustomerGroup"]),

         CustomerCategory =
             dr["CustomerCategory"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["CustomerCategory"]),

         CurrencyCode =
             dr["CurrencyCode"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["CurrencyCode"]),

         TaxCluster =
             dr["TaxCluster"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["TaxCluster"]),

         JISVII_Qty =
             dr["JISVII_Qty"] == DBNull.Value
                 ? 0
                 : Convert.ToDecimal(dr["JISVII_Qty"]),

         JISVII_Amount =
             dr["JISVII_Amount"] == DBNull.Value
                 ? 0
                 : Convert.ToDecimal(dr["JISVII_Amount"]),

         JISVII_GST_Amount =
             dr["JISVII_GST_Amount"] == DBNull.Value
                 ? 0
                 : Convert.ToDecimal(dr["JISVII_GST_Amount"]),

         Segregation =
             dr["Segregation"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["Segregation"]),

         WarehouseCode =
             dr["WarehouseCode"] == DBNull.Value
                 ? ""
                 : Convert.ToString(dr["WarehouseCode"]),
         PRS_ProcessName =
    dr["PRS_ProcessName"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["PRS_ProcessName"]),

         ItemGroup =
    dr["ItemGroup"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["ItemGroup"]),

         ItemCode =
    dr["ItemCode"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["ItemCode"]),

         ItemDescription =
    dr["ItemDescription"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["ItemDescription"]),

         OuterDia =
    dr["OuterDia"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["OuterDia"]),

         Thickness =
    dr["Thickness"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["Thickness"]),

         Length =
    dr["Length"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["Length"]),

         ITM_Width =
    dr["ITM_Width"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["ITM_Width"]),

         MaterialGrade =
    dr["MaterialGrade"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["MaterialGrade"]),

         UOM =
    dr["UOM"] == DBNull.Value
        ? ""
        : Convert.ToString(dr["UOM"])
     });
            }

            return InvoiceList;
        }

    }
}

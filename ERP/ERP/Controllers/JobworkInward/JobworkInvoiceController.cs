using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DAO.JobInwardTransaction;
using ERP_DL;
using ERP_DTO;
using ERP_DTO.JobInwardTransaction;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace ERP.Controllers.JobworkInward
{
    public class JobworkInvoiceController : Controller
    {
        Help Help = new Help();
        DataSet DS = new DataSet();
        JW_Invoice_DL JW_Inv_DL = new JW_Invoice_DL();
        List<JobworkInvoiceSummary_DTO> SIR_List = new List<JobworkInvoiceSummary_DTO>();
        List<JobworkInvoiceDetail_DTO> SIR_List_detail = new List<JobworkInvoiceDetail_DTO>();
        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;
        Int32? DPageNumber;
        Int32 DPageSize;

        #region JobInvoice Edit
        public IActionResult Edit(long SI_No)
        {
            GetJobworkInvoiceData();
            return View();
        }
        #endregion


        public IActionResult Create()
        {
            GetJobworkInvoiceData();
            return View();
        }



        public void GetJobworkInvoiceData()
        {
            JobworkInvoiceCreate_DTO DN_DTO = new JobworkInvoiceCreate_DTO();
            JobworkInvoice_DAO DN_DAO = new JobworkInvoice_DAO();
            DN_DTO.Header.JISVIH_InvoiceDate = DateTime.Now;
            DN_DTO.Header.JW_Inv_Id = 1;
           
            DataSet DS = new DataSet();
            DS = DN_DAO.JobworkInvoice(DN_DTO);
            ViewBag.Currency = Help.GetCat(DS.Tables[4]);         
            ViewBag.UoM = Help.GetCat(DS.Tables[5]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[7]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[11]);
            ViewBag.Process = Help.GetCat(DS.Tables[12]);
            ViewBag.SAC = Help.GetCat(DS.Tables[13]);
            ViewBag.SON = Help.GetCat(DS.Tables[14]);

        }
        #region GET DELIVERY NOTE ITEMS

        // Get delivery note items using customer number and return JSON
        [HttpGet]
        public JsonResult GetDeliveryNoteItems(long CustomerNumber)
        {
            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();

            DataTable dt = dao.GetDeliveryNoteItemsDB(CustomerNumber).Tables[0];

            var data = dt.AsEnumerable().Select(r => new
            {
                JIDNI_JIDNH_Number = r["JIDNI_JIDNH_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNI_JIDNH_Number"]),

                JIDNI_Number = r["JIDNI_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNI_Number"]),

                JIDNI_PRS_Number = r["JIDNI_PRS_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNI_PRS_Number"]),

                JIDNI_Item_Number = r["JIDNI_Item_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNI_Item_Number"]),

                JIDNI_WH_Number = r["JIDNI_WH_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNI_WH_Number"]),

                JIDNI_UoM_Number = r["JIDNI_UoM_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNI_UoM_Number"]),

                JIDNI_Qty = r["JIDNI_Qty"] == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(r["JIDNI_Qty"]),

                JIDNI_UnitPrice = r["JIDNI_UnitPrice"] == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(r["JIDNI_UnitPrice"]),

                JIDNI_Amount = r["JIDNI_Amount"] == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(r["JIDNI_Amount"]),

                JIDNI_JW_InvoiceTracking = r["JIDNI_JW_InvoiceTracking"] == DBNull.Value
                    ? ""
                    : r["JIDNI_JW_InvoiceTracking"].ToString(),

                JIDNH_Number = r["JIDNH_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNH_Number"]),

                JIDNH_DN_No = r["JIDNH_DN_No"] == DBNull.Value
                    ? ""
                    : r["JIDNH_DN_No"].ToString(),

                JIDNH_DN_Date = r["JIDNH_DN_Date"] == DBNull.Value
                    ? ""
                    : Convert.ToDateTime(r["JIDNH_DN_Date"]).ToString("dd MMM yyyy"),

                JIDNH_MS_Number = r["JIDNH_MS_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNH_MS_Number"]),

                JIDNH_JW_Customer_Number = r["JIDNH_JW_Customer_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNH_JW_Customer_Number"]),

                JIDNH_Currency_Number = r["JIDNH_Currency_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNH_Currency_Number"]),

                JIDNH_WH_Number = r["JIDNH_WH_Number"] == DBNull.Value
                    ? 0
                    : Convert.ToInt64(r["JIDNH_WH_Number"]),

                JIDNH_PaymentTerms = r["JIDNH_PaymentTerms"] == DBNull.Value
                    ? ""
                    : r["JIDNH_PaymentTerms"].ToString(),

                JIDNH_DeliveryTerms = r["JIDNH_DeliveryTerms"] == DBNull.Value
                    ? ""
                    : r["JIDNH_DeliveryTerms"].ToString(),

                JIDNH_DeliveryMode = r["JIDNH_DeliveryMode"] == DBNull.Value
                    ? ""
                    : r["JIDNH_DeliveryMode"].ToString(),

                JIDNH_DespatchDocumentNo = r["JIDNH_DespatchDocumentNo"] == DBNull.Value
                    ? ""
                    : r["JIDNH_DespatchDocumentNo"].ToString(),

                JIDNH_DespatchedThrough = r["JIDNH_DespatchedThrough"] == DBNull.Value
                    ? ""
                    : r["JIDNH_DespatchedThrough"].ToString(),

                JIDNH_Remarks = r["JIDNH_Remarks"] == DBNull.Value
                    ? ""
                    : r["JIDNH_Remarks"].ToString(),
                PRS_ProcessName = r["PRS_ProcessName"] == DBNull.Value
                    ? ""
                    : r["PRS_ProcessName"].ToString(),
                ItemDescription = r["ItemDescription"] == DBNull.Value
                    ? ""
                    : r["ItemDescription"].ToString(),
                OuterDia = r["OuterDia"] == DBNull.Value
                    ? ""
                    : r["OuterDia"].ToString(),
                Thickness = r["Thickness"] == DBNull.Value
                    ? ""
                    : r["Thickness"].ToString(),
                Length = r["Length"] == DBNull.Value
                    ? ""
                    : r["Length"].ToString(),
                ITM_Width = r["ITM_Width"] == DBNull.Value
                    ? ""
                    : r["ITM_Width"].ToString(),
                MaterialGrade = r["MaterialGrade"] == DBNull.Value
                    ? ""
                    : r["MaterialGrade"].ToString(),
                ItemGroup = r["ItemGroup"] == DBNull.Value
                    ? ""
                    : r["ItemGroup"].ToString(),
                UOM = r["UOM"] == DBNull.Value
                    ? ""
                    : r["UOM"].ToString(),
                ItemCode = r["ItemCode"] == DBNull.Value
                    ? ""
                    : r["ItemCode"].ToString(),
                SAC_Number = r["SAC_Number"] == DBNull.Value
                    ? ""
                    : r["ItemCode"].ToString(),
                SAC = r["SAC"] == DBNull.Value
                    ? ""
                    : r["SAC"].ToString()

            }).ToList();

            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }

        #endregion

        #region GET DELIVERY  NOTE GROUP ITEMS

        // Get delivery note items using customer number and return JSON
        [HttpGet]
        public JsonResult GetDeliveryNote_GroupItem(long CustomerNumber)
        {
            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();

            DataTable dt = dao.GetDeliveryNote_GroupItem(CustomerNumber).Tables[0];

            var data = dt.AsEnumerable().Select(r => new
            {
                

                TotalQty = r["RemainingQty"] == DBNull.Value
           ? 0
           : Convert.ToDecimal(r["RemainingQty"]),

                JIDNH_Number = r["JIDNH_Number"] == DBNull.Value
           ? 0
           : Convert.ToInt64(r["JIDNH_Number"]),

                JIDNH_DN_No = r["JIDNH_DN_No"] == DBNull.Value
           ? ""
           : r["JIDNH_DN_No"].ToString(),

                JIDNH_DN_Date = r["JIDNH_DN_Date"] == DBNull.Value
           ? ""
           : Convert.ToDateTime(r["JIDNH_DN_Date"]).ToString("dd MMM yyyy")

                

            }).ToList();
            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }

        #endregion

        #region get default selected address
        #region GET JWC ADDRESS

        [HttpGet]
        public JsonResult GetJWCAddress(long JWCNumber)
        {
            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();

            DataTable dt = dao.GetJWCAddressDB(JWCNumber).Tables[0];

            var data = dt.AsEnumerable().Select(r => new
            {
                JWC_ADD_Number = r["JWC_ADD_Number"] == DBNull.Value
        ? 0
        : Convert.ToInt64(r["JWC_ADD_Number"]),

                JWC_ADD_JWC_Number = r["JWC_ADD_JWC_Number"] == DBNull.Value
        ? 0
        : Convert.ToInt64(r["JWC_ADD_JWC_Number"]),

                JWC_ADD_ADTP_Number = r["JWC_ADD_ADTP_Number"] == DBNull.Value
        ? 0
        : Convert.ToInt64(r["JWC_ADD_ADTP_Number"]),

                JWC_ADD_Address_ID = r["JWC_ADD_Address_ID"] == DBNull.Value
        ? ""
        : r["JWC_ADD_Address_ID"].ToString(),

                JWC_ADD_Address = r["JWC_ADD_Address"] == DBNull.Value
        ? ""
        : r["JWC_ADD_Address"].ToString(),

                JWC_ADD_City = r["JWC_ADD_City"] == DBNull.Value
        ? ""
        : r["JWC_ADD_City"].ToString(),

                JWC_ADD_State = r["JWC_ADD_State"] == DBNull.Value
        ? ""
        : r["JWC_ADD_State"].ToString(),

                JWC_ADD_Country = r["JWC_ADD_Country"] == DBNull.Value
        ? ""
        : r["JWC_ADD_Country"].ToString(),

                JWC_ADD_PIN = r["JWC_ADD_PIN"] == DBNull.Value
        ? ""
        : r["JWC_ADD_PIN"].ToString(),

                JWC_ADD_GSTIN = r["JWC_ADD_GSTIN"] == DBNull.Value
        ? ""
        : r["JWC_ADD_GSTIN"].ToString(),

                JWC_ADD_Default = r["JWC_ADD_Default"] == DBNull.Value
        ? 0
        : Convert.ToInt32(r["JWC_ADD_Default"])
            }).ToList();
            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }

        #endregion
        #endregion

        #region Header Save
        #region save jobwork invoice

        [HttpPost]
        public IActionResult SaveJobworkInvoice(
            [FromBody] JobworkInvoiceCreate_DTO dto)
        {
            try
            {
                Console.Write(dto);

                JobworkInvoice_DAO DAO = new JobworkInvoice_DAO();

                DAO.JobworkInvoiceInsertDB(dto);

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action(
                        "JobworkInvoiceSummary",
                        "JobworkInvoice")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        #endregion
        #endregion

        #region GET DELIVERY NOTE FOR INVOICE

        // Get delivery note items using customer number and selected DN numbers
        [HttpGet]
        public JsonResult GetDeliveryNote_ForInvoice(long CustomerNumber, string DNNumbers)
        {
            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();

            DataTable dt = dao.GetDeliveryNote_ForInvoice(CustomerNumber, DNNumbers).Tables[0];

            var data = dt.AsEnumerable().Select(r => new
            {
                // ITEM

                JIDNI_JIDNH_Number = r["JIDNI_JIDNH_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNI_JIDNH_Number"]),

                JIDNI_Number = r["JIDNI_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNI_Number"]),

                JIDNI_PRS_Number = r["JIDNI_PRS_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNI_PRS_Number"]),

                PRS_ProcessName = r["PRS_ProcessName"] == DBNull.Value
               ? ""
               : r["PRS_ProcessName"].ToString(),

                JIDNI_Item_Number = r["JIDNI_Item_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNI_Item_Number"]),

                JIDNI_WH_Number = r["JIDNI_WH_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNI_WH_Number"]),

                JIDNI_UoM_Number = r["JIDNI_UoM_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNI_UoM_Number"]),

                JIDNI_Qty = r["JIDNI_Qty"] == DBNull.Value
               ? 0
               : Convert.ToDecimal(r["JIDNI_Qty"]),

                JIDNI_UnitPrice = r["JIDNI_UnitPrice"] == DBNull.Value
               ? 0
               : Convert.ToDecimal(r["JIDNI_UnitPrice"]),

                JIDNI_Amount = r["JIDNI_Amount"] == DBNull.Value
               ? 0
               : Convert.ToDecimal(r["JIDNI_Amount"]),

                JIDNI_JW_InvoiceTracking = r["JIDNI_JW_InvoiceTracking"] == DBNull.Value
               ? ""
               : r["JIDNI_JW_InvoiceTracking"].ToString(),

                // HEAD

                JIDNH_Number = r["JIDNH_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNH_Number"]),

                JIDNH_DN_No = r["JIDNH_DN_No"] == DBNull.Value
               ? ""
               : r["JIDNH_DN_No"].ToString(),

                JIDNH_DN_Date = r["JIDNH_DN_Date"] == DBNull.Value
               ? ""
               : Convert.ToDateTime(r["JIDNH_DN_Date"]).ToString("dd MMM yyyy"),

                JIDNH_MS_Number = r["JIDNH_MS_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNH_MS_Number"]),
                JISVOH_Number = r["JISVOH_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JISVOH_Number"]),
                JISVOI_UnitPrice = r["JISVOI_UnitPrice"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JISVOI_UnitPrice"]),                

                JIDNH_JW_Customer_Number = r["JIDNH_JW_Customer_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNH_JW_Customer_Number"]),

                JIDNH_Currency_Number = r["JIDNH_Currency_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNH_Currency_Number"]),

                JIDNH_WH_Number = r["JIDNH_WH_Number"] == DBNull.Value
               ? 0
               : Convert.ToInt64(r["JIDNH_WH_Number"]),

                JIDNH_PaymentTerms = r["JIDNH_PaymentTerms"] == DBNull.Value
               ? ""
               : r["JIDNH_PaymentTerms"].ToString(),

                JIDNH_DeliveryTerms = r["JIDNH_DeliveryTerms"] == DBNull.Value
               ? ""
               : r["JIDNH_DeliveryTerms"].ToString(),

                JIDNH_DeliveryMode = r["JIDNH_DeliveryMode"] == DBNull.Value
               ? ""
               : r["JIDNH_DeliveryMode"].ToString(),

                JIDNH_DespatchDocumentNo = r["JIDNH_DespatchDocumentNo"] == DBNull.Value
               ? ""
               : r["JIDNH_DespatchDocumentNo"].ToString(),

                JIDNH_DespatchedThrough = r["JIDNH_DespatchedThrough"] == DBNull.Value
               ? ""
               : r["JIDNH_DespatchedThrough"].ToString(),

                JIDNH_Remarks = r["JIDNH_Remarks"] == DBNull.Value
               ? ""
               : r["JIDNH_Remarks"].ToString(),

                // ITEM MASTER

                ItemDescription = r["ItemDescription"] == DBNull.Value
               ? ""
               : r["ItemDescription"].ToString(),

                OuterDia = r["OuterDia"] == DBNull.Value
               ? ""
               : r["OuterDia"].ToString(),

                Thickness = r["Thickness"] == DBNull.Value
               ? ""
               : r["Thickness"].ToString(),

                Length = r["Length"] == DBNull.Value
               ? ""
               : r["Length"].ToString(),

                ITM_Width = r["ITM_Width"] == DBNull.Value
               ? ""
               : r["ITM_Width"].ToString(),

                MaterialGrade = r["MaterialGrade"] == DBNull.Value
               ? ""
               : r["MaterialGrade"].ToString(),

                // GROUP / UOM

                ItemGroup = r["ItemGroup"] == DBNull.Value
               ? ""
               : r["ItemGroup"].ToString(),

                UOM = r["UOM"] == DBNull.Value
               ? ""
               : r["UOM"].ToString(),

                // ITEM CODE

                ItemCode = r["ItemCode"] == DBNull.Value
               ? ""
               : r["ItemCode"].ToString(),
                SAC_Number = r["SAC_Number"] == DBNull.Value
                    ? ""
                    : r["SAC_Number"].ToString(),
                SAC = r["SAC"] == DBNull.Value
                    ? ""
                    : r["SAC"].ToString(),
                //invoiced qty
                JISVII_Number =  0,
                InvoicedQty = r["InvoicedQty"] == DBNull.Value
      ? ""
      : r["InvoicedQty"].ToString()



            }).ToList();

            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }

        #endregion

        #region GET JWC GST ACTIVE

        // Get GST details based on customer and selected date
        [HttpGet]
        public JsonResult Get_JW_Invoice_Taxcluster(long JWC_Number, DateTime CheckDate)
        {
            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();

            DataTable dt = dao.Get_JW_Invoice_Taxcluster(JWC_Number, CheckDate).Tables[0];

            var data = dt.AsEnumerable().Select(r => new
            {
                // GST HEADER INFO

                JWC_GST_Number = r["JWC_GST_Number"] == DBNull.Value
                ? 0
                : Convert.ToInt64(r["JWC_GST_Number"]),

                JWC_GST_JWC_Number = r["JWC_GST_JWC_Number"] == DBNull.Value
                ? 0
                : Convert.ToInt64(r["JWC_GST_JWC_Number"]),

                JWC_GST_GSTC_Number = r["JWC_GST_GSTC_Number"] == DBNull.Value
                ? 0
                : Convert.ToInt64(r["JWC_GST_GSTC_Number"]),

                JWC_GST_GSTT_Number = r["JWC_GST_GSTT_Number"] == DBNull.Value
                ? 0
                : Convert.ToInt64(r["JWC_GST_GSTT_Number"]),

                JWC_GST_TCT_Number = r["JWC_GST_TCT_Number"] == DBNull.Value
                ? 0
                : Convert.ToInt64(r["JWC_GST_TCT_Number"]),

                JWC_GST_Description = r["JWC_GST_Description"] == DBNull.Value
                ? ""
                : r["JWC_GST_Description"].ToString(),

                CUS_GST_TCT_Name = r["CUS_GST_TCT_Name"] == DBNull.Value
                ? ""
                : r["CUS_GST_TCT_Name"].ToString(),

                CUS_GST_GSTC_Name = r["CUS_GST_GSTC_Name"] == DBNull.Value
                ? ""
                : r["CUS_GST_GSTC_Name"].ToString(),

                JWC_GST_FromDate = r["JWC_GST_FromDate"] == DBNull.Value
                ? ""
                : r["JWC_GST_FromDate"].ToString(),

                CUS_GST_ToDate = r["CUS_GST_ToDate"] == DBNull.Value
                ? ""
                : r["CUS_GST_ToDate"].ToString()

            }).ToList();

            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }

        #endregion


        [HttpGet]
        [Route("gst/view")]
        public JsonResult JobInvoiceInvoiceGstView(String? Cluster, String? SIHDate, String? SAC, String? BaseAmount)
        {
          Int64 nUserCode = Convert.ToInt64(UserCode);
         int nJI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            Int64 nJI_TCT_Number = Convert.ToInt64(Cluster);
            Int64 nJI_SAC_Number = Convert.ToInt64(SAC);
            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();
          
            DataTable dt = dao.GetTaxClusterCalculation(nJI_TCT_Number, nJI_SAC_Number, nJI_InvoiceDate).Tables[0];

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<JobInwardInvoiceGst> PurGST = new List<JobInwardInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = JW_Inv_DL.SaleInvGstView(dt).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
       new JobInwardInvoiceGst
       {
           TaxIndex = Group.Key,
           GSTCNumber = calculationOneItems.First().GSTCNumber,
           GSTTNumber = calculationOneItems.First().GSTTNumber,
           GSTENumber = calculationOneItems.First().GSTENumber,
           TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
           TaxType = calculationOneItems.First().TaxType.ToString(),
           TaxElement = calculationOneItems.First().TaxElementName.ToString(),
           LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
           LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
           Chargeable = calculationOneItems.First().Chargeable.ToString(),
           Calculation = 1,
           Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),

           AssessableValue = double.IsNaN(GroupAssessableValue)
                               ? 0
                               : GroupAssessableValue,

           Amount = double.IsNaN(GroupTotal)
                       ? 0
                       : GroupTotal
       });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return new JsonResult(PurGST, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
          
        }
        [HttpGet]
        [Route("income/gst")]
        public JsonResult JobInvoiceHeaderGst(string? Cluster, string? SIHDate, string? SAC, string? BaseAmount)
        {
            Int64 nJI_TCT_Number = Convert.ToInt64(Cluster);
            Int64 nJI_SAC_Number = Convert.ToInt64(SAC);
            Int32 nJI_InvoiceDate = Convert.ToInt32(
                Convert.ToDateTime(SIHDate).ToString("yyyyMMdd")
            );

            Double BaseValue = Convert.ToDouble(BaseAmount);

            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();

            DataSet DS = dao.GetTaxClusterCalculationSAC(
                nJI_TCT_Number,
                nJI_SAC_Number,
                nJI_InvoiceDate
            );

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = JW_Inv_DL.SaleInvGst(DS.Tables[0])
                                    .GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group
                    .Where(x => x.Calculation == 1)
                    .Select(x => x.TaxElement)
                    .FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 &&
                        item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            GroupTotal += BaseValue *
                                          (item.Percentage.Value / 100);
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Int64 taxElement = Convert.ToInt64(item.TaxElement);

                        if (GroupTotals.ContainsKey(taxElement))
                        {
                            Double BaseElementValue = GroupTotals[taxElement];

                            if (item.Percentage.HasValue)
                            {
                                GroupTotal += BaseElementValue *
                                              (item.Percentage.Value / 100);
                            }
                        }
                    }
                }

                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();

            return Json(OverallTotal);
        }

        #region summary
        //Sale Invoice summary
        [Route("JWInvoice/transactions/JWInvoice-summary")]
        public IActionResult JWInvoiceSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            SIR_List = SISummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View("JWInvoiceSummary", PaginatedList_DTO<JobworkInvoiceSummary_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }
        [Route("JWInvoice/transactions/JWInvoice-summary")]
        [HttpPost]
        public IActionResult JWInvoiceSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? SI_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            //ReceiptNote_DTO SH_DTO = new ReceiptNote_DTO();
            //if (Mode == "Delete")
            //{
            //    SH_DTO.JIRNH_Number = Convert.ToInt64(SI_No);
            //    SH_DTO.JIRN_Id = 104;
            //    SH_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
            //    DS = SI_DAO.JI_ReceiptNoteDB(SH_DTO);
            //    return RedirectToAction("ReciptNoteSummaryDetailed");
            //}
            //if (Mode == "View")
            //{


            //    return RedirectToAction("PreviewReceiptNote", new
            //    {
            //        SI_No = SI_No
            //    });
            //}
            if (Mode == "Edit")
            {


                return RedirectToAction("Edit", new
                {
                    SI_No = SI_No
                });
            }
            SIR_List = SISummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<JobworkInvoiceSummary_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }


        List<JobworkInvoiceSummary_DTO> SISummaryGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();

            DataSet DS = dao.GetJobworkInvoiceList();
            SIR_List = JW_Inv_DL.JobworkInvoiceSummaryList(DS.Tables[0]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title_desc";
            }
            if (Convert.ToInt32(PageNumber) == 0)
            {
                DPageNumber = 1;
            }
            if (PageFilter?.ToLower() == "PageFilter".ToLower())
            {
                DPageNumber = 1;
            }

            ViewData["CurrentSIrt"] = SortOrder;
            ViewData["KeySIrt"] = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["CurrentFilter"] = Search;

            var Key = SIR_List.OrderByDescending(Cs => Cs.JISVIH_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                //Key = Key.Where(K => K.SI_InvoiceDate.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_InvoiceNo.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_BUY_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_CUR_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_MS_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_NoOfItem.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_Qty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SI_Number);


                Key = Key.Where(K => K.JISVIH_InvoiceDate.ToString().ToLower().Contains(Search.ToLower()) ||
                K.JISVIH_InvoiceNo.ToString().ToLower().Contains(Search.ToLower()) ||
                K.CUS_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                K.CurrencyCode.ToString().ToLower().Contains(Search.ToLower()) ||          
                K.Amount.ToString().ToLower().Contains(Search.ToLower()) ||
                K.TotalQty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.JISVIH_Number);



            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.JISVIH_InvoiceDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.JISVIH_InvoiceDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.JISVIH_InvoiceDate);
                    break;
            }

            if (PSize != 0)
            {
                DPageSize = PSize;
            }
            Int32 Record = Key.ToList().Count;
            if (PageNumber > 1)
            {
                Int32 RecordPage = (Convert.ToInt32(PageNumber) - 1) * DPageSize;

                if (Record > RecordPage)
                {
                    if (Convert.ToInt32(PageNumber) == 0)
                    {
                        DPageNumber = 1;
                    }
                    else
                    {
                        DPageNumber = Convert.ToInt32(PageNumber);
                    }
                }
                else
                {
                    Double Page = Convert.ToDouble(Record) / Convert.ToDouble(DPageSize);
                    Int32 PageCount = Convert.ToInt32(Math.Ceiling(Page));
                    if (PageNumber > PageCount)
                    {
                        DPageNumber = Convert.ToInt32(PageCount);
                    }
                    else
                    {
                        DPageNumber = Convert.ToInt32(PageNumber);
                    }
                }
            }
            else
            {
                if (Convert.ToInt32(PageNumber) == 0)
                {
                    DPageNumber = 1;
                }
                else
                {
                    DPageNumber = Convert.ToInt32(PageNumber);
                }
            }

            Double Pages = Convert.ToDouble(Record) / Convert.ToDouble(DPageSize);
            Int32 PageCounts = Convert.ToInt32(Math.Ceiling(Pages));

            ViewBag.SumOfQty = Key.Sum(item => double.TryParse(item.TotalQty.ToString(), out double val) ? val : 0);

            ViewBag.SumOfAmount = Key.Sum(item => double.TryParse(item.TotalQty.ToString(), out double val) ? val : 0);

            ViewBag.SumOfHeadGst = Key.Sum(item => double.TryParse(item.GST_Amount.ToString(), out double val) ? val : 0);        //   ViewBag.SumOfReceivable = Key.Sum(item => Convert.ToDouble(item.RN_BuyerReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;
            if (DS.Tables.Count > 1 && DS.Tables[1].Rows.Count > 0)
            {
                ViewBag.SumOfQty = DS.Tables[1].Rows[0]["GrandTotalQty"] == DBNull.Value ? 0 : Convert.ToDouble(DS.Tables[1].Rows[0]["GrandTotalQty"]);

                ViewBag.SumOfAmount = DS.Tables[1].Rows[0]["GrandTotalAmount"] == DBNull.Value ? 0 : Convert.ToDouble(DS.Tables[1].Rows[0]["GrandTotalAmount"]);

                ViewBag.SumOfGST = DS.Tables[1].Rows[0]["GrandTotalGST"] == DBNull.Value ? 0 : Convert.ToDouble(DS.Tables[1].Rows[0]["GrandTotalGST"]);
            }
            return Key.ToList();
        }


        #endregion

        #region detailed
        //Sale Invoice summary
        [Route("JWInvoice/transactions/JWInvoice-Detailed")]
        public IActionResult JWInvoiceDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            SIR_List_detail = DetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View("JWInvoiceDetailed", PaginatedList_DTO<JobworkInvoiceDetail_DTO>.CreateAsync(SIR_List_detail, DPageNumber ?? 1, DPageSize));
        }
        [Route("JWInvoice/transactions/JWInvoice-JWInvoiceDetailed")]
        [HttpPost]
        public IActionResult JWInvoiceDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? SI_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            //ReceiptNote_DTO SH_DTO = new ReceiptNote_DTO();
            //if (Mode == "Delete")
            //{
            //    SH_DTO.JIRNH_Number = Convert.ToInt64(SI_No);
            //    SH_DTO.JIRN_Id = 104;
            //    SH_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
            //    DS = SI_DAO.JI_ReceiptNoteDB(SH_DTO);
            //    return RedirectToAction("ReciptNoteSummaryDetailed");
            //}
            //if (Mode == "View")
            //{


            //    return RedirectToAction("PreviewReceiptNote", new
            //    {
            //        SI_No = SI_No
            //    });
            //}
            if (Mode == "Edit")
            {


                return RedirectToAction("Edit", new
                {
                    SI_No = SI_No
                });
            }
            SIR_List_detail = DetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<JobworkInvoiceDetail_DTO>.CreateAsync(SIR_List_detail, DPageNumber ?? 1, DPageSize));
        }

        List<JobworkInvoiceDetail_DTO> DetailedGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();

            DataSet DS = dao.GetJobworkInvoiceListDetailed();
            SIR_List_detail = JW_Inv_DL.JobworkInvoiceDetailList(DS.Tables[0]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title_desc";
            }
            if (Convert.ToInt32(PageNumber) == 0)
            {
                DPageNumber = 1;
            }
            if (PageFilter?.ToLower() == "PageFilter".ToLower())
            {
                DPageNumber = 1;
            }

            ViewData["CurrentSIrt"] = SortOrder;
            ViewData["KeySIrt"] = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["CurrentFilter"] = Search;

            var Key = SIR_List_detail.OrderByDescending(Cs => Cs.JISVIH_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                //Key = Key.Where(K => K.SI_InvoiceDate.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_InvoiceNo.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_BUY_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_CUR_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_MS_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_NoOfItem.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_Qty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SI_Number);


                Key = Key.Where(K => K.JISVIH_InvoiceDate.ToString().ToLower().Contains(Search.ToLower()) ||
                K.JISVIH_InvoiceNo.ToString().ToLower().Contains(Search.ToLower()) ||
                K.CUS_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                K.CurrencyCode.ToString().ToLower().Contains(Search.ToLower()) ||
                K.JISVII_Amount.ToString().ToLower().Contains(Search.ToLower()) ||
                K.JISVII_Qty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.JISVIH_Number);



            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.JISVIH_InvoiceDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.JISVIH_InvoiceDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.JISVIH_InvoiceDate);
                    break;
            }

            if (PSize != 0)
            {
                DPageSize = PSize;
            }
            Int32 Record = Key.ToList().Count;
            if (PageNumber > 1)
            {
                Int32 RecordPage = (Convert.ToInt32(PageNumber) - 1) * DPageSize;

                if (Record > RecordPage)
                {
                    if (Convert.ToInt32(PageNumber) == 0)
                    {
                        DPageNumber = 1;
                    }
                    else
                    {
                        DPageNumber = Convert.ToInt32(PageNumber);
                    }
                }
                else
                {
                    Double Page = Convert.ToDouble(Record) / Convert.ToDouble(DPageSize);
                    Int32 PageCount = Convert.ToInt32(Math.Ceiling(Page));
                    if (PageNumber > PageCount)
                    {
                        DPageNumber = Convert.ToInt32(PageCount);
                    }
                    else
                    {
                        DPageNumber = Convert.ToInt32(PageNumber);
                    }
                }
            }
            else
            {
                if (Convert.ToInt32(PageNumber) == 0)
                {
                    DPageNumber = 1;
                }
                else
                {
                    DPageNumber = Convert.ToInt32(PageNumber);
                }
            }

            Double Pages = Convert.ToDouble(Record) / Convert.ToDouble(DPageSize);
            Int32 PageCounts = Convert.ToInt32(Math.Ceiling(Pages));

            //  ViewBag.SumOfItem = Key.Sum(item => Convert.ToDouble(item.RN_NoOfItem));
            ViewBag.SumOfQty = Key.Sum(item => Convert.ToDouble(item.JISVII_Qty));
            //  ViewBag.SumOfItemIncome = Key.Sum(item => Convert.ToDouble(item.RN_TotalItemIncome));
            //  ViewBag.SumOfHeadIncome = Key.Sum(item => Convert.ToDouble(item.RN_TotalHeadIncome));
            ViewBag.SumOfAmount = Key.Sum(item => Convert.ToDouble(item.JISVII_Amount));
            ViewBag.SumOfHeadGst = Key.Sum(item => Convert.ToDouble(item.JISVII_GST_Amount));
            //   ViewBag.SumOfReceivable = Key.Sum(item => Convert.ToDouble(item.RN_BuyerReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;
            if (DS.Tables.Count > 1 && DS.Tables[1].Rows.Count > 0)
            {
                ViewBag.SumOfQty =
                    Convert.ToDouble(DS.Tables[1].Rows[0]["TotalQty"]);

                ViewBag.SumOfAmount =
                    Convert.ToDouble(DS.Tables[1].Rows[0]["TotalAmount"]);

                ViewBag.SumOfGST =
                    Convert.ToDouble(DS.Tables[1].Rows[0]["TotalGST"]);
            }
            return Key.ToList();
        }

        #endregion

        #region View
        public ActionResult JWInvoiceView()
        {

            JobworkInvoiceCreate_DTO obj = new JobworkInvoiceCreate_DTO();

            return View(obj);
        }
        #endregion
        #region EDIT GET JOBWORK INVOICE JSON

        [HttpGet]
        public JsonResult GetJobworkInvoice(long JISVIH_Number)
        {
            JobworkInvoice_DAO dao = new JobworkInvoice_DAO();

            string json = dao.GetJobworkInvoiceJSON(JISVIH_Number);

            if (string.IsNullOrEmpty(json))
            {
                return new JsonResult(new
                {
                    Header = new object(),
                    Items = new object[] { },
                    Addresses = new object[] { },
                    GST = new object[] { }
                });
            }

            var obj = JsonSerializer.Deserialize<object>(json);

            return new JsonResult(obj, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }

        #endregion

        #region UPDATE JOBWORK INVOICE
        [HttpPost]
        public IActionResult UpdateJobworkInvoice(
            [FromBody] JobworkInvoiceCreate_DTO dto)
        {
            try
            {
                Console.Write(dto);

                JobworkInvoice_DAO JI_DAO =
                    new JobworkInvoice_DAO();

                JI_DAO.JobworkInvoiceUpdateDB(dto);

                // JI_DAO.JobworkInvoiceItemBulkUpdate(dto);
                //// JI_DAO.JobworkInvoiceAddressBulkUpdate(dto);

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action(
                        "JobworkInvoiceSummary",
                        "JobworkInvoice")
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        #endregion

        #region
        [HttpGet]
        public JsonResult GetServiceOrderItemInfo(
      long JISVOH_Number,
      long PRS_Number,
      long Item_Number,
      long UoM_Number)
        {
            DataTable dt = new JobworkInvoice_DAO()
                .GetServiceOrderItemInfo(
                    JISVOH_Number,
                    PRS_Number,
                    Item_Number,
                    UoM_Number).Tables[0];

            if (dt.Rows.Count == 0)
                return Json(null);

            var row = dt.Rows[0];

            return Json(new
            {
                UnitPrice = row["JISVOI_UnitPrice"],
                Amount = row["JISVOI_Amount"],
                JISVOI_Number = row["JISVOI_Number"].ToString(),
                
            });
        }

        #endregion
    }
}

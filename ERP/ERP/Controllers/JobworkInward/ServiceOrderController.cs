using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using ERP.Models;
using ERP_DAO.JobInwardTransaction;
using ERP_DL;
using ERP_DTO;
using ERP_DTO.JobInwardTransaction;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;

namespace ERP.Controllers.JobworkInward
{
    public class ServiceOrderController : Controller
    {
        Help Help = new Help();
        DataSet DS = new DataSet();
        // Service Order
        Int32? SOPageNumber;
        Int32 SOPageSize;
        List<ServiceOrderDetailed_DTO> SOD_List = new List<ServiceOrderDetailed_DTO>();
        List<ServiceOrderSummary_DTO> SOS_List =    new List<ServiceOrderSummary_DTO>();
        ServiceOrderSummary_DTO SOS_DTO =    new ServiceOrderSummary_DTO();
        ServiceOrder_DAO SO_DAO =    new ServiceOrder_DAO();
        ServiceOrder_DL SO_DL = new ServiceOrder_DL();
        [HttpPost]
        public IActionResult UpdateServiceOrder([FromBody] JI_ServiceOrder_DTO dto)
        {
            try
            {
                if (dto == null)
                    return Json(new { success = false, message = "DTO is null" });

                if (dto.Header == null)
                    return Json(new { success = false, message = "Header is null" });

                if (dto.Header.JISVOH_Number <= 0)
                    return Json(new { success = false, message = "Invalid Service Order Number" });

                ServiceOrder_DAO dao = new ServiceOrder_DAO();
                dao.ServiceOrderUpdateDB(dto);

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Index", "ServiceOrder")
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
        [HttpPost]
        public IActionResult SaveServiceOrder(
    [FromBody] JI_ServiceOrder_DTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "DTO is null"
                    });
                }

                if (dto.Header == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Header is null"
                    });
                }

                ServiceOrder_DAO serviceOrderDAO =
                    new ServiceOrder_DAO();

                //// Temporary values if needed
                //dto.Header.SVO_Id = 10;
                //dto.Header.JISVOH_RegDate = DateTime.Now;

                serviceOrderDAO.ServiceOrderInsertDB(dto);

                return Json(new
                {
                    success = true,
                    redirectUrl =
                        Url.Action("Index", "ServiceOrder")
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

        public void GetServiceOrderData()
        {
            JI_ServiceOrder_DTO SVO_DTO = new JI_ServiceOrder_DTO();
            ServiceOrder_DAO SVO_DAO = new ServiceOrder_DAO();

            SVO_DTO.Header.JISVOH_RegDate = DateTime.Now;
            SVO_DTO.Header.SVO_Id = 1;

            DataSet DS = new DataSet();
            DS = SVO_DAO.ServiceOrderDB(SVO_DTO);

            ViewBag.Currency = Help.GetCat(DS.Tables[0]);
            ViewBag.UoM = Help.GetCat(DS.Tables[1]);
        
            ViewBag.Process = Help.GetCat(DS.Tables[2]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[3]);
        }
        [Route("jobinward/transactions/service-order/item")]
        public IActionResult SaleItem(String? ItemCode, long? MS)
        {
            JI_ServiceOrder_DTO SI_DTO = new JI_ServiceOrder_DTO();
            ServiceOrder_DAO SVO_DAO = new ServiceOrder_DAO();
            ReceiptNote_DL S_DL = new ReceiptNote_DL();
            DataSet DS = new DataSet();
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            SI_DTO.Header.JISVOH_RegDate = DateTime.Now;
            SI_DTO.Header.JISVOH_MS_Number = MS;
            SI_DTO.Header.JISVOI_Item_Code = Convert.ToString(ItemCode);          
            SI_DTO.Header.SVO_Id = 6;
            DS = SVO_DAO.ServiceOrderDB(SI_DTO);
            var Item = S_DL.ItemList(DS.Tables[0]);
            return Json(Item);
        }


        public IActionResult Create()
        {

            GetServiceOrderData();

            ViewBag.Collapse = true;
            return View();
          
        }

        public IActionResult Edit()
        {

            GetServiceOrderData();
            ViewBag.Collapse = true;

            return View();

        }
        #region Service Order Summary

        [Route("service-order/transactions/service-order-summary")]
        public IActionResult ServiceOrderSummary(
            string? SortOrder,
            string? Search,
            int? PageNumber,
            int PSize,
            string? PageFilter)
        {
            SOS_List = SOSummaryGetData(
                SortOrder,
                Search,
                PageNumber,
                PSize,
                PageFilter);

            return View(
                PaginatedList_DTO<ServiceOrderSummary_DTO>
                .CreateAsync(SOS_List, SOPageNumber ?? 1, SOPageSize));
        }
        [Route("service-order/transactions/service-order-summary")]
        [HttpPost]
        public IActionResult ServiceOrderSummary(
    string? SortOrder,
    string? Search,
    int? PageNumber,
    int PSize,
    string? PageFilter,
    string? Mode,
    string? SI_No,
    string[] DeleteNumber,
    string selectAllCheckbox)
        {
            ServiceOrderSummary_DTO SO_DTO =
                new ServiceOrderSummary_DTO();

            if (Mode == "Delete")
            {
                SO_DTO.JISVOH_Number =
                    Convert.ToInt64(SI_No);

                SO_DTO.SO_Id = 104;
                SO_DTO.SO_CreatorCode =
                    Convert.ToInt32(1);

                DS = SO_DAO.ServiceOrderSummaryDB(SO_DTO);

                return RedirectToAction("ServiceOrderSummary");
            }

            if (Mode == "View")
            {
                return RedirectToAction("ServiceOrderView", new
                {
                    JISOH_Number = SI_No
                });
            }

            if (Mode == "Edit")
            {
                return RedirectToAction("Edit", new
                {
                    SI_No = SI_No
                });
            }

            SOS_List = SOSummaryGetData(
                SortOrder,
                Search,
                PageNumber,
                PSize,
                PageFilter);

            return View(
                PaginatedList_DTO<ServiceOrderSummary_DTO>
                .CreateAsync(SOS_List, SOPageNumber ?? 1, SOPageSize));
        }
        List<ServiceOrderSummary_DTO> SOSummaryGetData(
    string? SortOrder,
    string? Search,
    int? PageNumber,
    int PSize,
    string? PageFilter)
        {
            SOPageSize = 10;

            SOS_DTO.SO_Id = 1;
            SOS_DTO.SO_CreatorCode =
                Convert.ToInt32(1);

            DS = SO_DAO.ServiceOrderSummaryDB(SOS_DTO);

            SOS_List =
                SO_DL.SOSummaryList(DS.Tables[0]);

            if (string.IsNullOrEmpty(SortOrder))
                SortOrder = "Title_desc";

            if (Convert.ToInt32(PageNumber) == 0)
                SOPageNumber = 1;

            if (PageFilter?.ToLower() == "pagefilter")
                SOPageNumber = 1;

            ViewData["CurrentSort"] = SortOrder;

            ViewData["KeySort"] =
                SortOrder == "Title"
                ? "Title_desc"
                : "Title";

            ViewData["CurrentFilter"] = Search;

            var Key = SOS_List
                .OrderByDescending(x => x.JISVOH_Number);

            if (!string.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K =>

                    K.JISVOH_ServiceOrderDate.ToString()
                    .ToLower()
                    .Contains(Search.ToLower()) ||

                    K.JISVOH_ServiceOrderNo.ToLower()
                    .Contains(Search.ToLower()) ||

                    K.CUS_Name.ToLower()
                    .Contains(Search.ToLower())

                ).OrderByDescending(x => x.JISVOH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(
                        x => x.JISVOH_ServiceOrderDate);
                    break;

                case "Title":
                    Key = Key.OrderBy(
                        x => x.JISVOH_ServiceOrderDate);
                    break;

                default:
                    Key = Key.OrderByDescending(
                        x => x.JISVOH_Number);
                    break;
            }

            if (PSize != 0)
                SOPageSize = PSize;

            int Record = Key.Count();

            if (PageNumber > 1)
            {
                int RecordPage =
                    (Convert.ToInt32(PageNumber) - 1)
                    * SOPageSize;

                if (Record > RecordPage)
                {
                    SOPageNumber =
                        Convert.ToInt32(PageNumber);
                }
                else
                {
                    double Page =
                        Convert.ToDouble(Record) /
                        Convert.ToDouble(SOPageSize);

                    int PageCount =
                        Convert.ToInt32(
                            Math.Ceiling(Page));

                    SOPageNumber =
                        Convert.ToInt32(PageCount);
                }
            }
            else
            {
                SOPageNumber =
                    Convert.ToInt32(PageNumber) == 0
                    ? 1
                    : Convert.ToInt32(PageNumber);
            }

            double Pages =
                Convert.ToDouble(Record) /
                Convert.ToDouble(SOPageSize);

            int PageCounts =
                Convert.ToInt32(Math.Ceiling(Pages));

            ViewBag.SumOfQty =
        DS.Tables[1].Rows.Count > 0
            ? Convert.ToDouble(DS.Tables[1].Rows[0]["TotalQty"])
            : 0;

            ViewBag.SumOfAmount =
                DS.Tables[1].Rows.Count > 0
                    ? Convert.ToDouble(DS.Tables[1].Rows[0]["TotalAmount"])
                    : 0;
            ViewBag.SumOfItem =
    DS.Tables[1].Rows.Count > 0
        ? Convert.ToInt32(DS.Tables[1].Rows[0]["TotalItems"])
        : 0;

            ViewBag.Page =
                Help.PageSize(PSize.ToString());

            ViewData["PageNumber"] =
                SOPageNumber;

            ViewData["PageSize"] =
                SOPageSize;

            ViewData["PageCount"] =
                PageCounts;

            ViewData["TotalSize"] =
                Key.Count();

            return Key.ToList();
        }
        #endregion
        #region Service Order Detailed

        [Route("service-order/transactions/service-order-detailed")]
        public IActionResult ServiceOrderDetailed(
            String? SortOrder,
            String? Search,
            Int32? PageNumber,
            Int32 PSize,
            String? PageFilter)
        {
            SOD_List = SODetailedGetData(
                SortOrder,
                Search,
                PageNumber,
                PSize,
                PageFilter);

            return View(
                PaginatedList_DTO<ServiceOrderDetailed_DTO>
                .CreateAsync(SOD_List, SOPageNumber ?? 1, SOPageSize));
        }

        [Route("service-order/transactions/service-order-detailed")]
        [HttpPost]
        public IActionResult ServiceOrderDetailed(
            String? SortOrder,
            String? Search,
            Int32? PageNumber,
            Int32 PSize,
            String? PageFilter,
            String? Mode,
            String? DeleteNumbers,
            String? SO_No,
            String[] DeleteNumber,
            String selectAllCheckbox)
        {
            ServiceOrderSummary_DTO SO_DTO =
                new ServiceOrderSummary_DTO();

            if (Mode == "Delete")
            {
                SO_DTO.JISVOH_Number =
                    Convert.ToInt64(SO_No);

                SO_DTO.SO_Id = 104;

                SO_DTO.SO_CreatorCode =
                    Convert.ToInt32(1);

                DS = SO_DAO.ServiceOrderSummaryDB(SO_DTO);

                return RedirectToAction("ServiceOrderDetailed");
            }

            SOD_List = SODetailedGetData(
                SortOrder,
                Search,
                PageNumber,
                PSize,
                PageFilter);

            return View(
                PaginatedList_DTO<ServiceOrderDetailed_DTO>
                .CreateAsync(SOD_List, SOPageNumber ?? 1, SOPageSize));
        }

        List<ServiceOrderDetailed_DTO> SODetailedGetData(
            String? SortOrder,
            String? Search,
            Int32? PageNumber,
            Int32 PSize,
            String? PageFilter)
        {
            SOPageSize = 10;

            SOS_DTO.SO_Id = 2;

            SOS_DTO.SO_CreatorCode =
                Convert.ToInt32(1);

            DS = SO_DAO.ServiceOrderSummaryDB(SOS_DTO);

            SOD_List =
                SO_DL.SODetailedList(DS.Tables[0]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title_desc";
            }

            if (Convert.ToInt32(PageNumber) == 0)
            {
                SOPageNumber = 1;
            }

            if (PageFilter?.ToLower() == "pagefilter")
            {
                SOPageNumber = 1;
            }

            ViewData["CurrentSort"] = SortOrder;

            ViewData["KeySort"] =
                SortOrder == "Title"
                ? "Title_desc"
                : "Title";

            ViewData["CurrentFilter"] = Search;

            var Key = SOD_List
                .OrderByDescending(Cs => Cs.JISVOH_Number);

            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K =>

                    K.JISVOH_ServiceOrderDate
                        .ToString()
                        .ToLower()
                        .Contains(Search.ToLower()) ||

                    K.JISVOH_ServiceOrderNo
                        .ToLower()
                        .Contains(Search.ToLower()) ||

                    K.CUS_Name
                        .ToLower()
                        .Contains(Search.ToLower()) ||

                    K.CurrencyCode
                        .ToLower()
                        .Contains(Search.ToLower())

                ).OrderByDescending(Cs => Cs.JISVOH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(
                        K => K.JISVOH_ServiceOrderDate);
                    break;

                case "Title":
                    Key = Key.OrderBy(
                        K => K.JISVOH_ServiceOrderDate);
                    break;

                default:
                    Key = Key.OrderByDescending(
                        K => K.JISVOH_Number);
                    break;
            }

            if (PSize != 0)
            {
                SOPageSize = PSize;
            }

            Int32 Record = Key.ToList().Count;

            if (PageNumber > 1)
            {
                Int32 RecordPage =
                    (Convert.ToInt32(PageNumber) - 1)
                    * SOPageSize;

                if (Record > RecordPage)
                {
                    SOPageNumber =
                        Convert.ToInt32(PageNumber);
                }
                else
                {
                    Double Page =
                        Convert.ToDouble(Record) /
                        Convert.ToDouble(SOPageSize);

                    Int32 PageCount =
                        Convert.ToInt32(
                            Math.Ceiling(Page));

                    if (PageNumber > PageCount)
                    {
                        SOPageNumber = PageCount;
                    }
                    else
                    {
                        SOPageNumber =
                            Convert.ToInt32(PageNumber);
                    }
                }
            }
            else
            {
                if (Convert.ToInt32(PageNumber) == 0)
                {
                    SOPageNumber = 1;
                }
                else
                {
                    SOPageNumber =
                        Convert.ToInt32(PageNumber);
                }
            }

            Double Pages =
                Convert.ToDouble(Record) /
                Convert.ToDouble(SOPageSize);

            Int32 PageCounts =
                Convert.ToInt32(Math.Ceiling(Pages));

            ViewBag.Page =
                Help.PageSize(PSize.ToString());

            ViewData["PageNumber"] = SOPageNumber;
            ViewData["PageSize"] = SOPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;
            if (DS.Tables.Count > 1 && DS.Tables[1].Rows.Count > 0)
            {
                ViewBag.SumOfQty =
                    Convert.ToDouble(
                        DS.Tables[1].Rows[0]["TotalQty"]);

                ViewBag.SumOfUnitPrice =
                    Convert.ToDouble(
                        DS.Tables[1].Rows[0]["TotalUnitPrice"]);

                ViewBag.SumOfAmount =
                    Convert.ToDouble(
                        DS.Tables[1].Rows[0]["TotalAmount"]);
            }
            else
            {
                ViewBag.SumOfQty = 0;
                ViewBag.SumOfUnitPrice = 0;
                ViewBag.SumOfAmount = 0;
            }
            return Key.ToList();
        }

        #endregion

        [HttpGet]
        public JsonResult GetServiceOrderHead(long customerNumber)
        {
         
            DataTable dt =
                new ServiceOrder_DAO()
                .GetServiceOrderHead(customerNumber);

            var data = dt.AsEnumerable()
                .Select(r => new
                {
                    Value = r["JISVOH_Number"].ToString(),
                    Text = r["JISVOH_ServiceOrderNo"].ToString()
                });

            return Json(data);
        }

        #region edit
        #region EDIT GET SERVICE ORDER JSON

        [HttpGet]
        public JsonResult GetServiceOrder(long JISVOH_Number)
        {
            ServiceOrder_DAO dao =
                new ServiceOrder_DAO();

            string json =
                dao.GetServiceOrderJSON(JISVOH_Number);

            if (string.IsNullOrEmpty(json))
            {
                return new JsonResult(new
                {
                    Header = new object(),
                    Items = new object[] { }
                });
            }

            var obj =
                JsonSerializer.Deserialize<object>(json);

            return new JsonResult(
                obj,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy =
                        JsonNamingPolicy.CamelCase,

                    WriteIndented = true
                });
        }

        #endregion
        #endregion
    }
}

using DocumentFormat.OpenXml.Wordprocessing;
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DAO.JobInwardTransaction;
using ERP_DL;
using ERP_DTO;
using ERP_DTO.JobInwardTransaction;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SelectPdf;
using System.Data;
 


namespace ERP.Controllers
{
    public class DeliveryNoteController : Controller
    {
        Help Help = new Help();
        DataSet DS = new DataSet();


        //summary test
        Int32? DPageNumber;
        Int32 DPageSize;
        List<DeliveryNoteSummary_DTO> DNS_List = new List<DeliveryNoteSummary_DTO>();
        List<DeliveryNoteDetailed_DTO> DND_List = new List<DeliveryNoteDetailed_DTO>();
        
        DeliveryNoteSummary_DTO DNS_DTO = new DeliveryNoteSummary_DTO();
        DeliveryNote_DAO DN_DAO = new DeliveryNote_DAO();
       DeliveryNote_DL DN_DL = new DeliveryNote_DL();
        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;
    
        [Route("jobinward/transactions/delivery-note/buyer/address")]
        public IActionResult SaleBuyerAddressID(String? Buyer, String ADTPNumber)
        {
            SaleInvoice_DL S_DL = new SaleInvoice_DL();
            DataSet DS = new DataSet();
            DeliveryNoteCreate_DTO DN_DTO = new DeliveryNoteCreate_DTO();
            DeliveryNote_DAO DN_DAO = new DeliveryNote_DAO();
            if (Buyer == null)
            {
                Buyer = "";
            }
           
            DN_DTO.Header.DN_CUS_Number = Convert.ToInt32(Buyer);
            DN_DTO.Header.DN_ADD_ADTP_Number = Convert.ToInt32(ADTPNumber);
            DN_DTO.Header.DN_Id = 13;
            DS = DN_DAO.DeliveryNoteDB(DN_DTO);
             
            SaleInvoiceAddress SIA = new SaleInvoiceAddress();
            SIA.BuyerAddressId = S_DL.JWCCustAddressID(DS.Tables[0]);
            SIA.BuyerAddress = S_DL.JWCCustAddress(DS.Tables[1]).FirstOrDefault();
            return Json(SIA);
        }
        [Route("jobinward/transactions/delivery-note/item")]
        public IActionResult SaleItem(String? ItemCode, String MS)
        {
            ReceiptNote_DTO SI_DTO = new ReceiptNote_DTO();
            ReceiptNote_DAO SI_DAO = new ReceiptNote_DAO();
            ReceiptNote_DL S_DL = new ReceiptNote_DL();
            DataSet DS = new DataSet();
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            if (MS == null)
            {
                MS = "";
            }
            SI_DTO.JIRN_CreatorCode = Convert.ToInt64(1);
            SI_DTO.JIRNI_ITM_Code = Convert.ToString(ItemCode);
            SI_DTO.JIRNH_MS_Number = Convert.ToInt64(MS);
            SI_DTO.JIRN_Id = 2;
            DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);
            var Item = S_DL.ItemList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("jobinward/transactions/delivery-note/cutomer")]
        public IActionResult SaleBuyer(String? Buyer,   String? SIHDate)
        {
            ReceiptNote_DL S_DL = new ReceiptNote_DL();
            DeliveryNoteCreate_DTO DN_DTO = new DeliveryNoteCreate_DTO();
            DeliveryNote_DAO DN_DAO = new DeliveryNote_DAO();
            DataSet DS = new DataSet();

            if (Buyer == null)
            {
                Buyer = "";
            }
           
         
            DN_DTO.Header.JIDNI_Item_Code = Convert.ToString(Buyer);
            DN_DTO.Header.JIDNH_DN_Date = Convert.ToDateTime(SIHDate);
            DN_DTO.Header.DN_Id = 5;
            DS = DN_DAO.DeliveryNoteDB(DN_DTO);
            var Ven = S_DL.CustomerListData(DS.Tables[0]);
            return Json(Ven);
        }

        public DeliveryNoteCreate_DTO InsertDummyDeliveryNote()
        {

            DeliveryNoteCreate_DTO DN_   =new DeliveryNoteCreate_DTO();
            // ================================
            // 1. HEADER DUMMY DATA
            // ================================
            var header = new DeliveryNoteHeader_DTO
            {
                JIDNH_Number = 1,
                JIDNH_DN_No = "DN-0001",
                JIDNH_DN_Date = DateTime.Now,
                JIDNH_MS_Number = 100,
                JIDNH_JW_Customer_Number = 2001,
                JIDNH_Currency_Number = 1,
                JIDNH_WH_Number = 10,
                JIDNH_PaymentTerms = "30 Days",
                JIDNH_DeliveryTerms = "FOB",
                JIDNH_DeliveryMode = "Road",
                JIDNH_DespatchDocumentNo = "DOC-7788",
                JIDNH_DespatchedThrough = "Transport Pvt Ltd",
                JIDNH_Remarks = "Dummy insert for testing"
            };


            // ================================
            // 2. ITEM DUMMY DATA (MULTIPLE ROWS)
            // ================================
            var items = new List<DeliveryNoteItem_DTO>
    {
        new DeliveryNoteItem_DTO
        {
            JIDNI_JIDNH_Number = 1,
            JIDNI_Number = 1,
            JIDNI_PRS_Number = 501,
            JIDNI_Item_Number = 1001,
            JIDNI_WH_Number = 10,
            JIDNI_UoM_Number = 1,
            JIDNI_Qty = 5,
            JIDNI_UnitPrice = 100,
            JIDNI_Amount = 500,
            JIDNI_JW_InvoiceTracking = "Y"
        },

        new DeliveryNoteItem_DTO
        {
            JIDNI_JIDNH_Number = 1,
            JIDNI_Number = 2,
            JIDNI_PRS_Number = 502,
            JIDNI_Item_Number = 1002,
            JIDNI_WH_Number = 10,
            JIDNI_UoM_Number = 1,
            JIDNI_Qty = 2,
            JIDNI_UnitPrice = 250,
            JIDNI_Amount = 500,
            JIDNI_JW_InvoiceTracking = "N"
        }
    };


            // ================================
            // 3. ADDRESS DUMMY DATA
            // ================================
            var addresses = new List<DeliveryNoteAddress_DTO>
    {
        new DeliveryNoteAddress_DTO
        {
            JIDNA_JIDNH_Number = 1,
            JIDNA_Number = 1,
            JIDNA_ADTP_Number = 1,
            JIDNA_Address_ID = "ADDR-001",
            JIDNA_Address = "No 12, Main Street, Kattur",
            JIDNA_City = "Trichy",
            JIDNA_State = "Tamil Nadu",
            JIDNA_Country = "India",
            JIDNA_PIN = "620019",
            JIDNA_GSTIN = "33ABCDE1234F1Z5"
        }
    };
            DN_.Header= header;
            DN_.Items= items;
            DN_.Addresses= addresses;
            return DN_;



        }


        #region Delivery Note Create
        public IActionResult Index()
        {
            GetDevliverNoteData();
            return View();
        }

        public void GetDevliverNoteData()
        {
            DeliveryNoteCreate_DTO DN_DTO = new DeliveryNoteCreate_DTO();
            DeliveryNote_DAO DN_DAO = new DeliveryNote_DAO();
            DN_DTO.Header.JIDNH_DN_Date = DateTime.Now;
            DN_DTO.Header.DN_Id = 1;
            DataSet DS = new DataSet();
            DS = DN_DAO.DeliveryNoteDB(DN_DTO);
            ViewBag.Currency = Help.GetCat(DS.Tables[4]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[5]);
            ViewBag.UoM = Help.GetCat(DS.Tables[6]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[8]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[12]);
            ViewBag.Process = Help.GetCat(DS.Tables[13]);

        }
        [HttpPost]
        public IActionResult SaveDeliveryNote([FromBody] DeliveryNoteCreate_DTO dto)
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

                DeliveryNote_DAO DN_DAO = new DeliveryNote_DAO();

                dto.Header.DN_Id = 10;
                dto.Header.JIDNH_DN_Date = DateTime.Now;
                dto.Header.JIDNI_Item_Code = "1";

                DataSet ds = DN_DAO.DeliveryNoteCreateDB(dto);
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Index", "DeliveryNote")
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


        #region Delivery Note Summary
        // Delivery Note Summary
        [Route("delivery-note/transactions/delivery-note-summary")]
        public IActionResult DeliveryNoteSummary(
            String? SortOrder,
            String? Search,
            Int32? PageNumber,
            Int32 PSize,
            String? PageFilter)
        {
            DNS_List = DNSummaryGetData(
                SortOrder,
                Search,
                PageNumber,
                PSize,
                PageFilter);

            return View(
                PaginatedList_DTO<DeliveryNoteSummary_DTO>
                .CreateAsync(DNS_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("delivery-note/transactions/delivery-note-summary")]
        [HttpPost]
        public IActionResult DeliveryNoteSummary(
            String? SortOrder,
            String? Search,
            Int32? PageNumber,
            Int32 PSize,
            String? PageFilter,
            String? Mode,
            String? DeleteNumbers,
            String? DN_No,
            String[] DeleteNumber,
            String selectAllCheckbox)
        {
            DeliveryNoteSummary_DTO DN_DTO = new DeliveryNoteSummary_DTO();

            if (Mode == "Delete")
            {
                DN_DTO.JIDNH_Number = Convert.ToInt64(DN_No);

                DN_DTO.DN_Id = 104;

                DN_DTO.DN_CreatorCode = Convert.ToInt32(UserCode);

                DS = DN_DAO.DeliveryNoteSummaryDB(DN_DTO);

                return RedirectToAction("DeliveryNoteSummary");
            }

            if (Mode == "View")
            {
                return RedirectToAction("DeliveryNoteView", new
                {
                    JIDNH_Number = DN_No
                });
            }

            if (Mode == "Edit")
            {
                return RedirectToAction("Edit", new
                {
                    JIDNH_Number = DN_No
                });
            }

            DNS_List = DNSummaryGetData(
                SortOrder,
                Search,
                PageNumber,
                PSize,
                PageFilter);

            return View(
                PaginatedList_DTO<DeliveryNoteSummary_DTO>
                .CreateAsync(DNS_List, DPageNumber ?? 1, DPageSize));
        }

        List<DeliveryNoteSummary_DTO> DNSummaryGetData(
    String? SortOrder,
    String? Search,
    Int32? PageNumber,
    Int32 PSize,
    String? PageFilter)
        {
            DPageSize = 10;

            DNS_DTO.DN_Id = 1;

            DNS_DTO.DN_CreatorCode = Convert.ToInt32(UserCode);

            DS = DN_DAO.DeliveryNoteSummaryDB(DNS_DTO);

            DNS_List = DN_DL.DNSummaryList(DS.Tables[0]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title_desc";
            }

            if (Convert.ToInt32(PageNumber) == 0)
            {
                DPageNumber = 1;
            }

            if (PageFilter?.ToLower() == "pagefilter")
            {
                DPageNumber = 1;
            }

            ViewData["CurrentSort"] = SortOrder;

            ViewData["KeySort"] =
                SortOrder == "Title"
                ? "Title_desc"
                : "Title";

            ViewData["CurrentFilter"] = Search;

            var Key = DNS_List
                .OrderByDescending(Cs => Cs.JIDNH_Number);

            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K =>

                    K.JIDNH_DN_Date.ToString().ToLower().Contains(Search.ToLower()) ||

                    K.JIDNH_DN_No.ToLower().Contains(Search.ToLower()) ||

                    K.CUS_Name.ToLower().Contains(Search.ToLower()) ||

                    K.CurrencyCode.ToLower().Contains(Search.ToLower()) ||

                    K.WarehouseCode.ToLower().Contains(Search.ToLower()) ||

                    K.JIDNH_MS_Number.ToString().ToLower().Contains(Search.ToLower()) ||

                    K.NoOfLineItems.ToString().ToLower().Contains(Search.ToLower()) ||

                    K.Qty.ToString().ToLower().Contains(Search.ToLower())

                ).OrderByDescending(Cs => Cs.JIDNH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":

                    Key = Key.OrderByDescending(K => K.JIDNH_DN_Date);

                    break;

                case "Title":

                    Key = Key.OrderBy(K => K.JIDNH_DN_Date);

                    break;

                default:

                    Key = Key.OrderByDescending(K => K.JIDNH_Number);

                    break;
            }

            if (PSize != 0)
            {
                DPageSize = PSize;
            }

            Int32 Record = Key.ToList().Count;

            if (PageNumber > 1)
            {
                Int32 RecordPage =
                    (Convert.ToInt32(PageNumber) - 1) * DPageSize;

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
                    Double Page =
                        Convert.ToDouble(Record) /
                        Convert.ToDouble(DPageSize);

                    Int32 PageCount =
                        Convert.ToInt32(Math.Ceiling(Page));

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

            Double Pages =
                Convert.ToDouble(Record) /
                Convert.ToDouble(DPageSize);

            Int32 PageCounts =
                Convert.ToInt32(Math.Ceiling(Pages));

            ViewBag.SumOfItem =
                Key.Sum(item => Convert.ToDouble(item.NoOfLineItems));

            ViewBag.SumOfQty =
                Key.Sum(item => Convert.ToDouble(item.Qty));

            ViewBag.SumOfAmount =
                Key.Sum(item => Convert.ToDouble(item.Amount));

            ViewBag.Page = Help.PageSize(PSize.ToString());

            ViewData["PageNumber"] = DPageNumber;

            ViewData["PageSize"] = DPageSize;

            ViewData["PageCount"] = PageCounts;

            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }
        #endregion


        #region Delivery Note detailed
        // Delivery Note Summary
        [Route("delivery-note/transactions/delivery-note-detailed")]
        public IActionResult DeliveryNoteDetailed(
            String? SortOrder,
            String? Search,
            Int32? PageNumber,
            Int32 PSize,
            String? PageFilter)
        {
            DND_List = DNDetailedGetData(
                SortOrder,
                Search,
                PageNumber,
                PSize,
                PageFilter);

            return View(
                PaginatedList_DTO<DeliveryNoteDetailed_DTO>
                .CreateAsync(DND_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("delivery-note/transactions/delivery-note-detailed")]
        [HttpPost]
        public IActionResult DeliveryNoteDetailed(
            String? SortOrder,
            String? Search,
            Int32? PageNumber,
            Int32 PSize,
            String? PageFilter,
            String? Mode,
            String? DeleteNumbers,
            String? DN_No,
            String[] DeleteNumber,
            String selectAllCheckbox)
        {
            DeliveryNoteSummary_DTO DN_DTO = new DeliveryNoteSummary_DTO();

            if (Mode == "Delete")
            {
                DN_DTO.JIDNH_Number = Convert.ToInt64(DN_No);

                DN_DTO.DN_Id = 104;

                DN_DTO.DN_CreatorCode = Convert.ToInt32(UserCode);

                DS = DN_DAO.DeliveryNoteSummaryDB(DN_DTO);

                return RedirectToAction("DeliveryNoteDetailed");
            }

            if (Mode == "View")
            {
                return RedirectToAction("DeliveryNoteView", new
                {
                    JIDNH_Number = DN_No
                });
            }

            if (Mode == "Edit")
            {
                return RedirectToAction("Edit", new
                {
                    JIDNH_Number = DN_No
                });
            }

            DND_List = DNDetailedGetData(
                SortOrder,
                Search,
                PageNumber,
                PSize,
                PageFilter);

            return View(
                PaginatedList_DTO<DeliveryNoteDetailed_DTO>
                .CreateAsync(DND_List, DPageNumber ?? 1, DPageSize));
        }

        List<DeliveryNoteDetailed_DTO> DNDetailedGetData(
    String? SortOrder,
    String? Search,
    Int32? PageNumber,
    Int32 PSize,
    String? PageFilter)
        {
            DPageSize = 10;

            DNS_DTO.DN_Id = 2;

            DNS_DTO.DN_CreatorCode = Convert.ToInt32(UserCode);

            DS = DN_DAO.DeliveryNoteSummaryDB(DNS_DTO);

            DND_List = DN_DL.DNDetailedList(DS.Tables[0]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title_desc";
            }

            if (Convert.ToInt32(PageNumber) == 0)
            {
                DPageNumber = 1;
            }

            if (PageFilter?.ToLower() == "pagefilter")
            {
                DPageNumber = 1;
            }

            ViewData["CurrentSort"] = SortOrder;

            ViewData["KeySort"] =
                SortOrder == "Title"
                ? "Title_desc"
                : "Title";

            ViewData["CurrentFilter"] = Search;

            var Key = DND_List
                .OrderByDescending(Cs => Cs.JIDNH_Number);

            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K =>

                    K.JIDNH_DN_Date.ToString().ToLower().Contains(Search.ToLower()) ||

                    K.JIDNH_DN_No.ToLower().Contains(Search.ToLower()) ||

                    K.CUS_Name.ToLower().Contains(Search.ToLower()) ||

                    K.CurrencyCode.ToLower().Contains(Search.ToLower()) ||

                    K.WarehouseCode.ToLower().Contains(Search.ToLower()) ||

                    K.JIDNH_MS_Number.ToString().ToLower().Contains(Search.ToLower()) 

                ).OrderByDescending(Cs => Cs.JIDNH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":

                    Key = Key.OrderByDescending(K => K.JIDNH_DN_Date);

                    break;

                case "Title":

                    Key = Key.OrderBy(K => K.JIDNH_DN_Date);

                    break;

                default:

                    Key = Key.OrderByDescending(K => K.JIDNH_Number);

                    break;
            }

            if (PSize != 0)
            {
                DPageSize = PSize;
            }

            Int32 Record = Key.ToList().Count;

            if (PageNumber > 1)
            {
                Int32 RecordPage =
                    (Convert.ToInt32(PageNumber) - 1) * DPageSize;

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
                    Double Page =
                        Convert.ToDouble(Record) /
                        Convert.ToDouble(DPageSize);

                    Int32 PageCount =
                        Convert.ToInt32(Math.Ceiling(Page));

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

            Double Pages =
                Convert.ToDouble(Record) /
                Convert.ToDouble(DPageSize);

            Int32 PageCounts =
                Convert.ToInt32(Math.Ceiling(Pages));

            //ViewBag.SumOfItem =
            //    Key.Sum(item => Convert.ToDouble(item.NoOfLineItems));

            //ViewBag.SumOfQty =
            //    Key.Sum(item => Convert.ToDouble(item.Qty));

            //ViewBag.SumOfAmount =
            //    Key.Sum(item => Convert.ToDouble(item.Amount));

            ViewBag.Page = Help.PageSize(PSize.ToString());

            ViewData["PageNumber"] = DPageNumber;

            ViewData["PageSize"] = DPageSize;

            ViewData["PageCount"] = PageCounts;

            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }
        #endregion

        #region delivery note view
        public ActionResult DeliveryNoteView(long JIDNH_Number)
        {
            List<DeliveryNoteCreate_DTO> DN_List =
                DeliveryNoteViewGetData(JIDNH_Number);

            DeliveryNoteCreate_DTO DN_DTO =
                new DeliveryNoteCreate_DTO();

          
            if (DN_List.Count > 0)
            {
                DN_DTO = DN_List.FirstOrDefault();
            }

         
            return View(DN_DTO);
        }
        public List<DeliveryNoteCreate_DTO> DeliveryNoteViewGetData(long JIDNH_Number)
        {
            List<DeliveryNoteCreate_DTO> DN_List =   new List<DeliveryNoteCreate_DTO>();

            DeliveryNote_DAO DN_DAO =  new DeliveryNote_DAO();

            DeliveryNote_DL DN_DL =   new DeliveryNote_DL();

            DataSet DS = new DataSet();

            //-----------------------------------------
            // GET DATA
            //-----------------------------------------
            DS = DN_DAO.DeliveryNoteViewDB(JIDNH_Number);

            //-----------------------------------------
            // CONVERT DATASET TO DTO LIST
            //-----------------------------------------
            DN_List =  DN_DL.DeliveryNoteViewList(DS);

            return DN_List;
        }
        #endregion

        #region delivery note edit

        public IActionResult Edit(long JIDNH_Number)
        {
            DeliveryNoteCreate_DTO dto =
                new DeliveryNoteCreate_DTO();

            DeliveryNote_DAO DN_DAO =
                new DeliveryNote_DAO();

           
            GetDevliverNoteData();

            
            DataSet DS = DN_DAO.DeliveryNoteEditDB(JIDNH_Number);


            if (DS.Tables[0].Rows.Count > 0)
            {
                DataRow dr = DS.Tables[0].Rows[0];

                dto.Header.JIDNH_Number =
                    Convert.ToInt64(dr["JIDNH_Number"]);

                dto.Header.JIDNH_DN_No =
                    Convert.ToString(dr["JIDNH_DN_No"]);

                dto.Header.JIDNH_DN_Date =
                    Convert.ToDateTime(dr["JIDNH_DN_Date"]);

                dto.Header.JIDNH_MS_Number =
                    Convert.ToInt32(dr["JIDNH_MS_Number"]);

                dto.Header.JIDNH_JW_Customer_Number =
                    Convert.ToInt64(dr["JIDNH_JW_Customer_Number"]);

                dto.Header.JIDNH_JW_Customer_Name =
                    Convert.ToString(dr["JIDNH_JW_Customer_Name"]);

                dto.Header.JIDNH_Currency_Number =
                    Convert.ToInt32(dr["JIDNH_Currency_Number"]);

                dto.Header.JIDNH_WH_Number =
                    Convert.ToInt32(dr["JIDNH_WH_Number"]);

                dto.Header.JIDNH_PaymentTerms =
                    Convert.ToString(dr["JIDNH_PaymentTerms"]);

                dto.Header.JIDNH_DeliveryTerms =
                    Convert.ToString(dr["JIDNH_DeliveryTerms"]);

                dto.Header.JIDNH_DeliveryMode =
                    Convert.ToString(dr["JIDNH_DeliveryMode"]);

                dto.Header.JIDNH_DespatchDocumentNo =
                    Convert.ToString(dr["JIDNH_DespatchDocumentNo"]);

                dto.Header.JIDNH_DespatchedThrough =
                    Convert.ToString(dr["JIDNH_DespatchedThrough"]);

                dto.Header.JIDNH_Remarks =
                    Convert.ToString(dr["JIDNH_Remarks"]);
            }

            // ITEMS
            dto.Items = new List<DeliveryNoteItem_DTO>();

            foreach (DataRow item in DS.Tables[1].Rows)
            {
                dto.Items.Add(new DeliveryNoteItem_DTO
                {
                    JIDNI_Number =
                        Convert.ToInt64(item["JIDNI_Number"]),

                    JIDNI_PRS_Number =
                        Convert.ToInt32(item["JIDNI_PRS_Number"]),

                    JIDNI_Item_Number =
                        Convert.ToInt64(item["JIDNI_Item_Number"]),

                    JIDNI_Item_Code =
                        Convert.ToString(item["JIDNI_Item_Code"]),

                    JIDNI_Item_Description =
                        Convert.ToString(item["JIDNI_Item_Description"]),

                    JIDNI_OuterDia =
                        Convert.ToString(item["JIDNI_OuterDia"]),

                    JIDNI_Thickness =
                        Convert.ToString(item["JIDNI_Thickness"]),

                    JIDNI_Length =
                        Convert.ToString(item["JIDNI_Length"]),

                    JIDNI_Width =
                        Convert.ToString(item["JIDNI_Width"]),

                    JIDNI_MaterialGrade =
                        Convert.ToString(item["JIDNI_MaterialGrade"]),

                    JIDNI_ItemGroup =
                        Convert.ToString(item["JIDNI_ItemGroup"]),

                    JIDNI_WH_Number =
                        Convert.ToInt32(item["JIDNI_WH_Number"]),

                    JIDNI_UoM_Number =
                        Convert.ToInt32(item["JIDNI_UoM_Number"]),

                    JIDNI_Qty =
                        Convert.ToDouble(item["JIDNI_Qty"]),

                    JIDNI_UnitPrice =
                        Convert.ToDouble(item["JIDNI_UnitPrice"]),

                    JIDNI_Amount =
                        Convert.ToDouble(item["JIDNI_Amount"]),

                    JIDNI_JW_InvoiceTracking =
                        Convert.ToString(item["JIDNI_JW_InvoiceTracking"]) 
                });
            }

            // ADDRESS
            dto.Addresses = new List<DeliveryNoteAddress_DTO>();

            foreach (DataRow add in DS.Tables[2].Rows)
            {
                dto.Addresses.Add(new DeliveryNoteAddress_DTO
                {
                    JIDNA_Address =
                        Convert.ToString(add["JIDNA_Address"])
                });
            }

            return View("Edit", dto);
        }
        #endregion

        #region Show Batch
        [HttpGet]
        public JsonResult GetBatchDetails(long FromWarehouse, long LineItem_Number,int ItemGridIndex)
        {
            DeliveryNote_DAO dao = new DeliveryNote_DAO();
            DataTable dt = dao.GetBatchDetailsDB(FromWarehouse, LineItem_Number, ItemGridIndex).Tables[0];
            var data = dt.AsEnumerable().Select(r => new
            {
                LineBatch_Number = r["LineBatch_Number"] == DBNull.Value ? 0 : Convert.ToInt64(r["LineBatch_Number"]),
                FromWarehouse = r["FromWarehouse"] == DBNull.Value ? 0 : Convert.ToInt64(r["FromWarehouse"]),

                BatchDate = r["BatchDate"] == DBNull.Value
                    ? ""
                    : Convert.ToDateTime(r["BatchDate"]).ToString("dd MMM yyyy"),

                BatchNo = r["BatchNo"] == DBNull.Value ? "" : r["BatchNo"].ToString(),

                BatchQty = r["BatchQty"] == DBNull.Value ? 0 : Convert.ToDecimal(r["BatchQty"]),
                ReservedQty = r["ReservedQty"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ReservedQty"]),
                DeliveredQty = r["DeliveredQty"] == DBNull.Value ? 0 : Convert.ToDecimal(r["DeliveredQty"]),
                AvailableQty = r["AvailableQty"] == DBNull.Value ? 0 : Convert.ToDecimal(r["AvailableQty"]),

                BatchUnitPrice = r["BatchUnitPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(r["BatchUnitPrice"]),
                BatchValue = r["BatchValue"] == DBNull.Value ? 0 : Convert.ToDecimal(r["BatchValue"]),

                WareHouseCode =
                    dt.Columns.Contains("WarehouseCode") && r["WarehouseCode"] != DBNull.Value
                    ? r["WarehouseCode"].ToString()
                    : ""
            }).ToList();

            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }
        #endregion

        #region Show Other Batch
        [HttpGet]
        public JsonResult GetOtherBatchDetails(long FromWarehouse, long LineItem_Number, int ItemGridIndex)
        {
            DeliveryNote_DAO dao = new DeliveryNote_DAO();
            DataTable dt = dao.GetOtherBatchDetailsDB(FromWarehouse, LineItem_Number, ItemGridIndex).Tables[0];
            var data = dt.AsEnumerable().Select(r => new
            {
                LineBatch_Number = r["LineBatch_Number"] == DBNull.Value ? 0 : Convert.ToInt64(r["LineBatch_Number"]),
                FromWarehouse = r["FromWarehouse"] == DBNull.Value ? 0 : Convert.ToInt64(r["FromWarehouse"]),

                BatchDate = r["BatchDate"] == DBNull.Value
                    ? ""
                    : Convert.ToDateTime(r["BatchDate"]).ToString("dd MMM yyyy"),

                BatchNo = r["BatchNo"] == DBNull.Value ? "" : r["BatchNo"].ToString(),

                BatchQty = r["BatchQty"] == DBNull.Value ? 0 : Convert.ToDecimal(r["BatchQty"]),
                ReservedQty = r["ReservedQty"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ReservedQty"]),
                DeliveredQty = r["DeliveredQty"] == DBNull.Value ? 0 : Convert.ToDecimal(r["DeliveredQty"]),
                AvailableQty = r["AvailableQty"] == DBNull.Value ? 0 : Convert.ToDecimal(r["AvailableQty"]),

                BatchUnitPrice = r["BatchUnitPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(r["BatchUnitPrice"]),
                BatchValue = r["BatchValue"] == DBNull.Value ? 0 : Convert.ToDecimal(r["BatchValue"]),

                WareHouseCode =
                    dt.Columns.Contains("WarehouseCode") && r["WarehouseCode"] != DBNull.Value
                    ? r["WarehouseCode"].ToString()
                    : ""
            }).ToList();

            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }
        #endregion

        #region temp batch save (BULK)
        [HttpPost]
        public IActionResult SaveTempDeliveryBatch([FromBody] List<TempDeliveryBatch_DTO> dtoList)
        {
            try
            {
                if (dtoList == null || dtoList.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "DTO list is empty"
                    });
                }

                DeliveryNote_DAO DBCH_DAO = new DeliveryNote_DAO();
 

                    DBCH_DAO.TempDeliveryBatchSaveDB(dtoList);
                

                return Json(new
                {
                    success = true,
                    message = "Batch Saved Successfully",
                    count = dtoList.Count
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

        #region temp batch delete row
        [Route("DeliveryNote/DeleteTempDeliveryBatchRow")]
        [HttpPost]
        public IActionResult DeleteTempDeliveryBatchRow(int index)
        {
            try
            {
                if (index <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid index"
                    });
                }

                DeliveryNote_DAO dbch_DAO = new DeliveryNote_DAO();

                dbch_DAO.TempDeliveryBatchDeleteDBRow(index);

                return Json(new
                {
                    success = true,
                    message = "Row deleted successfully",
                    index = index
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


        #region temp batch delete row
        [Route("DeliveryNote/TempDeliveryBatchDeleteChangeItemDBRow")]
        [HttpPost]
        public IActionResult TempDeliveryBatchDeleteChangeItemDBRow(int index)
        {
            try
            {
                if (index <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid index"
                    });
                }

                DeliveryNote_DAO dbch_DAO = new DeliveryNote_DAO();

                dbch_DAO.TempDeliveryBatchDeleteChangeItemDBRow(index);

                return Json(new
                {
                    success = true,
                    message = "Row deleted successfully",
                    index = index
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

    }
}

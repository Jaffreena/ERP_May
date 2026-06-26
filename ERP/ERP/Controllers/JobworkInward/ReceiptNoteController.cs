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
using System.Data;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Transactions;

namespace ERP.Controllers.JobworkInward
{
    public class ReceiptNoteController : Controller
    {
        Validation Valid = new Validation();
        Help Help = new Help();
        DataSet DS = new DataSet();
        ReceiptNoteEdit_DTO SIE_DTO = new ReceiptNoteEdit_DTO();
        ReceiptNote_DTO SI_DTO = new ReceiptNote_DTO();
        ReceiptNote_DAO SI_DAO = new ReceiptNote_DAO();
        ReceiptNote_DL S_DL = new ReceiptNote_DL();

        //Summary
         
        ReceiptNoteInfo_DTO RI_DTO = new ReceiptNoteInfo_DTO();
        ReceiptNoteSummary_DTO SIR_DTO = new ReceiptNoteSummary_DTO();
        ReceiptNoteSummary_DAO RNS_DAO = new ReceiptNoteSummary_DAO();
        List<ReceiptNoteSummary_DTO> SIR_List = new List<ReceiptNoteSummary_DTO>();


        //RN NUMBERING
        RNNumber_DTO PON_DTO = new RNNumber_DTO();
        RNNumber_DAO PON_DAO = new RNNumber_DAO();
        RNNumbering_DL PON_DL = new RNNumbering_DL();
        List<RNNumberReset_DTO> POR_List = new List<RNNumberReset_DTO>();
        List<RNNumberPrefix_DTO> POP_List = new List<RNNumberPrefix_DTO>();
        List<RNNumberSuffix_DTO> POS_List = new List<RNNumberSuffix_DTO>();

        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;

    
 

        [Route("jobinward/transactions/receipt-note/cutomer")]
        public IActionResult SaleBuyer(String? Buyer, String? Export, String? SIHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            if (Export == null)
            {
                Export = "";
            }
            SI_DTO.JIRN_CreatorCode = Convert.ToInt64(1);
        
            SI_DTO.JIRNI_ITM_Code = Convert.ToString(Buyer).Trim();
            SI_DTO.JIRNH_RN_Date = Convert.ToDateTime(SIHDate);
            SI_DTO.JIRN_Id = 5;
            DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);
            var Ven = S_DL.CustomerListData(DS.Tables[0]);
            return Json(Ven);
        }
        [Route("jobinward/transactions/receipt-note/item")]
        public IActionResult SaleItem(String? ItemCode, String MS)
        {
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

        [Route("jobinward/transactions/receipt-note/itemView")]
        public IActionResult SaleItem2(String? ItemCode, String MS)
        {
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
            SI_DTO.JIRN_Id = 211;
            DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);
            var Item = S_DL.ItemList(DS.Tables[0]);
            return Json(Item);
        }
        [HttpGet]
        [Route("jobinward/transactions/receipt-note/process")]
        public IActionResult JobInvardProcess(String? ProcessCode )
        {
            if (ProcessCode == null)
            {
                ProcessCode = "";
            }
            //if (MS == null)
            //{
            //    MS = "";
            //}
            SI_DTO.JIRN_CreatorCode = Convert.ToInt64(1);
            SI_DTO.JIRNI_PRS_Name = Convert.ToString(ProcessCode);
            SI_DTO.JIRNH_MS_Number = Convert.ToInt64(1);
            SI_DTO.JIRN_Id = 3;
            DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);
            var Item = S_DL.ProcessList(DS.Tables[0]);
            return Json(Item);
        }
      

        Boolean BatchValidation(List<ReceiptNoteItem_DTO> Item_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                Double BatchAmount = 0;

                SI_DTO.JIRNI_WH_Number = Convert.ToInt64(Item.WH_Number);
                SI_DTO.JIRNI_ITM_Code = Convert.ToString(Item.Item_Number);
                //SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                //SI_DTO.SI_BCH_Mode = Convert.ToInt32(11);
                SI_DTO.JIRN_CreatorCode = Convert.ToInt32(1);
                SI_DTO.JIRN_Id = 156;
                DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);

                Double Qty = Convert.ToDouble(Item.Qty);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    BatchQty = Convert.ToDouble(DS.Tables[0].Rows[0][0].ToString());
                }

                if (Qty < 0)
                {
                    Qty = Qty * -1;
                }

                if (BatchQty == Qty) { }
                else
                {
                    Message += Item.Item_Number + " Batch Qty  Mismatched <br/>";
                    Result = false;
                }

                if (Message != "")
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Message;
                }
            }
            return Result;
        }

    


        #region receiptnote numbering

        //Purchase Order Numbering
        [Route("receiptnote/setup/receiptnote-numbering")]
        public IActionResult RNNumbering()
        {

            GetRNNumber();
            return View(PON_DTO);
        }

        [Route("receiptnote/setup/receiptnote-numbering")]
        [HttpPost]
        public IActionResult RNNumbering(RNNumber_DTO PN_DTO)
        {
            bool IsValid = false;
            RNNumber_DTO P_Head_DTO = new RNNumber_DTO();

            List<RNNumberReset_DTO>? Reset_DTO = new List<RNNumberReset_DTO>();
            List<RNNumberPrefix_DTO>? Prefix_DTO = new List<RNNumberPrefix_DTO>();
            List<RNNumberSuffix_DTO>? Suffix_DTO = new List<RNNumberSuffix_DTO>();

            P_Head_DTO = PON_DTO;

            if (PN_DTO.RNNumberReset != null)
                Reset_DTO = PN_DTO.RNNumberReset!.Where(K => !K.RNR_IsDeleted).ToList();

            if (PN_DTO.RNNumberPrefix != null)
                Prefix_DTO = PN_DTO.RNNumberPrefix!.Where(K => !K.RNP_IsDeleted).ToList();  
            if (PN_DTO.RNNumberSuffix != null)
                Suffix_DTO = PN_DTO.RNNumberSuffix!.Where(K => !K.RNS_IsDeleted).ToList();

            if (PN_DTO.RNN_Method == "2")
            {
                String ResetDTO = string.Join(", ", Reset_DTO.Where(x => Convert.ToInt64(x.RNR_Number) != 0).Select(x => x.RNR_Number));
                String PrefixDTO = string.Join(", ", Prefix_DTO.Where(x => Convert.ToInt64(x.RNP_Number) != 0).Select(x => x.RNP_Number));
                String SuffixDTO = string.Join(", ", Suffix_DTO.Where(x => Convert.ToInt64(x.RNS_Number) != 0).Select(x => x.RNS_Number));


                PON_DTO.CreatorCode = Convert.ToInt32(UserCode);
                PON_DTO.DeleteNumbers = Convert.ToString(ResetDTO);
                PON_DTO.Id = 31;
                PON_DAO.RNNumberDB(PON_DTO);

                PON_DTO.DeleteNumbers = Convert.ToString(PrefixDTO);
                PON_DTO.Id = 32;
                PON_DAO.RNNumberDB(PON_DTO);    
                PON_DTO.DeleteNumbers = Convert.ToString(SuffixDTO);
                PON_DTO.Id = 33;
                PON_DAO.RNNumberDB(PON_DTO);

                PON_DTO.RNN_Method = PN_DTO.RNN_Method;
                if (PN_DTO.RNN_Number == 0)
                {
                    PON_DTO.Id = 11;
                }
                else
                {
                    PON_DTO.Id = 41;
                    PON_DTO.RNN_Number = PN_DTO.RNN_Number;
                }
                PON_DAO.RNNumberDB(PON_DTO);

                foreach (var Reset in Reset_DTO)
                {
                    PON_DTO.RNN_Date = Convert.ToString(Convert.ToDateTime(Reset.RNR_Date).ToString("yyyyMMdd"));
                    PON_DTO.RNN_StartingNumber = Convert.ToInt32(Reset.RNR_StartingNumber).ToString();
                    PON_DTO.RNN_NumberofDigits = Convert.ToInt32(Reset.RNR_NumberofDigits).ToString();
                    PON_DTO.RNN_PrefilZero = Convert.ToInt64(Reset.RNR_PrefilZero).ToString();
                    PON_DTO.RNN_Frequency    = Convert.ToInt64(Reset.RNR_Frequency).ToString();
                    if (Reset.RNR_Number == 0)
                    {
                        PON_DTO.Id = 12;
                    }
                    else
                    {
                        PON_DTO.Id = 42;
                        PON_DTO.RNN_Number = Reset.RNR_Number;
                    }
                    PON_DAO.RNNumberDB(PON_DTO);
                }

                foreach (var Prefix in Prefix_DTO)
                {
                    PON_DTO.RNN_Date = Convert.ToString(Convert.ToDateTime(Prefix.RNP_Date).ToString("yyyyMMdd"));
                    PON_DTO.RNN_Particulars = Convert.ToString(Prefix.RNP_Particulars);
                    if (Prefix.RNP_Number == 0)
                    {
                        PON_DTO.Id = 13;
                    }
                    else
                    {
                        PON_DTO.Id = 43;
                        PON_DTO.RNN_Number = Prefix.RNP_Number;
                    }
                    PON_DAO.RNNumberDB(PON_DTO);
                }

                foreach (var Suffix in Suffix_DTO)
                {
                    PON_DTO.RNN_Date = Convert.ToString(Convert.ToDateTime(Suffix.RNS_Date).ToString("yyyyMMdd"));
                    PON_DTO.RNN_Particulars = Convert.ToString(Suffix.RNS_Particulars);
                    if (Suffix.RNS_Number == 0)
                    {
                        PON_DTO.Id = 14;
                    }
                    else
                    {
                        PON_DTO.Id = 44;
                        PON_DTO.RNN_Number = Suffix.RNS_Number;
                    }
                    PON_DAO.RNNumberDB(PON_DTO);
                }
                PON_DTO.Reset();
                Reset_DTO = null;
                Prefix_DTO = null;
                Suffix_DTO = null;
                ModelState.Clear();
            }
            else if (PN_DTO.RNN_Method == "3")
            {
                PON_DTO.RNN_Method = PN_DTO.RNN_Method;
                if (PN_DTO.RNN_Number == 0)
                {
                    PON_DTO.Id = 21;
                }
                else
                {
                    PON_DTO.Id = 22;
                    PON_DTO.RNN_Number = PN_DTO.RNN_Number;
                }
                PON_DAO.RNNumberDB(PON_DTO);
            }

            GetRNNumber();
            return View(PON_DTO);
        }
        void GetRNNumber()
        {
            PON_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PON_DTO.Id = 1;
            DS = PON_DAO.RNNumberDB(PON_DTO);

            ViewBag.Method = Help.GetCat(DS.Tables[0]);
            ViewBag.Frequency = Help.GetCat(DS.Tables[1]);
            ViewBag.Prefil = Help.GetCat(DS.Tables[2]);

            if (DS.Tables[3].Rows.Count > 0)
            {
                PON_DTO.RNN_Number = Convert.ToInt64(DS.Tables[3].Rows[0]["RNN_Number"]);
                PON_DTO.RNN_Method = DS.Tables[3].Rows[0]["RNN_Method"].ToString();
            }

            PON_DTO.RNNumberReset = PON_DL.PORList(DS.Tables[4]);
            PON_DTO.RNNumberPrefix = PON_DL.POPList(DS.Tables[5]);
            PON_DTO.RNNumberSuffix = PON_DL.POSList(DS.Tables[6]);
        }




        [Route("receiptnote/transactions/receiptnote/numbering")]
        public String OnReceiptNoteNumber(Int32 PODate)
        {
            
            SI_DTO.RNH_Date = PODate;
            SI_DTO.JIRN_Id = 0;
            SI_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);

            if (DS.Tables[0].Rows.Count > 0 && DS.Tables[1].Rows.Count > 0)
            {
                Int32 Number = 0;
                Int32 Order = 0;
                String Prefix = "";
                String Surfix = "";
                String Prefil = "";
                String OrderNum = "";

                if (DS.Tables[0].Rows.Count > 0)
                {
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["RNN_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["RNP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["RNS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0 )
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_Frequency"].ToString());

                            //if (RFre == 4)
                            //{
                            Number = OrNum + 1;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                            //}
                            //else if (RFre == 5)
                            //{
                            //    Number = OrNum + 1;
                            //    if (RZero == 2)
                            //    {
                            //        Prefil = "D" + RDigit;
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["RNR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_Frequency"].ToString());

                            //if (RFre == 4)
                            //{
                            Number = RNumber;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                            //}
                            //else if (RFre == 5)
                            //{
                            //Number = RNumber;
                            //if (RZero == 2)
                            //{
                            //    Prefil = "D" + RDigit;
                            //}
                            //}
                        }
                    }
                    OrderNum = Prefix + "" + Number.ToString(Prefil) + "" + Surfix;

                    return OrderNum;
                }
            }
            else
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = "RN number Not assigned for Given date";
            }
            return "";
        }


        #endregion

        #region receipt-note summary
        //Sale Invoice Detailed
        [Route("receipt-note/transactions/receipt-note-detailed")]
        public IActionResult ReciptNoteSummaryDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            SIR_List = SIDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<ReceiptNoteSummary_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("receipt-note/transactions/receipt-note-detailed")]
        [HttpPost]
        public IActionResult ReciptNoteSummaryDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? SI_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            ReceiptNote_DTO SH_DTO = new ReceiptNote_DTO();
              if (Mode == "Delete")
            {
                SH_DTO.JIRNH_Number = Convert.ToInt64(SI_No);
                SH_DTO.JIRN_Id = 104;
                SH_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
                DS = SI_DAO.JI_ReceiptNoteDB(SH_DTO);
                return RedirectToAction("ReciptNoteSummaryDetailed");
            }

            else if (Mode == "View")
            {


                return RedirectToAction("PreviewReceiptNote", new
                {
                    SI_No = SI_No
                });
            }
            else if (Mode == "Edit")
            {


                return RedirectToAction("EditReceiptNote", new
                {
                    SI_No = SI_No
                });
            }
            SIR_List = SIDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<ReceiptNoteSummary_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }

        //Sale Invoice summary
        [Route("receipt-note/transactions/receipt-note-summary")]
        public IActionResult ReciptNoteSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            SIR_List = SISummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<ReceiptNoteSummary_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }
        [Route("receipt-note/transactions/receipt-note-summary")]
        [HttpPost]
        public IActionResult ReciptNoteSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? SI_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            ReceiptNote_DTO SH_DTO = new ReceiptNote_DTO();
            if (Mode == "Delete")
            {
                SH_DTO.JIRNH_Number = Convert.ToInt64(SI_No);
                SH_DTO.JIRN_Id = 104;
                SH_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
                DS = SI_DAO.JI_ReceiptNoteDB(SH_DTO);
                return RedirectToAction("ReciptNoteSummaryDetailed");
            }
            if (Mode == "View")
            {
                 

                return RedirectToAction("PreviewReceiptNote", new
                {
                    SI_No = SI_No
                });
            }
            if (Mode == "Edit")
            {


                return RedirectToAction("EditReceiptNote", new
                {
                    SI_No = SI_No
                });
            }
            SIR_List = SISummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<ReceiptNoteSummary_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }
     
        
        List<ReceiptNoteSummary_DTO> SIDetailedGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            SI_DTO.JIRN_Id = 52;
            SI_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
            DS = RNS_DAO.JI_ReceiptNoteDB(SI_DTO);
            SIR_List = S_DL.RNDetailedList(DS.Tables[0]);

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

            var Key = SIR_List.OrderByDescending(Cs => Cs.RN_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.RN_Date!.ToLower().Contains(Search.ToLower()) ||
                         K.RN_No!.ToLower().Contains(Search.ToLower()) ||
                         K.RN_JWC_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.RN_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.RN_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.RN_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.RN_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.RN_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.RN_UoM_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.RN_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.RN_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.RN_TotalAmount!.ToString().ToLower().Contains(Search.ToLower())  
                          ).OrderByDescending(Cs => Cs.RN_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.RN_Date)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.RN_Date)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.RN_Number);
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
                    DPageNumber = Convert.ToInt32(PageNumber) - 1;
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

            ViewBag.SumOfQty = Key.Sum(item => Convert.ToDouble(item.RN_Qty));
            ViewBag.SumOfMaterialValue = Key.Sum(item => Convert.ToDouble(item.RN_MaterialValue));
            //ViewBag.SumOfItemIncome = Key.Sum(item => Convert.ToDouble(item.SI_TotalItemIncome));
            //ViewBag.SumOfHeadIncome = Key.Sum(item => Convert.ToDouble(item.SI_TotalHeadIncome));
            ViewBag.SumOfAmount = Key.Sum(item => Convert.ToDouble(item.RN_TotalAmount));
            //ViewBag.SumOfItemGST = Key.Sum(item => Convert.ToDouble(item.SI_ItemGST_Amount));
            //ViewBag.SumOfHeadGST = Key.Sum(item => Convert.ToDouble(item.SI_HeadGST_Amount));
            //ViewBag.SumOfInvoiceAmount = Key.Sum(item => Convert.ToDouble(item.SI_InvoiceAmount));
            //ViewBag.SumOfItemWHT = Key.Sum(item => Convert.ToDouble(item.SI_ItemWHT_Amount));
            //ViewBag.SumOfHeadWHT = Key.Sum(item => Convert.ToDouble(item.SI_HeadWHT_Amount));
            //ViewBag.SumOfRoundOff = Key.Sum(item => Convert.ToDouble(item.SI_RoundOff));
            //ViewBag.SumOfReceivable = Key.Sum(item => Convert.ToDouble(item.SI_BuyerReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }


        List<ReceiptNoteSummary_DTO> SISummaryGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            SI_DTO.JIRN_Id = 51;
            SI_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
            DS = RNS_DAO.JI_ReceiptNoteDB(SI_DTO);
            SIR_List = S_DL.RNSummaryList(DS.Tables[0]);

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

            var Key = SIR_List.OrderByDescending(Cs => Cs.RN_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                //Key = Key.Where(K => K.SI_InvoiceDate.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_InvoiceNo.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_BUY_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_CUR_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_MS_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_NoOfItem.ToString().ToLower().Contains(Search.ToLower()) ||
                // K.SI_Qty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SI_Number);


                Key = Key.Where(K => K.RN_Date.ToString().ToLower().Contains(Search.ToLower()) ||
                K.RN_No.ToString().ToLower().Contains(Search.ToLower()) ||
                K.RN_JWC_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                K.RN_CUR_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                K.RN_MS_Number.ToString().ToLower().Contains(Search.ToLower()) ||
                K.RN_NoOfItem.ToString().ToLower().Contains(Search.ToLower()) ||
                K.RN_Qty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.RN_Number);



            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.RN_Date)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.RN_Date)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.RN_Number);
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

            ViewBag.SumOfItem = Key.Sum(item => Convert.ToDouble(item.RN_NoOfItem));
            ViewBag.SumOfQty = Key.Sum(item => Convert.ToDouble(item.RN_Qty));
            ViewBag.SumOfMatrialValue = Key.Sum(item => Convert.ToDouble(item.RN_MaterialValue));
         //  ViewBag.SumOfItemIncome = Key.Sum(item => Convert.ToDouble(item.RN_TotalItemIncome));
          //  ViewBag.SumOfHeadIncome = Key.Sum(item => Convert.ToDouble(item.RN_TotalHeadIncome));
            ViewBag.SumOfAmount = Key.Sum(item => Convert.ToDouble(item.RN_TotalAmount));
            ViewBag.SumOfItemGst = Key.Sum(item => Convert.ToDouble(item.RN_ItemGST_Amount));
            ViewBag.SumOfHeadGst = Key.Sum(item => Convert.ToDouble(item.RN_HeadGST_Amount));
            ViewBag.SumOfInvoice = Key.Sum(item => Convert.ToDouble(item.RN_InvoiceAmount));
            ViewBag.SumOfItemWHT = Key.Sum(item => Convert.ToDouble(item.RN_ItemWHT_Amount));
            ViewBag.SumOfHeadWHT = Key.Sum(item => Convert.ToDouble(item.RN_HeadWHT_Amount));
            ViewBag.SumOfRound = Key.Sum(item => Convert.ToDouble(item.RN_RoundOff));
         //   ViewBag.SumOfReceivable = Key.Sum(item => Convert.ToDouble(item.RN_BuyerReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }


        #endregion

        #region receipt-note view
        public List<ReceiptNoteBatch_DTO> BatchList(DataTable dt)
        {
            List<ReceiptNoteBatch_DTO> list =
                new List<ReceiptNoteBatch_DTO>();

            foreach (DataRow dr in dt.Rows)
            {
                ReceiptNoteBatch_DTO batch =
                    new ReceiptNoteBatch_DTO();

                batch.RNI_BCH_Number =
                    Convert.ToString(dr["JIRNI_BCH_Number"].ToString());

                batch.JIRNI_BCH_JIRNH_Number =
                    Convert.ToInt64(dr["JIRNI_BCH_JIRNH_Number"]);

                batch.JIRNI_BCH_JIRNI_Number =
                    Convert.ToInt64(dr["JIRNI_BCH_JIRNI_Number"]);

                batch.RNI_BCH_WH_Number =
                    dr["JIRNI_BCH_WH_Number"].ToString();

                batch.RNI_BCH_Date =
                    Convert.ToDateTime(dr["JIRNI_BCH_BatchDate"])
                    .ToString("dd/MM/yyyy");
                batch.RNI_BCH_No = 0;
                batch.RNI_BCH_Num =
                    dr["JIRNI_BCH_BatchNo"] == DBNull.Value
                        ? ""
                        : Convert.ToString(dr["JIRNI_BCH_BatchNo"]);

                batch.RNI_BCH_Qty =
                    dr["JIRNI_BCH_BatchQty"].ToString();

                batch.RNI_BCH_UnitPrice =
                    dr["JIRNI_BCH_BatchUnitPrice"].ToString();

                batch.RNI_BCH_Value =
                    dr["JIRNI_BCH_BatchValue"].ToString();

                batch.WH_Number =
                    dr["JIRNI_BCH_WH_Number"].ToString();

                batch.RNI_BCH_Item_Number =
                    dr["JIRNI_Item_Number"].ToString();

                list.Add(batch);
            }

            return list;
        }


        [HttpGet]
        [Route("jobinward/transactions/receipt-note/getbatch")]
        public IActionResult GetBatch(long JIRNH_Number, long JIRNI_Number)
        {
            ReceiptNote_DTO model = new ReceiptNote_DTO();

            model.JIRNH_Number = JIRNH_Number;
            model.JIRNI_Number = JIRNI_Number;
            model.JIRN_Id = 103;
            DataSet ds = SI_DAO.JI_ReceiptNoteDB(model);

            List<ReceiptNoteBatch_DTO> batchList =
                BatchList(ds.Tables[0]);

            return PartialView(
                "CreateReceiptNoteBatchView",
                batchList
            );
        }


        [HttpGet]
        [Route("jobinward/transactions/receipt-note/getbatchedit")]
        public IActionResult GetBatchEdit(long JIRNH_Number, long JIRNI_Number)
        {
            ReceiptNote_DTO model = new ReceiptNote_DTO();

            model.JIRNH_Number = JIRNH_Number;
            model.JIRNI_Number = JIRNI_Number;
            model.JIRN_Id = 103;
            DataSet ds = SI_DAO.JI_ReceiptNoteDB(model);

            List<ReceiptNoteBatchEdit_DTO> batchList =
                BatchListEdit(ds.Tables[0]);

            return PartialView(
                "EditReceiptNoteBatch",
                batchList
            );
        }

        //Sale Preview
        //  [Route("receipt-note/transactions/receipt-note/{SI_No}/view")]
        public IActionResult PreviewReceiptNote(String? SI_No)
        {
            ReceiptNoteHead_DTO SH_DTO = new ReceiptNoteHead_DTO();
            if (TempData["SH_DTO_Json"] is string SHto)
            {
                SH_DTO = System.Text.Json.JsonSerializer.Deserialize<ReceiptNoteHead_DTO>(SHto);
            }
            SH_DTO.JW_CustomerDC_Date = DateTime.Now.ToString("dd-MMM-yy");
            SH_DTO.RN_No = OnReceiptNoteNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            ReceiptGetData();
            SH_DTO= GetRNEditData(SI_No);
            return View(SH_DTO);
        }



      //  [Route("sale/transactions/sale-invoice-register-summary")]
        [HttpPost]
        public IActionResult PreviewReceiptNote(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? SI_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            ReceiptNoteHead_DTO SH_DTO = new ReceiptNoteHead_DTO();

            if (Mode == "View")
            {
                //SH_DTO.JIRNH_Number = Convert.ToInt32(SI_No);
                //SI_DTO.JIRN_Id = 104;
                //SI_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
                //DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);

                //if (DS.Tables[0].Rows.Count > 0)
               // {
                    return RedirectToAction("PreviewReceiptNote", new { SI_No = SI_No });
               // }
            }

            SH_DTO = GetRNEditData(SI_No);
            return View(SH_DTO);
        }

        public ReceiptNoteHead_DTO GetRNEditData(String SI_No)
        {
            ReceiptNoteHead_DTO SH_DTO = new ReceiptNoteHead_DTO();
            ReceiptNote_DTO S_DTO = new ReceiptNote_DTO();
            S_DTO.JIRNH_Number = Convert.ToInt64(SI_No);
            S_DTO.JIRN_Id = 102;
           // SH_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.JI_ReceiptNoteDB(S_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SH_DTO = S_DL.ReceiptNoteHeadEditList(DS.Tables[0]).FirstOrDefault();
                SH_DTO.Items = S_DL.ReceiptNoteItemList(DS.Tables[1]);
                SH_DTO.ItemBatch = S_DL.ReceiptNoteBatchList(DS.Tables[2]);
              
               
              //  ViewBag.Mode = DS.Tables[0].Rows[0]["SIH_Mode"].ToString();
                return SH_DTO;
            }
            else
            {
                return SH_DTO;
            }
        }

        #endregion


        #region receipt-note-edit

        public List<ReceiptNoteBatchEdit_DTO> BatchListEdit(DataTable dt)
        {
            List<ReceiptNoteBatchEdit_DTO> list =
                new List<ReceiptNoteBatchEdit_DTO>();

            foreach (DataRow dr in dt.Rows)
            {
                ReceiptNoteBatchEdit_DTO batch =
                    new ReceiptNoteBatchEdit_DTO();

                batch.RNI_BCH_Number =
                    Convert.ToString(dr["JIRNI_BCH_Number"].ToString());

                batch.JIRNI_BCH_JIRNH_Number =
                    Convert.ToInt64(dr["JIRNI_BCH_JIRNH_Number"]);

                batch.JIRNI_BCH_JIRNI_Number =
                    Convert.ToInt64(dr["JIRNI_BCH_JIRNI_Number"]);

                batch.RNI_BCH_WH_Number =
                    dr["JIRNI_BCH_WH_Number"].ToString();

                batch.RNI_BCH_Date =
                    Convert.ToDateTime(dr["JIRNI_BCH_BatchDate"])
                    .ToString("dd/MM/yyyy");
                batch.RNI_BCH_No = 0;
                batch.RNI_BCH_Num =
                    dr["JIRNI_BCH_BatchNo"] == DBNull.Value
                        ? ""
                        : Convert.ToString(dr["JIRNI_BCH_BatchNo"]);

                batch.RNI_BCH_OriginalQty =
                    dr["JIRNI_BCH_BatchQty"].ToString();

                batch.RNI_BCH_UnitPrice =
                    dr["JIRNI_BCH_BatchUnitPrice"].ToString();

                batch.RNI_BCH_Value =
                    dr["JIRNI_BCH_BatchValue"].ToString();

                batch.WH_Number =
                    dr["JIRNI_BCH_WH_Number"].ToString();

                batch.RNI_BCH_Item_Number =
                    dr["JIRNI_Item_Number"].ToString();

                list.Add(batch);
            }

            return list;
        }


        public ReceiptNoteHeadEdit_DTO GetRNEditSelfData(String SI_No)
        {
            ReceiptNoteHeadEdit_DTO SH_DTO = new ReceiptNoteHeadEdit_DTO();
            ReceiptNote_DTO S_DTO = new ReceiptNote_DTO();
            S_DTO.JIRNH_Number = Convert.ToInt64(SI_No);
            S_DTO.JIRN_Id = 102;
            // SH_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.JI_ReceiptNoteDB(S_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SH_DTO = S_DL.ReceiptNoteHeadEditSelfList(DS.Tables[0]).FirstOrDefault();
                SH_DTO.Items = S_DL.ReceiptNoteItemSelfList(DS.Tables[1]);
                SH_DTO.ItemBatch = S_DL.ReceiptNoteBatchSelfList(DS.Tables[2]);


                //  ViewBag.Mode = DS.Tables[0].Rows[0]["SIH_Mode"].ToString();
                return SH_DTO;
            }
            else
            {
                return SH_DTO;
            }
        }
        [Route("receiptnote/transactions/create")]
        public IActionResult EditReceiptNote(String? SI_No)
        {
            ReceiptNoteHeadEdit_DTO SH_DTO = new ReceiptNoteHeadEdit_DTO();
            if (TempData["SH_DTO_Json"] is string SHto)
            {
                SH_DTO = System.Text.Json.JsonSerializer.Deserialize<ReceiptNoteHeadEdit_DTO>(SHto);
            }
            SH_DTO.JW_CustomerDC_Date = DateTime.Now.ToString("dd-MMM-yy");
            SH_DTO.RN_No = OnReceiptNoteNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            ReceiptGetData();
            SH_DTO = GetRNEditSelfData(SI_No);
            return View(SH_DTO);
        }

        #region Validation
        private string ValidateItemBatchQty(
      List<ReceiptNoteItemEdit_DTO> ITM_DTO,
      List<ReceiptNoteBatchEdit_DTO> BCH_DTO)
        {
            List<string> errors = new List<string>();

            // ================================
            // NULL CHECK
            // ================================
            if (ITM_DTO == null || ITM_DTO.Count == 0)
                return "";

            if (BCH_DTO == null)
                BCH_DTO = new List<ReceiptNoteBatchEdit_DTO>();

            // ================================
            // LOOP EACH ITEM
            // ================================
            for (int i = 0; i < ITM_DTO.Count; i++)
            {
                var item = ITM_DTO[i];

                // Skip deleted rows
                if (item.IsDeleted == "1")
                    continue;

                // ============================================
                // GET BATCHES FOR CURRENT ITEM
                // ============================================
                var itemBatches = BCH_DTO
                    .Where(x =>
                        x.RNI_BCH_Item_Number == item.RNI_Item_Number &&
                        (x.RNI_BCH_IsDeleted == "false" ||
                         x.RNI_BCH_IsDeleted == null))
                    .ToList();

                // ============================================
                // VALIDATION 1:
                // ITEM MUST HAVE ATLEAST ONE BATCH
                // ============================================
                if (itemBatches.Count == 0)
                {
                    errors.Add(
                        $"Grid Row {i + 1} : Batch details missing"
                    );

                    continue;
                }

                // ============================================
                // ITEM AMENDED QTY
                // ============================================
                decimal itemQty = 0;

                decimal.TryParse(item.AmendQty, out itemQty);

                // ============================================
                // SUM OF BATCH QTY
                // ============================================
                decimal batchQtySum = itemBatches.Sum(x =>
                {
                    decimal qty = 0;
                    decimal.TryParse(x.RNI_BCH_AmendedQty, out qty);
                    return qty;
                });

                // ============================================
                // VALIDATION 2:
                // ITEM QTY == BATCH SUM
                // ============================================
                if (itemQty != batchQtySum)
                {
                    errors.Add(
                        $"Grid Row {i + 1} : Item Qty ({itemQty}) " +
                        $"does not match Batch Qty Total ({batchQtySum})"
                    );
                }
            }

            // ============================================
            // RETURN ERRORS
            // ============================================
            return string.Join("<br/>", errors);
        }
        private string ValidateItemBatchMapping(
      List<ReceiptNoteItemEdit_DTO> ITM_DTO,
      List<ReceiptNoteBatchEdit_DTO> BCH_DTO)
        {
            List<string> errors = new List<string>();

            if (ITM_DTO == null || ITM_DTO.Count == 0)
                return "";

            int rowNo = 1;

            foreach (var item in ITM_DTO)
            {
                // Skip deleted items
                if (item.IsDeleted == "1")
                {
                    rowNo++;
                    continue;
                }

                // Check batch exists
                bool batchExists = BCH_DTO.Any(b =>
                    (b.RNI_BCH_IsDeleted == "false" || b.RNI_BCH_IsDeleted == null)
                    &&
                    b.RNI_BCH_Item_Number == item.JIRNI_Number.ToString()
                );

                // Validation fail
                if (!batchExists)
                {
                    errors.Add($"Batch missing for Item Grid Row : {rowNo}");
                }

                rowNo++;
            }

            return string.Join("<br>", errors);
        }
        #endregion

        [HttpPost]
        [Route("receiptnote/transactions/editpost")]
      
        public IActionResult EditReceiptNotePost(ReceiptNoteHeadEdit_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            ReceiptNoteHeadEdit_DTO S_Head_DTO = new ReceiptNoteHeadEdit_DTO();

            List<ReceiptNoteItemEdit_DTO>? ITM_DTO = new List<ReceiptNoteItemEdit_DTO>();
            List<ReceiptNoteBatchEdit_DTO>? BCH_DTO = new List<ReceiptNoteBatchEdit_DTO>();
            S_Head_DTO = S_DTO;

            if (S_DTO.Items != null)
                ITM_DTO = S_DTO.Items!.Where(K => K.IsDeleted == "0" || K.IsDeleted == null).ToList();

            if (S_DTO.Items != null)
                BCH_DTO = S_DTO.ItemBatch!.Where(K => K.RNI_BCH_IsDeleted == "false" || K.RNI_BCH_IsDeleted == null).ToList();

            SI_DTO.JIRN_CreatorCode = Convert.ToInt64(1);
            // SI_DTO.JIRN_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Update")
            {
                //var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.Item_Number) != Convert.ToInt64(S_DTO.MS_Number));
                ITM_DTO = ITM_DTO
    .Where(x => !string.IsNullOrWhiteSpace(x.Item_Number))
    .ToList();
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.AmendQty) == 0 || Convert.ToDouble(x.UnitPrice) == 0 || Convert.ToDouble(x.Amount) == 0);

                //if (CheckItem.ToList().Count > 0)
                //{
                //    ViewBag.ErrorCode = 2;
                //    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                //}
                //else
                string batchValidation = ValidateItemBatchMapping(ITM_DTO, BCH_DTO);
                string qtyValidation = ValidateItemBatchQty(ITM_DTO, BCH_DTO);
                if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Amended Qty or Unit Price is must be greater than 0 in Item Grid for all items";
                }
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Please fill at least one row in Item Grid";
                }
                else if (!string.IsNullOrEmpty(batchValidation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = batchValidation;
                }
                else if (!string.IsNullOrEmpty(qtyValidation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = qtyValidation;
                }
                else if (Convert.ToInt32(S_DTO.JWC_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.Items = ITM_DTO;

                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        // if (BatchValidation(ITM_DTO))
                        {
                            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
      new TransactionOptions
      {
          IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
      }))

                            {
                                try
                                {
                                    string SIHOrderNoOld = S_DTO.RN_No;

                                    String SIHOrderNoNew = OnReceiptNoteNumber(Convert.ToInt32(Convert.ToDateTime(S_DTO.RN_Date).ToString("yyyyMMdd")));

                                    // =========================
                                    // 🔹 HEADER INSERT
                                    // =========================
                                    SIE_DTO.JIRNH_RN_No = SIHOrderNoOld;
                                    SIE_DTO.JIRNH_RN_Date = Convert.ToDateTime(S_DTO.RN_Date);
                                    SIE_DTO.JIRNH_JW_CustomerDC_No = Convert.ToString(S_DTO.JW_CustomerDC_No);
                                    SIE_DTO.JIRNH_JW_CustomerDC_Date = Convert.ToDateTime(S_DTO.JW_CustomerDC_Date);
                                  
                                    SIE_DTO.JIRNH_MS_Number = Convert.ToInt64(S_DTO.MS_Number);
                                    SIE_DTO.JIRNH_JWC_Number = Convert.ToInt64(S_DTO.JWC_Number);
                                    SIE_DTO.JIRNH_Currency_Number = Convert.ToInt64(S_DTO.Currency_Number);
                                    SIE_DTO.JIRNH_WH_Number = Convert.ToInt64(S_DTO.WH_Number);
                                    SIE_DTO.JIRNH_Remarks = Convert.ToString(S_DTO.Remarks);
                               SIE_DTO.JIRNH_Number = Convert.ToInt64(S_DTO.JIRNH_Number);
                                    SIE_DTO.JIRN_Id = 121;

                                    DS = SI_DAO.JI_ReceiptNoteEditDB(SIE_DTO);



                                    if (DS == null || DS.Tables.Count == 0 || DS.Tables[0].Rows.Count == 0)
                                        throw new Exception("Header insert failed");

                                    long headerId = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                    // =========================
                                    // 🔹 ITEM LOOP
                                    // =========================
                                    foreach (var Item in ITM_DTO)
                                    {
                                        var itemDTO = new ReceiptNoteEdit_DTO(); // 🔥 IMPORTANT FIX (NO REUSE)

                                        itemDTO.JIRNH_Number = headerId;
                                        itemDTO.JIRNI_Item_Number = Convert.ToInt64(Item.Item_Number);
                                        itemDTO.JIRNI_WH_Number = Convert.ToInt64(Item.WH_Number);
                                        itemDTO.JIRNI_UoM_Number = Convert.ToInt64(Item.UoM_Number);
                                        itemDTO.JIRNI_AmendedQty = Convert.ToDouble(Item.AmendQty); //changed
                                        itemDTO.JIRNI_UnitPrice = Convert.ToDouble(Item.UnitPrice);
                                        itemDTO.JIRNI_Amount = Convert.ToDouble(Item.Amount);
                                        itemDTO.JIRNI_PRS_Number = Convert.ToInt64(Item.PRS_Number);
                                        itemDTO.JIRN_Id = 122;

                                        var itemResult = SI_DAO.JI_ReceiptNoteEditDB(itemDTO);

                                        if (itemResult == null || itemResult.Tables.Count == 0 || itemResult.Tables[0].Rows.Count == 0)
                                            throw new Exception("Item insert failed");

                                        long itemID = Convert.ToInt64(itemResult.Tables[0].Rows[0][0]);

                                        // =========================
                                        // 🔹 BATCH INSERT (FIXED)
                                        // =========================

                                        var relatedBatches = BCH_DTO
                                            .Where(x => x.RNI_BCH_Item_Number == Item.Item_Number)
                                            .ToList();

                                        foreach (var batch in relatedBatches)
                                        {
                                            var batchDTO = new ReceiptNoteEdit_DTO(); // 🔥 NEW OBJECT

                                            batchDTO.JIRNI_BCH_Number = batch.RNI_BCH_No;
                                            batchDTO.JIRNI_Item_Number = Convert.ToInt64(Item.Item_Number);
                                            batchDTO.JIRNH_Number = headerId;
                                            batchDTO.JIRNI_Number = itemID;
                                            batchDTO.JIRNI_BCH_BatchDate = DateTime.Now;
                                            //   batchDTO.JIRNI_BCH_BatchNo = "AUTO-" + headerId;
                                            batchDTO.JIRNI_BCH_BatchNo = batch.RNI_BCH_Number;
                                            batchDTO.JIRNI_BCH_WH_Number = Convert.ToInt64(Item.WH_Number);
                                            //batchDTO.JIRNI_BCH_BatchOriginalQty = Convert.ToDouble(batch.RNI_BCH_OriginalQty);
                                            //batchDTO.JIRNI_BCH_BatchUsedQty = Convert.ToDouble(batch.RNI_BCH_UsedQty);
                                            batchDTO.JIRNI_BCH_BatchAmendedQty = Convert.ToDouble(batch.RNI_BCH_AmendedQty);
                                            batchDTO.JIRNI_BCH_BatchUnitPrice = Convert.ToDouble(batch.RNI_BCH_UnitPrice);
                                            batchDTO.JIRNI_BCH_BatchValue = Convert.ToDouble(batch.RNI_BCH_Value);
                                            batchDTO.JIRN_Id = 123;

                                            SI_DAO.JI_ReceiptNoteEditDB(batchDTO);
                                        }
                                    }

                                    // =========================
                                    // 🔹 COMMIT
                                    // =========================
                                    transaction.Complete();

                                    // =========================
                                    // 🔹 CLEANUP
                                    // =========================
                                    S_Head_DTO.Reset();
                                    ITM_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    if (SIHOrderNoOld != SIHOrderNoNew)
                                    {
                                        ViewBag.ErrorCode = 2;
                                        ViewBag.ErrorMessage =
                                            "Receipt Note number " + SIHOrderNoOld + " changed to " + SIHOrderNoNew;
                                    }

                                    return RedirectToAction("CreateReceiptNote");
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                    throw;
                                }
                            }
                        }

                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;
                    }
                }
            }


            ReceiptGetData();
            return View("EditReceiptNote", Original_DTO);
        }

        #endregion



        #region create Receipt Note New




        [Route("procurement/transactions/purchase-invoice/po/numbering")]
        public String OnReceiptNumber(Int32 PIDate)
        {
            SI_DTO.RNH_Date = Convert.ToInt64(PIDate);
            SI_DTO.JIRN_Id = 101;
            SI_DTO.JIRN_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);

            if (DS.Tables[0].Rows.Count > 0 && DS.Tables[1].Rows.Count > 0)
            {
                Int32 Number = 0;
                Int32 Order = 0;
                String Prefix = "";
                String Surfix = "";
                String Prefil = "";
                String OrderNum = "";

                if (DS.Tables[0].Rows.Count > 0)
                {
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["RNN_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["RNP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["RNS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0)
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_Frequency"].ToString());

                            //if (RFre == 4)
                            //{
                            Number = OrNum + 1;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                            //}
                            //else if (RFre == 5)
                            //{
                            //    Number = OrNum + 1;
                            //    if (RZero == 2)
                            //    {
                            //        Prefil = "D" + RDigit;
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["PIR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["RNR_Frequency"].ToString());

                            //if (RFre == 4)
                            //{
                            Number = RNumber;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                            //}
                            //else if (RFre == 5)
                            //{
                            //Number = RNumber;
                            //if (RZero == 2)
                            //{
                            //    Prefil = "D" + RDigit;
                            //}
                            //}
                        }
                    }
                    OrderNum = Prefix + "" + Number.ToString(Prefil) + "" + Surfix;

                    return OrderNum;
                }
            }
            else
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = "GRN Number Not assigned for Given date";
            }
            return "";
        }


        Boolean DirectBatchValidation(List<ReceiptNoteItem_DTO> Item_DTO, List<ReceiptNoteBatch_DTO>? ItemBatch_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                //Double BatchAmount = 0;

                Int64 PIINumber = Convert.ToInt64(Item.JIRNI_Number);
                Int64 PIIIndex = Convert.ToInt64(Item.RNI_Index);
                String PIIItem = Convert.ToString(Item.RNI_Item_Number);
                Double PIIQty = Convert.ToDouble(Item.RNI_Qty);
                Double PIIAmount = Convert.ToDouble(Item.RNI_Amount);

                if (PIINumber != 0)
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.RNI_BCH_RNI_Number == PIINumber));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.RNI_BCH_Qty);
                    }
                }
                else
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.RNI_BCH_Item_Number == PIIItem) && (x.RNI_BCH_Item_Index == PIIIndex));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.RNI_BCH_Qty);
                    }
                }


                if (BatchQty == PIIQty) { }
                else
                {
                    Message += Item.RNI_ItemCode + " Batch Qty  Mismatched <br/>";
                    Result = false;
                }

                if (Message != "")
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Message;
                }
            }
            return Result;
        }


        //Purchase Invoice Create
        [Route("receipt-note/transactions/create")]
        public IActionResult ReceiptCreate()
        {
            ReceiptNoteHead_DTO PH_DTO = new ReceiptNoteHead_DTO();
            if (TempData["PH_DTO_Json"] is string PHto)
            {
                PH_DTO = System.Text.Json.JsonSerializer.Deserialize<ReceiptNoteHead_DTO>(PHto);
            }

            if (PH_DTO.RN_Date == null)
            {
                PH_DTO.RN_Date = DateTime.Now.ToString("dd-MMM-yy");
                PH_DTO.RN_No = OnReceiptNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            }
            else
            {
                PH_DTO.RN_No = OnReceiptNumber(Convert.ToInt32(Convert.ToDateTime(PH_DTO.RN_Date).ToString("yyyyMMdd")));
            }
            ReceiptGetData();

            return View(PH_DTO);
        }
        private T PurchaseClone<T>(T obj)
        {
            if (obj == null) return default(T);
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
      
        #endregion

        #region Create Receipt note old

        void OnReceiptNoteNumberGen(Int32 RNDate)
        {
            DataSet DS1 = new DataSet();

            PON_DTO.RNN_Date = RNDate.ToString();
            PON_DTO.Id = 101;

            DS1 = PON_DAO.RNNumberDB(PON_DTO);

            if (DS1.Tables[0].Rows.Count > 0)
            {
                Int32 Order = Convert.ToInt32(DS1.Tables[0].Rows[0]["RNN_Method"].ToString());

                DateTime Receipt_Date = Convert.ToDateTime(
                    DateTime.ParseExact(
                        RNDate.ToString(),
                        "yyyyMMdd",
                        CultureInfo.InvariantCulture
                    )
                );

                DateTime StartDate = new DateTime();
                DateTime EndDate = new DateTime();

                if (Order == 2)
                {
                    // Already Running Number Exists
                    if (DS1.Tables[1].Rows.Count > 0)
                    {
                        Int32 Number = Convert.ToInt32(
                            DS1.Tables[1].Rows[0]["StartingNumber"].ToString()
                        );

                        PON_DTO.RNN_Number = Convert.ToInt32(
                            DS1.Tables[1].Rows[0]["Number"].ToString()
                        );

                        PON_DTO.RNN_StartingNumber = Convert.ToString(
                            Convert.ToInt32(Number + 1)
                        );

                        PON_DTO.Id = 103;

                        PON_DAO.RNNumberDB(PON_DTO);
                    }
                    else
                    {
                        Int32 Frequency = 0;
                        Int32 Start = 0;

                        DateTime Date = new DateTime();

                        if (DS1.Tables[2].Rows.Count > 0)
                        {
                            Date = Convert.ToDateTime(
                                DS1.Tables[2].Rows[0]["RNR_Date"].ToString()
                            );

                            Start = Convert.ToInt32(
                                DS1.Tables[2].Rows[0]["RNR_StartingNumber"].ToString()
                            );

                            Frequency = Convert.ToInt32(
                                DS1.Tables[2].Rows[0]["RNR_Frequency"].ToString()
                            );
                        }

                        // Monthly
                        if (Frequency == 4)
                        {
                            if (Date.Month == Receipt_Date.Month)
                            {
                                StartDate = Date;

                                EndDate = new DateTime(
                                    Date.Year,
                                    Date.Month,
                                    1
                                ).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(
                                    Receipt_Date.Year,
                                    Receipt_Date.Month,
                                    1
                                );

                                EndDate = new DateTime(
                                    Receipt_Date.Year,
                                    Receipt_Date.Month,
                                    1
                                ).AddMonths(1).AddDays(-1);
                            }
                        }

                        // Yearly
                        else if (Frequency == 5)
                        {
                            if (
                                Date.Month == Receipt_Date.Month &&
                                Date.Year == Receipt_Date.Year
                            )
                            {
                                StartDate = Date;

                                EndDate = new DateTime(
                                    Date.Year,
                                    Date.Month,
                                    1
                                ).AddMonths(12).AddDays(-1);
                            }
                            else if (
                                Date.Month != Receipt_Date.Month &&
                                Date.Year == Receipt_Date.Year
                            )
                            {
                                StartDate = Date;

                                EndDate = new DateTime(
                                    Date.Year,
                                    Date.Month,
                                    1
                                ).AddMonths(12).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(
                                    Receipt_Date.Year,
                                    Date.Month,
                                    1
                                );

                                EndDate = new DateTime(
                                    Receipt_Date.Year,
                                    Date.Month,
                                    1
                                ).AddMonths(12).AddDays(-1);
                            }
                        }

                        PON_DTO.RNN_Number = Convert.ToInt32(
                            DS1.Tables[2].Rows[0]["RNR_Number"].ToString()
                        );

                        PON_DTO.RNN_StartingNumber = Convert.ToString(Start);

                        PON_DTO.RNN_Date = Convert.ToString(
                            StartDate.ToString("yyyyMMdd")
                        );

                        PON_DTO.RNN_Method = Convert.ToString(
                            EndDate.ToString("yyyyMMdd")
                        );

                        PON_DTO.Id = 102;

                        PON_DAO.RNNumberDB(PON_DTO);
                    }
                }
            }
        }
        //Sale Order Create
        [Route("receiptnote/transactions/sale-invoice/create")]
        public IActionResult CreateReceiptNote()
        {
            //SITempClear("11");
            ReceiptNoteHead_DTO SH_DTO = new ReceiptNoteHead_DTO();
            if (TempData["SH_DTO_Json"] is string SHto)
            {
                SH_DTO = System.Text.Json.JsonSerializer.Deserialize<ReceiptNoteHead_DTO>(SHto);
            }
            SH_DTO.JW_CustomerDC_Date = DateTime.Now.ToString("dd-MMM-yy");
          //  SH_DTO.RN_No = OnReceiptNoteNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            ReceiptGetData();
            return View(SH_DTO);
        }



        void ReceiptGetData()
        {
            SI_DTO.JIRNH_RN_Date = DateTime.Now;
            SI_DTO.JIRN_Id = 1;
            SI_DTO.JIRN_CreatorCode = Convert.ToInt64(1);
            DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);

            //ViewBag.IncomeCode = Help.GetCat(DS.Tables[0]);
            //ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            //ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            //ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            //ViewBag.Currency = Help.GetCat(DS.Tables[4]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[0]);
            ViewBag.UoM = Help.GetCat(DS.Tables[1]);
            //ViewBag.LedgerAccount = Help.GetCat(DS.Tables[7]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[2]);
            ViewBag.PRS = Help.GetCat(DS.Tables[3]);
            //ViewBag.HSN = Help.GetCat(DS.Tables[9]);
            //ViewBag.WHTax = Help.GetCat(DS.Tables[10]);
            //ViewBag.IsCalculate = Help.GetCat(DS.Tables[11]);
            //ViewBag.AddressType = Help.GetCat(DS.Tables[12]);
        }

        [HttpPost]
        [Route("receiptnote/transactions/sale-invoice/create")]
        public IActionResult CreateReceiptNote(ReceiptNoteHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            ReceiptNoteHead_DTO S_Head_DTO = new ReceiptNoteHead_DTO();

            List<ReceiptNoteItem_DTO>? ITM_DTO = new List<ReceiptNoteItem_DTO>();
            List<ReceiptNoteBatch_DTO>? BCH_DTO = new List<ReceiptNoteBatch_DTO>();
            S_Head_DTO = S_DTO;

            if (S_DTO.Items != null)
                ITM_DTO = S_DTO.Items!.Where(K => K.IsDeleted == "0").ToList();

            if (S_DTO.Items != null)
                BCH_DTO = S_DTO.ItemBatch!.Where(K => K.RNI_BCH_IsDeleted == "false").ToList();

            SI_DTO.JIRN_CreatorCode = Convert.ToInt64(1);
            // SI_DTO.JIRN_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Save")
            {
                //var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.Item_Number) != Convert.ToInt64(S_DTO.MS_Number));
                ITM_DTO = ITM_DTO
    .Where(x => !string.IsNullOrWhiteSpace(x.Item_Number))
    .ToList();
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.Qty) == 0 || Convert.ToDouble(x.UnitPrice) == 0 || Convert.ToDouble(x.Amount) == 0);

                //if (CheckItem.ToList().Count > 0)
                //{
                //    ViewBag.ErrorCode = 2;
                //    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                //}
                //else
                if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.JWC_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.Items = ITM_DTO;

                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        // if (BatchValidation(ITM_DTO))
                        {
                            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
      new TransactionOptions
      {
          IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
      }))

                            {
                                try
                                {
                                    string SIHOrderNoOld = S_DTO.RN_No;

                                    String SIHOrderNoNew = OnReceiptNoteNumber(Convert.ToInt32(Convert.ToDateTime(S_DTO.RN_Date).ToString("yyyyMMdd")));

                                    // =========================
                                    // 🔹 HEADER INSERT
                                    // =========================
                                    SI_DTO.JIRNH_RN_Date = Convert.ToDateTime(S_DTO.RN_Date);
                                    SI_DTO.JIRNH_RN_No = SIHOrderNoOld;
                                    SI_DTO.JIRNH_JWC_Number = Convert.ToInt64(S_DTO.JWC_Number);
                                    SI_DTO.JIRNH_Currency_Number = Convert.ToInt64(S_DTO.Currency_Number);
                                    SI_DTO.JIRNH_JW_CustomerDC_No = Convert.ToString(S_DTO.JW_CustomerDC_No);
                                    SI_DTO.JIRNH_JW_CustomerDC_Date = Convert.ToDateTime(S_DTO.JW_CustomerDC_Date);
                                    SI_DTO.JIRNH_MS_Number = Convert.ToInt64(S_DTO.MS_Number);
                                    SI_DTO.JIRNH_Remarks = Convert.ToString(S_DTO.Remarks);
                                    SI_DTO.JIRNH_WH_Number = Convert.ToInt64(S_DTO.WH_Number);
                                    SI_DTO.JIRN_Id = 21;

                                    DS = SI_DAO.JI_ReceiptNoteDB(SI_DTO);



                                    if (DS == null || DS.Tables.Count == 0 || DS.Tables[0].Rows.Count == 0)
                                        throw new Exception("Header insert failed");

                                    long headerId = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                    // =========================
                                    // 🔹 ITEM LOOP
                                    // =========================
                                    foreach (var Item in ITM_DTO)
                                    {
                                        var itemDTO = new ReceiptNote_DTO(); // 🔥 IMPORTANT FIX (NO REUSE)

                                        itemDTO.JIRNH_Number = headerId;
                                        itemDTO.JIRNI_Item_Number = Convert.ToInt64(Item.Item_Number);
                                        itemDTO.JIRNI_WH_Number = Convert.ToInt64(Item.WH_Number);
                                        itemDTO.JIRNI_UoM_Number = Convert.ToInt64(Item.UoM_Number);
                                        itemDTO.JIRNI_Qty = Convert.ToDouble(Item.Qty);
                                        itemDTO.JIRNI_UnitPrice = Convert.ToDouble(Item.UnitPrice);
                                        itemDTO.JIRNI_Amount = Convert.ToDouble(Item.Amount);
                                        itemDTO.JIRNI_PRS_Number = Convert.ToInt64(Item.PRS_Number);
                                        itemDTO.JIRN_Id = 22;

                                        var itemResult = SI_DAO.JI_ReceiptNoteDB(itemDTO);

                                        if (itemResult == null || itemResult.Tables.Count == 0 || itemResult.Tables[0].Rows.Count == 0)
                                            throw new Exception("Item insert failed");

                                        long itemID = Convert.ToInt64(itemResult.Tables[0].Rows[0][0]);

                                        // =========================
                                        // 🔹 BATCH INSERT (FIXED)
                                        // =========================

                                        var relatedBatches = BCH_DTO
                                            .Where(x => x.RNI_BCH_Item_Number == Item.Item_Number)
                                            .ToList();

                                        foreach (var batch in relatedBatches)
                                        {
                                            var batchDTO = new ReceiptNote_DTO(); // 🔥 NEW OBJECT

                                            batchDTO.JIRNI_BCH_Number = batch.RNI_BCH_No;
                                            batchDTO.JIRNI_Item_Number = Convert.ToInt64(Item.Item_Number);
                                            batchDTO.JIRNH_Number = headerId;
                                            batchDTO.JIRNI_Number = itemID;
                                            batchDTO.JIRNI_BCH_BatchDate = DateTime.Now;
                                            //   batchDTO.JIRNI_BCH_BatchNo = "AUTO-" + headerId;
                                            batchDTO.JIRNI_BCH_BatchNo = batch.RNI_BCH_Number;
                                            batchDTO.JIRNI_BCH_WH_Number = Convert.ToInt64(Item.WH_Number);
                                            batchDTO.JIRNI_BCH_BatchQty = Convert.ToDouble(batch.RNI_BCH_Qty);
                                            batchDTO.JIRNI_BCH_BatchUnitPrice = Convert.ToDouble(batch.RNI_BCH_UnitPrice);
                                            batchDTO.JIRNI_BCH_BatchValue = Convert.ToDouble(batch.RNI_BCH_Value);
                                            batchDTO.JIRNH_WH_Number = Convert.ToInt64(S_DTO.WH_Number);
                                            batchDTO.JIRNI_WH_Number = Convert.ToInt64(Item.WH_Number);
                                            batchDTO.JIRN_Id = 23;

                                            SI_DAO.JI_ReceiptNoteDB(batchDTO);
                                        }
                                    }

                                    // =========================
                                    // 🔹 COMMIT
                                    // =========================
                                    transaction.Complete();

                                    // =========================
                                    // 🔹 CLEANUP
                                    // =========================
                                    S_Head_DTO.Reset();
                                    ITM_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    if (SIHOrderNoOld != SIHOrderNoNew)
                                    {
                                        ViewBag.ErrorCode = 2;
                                        ViewBag.ErrorMessage =
                                            "Receipt Note number " + SIHOrderNoOld + " changed to " + SIHOrderNoNew;
                                    }

                                    return RedirectToAction("CreateReceiptNote");
                                }
                                catch (Exception ex)
                                {
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                    throw;
                                }
                            }
                        }

                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;
                    }
                }
            }


            ReceiptGetData();
            return View(Original_DTO);
        }


        #endregion


    }
}

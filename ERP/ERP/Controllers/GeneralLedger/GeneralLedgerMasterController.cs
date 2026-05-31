using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Data.SqlClient;
using System.Security.Policy;

namespace ERP.Controllers.GeneralLedger
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class GeneralLedgerMasterController : Controller
    {
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();
        GeneralLedgerMaster_DL C_DL = new GeneralLedgerMaster_DL();

        Currency_DAO C_DAO = new Currency_DAO();
        Currency_DTO C_DTO = new Currency_DTO();
        List<Currency_DTO> C_List = new List<Currency_DTO>();

        COA_Group_DAO COA_DAO = new COA_Group_DAO();
        COA_Group_DTO COA_DTO = new COA_Group_DTO();
        List<COA_Group_DTO> COA_List = new List<COA_Group_DTO>();

        COA_Ledger_DAO CL_DAO = new COA_Ledger_DAO();
        COA_Ledger_DTO CL_DTO = new COA_Ledger_DTO();
        List<COA_Ledger_DTO> CL_List = new List<COA_Ledger_DTO>();

        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;


        //Currency
        [Route("general-ledger/master/currency")]
        public IActionResult Currency(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<Currency_DTO> C_List = CGetData(SortOrder, Search, PageNumber, PSize, PageFilter, C_DTO.CurrencyLocation);
            return View(PaginatedList<Currency_DTO>.CreateAsync(C_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("general-ledger/master/currency")]
        [HttpPost]
        public IActionResult Currency(Currency_DTO C_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            C_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    C_DTO.Id = 6;
                    DS = C_DAO.CurrencyDB(C_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        C_DTO.Id = 1;
                        C_DAO.CurrencyDB(C_DTO);

                        C_DTO.Reset();
                        ModelState.Clear();
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
            else if (Mode == "DeleteAll")
            {
                try
                {
                    C_DTO.DeleteNumbers = DeleteNumbers;
                    C_DTO.Id = 3;
                    C_DAO.CurrencyDB(C_DTO);

                    C_DTO.Reset();
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                }
            }
            else if (Mode == "Delete")
            {
                try
                {
                    C_DTO.CurrencyNumber = Number;
                    C_DTO.Id = 8;
                    C_DAO.CurrencyDB(C_DTO);

                    C_DTO.Reset();
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                }
            }
            else if (Mode == "Clear")
            {
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                C_DTO.Id = 4;
                DS = C_DAO.CurrencyDB(C_DTO);
                C_DTO = C_DL.CList(DS.Tables[0]).FirstOrDefault();
                ViewBag.CurrencyEdit = C_DTO;
            }
            else if (Mode == "Update")
            {
                C_DTO.Id = 7;
                DS = C_DAO.CurrencyDB(C_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        C_DTO.Id = 5;
                        C_DAO.CurrencyDB(C_DTO);

                        C_DTO.Reset();
                        ModelState.Clear();
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


            List<Currency_DTO> C_List = CGetData(SortOrder, Search, PageNumber, PSize, PageFilter, C_DTO.CurrencyLocation);
            return View(PaginatedList<Currency_DTO>.CreateAsync(C_List, DPageNumber ?? 1, DPageSize));
        }
        List<Currency_DTO> CGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Location)
        {
            DPageSize = 10;

            C_DTO.Id = 2;
            C_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = C_DAO.CurrencyDB(C_DTO);
            C_List = C_DL.CList(DS.Tables[0]);
            if (Location != null)
            {
                ViewBag.Currency = C_DL.Currency(DS.Tables[1], Location);
            }
            else
            {
                ViewBag.Currency = C_DL.Currency(DS.Tables[1], "");
            }

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title";
            }
            if (Convert.ToInt32(PageNumber) == 0)
            {
                DPageNumber = 1;
            }
            if (PageFilter?.ToLower() == "PageFilter".ToLower())
            {
                DPageNumber = 1;
            }

            ViewData["CurrentSort"] = SortOrder;
            ViewData["KeySort"] = SortOrder == "Title" ? "Title_desc" : "Title";

            ViewData["CurrentFilter"] = Search;

            var Key = C_List.OrderByDescending(Cs => Cs.CurrencyNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.CurrencyCode!.ToLower().Contains(Search.ToLower()) || K.FormalName!.ToLower().Contains(Search.ToLower()) || K.Symbol!.ToLower().Contains(Search.ToLower()) || K.DecimalPortionName!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.CurrencyNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.CurrencyCode!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.CurrencyCode!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.CurrencyNumber);
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
                    DPageNumber = Convert.ToInt32(PageNumber) == 0 ? 1 : Convert.ToInt32(PageNumber);
                }
                else
                {
                    Int32 PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Record) / Convert.ToDouble(DPageSize)));

                    DPageNumber = PageNumber > PageCount ? Convert.ToInt32(PageCount) : Convert.ToInt32(PageNumber);
                }
            }
            else
            {
                DPageNumber = Convert.ToInt32(PageNumber) == 0 ? 1 : Convert.ToInt32(PageNumber);
            }

            Int32 PageCounts = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Record) / Convert.ToDouble(DPageSize)));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }


        [Route("general-ledger/master/currency/duplicate")]
        public Boolean CurrencyDuplicate(String? Title, String? Number)
        {
            C_DTO.CreatorCode = Convert.ToInt32(UserCode);
            C_DTO.CurrencyCode = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                C_DTO.Id = 6;
            }
            else
            {
                C_DTO.CurrencyNumber = Convert.ToInt32(Number);
                C_DTO.Id = 7;
            }
            DS = C_DAO.CurrencyDB(C_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //COA Group
        [Route("general-ledger/master/coa-group")]
        public IActionResult COAGroup(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<COA_Group_DTO> COA_List = COAGetData(SortOrder, Search, PageNumber, PSize, PageFilter, COA_DTO.LedgerGroupNumber);
            return View(PaginatedList<COA_Group_DTO>.CreateAsync(COA_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("general-ledger/master/coa-group")]
        [HttpPost]
        public IActionResult COAGroup(COA_Group_DTO COA_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            COA_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    COA_DTO.Id = 6;
                    DS = COA_DAO.COAGroupDB(COA_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        COA_DTO.Id = 1;
                        COA_DAO.COAGroupDB(COA_DTO);

                        COA_DTO.Reset();
                        ModelState.Clear();
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
            else if (Mode == "DeleteAll")
            {
                try
                {
                    COA_DTO.DeleteNumbers = DeleteNumbers;
                    COA_DTO.Id = 3;
                    COA_DAO.COAGroupDB(COA_DTO);

                    COA_DTO.Reset();
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                }
            }
            else if (Mode == "Delete")
            {
                try
                {
                    COA_DTO.LedgerGroupNumber = Number;
                    COA_DTO.Id = 8;
                    COA_DAO.COAGroupDB(COA_DTO);

                    COA_DTO.Reset();
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                }
            }
            else if (Mode == "Clear")
            {
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                COA_DTO.Id = 4;
                DS = COA_DAO.COAGroupDB(COA_DTO);
                COA_DTO = C_DL.CGList(DS.Tables[0]).FirstOrDefault();
                ViewBag.COAGroupEdit = COA_DTO;
            }
            else if (Mode == "Update")
            {
                COA_DTO.Id = 7;
                DS = COA_DAO.COAGroupDB(COA_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        COA_DTO.Id = 5;
                        COA_DAO.COAGroupDB(COA_DTO);

                        COA_DTO.Reset();
                        ModelState.Clear();
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

            List<COA_Group_DTO> COA_List = COAGetData(SortOrder, Search, PageNumber, PSize, PageFilter, COA_DTO.LedgerGroupNumber);
            return View(PaginatedList<COA_Group_DTO>.CreateAsync(COA_List, DPageNumber ?? 1, DPageSize));
        }
        List<COA_Group_DTO> COAGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;
            if (Number != null)
            {
                COA_DTO.LedgerGroupNumber = Convert.ToInt64(Number);
            }

            COA_DTO.Id = 2;
            COA_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = COA_DAO.COAGroupDB(COA_DTO);
            COA_List = C_DL.CGList(DS.Tables[0]);
            ViewBag.Under = Help.GetUnder(DS.Tables[1]);
            ViewBag.Nature = Help.GetCat(DS.Tables[2]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title";
            }
            if (Convert.ToInt32(PageNumber) == 0)
            {
                DPageNumber = 1;
            }
            if (PageFilter?.ToLower() == "PageFilter".ToLower())
            {
                DPageNumber = 1;
            }

            ViewData["CurrentSort"] = SortOrder;
            ViewData["KeySort"] = SortOrder == "Title" ? "Title_desc" : "Title";

            ViewData["CurrentFilter"] = Search;

            var Key = COA_List.OrderByDescending(Cs => Cs.LedgerGroupNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.LedgerGroup!.ToLower().Contains(Search.ToLower()) || K.GroupNature!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.LedgerGroupNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.LedgerGroup!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.LedgerGroup!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.LedgerGroupNumber);
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
                    DPageNumber = Convert.ToInt32(PageNumber) == 0 ? 1 : Convert.ToInt32(PageNumber);
                }
                else
                {
                    Int32 PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Record) / Convert.ToDouble(DPageSize)));

                    DPageNumber = PageNumber > PageCount ? Convert.ToInt32(PageCount) : Convert.ToInt32(PageNumber);
                }
            }
            else
            {
                DPageNumber = Convert.ToInt32(PageNumber) == 0 ? 1 : Convert.ToInt32(PageNumber);
            }

            Int32 PageCounts = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Record) / Convert.ToDouble(DPageSize)));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }


        [Route("general-ledger/master/coa-group/duplicate")]
        public Boolean COAGroupDuplicate(String? Title, String? Number)
        {
            COA_DTO.CreatorCode = Convert.ToInt32(UserCode);
            COA_DTO.LedgerGroup = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                COA_DTO.Id = 6;
            }
            else
            {
                COA_DTO.LedgerGroupNumber = Convert.ToInt32(Number);
                COA_DTO.Id = 7;
            }
            DS = COA_DAO.COAGroupDB(COA_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //COA Ledger
        [Route("general-ledger/master/coa-ledger")]
        public IActionResult COALedger(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<COA_Ledger_DTO> COA_List = CLGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<COA_Ledger_DTO>.CreateAsync(COA_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("general-ledger/master/coa-ledger")]
        [HttpPost]
        public IActionResult COALedger(COA_Ledger_DTO CL_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            CL_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    CL_DTO.Id = 6;
                    DS = CL_DAO.COALedgerDB(CL_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        CL_DTO.Id = 1;
                        CL_DAO.COALedgerDB(CL_DTO);

                        CL_DTO.Reset();
                        ModelState.Clear();
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
            else if (Mode == "DeleteAll")
            {
                try
                {
                    CL_DTO.DeleteNumbers = DeleteNumbers;
                    CL_DTO.Id = 3;
                    CL_DAO.COALedgerDB(CL_DTO);

                    CL_DTO.Reset();
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                }
            }
            else if (Mode == "Delete")
            {
                try
                {
                    CL_DTO.LedgerNumber = Number;
                    CL_DTO.Id = 8;
                    CL_DAO.COALedgerDB(CL_DTO);

                    CL_DTO.Reset();
                    ModelState.Clear();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                }
            }
            else if (Mode == "Clear")
            {
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                CL_DTO.Id = 4;
                DS = CL_DAO.COALedgerDB(CL_DTO);
                CL_DTO = C_DL.CLList(DS.Tables[0]).FirstOrDefault();
                ViewBag.COALedgerEdit = CL_DTO;
            }
            else if (Mode == "Update")
            {
                CL_DTO.Id = 7;
                DS = CL_DAO.COALedgerDB(CL_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        CL_DTO.Id = 5;
                        CL_DAO.COALedgerDB(CL_DTO);

                        CL_DTO.Reset();
                        ModelState.Clear();
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

            List<COA_Ledger_DTO> COA_List = CLGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<COA_Ledger_DTO>.CreateAsync(COA_List, DPageNumber ?? 1, DPageSize));
        }
        List<COA_Ledger_DTO> CLGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            CL_DTO.Id = 2;
            CL_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = CL_DAO.COALedgerDB(CL_DTO);
            CL_List = C_DL.CLList(DS.Tables[0]);
            ViewBag.Group = Help.GetCat(DS.Tables[1]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title";
            }
            if (Convert.ToInt32(PageNumber) == 0)
            {
                DPageNumber = 1;
            }
            if (PageFilter?.ToLower() == "PageFilter".ToLower())
            {
                DPageNumber = 1;
            }

            ViewData["CurrentSort"] = SortOrder;
            ViewData["KeySort"] = SortOrder == "Title" ? "Title_desc" : "Title";

            ViewData["CurrentFilter"] = Search;

            var Key = CL_List.OrderByDescending(Cs => Cs.LedgerNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.LedgerAccount!.ToLower().Contains(Search.ToLower()) || K.LedgerName!.ToLower().Contains(Search.ToLower()) || K.LedgerGroup!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.LedgerNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.LedgerAccount!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.LedgerAccount!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.LedgerNumber);
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
                    DPageNumber = Convert.ToInt32(PageNumber) == 0 ? 1 : Convert.ToInt32(PageNumber);
                }
                else
                {
                    Int32 PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Record) / Convert.ToDouble(DPageSize)));

                    DPageNumber = PageNumber > PageCount ? Convert.ToInt32(PageCount) : Convert.ToInt32(PageNumber);
                }
            }
            else
            {
                DPageNumber = Convert.ToInt32(PageNumber) == 0 ? 1 : Convert.ToInt32(PageNumber);
            }

            Int32 PageCounts = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Record) / Convert.ToDouble(DPageSize)));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }


        [Route("general-ledger/master/coa-ledger/duplicate")]
        public Boolean COALedgerDuplicate(String? Title, String? Number)
        {
            CL_DTO.CreatorCode = Convert.ToInt32(UserCode);
            CL_DTO.LedgerAccount = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                CL_DTO.Id = 6;
            }
            else
            {
                CL_DTO.LedgerNumber = Convert.ToInt32(Number);
                CL_DTO.Id = 7;
            }
            DS = CL_DAO.COALedgerDB(CL_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
using ERP.Models;
using ERP_DAO;
using ERP_DL;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Transactions;

namespace ERP.Controllers.Taxation
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class TaxationMasterController : Controller
    {
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();

        List<SAC_DTO> S_List = new List<SAC_DTO>();
        TaxationMaster_DL TM_DL = new TaxationMaster_DL();
        SAC_DAO S_DAO = new SAC_DAO();
        SAC_DTO S_DTO = new SAC_DTO();

        List<HSN_DTO> H_List = new List<HSN_DTO>();
        HSN_DAO H_DAO = new HSN_DAO();
        HSN_DTO H_DTO = new HSN_DTO();

        List<WithholdTax_DTO> WH_List = new List<WithholdTax_DTO>();
        WithholdTax_DAO WH_DAO = new WithholdTax_DAO();
        WithholdTax_DTO WH_DTO = new WithholdTax_DTO();

        List<WH_TaxAssign_DTO> WHA_List = new List<WH_TaxAssign_DTO>();
        WH_TaxAssign_DAO WHA_DAO = new WH_TaxAssign_DAO();
        WH_TaxAssign_DTO WHA_DTO = new WH_TaxAssign_DTO();

        List<TaxCluster_DTO> TC_List = new List<TaxCluster_DTO>();
        TaxCluster_DAO TC_DAO = new TaxCluster_DAO();
        TaxCluster_DTO TC_DTO = new TaxCluster_DTO();

        List<TaxElement_DTO> TE_List = new List<TaxElement_DTO>();
        TaxElement_DAO TE_DAO = new TaxElement_DAO();
        TaxElement_DTO TE_DTO = new TaxElement_DTO();

        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;



        //SAC
        [Route("taxation/master/sac")]
        public IActionResult SAC(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                List<SAC_DTO> TC_List = SACGetData(SortOrder, Search, PageNumber, PSize, PageFilter);

                return View(PaginatedList<SAC_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/master/sac")]
        [HttpPost]
        public IActionResult SAC(SAC_DTO S_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                try
                {
                    S_DTO.CreatorCode = Convert.ToInt32(UserCode);
                    if (Mode == "Save")
                    {
                        if (ModelState.IsValid)
                        {
                            S_DTO.Id = 6;
                            DS = S_DAO.SACDB(S_DTO);
                            if (DS.Tables[0].Rows.Count == 0)
                            {
                                S_DTO.Id = 1;
                                S_DAO.SACDB(S_DTO);

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
                            S_DTO.DeleteNumbers = DeleteNumbers;
                            S_DTO.Id = 3;
                            S_DAO.SACDB(S_DTO);

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
                            S_DTO.SAC_Number = Convert.ToInt64(Number);
                            S_DTO.Id = 8;
                            S_DAO.SACDB(S_DTO);

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
                        S_DTO.Id = 4;
                        DS = S_DAO.SACDB(S_DTO);
                        ViewBag.SACEdit = TM_DL.SACList(DS.Tables[0]).FirstOrDefault(); ;
                    }
                    else if (Mode == "Update")
                    {
                        S_DTO.Id = 7;
                        DS = S_DAO.SACDB(S_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            if (ModelState.IsValid)
                            {
                                S_DTO.Id = 5;
                                S_DAO.SACDB(S_DTO);

                                ModelState.Clear();
                            }
                            else
                            {
                                var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                                string CombinedString = string.Join("<br/>", Errors);

                                ViewBag.ErrorCode = 2;
                                ViewBag.ErrorMessage = CombinedString;

                                S_DTO.Id = 4;
                                DS = S_DAO.SACDB(S_DTO);
                                ViewBag.SACEdit = TM_DL.SACList(DS.Tables[0]).FirstOrDefault(); ;
                            }
                        }
                    }

                    List<SAC_DTO> TC_List = SACGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                    return View(PaginatedList<SAC_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                    return View();
                }
        }
        List<SAC_DTO> SACGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            S_DTO.Id = 2;
            S_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = S_DAO.SACDB(S_DTO);
            S_List = TM_DL.SACList(DS.Tables[0]);

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

            var Key = S_List.OrderByDescending(Cs => Cs.SAC_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.SAC_Code!.ToLower().Contains(Search.ToLower()) || K.Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SAC_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.SAC_Code!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.SAC_Code!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.SAC_Number);
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


        [Route("taxation/master/sac/duplicate")]
        public Boolean SACDuplicate(String? Title, String? Number)
        {
            S_DTO.CreatorCode = Convert.ToInt32(UserCode);
            S_DTO.SAC_Code = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                S_DTO.Id = 6;
            }
            else
            {
                S_DTO.SAC_Number = Convert.ToInt32(Number);
                S_DTO.Id = 7;
            }
            DS = S_DAO.SACDB(S_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        //HSN
        [Route("taxation/master/hsn")]
        public IActionResult HSN(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                List<HSN_DTO> HSN_List = HSNGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<HSN_DTO>.CreateAsync(HSN_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/master/hsn")]
        [HttpPost]
        public IActionResult HSN(HSN_DTO H_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                try
                {
                    H_DTO.CreatorCode = Convert.ToInt32(UserCode);
                    if (Mode == "Save")
                    {
                        if (ModelState.IsValid)
                        {
                            H_DTO.Id = 6;
                            DS = H_DAO.HSNDB(H_DTO);
                            if (DS.Tables[0].Rows.Count == 0)
                            {
                                H_DTO.Id = 1;
                                H_DAO.HSNDB(H_DTO);

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
                            H_DTO.DeleteNumbers = DeleteNumbers;
                            H_DTO.Id = 3;
                            H_DAO.HSNDB(H_DTO);

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
                            H_DTO.HSN_Number = Convert.ToInt64(Number);
                            H_DTO.Id = 8;
                            H_DAO.HSNDB(H_DTO);

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
                        H_DTO.Id = 4;
                        DS = H_DAO.HSNDB(H_DTO);
                        ViewBag.HSNEdit = TM_DL.HSNList(DS.Tables[0]).FirstOrDefault();
                    }
                    else if (Mode == "Update")
                    {
                        H_DTO.Id = 7;
                        DS = H_DAO.HSNDB(H_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            if (ModelState.IsValid)
                            {
                                H_DTO.Id = 5;
                                H_DAO.HSNDB(H_DTO);

                                ModelState.Clear();
                            }
                            else
                            {
                                var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                                string CombinedString = string.Join("<br/>", Errors);

                                ViewBag.ErrorCode = 2;
                                ViewBag.ErrorMessage = CombinedString;

                                H_DTO.Id = 4;
                                DS = H_DAO.HSNDB(H_DTO);
                                ViewBag.HSNEdit = TM_DL.HSNList(DS.Tables[0]).FirstOrDefault();
                            }
                        }
                    }

                    List<HSN_DTO> HSN_List = HSNGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                    return View(PaginatedList<HSN_DTO>.CreateAsync(HSN_List, DPageNumber ?? 1, DPageSize));
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                    return View();
                }
        }
        List<HSN_DTO> HSNGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            H_DTO.Id = 2;
            H_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = H_DAO.HSNDB(H_DTO);
            H_List = TM_DL.HSNList(DS.Tables[0]);

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

            var Key = H_List.OrderByDescending(Cs => Cs.HSN_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.HSN_Code!.ToLower().Contains(Search.ToLower()) || K.Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.HSN_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.HSN_Code!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.HSN_Code!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.HSN_Number);
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


        [Route("taxation/master/hsn/duplicate")]
        public Boolean HSNDuplicate(String? Title, String? Number)
        {
            H_DTO.CreatorCode = Convert.ToInt32(UserCode);
            H_DTO.HSN_Code = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                H_DTO.Id = 6;
            }
            else
            {
                H_DTO.HSN_Number = Convert.ToInt32(Number);
                H_DTO.Id = 7;
            }
            DS = H_DAO.HSNDB(H_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        //With Hold Tax
        [Route("taxation/master/with-hold-tax")]
        public IActionResult WithholdTax(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                List<WithholdTax_DTO> WH_List = WithholdTaxGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<WithholdTax_DTO>.CreateAsync(WH_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/master/with-hold-tax")]
        [HttpPost]
        public IActionResult WithholdTax(WithholdTax_DTO WH_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                    WH_DTO.CreatorCode = Convert.ToInt32(UserCode);
                    if (Mode == "Save")
                    {
                        if (ModelState.IsValid)
                        {
                            WH_DTO.Id = 6;
                            DS = WH_DAO.WithholdTaxDB(WH_DTO);
                            if (DS.Tables[0].Rows.Count == 0)
                            {
                                WH_DTO.Id = 1;
                                WH_DAO.WithholdTaxDB(WH_DTO);

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
                            WH_DTO.DeleteNumbers = DeleteNumbers;
                            WH_DTO.Id = 3;
                            WH_DAO.WithholdTaxDB(WH_DTO);

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
                            WH_DTO.WH_Number = Convert.ToInt64(Number);
                            WH_DTO.Id = 8;
                            WH_DAO.WithholdTaxDB(WH_DTO);

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
                        WH_DTO.Id = 4;
                        DS = WH_DAO.WithholdTaxDB(WH_DTO);
                        ViewBag.WithholdTaxEdit = TM_DL.WHList(DS.Tables[0]).FirstOrDefault();
                    }
                    else if (Mode == "Update")
                    {
                        WH_DTO.Id = 7;
                        DS = WH_DAO.WithholdTaxDB(WH_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            if (ModelState.IsValid)
                            {
                                WH_DTO.Id = 5;
                                WH_DAO.WithholdTaxDB(WH_DTO);

                                ModelState.Clear();
                            }
                            else
                            {
                                var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                                string CombinedString = string.Join("<br/>", Errors);

                                ViewBag.ErrorCode = 2;
                                ViewBag.ErrorMessage = CombinedString;

                            WH_DTO.Id = 4;
                            DS = WH_DAO.WithholdTaxDB(WH_DTO);
                            ViewBag.WithholdTaxEdit = TM_DL.WHList(DS.Tables[0]).FirstOrDefault();
                        }
                        }
                    }

                    List<WithholdTax_DTO> WH_List = WithholdTaxGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                    return View(PaginatedList<WithholdTax_DTO>.CreateAsync(WH_List, DPageNumber ?? 1, DPageSize));
               
        }
        List<WithholdTax_DTO> WithholdTaxGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            WH_DTO.Id = 2;
            WH_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = WH_DAO.WithholdTaxDB(WH_DTO);
            WH_List = TM_DL.WHList(DS.Tables[0]);
            ViewBag.Category = Help.GetCat(DS.Tables[1]);
            ViewBag.Type = Help.GetCat(DS.Tables[2]);
            ViewBag.Impact = Help.GetCat(DS.Tables[3]);

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

            var Key = WH_List.OrderByDescending(Cs => Cs.WH_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.WH_TaxCode!.ToLower().Contains(Search.ToLower()) || K.WH_TaxDescription!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.WH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.WH_TaxCode!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.WH_TaxCode!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.WH_Number);
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


        [Route("taxation/master/with-hold-tax/duplicate")]
        public Boolean WithholdTaxDuplicate(String? Title, String? Number)
        {
            WH_DTO.CreatorCode = Convert.ToInt32(UserCode);
            WH_DTO.WH_TaxCode = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                WH_DTO.Id = 6;
            }
            else
            {
                WH_DTO.WH_Number = Convert.ToInt32(Number);
                WH_DTO.Id = 7;
            }
            DS = WH_DAO.WithholdTaxDB(WH_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        //WH Tax Assign
        [Route("taxation/master/tax-assign")]
        public IActionResult WHTaxAssign(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                List<WH_TaxAssign_DTO> WHA_List = WHTaxAssignGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                var PageList = PaginatedList_DTO<WH_TaxAssign_DTO>.CreateAsync(WHA_List, DPageNumber ?? 1, DPageSize);

                var Model = new WH_TaxAssignHead_DTO()
                {
                    WH_TaxAssign_List = PageList,
                };
                return View(Model);
                //return View(PaginatedList<WH_TaxAssign_DTO>.CreateAsync(WHA_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/master/tax-assign")]
        [HttpPost]
        public IActionResult WHTaxAssign(WH_TaxAssignHead_DTO WHHA_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                //try
                //{
                bool IsValid = false;
                WH_TaxAssignHead_DTO WH_TaxAHead_DTO = new WH_TaxAssignHead_DTO();

                List<WH_TaxAssignDetail_DTO>? WHDA_DTO = WHHA_DTO.WH_TaxDetail!.Where(K => !K.IsDeleted).ToList();
                WH_TaxAHead_DTO = WHHA_DTO;
                WH_TaxAHead_DTO.WH_TaxDetail = WHDA_DTO;

                WHA_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (WHDA_DTO.Count > 0)
                    {
                        ModelState.Clear();
                        IsValid = TryValidateModel(WH_TaxAHead_DTO);

                        if (IsValid)
                        {
                            using (var transaction = new TransactionScope())
                            {
                                WHA_DTO.Id = 1;
                                DS = WHA_DAO.WHTaxAssignDB(WHA_DTO);
                                if (DS.Tables[0].Rows.Count == 0)
                                {
                                    WHA_DTO.WH_TaxCode = WHHA_DTO.WH_TaxCode;
                                    WHA_DTO.WH_TaxDescription = WHHA_DTO.WH_TaxDescription;
                                    WHA_DTO.Id = 2;
                                    DS = WHA_DAO.WHTaxAssignDB(WHA_DTO);

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                            WHA_DTO.WH_TaxNumber = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                            foreach (var detail in WHDA_DTO)
                                            {
                                                WHA_DTO.AssesseeNature = Convert.ToInt64(detail.AssesseeNature);
                                                WHA_DTO.FromDate = Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd");
                                                WHA_DTO.ToDate = Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd");
                                                WHA_DTO.SingleTransLimit = Convert.ToDouble(detail.SingleTransLimit);
                                                WHA_DTO.AggregateTransLimit = Convert.ToDouble(detail.AggregateTransLimit);
                                                WHA_DTO.IncludeTax = Convert.ToInt64(detail.IncludeTax);
                                                WHA_DTO.PAN_TaxPercent = Convert.ToDouble(detail.PAN_TaxPercent);
                                                WHA_DTO.NON_PAN_TaxPercent = Convert.ToDouble(detail.NON_PAN_TaxPercent);
                                                WHA_DTO.COA = Convert.ToInt64(detail.COA);
                                                WHA_DTO.Id = 3;
                                                WHA_DAO.WHTaxAssignDB(WHA_DTO);
                                            }
                                            transaction.Complete();

                                        WHHA_DTO.Reset();
                                        WHDA_DTO = WHHA_DTO.WH_TaxDetail!;
                                        WHA_DTO.Reset();
                                        ModelState.Clear();
                                    }
                                    else
                                    {
                                        transaction.Dispose();
                                        ViewBag.ErrorMessage = "Failed to insert tax details";
                                        ViewBag.ErrorCode = 2;
                                    }
                                }
                                else
                                {
                                    transaction.Dispose(); // Rollback
                                    ViewBag.ErrorMessage = "WH Tax is already assigned. Please check";
                                    ViewBag.ErrorCode = 2;
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
                    else
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = "Atleast one Row Required";
                    }
                }
                else if (Mode == "DeleteAll")
                {
                    try
                    {
                        WHA_DTO.DeleteNumbers = DeleteNumbers;
                        WHA_DTO.Id = 5;
                        WHA_DAO.WHTaxAssignDB(WHA_DTO);

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
                        WHA_DTO.WH_TaxNumber = Convert.ToInt64(Number);
                        WHA_DTO.Id = 6;
                        WHA_DAO.WHTaxAssignDB(WHA_DTO);

                        WHA_DTO.Reset();
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
                    WHA_DTO.Reset();
                    WHHA_DTO.Reset();
                    WHDA_DTO = WHHA_DTO.WH_TaxDetail;
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    WHA_DTO.WH_TaxNumber = Convert.ToInt64(WHHA_DTO.WH_TaxNumber);
                    WHA_DTO.Id = 11;
                    DS = WHA_DAO.WHTaxAssignDB(WHA_DTO);
                    WH_TaxAHead_DTO = TM_DL.WHEList(DS.Tables[0]).FirstOrDefault();
                    WHDA_DTO = TM_DL.WHDList(DS.Tables[1]);

                }
                else if (Mode == "Update")
                {
                    if (WHDA_DTO.Count > 0)
                    {
                        ModelState.Clear();
                        IsValid = TryValidateModel(WH_TaxAHead_DTO);
                        if (IsValid)
                        {
                            using (var transaction = new TransactionScope()) //Use a transaction to ensure all operations succeed or fail together
                            {
                                try
                                {
                                    String Check = string.Join(", ", WHDA_DTO.Where(x => Convert.ToInt64(x.WH_TaxAssignNumber) != 0).Select(x => x.WH_TaxAssignNumber));
                                    WHA_DTO.WH_TaxNumber = WHHA_DTO.WH_TaxNumber;
                                    WHA_DTO.DeleteNumbers = Check;
                                    WHA_DTO.Id = 14;
                                    DS = WHA_DAO.WHTaxAssignDB(WHA_DTO);
                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                                        {
                                            WHA_DTO.WH_TaxNumber = WHHA_DTO.WH_TaxNumber;
                                            WHA_DTO.WH_TaxAssignNumber = Convert.ToInt64(DS.Tables[0].Rows[0][0]);
                                            WHA_DTO.Id = 15;
                                            WHA_DAO.WHTaxAssignDB(WHA_DTO);
                                        }
                                    }

                                    WHA_DTO.WH_TaxNumber = WHHA_DTO.WH_TaxNumber;
                                    WHA_DTO.WH_TaxCode = WHHA_DTO.WH_TaxCode;
                                    WHA_DTO.WH_TaxDescription = WHHA_DTO.WH_TaxDescription;
                                    WHA_DTO.Id = 12;
                                    WHA_DAO.WHTaxAssignDB(WHA_DTO);

                                    foreach (var detail in WHDA_DTO)
                                    {
                                        WHA_DTO.WH_TaxNumber = Convert.ToInt64(WHHA_DTO.WH_TaxNumber);
                                        WHA_DTO.AssesseeNature = Convert.ToInt64(detail.AssesseeNature);
                                        WHA_DTO.FromDate = Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd");
                                        WHA_DTO.ToDate = Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd");
                                        WHA_DTO.SingleTransLimit = Convert.ToDouble(detail.SingleTransLimit);
                                        WHA_DTO.AggregateTransLimit = Convert.ToDouble(detail.AggregateTransLimit);
                                        WHA_DTO.IncludeTax = Convert.ToInt64(detail.IncludeTax);
                                        WHA_DTO.PAN_TaxPercent = Convert.ToDouble(detail.PAN_TaxPercent);
                                        WHA_DTO.NON_PAN_TaxPercent = Convert.ToDouble(detail.NON_PAN_TaxPercent);
                                        WHA_DTO.COA = Convert.ToInt64(detail.COA);

                                        // Is it an existing record or a new record? (check WH_TaxAssignNumber)
                                        if (detail.WH_TaxAssignNumber == 0)
                                        {
                                            WHA_DTO.Id = 3; // New record
                                        }
                                        else
                                        {
                                            WHA_DTO.WH_TaxAssignNumber = detail.WH_TaxAssignNumber;
                                            WHA_DTO.Id = 13;  // Existing record
                                        }
                                        WHA_DAO.WHTaxAssignDB(WHA_DTO);
                                    }
                                    transaction.Complete(); // mark the transaction as complete
                                    
                                    WHHA_DTO.Reset();
                                    WHDA_DTO = WHHA_DTO.WH_TaxDetail!;
                                    WHA_DTO.Reset();
                                    ModelState.Clear();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                }
                            }
                        }
                        else
                        {
                            var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                            string CombinedString = string.Join("<br/>", Errors);

                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = CombinedString;

                            WHA_DTO.WH_TaxNumber = Convert.ToInt64(WHHA_DTO.WH_TaxNumber);
                            WHA_DTO.Id = 11;
                            DS = WHA_DAO.WHTaxAssignDB(WHA_DTO);
                            WH_TaxAHead_DTO = TM_DL.WHEList(DS.Tables[0]).FirstOrDefault();
                            if (WHDA_DTO.Count < 1)
                                WHDA_DTO = TM_DL.WHDList(DS.Tables[1]);
                        }
                    }
                    else
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = "Atleast one Row Required";

                        WHA_DTO.WH_TaxNumber = Convert.ToInt64(WHHA_DTO.WH_TaxNumber);
                        WHA_DTO.Id = 11;
                        DS = WHA_DAO.WHTaxAssignDB(WHA_DTO);
                        WH_TaxAHead_DTO = TM_DL.WHEList(DS.Tables[0]).FirstOrDefault();
                    }
                }

                if (WHHA_DTO.WH_TaxCode != "" && WHHA_DTO.WH_TaxCode != null)
                {
                    WHA_DTO.WH_TaxCode = WHHA_DTO.WH_TaxCode;
                    WHA_DTO.Id = 4;
                    DS = WHA_DAO.WHTaxAssignDB(WHA_DTO);
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.TaxDes = DS.Tables[0].Rows[0][0].ToString();
                    }
                }

                List<WH_TaxAssign_DTO> WHA_List = WHTaxAssignGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                var PageList = PaginatedList_DTO<WH_TaxAssign_DTO>.CreateAsync(WHA_List, DPageNumber ?? 1, DPageSize);

                WH_TaxAHead_DTO.WH_TaxAssign_List = PageList;
                WH_TaxAHead_DTO.WH_TaxDetail = WHDA_DTO;
                return View(WH_TaxAHead_DTO);
                //}
                //catch (Exception ex)
                //{
                //    ViewBag.ErrorCode = 2;
                //    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                //    return View();
                //}
        }
        List<WH_TaxAssign_DTO> WHTaxAssignGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            WHA_DTO.Id = 21;
            WHA_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = WHA_DAO.WHTaxAssignDB(WHA_DTO);
            WHA_List = TM_DL.WHAList(DS.Tables[0]);

            ViewBag.WHTax = Help.GetCat(DS.Tables[1]);
            ViewBag.Nature = Help.GetCat(DS.Tables[2]);
            ViewBag.Tax = Help.GetCat(DS.Tables[3]);
            ViewBag.COA = Help.GetCat(DS.Tables[4]);

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

            var Key = WHA_List.OrderByDescending(Cs => Cs.WH_TaxNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.WH_TaxCode!.ToLower().Contains(Search.ToLower()) || K.WH_TaxDescription!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.WH_TaxNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.WH_TaxCode!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.WH_TaxCode!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.WH_TaxNumber);
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





        //Tax Cluster
        [Route("taxation/master/tax-cluster")]
        public IActionResult TaxCluster(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                List<TaxCluster_DTO> TC_List = TaxClusterGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                var PageList = PaginatedList_DTO<TaxCluster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize);

                var Model = new TaxClusterHead_DTO()
                {
                    TaxCluster_List = PageList,
                };
                return View(Model);
        }

        [Route("taxation/master/tax-cluster")]
        [HttpPost]
        public IActionResult TaxCluster(TaxClusterHead_DTO TXH_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                ////try
                ////{
                bool IsValid = false;
                TaxClusterHead_DTO TC_Head_DTO = new TaxClusterHead_DTO();

                List<TaxClusterDetail_DTO>? TCSD_DTO = new List<TaxClusterDetail_DTO>();
                if (TXH_DTO.TaxElement !=null)
                    TCSD_DTO = TXH_DTO.TaxElement!.Where(K => !K.IsDeleted).ToList();
                TC_Head_DTO = TXH_DTO;
                TC_Head_DTO.TaxElement = TCSD_DTO!;

                TC_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (TCSD_DTO.Count > 0)
                    {
                        ModelState.Clear();
                        IsValid = TryValidateModel(TC_Head_DTO);

                        if (IsValid)
                        {
                            using (var transaction = new TransactionScope())
                            {
                                TC_DTO.TaxCluster = TXH_DTO.TaxCluster;
                                TC_DTO.Id = 1;
                                DS = TC_DAO.TaxClusterDB(TC_DTO);
                                if (DS.Tables[0].Rows.Count == 0)
                                {
                                    TC_DTO.TaxCluster = TXH_DTO.TaxCluster;
                                    TC_DTO.ClusterDescription = TXH_DTO.ClusterDescription;
                                    TC_DTO.GST_Category = TXH_DTO.GST_Category;
                                    TC_DTO.GST_Type = TXH_DTO.GST_Type;
                                    TC_DTO.Id = 2;
                                    DS = TC_DAO.TaxClusterDB(TC_DTO);

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        TC_DTO.TaxClusterNumber = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var detail in TCSD_DTO)
                                        {
                                            TC_DTO.TaxIndex = Convert.ToInt32(detail.TaxIndex);
                                            TC_DTO.TaxElement = Convert.ToInt64(detail.TaxElement);
                                            TC_DTO.ChargeableBasis = Convert.ToInt64(detail.ChargeableBasis);
                                            TC_DTO.CalculationFactors = detail.CalculationFactors;
                                            TC_DTO.Id = 3;
                                            TC_DAO.TaxClusterDB(TC_DTO);
                                        }
                                        transaction.Complete();

                                        TXH_DTO.Reset();
                                        TCSD_DTO = TXH_DTO.TaxElement!;
                                        TC_DTO.Reset();
                                        ModelState.Clear();
                                    }
                                    else
                                    {
                                        transaction.Dispose();
                                        ViewBag.ErrorMessage = "Failed to insert tax details";
                                        ViewBag.ErrorCode = 2;
                                    }
                                }
                                else
                                {
                                    transaction.Dispose(); // Rollback
                                    ViewBag.ErrorMessage = "already assigned. Please check";
                                    ViewBag.ErrorCode = 2;
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
                    else
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = "Atleast one Row Required";
                    }
                }
                else if (Mode == "DeleteAll")
                {
                    try
                    {
                        TC_DTO.DeleteNumbers = DeleteNumbers;
                        TC_DTO.Id = 5;
                        TC_DAO.TaxClusterDB(TC_DTO);

                        TC_DTO.Reset();
                        TXH_DTO.Reset();
                        TCSD_DTO = TXH_DTO.TaxElement;
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
                        TC_DTO.TaxClusterNumber = Convert.ToInt64(Number);
                        TC_DTO.Id = 6;
                        TC_DAO.TaxClusterDB(TC_DTO);

                        TC_DTO.Reset();
                        TXH_DTO.Reset();
                        TCSD_DTO = TXH_DTO.TaxElement;
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
                    TC_DTO.Reset();
                    TXH_DTO.Reset();
                    TCSD_DTO = TXH_DTO.TaxElement;
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    TC_DTO.TaxClusterNumber = Convert.ToInt64(TXH_DTO.TaxClusterNumber);
                    TC_DTO.Id = 11;
                    DS = TC_DAO.TaxClusterDB(TC_DTO);
                    TC_Head_DTO = TM_DL.TCEList(DS.Tables[0]).FirstOrDefault();
                    TCSD_DTO = TM_DL.TCDList(DS.Tables[1]);
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        TC_DTO.GST_Category = DS.Tables[0].Rows[0]["GST_Category"].ToString();
                        TC_DTO.GST_Type = DS.Tables[0].Rows[0]["GST_Type"].ToString();
                    }
                }
                else if (Mode == "Update")
                {
                    if (TCSD_DTO.Count > 0)
                    {
                        ModelState.Clear();
                        IsValid = TryValidateModel(TC_Head_DTO);
                        if (IsValid)
                        {
                            using (var transaction = new TransactionScope()) //Use a transaction to ensure all operations succeed or fail together
                            {
                                try
                                {
                                    String Check = string.Join(", ", TCSD_DTO.Where(x => Convert.ToInt64(x.TaxElement) != 0).Select(x => x.TaxClusterDetailsNumber));
                                    TC_DTO.TaxClusterNumber = TXH_DTO.TaxClusterNumber;
                                    TC_DTO.DeleteNumbers = Check;
                                    TC_DTO.Id = 12;
                                    DS = TC_DAO.TaxClusterDB(TC_DTO);
                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        for (int i = 0; i < DS.Tables[0].Rows.Count; i++)
                                        {
                                            TC_DTO.TaxClusterNumber = TXH_DTO.TaxClusterNumber;
                                            TC_DTO.TaxClusterDetailsNumber = Convert.ToInt64(DS.Tables[0].Rows[0][0]);
                                            TC_DTO.Id = 13;
                                            TC_DAO.TaxClusterDB(TC_DTO);
                                        }
                                    }

                                    TC_DTO.TaxCluster = TXH_DTO.TaxCluster;
                                    TC_DTO.ClusterDescription = TXH_DTO.ClusterDescription;
                                    TC_DTO.GST_Category = TXH_DTO.GST_Category;
                                    TC_DTO.GST_Type = TXH_DTO.GST_Type;
                                    TC_DTO.Id = 14;
                                    DS = TC_DAO.TaxClusterDB(TC_DTO);

                                    foreach (var detail in TCSD_DTO)
                                    {
                                        TC_DTO.TaxIndex = Convert.ToInt32(detail.TaxIndex);
                                        TC_DTO.TaxClusterNumber = TXH_DTO.TaxClusterNumber;
                                        TC_DTO.TaxElement = Convert.ToInt64(detail.TaxElement);
                                        TC_DTO.ChargeableBasis = Convert.ToInt64(detail.ChargeableBasis);
                                        TC_DTO.CalculationFactors = detail.CalculationFactors;
                                        TC_DTO.Id = 3;
                                        if (detail.TaxClusterDetailsNumber == 0)
                                        {
                                            TC_DTO.Id = 3;
                                        }
                                        else
                                        {
                                            TC_DTO.TaxClusterDetailsNumber = detail.TaxClusterDetailsNumber;
                                            TC_DTO.Id = 15;
                                        }
                                        TC_DAO.TaxClusterDB(TC_DTO);
                                    }
                                    transaction.Complete();

                                    TXH_DTO.Reset();
                                    TCSD_DTO = TXH_DTO.TaxElement!;
                                    TC_DTO.Reset();
                                    ModelState.Clear();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                }
                            }
                        }
                        else
                        {
                            var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                            string CombinedString = string.Join("<br/>", Errors);

                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = CombinedString;

                            TC_DTO.TaxClusterNumber = Convert.ToInt64(TXH_DTO.TaxClusterNumber);
                            TC_DTO.Id = 11;
                            DS = TC_DAO.TaxClusterDB(TC_DTO);
                            TC_Head_DTO = TM_DL.TCEList(DS.Tables[0]).FirstOrDefault();
                            TCSD_DTO = TM_DL.TCDList(DS.Tables[1]);
                        }
                    }
                    else
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = "Atleast one Row Required";

                        TC_DTO.TaxClusterNumber = Convert.ToInt64(TXH_DTO.TaxClusterNumber);
                        TC_DTO.Id = 11;
                        DS = TC_DAO.TaxClusterDB(TC_DTO);
                        TC_Head_DTO = TM_DL.TCEList(DS.Tables[0]).FirstOrDefault();
                    }
                }

                if (TXH_DTO.GST_Category == "" || TXH_DTO.GST_Category == null) { }
                else
                {
                    TC_DTO.GST_Category = TXH_DTO.GST_Category;
                }
                if (TXH_DTO.GST_Type == "" || TXH_DTO.GST_Type == null) { }
                else
                {
                    TC_DTO.GST_Type = TXH_DTO.GST_Type;
                }

                List<TaxCluster_DTO> TC_List = TaxClusterGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                var PageList = PaginatedList_DTO<TaxCluster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize);

                TC_Head_DTO.TaxCluster_List = PageList;
                TC_Head_DTO.TaxElement = TCSD_DTO;
                return View(TC_Head_DTO);
                //}
                //catch (Exception ex)
                //{
                //    ViewBag.ErrorCode = 2;
                //    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                //    return View();
                //}
        }
        List<TaxCluster_DTO> TaxClusterGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TC_DTO.Id = 21;
            TC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TC_DAO.TaxClusterDB(TC_DTO);
            TC_List = TM_DL.TCList(DS.Tables[0]);

            ViewBag.Category = Help.GetCat(DS.Tables[1]);
            ViewBag.Type = Help.GetCat(DS.Tables[2]);
            ViewBag.TaxElement = Help.GetCat(DS.Tables[3]);
            ViewBag.Chargeable = Help.GetCat(DS.Tables[4]);

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

            var Key = TC_List.OrderByDescending(Cs => Cs.TaxClusterNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.TaxCluster!.ToLower().Contains(Search.ToLower()) || K.ClusterDescription!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.TaxClusterNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.TaxCluster!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.TaxCluster!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.TaxClusterNumber);
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

        [Route("taxation/master/tax-cluster/duplicate")]
        public Boolean TaxClusterDuplicate(String? Title, String? Number)
        {
            TC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TC_DTO.TaxCluster = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TC_DTO.Id = 7;
            }
            else
            {
                TC_DTO.TaxClusterNumber = Convert.ToInt32(Number);
                TC_DTO.Id = 8;
            }
            DS = TC_DAO.TaxClusterDB(TC_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("taxation/master/tax-cluster/getdescription")]
        public String TaxClusterDesc(String? Title)
        {
            TC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TC_DTO.TaxElement = Convert.ToInt32(Title);
            TC_DTO.Id = 4;
            DS = TC_DAO.TaxClusterDB(TC_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }





        //Tax Element
        [Route("taxation/master/tax-element")]
        public IActionResult TaxElement(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                List<TaxElement_DTO> TE_List = TaxElementGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                var PageList = PaginatedList_DTO<TaxElement_DTO>.CreateAsync(TE_List, DPageNumber ?? 1, DPageSize);

                var Model = new TaxElementHead_DTO()
                {
                    TaxElement_List = PageList,
                };
                return View(Model);
        }
        
        [Route("taxation/master/tax-element")]
        [HttpPost]
        public IActionResult TaxElement(TaxElementHead_DTO TEH_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                bool IsValid = false;
                TaxElementHead_DTO TE_Head_DTO = new TaxElementHead_DTO();

                List<TaxElementDetail_DTO>? A_DTO = new List<TaxElementDetail_DTO>();
                List<TaxElementDetail_DTO>? FN_DTO = new List<TaxElementDetail_DTO>();
                List<TaxElementVariableDetail_DTO>? VN_DTO = new List<TaxElementVariableDetail_DTO>();

                if (TEH_DTO.Abatement_List != null)
                    A_DTO = TEH_DTO.Abatement_List!.Where(K => !K.IsDeleted).ToList();

                if (TEH_DTO.Fixed_Nature_List != null)
                    FN_DTO = TEH_DTO.Fixed_Nature_List!.Where(K => !K.IsDeleted).ToList();

                if (TEH_DTO.Variable_Nature_List != null)
                    VN_DTO = TEH_DTO.Variable_Nature_List!.Where(K => !K.IsDeleted).ToList();

                TE_Head_DTO = TEH_DTO;
                TE_Head_DTO.Abatement_List = A_DTO!;
                TE_Head_DTO.Fixed_Nature_List = FN_DTO!;
                TE_Head_DTO.Variable_Nature_List = VN_DTO!;

                TE_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (TE_Head_DTO.LoadonInventory && Convert.ToDouble(TE_Head_DTO.LoadonInventoryPercent) < 0)
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = "Load on Inventory Percent must be above zero.";
                    }
                    else if ((TE_Head_DTO.GST_Abatement=="2" || A_DTO.Count > 0) && (FN_DTO.Count > 0 || VN_DTO.Count > 0))
                    {
                        ModelState.Clear();
                        IsValid = TryValidateModel(TE_Head_DTO);

                        if (IsValid)
                        {
                            using (var transaction = new TransactionScope())
                            {
                                TE_DTO.TaxElement = TEH_DTO.TaxElement;
                                TE_DTO.Id = 1;
                                DS = TE_DAO.TaxElementDB(TE_DTO);
                                if (DS.Tables[0].Rows.Count == 0)
                                {
                                    TE_DTO.TaxElement = TEH_DTO.TaxElement;
                                    TE_DTO.ElementDescription = TEH_DTO.ElementDescription;
                                    TE_DTO.TaxCategory = TEH_DTO.TaxCategory;
                                    TE_DTO.TaxType = TEH_DTO.TaxType;
                                    TE_DTO.LoadonInventory = Convert.ToBoolean(TEH_DTO.LoadonInventory);
                                    TE_DTO.LoadonInventoryPercent = TEH_DTO.LoadonInventoryPercent;
                                    TE_DTO.COA_LedgerAccount = TEH_DTO.COA_LedgerAccount;
                                    TE_DTO.GST_Abatement = TEH_DTO.GST_Abatement;
                                    TE_DTO.GST_TaxNature = TEH_DTO.GST_TaxNature;
                                    TE_DTO.Id = 2;
                                    DS = TE_DAO.TaxElementDB(TE_DTO);

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        TE_DTO.TaxElementNumber = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var detail in A_DTO)
                                        {
                                            TE_DTO.FromDate = Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd");
                                            TE_DTO.ToDate = Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd");
                                            TE_DTO.FixedPercent = Convert.ToString(detail.FixedPercent);
                                            TE_DTO.Id = 3;
                                            TE_DAO.TaxElementDB(TE_DTO);
                                        }
                                        foreach (var detail in FN_DTO)
                                        {
                                            TE_DTO.FromDate = Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd");
                                            TE_DTO.ToDate = Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd");
                                            TE_DTO.FixedPercent = Convert.ToString(detail.FixedPercent);
                                            TE_DTO.Id = 4;
                                            TE_DAO.TaxElementDB(TE_DTO);
                                        }
                                        foreach (var detail in VN_DTO)
                                        {
                                            TE_DTO.FromDate = Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd");
                                            TE_DTO.ToDate = Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd");
                                            TE_DTO.HSN = Convert.ToString(detail.HSN);
                                            TE_DTO.HSNPercent = Convert.ToString(detail.HSNPercent);
                                            TE_DTO.SAC = Convert.ToString(detail.SAC);
                                            TE_DTO.SACPercent = Convert.ToString(detail.SACPercent);
                                            TE_DTO.Id = 5;
                                            TE_DAO.TaxElementDB(TE_DTO);
                                        }

                                        transaction.Complete();

                                        TEH_DTO.Reset();
                                        A_DTO = TEH_DTO.Abatement_List!;
                                        FN_DTO = TEH_DTO.Fixed_Nature_List!;
                                        VN_DTO = TEH_DTO.Variable_Nature_List!;
                                        TE_DTO.Reset();
                                        ModelState.Clear();
                                    }
                                    else
                                    {
                                        transaction.Dispose();
                                        ViewBag.ErrorMessage = "Failed to insert tax details";
                                        ViewBag.ErrorCode = 2;
                                    }
                                }
                                else
                                {
                                    transaction.Dispose(); // Rollback
                                    ViewBag.ErrorMessage = "already assigned. Please check";
                                    ViewBag.ErrorCode = 2;
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
                    else
                    {
                        if (TE_Head_DTO.GST_Abatement == "2" || A_DTO.Count > 0)
                        { }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = "Abatement Atleast, one Row Required";
                        }
                        
                        if (FN_DTO.Count > 0 || VN_DTO.Count > 0)
                        { }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = "Fixed Nature or Fixed Nature, Atleast one Row Required";
                        }
                    }
                }
                else if (Mode == "DeleteAll")
                {
                    try
                    {
                        TE_DTO.DeleteNumbers = DeleteNumbers;
                        TE_DTO.Id = 11;
                        TE_DAO.TaxElementDB(TE_DTO);

                        TEH_DTO.Reset();
                        A_DTO = TEH_DTO.Abatement_List!;
                        FN_DTO = TEH_DTO.Fixed_Nature_List!;
                        VN_DTO = TEH_DTO.Variable_Nature_List!;
                        TE_DTO.Reset();
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
                        TE_DTO.TaxElementNumber = Convert.ToInt64(Number);
                        TE_DTO.Id = 12;
                        TE_DAO.TaxElementDB(TE_DTO);

                        TEH_DTO.Reset();
                        A_DTO = TEH_DTO.Abatement_List!;
                        FN_DTO = TEH_DTO.Fixed_Nature_List!;
                        VN_DTO = TEH_DTO.Variable_Nature_List!;
                        TE_DTO.Reset();
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
                    TEH_DTO.Reset();
                    A_DTO = TEH_DTO.Abatement_List!;
                    FN_DTO = TEH_DTO.Fixed_Nature_List!;
                    VN_DTO = TEH_DTO.Variable_Nature_List!;
                    TE_DTO.Reset();
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    TE_DTO.TaxElementNumber = Convert.ToInt64(TEH_DTO.TaxElementNumber);
                    TE_DTO.Id = 31;
                    DS = TE_DAO.TaxElementDB(TE_DTO);
                    TE_Head_DTO = TM_DL.TEEList(DS.Tables[0]).FirstOrDefault();
                    A_DTO = TM_DL.TEDList(DS.Tables[1]);
                    FN_DTO = TM_DL.TEDList(DS.Tables[2]);
                    VN_DTO = TM_DL.TEVDList(DS.Tables[3]);
                }
                else if (Mode == "Update")
                {
                    if (TE_Head_DTO.LoadonInventory && Convert.ToDouble(TE_Head_DTO.LoadonInventoryPercent) < 0)
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = "Load on Inventory Percent must be above zero.";
                    }
                    else if ((TE_Head_DTO.GST_Abatement == "2" || A_DTO.Count > 0) && (FN_DTO.Count > 0 || VN_DTO.Count > 0))
                    {
                        ModelState.Clear();
                        IsValid = TryValidateModel(TE_Head_DTO);
                        if (IsValid)
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String ACheck = string.Join(", ", A_DTO.Where(x => Convert.ToInt64(x.TaxNumber) != 0).Select(x => x.TaxNumber));
                                    String FNCheck = string.Join(", ", FN_DTO.Where(x => Convert.ToInt64(x.TaxNumber) != 0).Select(x => x.TaxNumber));
                                    String VNCheck = string.Join(", ", VN_DTO.Where(x => Convert.ToInt64(x.TaxNumber) != 0).Select(x => x.TaxNumber));

                                    TE_DTO.TaxElementNumber = TEH_DTO.TaxElementNumber;
                                    TE_DTO.DeleteNumbers = ACheck;
                                    TE_DTO.Id = 32;
                                    TE_DAO.TaxElementDB(TE_DTO);

                                    TE_DTO.TaxElementNumber = TEH_DTO.TaxElementNumber;
                                    TE_DTO.DeleteNumbers = FNCheck;
                                    TE_DTO.Id = 33;
                                    TE_DAO.TaxElementDB(TE_DTO);

                                    TE_DTO.TaxElementNumber = TEH_DTO.TaxElementNumber;
                                    TE_DTO.DeleteNumbers = VNCheck;
                                    TE_DTO.Id = 34;
                                    TE_DAO.TaxElementDB(TE_DTO);

                                    TE_DTO.TaxElementNumber = TEH_DTO.TaxElementNumber;
                                    TE_DTO.TaxElement = TEH_DTO.TaxElement;
                                    TE_DTO.ElementDescription = TEH_DTO.ElementDescription;
                                    TE_DTO.TaxCategory = TEH_DTO.TaxCategory;
                                    TE_DTO.TaxType = TEH_DTO.TaxType;
                                    TE_DTO.LoadonInventory = TEH_DTO.LoadonInventory;
                                    TE_DTO.LoadonInventoryPercent = TEH_DTO.LoadonInventoryPercent;
                                    TE_DTO.COA_LedgerAccount = TEH_DTO.COA_LedgerAccount;
                                    TE_DTO.GST_Abatement = TEH_DTO.GST_Abatement;
                                    TE_DTO.GST_TaxNature = TEH_DTO.GST_TaxNature;
                                    TE_DTO.Id = 35;
                                    TE_DAO.TaxElementDB(TE_DTO);

                                    foreach (var detail in A_DTO)
                                    {
                                        TE_DTO.FromDate = Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd");
                                        TE_DTO.ToDate = Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd");
                                        TE_DTO.FixedPercent = Convert.ToString(detail.FixedPercent);
                                        if (detail.TaxNumber == 0)
                                        {
                                            TE_DTO.Id = 3;
                                        }
                                        else
                                        {
                                            TE_DTO.TaxNumber = detail.TaxNumber;
                                            TE_DTO.Id = 36;
                                        }
                                        TE_DAO.TaxElementDB(TE_DTO);
                                    }
                                    foreach (var detail in FN_DTO)
                                    {
                                        TE_DTO.FromDate = Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd");
                                        TE_DTO.ToDate = Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd");
                                        TE_DTO.FixedPercent = Convert.ToString(detail.FixedPercent);
                                        if (detail.TaxNumber == 0)
                                        {
                                            TE_DTO.Id = 4;
                                        }
                                        else
                                        {
                                            TE_DTO.TaxNumber = detail.TaxNumber;
                                            TE_DTO.Id = 37;
                                        }
                                        TE_DAO.TaxElementDB(TE_DTO);
                                    }
                                    foreach (var detail in VN_DTO)
                                    {
                                        TE_DTO.FromDate = Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd");
                                        TE_DTO.ToDate = Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd");
                                        TE_DTO.HSN = Convert.ToString(detail.HSN);
                                        TE_DTO.HSNPercent = Convert.ToString(detail.HSNPercent);
                                        TE_DTO.SAC = Convert.ToString(detail.SAC);
                                        TE_DTO.SACPercent = Convert.ToString(detail.SACPercent);
                                        if (detail.TaxNumber == 0)
                                        {
                                            TE_DTO.Id = 5;
                                        }
                                        else
                                        {
                                            TE_DTO.TaxNumber = detail.TaxNumber;
                                            TE_DTO.Id = 38;
                                        }
                                        TE_DAO.TaxElementDB(TE_DTO);
                                    }

                                    transaction.Complete();

                                    TEH_DTO.Reset();
                                    A_DTO = TEH_DTO.Abatement_List!;
                                    FN_DTO = TEH_DTO.Fixed_Nature_List!;
                                    VN_DTO = TEH_DTO.Variable_Nature_List!;
                                    TE_DTO.Reset();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                                }
                            }
                        }
                        else
                        {
                            var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                            string CombinedString = string.Join("<br/>", Errors);

                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = CombinedString;

                            //TE_DTO.TaxElementNumber = Convert.ToInt64(TEH_DTO.TaxElementNumber);
                            //TE_DTO.Id = 31;
                            //DS = TE_DAO.TaxElementDB(TE_DTO);
                            //TE_Head_DTO = TM_DL.TEEList(DS.Tables[0]).FirstOrDefault();

                            //A_DTO = TM_DL.TEDList(DS.Tables[1]);
                            //FN_DTO = TM_DL.TEDList(DS.Tables[2]);
                            //VN_DTO = TM_DL.TEVDList(DS.Tables[3]);
                        }
                    }
                    else
                    {
                        if (TE_Head_DTO.GST_Abatement == "2" || A_DTO.Count > 0)
                        { }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = "Abatement Atleast, one Row Required";
                        }

                        if (FN_DTO.Count > 0 || VN_DTO.Count > 0)
                        { }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = "Fixed Nature or Fixed Nature, Atleast one Row Required";
                        }
                        //TE_DTO.TaxElementNumber = Convert.ToInt64(TEH_DTO.TaxElementNumber);
                        //TE_DTO.Id = 31;
                        //DS = TE_DAO.TaxElementDB(TE_DTO);
                        //TE_Head_DTO = TM_DL.TEEList(DS.Tables[0]).FirstOrDefault();
                        //A_DTO = TM_DL.TEDList(DS.Tables[1]);
                        //FN_DTO = TM_DL.TEDList(DS.Tables[2]);
                        //VN_DTO = TM_DL.TEVDList(DS.Tables[3]);
                    }
                }

                List<TaxElement_DTO> TE_List = TaxElementGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                var PageList = PaginatedList_DTO<TaxElement_DTO>.CreateAsync(TE_List, DPageNumber ?? 1, DPageSize);

                TE_Head_DTO.TaxElement_List = PageList;
                TE_Head_DTO.Fixed_Nature_List = FN_DTO;
                TE_Head_DTO.Variable_Nature_List = VN_DTO;
                TE_Head_DTO.Abatement_List = A_DTO;
                return View(TE_Head_DTO);
        }

        List<TaxElement_DTO> TaxElementGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TE_DTO.Id = 21;
            TE_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TE_DAO.TaxElementDB(TE_DTO);
            TE_List = TM_DL.TEList(DS.Tables[0]);

            ViewBag.Category = Help.GetCat(DS.Tables[1]);
            ViewBag.Type = Help.GetCat(DS.Tables[2]);
            ViewBag.Ledger = Help.GetCat(DS.Tables[3]);
            ViewBag.Nature = Help.GetUnder(DS.Tables[4]);
            ViewBag.Abatement = Help.GetUnder(DS.Tables[5]);
            ViewBag.HSN = Help.GetCat(DS.Tables[6]);
            ViewBag.SAC = Help.GetCat(DS.Tables[7]);

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

            var Key = TE_List.OrderByDescending(Cs => Cs.TaxElementNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.TaxElement!.ToLower().Contains(Search.ToLower()) || K.ElementDescription!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.TaxElementNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.TaxElement!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.TaxElement!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.TaxElementNumber);
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

        [Route("taxation/master/tax-element/duplicate")]
        public Boolean TaxElementDuplicate(String? Title, String? Number)
        {
            TE_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TE_DTO.TaxElement = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TE_DTO.Id = 41;
            }
            else
            {
                TE_DTO.TaxElementNumber = Convert.ToInt32(Number);
                TE_DTO.Id = 42;
            }
            DS = TE_DAO.TaxElementDB(TE_DTO);
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

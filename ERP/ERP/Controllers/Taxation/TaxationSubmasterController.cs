using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ERP.Controllers.Taxation
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class TaxationSubmasterController : Controller
    {
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();
        List<TaxationSubmaster_DTO> TS_List = new List<TaxationSubmaster_DTO>();
        TaxationSubmaster_DL TS_DL = new TaxationSubmaster_DL();
        TaxationSubmaster_DAO TS_DAO = new TaxationSubmaster_DAO();
        TaxationSubmaster_DTO TS_DTO = new TaxationSubmaster_DTO();

        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;

        //WH Tax Category
        [Route("taxation/submaster/wh-tax-category")]
        public IActionResult WHTaxCategory(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = WHTaxGetData(SortOrder, Search, PageNumber, PSize, PageFilter);

            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/wh-tax-category")]
        [HttpPost]
        public IActionResult WHTaxCategory(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 6;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 1;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Save();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 3;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                        TS_DTO.Id = 8;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                    TS_DTO.Id = 4;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.TCList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 7;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 5;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Edit();
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
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }

                List<TaxationSubmaster_DTO> TC_List = WHTaxGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> WHTaxGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 2;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.TCList(DS.Tables[0]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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

                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }



        [Route("taxation/submaster/wh-tax-category/duplicate")]
        public Boolean WHTaxCategoryDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 6;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 7;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //WH Tax Type
        [Route("taxation/submaster/wh-tax-type")]
        public IActionResult WHTaxType(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = WHGetData(SortOrder, Search, PageNumber, PSize, PageFilter);

            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/wh-tax-type")]
        [HttpPost]
        public IActionResult WHTaxType(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 26;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 21;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Save();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 23;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                        TS_DTO.Id = 28;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                    TS_DTO.Id = 24;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.TTList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 27;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 25;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Edit();
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
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }


                List<TaxationSubmaster_DTO> TC_List = WHGetData(SortOrder, Search, PageNumber, PSize, PageFilter);

                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> WHGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 22;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.TTList(DS.Tables[0]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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

                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }



        [Route("taxation/submaster/wh-tax-type/duplicate")]
        public Boolean WHTaxTypeDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 26;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 27;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //WH Tax Impact
        [Route("taxation/submaster/wh-tax-impact")]
        public IActionResult WHTaxImpact(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = WHIGetData(SortOrder, Search, PageNumber, PSize, PageFilter);

            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/wh-tax-impact")]
        [HttpPost]
        public IActionResult WHTaxImpact(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 46;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 41;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 43;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                        TS_DTO.Id = 48;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                    TS_DTO.Id = 44;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.TIList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 47;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 45;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                    else
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }


                List<TaxationSubmaster_DTO> TC_List = WHIGetData(SortOrder, Search, PageNumber, PSize, PageFilter);

                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> WHIGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 42;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.TIList(DS.Tables[0]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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

                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }



        [Route("taxation/submaster/wh-tax-impact/duplicate")]
        public Boolean WHTaxImpactDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 46;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 47;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);

            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //Nature of assessee
        [Route("taxation/submaster/nature-of-assessee")]
        public IActionResult NatureofAssessee(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = NOAGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/nature-of-assessee")]
        [HttpPost]
        public IActionResult NatureofAssessee(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 66;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 61;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Save();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 63;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                        TS_DTO.Id = 68;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                    TS_DTO.Id = 64;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.NAList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 67;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 65;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Edit();
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
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }


                List<TaxationSubmaster_DTO> TC_List = NOAGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> NOAGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 62;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.NAList(DS.Tables[0]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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

                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }



        [Route("taxation/submaster/nature-of-assessee/duplicate")]
        public Boolean NatureofAssesseeDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 66;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 67;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);

            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //Registration Type
        [Route("taxation/submaster/registration-type")]
        public IActionResult RegistrationType(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = RTGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/registration-type")]
        [HttpPost]
        public IActionResult RegistrationType(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 86;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 81;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Save();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 83;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                        TS_DTO.Id = 88;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                    TS_DTO.Id = 84;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.RTList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 87;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 85;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Edit();
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
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }

                List<TaxationSubmaster_DTO> TC_List = RTGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> RTGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 82;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.RTList(DS.Tables[0]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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

                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }



        [Route("taxation/submaster/registration-type/duplicate")]
        public Boolean RegistrationTypeDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 86;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 87;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);

            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //Assessee Territory
        [Route("taxation/submaster/assessee-territory")]
        public IActionResult AssesseeTerritory(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = ATGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/assessee-territory")]
        [HttpPost]
        public IActionResult AssesseeTerritory(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 106;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 101;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Save();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 103;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                        TS_DTO.Id = 108;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                    TS_DTO.Id = 104;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.ATList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 47;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 105;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Edit();
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
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }

                List<TaxationSubmaster_DTO> TC_List = ATGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> ATGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 102;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.ATList(DS.Tables[0]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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
                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }



        [Route("taxation/submaster/assessee-territory/duplicate")]
        public Boolean AssesseeTerritoryDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 106;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 107;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //Gst Tax Category
        [Route("taxation/submaster/gst-tax-category")]
        public IActionResult GSTTaxCategory(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = GSTCGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/gst-tax-category")]
        [HttpPost]
        public IActionResult GSTTaxCategory(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 126;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 121;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Save();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 123;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                        TS_DTO.Id = 128;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

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
                    TS_DTO.Id = 124;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.GCList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 127;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 125;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Edit();
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
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }


                List<TaxationSubmaster_DTO> TC_List = GSTCGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> GSTCGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 122;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.GCList(DS.Tables[0]);
            ViewBag.Location = Help.GetUnder(DS.Tables[1]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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
                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }



        [Route("taxation/submaster/gst-tax-category/duplicate")]
        public Boolean GSTTaxCategoryDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 126;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 127;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //Gst Tax Type
        [Route("taxation/submaster/gst-tax-type")]
        public IActionResult GSTTaxType(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = GSTTGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/gst-tax-type")]
        [HttpPost]
        public IActionResult GSTTaxType(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 146;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 141;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Save();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 143;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

                        ModelState.Clear();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                    }
                    //ViewBag.ErrorCode = 1;
                    //ViewBag.ErrorMessage = Alert.Delete();
                }
                else if (Mode == "Delete")
                {
                    try
                    {
                        TS_DTO.Id = 148;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

                        ModelState.Clear();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                    }
                    //ViewBag.ErrorCode = 1;
                    //ViewBag.ErrorMessage = Alert.Delete();
                }
                else if (Mode == "Clear")
                {
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    TS_DTO.Id = 144;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.GTList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 147;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 145;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Edit();
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
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }

                List<TaxationSubmaster_DTO> TC_List = GSTTGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> GSTTGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 142;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.GTList(DS.Tables[0]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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
                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }



        [Route("taxation/submaster/gst-tax-type/duplicate")]
        public Boolean GSTTaxTypeDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 146;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 147;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //Gst Tax Nature 
        [Route("taxation/submaster/gst-tax-nature")]
        public IActionResult GSTTaxNature(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = GSTNGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/gst-tax-nature")]
        [HttpPost]
        public IActionResult GSTTaxNature(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 166;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 161;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Save();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 163;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

                        ModelState.Clear();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                    }
                    //ViewBag.ErrorCode = 1;
                    //ViewBag.ErrorMessage = Alert.Delete();
                }
                else if (Mode == "Delete")
                {
                    try
                    {
                        TS_DTO.Id = 168;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

                        ModelState.Clear();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                    }
                    //ViewBag.ErrorCode = 1;
                    //ViewBag.ErrorMessage = Alert.Delete();
                }
                else if (Mode == "Clear")
                {
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    TS_DTO.Id = 164;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.GNList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 167;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 165;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Edit();
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
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }

                List<TaxationSubmaster_DTO> TC_List = GSTNGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> GSTNGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 162;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.GNList(DS.Tables[0]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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
                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }



        [Route("taxation/submaster/gst-tax-nature/duplicate")]
        public Boolean GSTTaxNatureDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 166;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 167;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }





        //Gst Chargeable Basis 
        [Route("taxation/submaster/gst-chargeable-basis")]
        public IActionResult GSTChargeableBasis(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<TaxationSubmaster_DTO> TC_List = CBGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("taxation/submaster/gst-chargeable-basis")]
        [HttpPost]
        public IActionResult GSTChargeableBasis(TaxationSubmaster_DTO TS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        TS_DTO.Id = 186;
                        DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            TS_DTO.Id = 181;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Save();
                        }
                        else
                        {
                            ViewBag.ErrorCode = 2;
                            ViewBag.ErrorMessage = Alert.Duplicate();
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
                        TS_DTO.DeleteNumbers = DeleteNumbers;
                        TS_DTO.Id = 183;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

                        ModelState.Clear();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                    }
                    //ViewBag.ErrorCode = 1;
                    //ViewBag.ErrorMessage = Alert.Delete();
                }
                else if (Mode == "Delete")
                {
                    try
                    {
                        TS_DTO.Id = 188;
                        TS_DAO.TaxationSubmasterDB(TS_DTO);

                        ModelState.Clear();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                    }
                    //ViewBag.ErrorCode = 1;
                    //ViewBag.ErrorMessage = Alert.Delete();
                }
                else if (Mode == "Clear")
                {
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    TS_DTO.Id = 184;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    ViewBag.Submaster = TS_DL.CBList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    TS_DTO.Id = 187;
                    DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            TS_DTO.Id = 185;
                            TS_DAO.TaxationSubmasterDB(TS_DTO);

                            ModelState.Clear();
                            //ViewBag.ErrorCode = 1;
                            //ViewBag.ErrorMessage = Alert.Edit();
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
                        ViewBag.ErrorMessage = Alert.Duplicate();
                    }
                }

                List<TaxationSubmaster_DTO> TC_List = CBGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<TaxationSubmaster_DTO>.CreateAsync(TC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<TaxationSubmaster_DTO> CBGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            TS_DTO.Id = 182;
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
            TS_List = TS_DL.CBList(DS.Tables[0]);

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

            var Key = TS_List.OrderByDescending(Cs => Cs.Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.Title!.ToLower().Contains(Search.ToLower()) || K.Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.Title!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.Title!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.Number);
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
                DPageNumber = Record > RecordPage ? Convert.ToInt32(PageNumber) : Convert.ToInt32(PageNumber) - 1;
            }

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }


        [Route("taxation/submaster/gst-chargeable-basis/duplicate")]
        public Boolean GSTChargeableBasisDuplicate(String? Title, String? Number)
        {
            TS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            TS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                TS_DTO.Id = 186;
            }
            else
            {
                TS_DTO.Number = Convert.ToInt32(Number);
                TS_DTO.Id = 187;
            }
            DS = TS_DAO.TaxationSubmasterDB(TS_DTO);
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
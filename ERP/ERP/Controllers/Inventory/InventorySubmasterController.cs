using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ERP.Controllers.Inventory
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class InventorySubmasterController : Controller
    {
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();
        List<InventorySubmaster_DTO> IS_List = new List<InventorySubmaster_DTO>();
        InventorySubmaster_DL IS_DL = new InventorySubmaster_DL();
        InventorySubmaster_DAO IS_DAO = new InventorySubmaster_DAO();
        InventorySubmaster_DTO IS_DTO = new InventorySubmaster_DTO();

        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;



        //Material Classification
        [Route("inventory/submaster/material-classification")]
        public IActionResult MaterialClassification(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<InventorySubmaster_DTO> IS_List = MCGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<InventorySubmaster_DTO>.CreateAsync(IS_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("inventory/submaster/material-classification")]
        [HttpPost]
        public IActionResult MaterialClassification(InventorySubmaster_DTO IS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        IS_DTO.Id = 6;
                        DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            IS_DTO.Id = 1;
                            IS_DAO.InventorySubmasterDB(IS_DTO);

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
                        IS_DTO.DeleteNumbers = DeleteNumbers;
                        IS_DTO.Id = 3;
                        IS_DAO.InventorySubmasterDB(IS_DTO);

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
                        IS_DTO.Id = 8;
                        IS_DAO.InventorySubmasterDB(IS_DTO);

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
                    IS_DTO.Id = 4;
                    DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                    ViewBag.Submaster = IS_DL.MCList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    IS_DTO.Id = 7;
                    DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            IS_DTO.Id = 5;
                            IS_DAO.InventorySubmasterDB(IS_DTO);

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
                    }
                }

                List<InventorySubmaster_DTO> IS_List = MCGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<InventorySubmaster_DTO>.CreateAsync(IS_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<InventorySubmaster_DTO> MCGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            IS_DTO.Id = 2;
            IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = IS_DAO.InventorySubmasterDB(IS_DTO);
            IS_List = IS_DL.MCList(DS.Tables[0]);

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

            var Key = IS_List.OrderByDescending(Cs => Cs.Number);
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


        [Route("inventory/submaster/material-classification/duplicate")]
        public Boolean MaterialClassificationDuplicate(String? Title, String? Number)
        {
            IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            IS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                IS_DTO.Id = 6;
            }
            else
            {
                IS_DTO.Number = Convert.ToInt32(Number);
                IS_DTO.Id = 7;
            }
            DS = IS_DAO.InventorySubmasterDB(IS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //Material Ownership
        [Route("inventory/submaster/material-ownership")]
        public IActionResult MaterialOwnership(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<InventorySubmaster_DTO> IS_List = MOGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<InventorySubmaster_DTO>.CreateAsync(IS_List, DPageNumber ?? 1, DPageSize));

        }

        [Route("inventory/submaster/material-ownership")]
        [HttpPost]
        public IActionResult MaterialOwnership(InventorySubmaster_DTO IS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        IS_DTO.Id = 26;
                        DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            IS_DTO.Id = 21;
                            IS_DAO.InventorySubmasterDB(IS_DTO);

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
                        IS_DTO.DeleteNumbers = DeleteNumbers;
                        IS_DTO.Id = 23;
                        IS_DAO.InventorySubmasterDB(IS_DTO);

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
                        IS_DTO.Id = 28;
                        IS_DAO.InventorySubmasterDB(IS_DTO);

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
                    IS_DTO.Id = 24;
                    DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                    ViewBag.Submaster = IS_DL.MOList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    IS_DTO.Id = 27;
                    DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            IS_DTO.Id = 25;
                            IS_DAO.InventorySubmasterDB(IS_DTO);

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


                List<InventorySubmaster_DTO> IS_List = MOGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<InventorySubmaster_DTO>.CreateAsync(IS_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }

        }
        List<InventorySubmaster_DTO> MOGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            IS_DTO.Id = 22;
            IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = IS_DAO.InventorySubmasterDB(IS_DTO);
            IS_List = IS_DL.MOList(DS.Tables[0]);

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

            var Key = IS_List.OrderByDescending(Cs => Cs.Number);
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


        [Route("inventory/submaster/material-ownership/duplicate")]
        public Boolean MaterialOwnershipDuplicate(String? Title, String? Number)
        {
            IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            IS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                IS_DTO.Id = 26;
                DS = IS_DAO.InventorySubmasterDB(IS_DTO);
            }
            else
            {
                IS_DTO.Number = Convert.ToInt32(Number);
                IS_DTO.Id = 27;
            }
            DS = IS_DAO.InventorySubmasterDB(IS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //Material Segregation
        [Route("inventory/submaster/material-segregation")]
        public IActionResult MaterialSegregation(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            List<InventorySubmaster_DTO> IS_List = MSGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<InventorySubmaster_DTO>.CreateAsync(IS_List, DPageNumber ?? 1, DPageSize));

        }

        [Route("inventory/submaster/material-segregation")]
        [HttpPost]
        public IActionResult MaterialSegregation(InventorySubmaster_DTO IS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            try
            {
                IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        IS_DTO.Id = 46;
                        DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            IS_DTO.Id = 41;
                            IS_DAO.InventorySubmasterDB(IS_DTO);

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
                        IS_DTO.DeleteNumbers = DeleteNumbers;
                        IS_DTO.Id = 43;
                        IS_DAO.InventorySubmasterDB(IS_DTO);

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
                        IS_DTO.Id = 48;
                        IS_DAO.InventorySubmasterDB(IS_DTO);

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
                    IS_DTO.Id = 44;
                    DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                    ViewBag.Submaster = IS_DL.MSList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    IS_DTO.Id = 47;
                    DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            IS_DTO.Id = 45;
                            IS_DAO.InventorySubmasterDB(IS_DTO);

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


                List<InventorySubmaster_DTO> IS_List = MSGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<InventorySubmaster_DTO>.CreateAsync(IS_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }

        }
        List<InventorySubmaster_DTO> MSGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            IS_DTO.Id = 42;
            IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = IS_DAO.InventorySubmasterDB(IS_DTO);
            IS_List = IS_DL.MSList(DS.Tables[0]);

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

            var Key = IS_List.OrderByDescending(Cs => Cs.Number);
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


        [Route("inventory/submaster/material-segregation/duplicate")]
        public Boolean MaterialSegregationDuplicate(String? Title, String? Number)
        {
            IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            IS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                IS_DTO.Id = 46;
            }
            else
            {
                IS_DTO.Number = Convert.ToInt32(Number);
                IS_DTO.Id = 47;
            }
            DS = IS_DAO.InventorySubmasterDB(IS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //Warehouse Category
        [Route("inventory/submaster/warehouse-category")]
        public IActionResult WarehouseCategory(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<InventorySubmaster_DTO> IS_List = WCGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<InventorySubmaster_DTO>.CreateAsync(IS_List, DPageNumber ?? 1, DPageSize));

        }

        [Route("inventory/submaster/warehouse-category")]
        [HttpPost]
        public IActionResult WarehouseCategory(InventorySubmaster_DTO IS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            try
            {
                IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        IS_DTO.Id = 66;
                        DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            IS_DTO.Id = 61;
                            IS_DAO.InventorySubmasterDB(IS_DTO);

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
                        IS_DTO.DeleteNumbers = DeleteNumbers;
                        IS_DTO.Id = 63;
                        IS_DAO.InventorySubmasterDB(IS_DTO);

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
                        IS_DTO.Id = 68;
                        IS_DAO.InventorySubmasterDB(IS_DTO);

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
                    IS_DTO.Id = 64;
                    DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                    ViewBag.Submaster = IS_DL.WCList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    IS_DTO.Id = 67;
                    DS = IS_DAO.InventorySubmasterDB(IS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            IS_DTO.Id = 65;
                            IS_DAO.InventorySubmasterDB(IS_DTO);

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


                List<InventorySubmaster_DTO> IS_List = WCGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<InventorySubmaster_DTO>.CreateAsync(IS_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }

        }
        List<InventorySubmaster_DTO> WCGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            IS_DTO.Id = 62;
            IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = IS_DAO.InventorySubmasterDB(IS_DTO);
            IS_List = IS_DL.WCList(DS.Tables[0]);

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

            var Key = IS_List.OrderByDescending(Cs => Cs.Number);
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


        [Route("inventory/submaster/warehouse-category/duplicate")]
        public Boolean WarehouseCategoryDuplicate(String? Title, String? Number)
        {
            IS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            IS_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                IS_DTO.Id = 66;
            }
            else
            {
                IS_DTO.Number = Convert.ToInt32(Number);
                IS_DTO.Id = 67;
            }
            DS = IS_DAO.InventorySubmasterDB(IS_DTO);
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
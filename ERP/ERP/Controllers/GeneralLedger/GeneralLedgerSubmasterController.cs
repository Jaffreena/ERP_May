using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Data.SqlClient;

namespace ERP.Controllers.GeneralLedger
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class GeneralLedgerSubmasterController : Controller
    {
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();
        List<GeneralLedgerSubmaster_DTO> GL_List = new List<GeneralLedgerSubmaster_DTO>();
        GeneralLedgerSubmaster_DL GL_DL = new GeneralLedgerSubmaster_DL();
        GeneralLedgerSubmaster_DAO GL_DAO = new GeneralLedgerSubmaster_DAO();
        GeneralLedgerSubmaster_DTO GL_DTO = new GeneralLedgerSubmaster_DTO();
        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;

        //COA Group Nature
        [Route("general-ledger/submaster/coa-group-nature")]
        public IActionResult COAGroupNature(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<GeneralLedgerSubmaster_DTO> GL_List = GetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<GeneralLedgerSubmaster_DTO>.CreateAsync(GL_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("general-ledger/submaster/coa-group-nature")]
        [HttpPost]
        public IActionResult COAGroupNature(GeneralLedgerSubmaster_DTO GL_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                GL_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        GL_DTO.Id = 6;
                        DS = GL_DAO.GeneralLedgerSubmasterDB(GL_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            GL_DTO.Id = 1;
                            GL_DAO.GeneralLedgerSubmasterDB(GL_DTO);

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
                        GL_DTO.DeleteNumbers = DeleteNumbers;
                        GL_DTO.Id = 3;
                        GL_DAO.GeneralLedgerSubmasterDB(GL_DTO);

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
                        GL_DTO.Id = 8;
                        GL_DAO.GeneralLedgerSubmasterDB(GL_DTO);

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
                    GL_DTO.Id = 4;
                    DS = GL_DAO.GeneralLedgerSubmasterDB(GL_DTO);
                    ViewBag.Submaster = GL_DL.GLList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    GL_DTO.Id = 7;
                    DS = GL_DAO.GeneralLedgerSubmasterDB(GL_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            GL_DTO.Id = 5;
                            GL_DAO.GeneralLedgerSubmasterDB(GL_DTO);

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


                List<GeneralLedgerSubmaster_DTO> GL_List = GetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<GeneralLedgerSubmaster_DTO>.CreateAsync(GL_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }

        }
        List<GeneralLedgerSubmaster_DTO> GetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            GL_DTO.Id = 2;
            GL_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = GL_DAO.GeneralLedgerSubmasterDB(GL_DTO);
            GL_List = GL_DL.GLList(DS.Tables[0]);

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

            var Key = GL_List.OrderByDescending(Cs => Cs.Number);
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


        [Route("general-ledger/submaster/coa-group-nature/duplicate")]
        public Boolean COAGroupNatureDuplicate(String? Title, String? Number)
        {
            GL_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GL_DTO.Title = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                GL_DTO.Id = 6;
            }
            else
            {
                GL_DTO.Number = Convert.ToInt32(Number);
                GL_DTO.Id = 7;
            }
            DS = GL_DAO.GeneralLedgerSubmasterDB(GL_DTO);
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
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Data.SqlClient;

namespace ERP.Controllers.Procurement
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class ProcurementSubmasterController : Controller
    {
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();
        List<ProcurementSubmaster_DTO> PS_List = new List<ProcurementSubmaster_DTO>();
        ProcurementSubmaster_DL PS_DL = new ProcurementSubmaster_DL();
        ProcurementSubmaster_DAO PS_DAO = new ProcurementSubmaster_DAO();
        ProcurementSubmaster_DTO PS_DTO = new ProcurementSubmaster_DTO();
        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;



        [Route("procurement/submaster/payment-base")]
        public IActionResult PaymentBase(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            List<ProcurementSubmaster_DTO> PS_List = GetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<ProcurementSubmaster_DTO>.CreateAsync(PS_List, DPageNumber ?? 1, DPageSize));

        }

        [Route("procurement/submaster/payment-base")]
        [HttpPost]
        public IActionResult PaymentBase(ProcurementSubmaster_DTO PS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            try
            {
                PS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        PS_DTO.Id = 6;
                        DS = PS_DAO.ProcurementSubmasterDB(PS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            PS_DTO.Id = 1;
                            PS_DAO.ProcurementSubmasterDB(PS_DTO);

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
                    PS_DTO.DeleteNumbers = DeleteNumbers;
                    PS_DTO.Id = 3;
                    PS_DAO.ProcurementSubmasterDB(PS_DTO);

                    ModelState.Clear();
                }
                else if (Mode == "Delete")
                {
                    PS_DTO.Id = 8;
                    PS_DAO.ProcurementSubmasterDB(PS_DTO);

                    ModelState.Clear();
                }
                else if (Mode == "Clear")
                {
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    PS_DTO.Id = 4;
                    DS = PS_DAO.ProcurementSubmasterDB(PS_DTO);
                    ViewBag.Submaster = PS_DL.PSList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    PS_DTO.Id = 7;
                    DS = PS_DAO.ProcurementSubmasterDB(PS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            PS_DTO.Id = 5;
                            PS_DAO.ProcurementSubmasterDB(PS_DTO);

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
                }

                DPageNumber = PageNumber;
                DPageSize = PSize;
                List<ProcurementSubmaster_DTO> PS_List = GetData(SortOrder, Search, PageNumber, PSize, PageFilter);

                return View(PaginatedList<ProcurementSubmaster_DTO>.CreateAsync(PS_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }

        }
        List<ProcurementSubmaster_DTO> GetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            PS_DTO.Id = 2;
            PS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PS_DAO.ProcurementSubmasterDB(PS_DTO);
            PS_List = PS_DL.PSList(DS.Tables[0]);

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

            var Key = PS_List.OrderByDescending(Cs => Cs.Number);
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


        [Route("procurement/submaster/payment-base/duplicate")]
        public Boolean PaymentBaseDuplicate(String? Title, String? Number)
        {
            PS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Title != null)
            {
                PS_DTO.Title = Convert.ToString(Title).Trim();
            }
            if (Convert.ToInt32(Number) == 0)
            {
                PS_DTO.Id = 6;
            }
            else
            {
                PS_DTO.Number = Convert.ToInt32(Number);
                PS_DTO.Id = 7;
            }
            DS = PS_DAO.ProcurementSubmasterDB(PS_DTO);
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

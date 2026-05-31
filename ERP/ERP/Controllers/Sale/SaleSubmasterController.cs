using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ERP.Controllers.sale
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class SaleSubmasterController : Controller
    {
        Alerts Alert=new Alerts();
        Help Help = new Help();
        Validation Valid=new Validation();
        DataSet DS = new DataSet();
        List<AddressType_DTO> AT_List = new List<AddressType_DTO>();
        AddressType_DL AT_DL = new AddressType_DL();
        AddressType_DAO AT_DAO = new AddressType_DAO();
        AddressType_DTO AT_DTO = new AddressType_DTO();
        
        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;




        [Route("Sale/submaster/address-type")]
        public IActionResult AddressType(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                List<AddressType_DTO> AT_List = GetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList<AddressType_DTO>.CreateAsync(AT_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("Sale/submaster/address-type")]
        [HttpPost]
        public IActionResult AddressType(AddressType_DTO AT_DTO, Int64 Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                //try
                //{
                    AT_DTO.ADTP_CreatorCode = Convert.ToInt32(UserCode);
                    if (Mode == "Save")
                    {
                        if (ModelState.IsValid)
                        {
                            AT_DTO.ADTP_Id = 6;
                            DS = AT_DAO.AddressTypeDB(AT_DTO);
                            if (DS.Tables[0].Rows.Count == 0)
                            {
                                AT_DTO.ADTP_Id = 1;
                                AT_DAO.AddressTypeDB(AT_DTO);

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
                        AT_DTO.ADTP_DeleteNumbers = DeleteNumbers;
                        AT_DTO.ADTP_Id = 3;
                        AT_DAO.AddressTypeDB(AT_DTO);

                        ModelState.Clear();
                    }
                    else if (Mode == "Delete")
                    {
                        AT_DTO.ADTP_Number = Number;
                        AT_DTO.ADTP_Id = 8;
                        AT_DAO.AddressTypeDB(AT_DTO);

                        ModelState.Clear();
                    }
                    else if (Mode == "Clear")
                    {
                        ModelState.Clear();
                    }
                    else if (Mode == "Edit")
                    {
                        AT_DTO.ADTP_Id = 4;
                        DS = AT_DAO.AddressTypeDB(AT_DTO);
                        ViewBag.Submaster = AT_DL.AddressTList(DS.Tables[0]).FirstOrDefault(); ;
                    }
                    else if (Mode == "Update")
                    {
                        AT_DTO.ADTP_Id = 7;
                        DS = AT_DAO.AddressTypeDB(AT_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            if (ModelState.IsValid)
                            {
                                AT_DTO.ADTP_Id = 5;
                                AT_DAO.AddressTypeDB(AT_DTO);

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
                    List<AddressType_DTO> AT_List = GetData(SortOrder, Search, PageNumber, PSize, PageFilter);

                    return View(PaginatedList<AddressType_DTO>.CreateAsync(AT_List, DPageNumber ?? 1, DPageSize));
                //}
                //catch(Exception ex)
                //{
                //    ViewBag.ErrorCode = 2;
                //    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                //    return View();
                //}
        }
        List<AddressType_DTO> GetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            AT_DTO.ADTP_Id = 2;
            AT_DTO.ADTP_CreatorCode = Convert.ToInt32(UserCode);
            DS = AT_DAO.AddressTypeDB(AT_DTO);
            AT_List = AT_DL.AddressTList(DS.Tables[0]);

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

            var Key = AT_List.OrderByDescending(Cs => Cs.ADTP_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.ADTP_Name!.ToLower().Contains(Search.ToLower()) || K.ADTP_Notes!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.ADTP_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.ADTP_Name!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.ADTP_Name!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.ADTP_Number);
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

        [Route("Sale/submaster/address-type/duplicate")]
        public Boolean AddressTypeDuplicate(String? Title, String? Number)
        {
            AT_DTO.ADTP_CreatorCode = Convert.ToInt32(UserCode);
            if (Title != null)
            {
                AT_DTO.ADTP_Name = Convert.ToString(Title).Trim();
            }
            if (Convert.ToInt32(Number) == 0)
            {
                AT_DTO.ADTP_Id = 6;
            }
            else
            {
                AT_DTO.ADTP_Number = Convert.ToInt32(Number);
                AT_DTO.ADTP_Id = 7;
            }
            DS = AT_DAO.AddressTypeDB(AT_DTO);
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

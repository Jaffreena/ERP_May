using DocumentFormat.OpenXml.Wordprocessing;
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SelectPdf;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace ERP.Controllers.Procurement
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class ProcurementMasterController : Controller
    {
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();
        ProcurementMaster_DL PM_DL = new ProcurementMaster_DL();

        Vendor_DAO V_DAO = new Vendor_DAO();
        Vendor_DTO V_DTO = new Vendor_DTO();
        List<Vendor_DTO> V_List = new List<Vendor_DTO>();

        VendorCategory_DAO VC_DAO = new VendorCategory_DAO();
        VendorCategory_DTO VC_DTO = new VendorCategory_DTO();
        List<VendorCategory_DTO> VC_List = new List<VendorCategory_DTO>();

        VendorGroup_DAO VG_DAO = new VendorGroup_DAO();
        VendorGroup_DTO VG_DTO = new VendorGroup_DTO();
        List<VendorGroup_DTO> VG_List = new List<VendorGroup_DTO>();

        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;




        //Vendor
        [Route("procurement/master/vendor")]
        public IActionResult Vendor(String? SortOrder, String? Search, Int32 PageNumber, Int32 PSize, String? PageFilter)
        {
            List<Vendor_DTO> VList = VGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            var PageList = PaginatedList_DTO<Vendor_DTO>.CreateAsync(VList, DPageNumber ?? 1, DPageSize);

            var Model = new VendorHead_DTO()
            {
                Vendor_List = PageList,
            };
            return View(Model);
        }
        [Route("procurement/master/vendor")]
        [HttpPost]
        public IActionResult Vendor(VendorHead_DTO VH_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32 PageNumber, Int32 PSize, String? PageFilter)
        {
            bool IsValid = false;
            VendorHead_DTO V_Head_DTO = new VendorHead_DTO();
            List<VendorGST_DTO>? GST_DTO = new List<VendorGST_DTO>();
            List<VendorWHT_DTO>? WHT_DTO = new List<VendorWHT_DTO>();

            V_Head_DTO = VH_DTO;

            if (VH_DTO.GST_List != null)
                GST_DTO = VH_DTO.GST_List!.Where(K => !K.IsDeleted).ToList();

            if (VH_DTO.WHT_List != null)
                WHT_DTO = VH_DTO.WHT_List!.Where(K => !K.IsDeleted).ToList();

            V_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                ModelState.Clear();
                IsValid = TryValidateModel(V_Head_DTO);

                if (IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        V_DTO.VendorCode = V_Head_DTO.VendorCode;
                        V_DTO.Id = 1;
                        DS = V_DAO.VendorDB(V_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            V_DTO.VendorCode = VH_DTO.VendorCode;
                            V_DTO.VendorName = VH_DTO.VendorName;
                            V_DTO.Address = VH_DTO.Address;
                            V_DTO.City = VH_DTO.City;
                            V_DTO.State = VH_DTO.State;
                            V_DTO.Country = VH_DTO.Country;
                            V_DTO.Pincode = VH_DTO.Pincode;
                            V_DTO.ContactPerson = VH_DTO.ContactPerson;
                            V_DTO.CP_Telephone = VH_DTO.CP_Telephone;
                            V_DTO.CP_Mobile = VH_DTO.CP_Mobile;
                            V_DTO.CP_Email = VH_DTO.CP_Email;
                            V_DTO.AccountsPerson = VH_DTO.AccountsPerson;
                            V_DTO.AP_Telephone = VH_DTO.AP_Telephone;
                            V_DTO.AP_Mobile = VH_DTO.AP_Mobile;
                            V_DTO.AP_Email = VH_DTO.AP_Email;
                            V_DTO.VendorLocation = Convert.ToString(VH_DTO.VendorLocation);
                            V_DTO.VendorGroup = Convert.ToString(VH_DTO.VendorGroup);
                            V_DTO.VendorCategory = Convert.ToString(VH_DTO.VendorCategory);
                            V_DTO.RegistrationType = Convert.ToInt64(VH_DTO.RegistrationType == null ? 1 : VH_DTO.RegistrationType);
                            V_DTO.GSTIN = VH_DTO.GSTIN;
                            V_DTO.AssesseeTerritory = Convert.ToInt64(VH_DTO.AssesseeTerritory == null ? 1 : VH_DTO.AssesseeTerritory);
                            V_DTO.TransportAgency = Convert.ToInt32(VH_DTO.TransportAgency == null ? 1 : VH_DTO.TransportAgency);
                            V_DTO.TransporterID = VH_DTO.TransporterID;
                            V_DTO.PaymentTerms = VH_DTO.PaymentTerms;
                            V_DTO.PaymentMode = VH_DTO.PaymentMode;
                            V_DTO.CreditDays = Convert.ToInt32(VH_DTO.CreditDays);
                            V_DTO.PaymentBase = Convert.ToInt64(VH_DTO.PaymentBase == null ? 1 : VH_DTO.PaymentBase);
                            V_DTO.Currency = Convert.ToString(VH_DTO.Currency == null ? 1 : VH_DTO.Currency);
                            V_DTO.AccountName = VH_DTO.AccountName;
                            V_DTO.AccountNumber = VH_DTO.AccountNumber;
                            V_DTO.IFSC = VH_DTO.IFSC;
                            V_DTO.BankName = VH_DTO.BankName;
                            V_DTO.PAN = VH_DTO.PAN;
                            V_DTO.WithholdTax = Convert.ToInt64(VH_DTO.WithholdTax == null ? 1 : VH_DTO.WithholdTax);
                            V_DTO.AssesseeNature = Convert.ToInt64(VH_DTO.AssesseeNature == null ? 1 : VH_DTO.AssesseeNature);
                            V_DTO.Id = 2;
                            DS = V_DAO.VendorDB(V_DTO);

                            if (DS.Tables[0].Rows.Count > 0)
                            {
                                V_DTO.VendorNumber = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                foreach (var detail in WHT_DTO)
                                {
                                    V_DTO.Category = Convert.ToInt64(detail.Category);
                                    V_DTO.Type = Convert.ToInt64(detail.Type);
                                    V_DTO.Tax = Convert.ToInt64(detail.Tax);
                                    V_DTO.FromDate = Convert.ToInt32(Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd"));
                                    V_DTO.ToDate = Convert.ToInt32(Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd"));
                                    V_DTO.Id = 3;
                                    V_DAO.VendorDB(V_DTO);
                                }
                                foreach (var detail in GST_DTO)
                                {
                                    V_DTO.Category = Convert.ToInt64(detail.Category);
                                    V_DTO.Type = Convert.ToInt64(detail.Type);
                                    V_DTO.Tax = Convert.ToInt64(detail.TaxNumber);
                                    V_DTO.FromDate = Convert.ToInt32(Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd"));
                                    V_DTO.ToDate = Convert.ToInt32(Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd"));
                                    V_DTO.Id = 4;
                                    V_DAO.VendorDB(V_DTO);
                                }
                                transaction.Complete();

                                VH_DTO.Reset();
                                V_Head_DTO.Reset();
                                GST_DTO = VH_DTO.GST_List!;
                                WHT_DTO = VH_DTO.WHT_List!;
                                V_DTO.Reset();
                                ModelState.Clear();
                            }
                            else
                            {
                                transaction.Dispose();
                                ViewBag.ErrorMessage = "Failed to insert Vendor Unit";
                                ViewBag.ErrorCode = 2;
                            }
                        }
                        else
                        {
                            transaction.Dispose();
                            ViewBag.ErrorMessage = "Already assigned. Please check";
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
            else if (Mode == "DeleteAll")
            {
                try
                {
                    V_DTO.DeleteNumbers = DeleteNumbers;
                    V_DTO.Id = 23;
                    V_DAO.VendorDB(V_DTO);

                    VH_DTO.Reset();
                    V_Head_DTO.Reset();
                    GST_DTO = VH_DTO.GST_List!;
                    WHT_DTO = VH_DTO.WHT_List!;
                    V_DTO.Reset();
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
                    V_DTO.VendorNumber = Convert.ToInt64(Number);
                    V_DTO.Id = 22;
                    V_DAO.VendorDB(V_DTO);

                    VH_DTO.Reset();
                    V_Head_DTO.Reset();
                    GST_DTO = VH_DTO.GST_List!;
                    WHT_DTO = VH_DTO.WHT_List!;
                    V_DTO.Reset();
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
                VH_DTO.Reset();
                V_Head_DTO.Reset();
                GST_DTO = VH_DTO.GST_List!;
                WHT_DTO = VH_DTO.WHT_List!;
                V_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                V_DTO.VendorNumber = Convert.ToInt64(VH_DTO.VendorNumber);
                V_DTO.Id = 11;
                DS = V_DAO.VendorDB(V_DTO);
                V_Head_DTO = PM_DL.VHList(DS.Tables[0]).FirstOrDefault();
                WHT_DTO = PM_DL.VWHTList(DS.Tables[1]);
                GST_DTO = PM_DL.VGSTList(DS.Tables[2]);
            }
            else if (Mode == "Update")
            {
                ModelState.Clear();
                IsValid = TryValidateModel(V_Head_DTO);
                if (IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            String GSTCheck = string.Join(", ", GST_DTO.Where(x => Convert.ToInt64(x.Number) != 0).Select(x => x.Number));
                            String WHTCheck = string.Join(", ", WHT_DTO.Where(x => Convert.ToInt64(x.Number) != 0).Select(x => x.Number));

                            V_DTO.VendorNumber = VH_DTO.VendorNumber;
                            V_DTO.DeleteNumbers = GSTCheck;
                            V_DTO.Id = 12;
                            V_DAO.VendorDB(V_DTO);

                            V_DTO.VendorNumber = VH_DTO.VendorNumber;
                            V_DTO.DeleteNumbers = WHTCheck;
                            V_DTO.Id = 13;
                            V_DAO.VendorDB(V_DTO);

                            V_DTO.VendorNumber = VH_DTO.VendorNumber;
                            V_DTO.VendorCode = VH_DTO.VendorCode;
                            V_DTO.VendorName = VH_DTO.VendorName;
                            V_DTO.Address = VH_DTO.Address;
                            V_DTO.City = VH_DTO.City;
                            V_DTO.State = VH_DTO.State;
                            V_DTO.Country = VH_DTO.Country;
                            V_DTO.Pincode = VH_DTO.Pincode;
                            V_DTO.ContactPerson = VH_DTO.ContactPerson;
                            V_DTO.CP_Telephone = VH_DTO.CP_Telephone;
                            V_DTO.CP_Mobile = VH_DTO.CP_Mobile;
                            V_DTO.CP_Email = VH_DTO.CP_Email;
                            V_DTO.AccountsPerson = VH_DTO.AccountsPerson;
                            V_DTO.AP_Telephone = VH_DTO.AP_Telephone;
                            V_DTO.AP_Mobile = VH_DTO.AP_Mobile;
                            V_DTO.AP_Email = VH_DTO.AP_Email;
                            V_DTO.VendorLocation = Convert.ToString(VH_DTO.VendorLocation);
                            V_DTO.VendorGroup = Convert.ToString(VH_DTO.VendorGroup);
                            V_DTO.VendorCategory = Convert.ToString(VH_DTO.VendorCategory);
                            V_DTO.RegistrationType = Convert.ToInt64(VH_DTO.RegistrationType == null ? 1 : VH_DTO.RegistrationType);
                            V_DTO.GSTIN = VH_DTO.GSTIN;
                            V_DTO.AssesseeTerritory = Convert.ToInt64(VH_DTO.AssesseeTerritory == null ? 1 : VH_DTO.AssesseeTerritory);
                            V_DTO.TransportAgency = Convert.ToInt32(VH_DTO.TransportAgency == null ? 1 : VH_DTO.TransportAgency);
                            V_DTO.TransporterID = VH_DTO.TransporterID;
                            V_DTO.PaymentTerms = VH_DTO.PaymentTerms;
                            V_DTO.PaymentMode = VH_DTO.PaymentMode;
                            V_DTO.CreditDays = Convert.ToInt32(VH_DTO.CreditDays);
                            V_DTO.PaymentBase = Convert.ToInt64(VH_DTO.PaymentBase == null ? 1 : VH_DTO.PaymentBase);
                            V_DTO.Currency = Convert.ToString(VH_DTO.Currency == null ? 1 : VH_DTO.Currency);
                            V_DTO.AccountName = VH_DTO.AccountName;
                            V_DTO.AccountNumber = VH_DTO.AccountNumber;
                            V_DTO.IFSC = VH_DTO.IFSC;
                            V_DTO.BankName = VH_DTO.BankName;
                            V_DTO.PAN = VH_DTO.PAN;
                            V_DTO.WithholdTax = Convert.ToInt64(VH_DTO.WithholdTax == null ? 1 : VH_DTO.WithholdTax);
                            V_DTO.AssesseeNature = Convert.ToInt64(VH_DTO.AssesseeNature == null ? 1 : VH_DTO.AssesseeNature);
                            V_DTO.Id = 14;
                            DS = V_DAO.VendorDB(V_DTO);


                            foreach (var detail in WHT_DTO)
                            {
                                V_DTO.Category = Convert.ToInt64(detail.Category);
                                V_DTO.Type = Convert.ToInt64(detail.Type);
                                V_DTO.Tax = Convert.ToInt64(detail.Tax);
                                V_DTO.FromDate = Convert.ToInt32(Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd"));
                                V_DTO.ToDate = Convert.ToInt32(Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd"));
                                if (detail.Number == 0)
                                {
                                    V_DTO.Id = 3;
                                }
                                else
                                {
                                    V_DTO.TaxNumber = detail.Number;
                                    V_DTO.Id = 15;
                                }
                                V_DAO.VendorDB(V_DTO);
                            }
                            foreach (var detail in GST_DTO)
                            {
                                V_DTO.Category = Convert.ToInt64(detail.Category);
                                V_DTO.Type = Convert.ToInt64(detail.Type);
                                V_DTO.Tax = Convert.ToInt64(detail.TaxNumber);
                                V_DTO.FromDate = Convert.ToInt32(Convert.ToDateTime(detail.FromDate).ToString("yyyyMMdd"));
                                V_DTO.ToDate = Convert.ToInt32(Convert.ToDateTime(detail.ToDate).ToString("yyyyMMdd"));
                                if (detail.Number == 0)
                                {
                                    V_DTO.Id = 4;
                                }
                                else
                                {
                                    V_DTO.TaxNumber = detail.Number;
                                    V_DTO.Id = 16;
                                }
                                V_DAO.VendorDB(V_DTO);
                            }
                            transaction.Complete();

                            VH_DTO.Reset();
                            V_Head_DTO.Reset();
                            GST_DTO = VH_DTO.GST_List!;
                            WHT_DTO = VH_DTO.WHT_List!;
                            V_DTO.Reset();
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
                }
            }

            List<Vendor_DTO> VList = VGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            var PageList = PaginatedList_DTO<Vendor_DTO>.CreateAsync(VList, DPageNumber ?? 1, DPageSize);

            V_Head_DTO.Vendor_List = PageList;
            V_Head_DTO.GST_List = GST_DTO;
            V_Head_DTO.WHT_List = WHT_DTO;
            return View(V_Head_DTO);
        }
        List<Vendor_DTO> VGetData(String? SortOrder, String? Search, Int32 PageNumber, Int32 PSize, String? PageFilter)
        {
            //DPageSize = 10;

            V_DTO.Id = 21;
            V_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = V_DAO.VendorDB(V_DTO);
            V_List = PM_DL.VList(DS.Tables[0]);

            ViewBag.VendorGroup = Help.GetCat(DS.Tables[1]);
            ViewBag.VendorCategory = Help.GetCat(DS.Tables[2]);
            ViewBag.Registration = Help.GetCat(DS.Tables[3]);
            ViewBag.PaymentBase = Help.GetCat(DS.Tables[4]);
            ViewBag.Currency = Help.GetCat(DS.Tables[5]);
            ViewBag.Transporter = Help.GetCat(DS.Tables[6]);
            ViewBag.NatureOfAssessee = Help.GetCat(DS.Tables[7]);
            ViewBag.WithHoldTax = Help.GetUnder(DS.Tables[8]);

            ViewBag.WHT_Category = Help.GetCat(DS.Tables[9]);
            ViewBag.WHT_Type = Help.GetCat(DS.Tables[10]);
            ViewBag.WHT_Tax = Help.GetCat(DS.Tables[11]);

            ViewBag.GST_Category = Help.GetCat(DS.Tables[12]);
            ViewBag.GST_Type = Help.GetCat(DS.Tables[13]);
            ViewBag.GST_Tax = Help.GetCat(DS.Tables[14]);

            ViewBag.Location = Help.GetUnder(DS.Tables[15]);


            //if (String.IsNullOrEmpty(SortOrder))
            //{
            //    SortOrder = "Code";
            //}
            //if (Convert.ToInt32(PageNumber) == 0)
            //{
            //    DPageNumber = 1;
            //}
            //if (PageFilter?.ToLower() == "PageFilter".ToLower())
            //{
            //    DPageNumber = 1;
            //}

            //ViewData["CurrentSort"] = SortOrder;
            ////ViewData["KeySort"] = SortOrder == "Title" ? "Title_desc" : "Title";

            //ViewData["CurrentFilter"] = Search;

            var Key = V_List.OrderByDescending(cs => cs.VendorNumber);

            //if (!string.IsNullOrEmpty(FilterVendorCode))
            //    Key = Key.Where(v => v.VendorCode!.Contains(FilterVendorCode));

            //if (!string.IsNullOrEmpty(FilterVendorName))
            //    Key = Key.Where(v => v.VendorName!.Contains(FilterVendorName));

            //if (!string.IsNullOrEmpty(FilterLocation))
            //    Key = Key.Where(v => v.VendorLocation!.Contains(FilterLocation));

            // Only when you really need IQueryable for an API or further LINQ extensions:
            IQueryable<Vendor_DTO> Query = Key.AsQueryable();

            var SortMap = new Dictionary<string, Expression<Func<Vendor_DTO, object>>>
            {
                ["Code"] = v => v.VendorCode!,
                ["Name"] = v => v.VendorName!,
                ["Location"] = v => v.VendorLocation!,
                ["Currency"] = v => v.Currency!,
                ["Group"] = v => v.VendorGroup!,
                ["Category"] = v => v.VendorCategory!
            };
            bool Desc = SortOrder?.EndsWith("_desc") == true;
            string SortKey = (SortOrder ?? "Code").Replace("_desc", "");
            if (!SortMap.ContainsKey(SortKey)) SortKey = "Code";

            Query = Desc == true ? Query.OrderByDescending(SortMap[SortKey]) : Query.OrderBy(SortMap[SortKey]);

            int TotalRecords = Key.Count();
            int PageSize = PSize <= 0 ? 10 : PSize;
            int TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);

            PageNumber = PageNumber < 1 ? 1 : PageNumber > TotalPages ? TotalPages : PageNumber;
            var Paged = Key.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            //if (PSize != 0)
            //{
            //    DPageSize = PSize;
            //}
            //Int32 Record = Key.ToList().Count;
            //if (PageNumber > 1)
            //{
            //    Int32 RecordPage = (Convert.ToInt32(PageNumber) - 1) * DPageSize;

            //    if (Record > RecordPage)
            //    {
            //        DPageNumber = Convert.ToInt32(PageNumber) == 0 ? 1 : Convert.ToInt32(PageNumber);
            //    }
            //    else
            //    {
            //        Int32 PageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Record) / Convert.ToDouble(DPageSize)));

            //        DPageNumber = PageNumber > PageCount ? Convert.ToInt32(PageCount) : Convert.ToInt32(PageNumber);
            //    }
            //}
            //else
            //{
            //    DPageNumber = Convert.ToInt32(PageNumber) == 0 ? 1 : Convert.ToInt32(PageNumber);
            //}

            //Int32 PageCounts = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Record) / Convert.ToDouble(DPageSize)));
            ViewData["CurrentSort"] = SortOrder;
            ViewData["PageNumber"] = PageNumber;
            ViewData["PageSize"] = PageSize;
            ViewData["PageCount"] = TotalPages;
            ViewData["TotalSize"] = TotalRecords;

            DPageNumber = PageNumber;
            DPageSize = PageSize;

            ViewBag.Page = Help.PageSize(PSize.ToString());
            //ViewData["PageNumber"] = DPageNumber;
            //ViewData["PageSize"] = PageSize;
            //ViewData["PageCount"] = TotalPages;
            //ViewData["TotalSize"] = Key.ToList().Count;

            //return Query.ToList();
            return V_List;
        }

        [Route("procurement/master/vendor/duplicate")]
        public Boolean VendorDuplicate(String? Title, String? Number)
        {
            V_DTO.CreatorCode = Convert.ToInt32(UserCode);
            V_DTO.VendorCode = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                V_DTO.Id = 5;
            }
            else
            {
                V_DTO.VendorNumber = Convert.ToInt32(Number);
                V_DTO.Id = 6;
            }
            DS = V_DAO.VendorDB(V_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [Route("procurement/master/vendor/assessee")]
        public IActionResult VendorAssessee(String? Assessee, String? Location)
        {
            if (Assessee == null)
            {
                Assessee = "";
            }
            V_DTO.CreatorCode = Convert.ToInt32(UserCode);
            V_DTO.VendorCode = Assessee;
            V_DTO.VendorLocation = Location;
            V_DTO.Id = 8;
            DS = V_DAO.VendorDB(V_DTO);

            var Category = PM_DL.VendorTaxCategory(DS.Tables[0]);
            return Json(Category);
        }

        [Route("procurement/master/vendor/cluster")]
        public IActionResult PurchaseItem(String? Tax, String? Category, String? Type)
        {
            if (Tax == null)
            {
                Tax = "";
            }

            V_DTO.CreatorCode = Convert.ToInt32(UserCode);
            V_DTO.VendorName = Convert.ToString(Tax);
            V_DTO.Category = Convert.ToInt32(Category);
            V_DTO.Type = Convert.ToInt32(Type);
            V_DTO.Id = 7;
            DS = V_DAO.VendorDB(V_DTO);

            var Cluster = PM_DL.VendorCluster(DS.Tables[0]);
            return Json(Cluster);
        }



        [Route("procurement/master/vendor-category")]
        public IActionResult VendorCategory(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            List<VendorCategory_DTO> VC_List = VCGetData(SortOrder, Search, PageNumber, PSize, PageFilter, VC_DTO.VendorCategoryNumber);
            return View(PaginatedList<VendorCategory_DTO>.CreateAsync(VC_List, DPageNumber ?? 1, DPageSize));

        }

        [Route("procurement/master/vendor-category")]
        [HttpPost]
        public IActionResult VendorCategory(VendorCategory_DTO VC_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                VC_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        VC_DTO.Id = 6;
                        DS = VC_DAO.VendorCategoryDB(VC_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            VC_DTO.Id = 1;
                            VC_DAO.VendorCategoryDB(VC_DTO);

                            VC_DTO.Reset();
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
                        VC_DTO.DeleteNumbers = DeleteNumbers;
                        VC_DTO.Id = 3;
                        VC_DAO.VendorCategoryDB(VC_DTO);

                        VC_DTO.Reset();
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
                        VC_DTO.VendorCategoryNumber = Number;
                        VC_DTO.Id = 8;
                        VC_DAO.VendorCategoryDB(VC_DTO);

                        VC_DTO.Reset();
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
                    VC_DTO.Reset();
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    VC_DTO.Id = 4;
                    DS = VC_DAO.VendorCategoryDB(VC_DTO);
                    ViewBag.VendorCategoryEdit = PM_DL.VCList(DS.Tables[0]).FirstOrDefault();
                }
                else if (Mode == "Update")
                {
                    VC_DTO.Id = 7;
                    DS = VC_DAO.VendorCategoryDB(VC_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            VC_DTO.Id = 5;
                            VC_DAO.VendorCategoryDB(VC_DTO);

                            VC_DTO.Reset();
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

                List<VendorCategory_DTO> VC_List = VCGetData(SortOrder, Search, PageNumber, PSize, PageFilter, VC_DTO.VendorCategoryNumber);
                return View(PaginatedList<VendorCategory_DTO>.CreateAsync(VC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }

        }
        List<VendorCategory_DTO> VCGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                VC_DTO.VendorCategoryNumber = Convert.ToInt64(Number);
            }

            VC_DTO.Id = 2;
            VC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = VC_DAO.VendorCategoryDB(VC_DTO);
            VC_List = PM_DL.VCList(DS.Tables[0]);
            ViewBag.Under = Help.GetUnder(DS.Tables[1]);

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

            var Key = VC_List.OrderByDescending(Cs => Cs.VendorCategoryNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.VendorCategory!.ToLower().Contains(Search.ToLower()) || K.VC_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.VendorCategoryNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.VendorCategory!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.VendorCategory!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.VendorCategoryNumber);
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



        [Route("procurement/master/vendor-category/duplicate")]
        public Boolean VendorCategoryDuplicate(String? Title, String? Number)
        {
            VC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            VC_DTO.VendorCategory = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                VC_DTO.Id = 6;
            }
            else
            {
                VC_DTO.VendorCategoryNumber = Convert.ToInt32(Number);
                VC_DTO.Id = 7;
            }
            DS = VC_DAO.VendorCategoryDB(VC_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        [Route("procurement/master/vendor-group")]
        public IActionResult VendorGroup(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            List<VendorGroup_DTO> VG_List = VGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, VG_DTO.VendorGroupNumber);
            return View(PaginatedList<VendorGroup_DTO>.CreateAsync(VG_List, DPageNumber ?? 1, DPageSize));

        }

        [Route("procurement/master/vendor-group")]
        [HttpPost]
        public IActionResult VendorGroup(VendorGroup_DTO VG_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            try
            {
                VG_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        VG_DTO.Id = 6;
                        DS = VG_DAO.VendorGroupDB(VG_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            VG_DTO.Id = 1;
                            VG_DAO.VendorGroupDB(VG_DTO);

                            VG_DTO.Reset();
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
                        VG_DTO.DeleteNumbers = DeleteNumbers;
                        VG_DTO.Id = 3;
                        VG_DAO.VendorGroupDB(VG_DTO);

                        VG_DTO.Reset();
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
                        VG_DTO.VendorGroupNumber = Number;
                        VG_DTO.Id = 8;
                        VG_DAO.VendorGroupDB(VG_DTO);

                        VG_DTO.Reset();
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
                    VG_DTO.Reset();
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    VG_DTO.Id = 4;
                    DS = VG_DAO.VendorGroupDB(VG_DTO);
                    ViewBag.VendorGroupEdit = PM_DL.VGList(DS.Tables[0]).FirstOrDefault();
                }
                else if (Mode == "Update")
                {
                    VG_DTO.Id = 7;
                    DS = VG_DAO.VendorGroupDB(VG_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            VG_DTO.Id = 5;
                            VG_DAO.VendorGroupDB(VG_DTO);

                            VG_DTO.Reset();
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


                List<VendorGroup_DTO> VG_List = VGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, VG_DTO.VendorGroupNumber);
                return View(PaginatedList<VendorGroup_DTO>.CreateAsync(VG_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }

        }
        List<VendorGroup_DTO> VGGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                VG_DTO.VendorGroupNumber = Convert.ToInt64(Number);
            }

            VG_DTO.Id = 2;
            VG_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = VG_DAO.VendorGroupDB(VG_DTO);
            VG_List = PM_DL.VGList(DS.Tables[0]);
            ViewBag.Under = Help.GetUnder(DS.Tables[1]);

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

            var Key = VG_List.OrderByDescending(Cs => Cs.VendorGroupNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.VendorGroup!.ToLower().Contains(Search.ToLower()) || K.VG_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.VendorGroupNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.VendorGroup!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.VendorGroup!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.VendorGroupNumber);
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



        [Route("procurement/master/vendor-group/duplicate")]
        public Boolean VendorGroupDuplicate(String? Title, String? Number)
        {
            VG_DTO.CreatorCode = Convert.ToInt32(UserCode);
            VG_DTO.VendorGroup = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                VG_DTO.Id = 6;
            }
            else
            {
                VG_DTO.VendorGroupNumber = Convert.ToInt32(Number);
                VG_DTO.Id = 7;
            }
            DS = VG_DAO.VendorGroupDB(VG_DTO);
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

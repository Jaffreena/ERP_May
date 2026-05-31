using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Transactions;

namespace ERP.Controllers.Inventory
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class InventoryMasterController : Controller
    {
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();
        InventoryMaster_DL IM_DL = new InventoryMaster_DL();

        Item_DAO I_DAO = new Item_DAO();
        Item_DTO I_DTO = new Item_DTO();
        List<Item_DTO> I_List = new List<Item_DTO>();

        ItemCategory_DAO IC_DAO = new ItemCategory_DAO();
        ItemCategory_DTO IC_DTO = new ItemCategory_DTO();
        List<ItemCategory_DTO> IC_List = new List<ItemCategory_DTO>();

        ItemSubcategory_DAO ISC_DAO = new ItemSubcategory_DAO();
        ItemSubcategory_DTO ISC_DTO = new ItemSubcategory_DTO();
        List<ItemSubcategory_DTO> ISC_List = new List<ItemSubcategory_DTO>();

        UoM_DAO U_DAO = new UoM_DAO();
        UoM_DTO U_DTO = new UoM_DTO();
        List<UoM_DTO> U_List = new List<UoM_DTO>();

        Warehouse_DAO W_DAO = new Warehouse_DAO();
        Warehouse_DTO W_DTO = new Warehouse_DTO();
        List<Warehouse_DTO> W_List = new List<Warehouse_DTO>();

        ItemGroup_DAO IG_DAO = new ItemGroup_DAO();
        ItemGroup_DTO IG_DTO = new ItemGroup_DTO();
        List<ItemGroup_DTO> IG_List = new List<ItemGroup_DTO>();

        ExpenseCode_DAO EC_DAO = new ExpenseCode_DAO();
        ExpenseCode_DTO EC_DTO = new ExpenseCode_DTO();
        List<ExpenseCode_DTO> EC_List = new List<ExpenseCode_DTO>();

        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;




        //Item
        [Route("inventory/master/item")]
        public IActionResult Item(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<Item_DTO> IList = IGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            var PageList = PaginatedList_DTO<Item_DTO>.CreateAsync(IList, DPageNumber ?? 1, DPageSize);

            var Model = new ItemHead_DTO()
            {
                Item_List = PageList,
            };
            return View(Model);
        }
        [Route("inventory/master/item")]
        [HttpPost]
        public IActionResult Item(ItemHead_DTO IH_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            bool IsValid = false;
            ItemHead_DTO I_Head_DTO = new ItemHead_DTO();
            List<ItemDetail_DTO>? ID_DTO = new List<ItemDetail_DTO>();

            I_Head_DTO = IH_DTO;

            if (IH_DTO.ItemDetail != null)
                ID_DTO = IH_DTO.ItemDetail!.Where(K => !K.IsDeleted).ToList();

            I_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                ModelState.Clear();
                IsValid = TryValidateModel(I_Head_DTO);

                if (IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        I_DTO.ItemCode = IH_DTO.ItemCode;
                        I_DTO.Id = 1;
                        DS = I_DAO.ItemDB(I_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            I_DTO.ItemCode = IH_DTO.ItemCode;
                            I_DTO.ItemDescription = IH_DTO.ItemDescription;
                            I_DTO.ItemPartNumber = IH_DTO.ItemPartNumber;
                            I_DTO.InnerDia = IH_DTO.InnerDia;
                            I_DTO.OuterDia = IH_DTO.OuterDia;
                            I_DTO.Thickness = IH_DTO.Thickness;
                            I_DTO.Length = IH_DTO.Length;
                            I_DTO.Spec = IH_DTO.Spec;
                            I_DTO.MaterialGrade = IH_DTO.MaterialGrade;

                            I_DTO.ItemGroup = Convert.ToString(IH_DTO.ItemGroup);
                            I_DTO.ItemCategory = Convert.ToString(IH_DTO.ItemCategory == null ? 1 : IH_DTO.ItemCategory);
                            I_DTO.ItemSubCategory = Convert.ToString(IH_DTO.ItemSubCategory == null ? 1 : IH_DTO.ItemSubCategory);
                            I_DTO.MaterialSegregation = Convert.ToString(IH_DTO.MaterialSegregation);
                            I_DTO.PurchaseWarehouse = Convert.ToInt64(IH_DTO.PurchaseWarehouse == null ? 1 : IH_DTO.PurchaseWarehouse);
                            I_DTO.SaleWarehouse = Convert.ToInt64(IH_DTO.SaleWarehouse == null ? 1 : IH_DTO.SaleWarehouse);
                            I_DTO.HSN_Code = Convert.ToString(IH_DTO.HSN_Code == null ? 1 : IH_DTO.HSN_Code);
                            I_DTO.UoM = Convert.ToString(IH_DTO.UoM);
                            I_DTO.PurchaseUnit = Convert.ToInt64(IH_DTO.PurchaseUnit == null ? 1 : IH_DTO.PurchaseUnit);
                            I_DTO.ProductionUnit = Convert.ToInt64(IH_DTO.ProductionUnit == null ? 1 : IH_DTO.ProductionUnit);
                            I_DTO.SaleUnit = Convert.ToInt64(IH_DTO.SaleUnit == null ? 1 : IH_DTO.SaleUnit);
                            I_DTO.Id = 2;
                            DS = I_DAO.ItemDB(I_DTO);

                            if (DS.Tables[0].Rows.Count > 0)
                            {
                                I_DTO.ItemNumber = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                foreach (var detail in ID_DTO)
                                {
                                    I_DTO.FromQty = Convert.ToDouble(detail.FromQty);
                                    I_DTO.FromUnit = Convert.ToInt64(detail.FromUnit);
                                    I_DTO.ToQty = Convert.ToDouble(detail.ToQty);
                                    I_DTO.ToUnit = Convert.ToInt64(detail.ToUnit);
                                    I_DTO.Id = 3;
                                    I_DAO.ItemDB(I_DTO);
                                }
                                transaction.Complete();

                                IH_DTO.Reset();
                                ID_DTO = IH_DTO.ItemDetail!;
                                I_DTO.Reset();
                                ModelState.Clear();
                            }
                            else
                            {
                                transaction.Dispose();
                                ViewBag.ErrorMessage = "Failed to insert Item";
                                ViewBag.ErrorCode = 2;
                            }
                        }
                        else
                        {
                            transaction.Dispose();
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
            else if (Mode == "DeleteAll")
            {
                try
                {
                    I_DTO.DeleteNumbers = DeleteNumbers;
                    I_DTO.Id = 23;
                    I_DAO.ItemDB(I_DTO);

                    IH_DTO.Reset();
                    ID_DTO = IH_DTO.ItemDetail!;
                    I_DTO.Reset();
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
                    I_DTO.ItemNumber = Convert.ToInt64(Number);
                    I_DTO.Id = 22;
                    I_DAO.ItemDB(I_DTO);

                    IH_DTO.Reset();
                    ID_DTO = IH_DTO.ItemDetail!;
                    I_DTO.Reset();
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
                IH_DTO.Reset();
                ID_DTO = IH_DTO.ItemDetail!;
                I_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                I_DTO.ItemNumber = Convert.ToInt64(IH_DTO.ItemNumber);
                I_DTO.Id = 11;
                DS = I_DAO.ItemDB(I_DTO);
                I_Head_DTO = IM_DL.IEList(DS.Tables[0]).FirstOrDefault();
                ID_DTO = IM_DL.IDList(DS.Tables[1]);
            }
            else if (Mode == "Update")
            {
                ModelState.Clear();
                IsValid = TryValidateModel(I_Head_DTO);
                if (IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            String Check = string.Join(", ", ID_DTO.Where(x => Convert.ToInt64(x.UnitNumber) != 0).Select(x => x.UnitNumber));

                            I_DTO.ItemNumber = IH_DTO.ItemNumber;
                            I_DTO.DeleteNumbers = Check;
                            I_DTO.Id = 12;
                            I_DAO.ItemDB(I_DTO);

                            I_DTO.ItemNumber = Convert.ToInt64(IH_DTO.ItemNumber);
                            I_DTO.ItemCode = IH_DTO.ItemCode;
                            I_DTO.ItemDescription = IH_DTO.ItemDescription;
                            I_DTO.ItemPartNumber = IH_DTO.ItemPartNumber;
                            I_DTO.InnerDia = IH_DTO.InnerDia;
                            I_DTO.OuterDia = IH_DTO.OuterDia;
                            I_DTO.Thickness = IH_DTO.Thickness;
                            I_DTO.Length = IH_DTO.Length;
                            I_DTO.Spec = IH_DTO.Spec;
                            I_DTO.MaterialGrade = IH_DTO.MaterialGrade;
                            I_DTO.ItemGroup = Convert.ToString(IH_DTO.ItemGroup);
                            I_DTO.ItemCategory = Convert.ToString(IH_DTO.ItemCategory == null ? 1 : IH_DTO.ItemCategory);
                            I_DTO.ItemSubCategory = Convert.ToString(IH_DTO.ItemSubCategory == null ? 1 : IH_DTO.ItemSubCategory);
                            I_DTO.MaterialSegregation = Convert.ToString(IH_DTO.MaterialSegregation);
                            I_DTO.PurchaseWarehouse = Convert.ToInt64(IH_DTO.PurchaseWarehouse == null ? 1 : IH_DTO.PurchaseWarehouse);
                            I_DTO.SaleWarehouse = Convert.ToInt64(IH_DTO.SaleWarehouse == null ? 1 : IH_DTO.SaleWarehouse);
                            I_DTO.HSN_Code = Convert.ToString(IH_DTO.HSN_Code == null ? 1 : IH_DTO.HSN_Code);
                            I_DTO.UoM = Convert.ToString(IH_DTO.UoM);
                            I_DTO.PurchaseUnit = Convert.ToInt64(IH_DTO.PurchaseUnit == null ? 1 : IH_DTO.PurchaseUnit);
                            I_DTO.ProductionUnit = Convert.ToInt64(IH_DTO.ProductionUnit == null ? 1 : IH_DTO.ProductionUnit);
                            I_DTO.SaleUnit = Convert.ToInt64(IH_DTO.SaleUnit == null ? 1 : IH_DTO.SaleUnit);
                            I_DTO.Id = 13;
                            DS = I_DAO.ItemDB(I_DTO);

                            foreach (var detail in ID_DTO)
                            {
                                I_DTO.FromQty = Convert.ToDouble(detail.FromQty);
                                I_DTO.FromUnit = Convert.ToInt64(detail.FromUnit);
                                I_DTO.ToQty = Convert.ToDouble(detail.ToQty);
                                I_DTO.ToUnit = Convert.ToInt64(detail.ToUnit);
                                if (detail.UnitNumber == 0)
                                {
                                    I_DTO.Id = 3;
                                }
                                else
                                {
                                    I_DTO.UnitNumber = detail.UnitNumber;
                                    I_DTO.Id = 14;
                                }
                                I_DAO.ItemDB(I_DTO);
                            }
                            transaction.Complete();

                            IH_DTO.Reset();
                            ID_DTO = IH_DTO.ItemDetail!;
                            I_DTO.Reset();
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

            List<Item_DTO> IList = IGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            var PageList = PaginatedList_DTO<Item_DTO>.CreateAsync(IList, DPageNumber ?? 1, DPageSize);

            I_Head_DTO.Item_List = PageList;
            I_Head_DTO.ItemDetail = ID_DTO;
            return View(I_Head_DTO);
        }
        List<Item_DTO> IGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            I_DTO.Id = 21;
            I_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = I_DAO.ItemDB(I_DTO);
            I_List = IM_DL.IList(DS.Tables[0]);
            ViewBag.ItemGroup = Help.GetCat(DS.Tables[1]);
            ViewBag.ItemCategory = Help.GetCat(DS.Tables[2]);
            ViewBag.ItemSubCategory = Help.GetCat(DS.Tables[3]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[4]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[5]);
            ViewBag.HSN = Help.GetCat(DS.Tables[6]);
            ViewBag.UOM = Help.GetCat(DS.Tables[7]);

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

            var Key = I_List.OrderByDescending(Cs => Cs.ItemNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.ItemCode!.ToLower().Contains(Search.ToLower()) || K.ItemDescription!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.ItemNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.ItemCode!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.ItemCode!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.ItemNumber);
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

        [Route("inventory/master/item/duplicate")]
        public Boolean ItemDuplicate(String? Title, String? Number)
        {
            I_DTO.CreatorCode = Convert.ToInt32(UserCode);
            I_DTO.ItemCode = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                I_DTO.Id = 4;
            }
            else
            {
                I_DTO.ItemNumber = Convert.ToInt32(Number);
                I_DTO.Id = 5;
            }
            DS = I_DAO.ItemDB(I_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //Item category
        [Route("inventory/master/item-category")]
        public IActionResult ItemCategory(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<ItemCategory_DTO> IC_List = ICGetData(SortOrder, Search, PageNumber, PSize, PageFilter, IC_DTO.ItemCategoryNumber);
            return View(PaginatedList<ItemCategory_DTO>.CreateAsync(IC_List, DPageNumber ?? 1, DPageSize));

        }

        [Route("inventory/master/item-category")]
        [HttpPost]
        public IActionResult ItemCategory(ItemCategory_DTO IC_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            IC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    IC_DTO.Id = 6;
                    DS = IC_DAO.ItemCategoryDB(IC_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        IC_DTO.Id = 1;
                        IC_DAO.ItemCategoryDB(IC_DTO);

                        IC_DTO.Reset();
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
                    IC_DTO.DeleteNumbers = DeleteNumbers;
                    IC_DTO.Id = 3;
                    IC_DAO.ItemCategoryDB(IC_DTO);

                    IC_DTO.Reset();
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
                    IC_DTO.ItemCategoryNumber = Number;
                    IC_DTO.Id = 8;
                    IC_DAO.ItemCategoryDB(IC_DTO);

                    IC_DTO.Reset();
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
                IC_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                IC_DTO.Id = 4;
                DS = IC_DAO.ItemCategoryDB(IC_DTO);
                ViewBag.ItemCategoryEdit = IM_DL.ICList(DS.Tables[0]).FirstOrDefault();
            }
            else if (Mode == "Update")
            {
                IC_DTO.Id = 7;
                DS = IC_DAO.ItemCategoryDB(IC_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        IC_DTO.Id = 5;
                        IC_DAO.ItemCategoryDB(IC_DTO);

                        IC_DTO.Reset();
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


            List<ItemCategory_DTO> IC_List = ICGetData(SortOrder, Search, PageNumber, PSize, PageFilter, IC_DTO.ItemCategoryNumber);
            return View(PaginatedList<ItemCategory_DTO>.CreateAsync(IC_List, DPageNumber ?? 1, DPageSize));
        }
        List<ItemCategory_DTO> ICGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                IC_DTO.ItemCategoryNumber = Convert.ToInt64(Number);
            }

            IC_DTO.Id = 2;
            IC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = IC_DAO.ItemCategoryDB(IC_DTO);
            IC_List = IM_DL.ICList(DS.Tables[0]);
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

            var Key = IC_List.OrderByDescending(Cs => Cs.ItemCategoryNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.ItemCategory!.ToLower().Contains(Search.ToLower()) || K.IC_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.ItemCategoryNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.ItemCategory!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.ItemCategory!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.ItemCategoryNumber);
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



        [Route("inventory/master/item-category/duplicate")]
        public Boolean ItemCategoryDuplicate(String? Title, String? Number)
        {
            IC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            IC_DTO.ItemCategory = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                IC_DTO.Id = 6;
            }
            else
            {
                IC_DTO.ItemCategoryNumber = Convert.ToInt32(Number);
                IC_DTO.Id = 7;
            }
            DS = IC_DAO.ItemCategoryDB(IC_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //Item Subcategory
        [Route("inventory/master/item-subcategory")]
        public IActionResult ItemSubcategory(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<ItemSubcategory_DTO> ISC_List = ISGetData(SortOrder, Search, PageNumber, PSize, PageFilter, ISC_DTO.ItemSubcategoryNumber);
            return View(PaginatedList<ItemSubcategory_DTO>.CreateAsync(ISC_List, DPageNumber ?? 1, DPageSize));

        }

        [Route("inventory/master/item-subcategory")]
        [HttpPost]
        public IActionResult ItemSubcategory(ItemSubcategory_DTO ISC_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        ISC_DTO.Id = 6;
                        DS = ISC_DAO.ItemSubcategoryDB(ISC_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            ISC_DTO.Id = 1;
                            ISC_DAO.ItemSubcategoryDB(ISC_DTO);

                            ISC_DTO.Reset();
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
                        ISC_DTO.DeleteNumbers = DeleteNumbers;
                        ISC_DTO.Id = 3;
                        ISC_DAO.ItemSubcategoryDB(ISC_DTO);

                        ISC_DTO.Reset();
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
                        ISC_DTO.ItemSubcategoryNumber = Number;
                        ISC_DTO.Id = 8;
                        ISC_DAO.ItemSubcategoryDB(ISC_DTO);

                        ISC_DTO.Reset();
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
                    ISC_DTO.Reset();
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    ISC_DTO.Id = 4;
                    DS = ISC_DAO.ItemSubcategoryDB(ISC_DTO);
                    ViewBag.ItemSubcategoryEdit = IM_DL.ISCList(DS.Tables[0]).FirstOrDefault();
                }
                else if (Mode == "Update")
                {
                    ISC_DTO.Id = 7;
                    DS = ISC_DAO.ItemSubcategoryDB(ISC_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            ISC_DTO.Id = 5;
                            ISC_DAO.ItemSubcategoryDB(ISC_DTO);

                            ISC_DTO.Reset();
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


                List<ItemSubcategory_DTO> ISC_List = ISGetData(SortOrder, Search, PageNumber, PSize, PageFilter, ISC_DTO.ItemSubcategoryNumber);
                return View(PaginatedList<ItemSubcategory_DTO>.CreateAsync(ISC_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }

        List<ItemSubcategory_DTO> ISGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                ISC_DTO.ItemSubcategoryNumber = Convert.ToInt64(Number);
            }

            ISC_DTO.Id = 2;
            ISC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = ISC_DAO.ItemSubcategoryDB(ISC_DTO);
            ISC_List = IM_DL.ISCList(DS.Tables[0]);
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

            var Key = ISC_List.OrderByDescending(Cs => Cs.ItemSubcategoryNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.ItemSubcategory!.ToLower().Contains(Search.ToLower()) || K.ISC_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.ItemSubcategoryNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.ItemSubcategory!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.ItemSubcategory!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.ItemSubcategoryNumber);
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



        [Route("inventory/master/item-subcategory/duplicate")]
        public Boolean ItemSubcategoryDuplicate(String? Title, String? Number)
        {
            ISC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            ISC_DTO.ItemSubcategory = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                ISC_DTO.Id = 6;
            }
            else
            {
                ISC_DTO.ItemSubcategoryNumber = Convert.ToInt32(Number);
                ISC_DTO.Id = 7;
            }
            DS = ISC_DAO.ItemSubcategoryDB(ISC_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //Item Subcategory
        [Route("inventory/master/uom")]
        public IActionResult UoM(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<UoM_DTO> U_List = UoMGetData(SortOrder, Search, PageNumber, PSize, PageFilter, U_DTO.UnitNumber);
            return View(PaginatedList<UoM_DTO>.CreateAsync(U_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("inventory/master/uom")]
        [HttpPost]
        public IActionResult UoM(UoM_DTO U_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    U_DTO.Id = 6;
                    DS = U_DAO.UoMDB(U_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        U_DTO.Id = 1;
                        U_DAO.UoMDB(U_DTO);

                        U_DTO.Reset();
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
                    U_DTO.DeleteNumbers = DeleteNumbers;
                    U_DTO.Id = 3;
                    U_DAO.UoMDB(U_DTO);

                    U_DTO.Reset();
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
                    U_DTO.UnitNumber = Number;
                    U_DTO.Id = 8;
                    U_DAO.UoMDB(U_DTO);

                    U_DTO.Reset();
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
                U_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                U_DTO.Id = 4;
                DS = U_DAO.UoMDB(U_DTO);
                ViewBag.UoMEdit = IM_DL.UoMList(DS.Tables[0]).FirstOrDefault();
            }
            else if (Mode == "Update")
            {
                U_DTO.Id = 7;
                DS = U_DAO.UoMDB(U_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        U_DTO.Id = 5;
                        U_DAO.UoMDB(U_DTO);

                        U_DTO.Reset();
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


            List<UoM_DTO> U_List = UoMGetData(SortOrder, Search, PageNumber, PSize, PageFilter, U_DTO.UnitNumber);
            return View(PaginatedList<UoM_DTO>.CreateAsync(U_List, DPageNumber ?? 1, DPageSize));
        }

        List<UoM_DTO> UoMGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                U_DTO.UnitNumber = Convert.ToInt64(Number);
            }

            U_DTO.Id = 2;
            U_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = U_DAO.UoMDB(U_DTO);
            U_List = IM_DL.UoMList(DS.Tables[0]);

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

            var Key = U_List.OrderByDescending(Cs => Cs.UnitNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.UnitCode!.ToLower().Contains(Search.ToLower()) || K.UnitDescription!.ToLower().Contains(Search.ToLower()) || Convert.ToString(K.DecimalPlaces).ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.UnitNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.UnitCode!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.UnitCode!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.UnitNumber);
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



        [Route("inventory/master/uom/duplicate")]
        public Boolean UoMDuplicate(String? Title, String? Number)
        {
            U_DTO.CreatorCode = Convert.ToInt32(UserCode);
            U_DTO.UnitCode = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                U_DTO.Id = 6;
            }
            else
            {
                U_DTO.UnitNumber = Convert.ToInt32(Number);
                U_DTO.Id = 7;
            }
            DS = U_DAO.UoMDB(U_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //Warehouse
        [Route("inventory/master/warehouse")]
        public IActionResult Warehouse(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<Warehouse_DTO> W_List = WGetData(SortOrder, Search, PageNumber, PSize, PageFilter, W_DTO.WarehouseNumber);
            return View(PaginatedList<Warehouse_DTO>.CreateAsync(W_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("inventory/master/warehouse")]
        [HttpPost]
        public IActionResult Warehouse(Warehouse_DTO W_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            W_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    W_DTO.Id = 6;
                    DS = W_DAO.WarehouseDB(W_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        W_DTO.Id = 1;
                        W_DAO.WarehouseDB(W_DTO);

                        W_DTO.Reset();
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
                    W_DTO.DeleteNumbers = DeleteNumbers;
                    W_DTO.Id = 3;
                    W_DAO.WarehouseDB(W_DTO);

                    W_DTO.Reset();
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
                    W_DTO.WarehouseNumber = Number;
                    W_DTO.Id = 8;
                    W_DAO.WarehouseDB(W_DTO);

                    W_DTO.Reset();
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
                W_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                W_DTO.Id = 4;
                DS = W_DAO.WarehouseDB(W_DTO);
                ViewBag.WarehouseEdit = IM_DL.WList(DS.Tables[0]).FirstOrDefault();
            }
            else if (Mode == "Update")
            {
                W_DTO.Id = 7;
                DS = W_DAO.WarehouseDB(W_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        W_DTO.Id = 5;
                        W_DAO.WarehouseDB(W_DTO);

                        W_DTO.Reset();
                        ModelState.Clear();
                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;

                        W_DTO.Id = 4;
                        DS = W_DAO.WarehouseDB(W_DTO);
                        ViewBag.WarehouseEdit = IM_DL.WList(DS.Tables[0]).FirstOrDefault();
                    }
                }
            }


            List<Warehouse_DTO> W_List = WGetData(SortOrder, Search, PageNumber, PSize, PageFilter, W_DTO.WarehouseNumber);
            return View(PaginatedList<Warehouse_DTO>.CreateAsync(W_List, DPageNumber ?? 1, DPageSize));
        }
        List<Warehouse_DTO> WGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                W_DTO.WarehouseNumber = Convert.ToInt64(Number);
            }

            W_DTO.Id = 2;
            W_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = W_DAO.WarehouseDB(W_DTO);
            W_List = IM_DL.WList(DS.Tables[0]);
            ViewBag.Category = Help.GetCat(DS.Tables[1]);
            ViewBag.Under = Help.GetUnder(DS.Tables[2]);

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

            var Key = W_List.OrderByDescending(Cs => Cs.WarehouseNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.WarehouseCode!.ToLower().Contains(Search.ToLower()) || K.WarehouseDescription!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.WarehouseNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.WarehouseCode!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.WarehouseCode!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.WarehouseNumber);
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



        [Route("inventory/master/warehouse/duplicate")]
        public Boolean WarehouseDuplicate(String? Title, String? Number)
        {
            W_DTO.CreatorCode = Convert.ToInt32(UserCode);
            W_DTO.WarehouseCode = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                W_DTO.Id = 6;
            }
            else
            {
                W_DTO.WarehouseNumber = Convert.ToInt32(Number);
                W_DTO.Id = 7;
            }
            DS = W_DAO.WarehouseDB(W_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        //Item Group
        [Route("inventory/master/item-group")]
        public IActionResult ItemGroup(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<ItemGroup_DTO> IG_List = IGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, IG_DTO.ItemGroupNumber);
            return View(PaginatedList<ItemGroup_DTO>.CreateAsync(IG_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("inventory/master/item-group")]
        [HttpPost]
        public IActionResult ItemGroup(ItemGroup_DTO IG_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            IG_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    IG_DTO.Id = 6;
                    DS = IG_DAO.ItemGroupDB(IG_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        IG_DTO.Id = 1;
                        IG_DAO.ItemGroupDB(IG_DTO);

                        IG_DTO.Reset();
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
                    IG_DTO.DeleteNumbers = DeleteNumbers;
                    IG_DTO.Id = 3;
                    IG_DAO.ItemGroupDB(IG_DTO);

                    IG_DTO.Reset();
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
                    IG_DTO.ItemGroupNumber = Number;
                    IG_DTO.Id = 8;
                    IG_DAO.ItemGroupDB(IG_DTO);

                    IG_DTO.Reset();
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
                IG_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                IG_DTO.Id = 4;
                DS = IG_DAO.ItemGroupDB(IG_DTO);
                ViewBag.ItemGroupEdit = IM_DL.IGList(DS.Tables[0]).FirstOrDefault();
            }
            else if (Mode == "Update")
            {
                IG_DTO.Id = 7;
                DS = IG_DAO.ItemGroupDB(IG_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        IG_DTO.Id = 5;
                        IG_DAO.ItemGroupDB(IG_DTO);

                        IG_DTO.Reset();
                        ModelState.Clear();
                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;

                        IG_DTO.Id = 4;
                        DS = IG_DAO.ItemGroupDB(IG_DTO);
                        ViewBag.ItemGroupEdit = IM_DL.IGList(DS.Tables[0]).FirstOrDefault();
                    }
                }
            }


            List<ItemGroup_DTO> IG_List = IGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, IG_DTO.ItemGroupNumber);
            return View(PaginatedList<ItemGroup_DTO>.CreateAsync(IG_List, DPageNumber ?? 1, DPageSize));
        }
        List<ItemGroup_DTO> IGGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                IG_DTO.ItemGroupNumber = Convert.ToInt64(Number);
            }

            IG_DTO.Id = 2;
            IG_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = IG_DAO.ItemGroupDB(IG_DTO);
            IG_List = IM_DL.IGList(DS.Tables[0]);
            ViewBag.Under = Help.GetUnder(DS.Tables[1]);
            ViewBag.Segregation = Help.GetCat(DS.Tables[2]);
            ViewBag.Ownership = Help.GetCat(DS.Tables[3]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[4]);

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

            var Key = IG_List.OrderByDescending(Cs => Cs.ItemGroupNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.ItemGroup!.ToLower().Contains(Search.ToLower()) || K.IG_Description!.ToLower().Contains(Search.ToLower()) || K.MaterialSegregation!.ToLower().Contains(Search.ToLower()) || K.MaterialOwnership!.ToLower().Contains(Search.ToLower()) || K.PurchaseWarehouse!.ToLower().Contains(Search.ToLower()) || K.SaleWarehouse!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.ItemGroupNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.ItemGroup!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.ItemGroup!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.ItemGroupNumber);
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



        [Route("inventory/master/item-group/duplicate")]
        public Boolean ItemGroupDuplicate(String? Title, String? Number)
        {
            IG_DTO.CreatorCode = Convert.ToInt32(UserCode);
            IG_DTO.ItemGroup = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                IG_DTO.Id = 6;
            }
            else
            {
                IG_DTO.ItemGroupNumber = Convert.ToInt32(Number);
                IG_DTO.Id = 7;
            }
            DS = IG_DAO.ItemGroupDB(IG_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //Expense Code
        [Route("inventory/master/expense-code")]
        public IActionResult ExpenseCode(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<ExpenseCode_DTO> EC_List = ECGetData(SortOrder, Search, PageNumber, PSize, PageFilter, EC_DTO.ExpenseCodeNumber);
            return View(PaginatedList<ExpenseCode_DTO>.CreateAsync(EC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("inventory/master/expense-code")]
        [HttpPost]
        public IActionResult ExpenseCode(ExpenseCode_DTO EC_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            EC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    EC_DTO.Id = 6;
                    DS = EC_DAO.ExpenseCodeDB(EC_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        EC_DTO.Id = 1;
                        EC_DAO.ExpenseCodeDB(EC_DTO);

                        EC_DTO.Reset();
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
                    EC_DTO.DeleteNumbers = DeleteNumbers;
                    EC_DTO.Id = 3;
                    EC_DAO.ExpenseCodeDB(EC_DTO);

                    EC_DTO.Reset();
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
                    EC_DTO.ExpenseCodeNumber = Number;
                    EC_DTO.Id = 8;
                    EC_DAO.ExpenseCodeDB(EC_DTO);

                    EC_DTO.Reset();
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
                EC_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                EC_DTO.Id = 4;
                DS = EC_DAO.ExpenseCodeDB(EC_DTO);
                ViewBag.ExpenseCodeEdit = IM_DL.ECList(DS.Tables[0]).FirstOrDefault();
            }
            else if (Mode == "Update")
            {
                EC_DTO.Id = 7;
                DS = EC_DAO.ExpenseCodeDB(EC_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        EC_DTO.Id = 5;
                        EC_DAO.ExpenseCodeDB(EC_DTO);

                        EC_DTO.Reset();
                        ModelState.Clear();
                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;

                        EC_DTO.Id = 4;
                        DS = EC_DAO.ExpenseCodeDB(EC_DTO);
                        ViewBag.ExpenseCodeEdit = IM_DL.ECList(DS.Tables[0]).FirstOrDefault();
                    }
                }
            }

            List<ExpenseCode_DTO> EC_List = ECGetData(SortOrder, Search, PageNumber, PSize, PageFilter, EC_DTO.ExpenseCodeNumber);
            return View(PaginatedList<ExpenseCode_DTO>.CreateAsync(EC_List, DPageNumber ?? 1, DPageSize));
        }
        List<ExpenseCode_DTO> ECGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                EC_DTO.ExpenseCodeNumber = Convert.ToInt64(Number);
            }

            EC_DTO.Id = 2;
            EC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = EC_DAO.ExpenseCodeDB(EC_DTO);
            EC_List = IM_DL.ECList(DS.Tables[0]);
            ViewBag.COA = Help.GetCat(DS.Tables[1]);
            ViewBag.SAC = Help.GetCat(DS.Tables[2]);

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

            var Key = EC_List.OrderByDescending(Cs => Cs.ExpenseCodeNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.ExpenseCode!.ToLower().Contains(Search.ToLower()) || K.EC_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.ExpenseCodeNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.ExpenseCode!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.ExpenseCode!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.ExpenseCodeNumber);
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



        [Route("inventory/master/expense-code/duplicate")]
        public Boolean ExpenseCodeDuplicate(String? Title, String? Number)
        {
            EC_DTO.CreatorCode = Convert.ToInt32(UserCode);
            EC_DTO.ExpenseCode = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                EC_DTO.Id = 6;
            }
            else
            {
                EC_DTO.ExpenseCodeNumber = Convert.ToInt32(Number);
                EC_DTO.Id = 7;
            }
            DS = EC_DAO.ExpenseCodeDB(EC_DTO);
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
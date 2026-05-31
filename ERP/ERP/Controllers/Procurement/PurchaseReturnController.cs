using ClosedXML.Excel;
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DL;
using ERP_DTO;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace ERP.Controllers.Procurement
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class PurchaseReturnController : Controller
    {
        CultureInfo India = new CultureInfo("hi-IN");
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        Procurement_DL P_DL = new Procurement_DL();


        //RETURN
        PurchaseReturnHead_DTO PRH_DTO = new PurchaseReturnHead_DTO();
        PurchaseReturn_DAO PR_DAO = new PurchaseReturn_DAO();
        PurchaseReturn_DTO PR_DTO = new PurchaseReturn_DTO();
        List<PurchaseReturn_DTO> PR_List = new List<PurchaseReturn_DTO>();


        //RETURN GRN
        GRNToPurchaseReturnHead_DTO GRNPRH_DTO = new GRNToPurchaseReturnHead_DTO();
        GRNToPurchaseReturn_DAO GRNPR_DAO = new GRNToPurchaseReturn_DAO();
        GRNToPurchaseReturn_DTO GRNPR_DTO = new GRNToPurchaseReturn_DTO();
        List<GRNToPurchaseReturn_DTO> GRNPR_List = new List<GRNToPurchaseReturn_DTO>();


        //RETURN GRN ITEM
        GRNItemToPurchaseReturnHead_DTO GRNIPRH_DTO = new GRNItemToPurchaseReturnHead_DTO();
        GRNItemToPurchaseReturn_DAO GRNIPR_DAO = new GRNItemToPurchaseReturn_DAO();
        GRNItemToPurchaseReturn_DTO GRNIPR_DTO = new GRNItemToPurchaseReturn_DTO();
        List<GRNItemToPurchaseReturn_DTO> GRNIPR_List = new List<GRNItemToPurchaseReturn_DTO>();


        //RETURN NUMBERING
        PRNumber_DTO PRN_DTO = new PRNumber_DTO();
        PRNumber_DAO PRN_DAO = new PRNumber_DAO();
        Numbering_DL PRN_DL = new Numbering_DL();
        List<PRNumberReset_DTO> PRR_List = new List<PRNumberReset_DTO>();
        List<PRNumberPrefix_DTO> PRP_List = new List<PRNumberPrefix_DTO>();
        List<PRNumberSuffix_DTO> PRS_List = new List<PRNumberSuffix_DTO>();


        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        DataSet DS = new DataSet();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;




        //Purchase Return Create
        [Route("procurement/transactions/purchase-return/create")]
        public IActionResult PurchaseReturnCreate()
        {
            PurchaseReturnTemp("1");
            PurchaseReturnHead_DTO PH_DTO = new PurchaseReturnHead_DTO();
            PH_DTO.PRH_ReturnDate = DateTime.Now.ToString("dd-MMM-yy");
            PurchaseReturnGetData();

            PH_DTO.PRH_ReturnNo = OnPurchaseReturnNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            return View(PH_DTO);
        }
        [HttpPost]
        [Route("procurement/transactions/purchase-return/create")]
        public IActionResult PurchaseReturnCreate(PurchaseReturnHead_DTO PRH_DTO, String? Mode)
        {
            var Original_PRH_DTO = Help.JsonClone(PRH_DTO);

            bool IsValid = false;
            PurchaseReturnHead_DTO P_Head_DTO = new PurchaseReturnHead_DTO();

            List<PurchaseReturnItem_DTO>? Item_DTO = new List<PurchaseReturnItem_DTO>();
            List<PurchaseReturnExpense_DTO>? Expense_DTO = new List<PurchaseReturnExpense_DTO>();
            List<PurchaseReturnIExpense_DTO>? ItemExpense_DTO = new List<PurchaseReturnIExpense_DTO>();
            List<PurchaseReturnBatch_DTO>? ItemBatch_DTO = new List<PurchaseReturnBatch_DTO>();

            P_Head_DTO = PRH_DTO;

            if (PRH_DTO.ReturnItems != null)
                Item_DTO = PRH_DTO.ReturnItems!.Where(K => K.PRI_IsDeleted == "false").ToList();

            if (PRH_DTO.Expenses != null)
                Expense_DTO = PRH_DTO.Expenses!.Where(K => K.PRH_EXP_IsDeleted == "false").ToList();

            if (PRH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PRH_DTO.ItemExpenses!.Where(K => K.PRI_EXP_IsDeleted == "false").ToList();

            if (PRH_DTO.ItemBatch != null)
                ItemBatch_DTO = PRH_DTO.ItemBatch!.Where(K => K.PRI_BCH_IsDeleted == "false").ToList();

            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PRI_MS_Number) != Convert.ToInt64(PRH_DTO.PRH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PRI_Qty) == 0 || Convert.ToDouble(x.PRI_UnitPrice) == 0 || Convert.ToDouble(x.PRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (Item_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(PRH_DTO.PRH_Vendor_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Vendor is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(PRH_DTO.PRH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PRH_DTO.PRH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();

                    P_Head_DTO.ReturnItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = ItemBatch_DTO;

                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        if (PurchaseReturnBatchValidation(Item_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String ReturnNoOld = PRH_DTO.PRH_ReturnNo;
                                    String ReturnNoNew = OnPurchaseReturnNumber(Convert.ToInt32(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd")));

                                    PR_DTO.PRH_ReturnNo = ReturnNoNew;
                                    PR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd"));
                                    PR_DTO.PRH_Vendor_Number = Convert.ToString(PRH_DTO.PRH_Vendor_Number);
                                    PR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PRH_DTO.PRH_ImportOrder) ? 1 : 0);
                                    PR_DTO.PRH_Currency_Number = Convert.ToString(PRH_DTO.PRH_Currency_Number);
                                    PR_DTO.PRH_DueDate = Convert.ToString(Convert.ToDateTime(PRH_DTO.PRH_DueDate).ToString("yyyyMMdd"));
                                    PR_DTO.PRH_MS_Number = Convert.ToString(PRH_DTO.PRH_MS_Number);
                                    PR_DTO.PRH_ExchangeRate = Convert.ToDouble(PRH_DTO.PRH_ExchangeRate);
                                    PR_DTO.PRH_TaxCluster_Number = Convert.ToString(PRH_DTO.PRH_TaxCluster_Number);
                                    PR_DTO.PRH_WHT_Number = Convert.ToString(PRH_DTO.PRH_WHT_Number);
                                    PR_DTO.PRH_MaterialCost = Convert.ToDouble(PRH_DTO.PRH_MaterialCost).ToString();
                                    PR_DTO.PRH_ItemMiscExpense = Convert.ToDouble(PRH_DTO.PRH_ItemMiscExpense).ToString();
                                    PR_DTO.PRH_HeaderMiscExpense = Convert.ToDouble(PRH_DTO.PRH_HeaderMiscExpense).ToString();
                                    PR_DTO.PRH_GST_Amount = Convert.ToDouble(PRH_DTO.PRH_GST_Amount).ToString();
                                    PR_DTO.PRH_ReturnAmount = Convert.ToDouble(PRH_DTO.PRH_ReturnAmount).ToString();
                                    PR_DTO.PRH_WHT_Amount = Convert.ToDouble(PRH_DTO.PRH_WHT_Amount).ToString();
                                    PR_DTO.PRH_RoundOff = Convert.ToDouble(PRH_DTO.PRH_RoundOff).ToString();
                                    PR_DTO.PRH_VendorReceivable = Convert.ToDouble(PRH_DTO.PRH_VendorReceivable).ToString();
                                    PR_DTO.PRH_DeliveryTerms = Convert.ToString(PRH_DTO.PRH_DeliveryTerms);
                                    PR_DTO.PRH_DeliveryMode = Convert.ToString(PRH_DTO.PRH_DeliveryMode);
                                    PR_DTO.Id = 21;
                                    DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                                    OnPurchaseReturnNumberGen(Convert.ToInt32(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        PR_DTO.PRH_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in Item_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                            PR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_Warehouse_Number);
                                            PR_DTO.PRI_UoM_Number = Convert.ToInt64(Item.PRI_UoM_Number).ToString();
                                            PR_DTO.PRI_Qty = Convert.ToDouble(Item.PRI_Qty).ToString();
                                            PR_DTO.PRI_UnitPrice = Convert.ToDouble(Item.PRI_UnitPrice).ToString();
                                            PR_DTO.PRI_Amount = Convert.ToDouble(Item.PRI_Amount).ToString();
                                            PR_DTO.PRI_ExpenseValue = Convert.ToDouble(Item.PRI_ExpenseValue);
                                            PR_DTO.PRI_HSN_Number = Convert.ToString(Item.PRI_HSN_Number);
                                            PR_DTO.PRI_GST_Amount = Convert.ToDouble(Item.PRI_GST_Amount).ToString();
                                            PR_DTO.PRI_WHT_Percent = Convert.ToDouble(Item.PRI_WHT_Percent).ToString();
                                            PR_DTO.PRI_WHT_Amount = Convert.ToDouble(Item.PRI_WHT_Amount).ToString();
                                            PR_DTO.Id = 22;
                                            D = PR_DAO.PurchaseReturnDB(PR_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PRI_EXP_Item_Number == Item.PRI_Item_Number) && (x.PRI_EXP_Item_Index == Item.PRI_Index));

                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PR_DTO.PRI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PR_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PRI_EXP_Expense_Number);
                                                PR_DTO.EXP_Remarks = ItemExp.PRI_EXP_Remarks;
                                                PR_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PRI_EXP_Occurrence_Number);
                                                PR_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PRI_EXP_CM_Number);
                                                PR_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PRI_EXP_ExpenseBase);
                                                PR_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PRI_EXP_ExpenseValue);
                                                PR_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PRI_EXP_Allocate_Number);
                                                PR_DTO.Id = 24;
                                                PR_DAO.PurchaseReturnDB(PR_DTO);
                                            }

                                            PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                            PR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                            PR_DTO.PRI_Op2 = Convert.ToString(1);
                                            PR_DTO.Id = 157;
                                            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
                                            if (DS.Tables[0].Rows.Count > 0)
                                            {
                                                DataTable dt = DS.Tables[0];
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    PR_DTO.BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                                    PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                                    PR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                                    PR_DTO.PRI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                    PR_DTO.PRI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                                    PR_DTO.PRI_BCH_No = Convert.ToString(row["BCH_No"]);
                                                    PR_DTO.PRI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                                    PR_DTO.PRI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                                    PR_DTO.PRI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                                    PR_DTO.Id = 25;
                                                    PR_DAO.PurchaseReturnDB(PR_DTO);
                                                }
                                            }
                                        }
                                        foreach (var Exp in Expense_DTO)
                                        {
                                            PR_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PRH_EXP_Expense_Number);
                                            PR_DTO.EXP_Remarks = Exp.PRH_EXP_Remarks;
                                            PR_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PRH_EXP_Occurrence_Number);
                                            PR_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PRH_EXP_CM_Number);
                                            PR_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PRH_EXP_ExpenseBase);
                                            PR_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.PRH_EXP_ExpenseValue);
                                            PR_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PRH_EXP_Allocate_Number);
                                            PR_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PRH_EXP_SAC_Number);
                                            PR_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PRH_EXP_TaxCalculate);
                                            PR_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PRH_EXP_TaxValue);
                                            PR_DTO.Id = 23;
                                            PR_DAO.PurchaseReturnDB(PR_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    P_Head_DTO.Reset();
                                    Expense_DTO = null;
                                    Item_DTO = null;
                                    ItemExpense_DTO = null;
                                    PRH_DTO.Reset();

                                    Original_PRH_DTO = Help.JsonClone(PRH_DTO);
                                    return RedirectToAction("PurchaseReturnCreate");
                                    //P_Head_DTO.PRH_ReturnDate = DateTime.Now.ToString("dd-MMM-yy");

                                    //if (ReturnNoOld != ReturnNoNew)
                                    //{
                                    //    ViewBag.ErrorCode = 2;
                                    //    ViewBag.ErrorMessage = "Purchase Return number " + ReturnNoOld + " used by another user. Next number will be allotted to you.";
                                    //}
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
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


            PurchaseReturnGetData();
            return View(Original_PRH_DTO);

        }
        Boolean PurchaseReturnBatchValidation(List<PurchaseReturnItem_DTO> Item_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                Double BatchAmount = 0;

                Int64 PRINumber = Convert.ToInt64(Item.PRI_Number);
                Int64 PRIIndex = Convert.ToInt64(Item.PRI_Index);
                String PRIItem = Convert.ToString(Item.PRI_Item_Number);
                Double PRIQty = Convert.ToDouble(Item.PRI_Qty);
                Double PRIAmount = Convert.ToDouble(Item.PRI_Amount);

                PR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_Warehouse_Number);
                PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                PR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                PR_DTO.PRI_Op2 = Convert.ToString(1);
                PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                PR_DTO.Id = 156;
                DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    BatchQty = Convert.ToDouble(DS.Tables[0].Rows[0][0].ToString());
                }

                if (PRIQty < 0)
                {
                    PRIQty = PRIQty * -1;
                }

                if (BatchQty == PRIQty) { }
                else
                {
                    Message += Item.PRI_ItemCode + " Batch Qty  Mismatched <br/>";
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

        void PurchaseReturnGetData()
        {
            PR_DTO.PRH_ReturnDate = Convert.ToString(DateTime.Now.ToString("yyyyMMdd"));
            PR_DTO.Id = 1;
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);

            //ViewBag.OrderNo = OnPurchaseNumber(DS);
            ViewBag.ExpenseCode = Help.GetCat(DS.Tables[0]);
            ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[4]);
            ViewBag.UoM = Help.GetCat(DS.Tables[5]);
            ViewBag.HSN = Help.GetCat(DS.Tables[6]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[7]);
            ViewBag.WHTax = Help.GetCat(DS.Tables[8]);
            ViewBag.IsCalculate = Help.GetCat(DS.Tables[9]);
        }

        [Route("procurement/transactions/purchase-return/item")]
        public IActionResult PurchaseReturnItem(String? ItemCode, String MS)
        {
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            if (MS == null)
            {
                MS = "";
            }
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRI_ItemCode = Convert.ToString(ItemCode).Trim();
            PR_DTO.PRH_MS_Number = Convert.ToString(MS).Trim();
            PR_DTO.Id = 2;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            var Item = P_DL.PRIList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("procurement/transactions/purchase-return/uom")]
        public String PurchaseReturnUoM(String? UoM)
        {
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRI_UoM_Number = Convert.ToString(UoM);
            PR_DTO.Id = 4;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("procurement/transactions/purchase-return/expense/des")]
        public IActionResult PurchaseReturnExpensiveDes(String? Title)
        {
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.EXP_Expense_Number = Convert.ToInt32(Title);
            PR_DTO.Id = 3;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            var Expensive = P_DL.ExReturnList(DS.Tables[0]).FirstOrDefault();
            return Json(Expensive);
        }

        [Route("procurement/transactions/purchase-return/gst")]
        public String PurchaseReturnGst(String? Cluster, String? PRHDate, String? HSN, String? BaseAmount)
        {
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            PR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            PR_DTO.PRI_HSN_Number = Convert.ToString(HSN);
            PR_DTO.Id = 6;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = P_DL.PurRerGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("procurement/transactions/purchase-return/wht")]
        public IActionResult PurchaseReturnWHT(String? Vendor, String? WHTNumber, String? PRHDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Vendor == null)
            {
                Vendor = "0";
            }
            if (PRHDate == null)
            {
                PRHDate = "0";
            }
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRH_Vendor_Number = Convert.ToString(Vendor);
            PR_DTO.PRH_WHT_Number = Convert.ToString(WHTNumber);
            PR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            PR_DTO.Id = 7;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            var WHT = P_DL.PurRerWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("procurement/transactions/purchase-return/vendor")]
        public IActionResult PurchaseReturnVendor(String? Vendor, String? PRHDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (PRHDate == null)
            {
                PRHDate = "0";
            }
            PR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PR_DTO.PRI_ItemCode = Convert.ToString(Vendor).Trim();
            PR_DTO.Id = 5;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            var Ven = P_DL.PRVList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-return/vendor/get")]
        public IActionResult PurchaseReturnVendorGet(String? Vendor, String? PRHDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (PRHDate == null)
            {
                PRHDate = "0";
            }
            PR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PR_DTO.PRH_Vendor_Number = Convert.ToString(Vendor).Trim();
            PR_DTO.Id = 10;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            var Ven = P_DL.PRVList(DS.Tables[0]).FirstOrDefault();
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-return/gst/view")]
        public IActionResult PurchaseReturnGstView(String? Cluster, String? PRHDate, String? HSN, String? BaseAmount)
        {
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            PR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            PR_DTO.PRI_HSN_Number = Convert.ToString(HSN);
            PR_DTO.Id = 9;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<PurchaseReturnGst> PurGST = new List<PurchaseReturnGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = P_DL.PurRerGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new PurchaseReturnGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("procurement/transactions/purchase-return/cluster")]
        public IActionResult PurchaseReturnCluster(String? Vendor, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.Search = Cluster;
            PR_DTO.PRH_Vendor_Number = Vendor;
            PR_DTO.Id = 8;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            var InvCluster = P_DL.PurRerCluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("procurement/transactions/purchase-return/expense/gst")]
        public String PurchaseReturnHeaderGst(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            PR_DTO.EXP_SAC_Number = Convert.ToInt64(SAC);

            PR_DTO.Id = 11;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = P_DL.PurInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("procurement/transactions/purchase-return/expense/gst/view")]
        public IActionResult PurchaseReturnGstHeaderView(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            PR_DTO.EXP_SAC_Number = Convert.ToInt64(SAC);
            PR_DTO.Id = 12;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<PurchaseInvoiceGst> PurGST = new List<PurchaseInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = P_DL.PurInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new PurchaseInvoiceGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("procurement/transactions/purchase-return/batch/get")]
        public IActionResult PurchaseReturnBatchGet(String? ItemNumber, String? Index, String? Warehouse)
        {
            if (ItemNumber == null)
            {
                ItemNumber = "";
            }
            if (Index == null)
            {
                Index = "0";
            }
            if (Warehouse == null)
            {
                Warehouse = "0";
            }

            PR_DTO.PRI_Item_Number = Convert.ToString(ItemNumber);
            PR_DTO.PRI_Warehouse_Number = Convert.ToString(Warehouse);
            PR_DTO.PRI_Op1 = Convert.ToString(Index);
            PR_DTO.PRI_Op2 = Convert.ToString(1);
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.Id = 151;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);

            PRBatchItem_DTO PRB = new PRBatchItem_DTO();
            PRB.PRBatch = P_DL.PRBatchList(DS.Tables[0]);
            PRB.PRView = P_DL.PRBatchOverallList(DS.Tables[1]);
            return Json(PRB);
        }

        [Route("procurement/transactions/purchase-return/batch/post")]
        [HttpPost]
        public IActionResult PurchaseReturnBatchGet([FromBody] MainItemBatch ItemBatch)
        {
            PR_DTO.PRI_Op2 = Convert.ToString(1);
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);

            foreach (var Item in ItemBatch.ItemBatch)
            {
                PR_DTO.BCH_Number = Convert.ToInt64(Item.PRI_BCH_Number);
                PR_DTO.PRI_BCH_Number = Convert.ToInt64(Item.PRI_BCH_BCH_Number);
                PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_BCH_Item_Number);
                PR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_BCH_Item_Index);
                PR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_BCH_Warehouse_Number);
                PR_DTO.PRI_BCH_Date = Convert.ToString(Item.PRI_BCH_Date);
                PR_DTO.PRI_BCH_No = Convert.ToString(Item.PRI_BCH_No);
                PR_DTO.PRI_BCH_Qty = Convert.ToDouble(Item.PRI_BCH_Qty);
                PR_DTO.PRI_BCH_UnitPrice = Convert.ToDouble(Item.PRI_BCH_UnitPrice);
                PR_DTO.PRI_BCH_Value = Convert.ToDouble(Item.PRI_BCH_Value);
                PR_DTO.Id = 153;
                DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (Item.PRI_BCH_Qty > 0)
                    {
                        PR_DTO.Id = 154;
                        PR_DAO.PurchaseReturnDB(PR_DTO);
                    }
                    else
                    {
                        PR_DTO.Id = 155;
                        PR_DAO.PurchaseReturnDB(PR_DTO);
                    }
                }
                else
                {
                    if (Item.PRI_BCH_Qty > 0)
                    {
                        PR_DTO.Id = 152;
                        PR_DAO.PurchaseReturnDB(PR_DTO);
                    }
                }
            }

            PRBatchItem_DTO PRB = new PRBatchItem_DTO();
            return Json(PRB);
        }


        void OnPurchaseReturnNumberGen(Int32 PRDate)
        {
            DataSet DS1 = new DataSet();
            PRN_DTO.PRN_Date = PRDate.ToString();
            PRN_DTO.Id = 101;
            DS1 = PRN_DAO.PRNumberDB(PRN_DTO);
            if (DS1.Tables[0].Rows.Count > 0)
            {
                Int32 Order = Convert.ToInt32(DS1.Tables[0].Rows[0]["PRN_Method"].ToString());
                DateTime PR_Date = Convert.ToDateTime(DateTime.ParseExact(PRDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));

                DateTime StartDate = new DateTime();
                DateTime EndDate = new DateTime();

                if (Order == 2)
                {
                    if (DS1.Tables[1].Rows.Count > 0)
                    {
                        Int32 Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["StartingNumber"].ToString());

                        PRN_DTO.PRN_Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["Number"].ToString());
                        PRN_DTO.PRN_StartingNumber = Convert.ToString(Convert.ToInt32(Number + 1));
                        PRN_DTO.Id = 103;
                        PRN_DAO.PRNumberDB(PRN_DTO);
                    }
                    else
                    {
                        Int32 Frequency = 0;
                        Int32 Start = 0;
                        DateTime Date = new DateTime();

                        if (DS1.Tables[2].Rows.Count > 0)
                        {
                            Date = Convert.ToDateTime(DS1.Tables[2].Rows[0]["PRR_Date"].ToString());
                            Start = Convert.ToInt32(DS1.Tables[2].Rows[0]["PRR_StartingNumber"].ToString());
                            Frequency = Convert.ToInt32(DS1.Tables[2].Rows[0]["PRR_Frequency"].ToString());
                        }

                        if (Frequency == 4)
                        {
                            if (Date.Month == PR_Date.Month)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PR_Date.Year, PR_Date.Month, 1);
                                EndDate = new DateTime(PR_Date.Year, PR_Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                        }
                        else if (Frequency == 5)
                        {
                            if (Date.Month == PR_Date.Month && Date.Year == PR_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else if (Date.Month != PR_Date.Month && Date.Year == PR_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PR_Date.Year, Date.Month, 1);
                                EndDate = new DateTime(PR_Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                        }

                        PRN_DTO.PRN_Number = Convert.ToInt32(DS1.Tables[2].Rows[0]["PRR_Number"].ToString());
                        PRN_DTO.PRN_StartingNumber = Convert.ToString(Start);
                        PRN_DTO.PRN_Date = Convert.ToString(StartDate.ToString("yyyyMMdd"));
                        PRN_DTO.PRN_Method = Convert.ToString(EndDate.ToString("yyyyMMdd"));
                        PRN_DTO.Id = 102;
                        PRN_DAO.PRNumberDB(PRN_DTO);
                    }
                }
            }
        }
        void PurchaseReturnTemp(String Mode)
        {
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PR_DTO.PRI_Op2 = Convert.ToString(Mode);
            PR_DTO.Id = 161;
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
        }


        [Route("procurement/transactions/purchase-return/grnitem/numbering")]
        [Route("procurement/transactions/purchase-return/grn/numbering")]
        [Route("procurement/transactions/purchase-return/numbering")]
        public String OnPurchaseReturnNumber(Int32 PRDate)
        {
            PR_DTO.PRH_ReturnDate = Convert.ToString(PRDate);
            PR_DTO.Id = 0;
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);

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
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["PRN_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["PRP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["PRS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0)
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["PRR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["PRR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["PRR_Frequency"].ToString());

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
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["PRR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["PRR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["PRR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["PRR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["PRR_Frequency"].ToString());

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
                ViewBag.ErrorMessage = "Purchase Return number Not assigned for Given date";
            }
            return "";
        }


        [Route("procurement/transactions/purchase-return/clear-temp")]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public IActionResult ClearPurchaseReturnTemp()
        {
            PurchaseReturnTemp("1");
            return Json(0);
        }







        //Purchase Return Summary
        [Route("procurement/transactions/purchase-return-register-summary")]
        public IActionResult PurchaseReturnRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            PR_List = PRSummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseReturn_DTO>.CreateAsync(PR_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("procurement/transactions/purchase-return-register-summary")]
        [HttpPost]
        public IActionResult PurchaseReturnRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? PR_No, String[] DeleteNumber, String selectAllCheckbox)
        {

            if (Mode == "DeleteAll")
            {
                PR_DTO.DeleteNumbers = DeleteNumbers;
                PR_DTO.Id = 31;
                PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            }
            else if (Mode == "Ascii" || Mode == "Excel" || Mode == "PDF")
            {
                List<Int64> AllowedPoNumbers = new List<Int64>();
                if (DeleteNumber.Length > 0)
                {
                    try
                    {
                        AllowedPoNumbers = DeleteNumber
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .Select(Int64.Parse)
                            .ToList();
                    }
                    catch
                    {
                    }
                }
                HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

                PR_DTO.Id = 51;
                PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                if (Mode == "Ascii")
                {
                    List<PurchaseReturnAscii> PR_List = P_DL.PRAscii(DS.Tables[0]);

                    var Key = PR_List;
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_VendorReceivable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PRH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_VendorReceivable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;
                    Decimal TotalGSTSum = 0;
                    Decimal TotalReturnAmountSum = 0;
                    Decimal TotalWithHoldTaxSum = 0;
                    Decimal TotalRoundOffSum = 0;
                    Decimal TotalVendorPayableSum = 0;

                    var HeaderRow = typeof(PurchaseReturnAscii)
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(prop => prop.Name != nameof(PurchaseReturnAscii.PRH_Number))
                            .Select(prop =>
                                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                             )
                            .ToList();

                    new StringBuilder().AppendLine(string.Join("\t", HeaderRow));



                    PropertyInfo[] PropertiesToInclude = typeof(PurchaseReturnAscii)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.Name != nameof(PurchaseReturnAscii.PRH_Number))
                        .ToArray();

                    foreach (var item in Key)
                    {
                        var rowValues = PropertiesToInclude
                            .Select(prop => prop.GetValue(item)?.ToString() ?? "")
                            .ToList();

                        var escapedRowValues = rowValues
                            .Select(val => val.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "))
                            .ToList();

                        new StringBuilder().AppendLine(string.Join("\t", escapedRowValues));

                        if (Decimal.TryParse(item.PRI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                        {
                            TotalQtySum += QtyValue;
                        }
                        if (Decimal.TryParse(item.PRH_MaterialCost, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                        {
                            TotalMaterialValueSum += MaterialValue;
                        }
                        if (Decimal.TryParse(item.PRH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                        {
                            TotalItemMiscExpenseSum += ItemMiscValue;
                        }
                        if (Decimal.TryParse(item.PRH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                        {
                            TotalHeadMiscExpenseSum += HeadMiscValue;
                        }
                        if (Decimal.TryParse(item.PRH_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                        {
                            TotalGSTSum += GSTAmount;
                        }
                        if (Decimal.TryParse(item.PRH_ReturnAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ReturnAmount))
                        {
                            TotalReturnAmountSum += ReturnAmount;
                        }
                        if (Decimal.TryParse(item.PRH_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                        {
                            TotalWithHoldTaxSum += WHTAmount;
                        }
                        if (Decimal.TryParse(item.PRH_RoundOff, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal RoundOff))
                        {
                            TotalRoundOffSum += RoundOff;
                        }
                        if (Decimal.TryParse(item.PRH_VendorReceivable, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal VendorPayable))
                        {
                            TotalVendorPayableSum += VendorPayable;
                        }
                    }

                    List<String> FooterCells = new List<String>();
                    Boolean TotalLabelAdded = false;
                    foreach (var prop in PropertiesToInclude)
                    {
                        string FooterCellValue = "";

                        if (!TotalLabelAdded && prop.Name != nameof(PurchaseReturnAscii.PRI_Qty) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_MaterialCost) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_ItemMiscExpense) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_HeaderMiscExpense) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_GST_Amount) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_ReturnAmount) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_WHT_Amount) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_RoundOff) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_VendorReceivable))
                        {
                            if (FooterCells.Count == 0)
                            {
                                FooterCellValue = "Total:";
                                TotalLabelAdded = true;
                            }
                        }


                        switch (prop.Name)
                        {
                            case nameof(PurchaseReturnAscii.PRI_Qty):
                                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnAscii.PRH_MaterialCost):
                                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnAscii.PRH_ItemMiscExpense):
                                FooterCellValue = TotalItemMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnAscii.PRH_HeaderMiscExpense):
                                FooterCellValue = TotalHeadMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnAscii.PRH_GST_Amount):
                                FooterCellValue = TotalGSTSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnAscii.PRH_ReturnAmount):
                                FooterCellValue = TotalReturnAmountSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnAscii.PRH_WHT_Amount):
                                FooterCellValue = TotalWithHoldTaxSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnAscii.PRH_RoundOff):
                                FooterCellValue = TotalRoundOffSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnAscii.PRH_VendorReceivable):
                                FooterCellValue = TotalVendorPayableSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                        }

                        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                    }

                    new StringBuilder().AppendLine(string.Join("\t", FooterCells));

                    String FileName = "PR-download";
                    byte[] fileBytes = Encoding.UTF8.GetBytes(new StringBuilder().ToString());
                    var contentType = "text/plain";
                    var fileDownloadName = $"{FileName}.txt";
                    return File(fileBytes, contentType, fileDownloadName);

                }
                else if (Mode == "Excel")
                {
                    PR_List = P_DL.PRList(DS.Tables[0]);

                    var Key = PR_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_MaterialCost!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_HeaderMiscExpense!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_GST_Amount!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnAmount!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Amount!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_RoundOff!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_VendorReceivable!.ToString().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PRH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_MaterialCost!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_HeaderMiscExpense!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_GST_Amount!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnAmount!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Amount!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_RoundOff!.ToString().Contains(Search.ToLower()) ||
                                 K.PRH_VendorReceivable!.ToString().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String FileName = "PR-download";
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add(PRSummaryDownload(Key.ToList()), "Sheet1");

                        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Style.Font.Bold = true;
                        ws.Columns().AdjustToContents();

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);

                            var content = stream.ToArray();
                            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            var fileDownloadName = $"{FileName}.xlsx";

                            return File(content, contentType, fileDownloadName);
                        }
                    }
                }
                else if (Mode == "PDF")
                {
                    PR_List = P_DL.PRList(DS.Tables[0]);

                    var Key = PR_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_VendorReceivable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PRH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_VendorReceivable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String PDFDownload = $@"<html>
                        <head></head>
                        <body>
                            <style>
                                html,body{{
                                    font-family: ""Noto Sans"", sans-serif;
                                    width: 100%;
                                    margin: 0;
                                    background-color: #fff;
                                }}

                                .table {{
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-size: 0.6rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                    .table th {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    background-color: #000;
                                        color:#fff;
                                    }}

                                    .table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    }}

                                    .table th {{
                                        font-weight: bold;
                                    }}

                                    .table tr td {{
                                        height: 25px;
                                    }}
                            </style>";

                    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Purchase Return No.</th><th style='width:30px;text-align:center'>Import Order</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th>Tax Cluster</th><th>WH_Tax Code</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Material Value</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th>GST Amount</th><th>Rejection Invoice Amount</th><th>WH_Tax Amount</th><th>Round off</th><th>Vendor Receivable</th></tr>";

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;
                    Decimal TotalGSTSum = 0;
                    Decimal TotalReturnAmountSum = 0;
                    Decimal TotalWithHoldTaxSum = 0;
                    Decimal TotalRoundOffSum = 0;
                    Decimal TotalVendorPayableSum = 0;

                    if (Key.ToList().Count > 0)
                    {
                        foreach (var Row in Key.ToList())
                        {
                            String Import = "";
                            if (Row.PRH_ImportOrder == 1)
                            {
                                Import = "Yes";
                            }
                            else
                            {
                                Import = "No";
                            }

                            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_MaterialCost));
                            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_ItemMiscExpense));
                            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_HeaderMiscExpense));
                            String GST = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_GST_Amount));
                            String Return = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_ReturnAmount));
                            String WHT = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_WHT_Amount));
                            String Pay = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_VendorReceivable));

                            PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PRH_ReturnDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.PRH_ReturnNo}</td>
                                <td style='width:30px;text-align:center'>{Import}</th>
                                <td>{Row.PRH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PRH_Currency_Number}</td>
                                <td>{Row.PRH_TaxCluster_Number}</td>
                                <td>{Row.PRH_WHT_Number}</td>
                                <td>{Row.PRH_MS_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PRH_TotalItem}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_Qty}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                <td style='width:30px;text-align:center'>{GST}</td>
                                <td style='width:70px;text-align:right'>{Return}</td>
                                <td style='width:70px;text-align:right'>{WHT}</td>
                                <td style='width:70px;text-align:right'>{Row.PRH_RoundOff}</td>
                                <td style='width:70px;text-align:right'>{Pay}</td>
                                </tr>";


                            if (Decimal.TryParse(Row.PRI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                            {
                                TotalQtySum += QtyValue;
                            }
                            if (Decimal.TryParse(Row.PRH_MaterialCost, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                            {
                                TotalMaterialValueSum += MaterialValue;
                            }
                            if (Decimal.TryParse(Row.PRH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                            {
                                TotalItemMiscExpenseSum += ItemMiscValue;
                            }
                            if (Decimal.TryParse(Row.PRH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                            {
                                TotalHeadMiscExpenseSum += HeadMiscValue;
                            }
                            if (Decimal.TryParse(Row.PRH_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                            {
                                TotalGSTSum += GSTAmount;
                            }
                            if (Decimal.TryParse(Row.PRH_ReturnAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ReturnAmount))
                            {
                                TotalReturnAmountSum += ReturnAmount;
                            }
                            if (Decimal.TryParse(Row.PRH_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                            {
                                TotalWithHoldTaxSum += WHTAmount;
                            }
                            if (Decimal.TryParse(Row.PRH_RoundOff, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal RoundOff))
                            {
                                TotalRoundOffSum += RoundOff;
                            }
                            if (Decimal.TryParse(Row.PRH_VendorReceivable, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal VendorPayable))
                            {
                                TotalVendorPayableSum += VendorPayable;
                            }
                        }
                    }

                    PDFDownload += $@"<tr>
                                <td>Total</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalGSTSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalReturnAmountSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalWithHoldTaxSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalRoundOffSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalVendorPayableSum))}</td>
                                </tr>";
                    PDFDownload += $@"</table></body></html>";

                    HtmlToPdf converter = new HtmlToPdf();
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                    converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                    converter.Options.MarginLeft = 10;
                    converter.Options.MarginRight = 10;
                    converter.Options.MarginTop = 10;
                    converter.Options.MarginBottom = 10;

                    PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                    MemoryStream memoryStream = new MemoryStream();
                    doc.Save(memoryStream);
                    doc.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "PR_Download.pdf");
                }
            }
            else if (Mode == "View")
            {
                return RedirectToAction("PurchaseReturnPreview", new { PR_No = PR_No });
            }
            else if (Mode == "Edit")
            {
                return RedirectToAction("PurchaseReturnEdit", new { PR_No = PR_No });
            }

            PR_List = PRSummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseReturn_DTO>.CreateAsync(PR_List, DPageNumber ?? 1, DPageSize));
        }
        List<PurchaseReturn_DTO> PRSummaryGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            PR_DTO.Id = 51;
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            PR_List = P_DL.PRList(DS.Tables[0]);

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

            var Key = PR_List.OrderByDescending(Cs => Cs.PRH_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PRI_Qty!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_ReturnAmount!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                 K.PRH_VendorReceivable!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.PRH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.PRH_ReturnNo!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.PRH_ReturnNo!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.PRH_Number);
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

            ViewBag.SumQty = Key.Sum(item => Convert.ToDouble(item.PRI_Qty));
            ViewBag.SumMaterial = Key.Sum(item => Convert.ToDouble(item.PRH_MaterialCost));
            ViewBag.SumItemMisc = Key.Sum(item => Convert.ToDouble(item.PRH_ItemMiscExpense));
            ViewBag.SumHeadMisc = Key.Sum(item => Convert.ToDouble(item.PRH_HeaderMiscExpense));
            ViewBag.SumHeaderGst = Key.Sum(item => Convert.ToDouble(item.PRH_GST_Amount));
            ViewBag.SumItemGst = Key.Sum(item => Convert.ToDouble(item.PRH_GST_Amount));
            ViewBag.SumAmount = Key.Sum(item => Convert.ToDouble(item.PRH_Amount));
            ViewBag.SumReturnAmount = Key.Sum(item => Convert.ToDouble(item.PRH_ReturnAmount));
            ViewBag.SumWHT = Key.Sum(item => Convert.ToDouble(item.PRH_WHT_Amount));
            ViewBag.SumRound = Key.Sum(item => Convert.ToDouble(item.PRH_RoundOff));
            ViewBag.SumVendor = Key.Sum(item => Convert.ToDouble(item.PRH_VendorReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }
        DataTable PRSummaryDownload(List<PurchaseReturn_DTO> PR_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "purchase-return-summary";

            TableDown.Clear();
            TableDown.Columns.Add("Date");
            TableDown.Columns.Add("Purchase Return No.");
            TableDown.Columns.Add("Import Order");
            TableDown.Columns.Add("Vendor Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Tax Cluster");
            TableDown.Columns.Add("WH_Tax Code");
            TableDown.Columns.Add("Material Segregation");
            TableDown.Columns.Add("No. of Line Item");
            TableDown.Columns.Add("Qty");
            TableDown.Columns.Add("Material Value");
            TableDown.Columns.Add("Item Misc Expense Value");
            TableDown.Columns.Add("Header Misc Expense Value");
            TableDown.Columns.Add("GST Amount");
            TableDown.Columns.Add("Rejection Invoice Amount");
            TableDown.Columns.Add("WH_Tax Amount");
            TableDown.Columns.Add("Round off");
            TableDown.Columns.Add("Vendor Receivable");


            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;
            Decimal TotalGSTSum = 0;
            Decimal TotalReturnAmountSum = 0;
            Decimal TotalWithHoldTaxSum = 0;
            Decimal TotalRoundOffSum = 0;
            Decimal TotalVendorPayableSum = 0;


            foreach (var Product in PR_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["Date"] = Product.PRH_ReturnDate;
                NewRow["Purchase Return No."] = Product.PRH_ReturnNo;
                NewRow["Import Order"] = (Product.PRH_ImportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Vendor Name"] = Product.PRH_Vendor_Number;
                NewRow["Currency"] = Product.PRH_Currency_Number;
                NewRow["Tax Cluster"] = Product.PRH_TaxCluster_Number;
                NewRow["WH_Tax Code"] = Product.PRH_WHT_Number;
                NewRow["Material Segregation"] = Product.PRH_MS_Number;
                NewRow["No. of Line Item"] = Product.PRH_TotalItem;
                NewRow["Qty"] = Product.PRI_Qty;
                NewRow["Material Value"] = Product.PRH_MaterialCost;
                NewRow["Item Misc Expense Value"] = Product.PRH_ItemMiscExpense;
                NewRow["Header Misc Expense Value"] = Product.PRH_HeaderMiscExpense;
                NewRow["GST Amount"] = Product.PRH_GST_Amount;
                NewRow["Rejection Invoice Amount"] = Product.PRH_ReturnAmount;
                NewRow["WH_Tax Amount"] = Product.PRH_WHT_Amount;
                NewRow["Round off"] = Product.PRH_RoundOff;
                NewRow["Vendor Receivable"] = Product.PRH_VendorReceivable;

                TableDown.Rows.Add(NewRow);


                if (Decimal.TryParse(Product.PRI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.PRH_MaterialCost, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.PRH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscExpenseSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.PRH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                {
                    TotalHeadMiscExpenseSum += HeadMiscValue;
                }
                if (Decimal.TryParse(Product.PRH_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                {
                    TotalGSTSum += GSTAmount;
                }
                if (Decimal.TryParse(Product.PRH_ReturnAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ReturnAmount))
                {
                    TotalReturnAmountSum += ReturnAmount;
                }
                if (Decimal.TryParse(Product.PRH_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                {
                    TotalWithHoldTaxSum += WHTAmount;
                }
                if (Decimal.TryParse(Product.PRH_RoundOff, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal RoundOff))
                {
                    TotalRoundOffSum += RoundOff;
                }
                if (Decimal.TryParse(Product.PRH_VendorReceivable, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal VendorPayable))
                {
                    TotalVendorPayableSum += VendorPayable;
                }
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["Date"] = "";
            NewRows["Purchase Return No."] = "";
            NewRows["Import Order"] = "";
            NewRows["Vendor Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Tax Cluster"] = "";
            NewRows["WH_Tax Code"] = "";
            NewRows["Material Segregation"] = "";
            NewRows["No. of Line Item"] = "";
            NewRows["Qty"] = TotalQtySum;
            NewRows["Material Value"] = TotalMaterialValueSum;
            NewRows["Item Misc Expense Value"] = TotalItemMiscExpenseSum;
            NewRows["Header Misc Expense Value"] = TotalHeadMiscExpenseSum;
            NewRows["GST Amount"] = TotalGSTSum;
            NewRows["Rejection Invoice Amount"] = TotalReturnAmountSum;
            NewRows["WH_Tax Amount"] = TotalWithHoldTaxSum;
            NewRows["Round off"] = TotalRoundOffSum;
            NewRows["Vendor Receivable"] = TotalVendorPayableSum;
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }


        [Route("procurement/transactions/purchase-return-register-summary/print")]
        public String PRSummaryPrint(String Search, String SelectedItem, bool AllItem)
        {
            List<Int64> AllowedPoNumbers = new List<Int64>();
            if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                try
                {
                    AllowedPoNumbers = SelectedItem.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Select(Int64.Parse)
                        .ToList();
                }
                catch
                {
                }
            }
            HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

            if (Search == null)
            {
                Search = "";
            }
            PR_DTO.Id = 51;
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            PR_List = P_DL.PRList(DS.Tables[0]);

            var Key = PR_List.ToList();
            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Qty!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ReturnAmount!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_VendorReceivable!.ToLower().Contains(Search.ToLower())).ToList();
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PRH_Number)).ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Qty!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ReturnAmount!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_VendorReceivable!.ToLower().Contains(Search.ToLower())).ToList();
                }
            }

            String PDFDownload = $@"<html>
                        <head></head>
                        <body>
                            <style>
                                html,body{{
                                    font-family: ""Noto Sans"", sans-serif;
                                    width: 100%;
                                    margin: 0;
                                    background-color: #fff;
                                }}

                                .table {{
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-size: 0.6rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                    .table th {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    background-color: #000;
                                        color:#fff;
                                    }}

                                    .table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    }}

                                    .table th {{
                                        font-weight: bold;
                                    }}

                                    .table tr td {{
                                        height: 25px;
                                    }}
                            </style>";

            PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Purchase Return No.</th><th style='width:30px;text-align:center'>Import Order</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th>Tax Cluster</th><th>WH_Tax Code</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Material Value</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th>GST Amount</th><th>Rejection Invoice Amount</th><th>WH_Tax Amount</th><th>Round off</th><th>Vendor Receivable</th></tr>";

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;
            Decimal TotalGSTSum = 0;
            Decimal TotalReturnAmountSum = 0;
            Decimal TotalWithHoldTaxSum = 0;
            Decimal TotalRoundOffSum = 0;
            Decimal TotalVendorPayableSum = 0;

            if (Key.ToList().Count > 0)
            {
                foreach (var Row in Key.ToList())
                {
                    String Import = "";
                    if (Row.PRH_ImportOrder == 1)
                    {
                        Import = "Yes";
                    }
                    else
                    {
                        Import = "No";
                    }

                    String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_MaterialCost));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_ItemMiscExpense));
                    String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_HeaderMiscExpense));
                    String GST = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_GST_Amount));
                    String Return = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_ReturnAmount));
                    String WHT = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_WHT_Amount));
                    String Pay = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_VendorReceivable));

                    PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PRH_ReturnDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.PRH_ReturnNo}</td>
                                <td style='width:30px;text-align:center'>{Import}</th>
                                <td>{Row.PRH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PRH_Currency_Number}</td>
                                <td>{Row.PRH_TaxCluster_Number}</td>
                                <td>{Row.PRH_WHT_Number}</td>
                                <td>{Row.PRH_MS_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PRH_TotalItem}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_Qty}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                <td style='width:30px;text-align:center'>{GST}</td>
                                <td style='width:70px;text-align:right'>{Return}</td>
                                <td style='width:70px;text-align:right'>{WHT}</td>
                                <td style='width:70px;text-align:right'>{Row.PRH_RoundOff}</td>
                                <td style='width:70px;text-align:right'>{Pay}</td>
                                </tr>";


                    if (Decimal.TryParse(Row.PRI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.PRH_MaterialCost, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalMaterialValueSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.PRH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscExpenseSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.PRH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                    {
                        TotalHeadMiscExpenseSum += HeadMiscValue;
                    }
                    if (Decimal.TryParse(Row.PRH_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                    {
                        TotalGSTSum += GSTAmount;
                    }
                    if (Decimal.TryParse(Row.PRH_ReturnAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ReturnAmount))
                    {
                        TotalReturnAmountSum += ReturnAmount;
                    }
                    if (Decimal.TryParse(Row.PRH_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                    {
                        TotalWithHoldTaxSum += WHTAmount;
                    }
                    if (Decimal.TryParse(Row.PRH_RoundOff, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal RoundOff))
                    {
                        TotalRoundOffSum += RoundOff;
                    }
                    if (Decimal.TryParse(Row.PRH_VendorReceivable, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal VendorPayable))
                    {
                        TotalVendorPayableSum += VendorPayable;
                    }
                }
            }

            PDFDownload += $@"<tr>
                                <td>Total</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalGSTSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalReturnAmountSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalWithHoldTaxSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalRoundOffSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalVendorPayableSum))}</td>
                                </tr>";
            PDFDownload += $@"</table></body></html>";

            return PDFDownload;
        }






        //Purchase Return Detailed
        [Route("procurement/transactions/purchase-return-register-detailed")]
        public IActionResult PurchaseReturnRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            PR_List = PRDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseReturn_DTO>.CreateAsync(PR_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("procurement/transactions/purchase-return-register-detailed")]
        [HttpPost]
        public IActionResult PurchaseReturnRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? PR_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            if (Mode == "Ascii" || Mode == "Excel" || Mode == "PDF")
            {
                List<Int64> AllowedPoNumbers = new List<Int64>();
                if (DeleteNumber.Length > 0)
                {
                    try
                    {
                        AllowedPoNumbers = DeleteNumber
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .Select(Int64.Parse)
                            .ToList();
                    }
                    catch
                    {
                    }
                }
                HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

                PR_DTO.Id = 52;
                PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                if (Mode == "Ascii")
                {
                    List<PurchaseReturnDetailAscii> PR_List = P_DL.PRDAscii(DS.Tables[0]);

                    var Key = PR_List;
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PRH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;
                    Decimal TotalGSTSum = 0;
                    Decimal TotalWithHoldTaxSum = 0;

                    var HeaderRow = typeof(PurchaseReturnDetailAscii)
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(prop => prop.Name != nameof(PurchaseReturnDetailAscii.PRH_Number))
                            .Select(prop =>
                                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                             )
                            .ToList();

                    var AsciiData = new StringBuilder();
                    AsciiData.AppendLine(string.Join("\t", HeaderRow));


                    PropertyInfo[] PropertiesToInclude = typeof(PurchaseReturnDetailAscii)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.Name != nameof(PurchaseReturnAscii.PRH_Number))
                        .ToArray();

                    foreach (var item in Key)
                    {
                        var rowValues = PropertiesToInclude
                            .Select(prop => prop.GetValue(item)?.ToString() ?? "")
                            .ToList();

                        var escapedRowValues = rowValues
                            .Select(val => val.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "))
                            .ToList();

                        AsciiData.AppendLine(string.Join("\t", escapedRowValues));

                        if (Decimal.TryParse(item.PRI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                        {
                            TotalQtySum += QtyValue;
                        }
                        if (Decimal.TryParse(item.PRI_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                        {
                            TotalMaterialValueSum += MaterialValue;
                        }
                        if (Decimal.TryParse(item.PRH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                        {
                            TotalItemMiscExpenseSum += ItemMiscValue;
                        }
                        if (Decimal.TryParse(item.PRH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeaderMiscValue))
                        {
                            TotalHeadMiscExpenseSum += HeaderMiscValue;
                        }
                        if (Decimal.TryParse(item.PRI_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                        {
                            TotalGSTSum += GSTAmount;
                        }
                        if (Decimal.TryParse(item.PRI_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                        {
                            TotalWithHoldTaxSum += WHTAmount;
                        }
                    }

                    List<String> FooterCells = new List<String>();
                    Boolean TotalLabelAdded = false;
                    foreach (var prop in PropertiesToInclude)
                    {
                        string FooterCellValue = "";

                        if (!TotalLabelAdded && prop.Name != nameof(PurchaseReturnAscii.PRI_Qty) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_MaterialCost) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_ItemMiscExpense) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_HeaderMiscExpense) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_GST_Amount) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_ReturnAmount) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_WHT_Amount) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_RoundOff) &&
                                                 prop.Name != nameof(PurchaseReturnAscii.PRH_VendorReceivable))
                        {
                            if (FooterCells.Count == 0)
                            {
                                FooterCellValue = "Total:";
                                TotalLabelAdded = true;
                            }
                        }

                        switch (prop.Name)
                        {
                            case nameof(PurchaseReturnDetailAscii.PRI_Qty):
                                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnDetailAscii.PRI_Amount):
                                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnDetailAscii.PRH_ItemMiscExpense):
                                FooterCellValue = TotalItemMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnDetailAscii.PRH_HeaderMiscExpense):
                                FooterCellValue = TotalHeadMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnDetailAscii.PRI_GST_Amount):
                                FooterCellValue = TotalGSTSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseReturnDetailAscii.PRI_WHT_Amount):
                                FooterCellValue = TotalWithHoldTaxSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                        }

                        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                    }

                    AsciiData.AppendLine(string.Join("\t", FooterCells));

                    String FileName = "PR-download";
                    byte[] fileBytes = Encoding.UTF8.GetBytes(AsciiData.ToString());
                    var contentType = "text/plain";
                    var fileDownloadName = $"{FileName}.txt";
                    return File(fileBytes, contentType, fileDownloadName);

                }
                else if (Mode == "Excel")
                {
                    PR_List = P_DL.PRDetailList(DS.Tables[0]);

                    var Key = PR_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_VendorReceivable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PRI_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_VendorReceivable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String FileName = "PR-download";
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add(PRDetailedDownload(Key.ToList()), "Sheet1");

                        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Style.Font.Bold = true;
                        ws.Columns().AdjustToContents();

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);

                            var content = stream.ToArray();
                            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            var fileDownloadName = $"{FileName}.xlsx";

                            return File(content, contentType, fileDownloadName);
                        }
                    }
                }
                else if (Mode == "PDF")
                {
                    PR_List = P_DL.PRDetailList(DS.Tables[0]);

                    var Key = PR_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PRI_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PRI_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String PDFDownload = $@"<html>
                        <head></head>
                        <body>
                            <style>
                                html,body{{
                                    font-family: ""Noto Sans"", sans-serif;
                                    width: 100%;
                                    margin: 0;
                                    background-color: #fff;
                                }}

                                .table {{
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-size: 0.6rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                    .table th {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    background-color: #000;
                                        color:#fff;
                                    }}

                                    .table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    }}

                                    .table th {{
                                        font-weight: bold;
                                    }}

                                    .table tr td {{
                                        height: 25px;
                                    }}
                            </style>";

                    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Purchase Return No.</th><th style='width:30px;text-align:center'>Import Order</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th>Supplier Invoice No.</th><th>Purchase Order Number</th><th>Material Segregation</th><th>Item Group</th><th>Item Code</th><th>Description</th><th>Warehouse</th><th style='width:30px;text-align:center'>UoM</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Unit Price</th><th style='width:70px'>Material Value</th><th>Expense Code</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th style='width:30px'>HSN</th><th>GST Amount</th><th>WH_Tax Code</th><th>WH_Tax Percent</th><th>WH_Tax Amount</th></tr>";

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;
                    Decimal TotalGSTSum = 0;
                    Decimal TotalWithHoldTaxSum = 0;

                    if (Key.ToList().Count > 0)
                    {
                        foreach (var Row in Key.ToList())
                        {
                            String Import = "";
                            if (Row.PRH_ImportOrder == 1)
                            {
                                Import = "Yes";
                            }
                            else
                            {
                                Import = "No";
                            }

                            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRI_Amount));
                            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_ItemMiscExpense));
                            String HeadeExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_HeaderMiscExpense));
                            String GST = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRI_GST_Amount));
                            String WHT = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRI_WHT_Amount));

                            PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PRH_ReturnDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.PRH_ReturnNo}</td>
                                <td>{Import}</td>
                                <td>{Row.PRH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PRH_Currency_Number}</td>
                                <td></td>
                                <td></td>
                                <td>{Row.PRI_MS_Number}</td>
                                <td>{Row.PRI_ItemGroup}</td>
                                <td>{Row.PRI_ItemCode}</td>
                                <td>{Row.PRI_Description}</td>
                                <td>{Row.PRI_Warehouse_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_UoM}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_Qty}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_UnitPrice}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{Row.PRH_EXP_Expense}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadeExp}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_HSN_Number}</td>
                                <td style='width:30px;text-align:center'>{GST}</td>
                                <td>{Row.PRH_WHT_Number}</td>
                                <td style='text-align:right'>{Row.PRI_WHT_Percent}</td>
                                <td style='width:50px;text-align:right'>{WHT}</td>
                                </tr>";


                            if (Decimal.TryParse(Row.PRI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                            {
                                TotalQtySum += QtyValue;
                            }
                            if (Decimal.TryParse(Row.PRI_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                            {
                                TotalMaterialValueSum += MaterialValue;
                            }
                            if (Decimal.TryParse(Row.PRH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                            {
                                TotalItemMiscExpenseSum += ItemMiscValue;
                            }
                            if (Decimal.TryParse(Row.PRH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeaderMiscValue))
                            {
                                TotalHeadMiscExpenseSum += HeaderMiscValue;
                            }
                            if (Decimal.TryParse(Row.PRI_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                            {
                                TotalGSTSum += GSTAmount;
                            }
                            if (Decimal.TryParse(Row.PRI_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                            {
                                TotalWithHoldTaxSum += WHTAmount;
                            }
                        }
                    }

                    PDFDownload += $@"<tr>
                                <td>Total</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td></td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td></td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalGSTSum))}</td>
                                <td></td>
                                <td></td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalWithHoldTaxSum))}</td>
                                </tr>";

                    PDFDownload += $@"</table></body></html>";

                    HtmlToPdf converter = new HtmlToPdf();
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                    converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                    converter.Options.MarginLeft = 10;
                    converter.Options.MarginRight = 10;
                    converter.Options.MarginTop = 10;
                    converter.Options.MarginBottom = 10;

                    PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                    MemoryStream memoryStream = new MemoryStream();
                    doc.Save(memoryStream);
                    doc.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "PR_Download.pdf");
                }
            }
            else if (Mode == "View")
            {
                PR_DTO.PRH_Number = Convert.ToInt32(PR_No);
                PR_DTO.Id = 41;
                PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    return RedirectToAction("PurchaseReturnPreview", new { PR_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }
            else if (Mode == "Edit")
            {
                PR_DTO.PRH_Number = Convert.ToInt32(PR_No);
                PR_DTO.Id = 41;
                PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    return RedirectToAction("PurchaseReturnEdit", new { PR_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }

            PR_List = PRDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseReturn_DTO>.CreateAsync(PR_List, DPageNumber ?? 1, DPageSize));
        }

        List<PurchaseReturn_DTO> PRDetailedGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            PR_DTO.Id = 52;
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            PR_List = P_DL.PRDetailList(DS.Tables[0]);

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

            var Key = PR_List.OrderByDescending(Cs => Cs.PRH_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.PRH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.PRH_ReturnNo!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.PRH_ReturnNo!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.PRH_Number);
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

            ViewBag.SumQty = Key.Sum(item => Convert.ToDouble(item.PRI_Qty));
            ViewBag.SumMaterial = Key.Sum(item => Convert.ToDouble(item.PRI_MaterialCost));
            ViewBag.SumAmount = Key.Sum(item => Convert.ToDouble(item.PRI_Amount));
            ViewBag.SumItemMisc = Key.Sum(item => Convert.ToDouble(item.PRH_ItemMiscExpense));
            ViewBag.SumHeaderMisc = Key.Sum(item => Convert.ToDouble(item.PRH_HeaderMiscExpense));
            ViewBag.SumItemGst = Key.Sum(item => Convert.ToDouble(item.PRI_GST_Amount));
            ViewBag.SumHeaderGst = Key.Sum(item => Convert.ToDouble(item.PRH_GST_Amount));
            ViewBag.SumWHT = Key.Sum(item => Convert.ToDouble(item.PRI_WHT_Amount));
            ViewBag.SumReturnAmount = Key.Sum(item => Convert.ToDouble(item.PRH_ReturnAmount));
            ViewBag.SumRoundOff = Key.Sum(item => Convert.ToDouble(item.PRH_RoundOff));
            ViewBag.SumReceivable = Key.Sum(item => Convert.ToDouble(item.PRH_VendorReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }
        DataTable PRDetailedDownload(List<PurchaseReturn_DTO> PR_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "purchase-return-detail";

            TableDown.Clear();
            TableDown.Columns.Add("Date");
            TableDown.Columns.Add("Purchase Return No.");
            TableDown.Columns.Add("Import Order");
            TableDown.Columns.Add("Vendor Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Supplier Invoice No.");
            TableDown.Columns.Add("Purchase Order Nnumber");
            TableDown.Columns.Add("Material Segregation");
            TableDown.Columns.Add("Item Group");
            TableDown.Columns.Add("Item Code");
            TableDown.Columns.Add("Description");
            TableDown.Columns.Add("Warehouse");
            TableDown.Columns.Add("UoM");
            TableDown.Columns.Add("Qty");
            TableDown.Columns.Add("Unit Price");
            TableDown.Columns.Add("Material Value");
            TableDown.Columns.Add("Expense Code");
            TableDown.Columns.Add("Item Misc Expense Value");
            TableDown.Columns.Add("Header Misc Expense Value");
            TableDown.Columns.Add("HSN");
            TableDown.Columns.Add("GST Amount");
            TableDown.Columns.Add("WH_Tax Code");
            TableDown.Columns.Add("WH_Tax Percent");
            TableDown.Columns.Add("WH_Tax Amount");

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeaderMiscExpenseSum = 0;
            Decimal TotalGSTSum = 0;
            Decimal TotalWithHoldTaxSum = 0;

            foreach (var Product in PR_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["Date"] = Product.PRH_ReturnDate;
                NewRow["Purchase Return No."] = Product.PRH_ReturnNo;
                NewRow["Import Order"] = (Product.PRH_ImportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Vendor Name"] = Product.PRH_Vendor_Number;
                NewRow["Currency"] = Product.PRH_Currency_Number;
                NewRow["Supplier Invoice No."] = "";
                NewRow["Purchase Order Nnumber"] = "";
                NewRow["Material Segregation"] = Product.PRI_MS_Number;
                NewRow["Item Group"] = Product.PRI_ItemGroup;
                NewRow["Item Code"] = Product.PRI_ItemCode;
                NewRow["Description"] = Product.PRI_Description;
                NewRow["Warehouse"] = Product.PRI_Warehouse_Number;
                NewRow["UoM"] = Product.PRI_UoM;
                NewRow["Qty"] = Product.PRI_Qty;
                NewRow["Unit Price"] = Product.PRI_UnitPrice;
                NewRow["Material Value"] = Product.PRI_Amount;
                NewRow["Expense Code"] = Product.PRH_EXP_Expense;
                NewRow["Item Misc Expense Value"] = Product.PRH_ItemMiscExpense;
                NewRow["Header Misc Expense Value"] = Product.PRH_HeaderMiscExpense;
                NewRow["HSN"] = Product.PRI_HSN_Number;
                NewRow["GST Amount"] = Product.PRI_GST_Amount;
                NewRow["WH_Tax Code"] = Product.PRH_WHT_Number;
                NewRow["WH_Tax Percent"] = Product.PRI_WHT_Percent;
                NewRow["WH_Tax Amount"] = Product.PRI_WHT_Amount;
                TableDown.Rows.Add(NewRow);


                if (Decimal.TryParse(Product.PRI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.PRI_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.PRH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscExpenseSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.PRH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeaderMiscValue))
                {
                    TotalHeaderMiscExpenseSum += HeaderMiscValue;
                }
                if (Decimal.TryParse(Product.PRI_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                {
                    TotalGSTSum += GSTAmount;
                }
                if (Decimal.TryParse(Product.PRI_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                {
                    TotalWithHoldTaxSum += WHTAmount;
                }
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["Date"] = "";
            NewRows["Purchase Return No."] = "";
            NewRows["Import Order"] = "";
            NewRows["Vendor Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Supplier Invoice No."] = "";
            NewRows["Purchase Order Nnumber"] = "";
            NewRows["Material Segregation"] = "";
            NewRows["Item Group"] = "";
            NewRows["Item Code"] = "";
            NewRows["Description"] = "";
            NewRows["Warehouse"] = "";
            NewRows["UoM"] = "";
            NewRows["Qty"] = TotalQtySum;
            NewRows["Unit Price"] = "";
            NewRows["Material Value"] = TotalMaterialValueSum;
            NewRows["Expense Code"] = TotalMaterialValueSum;
            NewRows["Item Misc Expense Value"] = TotalItemMiscExpenseSum;
            NewRows["Header Misc Expense Value"] = TotalHeaderMiscExpenseSum;
            NewRows["HSN"] = "";
            NewRows["GST Amount"] = TotalGSTSum;
            NewRows["WH_Tax Code"] = "";
            NewRows["WH_Tax Percent"] = "";
            NewRows["WH_Tax Amount"] = TotalWithHoldTaxSum;
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }

        [Route("procurement/transactions/purchase-return-register-detailed/print")]
        public String PRDetailedPrint(String Search, String SelectedItem, bool AllItem)
        {
            List<Int64> AllowedPoNumbers = new List<Int64>();
            if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                try
                {
                    AllowedPoNumbers = SelectedItem.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Select(Int64.Parse)
                        .ToList();
                }
                catch
                {
                }
            }
            HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

            if (Search == null)
            {
                Search = "";
            }
            PR_DTO.Id = 52;
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            PR_List = P_DL.PRDetailList(DS.Tables[0]);

            var Key = PR_List.ToList();

            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PRI_Number)).ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.PRH_ReturnNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ReturnDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PRI_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PRI_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                }
            }

            String PDFDownload = $@"<html>
                        <head></head>
                        <body>
                            <style>
                                html,body{{
                                    font-family: ""Noto Sans"", sans-serif;
                                    width: 100%;
                                    margin: 0;
                                    background-color: #fff;
                                }}

                                .table {{
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-size: 0.6rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                    .table th {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    background-color: #000;
                                        color:#fff;
                                    }}

                                    .table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                    }}

                                    .table th {{
                                        font-weight: bold;
                                    }}

                                    .table tr td {{
                                        height: 25px;
                                    }}
                            </style>";

            PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Purchase Return No.</th><th style='width:30px;text-align:center'>Import Order</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th>Supplier Invoice No.</th><th>Purchase Order Number</th><th>Material Segregation</th><th>Item Group</th><th>Item Code</th><th>Description</th><th>Warehouse</th><th style='width:30px;text-align:center'>UoM</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Unit Price</th><th style='width:70px'>Material Value</th><th>Expense Code</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th style='width:30px'>HSN</th><th>GST Amount</th><th>WH_Tax Code</th><th>WH_Tax Percent</th><th>WH_Tax Amount</th></tr>";

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;
            Decimal TotalGSTSum = 0;
            Decimal TotalWithHoldTaxSum = 0;

            if (Key.ToList().Count > 0)
            {
                foreach (var Row in Key.ToList())
                {
                    String Import = "";
                    if (Row.PRH_ImportOrder == 1)
                    {
                        Import = "Yes";
                    }
                    else
                    {
                        Import = "No";
                    }

                    String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRI_Amount));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_ItemMiscExpense));
                    String HeadeExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRH_HeaderMiscExpense));
                    String GST = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRI_GST_Amount));
                    String WHT = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PRI_WHT_Amount));

                    PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PRH_ReturnDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.PRH_ReturnNo}</td>
                                <td>{Import}</td>
                                <td>{Row.PRH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PRH_Currency_Number}</td>
                                <td></td>
                                <td></td>
                                <td>{Row.PRI_MS_Number}</td>
                                <td>{Row.PRI_ItemGroup}</td>
                                <td>{Row.PRI_ItemCode}</td>
                                <td>{Row.PRI_Description}</td>
                                <td>{Row.PRI_Warehouse_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_UoM}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_Qty}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_UnitPrice}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{Row.PRH_EXP_Expense}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadeExp}</td>
                                <td style='width:30px;text-align:center'>{Row.PRI_HSN_Number}</td>
                                <td style='width:30px;text-align:center'>{GST}</td>
                                <td>{Row.PRH_WHT_Number}</td>
                                <td style='text-align:right'>{Row.PRI_WHT_Percent}</td>
                                <td style='width:50px;text-align:right'>{WHT}</td>
                                </tr>";


                    if (Decimal.TryParse(Row.PRI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.PRI_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalMaterialValueSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.PRH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscExpenseSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.PRH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeaderMiscValue))
                    {
                        TotalHeadMiscExpenseSum += HeaderMiscValue;
                    }
                    if (Decimal.TryParse(Row.PRI_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                    {
                        TotalGSTSum += GSTAmount;
                    }
                    if (Decimal.TryParse(Row.PRI_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                    {
                        TotalWithHoldTaxSum += WHTAmount;
                    }
                }
            }

            PDFDownload += $@"<tr>
                                <td>Total</td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td></td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td></td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalGSTSum))}</td>
                                <td></td>
                                <td></td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalWithHoldTaxSum))}</td>
                                </tr>";

            PDFDownload += $@"</table></body></html>";

            return PDFDownload;
        }




        //Purchase Return View
        [Route("procurement/transactions/purchase-return/{PR_No}/view")]
        public IActionResult PurchaseReturnPreview(String PR_No)
        {

            String Active = PurchaseReturnData(PR_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseReturnRegisterSummary");
            }
            ViewBag.PR_No = PR_No;
            PurchaseReturnGetData();
            PurchaseReturnGetDataPrint(PR_No);
            return View(PRH_DTO);

        }

        [Route("procurement/transactions/purchase-return/{PR_No}/view")]
        [HttpPost]
        public IActionResult PurchaseReturnPreview(String Mode, String PR_No)
        {
            if (Mode == "PDF" || Mode == "Ascii" || Mode == "Excel")
            {
                PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                PR_DTO.Id = 71;
                PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                String Item = "";
                if (DS.Tables[1].Rows.Count > 0)
                {
                    for (Int32 i = 0; i < DS.Tables[1].Rows.Count; i++)
                    {
                        String UnitPrice = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["PRI_UnitPrice"]));
                        String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["PRI_Amount"]));
                        Item += $@"<tr>
                                <td colspan='1'>{DS.Tables[1].Rows[i]["PRI_HSN"]}</td>
                                <td colspan='1'>{DS.Tables[1].Rows[i]["PRI_ItemGroup"]}</td>
                                <td colspan='1'>{DS.Tables[1].Rows[i]["PRI_ItemCode"]}</td>
                                <td colspan='3'>{DS.Tables[1].Rows[i]["PRI_Description"]}</td>
                                <td class='align-center'>{DS.Tables[1].Rows[i]["PRI_UoM"]}</td>
                                <td colspan='1' class='align-center'>{DS.Tables[1].Rows[i]["PRI_Qty"]}</td>
                                <td colspan='2' class='align-right'>{UnitPrice}</td>
                                <td colspan='2' class='align-right'>{Amount}</td>
                                </tr>";
                    }

                    Int32 Count = 10 - Convert.ToInt32(DS.Tables[1].Rows.Count);
                    if (Count > 0)
                    {
                        for (Int32 i = 0; i < Count; i++)
                        {
                            Item += $@"<tr>
                                    <td colspan='1'></td>
                                    <td colspan='1'></td>
                                    <td colspan='1'></td>
                                    <td colspan='3'></td>
                                    <td class='align-center'></td>
                                    <td colspan='1' class='align-center'></td>
                                    <td colspan='2' class='align-right'></td>
                                    <td colspan='2' class='align-right'></td>
                                    </tr>";
                        }
                    }
                }

                String Download = $@"
                            <table class='PR-table' style='background-color: #fff;color:#000'>
                                <tbody>
                                    <tr>
                                        <td colspan='2' style='vertical-align: middle; text-align: center;'>[LOGO]</td>
                                        <td colspan='10' class='company-title'>ABC PRIVATE LIMITED</td>
                                    </tr>
                                    <tr>
                                        <td colspan='12'></td>
                                    </tr>
                                    <tr class='header-section'>
                                        <td colspan='6'>VENDOR</td>
                                        <td colspan='6'>Purchase Return</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["VendorName"]}</td>
                                        <td colspan='3'>PRH Number</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PRH_ReturnNo"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6' rowspan='3'>{DS.Tables[2].Rows[0]["Address"]}</td>
                                        <td colspan='3'>Date</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PRH_ReturnDate"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='3'>Purchase Order Number</td>
                                        <td colspan='3'></td>
                                    </tr>
                                    <tr>
                                        <td colspan='3'>Date</td>
                                        <td colspan='3'></td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["City"]}, {DS.Tables[2].Rows[0]["State"]}, {DS.Tables[2].Rows[0]["PRncode"]}</td>
                                        <td colspan='3'>Supplier Return No</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PRH_SupplierReturnNo"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>GST: {DS.Tables[2].Rows[0]["GSTIN"]}</td>
                                        <td colspan='3'>Date</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PRH_SupplierReturnDate"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>PAN: {DS.Tables[2].Rows[0]["PAN"]}</td>
                                        <td colspan='3'>Due Date</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PRH_DueDate"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='3'>Currency</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PRH_Currency"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='3'>Exchange Rate</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PRH_ExchangeRate"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='3'>Tax Cluster</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PRH_TaxCluster"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='3'>Withhold Tax Code</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PRH_WHT"]}</td>
                                    </tr>
                                    <tr class='header-section'>
                                        <th>HSN Code</th>
                                        <th>Item group</th>
                                        <th>Item Code</th>
                                        <th colspan='3'>Description</th>
                                        <th class='align-center'>Uom</th>
                                        <th colspan='1' class='align-center'>Qty</th>
                                        <th colspan='2' class='align-right'>Unit Price</th>
                                        <th colspan='2' class='align-right'>Value</th>
                                    </tr>";
                Download += Item;

                Double Mis = Convert.ToDouble(DS.Tables[0].Rows[0]["PRH_ItemMiscExpense"]) + Convert.ToDouble(DS.Tables[0].Rows[0]["PRH_HeaderMiscExpense"]);

                Download += $@"
                                    <tr>
                                        <td colspan='2'>Matrial Cost</td>
                                        <td colspan='2'>Misc. Expense</td>
                                        <td colspan='1'>GST</td>
                                        <td colspan='2'>Return Amount</td>
                                        <td colspan='2'>WH Tax</td>
                                        <td colspan='1'>Round off</td>
                                        <td colspan='2'>Payable</td>
                                    </tr>
                                    <tr>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["PRH_MaterialCost"]}</td>
                                        <td colspan='2'>{Mis}</td>
                                        <td colspan='1'>{DS.Tables[0].Rows[0]["PRH_GST_Amount"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["PRH_ReturnAmount"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["PRH_WHT_Amount"]}</td>
                                        <td colspan='1'>{DS.Tables[0].Rows[0]["PRH_RoundOff"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["PRH_VendorReceivable"]}</td>
                                    </tr>
                                    <tr>
                                        <td class='no-border' style='font-weight: bold;'>Tax</td>
                                        <td colspan='5' class='no-border'></td>
                                        <td colspan='6' class='no-border'></td>
                                    </tr>
                                    <tr>
                                        <td class='no-border' style='font-weight: bold;'>Remarks</td>
                                        <td colspan='11' class='no-border'></td>
                                    </tr>
                                    <tr class='signature-area'> <td colspan='12'> </td> </tr>
                                    <tr class='signature-area'> <td colspan='12'> </td> </tr>
                                    <tr class='signature-area'>
                                        <td colspan='8'></td>
                                        <td colspan='4' class='align-center'>for ABC Private Limited</td>
                                    </tr>
                                    <tr class='signature-area'> <td colspan='12'> </td> </tr>
                                    <tr class='signature-area'>
                                        <td colspan='8'></td>
                                        <td colspan='4' class='align-center' style='border-top: 1px solid #000; padding-top: 5px;'>Authorized Signatory</td>
                                    </tr>
                                </tbody>
                            </table>";


                if (Mode == "PDF")
                {
                    String PDFDownload = $@"<html>
                        <head>
                            <title>Part Label</title>
                        </head>
                        <body>
                            <style>
                                html,body{{
                                    font-size: 1rem;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}
                                .PR-container {{
                                    max-width: 100%;
                                    margin: auto;
                                    background-color: #fff;
                                    padding: 20px;
                                    height: 100%;
                                    font-size: 1rem;
                                }}

                                .PR-table {{
                                    width: 100%;
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}

                                    .PR-table th, .PR-table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                        line-height: 1.4;
                                    }}

                                    .PR-table th {{
                                        font-weight: bold;
                                    }}

                                    .PR-table tr td {{
                                        height: 29px;
                                    }}

                                .header-main {{
                                    background-color: #1E8449;
                                    color: white;
                                    font-weight: bold;
                                    text-align: center;
                                }}

                                .header-section {{
                                    background-color: #2ECC71;
                                    color: white;
                                    font-weight: bold;
                                    text-align: center;
                                }}

                                .header-sub {{
                                    background-color: #ABEBC6;
                                    color: #333;
                                    font-weight: bold;
                                }}

                                .align-right {{
                                    text-align: right;
                                }}

                                .align-center {{
                                    text-align: center;
                                }}

                                .company-title {{
                                    font-size: 1.4rem;
                                    font-weight: bold;
                                    text-align: center;
                                    vertical-align: middle;
                                }}

                                .PR-title {{
                                    font-size: 1.2rem;
                                    font-weight: bold;
                                    text-align: center;
                                    vertical-align: middle;
                                }}

                                .placeholder {{
                                    color: #777;
                                    font-style: italic;
                                }}

                                .total-row td {{
                                    background-color: #2ECC71;
                                    color: white;
                                    font-weight: bold;
                                }}

                                .no-border {{
                                    border: none !imPRrtant;
                                }}

                                .signature-area td {{
                                    border: none;
                                    padding-top: 15px;
                                    padding-bottom: 5px;
                                }}

                                a.details-link {{
                                    color: #0000EE;
                                    text-decoration: underline;
                                }}
                            </style>";
                    PDFDownload += Download;
                    PDFDownload += "</body></html>";

                    HtmlToPdf converter = new HtmlToPdf();
                    converter.Options.PdfPageSize = PdfPageSize.A4;
                    converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                    //converter.Options.PdfPageCustomSize = new System.Drawing.SizeF(420,160);
                    converter.Options.MarginLeft = 0;
                    converter.Options.MarginRight = 0;
                    converter.Options.MarginTop = 0;
                    converter.Options.MarginBottom = 0;

                    PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                    MemoryStream memoryStream = new MemoryStream();
                    doc.Save(memoryStream);
                    doc.Close();

                    ModelState.Clear();
                    return File(memoryStream.ToArray(), "application/pdf", "Print_Download.pdf");
                }
                else if (Mode == "Ascii")
                {

                    var doc = new HtmlDocument();
                    doc.LoadHtml(Download);

                    var asciiBuilder = new StringBuilder();

                    foreach (var row in doc.DocumentNode.SelectNodes("//tr"))
                    {
                        foreach (var cell in row.SelectNodes("th|td"))
                        {
                            //var colspan = cell.GetAttributeValue("colspan", 1);
                            asciiBuilder.Append(cell.InnerText);
                            //for (int i = 1; i < colspan; i++)
                            //{
                            //    asciiBuilder.Append("\t");
                            //}
                            asciiBuilder.Append("\t");
                        }
                        asciiBuilder.AppendLine();
                    }

                    byte[] fileBytes = Encoding.UTF8.GetBytes(asciiBuilder.ToString());
                    string contentType = "text/plain";
                    string fileDownloadName = "product-download.txt";

                    return File(fileBytes, contentType, fileDownloadName);
                }
                else if (Mode == "Excel")
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(Download);

                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Sheet1");

                        var rows = doc.DocumentNode.SelectNodes("//tr");
                        if (rows != null)
                        {
                            int rowIndex = 1;
                            foreach (var row in rows)
                            {
                                int colIndex = 1;
                                foreach (var cell in row.SelectNodes("td|th"))
                                {
                                    ws.Cell(rowIndex, colIndex).Value = cell.InnerText;
                                    colIndex++;
                                }
                                rowIndex++;
                            }
                        }

                        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Style.Font.Bold = true;
                        ws.Columns().AdjustToContents();

                        using (var stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            var content = stream.ToArray();
                            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            var fileDownloadName = "purchase-order-download.xlsx";

                            return File(content, contentType, fileDownloadName);
                        }
                    }
                }
            }
            else if (Mode == "Delete")
            {
                PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                PR_DTO.Id = 31;
                PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PR_DAO.PurchaseReturnDB(PR_DTO);
                return RedirectToAction("PurchaseReturnRegisterSummary");
            }

            String Active = PurchaseReturnData(PR_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseReturnRegisterSummary");
            }
            ViewBag.PR_No = PR_No;
            PurchaseReturnGetData();
            PurchaseReturnGetDataPrint(PR_No);
            return View(PRH_DTO);
        }
        String PurchaseReturnData(String PR_No)
        {
            PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
            PR_DTO.Id = 61;
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                PRH_DTO = P_DL.PRHeadList(DS.Tables[0]).FirstOrDefault();
                PRH_DTO.ReturnItems = P_DL.PRItemList(DS.Tables[1]);
                PRH_DTO.Expenses = P_DL.PRExpenseEditList(DS.Tables[2]);
                PRH_DTO.ItemExpenses = P_DL.PRIExpenseEditList(DS.Tables[3]);
                PRH_DTO.ItemBatch = P_DL.PRIBatchEditList(DS.Tables[4]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["PRH_Mode"].ToString();
                return "1";
            }
            else
            {
                return "0";
            }
        }
        String PurchaseReturnGetDataPrint(String PR_No)
        {
            PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
            PR_DTO.Id = 71;
            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                ViewBag.HeadPreview = P_DL.PRHeadList(DS.Tables[0]).FirstOrDefault();
                ViewBag.ItemPreview = P_DL.PRItemList(DS.Tables[1]);
                ViewBag.VendorPreview = P_DL.VendorList(DS.Tables[2]).FirstOrDefault();
                return "1";
            }
            else
            {
                return "0";
            }
        }




        //Purchase Return Edit
        [Route("procurement/transactions/purchase-return/{PR_No}/edit")]
        public IActionResult PurchaseReturnEdit(String PR_No)
        {
            PurchaseReturnTemp("2");
            String Active = PurchaseReturnData(PR_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseReturnRegisterSummary");
            }
            ViewBag.PR_No = PR_No;
            PurchaseReturnGetData();
            return View(PRH_DTO);
        }

        [Route("procurement/transactions/purchase-return/{PR_No}/edit")]
        [HttpPost]
        public IActionResult PurchaseReturnEdit(PurchaseReturnHead_DTO PRH_DTO, String? Mode, String PR_No)
        {
            var Original_PRH_DTO = Help.JsonClone(PRH_DTO);

            bool IsValid = false;
            PurchaseReturnHead_DTO P_Head_DTO = new PurchaseReturnHead_DTO();

            List<PurchaseReturnItem_DTO>? Item_DTO = new List<PurchaseReturnItem_DTO>();
            List<PurchaseReturnExpense_DTO>? Expense_DTO = new List<PurchaseReturnExpense_DTO>();
            List<PurchaseReturnIExpense_DTO>? ItemExpense_DTO = new List<PurchaseReturnIExpense_DTO>();
            List<PurchaseReturnBatch_DTO>? ItemBatch_DTO = new List<PurchaseReturnBatch_DTO>();

            P_Head_DTO = PRH_DTO;

            if (PRH_DTO.ReturnItems != null)
                Item_DTO = PRH_DTO.ReturnItems!.Where(K => K.PRI_IsDeleted == "false").ToList();

            if (PRH_DTO.Expenses != null)
                Expense_DTO = PRH_DTO.Expenses!.Where(K => K.PRH_EXP_IsDeleted == "false").ToList();

            if (PRH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PRH_DTO.ItemExpenses!.Where(K => K.PRI_EXP_IsDeleted == "false").ToList();

            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PRI_MS_Number) != Convert.ToInt64(PRH_DTO.PRH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PRI_Qty) == 0 || Convert.ToDouble(x.PRI_UnitPrice) == 0 || Convert.ToDouble(x.PRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (Item_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(PRH_DTO.PRH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PRH_DTO.PRH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    P_Head_DTO.ReturnItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = null;

                    ModelState.Clear();
                    IsValid = TryValidateModel(P_Head_DTO);
                    if (IsValid)
                    {
                        //if (PurchaseReturnBatchValidation(Item_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                                    PR_DTO.PRH_ReturnNo = PRH_DTO.PRH_ReturnNo;
                                    PR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd"));
                                    PR_DTO.PRH_Vendor_Number = Convert.ToString(PRH_DTO.PRH_Vendor_Number);
                                    PR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PRH_DTO.PRH_ImportOrder) ? 1 : 0);
                                    PR_DTO.PRH_Currency_Number = Convert.ToString(PRH_DTO.PRH_Currency_Number);
                                    PR_DTO.PRH_DueDate = Convert.ToString(Convert.ToDateTime(PRH_DTO.PRH_DueDate).ToString("yyyyMMdd"));
                                    PR_DTO.PRH_MS_Number = Convert.ToString(PRH_DTO.PRH_MS_Number);
                                    PR_DTO.PRH_ExchangeRate = Convert.ToDouble(PRH_DTO.PRH_ExchangeRate);
                                    PR_DTO.PRH_TaxCluster_Number = Convert.ToString(PRH_DTO.PRH_TaxCluster_Number);
                                    PR_DTO.PRH_WHT_Number = Convert.ToString(PRH_DTO.PRH_WHT_Number);
                                    PR_DTO.PRH_MaterialCost = Convert.ToDouble(PRH_DTO.PRH_MaterialCost).ToString();
                                    PR_DTO.PRH_ItemMiscExpense = Convert.ToDouble(PRH_DTO.PRH_ItemMiscExpense).ToString();
                                    PR_DTO.PRH_HeaderMiscExpense = Convert.ToDouble(PRH_DTO.PRH_HeaderMiscExpense).ToString();
                                    PR_DTO.PRH_GST_Amount = Convert.ToDouble(PRH_DTO.PRH_GST_Amount).ToString();
                                    PR_DTO.PRH_ReturnAmount = Convert.ToDouble(PRH_DTO.PRH_ReturnAmount).ToString();
                                    PR_DTO.PRH_WHT_Amount = Convert.ToDouble(PRH_DTO.PRH_WHT_Amount).ToString();
                                    PR_DTO.PRH_RoundOff = Convert.ToDouble(PRH_DTO.PRH_RoundOff).ToString();
                                    PR_DTO.PRH_VendorReceivable = Convert.ToDouble(PRH_DTO.PRH_VendorReceivable).ToString();
                                    PR_DTO.PRH_DeliveryTerms = Convert.ToString(PRH_DTO.PRH_DeliveryTerms);
                                    PR_DTO.PRH_DeliveryMode = Convert.ToString(PRH_DTO.PRH_DeliveryMode);
                                    PR_DTO.Id = 101;
                                    DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                                    String ItemDTO = string.Join(", ", Item_DTO.Where(x => Convert.ToInt64(x.PRI_Number) != 0).Select(x => x.PRI_Number));
                                    String ItemExpDTO = string.Join(", ", ItemExpense_DTO.Where(x => Convert.ToInt64(x.PRI_EXP_Number) != 0).Select(x => x.PRI_EXP_Number));
                                    String ExpDTO = string.Join(", ", Expense_DTO.Where(x => Convert.ToInt64(x.PRH_EXP_Number) != 0).Select(x => x.PRH_EXP_Number));
                                    String BatchDTO = string.Join(", ", ItemBatch_DTO.Where(x => Convert.ToInt64(x.PRI_BCH_Number) != 0).Select(x => x.PRI_BCH_Number));

                                    PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                                    PR_DTO.DeleteNumbers = Convert.ToString(ItemDTO);
                                    PR_DTO.Id = 102;
                                    DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                                    PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                                    PR_DTO.DeleteNumbers = Convert.ToString(ExpDTO);
                                    PR_DTO.Id = 103;
                                    DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                                    PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                                    PR_DTO.DeleteNumbers = Convert.ToString(ItemExpDTO);
                                    PR_DTO.Id = 104;
                                    DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                                    PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                                    PR_DTO.DeleteNumbers = Convert.ToString(BatchDTO);
                                    PR_DTO.Id = 105;
                                    DS = PR_DAO.PurchaseReturnDB(PR_DTO);

                                    foreach (var Item in Item_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                                        PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                        PR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_Warehouse_Number);
                                        PR_DTO.PRI_UoM_Number = Convert.ToInt64(Item.PRI_UoM_Number).ToString();
                                        PR_DTO.PRI_Qty = Convert.ToDouble(Item.PRI_Qty).ToString();
                                        PR_DTO.PRI_UnitPrice = Convert.ToDouble(Item.PRI_UnitPrice).ToString();
                                        PR_DTO.PRI_Amount = Convert.ToDouble(Item.PRI_Amount).ToString();
                                        PR_DTO.PRI_ExpenseValue = Convert.ToDouble(Item.PRI_ExpenseValue);
                                        PR_DTO.PRI_HSN_Number = Convert.ToInt64(Item.PRI_HSN_Number).ToString();
                                        PR_DTO.PRI_GST_Amount = Convert.ToDouble(Item.PRI_GST_Amount).ToString();
                                        PR_DTO.PRI_WHT_Percent = Convert.ToDouble(Item.PRI_WHT_Percent).ToString();
                                        PR_DTO.PRI_WHT_Amount = Convert.ToDouble(Item.PRI_WHT_Amount).ToString();

                                        if (Item.PRI_Number == 0)
                                        {
                                            PR_DTO.Id = 22;
                                            D = PR_DAO.PurchaseReturnDB(PR_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PRI_EXP_Item_Number == Item.PRI_Item_Number) && (x.PRI_EXP_Item_Index == Item.PRI_Index));
                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                                                PR_DTO.PRI_Number = Convert.ToInt64(ItemExp.PRI_EXP_PRI_Number);
                                                PR_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PRI_EXP_Expense_Number);
                                                PR_DTO.EXP_Remarks = ItemExp.PRI_EXP_Remarks;
                                                PR_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PRI_EXP_Occurrence_Number);
                                                PR_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PRI_EXP_CM_Number);
                                                PR_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PRI_EXP_ExpenseBase);
                                                PR_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PRI_EXP_Allocate_Number);
                                                if (ItemExp.PRI_EXP_Number == 0)
                                                {
                                                    PR_DTO.Id = 24;
                                                }
                                                else
                                                {
                                                    PR_DTO.EXP_Number = Convert.ToInt64(ItemExp.PRI_EXP_Number);
                                                    PR_DTO.Id = 107;
                                                }
                                                PR_DAO.PurchaseReturnDB(PR_DTO);
                                            }

                                            PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                            PR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                            PR_DTO.PRI_Op2 = Convert.ToString(1);
                                            PR_DTO.Id = 157;
                                            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
                                            if (DS.Tables[0].Rows.Count > 0)
                                            {
                                                DataTable dt = DS.Tables[0];
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    PR_DTO.BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                                    PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                                    PR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                                    PR_DTO.PRI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                    PR_DTO.PRI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                                    PR_DTO.PRI_BCH_No = Convert.ToString(row["BCH_No"]);
                                                    PR_DTO.PRI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                                    PR_DTO.PRI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                                    PR_DTO.PRI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                                    PR_DTO.Id = 25;
                                                    PR_DAO.PurchaseReturnDB(PR_DTO);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            PR_DTO.PRI_Number = Convert.ToInt64(Item.PRI_Number);
                                            PR_DTO.Id = 106;
                                            D = PR_DAO.PurchaseReturnDB(PR_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PRI_EXP_PRI_Number == Item.PRI_Number));
                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                                                PR_DTO.PRI_Number = Convert.ToInt64(ItemExp.PRI_EXP_PRI_Number);
                                                PR_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PRI_EXP_Expense_Number);
                                                PR_DTO.EXP_Remarks = ItemExp.PRI_EXP_Remarks;
                                                PR_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PRI_EXP_Occurrence_Number);
                                                PR_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PRI_EXP_CM_Number);
                                                PR_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PRI_EXP_ExpenseBase);
                                                PR_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PRI_EXP_Allocate_Number);
                                                if (ItemExp.PRI_EXP_Number == 0)
                                                {
                                                    PR_DTO.Id = 24;
                                                }
                                                else
                                                {
                                                    PR_DTO.EXP_Number = Convert.ToInt64(ItemExp.PRI_EXP_Number);
                                                    PR_DTO.Id = 107;
                                                }
                                                PR_DAO.PurchaseReturnDB(PR_DTO);
                                            }

                                            PR_DTO.PRI_Number = Convert.ToInt64(Item.PRI_Number);
                                            PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                            PR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                            PR_DTO.PRI_Op2 = Convert.ToString(1);
                                            PR_DTO.Id = 157;
                                            DS = PR_DAO.PurchaseReturnDB(PR_DTO);
                                            if (DS.Tables[0].Rows.Count > 0)
                                            {
                                                DataTable dt = DS.Tables[0];
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    PR_DTO.BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                                    PR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                                    PR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                                    PR_DTO.PRI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                    PR_DTO.PRI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                                    PR_DTO.PRI_BCH_No = Convert.ToString(row["BCH_No"]);
                                                    PR_DTO.PRI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                                    PR_DTO.PRI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                                    PR_DTO.PRI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                                    PR_DTO.Id = 25;
                                                    PR_DAO.PurchaseReturnDB(PR_DTO);
                                                }
                                            }
                                        }
                                    }

                                    foreach (var Exp in Expense_DTO)
                                    {
                                        PR_DTO.PRH_Number = Convert.ToInt64(PR_No);
                                        PR_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PRH_EXP_Expense_Number);
                                        PR_DTO.EXP_Remarks = Exp.PRH_EXP_Remarks;
                                        PR_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PRH_EXP_Occurrence_Number);
                                        PR_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PRH_EXP_CM_Number);
                                        PR_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PRH_EXP_ExpenseBase);
                                        PR_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PRH_EXP_Allocate_Number);
                                        PR_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PRH_EXP_SAC_Number);
                                        PR_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PRH_EXP_TaxCalculate);
                                        PR_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PRH_EXP_TaxValue);
                                        if (Exp.PRH_EXP_Number == 0)
                                        {
                                            PR_DTO.Id = 23;
                                        }
                                        else
                                        {
                                            PR_DTO.EXP_Number = Convert.ToInt64(Exp.PRH_EXP_Number);
                                            PR_DTO.Id = 108;
                                        }
                                        PR_DAO.PurchaseReturnDB(PR_DTO);
                                    }



                                    transaction.Complete();

                                    return RedirectToAction("PurchaseReturnRegisterSummary");
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
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
            //P_Head_DTO.ReturnItems = Item_DTO;
            //P_Head_DTO.Expenses = Expense_DTO;
            //P_Head_DTO.ItemExpenses = ItemExpense_DTO;
            //P_Head_DTO.ItemBatch = ItemBatch_DTO;

            ViewBag.PR_No = PR_No;
            PurchaseReturnGetData();
            return View(Original_PRH_DTO);
        }






        //Purchase Return GRN Create
        [Route("procurement/transactions/purchase-return/grn/create")]
        public IActionResult GRNToPurchaseReturnCreate()
        {
            GRNToPurchaseReturnTemp("3");
            GRNToPurchaseReturnHead_DTO PRH_DTO = new GRNToPurchaseReturnHead_DTO();
            PRH_DTO.PRH_ReturnDate = DateTime.Now.ToString("dd-MMM-yy");
            GRNToPurchaseReturnGetData();
            PRH_DTO.PRH_ReturnNo = OnPurchaseReturnNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            return View(PRH_DTO);
        }
        void GRNToPurchaseReturnGetData()
        {
            GRNPR_DTO.PRH_ReturnDate = Convert.ToString(DateTime.Now.ToString("yyyyMMdd"));
            GRNPR_DTO.Id = 1;
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

            ViewBag.ExpenseCode = Help.GetCat(DS.Tables[0]);
            ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[4]);
            ViewBag.UoM = Help.GetCat(DS.Tables[5]);
            ViewBag.HSN = Help.GetCat(DS.Tables[6]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[7]);
            ViewBag.WHTax = Help.GetCat(DS.Tables[8]);
            ViewBag.IsCalculate = Help.GetCat(DS.Tables[9]);
        }

        [Route("procurement/transactions/purchase-return/grn/create")]
        [HttpPost]
        public IActionResult GRNToPurchaseReturnCreate(GRNToPurchaseReturnHead_DTO PRH_DTO, String? Mode)
        {
            var Original_PRH_DTO = Help.JsonClone(PRH_DTO);

            bool IsValid = false;
            GRNToPurchaseReturnHead_DTO P_Head_DTO = new GRNToPurchaseReturnHead_DTO();

            List<GRNToPurchaseReturnItem_DTO>? Item_DTO = new List<GRNToPurchaseReturnItem_DTO>();
            List<GRNToPurchaseReturnExpense_DTO>? Expense_DTO = new List<GRNToPurchaseReturnExpense_DTO>();
            List<GRNToPurchaseReturnIExpense_DTO>? ItemExpense_DTO = new List<GRNToPurchaseReturnIExpense_DTO>();
            List<GRNToPurchaseReturnBatch_DTO>? ItemBatch_DTO = new List<GRNToPurchaseReturnBatch_DTO>();

            P_Head_DTO = PRH_DTO;

            if (PRH_DTO.ReturnItems != null)
                Item_DTO = PRH_DTO.ReturnItems!.Where(K => K.PRI_IsDeleted == "false").ToList();

            if (PRH_DTO.Expenses != null)
                Expense_DTO = PRH_DTO.Expenses!.Where(K => K.PRH_EXP_IsDeleted == "false").ToList();

            if (PRH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PRH_DTO.ItemExpenses!.Where(K => K.PRI_EXP_IsDeleted == "false").ToList();

            if (PRH_DTO.ItemBatch != null)
                ItemBatch_DTO = PRH_DTO.ItemBatch!.Where(K => K.PRI_BCH_IsDeleted == "false").ToList();

            if (Mode == "Save")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PRI_MS_Number) != Convert.ToInt64(PRH_DTO.PRH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PRI_Qty) == 0 || Convert.ToDouble(x.PRI_UnitPrice) == 0 || Convert.ToDouble(x.PRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (Item_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(PRH_DTO.PRH_Vendor_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Vendor is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(PRH_DTO.PRH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PRH_DTO.PRH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();

                    P_Head_DTO.ReturnItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = ItemBatch_DTO;

                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        if (GRNToPurchaseReturnBatchValidation(Item_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);

                                    String ReturnNoOld = PRH_DTO.PRH_ReturnNo;
                                    String ReturnNoNew = OnPurchaseReturnNumber(Convert.ToInt32(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd")));

                                    GRNPR_DTO.PRH_ReturnNo = ReturnNoNew;
                                    GRNPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd"));
                                    GRNPR_DTO.PRH_Vendor_Number = Convert.ToString(PRH_DTO.PRH_Vendor_Number);
                                    GRNPR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PRH_DTO.PRH_ImportOrder) ? 1 : 0);
                                    GRNPR_DTO.PRH_Currency_Number = Convert.ToString(PRH_DTO.PRH_Currency_Number);
                                    GRNPR_DTO.PRH_MS_Number = Convert.ToString(PRH_DTO.PRH_MS_Number);
                                    GRNPR_DTO.PRH_ExchangeRate = Convert.ToDouble(PRH_DTO.PRH_ExchangeRate);
                                    GRNPR_DTO.PRH_TaxCluster_Number = Convert.ToString(PRH_DTO.PRH_TaxCluster_Number);
                                    GRNPR_DTO.PRH_WHT_Number = Convert.ToString(PRH_DTO.PRH_WHT_Number);
                                    GRNPR_DTO.PRH_MaterialCost = Convert.ToDouble(PRH_DTO.PRH_MaterialCost).ToString();
                                    GRNPR_DTO.PRH_ItemMiscExpense = Convert.ToDouble(PRH_DTO.PRH_ItemMiscExpense).ToString();
                                    GRNPR_DTO.PRH_HeaderMiscExpense = Convert.ToDouble(PRH_DTO.PRH_HeaderMiscExpense).ToString();
                                    GRNPR_DTO.PRH_GST_Amount = Convert.ToDouble(PRH_DTO.PRH_GST_Amount).ToString();
                                    GRNPR_DTO.PRH_ReturnAmount = Convert.ToDouble(PRH_DTO.PRH_ReturnAmount).ToString();
                                    GRNPR_DTO.PRH_WHT_Amount = Convert.ToDouble(PRH_DTO.PRH_WHT_Amount).ToString();
                                    GRNPR_DTO.PRH_RoundOff = Convert.ToDouble(PRH_DTO.PRH_RoundOff).ToString();
                                    GRNPR_DTO.PRH_VendorReceivable = Convert.ToDouble(PRH_DTO.PRH_VendorReceivable).ToString();
                                    GRNPR_DTO.PRH_DeliveryTerms = Convert.ToString(PRH_DTO.PRH_DeliveryTerms);
                                    GRNPR_DTO.PRH_DeliveryMode = Convert.ToString(PRH_DTO.PRH_DeliveryMode);
                                    GRNPR_DTO.Id = 21;
                                    DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

                                    OnPurchaseReturnNumberGen(Convert.ToInt32(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        GRNPR_DTO.PRH_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in Item_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            GRNPR_DTO.PRI_PIH_Number = Convert.ToInt64(Item.PRI_PIH_Number);
                                            GRNPR_DTO.PRI_PII_Number = Convert.ToInt64(Item.PRI_PII_Number);
                                            GRNPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                            GRNPR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_Warehouse_Number);
                                            GRNPR_DTO.PRI_UoM_Number = Convert.ToInt64(Item.PRI_UoM_Number).ToString();
                                            GRNPR_DTO.PRI_Qty = Convert.ToDouble(Item.PRI_Qty).ToString();
                                            GRNPR_DTO.PRI_UnitPrice = Convert.ToDouble(Item.PRI_UnitPrice).ToString();
                                            GRNPR_DTO.PRI_Amount = Convert.ToDouble(Item.PRI_Amount).ToString();
                                            GRNPR_DTO.PRI_ExpenseValue = Convert.ToDouble(Item.PRI_ExpenseValue);
                                            GRNPR_DTO.PRI_HSN_Number = Convert.ToString(Item.PRI_HSN_Number);
                                            GRNPR_DTO.PRI_GST_Amount = Convert.ToDouble(Item.PRI_GST_Amount).ToString();
                                            GRNPR_DTO.PRI_WHT_Percent = Convert.ToDouble(Item.PRI_WHT_Percent).ToString();
                                            GRNPR_DTO.PRI_WHT_Amount = Convert.ToDouble(Item.PRI_WHT_Amount).ToString();
                                            GRNPR_DTO.Id = 22;
                                            D = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PRI_EXP_PII_Number == Item.PRI_PII_Number) && (x.PRI_EXP_PIH_Number == Item.PRI_PIH_Number));

                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                GRNPR_DTO.PRI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                GRNPR_DTO.EXP_Number = Convert.ToInt64(ItemExp.PRI_EXP_Number);
                                                GRNPR_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PRI_EXP_Expense_Number);
                                                GRNPR_DTO.EXP_Remarks = ItemExp.PRI_EXP_Remarks;
                                                GRNPR_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PRI_EXP_Occurrence_Number);
                                                GRNPR_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PRI_EXP_CM_Number);
                                                GRNPR_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PRI_EXP_ExpenseBase);
                                                GRNPR_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PRI_EXP_ExpenseValue);
                                                GRNPR_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PRI_EXP_Allocate_Number);
                                                GRNPR_DTO.Id = 24;
                                                GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
                                            }

                                            GRNPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                            GRNPR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                            GRNPR_DTO.PRI_Op2 = Convert.ToString(3);
                                            GRNPR_DTO.Id = 157;
                                            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
                                            if (DS.Tables[0].Rows.Count > 0)
                                            {
                                                DataTable dt = DS.Tables[0];
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    GRNPR_DTO.BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                                    GRNPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                                    GRNPR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                                    GRNPR_DTO.PRI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                    GRNPR_DTO.PRI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                                    GRNPR_DTO.PRI_BCH_No = Convert.ToString(row["BCH_No"]);
                                                    GRNPR_DTO.PRI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                                    GRNPR_DTO.PRI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                                    GRNPR_DTO.PRI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                                    GRNPR_DTO.Id = 25;
                                                    GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
                                                }
                                            }

                                            //var Batch = ItemBatch_DTO.Where(x => (x.PRI_BCH_PII_Number == Item.PRI_PII_Number) && (x.PRI_BCH_PIH_Number == Item.PRI_PIH_Number));

                                            //foreach (var ItemBatch in Batch)
                                            //{
                                            //    GRNPR_DTO.PRI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                            //    GRNPR_DTO.PRI_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PRI_BCH_Date).ToString("yyyyMMdd"));
                                            //    GRNPR_DTO.PRI_BCH_No = Convert.ToString(ItemBatch.PRI_BCH_No);
                                            //    GRNPR_DTO.PRI_BCH_Qty = Convert.ToDouble(ItemBatch.PRI_BCH_Qty);
                                            //    GRNPR_DTO.PRI_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PRI_BCH_UnitPrice);
                                            //    GRNPR_DTO.PRI_BCH_Value = Convert.ToDouble(ItemBatch.PRI_BCH_Value);
                                            //    GRNPR_DTO.Id = 25;
                                            //    GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
                                            //}
                                        }
                                        foreach (var Exp in Expense_DTO)
                                        {
                                            GRNPR_DTO.EXP_Number = Convert.ToInt64(Exp.PRH_EXP_Number);
                                            GRNPR_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PRH_EXP_Expense_Number);
                                            GRNPR_DTO.EXP_Remarks = Exp.PRH_EXP_Remarks;
                                            GRNPR_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PRH_EXP_Occurrence_Number);
                                            GRNPR_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PRH_EXP_CM_Number);
                                            GRNPR_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PRH_EXP_ExpenseBase);
                                            GRNPR_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.PRH_EXP_ExpenseValue);
                                            GRNPR_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PRH_EXP_Allocate_Number);
                                            GRNPR_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PRH_EXP_SAC_Number);
                                            GRNPR_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PRH_EXP_TaxCalculate);
                                            GRNPR_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PRH_EXP_TaxValue);
                                            GRNPR_DTO.Id = 23;
                                            GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    P_Head_DTO.Reset();
                                    Expense_DTO = null;
                                    Item_DTO = null;
                                    ItemExpense_DTO = null;
                                    PRH_DTO.Reset();

                                    Original_PRH_DTO = Help.JsonClone(PRH_DTO);
                                    return RedirectToAction("GRNToPurchaseReturnCreate");
                                    //P_Head_DTO.PRH_ReturnDate = DateTime.Now.ToString("dd-MMM-yy");

                                    //if (ReturnNoOld != ReturnNoNew)
                                    //{
                                    //    ViewBag.ErrorCode = 2;
                                    //    ViewBag.ErrorMessage = "Purchase Return number " + ReturnNoOld + " used by another user. Next number will be allotted to you.";
                                    //}
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
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


            PurchaseReturnGetData();
            return View(Original_PRH_DTO);
        }
        Boolean GRNToPurchaseReturnBatchValidation(List<GRNToPurchaseReturnItem_DTO> Item_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                Double BatchAmount = 0;

                Int64 PRINumber = Convert.ToInt64(Item.PRI_Number);
                Int64 PRIIndex = Convert.ToInt64(Item.PRI_Index);
                String PRIItem = Convert.ToString(Item.PRI_Item_Number);
                Double PRIQty = Convert.ToDouble(Item.PRI_Qty);
                Double PRIAmount = Convert.ToDouble(Item.PRI_Amount);

                GRNPR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_Warehouse_Number);
                GRNPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                GRNPR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                GRNPR_DTO.PRI_Op2 = Convert.ToString(3);
                GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                GRNPR_DTO.Id = 156;
                DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    BatchQty = Convert.ToDouble(DS.Tables[0].Rows[0][0].ToString());
                }

                if (PRIQty < 0)
                {
                    PRIQty = PRIQty * -1;
                }

                if (BatchQty == PRIQty) { }
                else
                {
                    Message += Item.PRI_ItemCode + " Batch Qty  Mismatched <br/>";
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


        [Route("procurement/transactions/purchase-return/grn/vendor")]
        public IActionResult GRNToPurchaseReturnVendor(String? Vendor, String? PRHDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (PRHDate == null)
            {
                PRHDate = "0";
            }
            GRNPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            GRNPR_DTO.PRI_ItemCode = Convert.ToString(Vendor).Trim();
            GRNPR_DTO.Id = 2;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
            var Ven = P_DL.PRVList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-return/grn/cluster")]
        public IActionResult GRNToPurchaseReturnCluster(String? Vendor, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.Search = Cluster;
            GRNPR_DTO.PRH_Vendor_Number = Vendor;
            GRNPR_DTO.Id = 3;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
            var InvCluster = P_DL.PRCluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("procurement/transactions/purchase-return/grn/invoice")]
        public IActionResult GRNToPurchaseReturnInvoice(String? Vendor, String? Import, String? MSNumber, String? PRHDate)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }

            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.PRH_MS_Number = MSNumber;
            GRNPR_DTO.PRH_Vendor_Number = Vendor;
            GRNPR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 1 : 0);
            GRNPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNPR_DTO.Id = 4;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
            var PRInvoice = P_DL.PRInvoice(DS.Tables[0]);
            return Json(PRInvoice);
        }

        [Route("procurement/transactions/purchase-return/grn/invoice/item")]
        public IActionResult GRNToPurchaseReturnInvoiceItem(String? PIHNumber)
        {
            if (PIHNumber == null)
            {
                PIHNumber = "";
            }

            GRNPR_DTO.Search = PIHNumber;
            GRNPR_DTO.Id = 5;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

            PIToPR_DTO PIPR = new PIToPR_DTO();
            var PRItems = P_DL.PRInvoiceGet(DS.Tables[0]);
            var PRExpenses = P_DL.PRExpenseGet(DS.Tables[1]);
            var PRItemExpenses = P_DL.PRIExpenseGet(DS.Tables[2]);

            PIPR.PRItems = PRItems;
            PIPR.PRExpenses = PRExpenses;
            PIPR.PRItemExpenses = PRItemExpenses;

            return Json(PIPR);
        }

        [Route("procurement/transactions/purchase-return/grn/gst")]
        public String GRNToPurchaseReturnGst(String? Cluster, String? PRHDate, String? HSN, String? BaseAmount)
        {
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNPR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            GRNPR_DTO.PRI_HSN_Number = Convert.ToString(HSN);
            GRNPR_DTO.Id = 6;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = P_DL.PurInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("procurement/transactions/purchase-return/grn/vendor/get")]
        public IActionResult GRNToPurchaseReturnVendorGet(String? Vendor, String? PRHDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (PRHDate == null)
            {
                PRHDate = "0";
            }
            GRNPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            GRNPR_DTO.PRH_Vendor_Number = Convert.ToString(Vendor).Trim();
            GRNPR_DTO.Id = 9;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
            var Ven = P_DL.PRVList(DS.Tables[0]).FirstOrDefault();
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-return/grn/wht")]
        public IActionResult GRNToPurchaseReturnWHT(String? Vendor, String? WHTNumber, String? PRHDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Vendor == null)
            {
                Vendor = "0";
            }
            if (PRHDate == null)
            {
                PRHDate = "0";
            }
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.PRH_Vendor_Number = Convert.ToString(Vendor);
            GRNPR_DTO.PRH_WHT_Number = Convert.ToString(WHTNumber);
            GRNPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNPR_DTO.Id = 8;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
            var WHT = P_DL.PurInvWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("procurement/transactions/purchase-return/grn/gst/view")]
        public IActionResult GRNToPurchaseReturnGstView(String? Cluster, String? PRHDate, String? HSN, String? BaseAmount)
        {
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNPR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            GRNPR_DTO.PRI_HSN_Number = Convert.ToString(HSN);
            GRNPR_DTO.Id = 10;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<PurchaseReturnGst> PurGST = new List<PurchaseReturnGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = P_DL.PurInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new PurchaseReturnGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("procurement/transactions/purchase-return/grn/expense/gst")]
        public String GRNToPurchaseReturnHeaderGst(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            GRNPR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            GRNPR_DTO.EXP_SAC_Number = Convert.ToInt64(SAC);
            GRNPR_DTO.Id = 11;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = P_DL.PurInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("procurement/transactions/purchase-return/grn/expense/gst/view")]
        public IActionResult GRNToPurchaseReturnGstHeaderView(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            GRNPR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            GRNPR_DTO.EXP_SAC_Number = Convert.ToInt64(SAC);
            GRNPR_DTO.Id = 12;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<PurchaseInvoiceGst> PurGST = new List<PurchaseInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = P_DL.PurInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new PurchaseInvoiceGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("procurement/transactions/purchase-return/grn/batch/get")]
        public IActionResult GRNToPurchaseReturnBatchGet(String? ItemNumber, String? Index, String? Warehouse)
        {
            if (ItemNumber == null)
            {
                ItemNumber = "";
            }
            if (Index == null)
            {
                Index = "0";
            }
            if (Warehouse == null)
            {
                Warehouse = "0";
            }

            GRNPR_DTO.PRI_Item_Number = Convert.ToString(ItemNumber);
            GRNPR_DTO.PRI_Warehouse_Number = Convert.ToString(Warehouse);
            GRNPR_DTO.PRI_Op1 = Convert.ToString(Index);
            GRNPR_DTO.PRI_Op2 = Convert.ToString(3);
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.Id = 151;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

            PRBatchItem_DTO PRB = new PRBatchItem_DTO();
            PRB.PRBatch = P_DL.PRBatchList(DS.Tables[0]);
            PRB.PRView = P_DL.PRBatchOverallList(DS.Tables[1]);
            return Json(PRB);
        }

        [Route("procurement/transactions/purchase-return/grn/batch/post")]
        [HttpPost]
        public IActionResult GRNToPurchaseReturnBatchPost([FromBody] MainItemBatch ItemBatch)
        {
            GRNPR_DTO.PRI_Op2 = Convert.ToString(3);
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);

            //var Items = ItemBatch.ItemBatch.Where(R => R.PRI_BCH_Qty > 0);
            foreach (var Item in ItemBatch.ItemBatch)
            {
                GRNPR_DTO.BCH_Number = Convert.ToInt64(Item.PRI_BCH_Number);
                GRNPR_DTO.PRI_BCH_Number = Convert.ToInt64(Item.PRI_BCH_BCH_Number);
                GRNPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_BCH_Item_Number);
                GRNPR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_BCH_Item_Index);
                GRNPR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_BCH_Warehouse_Number);
                GRNPR_DTO.PRI_BCH_Date = Convert.ToString(Item.PRI_BCH_Date);
                GRNPR_DTO.PRI_BCH_No = Convert.ToString(Item.PRI_BCH_No);
                GRNPR_DTO.PRI_BCH_Qty = Convert.ToDouble(Item.PRI_BCH_Qty);
                GRNPR_DTO.PRI_BCH_UnitPrice = Convert.ToDouble(Item.PRI_BCH_UnitPrice);
                GRNPR_DTO.PRI_BCH_Value = Convert.ToDouble(Item.PRI_BCH_Value);
                GRNPR_DTO.Id = 153;
                DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (Item.PRI_BCH_Qty > 0)
                    {
                        GRNPR_DTO.Id = 154;
                        GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
                    }
                    else
                    {
                        GRNPR_DTO.Id = 155;
                        GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
                    }
                    //GRNPR_DTO.Id = 154;
                    //GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
                }
                else
                {
                    if (Item.PRI_BCH_Qty > 0)
                    {
                        GRNPR_DTO.Id = 152;
                        GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
                    }
                }
            }

            PRBatchItem_DTO PRB = new PRBatchItem_DTO();
            return Json(PRB);
        }
        void GRNToPurchaseReturnTemp(String Mode)
        {
            GRNPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNPR_DTO.PRI_Op2 = Convert.ToString(Mode);
            GRNPR_DTO.Id = 161;
            DS = GRNPR_DAO.GRNToPurchaseReturnDB(GRNPR_DTO);
        }


        [Route("procurement/transactions/purchase-return/grn/clear-temp")]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public IActionResult ClearGRNToPurchaseReturnTemp()
        {
            GRNToPurchaseReturnTemp("3");
            return Json(0);
        }







        //Purchase Return GRN Item Create
        [Route("procurement/transactions/purchase-return/grnitem/create")]
        public IActionResult GRNItemToPurchaseReturnCreate()
        {
            GRNItemToPurchaseReturnHead_DTO PRH_DTO = new GRNItemToPurchaseReturnHead_DTO();
            PRH_DTO.PRH_ReturnDate = DateTime.Now.ToString("dd-MMM-yy");
            GRNItemToPurchaseReturnGetData();
            PRH_DTO.PRH_ReturnNo = OnPurchaseReturnNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            return View(PRH_DTO);
        }
        void GRNItemToPurchaseReturnGetData()
        {
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(DateTime.Now.ToString("yyyyMMdd"));
            GRNIPR_DTO.Id = 1;
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

            ViewBag.ExpenseCode = Help.GetCat(DS.Tables[0]);
            ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[4]);
            ViewBag.UoM = Help.GetCat(DS.Tables[5]);
            ViewBag.HSN = Help.GetCat(DS.Tables[6]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[7]);
            ViewBag.WHTax = Help.GetCat(DS.Tables[8]);
            ViewBag.IsCalculate = Help.GetCat(DS.Tables[9]);
        }

        [Route("procurement/transactions/purchase-return/grnitem/create")]
        [HttpPost]
        public IActionResult GRNItemToPurchaseReturnCreate(GRNItemToPurchaseReturnHead_DTO PRH_DTO, String? Mode)
        {
            var Original_PRH_DTO = Help.JsonClone(PRH_DTO);

            bool IsValid = false;
            GRNItemToPurchaseReturnHead_DTO P_Head_DTO = new GRNItemToPurchaseReturnHead_DTO();

            List<GRNItemToPurchaseReturnItem_DTO>? Item_DTO = new List<GRNItemToPurchaseReturnItem_DTO>();
            List<GRNItemToPurchaseReturnExpense_DTO>? Expense_DTO = new List<GRNItemToPurchaseReturnExpense_DTO>();
            List<GRNItemToPurchaseReturnIExpense_DTO>? ItemExpense_DTO = new List<GRNItemToPurchaseReturnIExpense_DTO>();
            List<GRNItemToPurchaseReturnBatch_DTO>? ItemBatch_DTO = new List<GRNItemToPurchaseReturnBatch_DTO>();

            P_Head_DTO = PRH_DTO;

            if (PRH_DTO.ReturnItems != null)
                Item_DTO = PRH_DTO.ReturnItems!.Where(K => K.PRI_IsDeleted == "false").ToList();

            if (PRH_DTO.Expenses != null)
                Expense_DTO = PRH_DTO.Expenses!.Where(K => K.PRH_EXP_IsDeleted == "false").ToList();

            if (PRH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PRH_DTO.ItemExpenses!.Where(K => K.PRI_EXP_IsDeleted == "false").ToList();

            if (PRH_DTO.ItemBatch != null)
                ItemBatch_DTO = PRH_DTO.ItemBatch!.Where(K => K.PRI_BCH_IsDeleted == "false").ToList();

            PR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PRI_MS_Number) != Convert.ToInt64(PRH_DTO.PRH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PRI_Qty) == 0 || Convert.ToDouble(x.PRI_UnitPrice) == 0 || Convert.ToDouble(x.PRI_Amount) == 0);

                if (CheckItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Material Segregation and Item Mismatched";
                }
                else if (ValueItem.ToList().Count > 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Qty or Unit Price Must above one";
                }
                else if (Item_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(PRH_DTO.PRH_Vendor_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Vendor is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(PRH_DTO.PRH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PRH_DTO.PRH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();

                    P_Head_DTO.ReturnItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = ItemBatch_DTO;

                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        if (GRNItemToPurchaseReturnBatchValidation(Item_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String ReturnNoOld = PRH_DTO.PRH_ReturnNo;
                                    String ReturnNoNew = OnPurchaseReturnNumber(Convert.ToInt32(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd")));

                                    GRNIPR_DTO.PRH_ReturnNo = ReturnNoNew;
                                    GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd"));
                                    GRNIPR_DTO.PRH_Vendor_Number = Convert.ToString(PRH_DTO.PRH_Vendor_Number);
                                    GRNIPR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PRH_DTO.PRH_ImportOrder) ? 1 : 0);
                                    GRNIPR_DTO.PRH_Currency_Number = Convert.ToString(PRH_DTO.PRH_Currency_Number);
                                    GRNIPR_DTO.PRH_MS_Number = Convert.ToString(PRH_DTO.PRH_MS_Number);
                                    GRNIPR_DTO.PRH_ExchangeRate = Convert.ToDouble(PRH_DTO.PRH_ExchangeRate);
                                    GRNIPR_DTO.PRH_TaxCluster_Number = Convert.ToString(PRH_DTO.PRH_TaxCluster_Number);
                                    GRNIPR_DTO.PRH_WHT_Number = Convert.ToString(PRH_DTO.PRH_WHT_Number);
                                    GRNIPR_DTO.PRH_MaterialCost = Convert.ToDouble(PRH_DTO.PRH_MaterialCost).ToString();
                                    GRNIPR_DTO.PRH_ItemMiscExpense = Convert.ToDouble(PRH_DTO.PRH_ItemMiscExpense).ToString();
                                    GRNIPR_DTO.PRH_HeaderMiscExpense = Convert.ToDouble(PRH_DTO.PRH_HeaderMiscExpense).ToString();
                                    GRNIPR_DTO.PRH_GST_Amount = Convert.ToDouble(PRH_DTO.PRH_GST_Amount).ToString();
                                    GRNIPR_DTO.PRH_ReturnAmount = Convert.ToDouble(PRH_DTO.PRH_ReturnAmount).ToString();
                                    GRNIPR_DTO.PRH_WHT_Amount = Convert.ToDouble(PRH_DTO.PRH_WHT_Amount).ToString();
                                    GRNIPR_DTO.PRH_RoundOff = Convert.ToDouble(PRH_DTO.PRH_RoundOff).ToString();
                                    GRNIPR_DTO.PRH_VendorReceivable = Convert.ToDouble(PRH_DTO.PRH_VendorReceivable).ToString();
                                    GRNIPR_DTO.PRH_DeliveryTerms = Convert.ToString(PRH_DTO.PRH_DeliveryTerms);
                                    GRNIPR_DTO.PRH_DeliveryMode = Convert.ToString(PRH_DTO.PRH_DeliveryMode);
                                    GRNIPR_DTO.Id = 21;
                                    DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

                                    OnPurchaseReturnNumberGen(Convert.ToInt32(Convert.ToDateTime(PRH_DTO.PRH_ReturnDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        GRNIPR_DTO.PRH_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in Item_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            GRNIPR_DTO.PRI_PIH_Number = Convert.ToInt64(Item.PRI_PIH_Number);
                                            GRNIPR_DTO.PRI_PII_Number = Convert.ToInt64(Item.PRI_PII_Number);
                                            GRNIPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                            GRNIPR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_Warehouse_Number);
                                            GRNIPR_DTO.PRI_UoM_Number = Convert.ToInt64(Item.PRI_UoM_Number).ToString();
                                            GRNIPR_DTO.PRI_Qty = Convert.ToDouble(Item.PRI_Qty).ToString();
                                            GRNIPR_DTO.PRI_UnitPrice = Convert.ToDouble(Item.PRI_UnitPrice).ToString();
                                            GRNIPR_DTO.PRI_Amount = Convert.ToDouble(Item.PRI_Amount).ToString();
                                            GRNIPR_DTO.PRI_ExpenseValue = Convert.ToDouble(Item.PRI_ExpenseValue);
                                            GRNIPR_DTO.PRI_HSN_Number = Convert.ToString(Item.PRI_HSN_Number);
                                            GRNIPR_DTO.PRI_GST_Amount = Convert.ToDouble(Item.PRI_GST_Amount).ToString();
                                            GRNIPR_DTO.PRI_WHT_Percent = Convert.ToDouble(Item.PRI_WHT_Percent).ToString();
                                            GRNIPR_DTO.PRI_WHT_Amount = Convert.ToDouble(Item.PRI_WHT_Amount).ToString();
                                            GRNIPR_DTO.Id = 22;
                                            D = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PRI_EXP_PII_Number == Item.PRI_PII_Number) && (x.PRI_EXP_PIH_Number == Item.PRI_PIH_Number));

                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                GRNIPR_DTO.PRI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                GRNIPR_DTO.EXP_Number = Convert.ToInt64(ItemExp.PRI_EXP_Number);
                                                GRNIPR_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PRI_EXP_Expense_Number);
                                                GRNIPR_DTO.EXP_Remarks = ItemExp.PRI_EXP_Remarks;
                                                GRNIPR_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PRI_EXP_Occurrence_Number);
                                                GRNIPR_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PRI_EXP_CM_Number);
                                                GRNIPR_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PRI_EXP_ExpenseBase);
                                                GRNIPR_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PRI_EXP_ExpenseValue);
                                                GRNIPR_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PRI_EXP_Allocate_Number);
                                                GRNIPR_DTO.Id = 24;
                                                GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
                                            }

                                            GRNIPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                            GRNIPR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                            GRNIPR_DTO.PRI_Op2 = Convert.ToString(5);
                                            GRNIPR_DTO.Id = 157;
                                            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
                                            if (DS.Tables[0].Rows.Count > 0)
                                            {
                                                DataTable dt = DS.Tables[0];
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    GRNIPR_DTO.BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                                    GRNIPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                                                    GRNIPR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                                                    GRNIPR_DTO.PRI_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                    GRNIPR_DTO.PRI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                                    GRNIPR_DTO.PRI_BCH_No = Convert.ToString(row["BCH_No"]);
                                                    GRNIPR_DTO.PRI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                                    GRNIPR_DTO.PRI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                                    GRNIPR_DTO.PRI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                                    GRNIPR_DTO.Id = 25;
                                                    GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
                                                }
                                            }
                                        }
                                        foreach (var Exp in Expense_DTO)
                                        {
                                            GRNIPR_DTO.EXP_Number = Convert.ToInt64(Exp.PRH_EXP_Number);
                                            GRNIPR_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PRH_EXP_Expense_Number);
                                            GRNIPR_DTO.EXP_Remarks = Exp.PRH_EXP_Remarks;
                                            GRNIPR_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PRH_EXP_Occurrence_Number);
                                            GRNIPR_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PRH_EXP_CM_Number);
                                            GRNIPR_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PRH_EXP_ExpenseBase);
                                            GRNIPR_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.PRH_EXP_ExpenseValue);
                                            GRNIPR_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PRH_EXP_Allocate_Number);
                                            GRNIPR_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PRH_EXP_SAC_Number);
                                            GRNIPR_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PRH_EXP_TaxCalculate);
                                            GRNIPR_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PRH_EXP_TaxValue);
                                            GRNIPR_DTO.Id = 23;
                                            GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    P_Head_DTO.Reset();
                                    Expense_DTO = null;
                                    Item_DTO = null;
                                    ItemExpense_DTO = null;
                                    PRH_DTO.Reset();

                                    Original_PRH_DTO = Help.JsonClone(PRH_DTO);
                                    return RedirectToAction("GRNItemToPurchaseReturnCreate");
                                    //P_Head_DTO.PRH_ReturnDate = DateTime.Now.ToString("dd-MMM-yy");

                                    //if (ReturnNoOld != ReturnNoNew)
                                    //{
                                    //    ViewBag.ErrorCode = 2;
                                    //    ViewBag.ErrorMessage = "Purchase Return number " + ReturnNoOld + " used by another user. Next number will be allotted to you.";
                                    //}
                                }
                                catch (Exception ex)
                                {
                                    transaction.Dispose();
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
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


            PurchaseReturnGetData();
            return View(Original_PRH_DTO);
        }
        Boolean GRNItemToPurchaseReturnBatchValidation(List<GRNItemToPurchaseReturnItem_DTO> Item_DTO, List<GRNItemToPurchaseReturnBatch_DTO>? ItemBatch_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                Double BatchAmount = 0;

                Int64 PRINumber = Convert.ToInt64(Item.PRI_Number);
                Int64 PRIIndex = Convert.ToInt64(Item.PRI_Index);
                String PRIItem = Convert.ToString(Item.PRI_Item_Number);
                Double PRIQty = Convert.ToDouble(Item.PRI_Qty);
                Double PRIAmount = Convert.ToDouble(Item.PRI_Amount);

                GRNIPR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_Warehouse_Number);
                GRNIPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_Item_Number);
                GRNIPR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_Index);
                GRNIPR_DTO.PRI_Op2 = Convert.ToString(5);
                GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
                GRNIPR_DTO.Id = 156;
                DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    BatchQty = Convert.ToDouble(DS.Tables[0].Rows[0][0].ToString());
                }

                if (PRIQty < 0)
                {
                    PRIQty = PRIQty * -1;
                }

                if (BatchQty == PRIQty) { }
                else
                {
                    Message += Item.PRI_ItemCode + " Batch Qty  Mismatched <br/>";
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


        [Route("procurement/transactions/purchase-return/grnitem/vendor")]
        public IActionResult GRNItemToPurchaseReturnVendor(String? Vendor, String? PRHDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (PRHDate == null)
            {
                PRHDate = "0";
            }
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            GRNIPR_DTO.PRI_ItemCode = Convert.ToString(Vendor).Trim();
            GRNIPR_DTO.Id = 2;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
            var Ven = P_DL.PRVList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-return/grnitem/cluster")]
        public IActionResult GRNItemToPurchaseReturnCluster(String? Vendor, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.Search = Cluster;
            GRNIPR_DTO.PRH_Vendor_Number = Vendor;
            GRNIPR_DTO.Id = 3;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
            var InvCluster = P_DL.PRCluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("procurement/transactions/purchase-return/grnitem/invoice")]
        public IActionResult GRNItemToPurchaseReturnInvoice(String? Vendor, String? Import, String? MSNumber, String? PRHDate)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }

            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRH_MS_Number = MSNumber;
            GRNIPR_DTO.PRH_Vendor_Number = Vendor;
            GRNIPR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 1 : 0);
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNIPR_DTO.Id = 4;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
            var PRInvoice = P_DL.PRInvoiceItem(DS.Tables[0]);
            return Json(PRInvoice);
        }

        [Route("procurement/transactions/purchase-return/grnitem/invoice/item")]
        public IActionResult GRNItemToPurchaseReturnInvoiceItem(String? PIHNumber, String? Vendor, String? Import, String? MSNumber, String? PRHDate)
        {
            if (PIHNumber == null)
            {
                PIHNumber = "";
            }
            if (Vendor == null)
            {
                Vendor = "";
            }

            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRH_MS_Number = MSNumber;
            GRNIPR_DTO.PRH_Vendor_Number = Vendor;
            GRNIPR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 1 : 0);
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNIPR_DTO.Search = PIHNumber;
            GRNIPR_DTO.Id = 5;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

            PIToItemPR_DTO PIPR = new PIToItemPR_DTO();
            var PRItems = P_DL.ItemPRInvoiceGet(DS.Tables[0]);
            var PRExpenses = P_DL.ItemPRExpenseGet(DS.Tables[1]);
            var PRItemExpenses = P_DL.ItemPRIExpenseGet(DS.Tables[2]);

            PIPR.PRItems = PRItems;
            PIPR.PRExpenses = PRExpenses;
            PIPR.PRItemExpenses = PRItemExpenses;

            return Json(PIPR);
        }

        [Route("procurement/transactions/purchase-return/grnitem/gst")]
        public String GRNItemToPurchaseReturnGst(String? Cluster, String? PRHDate, String? HSN, String? BaseAmount)
        {
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNIPR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            GRNIPR_DTO.PRI_HSN_Number = Convert.ToString(HSN);
            GRNIPR_DTO.Id = 6;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = P_DL.PurInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("procurement/transactions/purchase-return/grnitem/vendor/get")]
        public IActionResult GRNItemToPurchaseReturnVendorGet(String? Vendor, String? PRHDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (PRHDate == null)
            {
                PRHDate = "0";
            }
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            GRNIPR_DTO.PRH_Vendor_Number = Convert.ToString(Vendor).Trim();
            GRNIPR_DTO.Id = 9;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
            var Ven = P_DL.PRVList(DS.Tables[0]).FirstOrDefault();
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-return/grnitem/wht")]
        public IActionResult GRNItemToPurchaseReturnWHT(String? Vendor, String? WHTNumber, String? PRHDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Vendor == null)
            {
                Vendor = "0";
            }
            if (PRHDate == null)
            {
                PRHDate = "0";
            }
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRH_Vendor_Number = Convert.ToString(Vendor);
            GRNIPR_DTO.PRH_WHT_Number = Convert.ToString(WHTNumber);
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNIPR_DTO.Id = 8;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
            var WHT = P_DL.PurInvWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("procurement/transactions/purchase-return/grnitem/gst/view")]
        public IActionResult GRNItemToPurchaseReturnGstView(String? Cluster, String? PRHDate, String? HSN, String? BaseAmount)
        {
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(PRHDate).ToString("yyyyMMdd"));
            GRNIPR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            GRNIPR_DTO.PRI_HSN_Number = Convert.ToString(HSN);
            GRNIPR_DTO.Id = 10;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<PurchaseReturnGst> PurGST = new List<PurchaseReturnGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = P_DL.PurInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new PurchaseReturnGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }

        [Route("procurement/transactions/purchase-return/grnitem/expense/gst")]
        public String GRNItemToPurchaseReturnHeaderGst(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            GRNIPR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            GRNIPR_DTO.EXP_SAC_Number = Convert.ToInt64(SAC);
            GRNIPR_DTO.Id = 11;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = P_DL.PurInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;

                var TaxElement = Group.Where(TE => TE.Calculation == 1).Select(TE => TE.TaxElement).FirstOrDefault();

                foreach (var item in Group)
                {
                    if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                    {
                        if (item.Percentage.HasValue)
                        {
                            Double ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                    else if (item.Calculation == 0)
                    {
                        Double BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];
                        if (item.Percentage.HasValue)
                        {
                            double ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                            GroupTotal += ItemTotal;
                        }
                    }
                }
                GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
            }

            Double OverallTotal = GroupTotals.Values.Sum();
            return OverallTotal.ToString();
        }

        [Route("procurement/transactions/purchase-return/grnitem/expense/gst/view")]
        public IActionResult GRNItemToPurchaseReturnGstHeaderView(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRH_ReturnDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            GRNIPR_DTO.PRH_TaxCluster_Number = Convert.ToString(Cluster);
            GRNIPR_DTO.EXP_SAC_Number = Convert.ToInt64(SAC);
            GRNIPR_DTO.Id = 12;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<PurchaseInvoiceGst> PurGST = new List<PurchaseInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = P_DL.PurInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

            foreach (var Group in TaxIndex)
            {
                Double GroupTotal = 0;
                Double GroupAssessableValue = 0;

                var calculationOneItems = Group.Where(TE => TE.Calculation == 1).ToList();
                if (calculationOneItems.Any())
                {
                    var TaxElement = calculationOneItems.First().TaxElement;

                    foreach (var item in Group)
                    {
                        Double ItemTotal = 0;
                        Double ItemValue = 0;
                        Double BaseElementValue = 0;

                        if (Convert.ToInt32(item.Chargeable) == 4 && item.Calculation == 1)
                        {
                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseValue;
                                ItemTotal = (BaseValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseValue;
                            }
                        }
                        else if (item.Calculation == 0)
                        {
                            if (!GroupTotals.ContainsKey(Convert.ToInt32(item.TaxElement)))
                            {
                                continue;
                            }

                            BaseElementValue = GroupTotals[Convert.ToInt32(item.TaxElement)];

                            if (item.Percentage.HasValue)
                            {
                                ItemValue += BaseElementValue;
                                ItemTotal = (BaseElementValue * (item.Percentage.Value / 100));
                                GroupTotal += ItemTotal;
                                GroupAssessableValue += BaseElementValue;
                            }
                        }
                    }

                    PurGST.Add(
                        new PurchaseInvoiceGst
                        {
                            TaxIndex = Group.Key,
                            TaxCategory = calculationOneItems.First().TaxCategory.ToString(),
                            TaxType = calculationOneItems.First().TaxType.ToString(),
                            TaxElement = calculationOneItems.First().TaxElementName.ToString(),
                            LoadonInventory = calculationOneItems.First().LoadonInventory == "1" ? "Yes" : "No",
                            LoadonInventoryPercent = calculationOneItems.First().LoadonInventoryPercent.ToString(),
                            Chargeable = calculationOneItems.First().Chargeable.ToString(),
                            Calculation = 1,
                            Percentage = Convert.ToDouble(calculationOneItems.First().Percentage),
                            AssessableValue = GroupAssessableValue,
                            Amount = GroupTotal,
                        });
                    GroupTotals[Convert.ToInt64(TaxElement)] = GroupTotal;
                }
            }
            return Json(PurGST);
        }


        [Route("procurement/transactions/purchase-return/grnitem/batch/get")]
        public IActionResult GRNIToPurchaseReturnBatchGet(String? ItemNumber, String? Index, String? Warehouse)
        {
            if (ItemNumber == null)
            {
                ItemNumber = "";
            }
            if (Index == null)
            {
                Index = "0";
            }
            if (Warehouse == null)
            {
                Warehouse = "0";
            }

            GRNIPR_DTO.PRI_Item_Number = Convert.ToString(ItemNumber);
            GRNIPR_DTO.PRI_Warehouse_Number = Convert.ToString(Warehouse);
            GRNIPR_DTO.PRI_Op1 = Convert.ToString(Index);
            GRNIPR_DTO.PRI_Op2 = Convert.ToString(5);
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.Id = 151;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

            PRBatchItem_DTO PRB = new PRBatchItem_DTO();
            PRB.PRBatch = P_DL.PRBatchList(DS.Tables[0]);
            PRB.PRView = P_DL.PRBatchOverallList(DS.Tables[1]);
            return Json(PRB);
        }

        [Route("procurement/transactions/purchase-return/grnitem/batch/post")]
        [HttpPost]
        public IActionResult GRNIToPurchaseReturnBatchPost([FromBody] MainItemBatch ItemBatch)
        {
            GRNIPR_DTO.PRI_Op2 = Convert.ToString(5);
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);

            //var Items = ItemBatch.ItemBatch.Where(R => R.PRI_BCH_Qty > 0);
            foreach (var Item in ItemBatch.ItemBatch)
            {
                GRNIPR_DTO.BCH_Number = Convert.ToInt64(Item.PRI_BCH_Number);
                GRNIPR_DTO.PRI_BCH_Number = Convert.ToInt64(Item.PRI_BCH_BCH_Number);
                GRNIPR_DTO.PRI_Item_Number = Convert.ToString(Item.PRI_BCH_Item_Number);
                GRNIPR_DTO.PRI_Op1 = Convert.ToString(Item.PRI_BCH_Item_Index);
                GRNIPR_DTO.PRI_Warehouse_Number = Convert.ToString(Item.PRI_BCH_Warehouse_Number);
                GRNIPR_DTO.PRI_BCH_Date = Convert.ToString(Item.PRI_BCH_Date);
                GRNIPR_DTO.PRI_BCH_No = Convert.ToString(Item.PRI_BCH_No);
                GRNIPR_DTO.PRI_BCH_Qty = Convert.ToDouble(Item.PRI_BCH_Qty);
                GRNIPR_DTO.PRI_BCH_UnitPrice = Convert.ToDouble(Item.PRI_BCH_UnitPrice);
                GRNIPR_DTO.PRI_BCH_Value = Convert.ToDouble(Item.PRI_BCH_Value);
                GRNIPR_DTO.Id = 153;
                DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (Item.PRI_BCH_Qty > 0)
                    {
                        GRNIPR_DTO.Id = 154;
                        GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
                    }
                    else
                    {
                        GRNIPR_DTO.Id = 155;
                        GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
                    }
                    //GRNIPR_DTO.Id = 154;
                    //GRNPR_DAO.GRNToPurchaseReturnDB(GRNIPR_DTO);
                }
                else
                {
                    if (Item.PRI_BCH_Qty > 0)
                    {
                        GRNIPR_DTO.Id = 152;
                        GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
                    }
                }
            }

            PRBatchItem_DTO PRB = new PRBatchItem_DTO();
            return Json(PRB);
        }
        void GRNIToPurchaseReturnTemp(String Mode)
        {
            GRNIPR_DTO.CreatorCode = Convert.ToInt32(UserCode);
            GRNIPR_DTO.PRI_Op2 = Convert.ToString(Mode);
            GRNIPR_DTO.Id = 161;
            DS = GRNIPR_DAO.GRNItemToPurchaseReturnDB(GRNIPR_DTO);
        }


        [Route("procurement/transactions/purchase-return/grnitem/clear-temp")]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public IActionResult ClearGRNIToPurchaseReturnTemp()
        {
            GRNToPurchaseReturnTemp("5");
            return Json(0);
        }






        //Purchase Return Numbering
        [Route("procurement/setup/purchase-return-numbering")]
        public IActionResult PRNumbering()
        {
            GetPRNumber();
            return View(PRN_DTO);
        }
        [Route("procurement/setup/purchase-return-numbering")]
        [HttpPost]
        public IActionResult PRNumbering(PRNumber_DTO PN_DTO)
        {
            bool IsValid = false;
            PRNumber_DTO P_Head_DTO = new PRNumber_DTO();

            List<PRNumberReset_DTO>? Reset_DTO = new List<PRNumberReset_DTO>();
            List<PRNumberPrefix_DTO>? Prefix_DTO = new List<PRNumberPrefix_DTO>();
            List<PRNumberSuffix_DTO>? Suffix_DTO = new List<PRNumberSuffix_DTO>();

            P_Head_DTO = PRN_DTO;

            if (PN_DTO.PRNumberReset != null)
                Reset_DTO = PN_DTO.PRNumberReset!.Where(K => !K.PRR_IsDeleted).ToList();

            if (PN_DTO.PRNumberPrefix != null)
                Prefix_DTO = PN_DTO.PRNumberPrefix!.Where(K => !K.PRP_IsDeleted).ToList();

            if (PN_DTO.PRNumberSuffix != null)
                Suffix_DTO = PN_DTO.PRNumberSuffix!.Where(K => !K.PRS_IsDeleted).ToList();

            if (PN_DTO.PRN_Method == "2")
            {
                String ResetDTO = string.Join(", ", Reset_DTO.Where(x => Convert.ToInt64(x.PRR_Number) != 0).Select(x => x.PRR_Number));
                String PrefixDTO = string.Join(", ", Prefix_DTO.Where(x => Convert.ToInt64(x.PRP_Number) != 0).Select(x => x.PRP_Number));
                String SuffixDTO = string.Join(", ", Suffix_DTO.Where(x => Convert.ToInt64(x.PRS_Number) != 0).Select(x => x.PRS_Number));

                PRN_DTO.DeleteNumbers = Convert.ToString(ResetDTO);
                PRN_DTO.Id = 31;
                PRN_DAO.PRNumberDB(PRN_DTO);

                PRN_DTO.DeleteNumbers = Convert.ToString(PrefixDTO);
                PRN_DTO.Id = 32;
                PRN_DAO.PRNumberDB(PRN_DTO);

                PRN_DTO.DeleteNumbers = Convert.ToString(SuffixDTO);
                PRN_DTO.Id = 33;
                PRN_DAO.PRNumberDB(PRN_DTO);

                PR_DTO.CreatorCode = Convert.ToInt32(UserCode);

                PRN_DTO.PRN_Method = PN_DTO.PRN_Method;
                if (PN_DTO.PRN_Number == 0)
                {
                    PRN_DTO.Id = 11;
                }
                else
                {
                    PRN_DTO.Id = 41;
                    PRN_DTO.PRN_Number = PN_DTO.PRN_Number;
                }
                PRN_DAO.PRNumberDB(PRN_DTO);

                foreach (var Reset in Reset_DTO)
                {
                    PRN_DTO.PRN_Date = Convert.ToString(Convert.ToDateTime(Reset.PRR_Date).ToString("yyyyMMdd"));
                    PRN_DTO.PRN_StartingNumber = Convert.ToInt32(Reset.PRR_StartingNumber).ToString();
                    PRN_DTO.PRN_NumberofDigits = Convert.ToInt32(Reset.PRR_NumberofDigits).ToString();
                    PRN_DTO.PRN_PrefilZero = Convert.ToInt64(Reset.PRR_PrefilZero).ToString();
                    PRN_DTO.PRN_Frequency = Convert.ToInt64(Reset.PRR_Frequency).ToString();
                    if (Reset.PRR_Number == 0)
                    {
                        PRN_DTO.Id = 12;
                    }
                    else
                    {
                        PRN_DTO.Id = 42;
                        PRN_DTO.PRN_Number = Reset.PRR_Number;
                    }
                    PRN_DAO.PRNumberDB(PRN_DTO);
                }

                foreach (var Prefix in Prefix_DTO)
                {
                    PRN_DTO.PRN_Date = Convert.ToString(Convert.ToDateTime(Prefix.PRP_Date).ToString("yyyyMMdd"));
                    PRN_DTO.PRN_Particulars = Convert.ToString(Prefix.PRP_Particulars);
                    if (Prefix.PRP_Number == 0)
                    {
                        PRN_DTO.Id = 13;
                    }
                    else
                    {
                        PRN_DTO.Id = 43;
                        PRN_DTO.PRN_Number = Prefix.PRP_Number;
                    }
                    PRN_DAO.PRNumberDB(PRN_DTO);
                }

                foreach (var Suffix in Suffix_DTO)
                {
                    PRN_DTO.PRN_Date = Convert.ToString(Convert.ToDateTime(Suffix.PRS_Date).ToString("yyyyMMdd"));
                    PRN_DTO.PRN_Particulars = Convert.ToString(Suffix.PRS_Particulars);
                    if (Suffix.PRS_Number == 0)
                    {
                        PRN_DTO.Id = 14;
                    }
                    else
                    {
                        PRN_DTO.Id = 44;
                        PRN_DTO.PRN_Number = Suffix.PRS_Number;
                    }
                    PRN_DAO.PRNumberDB(PRN_DTO);
                }
                PRN_DTO.Reset();
                Reset_DTO = null;
                Prefix_DTO = null;
                Suffix_DTO = null;
                ModelState.Clear();
            }
            else if (PN_DTO.PRN_Method == "3")
            {
                PRN_DTO.PRN_Method = PN_DTO.PRN_Method;
                if (PN_DTO.PRN_Number == 0)
                {
                    PRN_DTO.Id = 21;
                }
                else
                {
                    PRN_DTO.Id = 22;
                    PRN_DTO.PRN_Number = PN_DTO.PRN_Number;
                }
                PRN_DAO.PRNumberDB(PRN_DTO);
            }

            GetPRNumber();
            return View(PRN_DTO);
        }
        void GetPRNumber()
        {
            PRN_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PRN_DTO.Id = 1;
            DS = PRN_DAO.PRNumberDB(PRN_DTO);

            ViewBag.Method = Help.GetCat(DS.Tables[0]);
            ViewBag.Frequency = Help.GetCat(DS.Tables[1]);
            ViewBag.Prefil = Help.GetCat(DS.Tables[2]);

            if (DS.Tables[3].Rows.Count > 0)
            {
                PRN_DTO.PRN_Number = Convert.ToInt64(DS.Tables[3].Rows[0]["PRN_Number"]);
                PRN_DTO.PRN_Method = DS.Tables[3].Rows[0]["PRN_Method"].ToString();
            }

            PRN_DTO.PRNumberReset = PRN_DL.PRRList(DS.Tables[4]);
            PRN_DTO.PRNumberPrefix = PRN_DL.PRPList(DS.Tables[5]);
            PRN_DTO.PRNumberSuffix = PRN_DL.PRSList(DS.Tables[6]);
        }
    }
}
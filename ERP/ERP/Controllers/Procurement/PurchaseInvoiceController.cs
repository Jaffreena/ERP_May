using ClosedXML.Excel;
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DL;
using ERP_DTO;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Map;
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
    public class PurchaseInvoiceController : Controller
    {
        CultureInfo India = new CultureInfo("hi-IN");
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        Procurement_DL P_DL = new Procurement_DL();

        //GRN
        PurchaseInvoiceHead_DTO PIH_DTO = new PurchaseInvoiceHead_DTO();
        PurchaseInvoice_DAO PI_DAO = new PurchaseInvoice_DAO();
        PurchaseInvoice_DTO PI_DTO = new PurchaseInvoice_DTO();
        List<PurchaseInvoice_DTO> PI_List = new List<PurchaseInvoice_DTO>();

        //GRN PO
        POToPurchaseInvoiceHead_DTO PIHPO_DTO = new POToPurchaseInvoiceHead_DTO();
        POToPurchaseInvoice_DAO PIPO_DAO = new POToPurchaseInvoice_DAO();
        POToPurchaseInvoice_DTO PIPO_DTO = new POToPurchaseInvoice_DTO();
        List<POToPurchaseInvoice_DTO> PIPO_List = new List<POToPurchaseInvoice_DTO>();

        //GRN PO TO ITEM
        POItemTOPurchaseInvoiceHead_DTO PIHPOI_DTO = new POItemTOPurchaseInvoiceHead_DTO();
        POItemTOPurchaseInvoice_DAO PIPOI_DAO = new POItemTOPurchaseInvoice_DAO();
        POItemTOPurchaseInvoice_DTO PIPOI_DTO = new POItemTOPurchaseInvoice_DTO();
        List<POItemTOPurchaseInvoice_DTO> PIPOI_List = new List<POItemTOPurchaseInvoice_DTO>();


        //GRN NUMBERING
        PINumber_DTO PIN_DTO = new PINumber_DTO();
        PINumber_DAO PIN_DAO = new PINumber_DAO();
        Numbering_DL PIN_DL = new Numbering_DL();
        List<PINumberReset_DTO> PIR_List = new List<PINumberReset_DTO>();
        List<PINumberPrefix_DTO> PIP_List = new List<PINumberPrefix_DTO>();
        List<PINumberSuffix_DTO> PIS_List = new List<PINumberSuffix_DTO>();


        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        DataSet DS = new DataSet();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;



        //Purchase Invoice Create
        [Route("procurement/transactions/purchase-invoice/create")]
        public IActionResult PurchaseInvoiceCreate()
        {
            PurchaseInvoiceHead_DTO PH_DTO = new PurchaseInvoiceHead_DTO();
            if (TempData["PH_DTO_Json"] is string PHto)
            {
                PH_DTO = System.Text.Json.JsonSerializer.Deserialize<PurchaseInvoiceHead_DTO>(PHto);
            }

            if (PH_DTO.PIH_InvoiceDate == null)
            {
                PH_DTO.PIH_InvoiceDate = DateTime.Now.ToString("dd-MMM-yy");
                PH_DTO.PIH_InvoiceNo = OnPOToPurchaseInvoiceNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            }
            else
            {
                PH_DTO.PIH_InvoiceNo = OnPOToPurchaseInvoiceNumber(Convert.ToInt32(Convert.ToDateTime(PH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd")));
            }
            PurchaseInvoiceGetData();

            return View(PH_DTO);
        }

        [HttpPost]
        [Route("procurement/transactions/purchase-invoice/create")]
        public IActionResult PurchaseInvoiceCreate(PurchaseInvoiceHead_DTO PIH_DTO, String? Mode)
        {

            var Original_PIH_DTO = PurchaseClone(PIH_DTO);

            bool IsValid = false;
            PurchaseInvoiceHead_DTO P_Head_DTO = new PurchaseInvoiceHead_DTO();

            List<PurchaseInvoiceItem_DTO>? Item_DTO = new List<PurchaseInvoiceItem_DTO>();
            List<PurchaseInvoiceExpense_DTO>? Expense_DTO = new List<PurchaseInvoiceExpense_DTO>();
            List<PurchaseInvoiceIExpense_DTO>? ItemExpense_DTO = new List<PurchaseInvoiceIExpense_DTO>();
            List<PurchaseInvoiceBatch_DTO>? ItemBatch_DTO = new List<PurchaseInvoiceBatch_DTO>();

            P_Head_DTO = PIH_DTO;

            if (PIH_DTO.InvoiceItems != null)
                Item_DTO = PIH_DTO.InvoiceItems!.Where(PI => PI.IsDeleted == "false").ToList();

            if (PIH_DTO.Expenses != null)
                Expense_DTO = PIH_DTO.Expenses!.Where(PI => PI.PIH_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PIH_DTO.ItemExpenses!.Where(PI => PI.PII_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemBatch != null)
                ItemBatch_DTO = PIH_DTO.ItemBatch!.Where(PI => PI.PII_BCH_IsDeleted == "false").ToList();

            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PII_MS_Number) != Convert.ToInt64(PIH_DTO.PIH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PII_Qty) == 0 || Convert.ToDouble(x.PII_UnitPrice) == 0 || Convert.ToDouble(x.PII_Amount) == 0);

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
                else if (Convert.ToInt32(PIH_DTO.PIH_Vendor_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Vendor is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PIH_DTO.PIH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();

                    P_Head_DTO.InvoiceItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = ItemBatch_DTO;

                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        if (DirectBatchValidation(Item_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String InvoiceNoOld = PIH_DTO.PIH_InvoiceNo;
                                    String InvoiceNoNew = OnPurchaseInvoiceNumber(Convert.ToInt32(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd")));

                                    PI_DTO.PIH_InvoiceNo = InvoiceNoNew;
                                    PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd"));
                                    PI_DTO.PIH_DueDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_DueDate).ToString("yyyyMMdd"));
                                    PI_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                                    PI_DTO.PIH_SupplierInvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_SupplierInvoiceDate).ToString("yyyyMMdd"));
                                    PI_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                                    PI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 1 : 0);
                                    PI_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                                    PI_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                                    PI_DTO.PIH_ExchangeRate = Convert.ToDouble(PIH_DTO.PIH_ExchangeRate);
                                    PI_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                                    PI_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                                    PI_DTO.PIH_MaterialCost = Convert.ToDouble(PIH_DTO.PIH_MaterialCost).ToString();
                                    PI_DTO.PIH_ItemMiscExpense = Convert.ToDouble(PIH_DTO.PIH_ItemMiscExpense).ToString();
                                    PI_DTO.PIH_HeaderMiscExpense = Convert.ToDouble(PIH_DTO.PIH_HeaderMiscExpense).ToString();
                                    PI_DTO.PIH_GST_Amount = Convert.ToDouble(PIH_DTO.PIH_GST_Amount).ToString();
                                    PI_DTO.PIH_InvoiceAmount = Convert.ToDouble(PIH_DTO.PIH_InvoiceAmount).ToString();
                                    PI_DTO.PIH_WHT_Amount = Convert.ToDouble(PIH_DTO.PIH_WHT_Amount).ToString();
                                    PI_DTO.PIH_RoundOff = Convert.ToDouble(PIH_DTO.PIH_RoundOff).ToString();
                                    PI_DTO.PIH_VendorPayable = Convert.ToDouble(PIH_DTO.PIH_VendorPayable).ToString();
                                    PI_DTO.Id = 21;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    OnPurchaseInvoiceNumberGen(Convert.ToInt32(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        PI_DTO.PIH_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in Item_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            PI_DTO.PII_Item_Number = Convert.ToString(Item.PII_Item_Number);
                                            PI_DTO.PII_Warehouse_Number = Convert.ToString(Item.PII_Warehouse_Number);
                                            PI_DTO.PII_UoM_Number = Convert.ToInt64(Item.PII_UoM_Number).ToString();
                                            PI_DTO.PII_Qty = Convert.ToDouble(Item.PII_Qty).ToString();
                                            PI_DTO.PII_UnitPrice = Convert.ToDouble(Item.PII_UnitPrice).ToString();
                                            PI_DTO.PII_Amount = Convert.ToDouble(Item.PII_Amount).ToString();
                                            PI_DTO.PII_ExpenseValue = Convert.ToDouble(Item.PII_ExpenseValue);
                                            PI_DTO.PII_HSN_Number = Convert.ToString(Item.PII_HSN_Number);
                                            PI_DTO.PII_GST_Amount = Convert.ToDouble(Item.PII_GST_Amount).ToString();
                                            PI_DTO.PII_WHT_Percent = Convert.ToDouble(Item.PII_WHT_Percent).ToString();
                                            PI_DTO.PII_WHT_Amount = Convert.ToDouble(Item.PII_WHT_Amount).ToString();
                                            PI_DTO.Id = 22;
                                            D = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PII_EXP_Item_Number == Item.PII_Item_Number) && (x.PII_EXP_Item_Index == Item.PII_Index));

                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PI_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PI_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PII_EXP_Expense_Number);
                                                PI_DTO.EXP_Remarks = ItemExp.PII_EXP_Remarks;
                                                PI_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PII_EXP_Occurrence_Number);
                                                PI_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PII_EXP_CM_Number);
                                                PI_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PII_EXP_ExpenseBase);
                                                PI_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PII_EXP_ExpenseValue);
                                                PI_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PII_EXP_Allocate_Number);
                                                PI_DTO.Id = 24;
                                                PI_DAO.PurchaseInvoiceDB(PI_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_Item_Number == Item.PII_Item_Number) && (x.PII_BCH_Item_Index == Item.PII_Index));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                PI_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PI_DTO.PII_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PII_BCH_Date).ToString("yyyyMMdd"));
                                                PI_DTO.PII_BCH_No = Convert.ToString(ItemBatch.PII_BCH_No);
                                                PI_DTO.PII_BCH_Qty = Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                                                PI_DTO.PII_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PII_BCH_UnitPrice);
                                                PI_DTO.PII_BCH_Value = Convert.ToDouble(ItemBatch.PII_BCH_Value);
                                                PI_DTO.Id = 25;
                                                PI_DAO.PurchaseInvoiceDB(PI_DTO);
                                            }
                                        }
                                        foreach (var Exp in Expense_DTO)
                                        {
                                            PI_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PIH_EXP_Expense_Number);
                                            PI_DTO.EXP_Remarks = Exp.PIH_EXP_Remarks;
                                            PI_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PIH_EXP_Occurrence_Number);
                                            PI_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PIH_EXP_CM_Number);
                                            PI_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PIH_EXP_ExpenseBase);
                                            PI_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.PIH_EXP_ExpenseValue);
                                            PI_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PIH_EXP_Allocate_Number);
                                            PI_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PIH_EXP_SAC_Number);
                                            PI_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PIH_EXP_TaxCalculate);
                                            PI_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PIH_EXP_TaxValue);
                                            PI_DTO.Id = 23;
                                            PI_DAO.PurchaseInvoiceDB(PI_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    P_Head_DTO.Reset();
                                    Expense_DTO = null;
                                    Item_DTO = null;
                                    ItemExpense_DTO = null;
                                    PIH_DTO.Reset();
                                    Original_PIH_DTO = PurchaseClone(PIH_DTO);

                                    P_Head_DTO.PIH_InvoiceDate = DateTime.Now.ToString("dd-MMM-yy");
                                    if (InvoiceNoOld != InvoiceNoNew)
                                    {
                                        ViewBag.ErrorCode = 2;
                                        ViewBag.ErrorMessage = "Purchase Invoice number " + InvoiceNoOld + " used by another user. Next number will be allotted to you.";
                                    }

                                    return RedirectToAction("PurchaseInvoiceCreate");

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
            else if (Mode == "POTOITEM")
            {
                POToPurchaseInvoiceHead_DTO PHPO_DTO = new POToPurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PHPO_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("POToPurchaseInvoiceCreate");
            }
            else if (Mode == "POITEMTO")
            {
                POItemTOPurchaseInvoiceHead_DTO PHPO_DTO = new POItemTOPurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PHPO_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("POItemToPurchaseInvoiceCreate");
            }

            PurchaseInvoiceGetData();
            //if (PH_DTO.PIH_InvoiceNo != null)
            //{
            //    ViewBag.InvoiceNo = PH_DTO.PIH_InvoiceNo;
            //}
            //else
            //{
            //    ViewBag.InvoiceNo = OnPurchaseInvoiceNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            //}
            return View(Original_PIH_DTO);

        }
        private T PurchaseClone<T>(T obj)
        {
            if (obj == null) return default(T);
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
        Boolean DirectBatchValidation(List<PurchaseInvoiceItem_DTO> Item_DTO, List<PurchaseInvoiceBatch_DTO>? ItemBatch_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                //Double BatchAmount = 0;

                Int64 PIINumber = Convert.ToInt64(Item.PII_Number);
                Int64 PIIIndex = Convert.ToInt64(Item.PII_Index);
                String PIIItem = Convert.ToString(Item.PII_Item_Number);
                Double PIIQty = Convert.ToDouble(Item.PII_Qty);
                Double PIIAmount = Convert.ToDouble(Item.PII_Amount);

                if (PIINumber != 0)
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_PII_Number == PIINumber));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                    }
                }
                else
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_Item_Number == PIIItem) && (x.PII_BCH_Item_Index == PIIIndex));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                    }
                }


                if (BatchQty == PIIQty) { }
                else
                {
                    Message += Item.PII_ItemCode + " Batch Qty  Mismatched <br/>";
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

        void PurchaseInvoiceGetData()
        {
            PI_DTO.PIH_InvoiceDate = Convert.ToString(DateTime.Now.ToString("yyyyMMdd"));
            PI_DTO.Id = 1;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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
            ViewBag.LedgerAccount = Help.GetCat(DS.Tables[10]);
        }

        [Route("procurement/transactions/purchase-invoice/item")]
        public IActionResult PurchaseInvoiceItem(String? ItemCode, String MS)
        {
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            if (MS == null)
            {
                MS = "";
            }
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PII_ItemCode = Convert.ToString(ItemCode).Trim();
            PI_DTO.PIH_MS_Number = Convert.ToString(MS).Trim();
            PI_DTO.Id = 2;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            var Item = P_DL.PIIList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("procurement/transactions/purchase-invoice/uom")]
        public String PurchaseInvoiceUoM(String? UoM)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PII_UoM_Number = Convert.ToString(UoM);
            PI_DTO.Id = 4;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("procurement/transactions/purchase-invoice/expense/des")]
        public IActionResult PurchaseInvoiceExpensiveDes(String? Title)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.EXP_Expense_Number = Convert.ToInt32(Title);
            PI_DTO.Id = 3;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            var Expensive = P_DL.ExInvoiceList(DS.Tables[0]).FirstOrDefault();
            return Json(Expensive);
        }

        [Route("procurement/transactions/purchase-invoice/gst")]
        public String PurchaseInvoiceGst(String? Cluster, String? GRNDate, String? HSN, String? BaseAmount)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PI_DTO.PII_HSN_Number = Convert.ToString(HSN);
            PI_DTO.Id = 6;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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

        [Route("procurement/transactions/purchase-invoice/gst/view")]
        public IActionResult PurchaseInvoiceGstView(String? Cluster, String? GRNDate, String? HSN, String? BaseAmount)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PI_DTO.PII_HSN_Number = Convert.ToString(HSN);
            PI_DTO.Id = 9;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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

        [Route("procurement/transactions/purchase-invoice/expense/gst")]
        public String PurchaseInvoiceHeaderGst(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PI_DTO.PII_SAC_Number = Convert.ToString(SAC);
            PI_DTO.Id = 11;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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

        [Route("procurement/transactions/purchase-invoice/expense/gst/view")]
        public IActionResult PurchaseInvoiceGstHeaderView(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PI_DTO.PII_SAC_Number = Convert.ToString(SAC);
            PI_DTO.Id = 12;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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

        [Route("procurement/transactions/purchase-invoice/wht")]
        public IActionResult PurchaseInvoiceWHT(String? Vendor, String? WHTNumber, String? GRNDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Vendor == null)
            {
                Vendor = "0";
            }
            if (GRNDate == null)
            {
                GRNDate = "0";
            }
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_Vendor_Number = Convert.ToString(Vendor);
            PI_DTO.PIH_WHT_Number = Convert.ToString(WHTNumber);
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.Id = 7;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            var WHT = P_DL.PurInvWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("procurement/transactions/purchase-invoice/vendor")]
        public IActionResult PurchaseInvoiceVendor(String? Vendor, String? GRNDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (GRNDate == null)
            {
                GRNDate = "0";
            }
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PI_DTO.PII_ItemCode = Convert.ToString(Vendor).Trim();
            PI_DTO.Id = 5;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            var Ven = P_DL.PIVList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-invoice/vendor/get")]
        public IActionResult PurchaseInvoiceVendorGet(String? Vendor, String? GRNDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (GRNDate == null)
            {
                GRNDate = "0";
            }
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PI_DTO.PIH_Vendor_Number = Convert.ToString(Vendor).Trim();
            PI_DTO.Id = 10;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            var Ven = P_DL.PIVList(DS.Tables[0]).FirstOrDefault();
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-invoice/cluster")]
        public IActionResult PurchaseInvoiceCluster(String? Vendor, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.Search = Cluster;
            PI_DTO.PIH_Vendor_Number = Vendor;
            PI_DTO.Id = 8;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            var InvCluster = P_DL.PurCluster(DS.Tables[0]);
            return Json(InvCluster);
        }
        void OnPurchaseInvoiceNumberGen(Int32 PIDate)
        {
            DataSet DS1 = new DataSet();
            PIN_DTO.PIN_Date = PIDate.ToString();
            PIN_DTO.Id = 101;
            DS1 = PIN_DAO.PINumberDB(PIN_DTO);
            if (DS1.Tables[0].Rows.Count > 0)
            {
                Int32 Order = Convert.ToInt32(DS1.Tables[0].Rows[0]["PIN_Method"].ToString());
                DateTime PI_Date = Convert.ToDateTime(DateTime.ParseExact(PIDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));

                DateTime StartDate = new DateTime();
                DateTime EndDate = new DateTime();

                if (Order == 2)
                {
                    if (DS1.Tables[1].Rows.Count > 0)
                    {
                        Int32 Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["StartingNumber"].ToString());

                        PIN_DTO.PIN_Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["Number"].ToString());
                        PIN_DTO.PIN_StartingNumber = Convert.ToString(Convert.ToInt32(Number + 1));
                        PIN_DTO.Id = 103;
                        PIN_DAO.PINumberDB(PIN_DTO);
                    }
                    else
                    {
                        Int32 Frequency = 0;
                        Int32 Start = 0;
                        DateTime Date = new DateTime();

                        if (DS1.Tables[2].Rows.Count > 0)
                        {
                            Date = Convert.ToDateTime(DS1.Tables[2].Rows[0]["PIR_Date"].ToString());
                            Start = Convert.ToInt32(DS1.Tables[2].Rows[0]["PIR_StartingNumber"].ToString());
                            Frequency = Convert.ToInt32(DS1.Tables[2].Rows[0]["PIR_Frequency"].ToString());
                        }

                        if (Frequency == 4)
                        {
                            if (Date.Month == PI_Date.Month)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PI_Date.Year, PI_Date.Month, 1);
                                EndDate = new DateTime(PI_Date.Year, PI_Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                        }
                        else if (Frequency == 5)
                        {
                            if (Date.Month == PI_Date.Month && Date.Year == PI_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else if (Date.Month != PI_Date.Month && Date.Year == PI_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PI_Date.Year, Date.Month, 1);
                                EndDate = new DateTime(PI_Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                        }

                        PIN_DTO.PIN_Number = Convert.ToInt32(DS1.Tables[2].Rows[0]["PIR_Number"].ToString());
                        PIN_DTO.PIN_StartingNumber = Convert.ToString(Start);
                        PIN_DTO.PIN_Date = Convert.ToString(StartDate.ToString("yyyyMMdd"));
                        PIN_DTO.PIN_Method = Convert.ToString(EndDate.ToString("yyyyMMdd"));
                        PIN_DTO.Id = 102;
                        PIN_DAO.PINumberDB(PIN_DTO);
                    }
                }
            }
        }


        [Route("procurement/transactions/purchase-invoice/numbering")]
        public String OnPurchaseInvoiceNumber(Int32 PIDate)
        {
            PI_DTO.PIH_InvoiceDate = Convert.ToString(PIDate);
            PI_DTO.Id = 0;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["PIN_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["PIP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["PIS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0)
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_Frequency"].ToString());

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
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["PIR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_Frequency"].ToString());

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
                ViewBag.ErrorMessage = "GRN Number Not assigned for Given date";
            }
            return "";
        }






        //Purchase Invoice Summary
        [Route("procurement/transactions/purchase-invoice-register-summary")]
        public IActionResult PurchaseInvoiceRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            PI_List = PISummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseInvoice_DTO>.CreateAsync(PI_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("procurement/transactions/purchase-invoice-register-summary")]
        [HttpPost]
        public IActionResult PurchaseInvoiceRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? PI_No, String[] DeleteNumber, String selectAllCheckbox)
        {

            if (Mode == "DeleteAll")
            {
                PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                PI_DTO.DeleteNumbers = DeleteNumbers;
                PI_DTO.Id = 31;
                PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
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

                PI_DTO.Id = 51;
                PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                if (Mode == "Ascii")
                {
                    List<PurchaseInvoiceAscii> PI_List = P_DL.PIAscii(DS.Tables[0]);

                    var Key = PI_List;
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_VendorPayable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PIH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_VendorPayable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;
                    Decimal TotalGSTSum = 0;
                    Decimal TotalInvoiceAmountSum = 0;
                    Decimal TotalWithHoldTaxSum = 0;
                    Decimal TotalRoundOffSum = 0;
                    Decimal TotalVendorPayableSum = 0;

                    var HeaderRow = typeof(PurchaseInvoiceAscii)
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(prop => prop.Name != nameof(PurchaseInvoiceAscii.PIH_Number))
                            .Select(prop =>
                                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                             )
                            .ToList();

                    var AsciiData = new StringBuilder();
                    AsciiData.AppendLine(string.Join("\t", HeaderRow));



                    PropertyInfo[] PropertiesToInclude = typeof(PurchaseInvoiceAscii)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.Name != nameof(PurchaseInvoiceAscii.PIH_Number))
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

                        if (Decimal.TryParse(item.PII_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                        {
                            TotalQtySum += QtyValue;
                        }
                        if (Decimal.TryParse(item.PIH_MaterialCost, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                        {
                            TotalMaterialValueSum += MaterialValue;
                        }
                        if (Decimal.TryParse(item.PIH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                        {
                            TotalItemMiscExpenseSum += ItemMiscValue;
                        }
                        if (Decimal.TryParse(item.PIH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                        {
                            TotalHeadMiscExpenseSum += HeadMiscValue;
                        }
                        if (Decimal.TryParse(item.PIH_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                        {
                            TotalGSTSum += GSTAmount;
                        }
                        if (Decimal.TryParse(item.PIH_InvoiceAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal InvoiceAmount))
                        {
                            TotalInvoiceAmountSum += InvoiceAmount;
                        }
                        if (Decimal.TryParse(item.PIH_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                        {
                            TotalWithHoldTaxSum += WHTAmount;
                        }
                        if (Decimal.TryParse(item.PIH_RoundOff, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal RoundOff))
                        {
                            TotalRoundOffSum += RoundOff;
                        }
                        if (Decimal.TryParse(item.PIH_VendorPayable, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal VendorPayable))
                        {
                            TotalVendorPayableSum += VendorPayable;
                        }
                    }

                    List<String> FooterCells = new List<String>();
                    Boolean TotalLabelAdded = false;
                    foreach (var prop in PropertiesToInclude)
                    {
                        string FooterCellValue = "";

                        if (!TotalLabelAdded && prop.Name != nameof(PurchaseInvoiceAscii.PII_Qty) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_MaterialCost) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_ItemMiscExpense) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_HeaderMiscExpense) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_GST_Amount) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_InvoiceAmount) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_WHT_Amount) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_RoundOff) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_VendorPayable))
                        {
                            if (FooterCells.Count == 0)
                            {
                                FooterCellValue = "Total:";
                                TotalLabelAdded = true;
                            }
                        }


                        switch (prop.Name)
                        {
                            case nameof(PurchaseInvoiceAscii.PII_Qty):
                                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceAscii.PIH_MaterialCost):
                                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceAscii.PIH_ItemMiscExpense):
                                FooterCellValue = TotalItemMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceAscii.PIH_HeaderMiscExpense):
                                FooterCellValue = TotalHeadMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceAscii.PIH_GST_Amount):
                                FooterCellValue = TotalGSTSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceAscii.PIH_InvoiceAmount):
                                FooterCellValue = TotalInvoiceAmountSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceAscii.PIH_WHT_Amount):
                                FooterCellValue = TotalWithHoldTaxSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceAscii.PIH_RoundOff):
                                FooterCellValue = TotalRoundOffSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceAscii.PIH_VendorPayable):
                                FooterCellValue = TotalVendorPayableSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                        }

                        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                    }

                    AsciiData.AppendLine(string.Join("\t", FooterCells));

                    String FileName = "PI-download";
                    byte[] fileBytes = Encoding.UTF8.GetBytes(AsciiData.ToString());
                    var contentType = "text/plain";
                    var fileDownloadName = $"{FileName}.txt";
                    return File(fileBytes, contentType, fileDownloadName);

                }
                else if (Mode == "Excel")
                {
                    PI_List = P_DL.PIList(DS.Tables[0]);

                    var Key = PI_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_MaterialCost!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_HeaderMiscExpense!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_GST_Amount!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceAmount!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Amount!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_RoundOff!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_VendorPayable!.ToString().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PIH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_MaterialCost!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_HeaderMiscExpense!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_GST_Amount!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceAmount!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Amount!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_RoundOff!.ToString().Contains(Search.ToLower()) ||
                                 K.PIH_VendorPayable!.ToString().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String FileName = "PI-download";
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add(PISummaryDownload(Key.ToList()), "Sheet1");

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
                    PI_List = P_DL.PIList(DS.Tables[0]);

                    var Key = PI_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_VendorPayable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PIH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_VendorPayable!.ToLower().Contains(Search.ToLower())).ToList();
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

                    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>GRN Date</th><th>GRN Number</th><th>Supplier Invoice Number</th><th style='width:70px;text-align:center'>Supplier Invoice Date</th><th style='width:30px;text-align:center'>Import Order</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th>Tax Cluster</th><th>WH Tax Code</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Material Value</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th>GST</th><th>Invoice amount</th><th>WithHold Tax</th><th>Round off</th><th>Vendor Payable</th></tr>";

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;
                    Decimal TotalGSTSum = 0;
                    Decimal TotalInvoiceAmountSum = 0;
                    Decimal TotalWithHoldTaxSum = 0;
                    Decimal TotalRoundOffSum = 0;
                    Decimal TotalVendorPayableSum = 0;

                    if (Key.ToList().Count > 0)
                    {
                        foreach (var Row in Key.ToList())
                        {
                            String Import = "";
                            if (Row.PIH_ImportOrder == 1)
                            {
                                Import = "Yes";
                            }
                            else
                            {
                                Import = "No";
                            }

                            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_MaterialCost));
                            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_ItemMiscExpense));
                            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_HeaderMiscExpense));
                            String GST = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_GST_Amount));
                            String Invoice = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_InvoiceAmount));
                            String WHT = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_WHT_Amount));
                            String Pay = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_VendorPayable));

                            PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PIH_InvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.PIH_InvoiceNo}</td>
                                <td>{Row.PIH_SupplierInvoiceNo}</td>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PIH_SupplierInvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                <td style='width:30px;text-align:center'>{Import}</th>
                                <td>{Row.PIH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PIH_Currency_Number}</td>
                                <td>{Row.PIH_TaxCluster_Number}</td>
                                <td>{Row.PIH_WHT_Number}</td>
                                <td>{Row.PIH_MS_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PIH_TotalItem}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_Qty}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                <td style='width:30px;text-align:center'>{GST}</td>
                                <td style='width:70px;text-align:right'>{Invoice}</td>
                                <td style='width:70px;text-align:right'>{WHT}</td>
                                <td style='width:70px;text-align:right'>{Row.PIH_RoundOff}</td>
                                <td style='width:70px;text-align:right'>{Pay}</td>
                                </tr>";


                            if (Decimal.TryParse(Row.PII_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                            {
                                TotalQtySum += QtyValue;
                            }
                            if (Decimal.TryParse(Row.PIH_MaterialCost, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                            {
                                TotalMaterialValueSum += MaterialValue;
                            }
                            if (Decimal.TryParse(Row.PIH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                            {
                                TotalItemMiscExpenseSum += ItemMiscValue;
                            }
                            if (Decimal.TryParse(Row.PIH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                            {
                                TotalHeadMiscExpenseSum += HeadMiscValue;
                            }
                            if (Decimal.TryParse(Row.PIH_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                            {
                                TotalGSTSum += GSTAmount;
                            }
                            if (Decimal.TryParse(Row.PIH_InvoiceAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal InvoiceAmount))
                            {
                                TotalInvoiceAmountSum += InvoiceAmount;
                            }
                            if (Decimal.TryParse(Row.PIH_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                            {
                                TotalWithHoldTaxSum += WHTAmount;
                            }
                            if (Decimal.TryParse(Row.PIH_RoundOff, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal RoundOff))
                            {
                                TotalRoundOffSum += RoundOff;
                            }
                            if (Decimal.TryParse(Row.PIH_VendorPayable, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal VendorPayable))
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
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalGSTSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalInvoiceAmountSum))}</td>
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

                    return File(memoryStream.ToArray(), "application/pdf", "PI_Download.pdf");
                }
            }
            else if (Mode == "View")
            {
                return RedirectToAction("PurchaseInvoicePreview", new { PI_No = PI_No });
            }
            else if (Mode == "Edit")
            {
                PI_DTO.PIH_Number = Convert.ToInt32(PI_No);
                PI_DTO.Id = 42;
                PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                if (DS.Tables[0].Rows[0]["PIH_Mode"].ToString() == "1")
                {
                    return RedirectToAction("PurchaseInvoiceEdit", new { PI_No = PI_No });
                }
                else if (DS.Tables[0].Rows[0]["PIH_Mode"].ToString() == "2")
                {
                    return RedirectToAction("POToPurchaseInvoiceEdit", new { PI_No = PI_No });
                }
                else if (DS.Tables[0].Rows[0]["PIH_Mode"].ToString() == "3")
                {
                    return RedirectToAction("POItemToPurchaseInvoiceEdit", new { PI_No = PI_No });
                }
            }

            PI_List = PISummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseInvoice_DTO>.CreateAsync(PI_List, DPageNumber ?? 1, DPageSize));
        }
        List<PurchaseInvoice_DTO> PISummaryGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            PI_DTO.Id = 51;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            PI_List = P_DL.PIList(DS.Tables[0]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title_desc";
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

            var Key = PI_List.OrderByDescending(Cs => Cs.PIH_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                 K.PII_Qty!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_InvoiceAmount!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                 K.PIH_VendorPayable!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.PIH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.PIH_InvoiceDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.PIH_InvoiceDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.PIH_Number);
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

            ViewBag.TotalItem = Key.Sum(item => Convert.ToDouble(item.PIH_TotalItem));
            ViewBag.SumQty = Key.Sum(item => Convert.ToDouble(item.PII_Qty));
            ViewBag.SumMaterial = Key.Sum(item => Convert.ToDouble(item.PIH_MaterialCost));
            ViewBag.SumItemMisc = Key.Sum(item => Convert.ToDouble(item.PIH_ItemMiscExpense));
            ViewBag.SumHeadMisc = Key.Sum(item => Convert.ToDouble(item.PIH_HeaderMiscExpense));
            ViewBag.SumAmount = Key.Sum(item => Convert.ToDouble(item.PIH_Amount));
            ViewBag.SumItemGst = Key.Sum(item => Convert.ToDouble(item.PII_GST_Amount));
            ViewBag.SumHeaderGst = Key.Sum(item => Convert.ToDouble(item.PIH_GST_Amount));
            ViewBag.SumInvoiceAmount = Key.Sum(item => Convert.ToDouble(item.PIH_InvoiceAmount));
            ViewBag.SumWHT = Key.Sum(item => Convert.ToDouble(item.PIH_WHT_Amount));
            ViewBag.SumRound = Key.Sum(item => Convert.ToDouble(item.PIH_RoundOff));
            ViewBag.SumPayable = Key.Sum(item => Convert.ToDouble(item.PIH_VendorPayable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }
        DataTable PISummaryDownload(List<PurchaseInvoice_DTO> PI_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "purchase-invoice-summary";

            TableDown.Clear();
            TableDown.Columns.Add("GRN Date");
            TableDown.Columns.Add("GRN Number");
            TableDown.Columns.Add("Supplier Invoice Number");
            TableDown.Columns.Add("Supplier Invoice Date");
            TableDown.Columns.Add("Import Order");
            TableDown.Columns.Add("Vendor Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Tax Cluster");
            TableDown.Columns.Add("WH Tax Code");
            TableDown.Columns.Add("Material Segregation");
            TableDown.Columns.Add("No. of Line Item");
            TableDown.Columns.Add("Qty");
            TableDown.Columns.Add("Material Value");
            TableDown.Columns.Add("Item Misc Expense Value");
            TableDown.Columns.Add("Header Misc Expense Value");
            TableDown.Columns.Add("GST");
            TableDown.Columns.Add("Invoice amount");
            TableDown.Columns.Add("WithHold Tax");
            TableDown.Columns.Add("Round off");
            TableDown.Columns.Add("Vendor Payable");


            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;
            Decimal TotalGSTSum = 0;
            Decimal TotalInvoiceAmountSum = 0;
            Decimal TotalWithHoldTaxSum = 0;
            Decimal TotalRoundOffSum = 0;
            Decimal TotalVendorPayableSum = 0;


            foreach (var Product in PI_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["GRN Date"] = Product.PIH_InvoiceDate;
                NewRow["GRN Number"] = Product.PIH_InvoiceNo;
                NewRow["Supplier Invoice Number"] = Product.PIH_SupplierInvoiceNo;
                NewRow["Supplier Invoice Date"] = Product.PIH_SupplierInvoiceDate;
                NewRow["Import Order"] = (Product.PIH_ImportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Vendor Name"] = Product.PIH_Vendor_Number;
                NewRow["Currency"] = Product.PIH_Currency_Number;
                NewRow["Tax Cluster"] = Product.PIH_TaxCluster_Number;
                NewRow["WH Tax Code"] = Product.PIH_WHT_Number;
                NewRow["Material Segregation"] = Product.PIH_MS_Number;
                NewRow["No. of Line Item"] = Product.PIH_TotalItem;
                NewRow["Qty"] = Product.PII_Qty;
                NewRow["Material Value"] = Product.PIH_MaterialCost;
                NewRow["Item Misc Expense Value"] = Product.PIH_ItemMiscExpense;
                NewRow["Header Misc Expense Value"] = Product.PIH_HeaderMiscExpense;
                NewRow["GST"] = Product.PIH_GST_Amount;
                NewRow["Invoice amount"] = Product.PIH_InvoiceAmount;
                NewRow["WithHold Tax"] = Product.PIH_WHT_Amount;
                NewRow["Round off"] = Product.PIH_RoundOff;
                NewRow["Vendor Payable"] = Product.PIH_VendorPayable;

                TableDown.Rows.Add(NewRow);


                if (Decimal.TryParse(Product.PII_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.PIH_MaterialCost, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.PIH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscExpenseSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.PIH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                {
                    TotalHeadMiscExpenseSum += HeadMiscValue;
                }
                if (Decimal.TryParse(Product.PIH_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                {
                    TotalGSTSum += GSTAmount;
                }
                if (Decimal.TryParse(Product.PIH_InvoiceAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal InvoiceAmount))
                {
                    TotalInvoiceAmountSum += InvoiceAmount;
                }
                if (Decimal.TryParse(Product.PIH_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                {
                    TotalWithHoldTaxSum += WHTAmount;
                }
                if (Decimal.TryParse(Product.PIH_RoundOff, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal RoundOff))
                {
                    TotalRoundOffSum += RoundOff;
                }
                if (Decimal.TryParse(Product.PIH_VendorPayable, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal VendorPayable))
                {
                    TotalVendorPayableSum += VendorPayable;
                }
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["GRN Date"] = "";
            NewRows["GRN Number"] = "";
            NewRows["Supplier Invoice Number"] = "";
            NewRows["Supplier Invoice Date"] = "";
            NewRows["Import Order"] = "";
            NewRows["Vendor Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Tax Cluster"] = "";
            NewRows["WH Tax Code"] = "";
            NewRows["Material Segregation"] = "";
            NewRows["No. of Line Item"] = "";
            NewRows["Qty"] = TotalQtySum;
            NewRows["Material Value"] = TotalMaterialValueSum;
            NewRows["Item Misc Expense Value"] = TotalItemMiscExpenseSum;
            NewRows["Header Misc Expense Value"] = TotalHeadMiscExpenseSum;
            NewRows["GST"] = TotalGSTSum;
            NewRows["Invoice amount"] = TotalInvoiceAmountSum;
            NewRows["WithHold Tax"] = TotalWithHoldTaxSum;
            NewRows["Round off"] = TotalRoundOffSum;
            NewRows["Vendor Payable"] = TotalVendorPayableSum;
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }


        [Route("procurement/transactions/purchase-invoice-register-summary/print")]
        public String PISummaryPrint(String Search, String SelectedItem, bool AllItem)
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
            PI_DTO.Id = 51;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            PI_List = P_DL.PIList(DS.Tables[0]);

            var Key = PI_List.ToList();
            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_Qty!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_InvoiceAmount!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_VendorPayable!.ToLower().Contains(Search.ToLower())).ToList();
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PIH_Number)).ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_Qty!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_InvoiceAmount!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_VendorPayable!.ToLower().Contains(Search.ToLower())).ToList();
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

            PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>GRN Date</th><th>GRN Number</th><th>Supplier Invoice Number</th><th style='width:70px;text-align:center'>Supplier Invoice Date</th><th style='width:30px;text-align:center'>Import Order</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th>Tax Cluster</th><th>WH Tax Code</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Material Value</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th>GST</th><th>Invoice amount</th><th>WithHold Tax</th><th>Round off</th><th>Vendor Payable</th></tr>";

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;
            Decimal TotalGSTSum = 0;
            Decimal TotalInvoiceAmountSum = 0;
            Decimal TotalWithHoldTaxSum = 0;
            Decimal TotalRoundOffSum = 0;
            Decimal TotalVendorPayableSum = 0;

            if (Key.ToList().Count > 0)
            {
                foreach (var Row in Key.ToList())
                {
                    String Import = "";
                    if (Row.PIH_ImportOrder == 1)
                    {
                        Import = "Yes";
                    }
                    else
                    {
                        Import = "No";
                    }

                    String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_MaterialCost));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_ItemMiscExpense));
                    String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_HeaderMiscExpense));
                    String GST = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_GST_Amount));
                    String Invoice = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_InvoiceAmount));
                    String WHT = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_WHT_Amount));
                    String Pay = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_VendorPayable));

                    PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PIH_InvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.PIH_InvoiceNo}</td>
                                <td>{Row.PIH_SupplierInvoiceNo}</td>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PIH_SupplierInvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                <td style='width:30px;text-align:center'>{Import}</th>
                                <td>{Row.PIH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PIH_Currency_Number}</td>
                                <td>{Row.PIH_TaxCluster_Number}</td>
                                <td>{Row.PIH_WHT_Number}</td>
                                <td>{Row.PIH_MS_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PIH_TotalItem}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_Qty}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                <td style='width:30px;text-align:center'>{GST}</td>
                                <td style='width:70px;text-align:right'>{Invoice}</td>
                                <td style='width:70px;text-align:right'>{WHT}</td>
                                <td style='width:70px;text-align:right'>{Row.PIH_RoundOff}</td>
                                <td style='width:70px;text-align:right'>{Pay}</td>
                                </tr>";


                    if (Decimal.TryParse(Row.PII_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.PIH_MaterialCost, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalMaterialValueSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.PIH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscExpenseSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.PIH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                    {
                        TotalHeadMiscExpenseSum += HeadMiscValue;
                    }
                    if (Decimal.TryParse(Row.PIH_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                    {
                        TotalGSTSum += GSTAmount;
                    }
                    if (Decimal.TryParse(Row.PIH_InvoiceAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal InvoiceAmount))
                    {
                        TotalInvoiceAmountSum += InvoiceAmount;
                    }
                    if (Decimal.TryParse(Row.PIH_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                    {
                        TotalWithHoldTaxSum += WHTAmount;
                    }
                    if (Decimal.TryParse(Row.PIH_RoundOff, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal RoundOff))
                    {
                        TotalRoundOffSum += RoundOff;
                    }
                    if (Decimal.TryParse(Row.PIH_VendorPayable, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal VendorPayable))
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
                                <td></td>
                                <td></td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalGSTSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalInvoiceAmountSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalWithHoldTaxSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalRoundOffSum))}</td>
                                <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalVendorPayableSum))}</td>
                                </tr>";
            PDFDownload += $@"</table></body></html>";

            return PDFDownload;
        }






        //Purchase Invoice Detailed
        [Route("procurement/transactions/purchase-invoice-register-detailed")]
        public IActionResult PurchaseInvoiceRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            PI_List = PIDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseInvoice_DTO>.CreateAsync(PI_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("procurement/transactions/purchase-invoice-register-detailed")]
        [HttpPost]
        public IActionResult PurchaseInvoiceRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? PI_No, String[] DeleteNumber, String selectAllCheckbox)
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

                PI_DTO.Id = 52;
                PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                if (Mode == "Ascii")
                {
                    List<PurchaseInvoiceDetailAscii> PI_List = P_DL.PIDAscii(DS.Tables[0]);

                    var Key = PI_List;
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PIH_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    Decimal TotalQtySum = 0;
                    Decimal TotalMaterialValueSum = 0;
                    Decimal TotalItemMiscExpenseSum = 0;
                    Decimal TotalHeadMiscExpenseSum = 0;
                    Decimal TotalGSTSum = 0;
                    Decimal TotalWithHoldTaxSum = 0;

                    var HeaderRow = typeof(PurchaseInvoiceDetailAscii)
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(prop => prop.Name != nameof(PurchaseInvoiceDetailAscii.PIH_Number))
                            .Select(prop =>
                                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                             )
                            .ToList();

                    var AsciiData = new StringBuilder();
                    AsciiData.AppendLine(string.Join("\t", HeaderRow));


                    PropertyInfo[] PropertiesToInclude = typeof(PurchaseInvoiceDetailAscii)
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.Name != nameof(PurchaseInvoiceAscii.PIH_Number))
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

                        if (Decimal.TryParse(item.PII_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                        {
                            TotalQtySum += QtyValue;
                        }
                        if (Decimal.TryParse(item.PII_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                        {
                            TotalMaterialValueSum += MaterialValue;
                        }
                        if (Decimal.TryParse(item.PIH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                        {
                            TotalItemMiscExpenseSum += ItemMiscValue;
                        }
                        if (Decimal.TryParse(item.PIH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeaderMiscValue))
                        {
                            TotalHeadMiscExpenseSum += HeaderMiscValue;
                        }
                        if (Decimal.TryParse(item.PII_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                        {
                            TotalGSTSum += GSTAmount;
                        }
                        if (Decimal.TryParse(item.PII_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                        {
                            TotalWithHoldTaxSum += WHTAmount;
                        }
                    }

                    List<String> FooterCells = new List<String>();
                    Boolean TotalLabelAdded = false;
                    foreach (var prop in PropertiesToInclude)
                    {
                        string FooterCellValue = "";

                        if (!TotalLabelAdded && prop.Name != nameof(PurchaseInvoiceAscii.PII_Qty) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_MaterialCost) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_ItemMiscExpense) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_HeaderMiscExpense) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_GST_Amount) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_InvoiceAmount) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_WHT_Amount) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_RoundOff) &&
                                                 prop.Name != nameof(PurchaseInvoiceAscii.PIH_VendorPayable))
                        {
                            if (FooterCells.Count == 0)
                            {
                                FooterCellValue = "Total:";
                                TotalLabelAdded = true;
                            }
                        }

                        switch (prop.Name)
                        {
                            case nameof(PurchaseInvoiceDetailAscii.PII_Qty):
                                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceDetailAscii.PII_Amount):
                                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceDetailAscii.PIH_ItemMiscExpense):
                                FooterCellValue = TotalItemMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceDetailAscii.PIH_HeaderMiscExpense):
                                FooterCellValue = TotalHeadMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceDetailAscii.PII_GST_Amount):
                                FooterCellValue = TotalGSTSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                            case nameof(PurchaseInvoiceDetailAscii.PII_WHT_Amount):
                                FooterCellValue = TotalWithHoldTaxSum.ToString("N2", CultureInfo.InvariantCulture);
                                break;
                        }

                        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                    }

                    AsciiData.AppendLine(string.Join("\t", FooterCells));

                    String FileName = "PI-download";
                    byte[] fileBytes = Encoding.UTF8.GetBytes(AsciiData.ToString());
                    var contentType = "text/plain";
                    var fileDownloadName = $"{FileName}.txt";
                    return File(fileBytes, contentType, fileDownloadName);

                }
                else if (Mode == "Excel")
                {
                    PI_List = P_DL.PIDetailList(DS.Tables[0]);

                    var Key = PI_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_VendorPayable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PII_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_TaxCluster_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_MaterialCost!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_HeaderMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_GST_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceAmount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_WHT_Amount!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_RoundOff!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_VendorPayable!.ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }

                    String FileName = "PI-download";
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add(PIDetailedDownload(Key.ToList()), "Sheet1");

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
                    PI_List = P_DL.PIDetailList(DS.Tables[0]);

                    var Key = PI_List.ToList();
                    if (selectAllCheckbox == "on")
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                        }
                    }
                    else if (DeleteNumber.Length > 0)
                    {
                        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PII_Number)).ToList();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Search))
                        {
                            Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Description!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                                 K.PII_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PIH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                                 K.PII_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
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

                    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>GRN Date</th><th>GRN Number</th><th>Supplier Invoice No</th><th style='width:70px;text-align:center'>Supplier Invoice Date</th><th style='width:30px;text-align:center'>Import Order</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th>Material Segregation</th><th>Purchase Order Number</th><th>Item Group</th><th>Item Code</th><th>Description</th><th>Warehouse</th><th style='width:30px;text-align:center'>UoM</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Unit Price</th><th style='width:70px'>Material Value</th><th>Expense Code</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th style='width:30px'>HSN</th><th>GST</th><th>WH_Tax Code</th><th>Percent</th><th>WH_Tax Amount</th></tr>";

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
                            if (Row.PIH_ImportOrder == 1)
                            {
                                Import = "Yes";
                            }
                            else
                            {
                                Import = "No";
                            }

                            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PII_Amount));
                            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_ItemMiscExpense));
                            String HeadeExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_HeaderMiscExpense));
                            String GST = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PII_GST_Amount));
                            String WHT = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PII_WHT_Amount));

                            PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PIH_InvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.PIH_InvoiceNo}</td>
                                <td>{Row.PIH_SupplierInvoiceNo}</td>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PIH_SupplierInvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Import}</td>
                                <td>{Row.PIH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PIH_Currency_Number}</td>
                                <td>{Row.PII_MS_Number}</td>
                                <td></td>
                                <td>{Row.PII_ItemGroup}</td>
                                <td>{Row.PII_ItemCode}</td>
                                <td>{Row.PII_Description}</td>
                                <td>{Row.PII_Warehouse_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_UoM}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_Qty}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_UnitPrice}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{Row.PIH_EXP_Expense}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadeExp}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_HSN_Number}</td>
                                <td style='width:30px;text-align:center'>{GST}</td>
                                <td>{Row.PIH_WHT_Number}</td>
                                <td style='text-align:right'>{Row.PII_WHT_Percent}</td>
                                <td style='width:50px;text-align:right'>{WHT}</td>
                                </tr>";


                            if (Decimal.TryParse(Row.PII_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                            {
                                TotalQtySum += QtyValue;
                            }
                            if (Decimal.TryParse(Row.PII_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                            {
                                TotalMaterialValueSum += MaterialValue;
                            }
                            if (Decimal.TryParse(Row.PIH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                            {
                                TotalItemMiscExpenseSum += ItemMiscValue;
                            }
                            if (Decimal.TryParse(Row.PIH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeaderMiscValue))
                            {
                                TotalHeadMiscExpenseSum += HeaderMiscValue;
                            }
                            if (Decimal.TryParse(Row.PII_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                            {
                                TotalGSTSum += GSTAmount;
                            }
                            if (Decimal.TryParse(Row.PII_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
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

                    return File(memoryStream.ToArray(), "application/pdf", "PI_Download.pdf");
                }
            }
            else if (Mode == "View")
            {
                PI_DTO.PIH_Number = Convert.ToInt32(PI_No);
                PI_DTO.Id = 41;
                PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    return RedirectToAction("PurchaseInvoicePreview", new { PI_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }
            else if (Mode == "Edit")
            {
                PI_DTO.PIH_Number = Convert.ToInt32(PI_No);
                PI_DTO.Id = 41;
                PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (DS.Tables[0].Rows[0]["PIH_Mode"].ToString() == "1")
                    {
                        return RedirectToAction("PurchaseInvoiceEdit", new { PI_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["PIH_Mode"].ToString() == "2")
                    {
                        return RedirectToAction("POToPurchaseInvoiceEdit", new { PI_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["PIH_Mode"].ToString() == "3")
                    {
                        return RedirectToAction("POItemToPurchaseInvoiceEdit", new { PI_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    //return RedirectToAction("PurchaseInvoiceEdit", new { PI_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }

            PI_List = PIDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<PurchaseInvoice_DTO>.CreateAsync(PI_List, DPageNumber ?? 1, DPageSize));
        }
        List<PurchaseInvoice_DTO> PIDetailedGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            PI_DTO.Id = 52;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            PI_List = P_DL.PIDetailList(DS.Tables[0]);

            if (String.IsNullOrEmpty(SortOrder))
            {
                SortOrder = "Title_desc";
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

            var Key = PI_List.OrderByDescending(Cs => Cs.PIH_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PIH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.PIH_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.PIH_InvoiceDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.PIH_InvoiceDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.PIH_Number);
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

            ViewBag.SumQty = Key.Sum(item => Convert.ToDouble(item.PII_Qty));
            ViewBag.SumMaterial = Key.Sum(item => Convert.ToDouble(item.PII_Amount));
            ViewBag.SumItemMisc = Key.Sum(item => Convert.ToDouble(item.PIH_ItemMiscExpense));
            ViewBag.SumHeaderMisc = Key.Sum(item => Convert.ToDouble(item.PIH_HeaderMiscExpense));
            ViewBag.SumInvoiceAmount = Key.Sum(item => Convert.ToDouble(item.PIH_InvoiceAmount));
            ViewBag.SumAmount = Key.Sum(item => Convert.ToDouble(item.PIH_Amount));
            ViewBag.SumItemGst = Key.Sum(item => Convert.ToDouble(item.PII_GST_Amount));
            ViewBag.SumHeaderGst = Key.Sum(item => Convert.ToDouble(item.PIH_GST_Amount));
            ViewBag.SumWHT = Key.Sum(item => Convert.ToDouble(item.PII_WHT_Amount));
            ViewBag.SumRound = Key.Sum(item => Convert.ToDouble(item.PIH_RoundOff));
            ViewBag.SumPayable = Key.Sum(item => Convert.ToDouble(item.PIH_VendorPayable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }
        DataTable PIDetailedDownload(List<PurchaseInvoice_DTO> PI_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "purchase-invoice-detail";

            TableDown.Clear();
            TableDown.Columns.Add("GRN Date");
            TableDown.Columns.Add("GRN Number");
            TableDown.Columns.Add("Supplier Invoice Number");
            TableDown.Columns.Add("Supplier Invoice Date");
            TableDown.Columns.Add("Import Order");
            TableDown.Columns.Add("Vendor Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Material Segregation");
            TableDown.Columns.Add("Purchase Order Nnumber");
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
            TableDown.Columns.Add("GST");
            TableDown.Columns.Add("WH_Tax Code");
            TableDown.Columns.Add("Percent");
            TableDown.Columns.Add("WH_Tax Amount");

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeaderMiscExpenseSum = 0;
            Decimal TotalGSTSum = 0;
            Decimal TotalWithHoldTaxSum = 0;

            foreach (var Product in PI_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["GRN Date"] = Product.PIH_InvoiceDate;
                NewRow["GRN Number"] = Product.PIH_InvoiceNo;
                NewRow["Supplier Invoice Number"] = Product.PIH_SupplierInvoiceNo;
                NewRow["Supplier Invoice Date"] = Product.PIH_SupplierInvoiceDate;
                NewRow["Import Order"] = (Product.PIH_ImportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Vendor Name"] = Product.PIH_Vendor_Number;
                NewRow["Currency"] = Product.PIH_Currency_Number;
                NewRow["Material Segregation"] = Product.PII_MS_Number;
                NewRow["Purchase Order Nnumber"] = "";
                NewRow["Item Group"] = Product.PII_ItemGroup;
                NewRow["Item Code"] = Product.PII_ItemCode;
                NewRow["Description"] = Product.PII_Description;
                NewRow["Warehouse"] = Product.PII_Warehouse_Number;
                NewRow["UoM"] = Product.PII_UoM;
                NewRow["Qty"] = Product.PII_Qty;
                NewRow["Unit Price"] = Product.PII_UnitPrice;
                NewRow["Material Value"] = Product.PII_Amount;
                NewRow["Expense Code"] = Product.PIH_EXP_Expense;
                NewRow["Item Misc Expense Value"] = Product.PIH_ItemMiscExpense;
                NewRow["Header Misc Expense Value"] = Product.PIH_HeaderMiscExpense;
                NewRow["HSN"] = Product.PII_HSN_Number;
                NewRow["GST"] = Product.PII_GST_Amount;
                NewRow["WH_Tax Code"] = Product.PIH_WHT_Number;
                NewRow["Percent"] = Product.PII_WHT_Percent;
                NewRow["WH_Tax Amount"] = Product.PII_WHT_Amount;
                TableDown.Rows.Add(NewRow);


                if (Decimal.TryParse(Product.PII_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.PII_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.PIH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscExpenseSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.PIH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeaderMiscValue))
                {
                    TotalHeaderMiscExpenseSum += HeaderMiscValue;
                }
                if (Decimal.TryParse(Product.PII_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                {
                    TotalGSTSum += GSTAmount;
                }
                if (Decimal.TryParse(Product.PII_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
                {
                    TotalWithHoldTaxSum += WHTAmount;
                }
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["GRN Date"] = "";
            NewRows["GRN Number"] = "";
            NewRows["Supplier Invoice Number"] = "";
            NewRows["Supplier Invoice Date"] = "";
            NewRows["Import Order"] = "";
            NewRows["Vendor Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Material Segregation"] = "";
            NewRows["Purchase Order Nnumber"] = "";
            NewRows["Item Group"] = "";
            NewRows["Item Code"] = "";
            NewRows["Description"] = "";
            NewRows["Warehouse"] = "";
            NewRows["UoM"] = "";
            NewRows["Qty"] = TotalQtySum;
            NewRows["Unit Price"] = "";
            NewRows["Material Value"] = TotalMaterialValueSum;
            NewRows["Item Misc Expense Value"] = TotalItemMiscExpenseSum;
            NewRows["Header Misc Expense Value"] = TotalHeaderMiscExpenseSum;
            NewRows["HSN"] = "";
            NewRows["GST"] = TotalGSTSum;
            NewRows["WH_Tax Code"] = "";
            NewRows["Percent"] = "";
            NewRows["WH_Tax Amount"] = TotalWithHoldTaxSum;
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }

        [Route("procurement/transactions/purchase-invoice-register-detailed/print")]
        public String PIDetailedPrint(String Search, String SelectedItem, bool AllItem)
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
            PI_DTO.Id = 52;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            PI_List = P_DL.PIDetailList(DS.Tables[0]);

            var Key = PI_List.ToList();

            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PIH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.PII_Number)).ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.PIH_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_SupplierInvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Vendor_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_ItemGroup!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_ItemCode!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_Warehouse_Number!.ToLower().Contains(Search.ToLower()) ||
                         K.PII_UoM!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PIH_ItemMiscExpense!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_HSN_Number!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_GST_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.PII_WHT_Amount!.ToString().ToLower().Contains(Search.ToLower())).ToList();
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

            PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>GRN Date</th><th>GRN Number</th><th>Supplier Invoice No</th><th style='width:70px;text-align:center'>Supplier Invoice Date</th><th style='width:30px;text-align:center'>Import Order</th><th>Vendor Name</th><th style='width:30px;text-align:center'>Currency</th><th>Material Segregation</th><th>Purchase Order Number</th><th>Item Group</th><th>Item Code</th><th>Description</th><th>Warehouse</th><th style='width:30px;text-align:center'>UoM</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Unit Price</th><th style='width:70px'>Material Value</th><th>Expense Code</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th style='width:30px'>HSN</th><th>GST</th><th>WH_Tax Code</th><th>Percent</th><th>WH_Tax Amount</th></tr>";

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
                    if (Row.PIH_ImportOrder == 1)
                    {
                        Import = "Yes";
                    }
                    else
                    {
                        Import = "No";
                    }

                    String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PII_Amount));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_ItemMiscExpense));
                    String HeadeExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PIH_HeaderMiscExpense));
                    String GST = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PII_GST_Amount));
                    String WHT = string.Format(India, "{0:N2}", Convert.ToDouble(Row.PII_WHT_Amount));

                    PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PIH_InvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.PIH_InvoiceNo}</td>
                                <td>{Row.PIH_SupplierInvoiceNo}</td>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.PIH_SupplierInvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Import}</td>
                                <td>{Row.PIH_Vendor_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PIH_Currency_Number}</td>
                                <td>{Row.PII_MS_Number}</td>
                                <td></td>
                                <td>{Row.PII_ItemGroup}</td>
                                <td>{Row.PII_ItemCode}</td>
                                <td>{Row.PII_Description}</td>
                                <td>{Row.PII_Warehouse_Number}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_UoM}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_Qty}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_UnitPrice}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{Row.PIH_EXP_Expense}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadeExp}</td>
                                <td style='width:30px;text-align:center'>{Row.PII_HSN_Number}</td>
                                <td style='width:30px;text-align:center'>{GST}</td>
                                <td>{Row.PIH_WHT_Number}</td>
                                <td style='text-align:right'>{Row.PII_WHT_Percent}</td>
                                <td style='width:50px;text-align:right'>{WHT}</td>
                                </tr>";


                    if (Decimal.TryParse(Row.PII_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.PII_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalMaterialValueSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.PIH_ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscExpenseSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.PIH_HeaderMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeaderMiscValue))
                    {
                        TotalHeadMiscExpenseSum += HeaderMiscValue;
                    }
                    if (Decimal.TryParse(Row.PII_GST_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal GSTAmount))
                    {
                        TotalGSTSum += GSTAmount;
                    }
                    if (Decimal.TryParse(Row.PII_WHT_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal WHTAmount))
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




        //Purchase Invoice View
        [Route("procurement/transactions/purchase-invoice/{PI_No}/view")]
        public IActionResult PurchaseInvoicePreview(String PI_No)
        {
            String Active = PurchaseInvoiceData(PI_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseInvoiceRegisterSummary");
            }
            ViewBag.PI_No = PI_No;
            PurchaseInvoiceGetData();
            PurchaseInvoiceGetDataPrint(PI_No);
            return View(PIH_DTO);
        }
        [Route("procurement/transactions/purchase-invoice/{PI_No}/view")]
        [HttpPost]
        public IActionResult PurchaseInvoicePreview(String Mode, String PI_No)
        {
            if (Mode == "PDF" || Mode == "Ascii" || Mode == "Excel")
            {
                PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                PI_DTO.Id = 71;
                PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                String Item = "";
                if (DS.Tables[1].Rows.Count > 0)
                {
                    for (Int32 i = 0; i < DS.Tables[1].Rows.Count; i++)
                    {
                        String UnitPrice = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["PII_UnitPrice"]));
                        String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["PII_Amount"]));
                        Item += $@"<tr>
                                <td colspan='1'>{DS.Tables[1].Rows[i]["PII_HSN"]}</td>
                                <td colspan='1'>{DS.Tables[1].Rows[i]["PII_ItemGroup"]}</td>
                                <td colspan='1'>{DS.Tables[1].Rows[i]["PII_ItemCode"]}</td>
                                <td colspan='3'>{DS.Tables[1].Rows[i]["PII_Description"]}</td>
                                <td class='align-center'>{DS.Tables[1].Rows[i]["PII_UoM"]}</td>
                                <td colspan='1' class='align-center'>{DS.Tables[1].Rows[i]["PII_Qty"]}</td>
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
                            <table class='PI-table' style='background-color: #fff;color:#000'>
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
                                        <td colspan='6'>Purchase Invoice</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["VendorName"]}</td>
                                        <td colspan='3'>GRN Number</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PIH_InvoiceNo"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6' rowspan='3'>{DS.Tables[2].Rows[0]["Address"]}</td>
                                        <td colspan='3'>Date</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PIH_InvoiceDate"]}</td>
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
                                        <td colspan='6'>{DS.Tables[2].Rows[0]["City"]}, {DS.Tables[2].Rows[0]["State"]}, {DS.Tables[2].Rows[0]["Pincode"]}</td>
                                        <td colspan='3'>Supplier Invoice No</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PIH_SupplierInvoiceNo"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>GST: {DS.Tables[2].Rows[0]["GSTIN"]}</td>
                                        <td colspan='3'>Date</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PIH_SupplierInvoiceDate"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>PAN: {DS.Tables[2].Rows[0]["PAN"]}</td>
                                        <td colspan='3'>Due Date</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PIH_DueDate"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='3'>Currency</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PIH_Currency"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='3'>Exchange Rate</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PIH_ExchangeRate"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='3'>Tax Cluster</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PIH_TaxCluster"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='3'>Withhold Tax Code</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["PIH_WHT"]}</td>
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

                Double Mis = Convert.ToDouble(DS.Tables[0].Rows[0]["PIH_ItemMiscExpense"]) + Convert.ToDouble(DS.Tables[0].Rows[0]["PIH_HeaderMiscExpense"]);

                Download += $@"
                                    <tr>
                                        <td colspan='2'>Matrial Cost</td>
                                        <td colspan='2'>Misc. Expense</td>
                                        <td colspan='1'>GST</td>
                                        <td colspan='2'>Invoice Amount</td>
                                        <td colspan='2'>WH Tax</td>
                                        <td colspan='1'>Round off</td>
                                        <td colspan='2'>Payable</td>
                                    </tr>
                                    <tr>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["PIH_MaterialCost"]}</td>
                                        <td colspan='2'>{Mis}</td>
                                        <td colspan='1'>{DS.Tables[0].Rows[0]["PIH_GST_Amount"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["PIH_InvoiceAmount"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["PIH_WHT_Amount"]}</td>
                                        <td colspan='1'>{DS.Tables[0].Rows[0]["PIH_RoundOff"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["PIH_VendorPayable"]}</td>
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
                                .PI-container {{
                                    max-width: 100%;
                                    margin: auto;
                                    background-color: #fff;
                                    padding: 20px;
                                    height: 100%;
                                    font-size: 1rem;
                                }}

                                .PI-table {{
                                    width: 100%;
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}

                                    .PI-table th, .PI-table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                        line-height: 1.4;
                                    }}

                                    .PI-table th {{
                                        font-weight: bold;
                                    }}

                                    .PI-table tr td {{
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

                                .PI-title {{
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
                                    border: none !imPIrtant;
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
                PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                PI_DTO.Id = 31;
                PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
                return RedirectToAction("PurchaseInvoiceRegisterSummary");
            }

            String Active = PurchaseInvoiceData(PI_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseInvoiceRegisterSummary");
            }
            ViewBag.PI_No = PI_No;
            PurchaseInvoiceGetData();
            PurchaseInvoiceGetDataPrint(PI_No);
            return View(PIH_DTO);
        }
        String PurchaseInvoiceData(String PI_No)
        {
            PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
            PI_DTO.Id = 61;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                PIH_DTO = P_DL.PIHeadList(DS.Tables[0]).FirstOrDefault();
                PIH_DTO.InvoiceItems = P_DL.PIItemList(DS.Tables[1]);
                PIH_DTO.Expenses = P_DL.PIExpenseEditList(DS.Tables[2]);
                PIH_DTO.ItemExpenses = P_DL.PIIExpenseEditList(DS.Tables[3]);
                PIH_DTO.ItemBatch = P_DL.PIIBatchEditList(DS.Tables[4]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["PIH_Mode"].ToString();

                return "1";
            }
            else
            {
                return "0";
            }
        }
        String PurchaseInvoiceGetDataPrint(String PI_No)
        {
            PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
            PI_DTO.Id = 71;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                ViewBag.HeadPreview = P_DL.PIHeadList(DS.Tables[0]).FirstOrDefault();
                ViewBag.ItemPreview = P_DL.PIItemList(DS.Tables[1]);
                ViewBag.VendorPreview = P_DL.VendorList(DS.Tables[2]).FirstOrDefault();
                return "1";
            }
            else
            {
                return "0";
            }
        }




        //Purchase Invoice Edit
        [Route("procurement/transactions/purchase-invoice/{PI_No}/edit")]
        public IActionResult PurchaseInvoiceEdit(String PI_No)
        {
            String Active = PurchaseInvoiceDirectData(PI_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseInvoiceRegisterSummary");
            }
            ViewBag.PI_No = PI_No;
            PurchaseInvoiceGetData();
            return View(PIH_DTO);

        }

        [Route("procurement/transactions/purchase-invoice/{PI_No}/edit")]
        [HttpPost]
        public IActionResult PurchaseInvoiceEdit(PurchaseInvoiceHead_DTO PIH_DTO, String? Mode, String PI_No)
        {

            var Original_PIH_DTO = PurchaseClone(PIH_DTO);

            bool IsValid = false;
            PurchaseInvoiceHead_DTO P_Head_DTO = new PurchaseInvoiceHead_DTO();

            List<PurchaseInvoiceItem_DTO>? Item_DTO = new List<PurchaseInvoiceItem_DTO>();
            List<PurchaseInvoiceExpense_DTO>? Expense_DTO = new List<PurchaseInvoiceExpense_DTO>();
            List<PurchaseInvoiceIExpense_DTO>? ItemExpense_DTO = new List<PurchaseInvoiceIExpense_DTO>();
            List<PurchaseInvoiceBatch_DTO>? ItemBatch_DTO = new List<PurchaseInvoiceBatch_DTO>();

            P_Head_DTO = PIH_DTO;

            if (PIH_DTO.InvoiceItems != null)
                Item_DTO = PIH_DTO.InvoiceItems!.Where(PI => PI.IsDeleted == "false").ToList();

            if (PIH_DTO.Expenses != null)
                Expense_DTO = PIH_DTO.Expenses!.Where(PI => PI.PIH_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PIH_DTO.ItemExpenses!.Where(PI => PI.PII_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemBatch != null)
                ItemBatch_DTO = PIH_DTO.ItemBatch!.Where(PI => PI.PII_BCH_IsDeleted == "false").ToList();

            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PII_MS_Number) != Convert.ToInt64(PIH_DTO.PIH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PII_Qty) == 0 || Convert.ToDouble(x.PII_UnitPrice) == 0 || Convert.ToDouble(x.PII_Amount) == 0);

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
                else if (Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PIH_DTO.PIH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();

                    P_Head_DTO.InvoiceItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = ItemBatch_DTO;

                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        if (DirectBatchValidation(Item_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd"));
                                    PI_DTO.PIH_DueDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_DueDate).ToString("yyyyMMdd"));
                                    PI_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                                    PI_DTO.PIH_SupplierInvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_SupplierInvoiceDate).ToString("yyyyMMdd"));
                                    PI_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                                    PI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 1 : 0);
                                    PI_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                                    PI_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                                    PI_DTO.PIH_ExchangeRate = Convert.ToDouble(PIH_DTO.PIH_ExchangeRate);
                                    PI_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                                    PI_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                                    PI_DTO.PIH_MaterialCost = Convert.ToDouble(PIH_DTO.PIH_MaterialCost).ToString();
                                    PI_DTO.PIH_ItemMiscExpense = Convert.ToDouble(PIH_DTO.PIH_ItemMiscExpense).ToString();
                                    PI_DTO.PIH_HeaderMiscExpense = Convert.ToDouble(PIH_DTO.PIH_HeaderMiscExpense).ToString();
                                    PI_DTO.PIH_GST_Amount = Convert.ToDouble(PIH_DTO.PIH_GST_Amount).ToString();
                                    PI_DTO.PIH_InvoiceAmount = Convert.ToDouble(PIH_DTO.PIH_InvoiceAmount).ToString();
                                    PI_DTO.PIH_WHT_Amount = Convert.ToDouble(PIH_DTO.PIH_WHT_Amount).ToString();
                                    PI_DTO.PIH_RoundOff = Convert.ToDouble(PIH_DTO.PIH_RoundOff).ToString();
                                    PI_DTO.PIH_VendorPayable = Convert.ToDouble(PIH_DTO.PIH_VendorPayable).ToString();
                                    PI_DTO.Id = 101;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    String ItemDTO = string.Join(", ", Item_DTO.Where(x => Convert.ToInt64(x.PII_Number) != 0).Select(x => x.PII_Number));
                                    String ItemExpDTO = string.Join(", ", ItemExpense_DTO.Where(x => Convert.ToInt64(x.PII_EXP_Number) != 0).Select(x => x.PII_EXP_Number));
                                    String ExpDTO = string.Join(", ", Expense_DTO.Where(x => Convert.ToInt64(x.PIH_EXP_Number) != 0).Select(x => x.PIH_EXP_Number));
                                    String BatchDTO = string.Join(", ", ItemBatch_DTO.Where(x => Convert.ToInt64(x.PII_BCH_Number) != 0).Select(x => x.PII_BCH_Number));

                                    //BACTH DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(BatchDTO);
                                    PI_DTO.Id = 105;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    //PI DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(ItemExpDTO);
                                    PI_DTO.Id = 104;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    //PH DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(ItemDTO);
                                    PI_DTO.Id = 102;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    //PH EXPENSE DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(ExpDTO);
                                    PI_DTO.Id = 103;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);


                                    foreach (var Item in Item_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                        PI_DTO.PII_Item_Number = Convert.ToString(Item.PII_Item_Number);
                                        PI_DTO.PII_Warehouse_Number = Convert.ToString(Item.PII_Warehouse_Number);
                                        PI_DTO.PII_UoM_Number = Convert.ToInt64(Item.PII_UoM_Number).ToString();
                                        PI_DTO.PII_Qty = Convert.ToDouble(Item.PII_Qty).ToString();
                                        PI_DTO.PII_UnitPrice = Convert.ToDouble(Item.PII_UnitPrice).ToString();
                                        PI_DTO.PII_Amount = Convert.ToDouble(Item.PII_Amount).ToString();
                                        PI_DTO.PII_ExpenseValue = Convert.ToDouble(Item.PII_ExpenseValue);
                                        PI_DTO.PII_HSN_Number = Convert.ToString(Item.PII_HSN_Number);
                                        PI_DTO.PII_GST_Amount = Convert.ToDouble(Item.PII_GST_Amount).ToString();
                                        PI_DTO.PII_WHT_Percent = Convert.ToDouble(Item.PII_WHT_Percent).ToString();
                                        PI_DTO.PII_WHT_Amount = Convert.ToDouble(Item.PII_WHT_Amount).ToString();

                                        if (Item.PII_Number == 0)
                                        {
                                            PI_DTO.Id = 22;
                                            D = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PII_EXP_Item_Number == Item.PII_Item_Number) && (x.PII_EXP_Item_Index == Item.PII_Index));
                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                                PI_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PI_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PII_EXP_Expense_Number);
                                                PI_DTO.EXP_Remarks = ItemExp.PII_EXP_Remarks;
                                                PI_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PII_EXP_Occurrence_Number);
                                                PI_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PII_EXP_CM_Number);
                                                PI_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PII_EXP_ExpenseBase);
                                                PI_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PII_EXP_ExpenseValue);
                                                PI_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PII_EXP_Allocate_Number);
                                                if (ItemExp.PII_EXP_Number == 0)
                                                {
                                                    PI_DTO.Id = 24;
                                                }
                                                else
                                                {
                                                    PI_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_Number);
                                                    PI_DTO.Id = 107;
                                                }
                                                PI_DAO.PurchaseInvoiceDB(PI_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_Item_Number == Item.PII_Item_Number) && (x.PII_BCH_Item_Index == Item.PII_Index));
                                            foreach (var ItemBatch in Batch)
                                            {
                                                PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                                PI_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PI_DTO.PII_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PII_BCH_Date).ToString("yyyyMMdd"));
                                                PI_DTO.PII_BCH_No = Convert.ToString(ItemBatch.PII_BCH_No);
                                                PI_DTO.PII_BCH_Qty = Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                                                PI_DTO.PII_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PII_BCH_UnitPrice);
                                                PI_DTO.PII_BCH_Value = Convert.ToDouble(ItemBatch.PII_BCH_Value);
                                                if (ItemBatch.PII_BCH_Number == 0)
                                                {
                                                    PI_DTO.Id = 25;
                                                }
                                                else
                                                {
                                                    PI_DTO.PII_BCH_Number = Convert.ToInt64(ItemBatch.PII_BCH_Number);
                                                    PI_DTO.Id = 109;
                                                }
                                                PI_DAO.PurchaseInvoiceDB(PI_DTO);
                                            }
                                        }
                                        else
                                        {
                                            PI_DTO.PII_Number = Convert.ToInt64(Item.PII_Number);
                                            PI_DTO.Id = 106;
                                            D = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PII_EXP_PII_Number == Item.PII_Number));
                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                                PI_DTO.PII_Number = Convert.ToInt64(Item.PII_Number);
                                                PI_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PII_EXP_Expense_Number);
                                                PI_DTO.EXP_Remarks = ItemExp.PII_EXP_Remarks;
                                                PI_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PII_EXP_Occurrence_Number);
                                                PI_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PII_EXP_CM_Number);
                                                PI_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PII_EXP_ExpenseBase);
                                                PI_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PII_EXP_ExpenseValue);
                                                PI_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PII_EXP_Allocate_Number);
                                                if (ItemExp.PII_EXP_Number == 0)
                                                {
                                                    PI_DTO.Id = 24;
                                                }
                                                else
                                                {
                                                    PI_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_Number);
                                                    PI_DTO.Id = 108;
                                                }
                                                PI_DAO.PurchaseInvoiceDB(PI_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_PII_Number == Item.PII_Number));
                                            foreach (var ItemBatch in Batch)
                                            {
                                                PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                                PI_DTO.PII_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PII_BCH_Date).ToString("yyyyMMdd"));
                                                PI_DTO.PII_BCH_No = Convert.ToString(ItemBatch.PII_BCH_No);
                                                PI_DTO.PII_BCH_Qty = Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                                                PI_DTO.PII_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PII_BCH_UnitPrice);
                                                PI_DTO.PII_BCH_Value = Convert.ToDouble(ItemBatch.PII_BCH_Value);
                                                if (ItemBatch.PII_BCH_Number == 0)
                                                {
                                                    PI_DTO.Id = 25;
                                                }
                                                else
                                                {
                                                    PI_DTO.PII_BCH_Number = Convert.ToInt64(ItemBatch.PII_BCH_Number);
                                                    PI_DTO.Id = 109;
                                                }
                                                PI_DAO.PurchaseInvoiceDB(PI_DTO);
                                            }
                                        }
                                    }

                                    foreach (var Exp in Expense_DTO)
                                    {
                                        PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                        PI_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PIH_EXP_Expense_Number);
                                        PI_DTO.EXP_Remarks = Exp.PIH_EXP_Remarks;
                                        PI_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PIH_EXP_Occurrence_Number);
                                        PI_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PIH_EXP_CM_Number);
                                        PI_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PIH_EXP_ExpenseBase);
                                        PI_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.PIH_EXP_ExpenseValue);
                                        PI_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PIH_EXP_Allocate_Number);
                                        PI_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PIH_EXP_SAC_Number);
                                        PI_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PIH_EXP_TaxCalculate);
                                        PI_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PIH_EXP_TaxValue);
                                        if (Exp.PIH_EXP_Number == 0)
                                        {
                                            PI_DTO.Id = 23;
                                        }
                                        else
                                        {
                                            PI_DTO.EXP_Number = Convert.ToInt64(Exp.PIH_EXP_Number);
                                            PI_DTO.Id = 107;
                                        }
                                        PI_DAO.PurchaseInvoiceDB(PI_DTO);
                                    }
                                    transaction.Complete();

                                    return RedirectToAction("PurchaseInvoiceRegisterSummary");
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

            //P_Head_DTO.InvoiceItems = Item_DTO;
            //P_Head_DTO.Expenses = Expense_DTO;
            //P_Head_DTO.ItemExpenses = ItemExpense_DTO;
            //P_Head_DTO.ItemBatch = ItemBatch_DTO;

            ViewBag.PI_No = PI_No;
            PurchaseInvoiceGetData();
            return View(Original_PIH_DTO);

        }
        String PurchaseInvoiceDirectData(String PI_No)
        {
            PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
            PI_DTO.Id = 61;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                PIH_DTO = P_DL.PIHeadList(DS.Tables[0]).FirstOrDefault();
                PIH_DTO.InvoiceItems = P_DL.PIItemList(DS.Tables[1]);
                PIH_DTO.Expenses = P_DL.PIExpenseEditList(DS.Tables[2]);
                PIH_DTO.ItemExpenses = P_DL.PIIExpenseEditList(DS.Tables[3]);
                PIH_DTO.ItemBatch = P_DL.PIIBatchEditList(DS.Tables[4]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["PIH_Mode"].ToString();

                if (DS.Tables[0].Rows[0]["PIH_Mode"].ToString() == "1")
                {

                }
                else
                {
                    return "0";
                }
                return "1";
            }
            else
            {
                return "0";
            }
        }






        //Purchase Invoice PO To Item Invoice Create
        [Route("procurement/transactions/purchase-invoice/po/create")]
        public IActionResult POToPurchaseInvoiceCreate()
        {
            POToPurchaseInvoiceHead_DTO PHPO_DTO = new POToPurchaseInvoiceHead_DTO();
            if (TempData["PHPO_DTO_Json"] is string PHPODto)
            {
                PHPO_DTO = System.Text.Json.JsonSerializer.Deserialize<POToPurchaseInvoiceHead_DTO>(PHPODto);
            }

            if (PHPO_DTO.PIH_InvoiceDate == null)
            {
                PHPO_DTO.PIH_InvoiceDate = DateTime.Now.ToString("dd-MMM-yy");
                PHPO_DTO.PIH_InvoiceNo = OnPOToPurchaseInvoiceNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            }
            else
            {
                PHPO_DTO.PIH_InvoiceNo = OnPOToPurchaseInvoiceNumber(Convert.ToInt32(Convert.ToDateTime(PHPO_DTO.PIH_InvoiceDate).ToString("yyyyMMdd")));
            }
            POToPurchaseInvoiceGetData();

            PHPO_DTO.PIH_InvoiceNo = OnPOToPurchaseInvoiceNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            return View(PHPO_DTO);

        }
        [HttpPost]
        [Route("procurement/transactions/purchase-invoice/po/create")]
        public IActionResult POToPurchaseInvoiceCreate(POToPurchaseInvoiceHead_DTO PIH_DTO, String? Mode)
        {
            var Original_PIH_DTO = PurchaseClone(PIH_DTO);

            bool IsValid = false;
            POToPurchaseInvoiceHead_DTO P_Head_DTO = new POToPurchaseInvoiceHead_DTO();

            List<POToPurchaseInvoiceItem_DTO>? Item_DTO = new List<POToPurchaseInvoiceItem_DTO>();
            List<POToPurchaseInvoiceExpense_DTO>? Expense_DTO = new List<POToPurchaseInvoiceExpense_DTO>();
            List<POToPurchaseInvoiceIExpense_DTO>? ItemExpense_DTO = new List<POToPurchaseInvoiceIExpense_DTO>();
            List<POToPurchaseInvoiceBatch_DTO>? ItemBatch_DTO = new List<POToPurchaseInvoiceBatch_DTO>();

            P_Head_DTO = PIH_DTO;

            if (PIH_DTO.InvoiceItems != null)
                Item_DTO = PIH_DTO.InvoiceItems!.Where(K => K.PII_IsDeleted == "false").ToList();

            if (PIH_DTO.Expenses != null)
                Expense_DTO = PIH_DTO.Expenses!.Where(K => K.PIH_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PIH_DTO.ItemExpenses!.Where(K => K.PII_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemBatch != null)
                ItemBatch_DTO = PIH_DTO.ItemBatch!.Where(K => K.PII_BCH_IsDeleted == "false").ToList();

            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PII_MS_Number) != Convert.ToInt64(PIH_DTO.PIH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PII_Qty) == 0 || Convert.ToDouble(x.PII_UnitPrice) == 0 || Convert.ToDouble(x.PII_Amount) == 0);

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
                else if (Convert.ToInt32(PIH_DTO.PIH_Vendor_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Vendor is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PIH_DTO.PIH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();

                    P_Head_DTO.InvoiceItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = ItemBatch_DTO;
                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        if (POtoBatchValidation(Item_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String InvoiceNoOld = PIH_DTO.PIH_InvoiceNo;
                                    String InvoiceNoNew = OnPOToPurchaseInvoiceNumber(Convert.ToInt32(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd")));

                                    PIPO_DTO.PIH_InvoiceNo = InvoiceNoNew;
                                    PIPO_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd"));
                                    PIPO_DTO.PIH_DueDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_DueDate).ToString("yyyyMMdd"));
                                    PIPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                                    PIPO_DTO.PIH_SupplierInvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_SupplierInvoiceDate).ToString("yyyyMMdd"));
                                    PIPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                                    PIPO_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 1 : 0);
                                    PIPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                                    PIPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                                    PIPO_DTO.PIH_ExchangeRate = Convert.ToDouble(PIH_DTO.PIH_ExchangeRate);
                                    PIPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                                    PIPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                                    PIPO_DTO.PIH_MaterialCost = Convert.ToDouble(PIH_DTO.PIH_MaterialCost).ToString();
                                    PIPO_DTO.PIH_ItemMiscExpense = Convert.ToDouble(PIH_DTO.PIH_ItemMiscExpense).ToString();
                                    PIPO_DTO.PIH_HeaderMiscExpense = Convert.ToDouble(PIH_DTO.PIH_HeaderMiscExpense).ToString();
                                    PIPO_DTO.PIH_GST_Amount = Convert.ToDouble(PIH_DTO.PIH_GST_Amount).ToString();
                                    PIPO_DTO.PIH_InvoiceAmount = Convert.ToDouble(PIH_DTO.PIH_InvoiceAmount).ToString();
                                    PIPO_DTO.PIH_WHT_Amount = Convert.ToDouble(PIH_DTO.PIH_WHT_Amount).ToString();
                                    PIPO_DTO.PIH_RoundOff = Convert.ToDouble(PIH_DTO.PIH_RoundOff).ToString();
                                    PIPO_DTO.PIH_VendorPayable = Convert.ToDouble(PIH_DTO.PIH_VendorPayable).ToString();
                                    PIPO_DTO.Id = 21;
                                    DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);

                                    OnPOToPurchaseInvoiceNumberGen(Convert.ToInt32(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        PIPO_DTO.PIH_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in Item_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            PIPO_DTO.PII_POH_Number = Convert.ToInt64(Item.PII_POH_Number);
                                            PIPO_DTO.PII_POI_Number = Convert.ToInt64(Item.PII_POI_Number);
                                            PIPO_DTO.PII_Item_Number = Convert.ToString(Item.PII_Item_Number);
                                            PIPO_DTO.PII_Warehouse_Number = Convert.ToString(Item.PII_Warehouse_Number);
                                            PIPO_DTO.PII_UoM_Number = Convert.ToInt64(Item.PII_UoM_Number).ToString();
                                            PIPO_DTO.PII_Qty = Convert.ToDouble(Item.PII_Qty).ToString();
                                            PIPO_DTO.PII_UnitPrice = Convert.ToDouble(Item.PII_UnitPrice).ToString();
                                            PIPO_DTO.PII_Amount = Convert.ToDouble(Item.PII_Amount).ToString();
                                            PIPO_DTO.PII_ExpenseValue = Convert.ToDouble(Item.PII_ExpenseValue);
                                            PIPO_DTO.PII_HSN_Number = Convert.ToString(Item.PII_HSN_Number);
                                            PIPO_DTO.PII_GST_Amount = Convert.ToDouble(Item.PII_GST_Amount).ToString();
                                            PIPO_DTO.PII_WHT_Percent = Convert.ToDouble(Item.PII_WHT_Percent).ToString();
                                            PIPO_DTO.PII_WHT_Amount = Convert.ToDouble(Item.PII_WHT_Amount).ToString();
                                            PIPO_DTO.Id = 22;
                                            D = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PII_EXP_POI_Number == Item.PII_POI_Number && x.PII_EXP_POH_Number == Item.PII_POH_Number));

                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PIPO_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PIPO_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_Number);
                                                PIPO_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PII_EXP_Expense_Number);
                                                PIPO_DTO.EXP_Remarks = ItemExp.PII_EXP_Remarks;
                                                PIPO_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PII_EXP_Occurrence_Number);
                                                PIPO_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PII_EXP_CM_Number);
                                                PIPO_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PII_EXP_ExpenseBase);
                                                PIPO_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PII_EXP_ExpenseValue);
                                                PIPO_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PII_EXP_Allocate_Number);
                                                PIPO_DTO.Id = 24;
                                                PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_POI_Number == Item.PII_POI_Number && x.PII_BCH_POH_Number == Item.PII_POH_Number));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                PIPO_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PIPO_DTO.PII_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PII_BCH_Date).ToString("yyyyMMdd"));
                                                PIPO_DTO.PII_BCH_No = Convert.ToString(ItemBatch.PII_BCH_No);
                                                PIPO_DTO.PII_BCH_Qty = Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                                                PIPO_DTO.PII_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PII_BCH_UnitPrice);
                                                PIPO_DTO.PII_BCH_Value = Convert.ToDouble(ItemBatch.PII_BCH_Value);
                                                PIPO_DTO.Id = 25;
                                                PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
                                            }
                                        }
                                        foreach (var Exp in Expense_DTO)
                                        {
                                            PIPO_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PIH_EXP_Expense_Number);
                                            PIPO_DTO.EXP_Number = Convert.ToInt64(Exp.PIH_EXP_Number);
                                            PIPO_DTO.EXP_Remarks = Exp.PIH_EXP_Remarks;
                                            PIPO_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PIH_EXP_Occurrence_Number);
                                            PIPO_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PIH_EXP_CM_Number);
                                            PIPO_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PIH_EXP_ExpenseBase);
                                            PIPO_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.PIH_EXP_ExpenseValue);
                                            PIPO_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PIH_EXP_Allocate_Number);
                                            PIPO_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PIH_EXP_SAC_Number);
                                            PIPO_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PIH_EXP_TaxCalculate);
                                            PIPO_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PIH_EXP_TaxValue);
                                            PIPO_DTO.Id = 23;
                                            PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    P_Head_DTO.Reset();
                                    Expense_DTO = null;
                                    Item_DTO = null;
                                    ItemExpense_DTO = null;
                                    PIH_DTO.Reset();
                                    Original_PIH_DTO = PurchaseClone(PIH_DTO);

                                    return RedirectToAction("POToPurchaseInvoiceCreate");
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
            else if (Mode == "Direct")
            {
                PurchaseInvoiceHead_DTO PHPO_DTO = new PurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("PurchaseInvoiceCreate");
            }
            else if (Mode == "POITEMTO")
            {
                POItemTOPurchaseInvoiceHead_DTO PHPO_DTO = new POItemTOPurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PHPO_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("POItemToPurchaseInvoiceCreate");
            }


            POToPurchaseInvoiceGetData();
            //if (PIH_DTO.PIH_InvoiceNo != null)
            //{
            //    ViewBag.InvoiceNo = PIH_DTO.PIH_InvoiceNo;
            //}
            //else
            //{
            //    ViewBag.InvoiceNo = OnPOToPurchaseInvoiceNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            //}
            return View(Original_PIH_DTO);

        }
        Boolean POtoBatchValidation(List<POToPurchaseInvoiceItem_DTO> Item_DTO, List<POToPurchaseInvoiceBatch_DTO>? ItemBatch_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                Double BatchAmount = 0;

                Int64 POINumber = Convert.ToInt64(Item.PII_POI_Number);
                Int64 POHNumber = Convert.ToInt64(Item.PII_POH_Number);
                String PIIItem = Convert.ToString(Item.PII_Item_Number);
                Double PIIQty = Convert.ToDouble(Item.PII_Qty);
                Double PIIAmount = Convert.ToDouble(Item.PII_Amount);

                if (POINumber != 0)
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_POI_Number == POINumber));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                    }
                }

                if (BatchQty == PIIQty) { }
                else
                {
                    Message += Item.PII_ItemCode + " Batch Qty  Mismatched <br/>";
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
        void POToPurchaseInvoiceGetData()
        {
            PIPO_DTO.PIH_InvoiceDate = Convert.ToString(DateTime.Now.ToString("yyyyMMdd"));
            PIPO_DTO.Id = 1;
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);

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

        [Route("procurement/transactions/purchase-invoice/po/item")]
        public IActionResult POToPurchaseInvoiceItem(String? ItemCode, String MS)
        {
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            if (MS == null)
            {
                MS = "";
            }
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.PII_ItemCode = Convert.ToString(ItemCode).Trim();
            PIPO_DTO.PIH_MS_Number = Convert.ToString(MS).Trim();
            PIPO_DTO.Id = 2;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
            var Item = P_DL.PIIList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("procurement/transactions/purchase-invoice/po/uom")]
        public String POToPurchaseInvoiceUoM(String? UoM)
        {
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.PII_UoM_Number = Convert.ToString(UoM);
            PIPO_DTO.Id = 4;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("procurement/transactions/purchase-invoice/po/expense/des")]
        public IActionResult POToPurchaseInvoiceExpensiveDes(String? Title)
        {
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.EXP_Expense_Number = Convert.ToInt32(Title);
            PIPO_DTO.Id = 3;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
            var Expensive = P_DL.POtoPIList(DS.Tables[0]).FirstOrDefault();
            return Json(Expensive);
        }

        [Route("procurement/transactions/purchase-invoice/po/gst")]
        public String POToPurchaseInvoiceGst(String? Cluster, String? GRNDate, String? HSN, String? BaseAmount)
        {
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPO_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PIPO_DTO.PII_HSN_Number = Convert.ToString(HSN);
            PIPO_DTO.Id = 6;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = P_DL.POtoPIGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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

        [Route("procurement/transactions/purchase-invoice/po/wht")]
        public IActionResult POToPurchaseInvoiceWHT(String? Vendor, String? WHTNumber, String? GRNDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Vendor == null)
            {
                Vendor = "0";
            }
            if (GRNDate == null)
            {
                GRNDate = "0";
            }
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.PIH_Vendor_Number = Convert.ToString(Vendor);
            PIPO_DTO.PIH_WHT_Number = Convert.ToString(WHTNumber);
            PIPO_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPO_DTO.Id = 7;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
            var WHT = P_DL.POtoPIWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("procurement/transactions/purchase-invoice/po/vendor")]
        public IActionResult POToPurchaseInvoiceVendor(String? Vendor, String? GRNDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (GRNDate == null)
            {
                GRNDate = "0";
            }
            PIPO_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PIPO_DTO.PII_ItemCode = Convert.ToString(Vendor).Trim();
            PIPO_DTO.Id = 5;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
            var Ven = P_DL.PIVList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-invoice/po/vendor/get")]
        public IActionResult POToPurchaseInvoiceVendorGet(String? Vendor, String? GRNDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (GRNDate == null)
            {
                GRNDate = "0";
            }
            PIPO_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PIPO_DTO.PIH_Vendor_Number = Convert.ToString(Vendor).Trim();
            PIPO_DTO.Id = 10;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
            var Ven = P_DL.PIVList(DS.Tables[0]).FirstOrDefault();
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-invoice/po/gst/view")]
        public IActionResult POToPurchaseInvoiceGstView(String? Cluster, String? GRNDate, String? HSN, String? BaseAmount)
        {
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPO_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PIPO_DTO.PII_HSN_Number = Convert.ToString(HSN);
            PIPO_DTO.Id = 9;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<POToPurchaseInvoiceGst> PurGST = new List<POToPurchaseInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = P_DL.POtoPIGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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
                        new POToPurchaseInvoiceGst
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

        [Route("procurement/transactions/purchase-invoice/po/cluster")]
        public IActionResult POToPurchaseInvoiceCluster(String? Vendor, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.Search = Cluster;
            PIPO_DTO.PIH_Vendor_Number = Vendor;
            PIPO_DTO.Id = 8;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
            var InvCluster = P_DL.POtoPICluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("procurement/transactions/purchase-invoice/po/order")]
        public IActionResult POToPurchaseInvoicePO(String? Vendor, String? Import, String? MSNumber, String? GRNDate)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }

            PIPO_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.PIH_MS_Number = MSNumber;
            PIPO_DTO.PIH_Vendor_Number = Vendor;
            PIPO_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 1 : 0);
            PIPO_DTO.Id = 11;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
            var POOrder = P_DL.POToPIOrder(DS.Tables[0]);
            return Json(POOrder);
        }

        [Route("procurement/transactions/purchase-invoice/po/expense/gst")]
        public String POToPurchaseInvoiceHeaderGst(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PI_DTO.PII_SAC_Number = Convert.ToString(SAC);
            PI_DTO.Id = 11;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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

        [Route("procurement/transactions/purchase-invoice/po/expense/gst/view")]
        public IActionResult POToPurchaseInvoiceGstHeaderView(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PI_DTO.PII_SAC_Number = Convert.ToString(SAC);
            PI_DTO.Id = 12;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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

        [Route("procurement/transactions/purchase-invoice/po/order/item")]
        public IActionResult POToPurchaseInvoicePOItem(String? PONumber)
        {
            if (PONumber == null)
            {
                PONumber = "";
            }

            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPO_DTO.Search = PONumber;
            PIPO_DTO.Id = 12;
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);

            POToPI_DTO POPI = new POToPI_DTO();
            var POItem = P_DL.POToPIOrderItem(DS.Tables[0]);
            var POExpense = P_DL.POToPIExpense(DS.Tables[1]);
            var POItemExpeense = P_DL.POToPIIExpense(DS.Tables[2]);

            POPI.POItems = POItem;
            POPI.POExpenses = POExpense;
            POPI.POItemExpenses = POItemExpeense;

            return Json(POPI);
        }



        void OnPOToPurchaseInvoiceNumberGen(Int32 PIDate)
        {
            DataSet DS1 = new DataSet();
            PIN_DTO.PIN_Date = PIDate.ToString();
            PIN_DTO.Id = 101;
            DS1 = PIN_DAO.PINumberDB(PIN_DTO);
            if (DS1.Tables[0].Rows.Count > 0)
            {
                Int32 Order = Convert.ToInt32(DS1.Tables[0].Rows[0]["PIN_Method"].ToString());
                DateTime PI_Date = Convert.ToDateTime(DateTime.ParseExact(PIDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));

                DateTime StartDate = new DateTime();
                DateTime EndDate = new DateTime();

                if (Order == 2)
                {
                    if (DS1.Tables[1].Rows.Count > 0)
                    {
                        Int32 Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["StartingNumber"].ToString());

                        PIN_DTO.PIN_Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["Number"].ToString());
                        PIN_DTO.PIN_StartingNumber = Convert.ToString(Convert.ToInt32(Number + 1));
                        PIN_DTO.Id = 103;
                        PIN_DAO.PINumberDB(PIN_DTO);
                    }
                    else
                    {
                        Int32 Frequency = 0;
                        Int32 Start = 0;
                        DateTime Date = new DateTime();

                        if (DS1.Tables[2].Rows.Count > 0)
                        {
                            Date = Convert.ToDateTime(DS1.Tables[2].Rows[0]["PIR_Date"].ToString());
                            Start = Convert.ToInt32(DS1.Tables[2].Rows[0]["PIR_StartingNumber"].ToString());
                            Frequency = Convert.ToInt32(DS1.Tables[2].Rows[0]["PIR_Frequency"].ToString());
                        }

                        if (Frequency == 4)
                        {
                            if (Date.Month == PI_Date.Month)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PI_Date.Year, PI_Date.Month, 1);
                                EndDate = new DateTime(PI_Date.Year, PI_Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                        }
                        else if (Frequency == 5)
                        {
                            if (Date.Month == PI_Date.Month && Date.Year == PI_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else if (Date.Month != PI_Date.Month && Date.Year == PI_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PI_Date.Year, Date.Month, 1);
                                EndDate = new DateTime(PI_Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                        }

                        PIN_DTO.PIN_Number = Convert.ToInt32(DS1.Tables[2].Rows[0]["PIR_Number"].ToString());
                        PIN_DTO.PIN_StartingNumber = Convert.ToString(Start);
                        PIN_DTO.PIN_Date = Convert.ToString(StartDate.ToString("yyyyMMdd"));
                        PIN_DTO.PIN_Method = Convert.ToString(EndDate.ToString("yyyyMMdd"));
                        PIN_DTO.Id = 102;
                        PIN_DAO.PINumberDB(PIN_DTO);
                    }
                }
            }
        }

        [Route("procurement/transactions/purchase-invoice/po/numbering")]
        public String OnPOToPurchaseInvoiceNumber(Int32 PIDate)
        {
            PIPO_DTO.PIH_InvoiceDate = Convert.ToString(PIDate);
            PIPO_DTO.Id = 0;
            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);

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
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["PIN_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["PIP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["PIS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0)
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_Frequency"].ToString());

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
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["PIR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_Frequency"].ToString());

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
                ViewBag.ErrorMessage = "GRN Number Not assigned for Given date";
            }
            return "";
        }






        //Purchase Invoice PO To Item Invoice Edit
        [Route("procurement/transactions/purchase-invoice/po/{PI_No}/edit")]
        public IActionResult POToPurchaseInvoiceEdit(String PI_No)
        {
            String Active = PurchasePOInvoiceData(PI_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseInvoiceRegisterSummary");
            }
            ViewBag.PI_No = PI_No;
            PurchaseInvoiceGetData();
            return View(PIHPO_DTO);
        }
        [HttpPost]
        [Route("procurement/transactions/purchase-invoice/po/{PI_No}/edit")]
        public IActionResult POToPurchaseInvoiceEdit(POToPurchaseInvoiceHead_DTO PIH_DTO, String? Mode, String PI_No)
        {
            var Original_PIH_DTO = PurchaseClone(PIH_DTO);

            bool IsValid = false;
            POToPurchaseInvoiceHead_DTO P_Head_DTO = new POToPurchaseInvoiceHead_DTO();

            List<POToPurchaseInvoiceItem_DTO>? Item_DTO = new List<POToPurchaseInvoiceItem_DTO>();
            List<POToPurchaseInvoiceExpense_DTO>? Expense_DTO = new List<POToPurchaseInvoiceExpense_DTO>();
            List<POToPurchaseInvoiceIExpense_DTO>? ItemExpense_DTO = new List<POToPurchaseInvoiceIExpense_DTO>();
            List<POToPurchaseInvoiceBatch_DTO>? ItemBatch_DTO = new List<POToPurchaseInvoiceBatch_DTO>();

            P_Head_DTO = PIH_DTO;

            if (PIH_DTO.InvoiceItems != null)
                Item_DTO = PIH_DTO.InvoiceItems!.Where(K => K.PII_IsDeleted == "false").ToList();

            if (PIH_DTO.Expenses != null)
                Expense_DTO = PIH_DTO.Expenses!.Where(K => K.PIH_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PIH_DTO.ItemExpenses!.Where(K => K.PII_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemBatch != null)
                ItemBatch_DTO = PIH_DTO.ItemBatch!.Where(K => K.PII_BCH_IsDeleted == "false").ToList();

            PIPO_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PII_MS_Number) != Convert.ToInt64(PIH_DTO.PIH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PII_Qty) == 0 || Convert.ToDouble(x.PII_UnitPrice) == 0 || Convert.ToDouble(x.PII_Amount) == 0);

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
                else if (Convert.ToInt32(PIH_DTO.PIH_Vendor_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Vendor is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PIH_DTO.PIH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();

                    P_Head_DTO.InvoiceItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = ItemBatch_DTO;
                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        if (POtoBatchValidation(Item_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    PIPO_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PIPO_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd"));
                                    PIPO_DTO.PIH_DueDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_DueDate).ToString("yyyyMMdd"));
                                    PIPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                                    PIPO_DTO.PIH_SupplierInvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_SupplierInvoiceDate).ToString("yyyyMMdd"));
                                    PIPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                                    PIPO_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 1 : 0);
                                    PIPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                                    PIPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                                    PIPO_DTO.PIH_ExchangeRate = Convert.ToDouble(PIH_DTO.PIH_ExchangeRate);
                                    PIPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                                    PIPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                                    PIPO_DTO.PIH_MaterialCost = Convert.ToDouble(PIH_DTO.PIH_MaterialCost).ToString();
                                    PIPO_DTO.PIH_ItemMiscExpense = Convert.ToDouble(PIH_DTO.PIH_ItemMiscExpense).ToString();
                                    PIPO_DTO.PIH_HeaderMiscExpense = Convert.ToDouble(PIH_DTO.PIH_HeaderMiscExpense).ToString();
                                    PIPO_DTO.PIH_GST_Amount = Convert.ToDouble(PIH_DTO.PIH_GST_Amount).ToString();
                                    PIPO_DTO.PIH_InvoiceAmount = Convert.ToDouble(PIH_DTO.PIH_InvoiceAmount).ToString();
                                    PIPO_DTO.PIH_WHT_Amount = Convert.ToDouble(PIH_DTO.PIH_WHT_Amount).ToString();
                                    PIPO_DTO.PIH_RoundOff = Convert.ToDouble(PIH_DTO.PIH_RoundOff).ToString();
                                    PIPO_DTO.PIH_VendorPayable = Convert.ToDouble(PIH_DTO.PIH_VendorPayable).ToString();
                                    PIPO_DTO.Id = 101;
                                    DS = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);


                                    String ItemDTO = string.Join(", ", Item_DTO.Where(x => Convert.ToInt64(x.PII_Number) != 0).Select(x => x.PII_Number));
                                    String ItemExpDTO = string.Join(", ", ItemExpense_DTO.Where(x => Convert.ToInt64(x.PII_EXP_Number) != 0).Select(x => x.PII_EXP_Number));
                                    String ExpDTO = string.Join(", ", Expense_DTO.Where(x => Convert.ToInt64(x.PIH_EXP_Number) != 0).Select(x => x.PIH_EXP_Number));
                                    String BatchDTO = string.Join(", ", ItemBatch_DTO.Where(x => Convert.ToInt64(x.PII_BCH_Number) != 0).Select(x => x.PII_BCH_Number));

                                    //BACTH DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(BatchDTO);
                                    PI_DTO.Id = 105;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    //PI DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(ItemExpDTO);
                                    PI_DTO.Id = 104;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    //PH DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(ItemDTO);
                                    PI_DTO.Id = 102;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    //PH EXPENSE DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(ExpDTO);
                                    PI_DTO.Id = 103;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    PIPO_DTO.PIH_Number = Convert.ToInt64(PI_No);

                                    foreach (var Item in Item_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        PIPO_DTO.PII_POH_Number = Convert.ToInt64(Item.PII_POH_Number);
                                        PIPO_DTO.PII_POI_Number = Convert.ToInt64(Item.PII_POI_Number);
                                        PIPO_DTO.PII_Item_Number = Convert.ToString(Item.PII_Item_Number);
                                        PIPO_DTO.PII_Warehouse_Number = Convert.ToString(Item.PII_Warehouse_Number);
                                        PIPO_DTO.PII_UoM_Number = Convert.ToInt64(Item.PII_UoM_Number).ToString();
                                        PIPO_DTO.PII_Qty = Convert.ToDouble(Item.PII_Qty).ToString();
                                        PIPO_DTO.PII_UnitPrice = Convert.ToDouble(Item.PII_UnitPrice).ToString();
                                        PIPO_DTO.PII_Amount = Convert.ToDouble(Item.PII_Amount).ToString();
                                        PIPO_DTO.PII_ExpenseValue = Convert.ToDouble(Item.PII_ExpenseValue);
                                        PIPO_DTO.PII_HSN_Number = Convert.ToString(Item.PII_HSN_Number);
                                        PIPO_DTO.PII_GST_Amount = Convert.ToDouble(Item.PII_GST_Amount).ToString();
                                        PIPO_DTO.PII_WHT_Percent = Convert.ToDouble(Item.PII_WHT_Percent).ToString();
                                        PIPO_DTO.PII_WHT_Amount = Convert.ToDouble(Item.PII_WHT_Amount).ToString();

                                        if (Item.PII_Number == 0)
                                        {
                                            PIPO_DTO.Id = 22;
                                            D = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PII_EXP_POI_Number == Item.PII_POI_Number && x.PII_EXP_POH_Number == Item.PII_POH_Number));

                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PIPO_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PIPO_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_POI_EXP_Number);
                                                PIPO_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PII_EXP_Expense_Number);
                                                PIPO_DTO.EXP_Remarks = ItemExp.PII_EXP_Remarks;
                                                PIPO_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PII_EXP_Occurrence_Number);
                                                PIPO_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PII_EXP_CM_Number);
                                                PIPO_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PII_EXP_ExpenseBase);
                                                PIPO_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PII_EXP_ExpenseValue);
                                                PIPO_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PII_EXP_Allocate_Number);
                                                PIPO_DTO.Id = 24;
                                                PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_POI_Number == Item.PII_POI_Number && x.PII_BCH_POH_Number == Item.PII_POH_Number));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                PIPO_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PIPO_DTO.PII_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PII_BCH_Date).ToString("yyyyMMdd"));
                                                PIPO_DTO.PII_BCH_No = Convert.ToString(ItemBatch.PII_BCH_No);
                                                PIPO_DTO.PII_BCH_Qty = Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                                                PIPO_DTO.PII_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PII_BCH_UnitPrice);
                                                PIPO_DTO.PII_BCH_Value = Convert.ToDouble(ItemBatch.PII_BCH_Value);
                                                PIPO_DTO.Id = 25;
                                                PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
                                            }
                                        }
                                        else
                                        {
                                            PIPO_DTO.PII_Number = Convert.ToInt64(Item.PII_Number);
                                            PIPO_DTO.Id = 106;
                                            D = PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PII_EXP_POI_Number == Item.PII_POI_Number && x.PII_EXP_POH_Number == Item.PII_POH_Number));
                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PIPO_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                                PIPO_DTO.PII_Number = Convert.ToInt64(Item.PII_Number);
                                                PIPO_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PII_EXP_Expense_Number);
                                                PIPO_DTO.EXP_Remarks = ItemExp.PII_EXP_Remarks;
                                                PIPO_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PII_EXP_Occurrence_Number);
                                                PIPO_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PII_EXP_CM_Number);
                                                PIPO_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PII_EXP_ExpenseBase);
                                                PIPO_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PII_EXP_ExpenseValue);
                                                PIPO_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PII_EXP_Allocate_Number);
                                                if (ItemExp.PII_EXP_Number == 0)
                                                {
                                                    PIPO_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_POI_EXP_Number);
                                                    PIPO_DTO.Id = 24;
                                                }
                                                else
                                                {
                                                    PIPO_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_Number);
                                                    PIPO_DTO.Id = 108;
                                                }
                                                PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_POI_Number == Item.PII_POI_Number && x.PII_BCH_POH_Number == Item.PII_POH_Number));
                                            //var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_PII_Number == Item.PII_Number));
                                            foreach (var ItemBatch in Batch)
                                            {
                                                PIPO_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                                PIPO_DTO.PII_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PII_BCH_Date).ToString("yyyyMMdd"));
                                                PIPO_DTO.PII_BCH_No = Convert.ToString(ItemBatch.PII_BCH_No);
                                                PIPO_DTO.PII_BCH_Qty = Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                                                PIPO_DTO.PII_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PII_BCH_UnitPrice);
                                                PIPO_DTO.PII_BCH_Value = Convert.ToDouble(ItemBatch.PII_BCH_Value);
                                                if (ItemBatch.PII_BCH_Number == 0)
                                                {
                                                    PIPO_DTO.Id = 25;
                                                }
                                                else
                                                {
                                                    PIPO_DTO.PII_BCH_Number = Convert.ToInt64(ItemBatch.PII_BCH_Number);
                                                    PIPO_DTO.Id = 109;
                                                }
                                                PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
                                            }
                                        }

                                    }
                                    foreach (var Exp in Expense_DTO)
                                    {
                                        PIPO_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PIH_EXP_Expense_Number);
                                        PIPO_DTO.EXP_Remarks = Exp.PIH_EXP_Remarks;
                                        PIPO_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PIH_EXP_Occurrence_Number);
                                        PIPO_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PIH_EXP_CM_Number);
                                        PIPO_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PIH_EXP_ExpenseBase);
                                        PIPO_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.PIH_EXP_ExpenseValue);
                                        PIPO_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PIH_EXP_Allocate_Number);
                                        PIPO_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PIH_EXP_SAC_Number);
                                        PIPO_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PIH_EXP_TaxCalculate);
                                        PIPO_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PIH_EXP_TaxValue);
                                        if (Exp.PIH_EXP_Number == 0)
                                        {
                                            PIPO_DTO.EXP_Number = Convert.ToInt64(Exp.PIH_EXP_POH_EXP_Number);
                                            PIPO_DTO.Id = 23;
                                        }
                                        else
                                        {
                                            PIPO_DTO.EXP_Number = Convert.ToInt64(Exp.PIH_EXP_Number);
                                            PIPO_DTO.Id = 107;
                                        }
                                        PIPO_DAO.POToPurchaseInvoiceDB(PIPO_DTO);
                                    }

                                    transaction.Complete();

                                    P_Head_DTO.Reset();
                                    Expense_DTO = null;
                                    Item_DTO = null;
                                    ItemExpense_DTO = null;
                                    PIH_DTO.Reset();
                                    Original_PIH_DTO = PurchaseClone(PIH_DTO);

                                    return RedirectToAction("PurchaseInvoiceRegisterSummary");
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
            else if (Mode == "Direct")
            {
                PurchaseInvoiceHead_DTO PHPO_DTO = new PurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("PurchaseInvoiceCreate");
            }
            else if (Mode == "POITEMTO")
            {
                POItemTOPurchaseInvoiceHead_DTO PHPO_DTO = new POItemTOPurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PHPO_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("POItemToPurchaseInvoiceCreate");
            }


            POToPurchaseInvoiceGetData();
            return View(Original_PIH_DTO);
        }
        String PurchasePOInvoiceData(String PI_No)
        {
            PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
            PI_DTO.Id = 61;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                PIHPO_DTO = P_DL.POToPIHeadList(DS.Tables[0]).FirstOrDefault();
                PIHPO_DTO.InvoiceItems = P_DL.POToPIItemList(DS.Tables[1]);
                PIHPO_DTO.Expenses = P_DL.POToPIExpenseEditList(DS.Tables[2]);
                PIHPO_DTO.ItemExpenses = P_DL.POToPIIExpenseEditList(DS.Tables[3]);
                PIHPO_DTO.ItemBatch = P_DL.POToPIIBatchEditList(DS.Tables[4]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["PIH_Mode"].ToString();

                if (DS.Tables[0].Rows[0]["PIH_Mode"].ToString() == "2")
                {

                }
                else
                {
                    return "0";
                }
                return "1";
            }
            else
            {
                return "0";
            }
        }








        //Purchase Invoice PO Item To Invoice Create
        [Route("procurement/transactions/purchase-invoice/poitem/create")]
        public IActionResult POItemTOPurchaseInvoiceCreate()
        {
            POItemTOPurchaseInvoiceHead_DTO PHPO_DTO = new POItemTOPurchaseInvoiceHead_DTO();
            if (TempData["PHPO_DTO_Json"] is string PHPODto)
            {
                PHPO_DTO = System.Text.Json.JsonSerializer.Deserialize<POItemTOPurchaseInvoiceHead_DTO>(PHPODto);
            }

            if (PHPO_DTO.PIH_InvoiceDate == null)
            {
                PHPO_DTO.PIH_InvoiceDate = DateTime.Now.ToString("dd-MMM-yy");
                PHPO_DTO.PIH_InvoiceNo = OnPOItemTOPurchaseInvoiceNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            }
            else
            {
                PHPO_DTO.PIH_InvoiceNo = OnPOItemTOPurchaseInvoiceNumber(Convert.ToInt32(Convert.ToDateTime(PHPO_DTO.PIH_InvoiceDate).ToString("yyyyMMdd")));
            }
            POItemTOPurchaseInvoiceGetData();

            //PHPO_DTO.PIH_InvoiceNo = OnPOItemTOPurchaseInvoiceNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            return View(PHPO_DTO);

        }
        [HttpPost]
        [Route("procurement/transactions/purchase-invoice/poitem/create")]
        public IActionResult POItemTOPurchaseInvoiceCreate(POItemTOPurchaseInvoiceHead_DTO PIH_DTO, String? Mode)
        {
            var Original_PIH_DTO = PurchaseClone(PIH_DTO);

            bool IsValid = false;
            POItemTOPurchaseInvoiceHead_DTO P_Head_DTO = new POItemTOPurchaseInvoiceHead_DTO();

            List<POItemTOPurchaseInvoiceItem_DTO>? Item_DTO = new List<POItemTOPurchaseInvoiceItem_DTO>();
            List<POItemTOPurchaseInvoiceExpense_DTO>? Expense_DTO = new List<POItemTOPurchaseInvoiceExpense_DTO>();
            List<POItemTOPurchaseInvoiceIExpense_DTO>? ItemExpense_DTO = new List<POItemTOPurchaseInvoiceIExpense_DTO>();
            List<POItemTOPurchaseInvoiceBatch_DTO>? ItemBatch_DTO = new List<POItemTOPurchaseInvoiceBatch_DTO>();

            P_Head_DTO = PIH_DTO;

            if (PIH_DTO.InvoiceItems != null)
                Item_DTO = PIH_DTO.InvoiceItems!.Where(K => K.PII_IsDeleted == "false").ToList();

            if (PIH_DTO.Expenses != null)
                Expense_DTO = PIH_DTO.Expenses!.Where(K => K.PIH_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PIH_DTO.ItemExpenses!.Where(K => K.PII_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemBatch != null)
                ItemBatch_DTO = PIH_DTO.ItemBatch!.Where(K => K.PII_BCH_IsDeleted == "false").ToList();

            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PII_MS_Number) != Convert.ToInt64(PIH_DTO.PIH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PII_Qty) == 0 || Convert.ToDouble(x.PII_UnitPrice) == 0 || Convert.ToDouble(x.PII_Amount) == 0);

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
                else if (Convert.ToInt32(PIH_DTO.PIH_Vendor_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Vendor is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PIH_DTO.PIH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();
                    P_Head_DTO.InvoiceItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = ItemBatch_DTO;
                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        if (POItemtoBatchValidation(Item_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String InvoiceNoOld = PIH_DTO.PIH_InvoiceNo;
                                    String InvoiceNoNew = OnPOItemTOPurchaseInvoiceNumber(Convert.ToInt32(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd")));

                                    PIPOI_DTO.PIH_InvoiceNo = InvoiceNoNew;
                                    PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd"));
                                    PIPOI_DTO.PIH_DueDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_DueDate).ToString("yyyyMMdd"));
                                    PIPOI_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                                    PIPOI_DTO.PIH_SupplierInvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_SupplierInvoiceDate).ToString("yyyyMMdd"));
                                    PIPOI_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                                    PIPOI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 1 : 0);
                                    PIPOI_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                                    PIPOI_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                                    PIPOI_DTO.PIH_ExchangeRate = Convert.ToDouble(PIH_DTO.PIH_ExchangeRate);
                                    PIPOI_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                                    PIPOI_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                                    PIPOI_DTO.PIH_MaterialCost = Convert.ToDouble(PIH_DTO.PIH_MaterialCost).ToString();
                                    PIPOI_DTO.PIH_ItemMiscExpense = Convert.ToDouble(PIH_DTO.PIH_ItemMiscExpense).ToString();
                                    PIPOI_DTO.PIH_HeaderMiscExpense = Convert.ToDouble(PIH_DTO.PIH_HeaderMiscExpense).ToString();
                                    PIPOI_DTO.PIH_GST_Amount = Convert.ToDouble(PIH_DTO.PIH_GST_Amount).ToString();
                                    PIPOI_DTO.PIH_InvoiceAmount = Convert.ToDouble(PIH_DTO.PIH_InvoiceAmount).ToString();
                                    PIPOI_DTO.PIH_WHT_Amount = Convert.ToDouble(PIH_DTO.PIH_WHT_Amount).ToString();
                                    PIPOI_DTO.PIH_RoundOff = Convert.ToDouble(PIH_DTO.PIH_RoundOff).ToString();
                                    PIPOI_DTO.PIH_VendorPayable = Convert.ToDouble(PIH_DTO.PIH_VendorPayable).ToString();
                                    PIPOI_DTO.Id = 21;
                                    DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);

                                    OnPOItemTOPurchaseInvoiceNumberGen(Convert.ToInt32(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        PIPOI_DTO.PIH_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in Item_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            PIPOI_DTO.PII_POH_Number = Convert.ToInt64(Item.PII_POH_Number);
                                            PIPOI_DTO.PII_POI_Number = Convert.ToInt64(Item.PII_POI_Number);
                                            PIPOI_DTO.PII_Item_Number = Convert.ToString(Item.PII_Item_Number);
                                            PIPOI_DTO.PII_Warehouse_Number = Convert.ToString(Item.PII_Warehouse_Number);
                                            PIPOI_DTO.PII_UoM_Number = Convert.ToInt64(Item.PII_UoM_Number).ToString();
                                            PIPOI_DTO.PII_Qty = Convert.ToDouble(Item.PII_Qty).ToString();
                                            PIPOI_DTO.PII_UnitPrice = Convert.ToDouble(Item.PII_UnitPrice).ToString();
                                            PIPOI_DTO.PII_Amount = Convert.ToDouble(Item.PII_Amount).ToString();
                                            PIPOI_DTO.PII_ExpenseValue = Convert.ToDouble(Item.PII_ExpenseValue);
                                            PIPOI_DTO.PII_HSN_Number = Convert.ToString(Item.PII_HSN_Number);
                                            PIPOI_DTO.PII_GST_Amount = Convert.ToDouble(Item.PII_GST_Amount).ToString();
                                            PIPOI_DTO.PII_WHT_Percent = Convert.ToDouble(Item.PII_WHT_Percent).ToString();
                                            PIPOI_DTO.PII_WHT_Amount = Convert.ToDouble(Item.PII_WHT_Amount).ToString();
                                            PIPOI_DTO.Id = 22;
                                            D = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PII_EXP_POI_Number == Item.PII_POI_Number && x.PII_EXP_POH_Number == Item.PII_POH_Number));

                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PIPOI_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PIPOI_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_Number);
                                                PIPOI_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PII_EXP_Expense_Number);
                                                PIPOI_DTO.EXP_Remarks = ItemExp.PII_EXP_Remarks;
                                                PIPOI_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PII_EXP_Occurrence_Number);
                                                PIPOI_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PII_EXP_CM_Number);
                                                PIPOI_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PII_EXP_ExpenseBase);
                                                PIPOI_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PII_EXP_ExpenseValue);
                                                PIPOI_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PII_EXP_Allocate_Number);
                                                PIPOI_DTO.Id = 24;
                                                PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_POI_Number == Item.PII_POI_Number && x.PII_BCH_POH_Number == Item.PII_POH_Number));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                PIPOI_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PIPOI_DTO.PII_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PII_BCH_Date).ToString("yyyyMMdd"));
                                                PIPOI_DTO.PII_BCH_No = Convert.ToString(ItemBatch.PII_BCH_No);
                                                PIPOI_DTO.PII_BCH_Qty = Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                                                PIPOI_DTO.PII_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PII_BCH_UnitPrice);
                                                PIPOI_DTO.PII_BCH_Value = Convert.ToDouble(ItemBatch.PII_BCH_Value);
                                                PIPOI_DTO.Id = 25;
                                                PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
                                            }
                                        }
                                        foreach (var Exp in Expense_DTO)
                                        {
                                            PIPOI_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PIH_EXP_Expense_Number);
                                            PIPOI_DTO.EXP_Number = Convert.ToInt64(Exp.PIH_EXP_Number);
                                            PIPOI_DTO.EXP_Remarks = Exp.PIH_EXP_Remarks;
                                            PIPOI_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PIH_EXP_Occurrence_Number);
                                            PIPOI_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PIH_EXP_CM_Number);
                                            PIPOI_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PIH_EXP_ExpenseBase);
                                            PIPOI_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.PIH_EXP_ExpenseValue);
                                            PIPOI_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PIH_EXP_Allocate_Number);
                                            PIPOI_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PIH_EXP_SAC_Number);
                                            PIPOI_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PIH_EXP_TaxCalculate);
                                            PIPOI_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PIH_EXP_TaxValue);
                                            PIPOI_DTO.Id = 23;
                                            PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    P_Head_DTO.Reset();
                                    Expense_DTO = null;
                                    Item_DTO = null;
                                    ItemExpense_DTO = null;
                                    PIH_DTO.Reset();
                                    Original_PIH_DTO = PurchaseClone(PIH_DTO);

                                    return RedirectToAction("POItemTOPurchaseInvoiceCreate");
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
            else if (Mode == "Direct")
            {
                PurchaseInvoiceHead_DTO PHPO_DTO = new PurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("PurchaseInvoiceCreate");
            }
            else if (Mode == "POTOITEM")
            {
                POToPurchaseInvoiceHead_DTO PHPO_DTO = new POToPurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PHPO_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("POToPurchaseInvoiceCreate");
            }


            POItemTOPurchaseInvoiceGetData();
            //if (PIH_DTO.PIH_InvoiceNo != null)
            //{
            //    ViewBag.InvoiceNo = PIH_DTO.PIH_InvoiceNo;
            //}
            //else
            //{
            //    ViewBag.InvoiceNo = OnPOItemTOPurchaseInvoiceNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            //}

            return View(Original_PIH_DTO);
        }

        Boolean POItemtoBatchValidation(List<POItemTOPurchaseInvoiceItem_DTO> Item_DTO, List<POItemTOPurchaseInvoiceBatch_DTO>? ItemBatch_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                Double BatchAmount = 0;

                Int64 POINumber = Convert.ToInt64(Item.PII_POI_Number);
                Int64 POHNumber = Convert.ToInt64(Item.PII_POH_Number);
                String PIIItem = Convert.ToString(Item.PII_Item_Number);
                Double PIIQty = Convert.ToDouble(Item.PII_Qty);
                Double PIIAmount = Convert.ToDouble(Item.PII_Amount);

                if (POINumber != 0)
                {
                    var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_POI_Number == POINumber));

                    foreach (var ItemBatch in Batch)
                    {
                        BatchQty += Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                    }
                }

                if (BatchQty == PIIQty) { }
                else
                {
                    Message += Item.PII_ItemCode + " Batch Qty  Mismatched <br/>";
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
        void POItemTOPurchaseInvoiceGetData()
        {
            PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(DateTime.Now.ToString("yyyyMMdd"));
            PIPOI_DTO.Id = 1;
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);

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

        [Route("procurement/transactions/purchase-invoice/poitem/item")]
        public IActionResult POItemTOPurchaseInvoiceItem(String? ItemCode, String MS)
        {
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            if (MS == null)
            {
                MS = "";
            }
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.PII_ItemCode = Convert.ToString(ItemCode).Trim();
            PIPOI_DTO.PIH_MS_Number = Convert.ToString(MS).Trim();
            PIPOI_DTO.Id = 2;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
            var Item = P_DL.PIIList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("procurement/transactions/purchase-invoice/poitem/uom")]
        public String POItemTOPurchaseInvoiceUoM(String? UoM)
        {
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.PII_UoM_Number = Convert.ToString(UoM);
            PIPOI_DTO.Id = 4;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("procurement/transactions/purchase-invoice/poitem/expense/des")]
        public IActionResult POItemTOPurchaseInvoiceExpensiveDes(String? Title)
        {
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.EXP_Expense_Number = Convert.ToInt32(Title);
            PIPOI_DTO.Id = 3;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
            var Expensive = P_DL.POItemTOPIList(DS.Tables[0]).FirstOrDefault();
            return Json(Expensive);
        }

        [Route("procurement/transactions/purchase-invoice/poitem/gst")]
        public String POItemTOPurchaseInvoiceGst(String? Cluster, String? GRNDate, String? HSN, String? BaseAmount)
        {
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPOI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PIPOI_DTO.PII_HSN_Number = Convert.ToString(HSN);
            PIPOI_DTO.Id = 6;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = P_DL.POItemTOPIGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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

        [Route("procurement/transactions/purchase-invoice/poitem/wht")]
        public IActionResult POItemTOPurchaseInvoiceWHT(String? Vendor, String? WHTNumber, String? GRNDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Vendor == null)
            {
                Vendor = "0";
            }
            if (GRNDate == null)
            {
                GRNDate = "0";
            }
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.PIH_Vendor_Number = Convert.ToString(Vendor);
            PIPOI_DTO.PIH_WHT_Number = Convert.ToString(WHTNumber);
            PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPOI_DTO.Id = 7;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
            var WHT = P_DL.POItemTOPIWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("procurement/transactions/purchase-invoice/poitem/vendor")]
        public IActionResult POItemTOPurchaseInvoiceVendor(String? Vendor, String? GRNDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (GRNDate == null)
            {
                GRNDate = "0";
            }
            PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PIPOI_DTO.PII_ItemCode = Convert.ToString(Vendor).Trim();
            PIPOI_DTO.Id = 5;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
            var Ven = P_DL.PIVList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-invoice/poitem/vendor/get")]
        public IActionResult POItemTOPurchaseInvoiceVendorGet(String? Vendor, String? GRNDate, String? Import)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }
            if (GRNDate == null)
            {
                GRNDate = "0";
            }
            PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 2 : 1);
            PIPOI_DTO.PIH_Vendor_Number = Convert.ToString(Vendor).Trim();
            PIPOI_DTO.Id = 10;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
            var Ven = P_DL.PIVList(DS.Tables[0]).FirstOrDefault();
            return Json(Ven);
        }

        [Route("procurement/transactions/purchase-invoice/poitem/gst/view")]
        public IActionResult POItemTOPurchaseInvoiceGstView(String? Cluster, String? GRNDate, String? HSN, String? BaseAmount)
        {
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPOI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PIPOI_DTO.PII_HSN_Number = Convert.ToString(HSN);
            PIPOI_DTO.Id = 9;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<POItemTOPurchaseInvoiceGst> PurGST = new List<POItemTOPurchaseInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = P_DL.POItemTOPIGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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
                        new POItemTOPurchaseInvoiceGst
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

        [Route("procurement/transactions/purchase-invoice/poitem/cluster")]
        public IActionResult POItemTOPurchaseInvoiceCluster(String? Vendor, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.Search = Cluster;
            PIPOI_DTO.PIH_Vendor_Number = Vendor;
            PIPOI_DTO.Id = 8;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
            var InvCluster = P_DL.POItemTOPICluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("procurement/transactions/purchase-invoice/poitem/order")]
        public IActionResult POItemTOPurchaseInvoicePO(String? Vendor, String? Import, String? MSNumber, String? GRNDate)
        {
            if (Vendor == null)
            {
                Vendor = "";
            }

            PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.PIH_MS_Number = MSNumber;
            PIPOI_DTO.PIH_Vendor_Number = Vendor;
            PIPOI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 1 : 0);
            PIPOI_DTO.Id = 11;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
            var POOrder = P_DL.POItemTOPIOrder(DS.Tables[0]);
            return Json(POOrder);
        }

        [Route("procurement/transactions/purchase-invoice/poitem/order/item")]
        public IActionResult POItemTOPurchaseInvoicePOItem(String? PONumber, String? Vendor, String? Import, String? MSNumber, String? GRNDate)
        {
            if (PONumber == null)
            {
                PONumber = "";
            }

            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIPOI_DTO.Search = PONumber;
            PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PIPOI_DTO.PIH_MS_Number = MSNumber;
            PIPOI_DTO.PIH_Vendor_Number = Vendor;
            PIPOI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(Import) == true ? 1 : 0);
            PIPOI_DTO.Id = 12;
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);

            POItemTOPI_DTO POPI = new POItemTOPI_DTO();
            var POItem = P_DL.POItemTOPIOrderItem(DS.Tables[0]);
            var POExpense = P_DL.POItemTOPIExpense(DS.Tables[1]);
            var POItemExpeense = P_DL.POItemTOPIIExpense(DS.Tables[2]);

            POPI.POItems = POItem;
            POPI.POExpenses = POExpense;
            POPI.POItemExpenses = POItemExpeense;

            return Json(POPI);
        }

        [Route("procurement/transactions/purchase-invoice/poitem/expense/gst")]
        public String POItemToPurchaseInvoiceHeaderGst(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PI_DTO.PII_SAC_Number = Convert.ToString(SAC);
            PI_DTO.Id = 11;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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

        [Route("procurement/transactions/purchase-invoice/poitem/expense/gst/view")]
        public IActionResult POItemToPurchaseInvoiceGstHeaderView(String? Cluster, String? GRNDate, String? SAC, String? BaseAmount)
        {
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(GRNDate).ToString("yyyyMMdd"));
            PI_DTO.PIH_TaxCluster_Number = Convert.ToString(Cluster);
            PI_DTO.PII_SAC_Number = Convert.ToString(SAC);
            PI_DTO.Id = 12;
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

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



        void OnPOItemTOPurchaseInvoiceNumberGen(Int32 PIDate)
        {
            DataSet DS1 = new DataSet();
            PIN_DTO.PIN_Date = PIDate.ToString();
            PIN_DTO.Id = 101;
            DS1 = PIN_DAO.PINumberDB(PIN_DTO);
            if (DS1.Tables[0].Rows.Count > 0)
            {
                Int32 Order = Convert.ToInt32(DS1.Tables[0].Rows[0]["PIN_Method"].ToString());
                DateTime PI_Date = Convert.ToDateTime(DateTime.ParseExact(PIDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));

                DateTime StartDate = new DateTime();
                DateTime EndDate = new DateTime();

                if (Order == 2)
                {
                    if (DS1.Tables[1].Rows.Count > 0)
                    {
                        Int32 Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["StartingNumber"].ToString());

                        PIN_DTO.PIN_Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["Number"].ToString());
                        PIN_DTO.PIN_StartingNumber = Convert.ToString(Convert.ToInt32(Number + 1));
                        PIN_DTO.Id = 103;
                        PIN_DAO.PINumberDB(PIN_DTO);
                    }
                    else
                    {
                        Int32 Frequency = 0;
                        Int32 Start = 0;
                        DateTime Date = new DateTime();

                        if (DS1.Tables[2].Rows.Count > 0)
                        {
                            Date = Convert.ToDateTime(DS1.Tables[2].Rows[0]["PIR_Date"].ToString());
                            Start = Convert.ToInt32(DS1.Tables[2].Rows[0]["PIR_StartingNumber"].ToString());
                            Frequency = Convert.ToInt32(DS1.Tables[2].Rows[0]["PIR_Frequency"].ToString());
                        }

                        if (Frequency == 4)
                        {
                            if (Date.Month == PI_Date.Month)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PI_Date.Year, PI_Date.Month, 1);
                                EndDate = new DateTime(PI_Date.Year, PI_Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                        }
                        else if (Frequency == 5)
                        {
                            if (Date.Month == PI_Date.Month && Date.Year == PI_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else if (Date.Month != PI_Date.Month && Date.Year == PI_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(PI_Date.Year, Date.Month, 1);
                                EndDate = new DateTime(PI_Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                        }

                        PIN_DTO.PIN_Number = Convert.ToInt32(DS1.Tables[2].Rows[0]["PIR_Number"].ToString());
                        PIN_DTO.PIN_StartingNumber = Convert.ToString(Start);
                        PIN_DTO.PIN_Date = Convert.ToString(StartDate.ToString("yyyyMMdd"));
                        PIN_DTO.PIN_Method = Convert.ToString(EndDate.ToString("yyyyMMdd"));
                        PIN_DTO.Id = 102;
                        PIN_DAO.PINumberDB(PIN_DTO);
                    }
                }
            }
        }

        [Route("procurement/transactions/purchase-invoice/poitem/numbering")]
        public String OnPOItemTOPurchaseInvoiceNumber(Int32 PIDate)
        {
            PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(PIDate);
            PIPOI_DTO.Id = 0;
            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);

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
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["PIN_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["PIP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["PIS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0)
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_Frequency"].ToString());

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
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["PIR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["PIR_Frequency"].ToString());

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
                ViewBag.ErrorMessage = "GRN Number Not assigned for Given date";
            }
            return "";
        }







        //Purchase Invoice PO Item To Invoice Edit
        [Route("procurement/transactions/purchase-invoice/poitem/{PI_No}/edit")]
        public IActionResult POItemToPurchaseInvoiceEdit(String PI_No)
        {
            String Active = PurchasePOItemInvoiceData(PI_No);
            if (Active != "1")
            {
                return RedirectToAction("PurchaseInvoiceRegisterSummary");
            }
            ViewBag.PI_No = PI_No;
            PurchaseInvoiceGetData();
            return View(PIHPOI_DTO);

        }
        [HttpPost]
        [Route("procurement/transactions/purchase-invoice/poitem/{PI_No}/edit")]
        public IActionResult POItemToPurchaseInvoiceEdit(POItemTOPurchaseInvoiceHead_DTO PIH_DTO, String? Mode, String PI_No)
        {
            var Original_PIH_DTO = PurchaseClone(PIH_DTO);

            bool IsValid = false;
            POItemTOPurchaseInvoiceHead_DTO P_Head_DTO = new POItemTOPurchaseInvoiceHead_DTO();

            List<POItemTOPurchaseInvoiceItem_DTO>? Item_DTO = new List<POItemTOPurchaseInvoiceItem_DTO>();
            List<POItemTOPurchaseInvoiceExpense_DTO>? Expense_DTO = new List<POItemTOPurchaseInvoiceExpense_DTO>();
            List<POItemTOPurchaseInvoiceIExpense_DTO>? ItemExpense_DTO = new List<POItemTOPurchaseInvoiceIExpense_DTO>();
            List<POItemTOPurchaseInvoiceBatch_DTO>? ItemBatch_DTO = new List<POItemTOPurchaseInvoiceBatch_DTO>();

            P_Head_DTO = PIH_DTO;

            if (PIH_DTO.InvoiceItems != null)
                Item_DTO = PIH_DTO.InvoiceItems!.Where(K => K.PII_IsDeleted == "false").ToList();

            if (PIH_DTO.Expenses != null)
                Expense_DTO = PIH_DTO.Expenses!.Where(K => K.PIH_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemExpenses != null)
                ItemExpense_DTO = PIH_DTO.ItemExpenses!.Where(K => K.PII_EXP_IsDeleted == "false").ToList();

            if (PIH_DTO.ItemBatch != null)
                ItemBatch_DTO = PIH_DTO.ItemBatch!.Where(K => K.PII_BCH_IsDeleted == "false").ToList();

            PIPOI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = Item_DTO.Where(x => Convert.ToInt64(x.PII_MS_Number) != Convert.ToInt64(PIH_DTO.PIH_MS_Number));
                var ValueItem = Item_DTO.Where(x => Convert.ToDouble(x.PII_Qty) == 0 || Convert.ToDouble(x.PII_UnitPrice) == 0 || Convert.ToDouble(x.PII_Amount) == 0);

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
                else if (Convert.ToInt32(PIH_DTO.PIH_Vendor_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Vendor is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 2 : 1) != Convert.ToInt32(PIH_DTO.PIH_VendorLocation))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Import order and vedor not match";
                }
                else
                {
                    ModelState.Clear();

                    P_Head_DTO.InvoiceItems = Item_DTO;
                    P_Head_DTO.Expenses = Expense_DTO;
                    P_Head_DTO.ItemExpenses = ItemExpense_DTO;
                    P_Head_DTO.ItemBatch = ItemBatch_DTO;
                    IsValid = TryValidateModel(P_Head_DTO);

                    if (IsValid)
                    {
                        if (POItemtoBatchValidation(Item_DTO, ItemBatch_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    PIPOI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PIPOI_DTO.PIH_InvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_InvoiceDate).ToString("yyyyMMdd"));
                                    PIPOI_DTO.PIH_DueDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_DueDate).ToString("yyyyMMdd"));
                                    PIPOI_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                                    PIPOI_DTO.PIH_SupplierInvoiceDate = Convert.ToString(Convert.ToDateTime(PIH_DTO.PIH_SupplierInvoiceDate).ToString("yyyyMMdd"));
                                    PIPOI_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                                    PIPOI_DTO.PIH_ImportOrder = Convert.ToInt32(Convert.ToBoolean(PIH_DTO.PIH_ImportOrder) ? 1 : 0);
                                    PIPOI_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                                    PIPOI_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                                    PIPOI_DTO.PIH_ExchangeRate = Convert.ToDouble(PIH_DTO.PIH_ExchangeRate);
                                    PIPOI_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                                    PIPOI_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                                    PIPOI_DTO.PIH_MaterialCost = Convert.ToDouble(PIH_DTO.PIH_MaterialCost).ToString();
                                    PIPOI_DTO.PIH_ItemMiscExpense = Convert.ToDouble(PIH_DTO.PIH_ItemMiscExpense).ToString();
                                    PIPOI_DTO.PIH_HeaderMiscExpense = Convert.ToDouble(PIH_DTO.PIH_HeaderMiscExpense).ToString();
                                    PIPOI_DTO.PIH_GST_Amount = Convert.ToDouble(PIH_DTO.PIH_GST_Amount).ToString();
                                    PIPOI_DTO.PIH_InvoiceAmount = Convert.ToDouble(PIH_DTO.PIH_InvoiceAmount).ToString();
                                    PIPOI_DTO.PIH_WHT_Amount = Convert.ToDouble(PIH_DTO.PIH_WHT_Amount).ToString();
                                    PIPOI_DTO.PIH_RoundOff = Convert.ToDouble(PIH_DTO.PIH_RoundOff).ToString();
                                    PIPOI_DTO.PIH_VendorPayable = Convert.ToDouble(PIH_DTO.PIH_VendorPayable).ToString();
                                    PIPOI_DTO.Id = 101;
                                    DS = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);


                                    String ItemDTO = string.Join(", ", Item_DTO.Where(x => Convert.ToInt64(x.PII_Number) != 0).Select(x => x.PII_Number));
                                    String ItemExpDTO = string.Join(", ", ItemExpense_DTO.Where(x => Convert.ToInt64(x.PII_EXP_Number) != 0).Select(x => x.PII_EXP_Number));
                                    String ExpDTO = string.Join(", ", Expense_DTO.Where(x => Convert.ToInt64(x.PIH_EXP_Number) != 0).Select(x => x.PIH_EXP_Number));
                                    String BatchDTO = string.Join(", ", ItemBatch_DTO.Where(x => Convert.ToInt64(x.PII_BCH_Number) != 0).Select(x => x.PII_BCH_Number));

                                    //BACTH DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(BatchDTO);
                                    PI_DTO.Id = 105;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    //PI DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(ItemExpDTO);
                                    PI_DTO.Id = 104;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    //I DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(ItemDTO);
                                    PI_DTO.Id = 102;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    //PH EXPENSE DELETE
                                    PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                    PI_DTO.DeleteNumbers = Convert.ToString(ExpDTO);
                                    PI_DTO.Id = 103;
                                    DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);

                                    PIPOI_DTO.PIH_Number = Convert.ToInt64(PI_No);

                                    foreach (var Item in Item_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        PIPOI_DTO.PII_POH_Number = Convert.ToInt64(Item.PII_POH_Number);
                                        PIPOI_DTO.PII_POI_Number = Convert.ToInt64(Item.PII_POI_Number);
                                        PIPOI_DTO.PII_Item_Number = Convert.ToString(Item.PII_Item_Number);
                                        PIPOI_DTO.PII_Warehouse_Number = Convert.ToString(Item.PII_Warehouse_Number);
                                        PIPOI_DTO.PII_UoM_Number = Convert.ToInt64(Item.PII_UoM_Number).ToString();
                                        PIPOI_DTO.PII_Qty = Convert.ToDouble(Item.PII_Qty).ToString();
                                        PIPOI_DTO.PII_UnitPrice = Convert.ToDouble(Item.PII_UnitPrice).ToString();
                                        PIPOI_DTO.PII_Amount = Convert.ToDouble(Item.PII_Amount).ToString();
                                        PIPOI_DTO.PII_ExpenseValue = Convert.ToDouble(Item.PII_ExpenseValue);
                                        PIPOI_DTO.PII_HSN_Number = Convert.ToString(Item.PII_HSN_Number);
                                        PIPOI_DTO.PII_GST_Amount = Convert.ToDouble(Item.PII_GST_Amount).ToString();
                                        PIPOI_DTO.PII_WHT_Percent = Convert.ToDouble(Item.PII_WHT_Percent).ToString();
                                        PIPOI_DTO.PII_WHT_Amount = Convert.ToDouble(Item.PII_WHT_Amount).ToString();

                                        if (Item.PII_Number == 0)
                                        {
                                            PIPOI_DTO.Id = 22;
                                            D = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PII_EXP_POI_Number == Item.PII_POI_Number && x.PII_EXP_POH_Number == Item.PII_POH_Number));

                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PIPOI_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PIPOI_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_POI_EXP_Number);
                                                PIPOI_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PII_EXP_Expense_Number);
                                                PIPOI_DTO.EXP_Remarks = ItemExp.PII_EXP_Remarks;
                                                PIPOI_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PII_EXP_Occurrence_Number);
                                                PIPOI_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PII_EXP_CM_Number);
                                                PIPOI_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PII_EXP_ExpenseBase);
                                                PIPOI_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PII_EXP_ExpenseValue);
                                                PIPOI_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PII_EXP_Allocate_Number);
                                                PIPOI_DTO.Id = 24;
                                                PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_POI_Number == Item.PII_POI_Number && x.PII_BCH_POH_Number == Item.PII_POH_Number));

                                            foreach (var ItemBatch in Batch)
                                            {
                                                PIPOI_DTO.PII_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                PIPOI_DTO.PII_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PII_BCH_Date).ToString("yyyyMMdd"));
                                                PIPOI_DTO.PII_BCH_No = Convert.ToString(ItemBatch.PII_BCH_No);
                                                PIPOI_DTO.PII_BCH_Qty = Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                                                PIPOI_DTO.PII_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PII_BCH_UnitPrice);
                                                PIPOI_DTO.PII_BCH_Value = Convert.ToDouble(ItemBatch.PII_BCH_Value);
                                                PIPOI_DTO.Id = 25;
                                                PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
                                            }
                                        }
                                        else
                                        {
                                            PIPOI_DTO.PII_Number = Convert.ToInt64(Item.PII_Number);
                                            PIPOI_DTO.Id = 106;
                                            D = PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);

                                            var ItemExpense = ItemExpense_DTO.Where(x => (x.PII_EXP_POI_Number == Item.PII_POI_Number && x.PII_EXP_POH_Number == Item.PII_POH_Number));
                                            foreach (var ItemExp in ItemExpense)
                                            {
                                                PIPOI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                                PIPOI_DTO.PII_Number = Convert.ToInt64(Item.PII_Number);
                                                PIPOI_DTO.EXP_Expense_Number = Convert.ToInt64(ItemExp.PII_EXP_Expense_Number);
                                                PIPOI_DTO.EXP_Remarks = ItemExp.PII_EXP_Remarks;
                                                PIPOI_DTO.EXP_Occurrence_Number = Convert.ToInt64(ItemExp.PII_EXP_Occurrence_Number);
                                                PIPOI_DTO.EXP_CM_Number = Convert.ToInt64(ItemExp.PII_EXP_CM_Number);
                                                PIPOI_DTO.EXP_ExpenseBase = Convert.ToDouble(ItemExp.PII_EXP_ExpenseBase);
                                                PIPOI_DTO.EXP_ExpenseValue = Convert.ToDouble(ItemExp.PII_EXP_ExpenseValue);
                                                PIPOI_DTO.EXP_Allocate_Number = Convert.ToInt64(ItemExp.PII_EXP_Allocate_Number);
                                                if (ItemExp.PII_EXP_Number == 0)
                                                {
                                                    PIPOI_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_POI_EXP_Number);
                                                    PIPOI_DTO.Id = 24;
                                                }
                                                else
                                                {
                                                    PIPOI_DTO.EXP_Number = Convert.ToInt64(ItemExp.PII_EXP_Number);
                                                    PIPOI_DTO.Id = 108;
                                                }
                                                PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
                                            }

                                            var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_POI_Number == Item.PII_POI_Number && x.PII_BCH_POH_Number == Item.PII_POH_Number));
                                            //var Batch = ItemBatch_DTO.Where(x => (x.PII_BCH_PII_Number == Item.PII_Number));
                                            foreach (var ItemBatch in Batch)
                                            {
                                                PIPOI_DTO.PIH_Number = Convert.ToInt64(PI_No);
                                                PIPOI_DTO.PII_BCH_Date = Convert.ToString(Convert.ToDateTime(ItemBatch.PII_BCH_Date).ToString("yyyyMMdd"));
                                                PIPOI_DTO.PII_BCH_No = Convert.ToString(ItemBatch.PII_BCH_No);
                                                PIPOI_DTO.PII_BCH_Qty = Convert.ToDouble(ItemBatch.PII_BCH_Qty);
                                                PIPOI_DTO.PII_BCH_UnitPrice = Convert.ToDouble(ItemBatch.PII_BCH_UnitPrice);
                                                PIPOI_DTO.PII_BCH_Value = Convert.ToDouble(ItemBatch.PII_BCH_Value);
                                                if (ItemBatch.PII_BCH_Number == 0)
                                                {
                                                    PIPOI_DTO.Id = 25;
                                                }
                                                else
                                                {
                                                    PIPOI_DTO.PII_BCH_Number = Convert.ToInt64(ItemBatch.PII_BCH_Number);
                                                    PIPOI_DTO.Id = 109;
                                                }
                                                PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
                                            }
                                        }

                                    }
                                    foreach (var Exp in Expense_DTO)
                                    {
                                        PIPOI_DTO.EXP_Expense_Number = Convert.ToInt64(Exp.PIH_EXP_Expense_Number);
                                        PIPOI_DTO.EXP_Remarks = Exp.PIH_EXP_Remarks;
                                        PIPOI_DTO.EXP_Occurrence_Number = Convert.ToInt64(Exp.PIH_EXP_Occurrence_Number);
                                        PIPOI_DTO.EXP_CM_Number = Convert.ToInt64(Exp.PIH_EXP_CM_Number);
                                        PIPOI_DTO.EXP_ExpenseBase = Convert.ToDouble(Exp.PIH_EXP_ExpenseBase);
                                        PIPOI_DTO.EXP_ExpenseValue = Convert.ToDouble(Exp.PIH_EXP_ExpenseValue);
                                        PIPOI_DTO.EXP_Allocate_Number = Convert.ToInt64(Exp.PIH_EXP_Allocate_Number);
                                        PIPOI_DTO.EXP_SAC_Number = Convert.ToInt64(Exp.PIH_EXP_SAC_Number);
                                        PIPOI_DTO.EXP_TaxCalculate = Convert.ToInt64(Exp.PIH_EXP_TaxCalculate);
                                        PIPOI_DTO.EXP_TaxValue = Convert.ToDouble(Exp.PIH_EXP_TaxValue);
                                        if (Exp.PIH_EXP_Number == 0)
                                        {
                                            PIPOI_DTO.EXP_Number = Convert.ToInt64(Exp.PIH_EXP_POH_EXP_Number);
                                            PIPOI_DTO.Id = 23;
                                        }
                                        else
                                        {
                                            PIPOI_DTO.EXP_Number = Convert.ToInt64(Exp.PIH_EXP_Number);
                                            PIPOI_DTO.Id = 107;
                                        }
                                        PIPOI_DAO.POItemTOPurchaseInvoiceDB(PIPOI_DTO);
                                    }

                                    transaction.Complete();

                                    P_Head_DTO.Reset();
                                    Expense_DTO = null;
                                    Item_DTO = null;
                                    ItemExpense_DTO = null;
                                    PIH_DTO.Reset();
                                    Original_PIH_DTO = PurchaseClone(PIH_DTO);

                                    return RedirectToAction("PurchaseInvoiceRegisterSummary");
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
            else if (Mode == "Direct")
            {
                PurchaseInvoiceHead_DTO PHPO_DTO = new PurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("PurchaseInvoiceCreate");
            }
            else if (Mode == "POITEMTO")
            {
                POItemTOPurchaseInvoiceHead_DTO PHPO_DTO = new POItemTOPurchaseInvoiceHead_DTO();

                PHPO_DTO.PIH_InvoiceNo = PIH_DTO.PIH_InvoiceNo;
                PHPO_DTO.PIH_InvoiceDate = PIH_DTO.PIH_InvoiceDate;
                PHPO_DTO.PIH_DueDate = PIH_DTO.PIH_DueDate;
                PHPO_DTO.PIH_SupplierInvoiceNo = PIH_DTO.PIH_SupplierInvoiceNo;
                PHPO_DTO.PIH_SupplierInvoiceDate = PIH_DTO.PIH_SupplierInvoiceDate;
                PHPO_DTO.PIH_Vendor_Number = Convert.ToString(PIH_DTO.PIH_Vendor_Number);
                PHPO_DTO.PIH_VendorLocation = Convert.ToString(PIH_DTO.PIH_VendorLocation);
                PHPO_DTO.PIH_CreditDays = Convert.ToString(PIH_DTO.PIH_CreditDays);
                PHPO_DTO.PIH_PaymentBase = Convert.ToString(PIH_DTO.PIH_PaymentBase);
                PHPO_DTO.PIH_Vendor = Convert.ToString(PIH_DTO.PIH_Vendor);
                PHPO_DTO.PIH_ImportOrder = PIH_DTO.PIH_ImportOrder.ToString();
                PHPO_DTO.PIH_Currency_Number = Convert.ToString(PIH_DTO.PIH_Currency_Number);
                PHPO_DTO.PIH_MS_Number = Convert.ToString(PIH_DTO.PIH_MS_Number);
                PHPO_DTO.PIH_ExchangeRate = Convert.ToString(PIH_DTO.PIH_ExchangeRate);
                PHPO_DTO.PIH_TaxCluster_Number = Convert.ToString(PIH_DTO.PIH_TaxCluster_Number);
                PHPO_DTO.PIH_WHT_Number = Convert.ToString(PIH_DTO.PIH_WHT_Number);
                PHPO_DTO.PIH_WHT_Tax = Convert.ToString(PIH_DTO.PIH_WHT_Tax);
                PHPO_DTO.PIH_WHT_Percent = Convert.ToString(PIH_DTO.PIH_WHT_Percent);
                PHPO_DTO.PIH_Currency = Convert.ToString(PIH_DTO.PIH_Currency);
                PHPO_DTO.PIH_DecimalPlaces = Convert.ToString(PIH_DTO.PIH_DecimalPlaces);

                TempData["PHPO_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                return RedirectToAction("POItemToPurchaseInvoiceCreate");
            }


            POItemTOPurchaseInvoiceGetData();
            return View(Original_PIH_DTO);

        }
        String PurchasePOItemInvoiceData(String PI_No)
        {
            PI_DTO.PIH_Number = Convert.ToInt64(PI_No);
            PI_DTO.Id = 61;
            PI_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PI_DAO.PurchaseInvoiceDB(PI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                PIHPOI_DTO = P_DL.POItemTOPIHeadList(DS.Tables[0]).FirstOrDefault();
                PIHPOI_DTO.InvoiceItems = P_DL.POItemTOPIItemList(DS.Tables[1]);
                PIHPOI_DTO.Expenses = P_DL.POItemTOPIExpenseEditList(DS.Tables[2]);
                PIHPOI_DTO.ItemExpenses = P_DL.POItemTOPIIExpenseEditList(DS.Tables[3]);
                PIHPOI_DTO.ItemBatch = P_DL.POItemTOPIIBatchEditList(DS.Tables[4]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["PIH_Mode"].ToString();

                if (DS.Tables[0].Rows[0]["PIH_Mode"].ToString() == "3")
                {

                }
                else
                {
                    return "0";
                }
                return "1";
            }
            else
            {
                return "0";
            }
        }









        //Purchase Invoice Numbering
        [Route("procurement/setup/purchase-invoice-numbering")]
        public IActionResult PINumbering()
        {

            GetPINumber();
            return View(PIN_DTO);

        }
        [Route("procurement/setup/purchase-invoice-numbering")]
        [HttpPost]
        public IActionResult PINumbering(PINumber_DTO PN_DTO)
        {

            bool IsValid = false;
            PINumber_DTO P_Head_DTO = new PINumber_DTO();

            List<PINumberReset_DTO>? Reset_DTO = new List<PINumberReset_DTO>();
            List<PINumberPrefix_DTO>? Prefix_DTO = new List<PINumberPrefix_DTO>();
            List<PINumberSuffix_DTO>? Suffix_DTO = new List<PINumberSuffix_DTO>();

            P_Head_DTO = PIN_DTO;

            if (PN_DTO.PINumberReset != null)
                Reset_DTO = PN_DTO.PINumberReset!.Where(K => !K.PIR_IsDeleted).ToList();

            if (PN_DTO.PINumberPrefix != null)
                Prefix_DTO = PN_DTO.PINumberPrefix!.Where(K => !K.PIP_IsDeleted).ToList();

            if (PN_DTO.PINumberSuffix != null)
                Suffix_DTO = PN_DTO.PINumberSuffix!.Where(K => !K.PIS_IsDeleted).ToList();

            if (PN_DTO.PIN_Method == "2")
            {
                String ResetDTO = string.Join(", ", Reset_DTO.Where(x => Convert.ToInt64(x.PIR_Number) != 0).Select(x => x.PIR_Number));
                String PrefixDTO = string.Join(", ", Prefix_DTO.Where(x => Convert.ToInt64(x.PIP_Number) != 0).Select(x => x.PIP_Number));
                String SuffixDTO = string.Join(", ", Suffix_DTO.Where(x => Convert.ToInt64(x.PIS_Number) != 0).Select(x => x.PIS_Number));

                PIN_DTO.CreatorCode = Convert.ToInt32(UserCode);
                PIN_DTO.DeleteNumbers = Convert.ToString(ResetDTO);
                PIN_DTO.Id = 31;
                PIN_DAO.PINumberDB(PIN_DTO);

                PIN_DTO.DeleteNumbers = Convert.ToString(PrefixDTO);
                PIN_DTO.Id = 32;
                PIN_DAO.PINumberDB(PIN_DTO);

                PIN_DTO.DeleteNumbers = Convert.ToString(SuffixDTO);
                PIN_DTO.Id = 33;
                PIN_DAO.PINumberDB(PIN_DTO);


                PIN_DTO.PIN_Method = PN_DTO.PIN_Method;
                if (PN_DTO.PIN_Number == 0)
                {
                    PIN_DTO.Id = 11;
                }
                else
                {
                    PIN_DTO.Id = 41;
                    PIN_DTO.PIN_Number = PN_DTO.PIN_Number;
                }
                PIN_DAO.PINumberDB(PIN_DTO);

                foreach (var Reset in Reset_DTO)
                {
                    PIN_DTO.PIN_Date = Convert.ToString(Convert.ToDateTime(Reset.PIR_Date).ToString("yyyyMMdd"));
                    PIN_DTO.PIN_StartingNumber = Convert.ToInt32(Reset.PIR_StartingNumber).ToString();
                    PIN_DTO.PIN_NumberofDigits = Convert.ToInt32(Reset.PIR_NumberofDigits).ToString();
                    PIN_DTO.PIN_PrefilZero = Convert.ToInt64(Reset.PIR_PrefilZero).ToString();
                    PIN_DTO.PIN_Frequency = Convert.ToInt64(Reset.PIR_Frequency).ToString();
                    if (Reset.PIR_Number == 0)
                    {
                        PIN_DTO.Id = 12;
                    }
                    else
                    {
                        PIN_DTO.Id = 42;
                        PIN_DTO.PIN_Number = Reset.PIR_Number;
                    }
                    PIN_DAO.PINumberDB(PIN_DTO);
                }

                foreach (var Prefix in Prefix_DTO)
                {
                    PIN_DTO.PIN_Date = Convert.ToString(Convert.ToDateTime(Prefix.PIP_Date).ToString("yyyyMMdd"));
                    PIN_DTO.PIN_Particulars = Convert.ToString(Prefix.PIP_Particulars);
                    if (Prefix.PIP_Number == 0)
                    {
                        PIN_DTO.Id = 13;
                    }
                    else
                    {
                        PIN_DTO.Id = 43;
                        PIN_DTO.PIN_Number = Prefix.PIP_Number;
                    }
                    PIN_DAO.PINumberDB(PIN_DTO);
                }

                foreach (var Suffix in Suffix_DTO)
                {
                    PIN_DTO.PIN_Date = Convert.ToString(Convert.ToDateTime(Suffix.PIS_Date).ToString("yyyyMMdd"));
                    PIN_DTO.PIN_Particulars = Convert.ToString(Suffix.PIS_Particulars);
                    if (Suffix.PIS_Number == 0)
                    {
                        PIN_DTO.Id = 14;
                    }
                    else
                    {
                        PIN_DTO.Id = 44;
                        PIN_DTO.PIN_Number = Suffix.PIS_Number;
                    }
                    PIN_DAO.PINumberDB(PIN_DTO);
                }
                PIN_DTO.Reset();
                Reset_DTO = null;
                Prefix_DTO = null;
                Suffix_DTO = null;
                ModelState.Clear();
            }
            else if (PN_DTO.PIN_Method == "3")
            {
                PIN_DTO.PIN_Method = PN_DTO.PIN_Method;
                if (PN_DTO.PIN_Number == 0)
                {
                    PIN_DTO.Id = 21;
                }
                else
                {
                    PIN_DTO.Id = 22;
                    PIN_DTO.PIN_Number = PN_DTO.PIN_Number;
                }
                PIN_DAO.PINumberDB(PIN_DTO);
            }

            GetPINumber();
            return View(PIN_DTO);

        }
        void GetPINumber()
        {
            PIN_DTO.CreatorCode = Convert.ToInt32(UserCode);
            PIN_DTO.Id = 1;
            DS = PIN_DAO.PINumberDB(PIN_DTO);

            ViewBag.Method = Help.GetCat(DS.Tables[0]);
            ViewBag.Frequency = Help.GetCat(DS.Tables[1]);
            ViewBag.Prefil = Help.GetCat(DS.Tables[2]);

            if (DS.Tables[3].Rows.Count > 0)
            {
                PIN_DTO.PIN_Number = Convert.ToInt64(DS.Tables[3].Rows[0]["PIN_Number"]);
                PIN_DTO.PIN_Method = DS.Tables[3].Rows[0]["PIN_Method"].ToString();
            }

            PIN_DTO.PINumberReset = PIN_DL.PIRList(DS.Tables[4]);
            PIN_DTO.PINumberPrefix = PIN_DL.PIPList(DS.Tables[5]);
            PIN_DTO.PINumberSuffix = PIN_DL.PISList(DS.Tables[6]);
        }
    }
}
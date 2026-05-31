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
using System.Data;
using System.Globalization;
using System.Text;
using System.Transactions;

namespace ERP.Controllers.sale
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class SaleInvoiceController : Controller
    {
        CultureInfo India = new CultureInfo("hi-IN");
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();

        //SI
        SaleInvoice_DL S_DL = new SaleInvoice_DL();
        SaleInvoiceHead_DTO SIH_DTO = new SaleInvoiceHead_DTO();
        SaleInvoice_DAO SI_DAO = new SaleInvoice_DAO();
        SaleInvoice_DTO SI_DTO = new SaleInvoice_DTO();
        SaleInvoiceRegister_DTO SIR_DTO = new SaleInvoiceRegister_DTO();
        List<SaleInvoiceRegister_DTO> SIR_List = new List<SaleInvoiceRegister_DTO>();


        //SO To SI
        SOToSaleInvoiceHead_DTO SOSIH_DTO = new SOToSaleInvoiceHead_DTO();
        SOToSaleInvoice_DAO SOSI_DAO = new SOToSaleInvoice_DAO();
        SOToSaleInvoice_DTO SOSI_DTO = new SOToSaleInvoice_DTO();


        //SO Item To SI
        SOItemToSaleInvoiceHead_DTO SOISIH_DTO = new SOItemToSaleInvoiceHead_DTO();
        SOItemToSaleInvoice_DAO SOISI_DAO = new SOItemToSaleInvoice_DAO();
        SOItemToSaleInvoice_DTO SOISI_DTO = new SOItemToSaleInvoice_DTO();


        //SI NUMBERING
        SINumber_DTO SIN_DTO = new SINumber_DTO();
        SINumber_DAO SIN_DAO = new SINumber_DAO();
        SINumbering_DL SIN_DL = new SINumbering_DL();
        List<SINumberPrefix_DTO> SIP_List = new List<SINumberPrefix_DTO>();
        List<SINumberSuffix_DTO> SIS_List = new List<SINumberSuffix_DTO>();


        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;



        //Sale Order Create
        [Route("sale/transactions/sale-invoice/create")]
        public IActionResult CreateSaleInvoice()
        {
            SITempClear("11");
            SaleInvoiceHead_DTO SH_DTO = new SaleInvoiceHead_DTO();
            if (TempData["SH_DTO_Json"] is string SHto)
            {
                SH_DTO = System.Text.Json.JsonSerializer.Deserialize<SaleInvoiceHead_DTO>(SHto);
            }
            SH_DTO.SIH_InvoiceDate = DateTime.Now.ToString("dd-MMM-yy");
            SH_DTO.SIH_InvoiceNo = OnSaleNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            SaleGetData();
            return View(SH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-invoice/create")]
        public IActionResult CreateSaleInvoice(SaleInvoiceHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SaleInvoiceHead_DTO S_Head_DTO = new SaleInvoiceHead_DTO();

            List<SaleInvoiceItem_DTO>? ITM_DTO = new List<SaleInvoiceItem_DTO>();
            List<SaleInvoiceIncome_DTO>? Income_DTO = new List<SaleInvoiceIncome_DTO>();
            List<SaleInvoiceIIncome_DTO>? ItemIncome_DTO = new List<SaleInvoiceIIncome_DTO>();
            List<SaleInvoiceAddress_DTO>? BuyerAddress_DTO = new List<SaleInvoiceAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.InvoiceItem != null)
                ITM_DTO = S_DTO.InvoiceItem!.Where(K => K.SII_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SIH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SII_INC_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SIH_ADD_IsDeleted == 0).ToList();

            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SII_MS_Number) != Convert.ToInt64(S_DTO.SIH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SII_Qty) == 0 || Convert.ToDouble(x.SII_UnitPrice) == 0 || Convert.ToDouble(x.SII_Amount) == 0);

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
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SIH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SIH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.InvoiceItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        if (BatchValidation(ITM_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String SIHOrderNoOld = S_DTO.SIH_InvoiceNo;
                                    String SIHOrderNoNew = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd")));

                                    SI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd"));
                                    SI_DTO.SI_InvoiceNo = SIHOrderNoNew;
                                    SI_DTO.SI_BUY_Number = Convert.ToInt64(S_DTO.SIH_BUY_Number);
                                    SI_DTO.SI_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 1 : 0);
                                    SI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                    SI_DTO.SI_MS_Number = Convert.ToInt64(S_DTO.SIH_MS_Number);
                                    SI_DTO.SI_TCT_Number = Convert.ToInt64(S_DTO.SIH_TCT_Number);
                                    SI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                    SI_DTO.SI_WHT_Number = Convert.ToInt64(S_DTO.SIH_WHT_Number);
                                    SI_DTO.SI_ExchangeRate = Convert.ToDouble(S_DTO.SIH_ExchangeRate) == 0 ? "1" : S_DTO.SIH_ExchangeRate;
                                    SI_DTO.SI_MaterialCost = Convert.ToDouble(S_DTO.SIH_MaterialCost);
                                    SI_DTO.SI_ItemMiscIncome = Convert.ToDouble(S_DTO.SIH_ItemMiscIncome);
                                    SI_DTO.SI_HeaderMiscIncome = Convert.ToDouble(S_DTO.SIH_HeaderMiscIncome);
                                    SI_DTO.SI_GST_Amount = Convert.ToDouble(S_DTO.SIH_GST_Amount);
                                    SI_DTO.SI_InvoiceAmount = Convert.ToDouble(S_DTO.SIH_InvoiceAmount);
                                    SI_DTO.SI_WHT_Amount = Convert.ToDouble(S_DTO.SIH_WHT_Amount);
                                    SI_DTO.SI_RoundOff = Convert.ToDouble(S_DTO.SIH_RoundOff);
                                    SI_DTO.SI_BuyerReceivable = Convert.ToDouble(S_DTO.SIH_BuyerReceivable);
                                    SI_DTO.SI_Id = 21;
                                    DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                                    OnSaleNumberGen(Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        SI_DTO.SI_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in ITM_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                            SI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_WH_Number);
                                            SI_DTO.SI_UoM_Number = Convert.ToInt64(Item.SII_UoM_Number);
                                            SI_DTO.SI_Qty = Convert.ToDouble(Item.SII_Qty);
                                            SI_DTO.SI_UnitPrice = Convert.ToDouble(Item.SII_UnitPrice);
                                            SI_DTO.SI_Amount = Convert.ToDouble(Item.SII_Amount);
                                            SI_DTO.SI_IncomeValue = Convert.ToDouble(Item.SII_IncomeValue);
                                            SI_DTO.SI_HSN_Number = Convert.ToInt64(Item.SII_HSN_Number);
                                            SI_DTO.SI_GST_Amount = Convert.ToDouble(Item.SII_GST_Amount);
                                            SI_DTO.SI_WHT_Percent = Convert.ToDouble(Item.SII_WHT_Percent);
                                            SI_DTO.SI_WHT_Amount = Convert.ToDouble(Item.SII_WHT_Amount);
                                            SI_DTO.SI_Id = 22;
                                            D = SI_DAO.SaleInvoiceDB(SI_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (Convert.ToInt64(x.SII_INC_ITM_Number) == Item.SII_ITM_Number) && (x.SII_INC_ITM_Index == Item.SII_Index));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SI_DTO.SI_ITM_Number = Convert.ToInt64(ItemInc.SII_INC_ITM_Number);
                                                SI_DTO.SI_INC_MIC_Number = Convert.ToInt64(ItemInc.SII_INC_MIC_Number);
                                                SI_DTO.SI_INC_Remarks = ItemInc.SII_INC_Remarks;
                                                SI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(ItemInc.SII_INC_OCRN_Number);
                                                SI_DTO.SI_INC_CM_Number = Convert.ToInt64(ItemInc.SII_INC_CM_Number);
                                                SI_DTO.SI_INC_IncomeBase = Convert.ToDouble(ItemInc.SII_INC_IncomeBase);
                                                SI_DTO.SI_INC_IncomeValue = Convert.ToDouble(ItemInc.SII_INC_IncomeValue);
                                                SI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(ItemInc.SII_INC_ALCT_Number);
                                                SI_DTO.SI_INC_LA_Number = Convert.ToInt64(ItemInc.SII_INC_LA_Number);
                                                SI_DTO.SI_Id = 24;
                                                SI_DAO.SaleInvoiceDB(SI_DTO);
                                            }

                                            SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                            SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                            SI_DTO.SI_BCH_Mode = Convert.ToInt32(11);
                                            SI_DTO.SI_Id = 157;
                                            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
                                            if (DS.Tables[0].Rows.Count > 0)
                                            {
                                                DataTable dt = DS.Tables[0];
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    SI_DTO.SI_BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                                    SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                                    SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                                    SI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                    SI_DTO.SI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                                    SI_DTO.SI_BCH_No = Convert.ToString(row["BCH_No"]);
                                                    SI_DTO.SI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                                    SI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                                    SI_DTO.SI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                                    SI_DTO.SI_Id = 25;
                                                    SI_DAO.SaleInvoiceDB(SI_DTO);
                                                }
                                            }
                                        }
                                        foreach (var Income in Income_DTO)
                                        {
                                            SI_DTO.SI_INC_MIC_Number = Convert.ToInt64(Income.SIH_INC_MIC_Number);
                                            SI_DTO.SI_INC_Remarks = Income.SIH_INC_Remarks;
                                            SI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(Income.SIH_INC_OCRN_Number);
                                            SI_DTO.SI_INC_CM_Number = Convert.ToInt64(Income.SIH_INC_CM_Number);
                                            SI_DTO.SI_INC_IncomeBase = Convert.ToDouble(Income.SIH_INC_IncomeBase);
                                            SI_DTO.SI_INC_IncomeValue = Convert.ToDouble(Income.SIH_INC_IncomeValue);
                                            SI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(Income.SIH_INC_ALCT_Number);
                                            SI_DTO.SI_INC_LA_Number = Convert.ToInt64(Income.SIH_INC_LA_Number);
                                            SI_DTO.SI_INC_CalculateGST = Convert.ToInt64(Income.SIH_INC_CalculateGST);
                                            SI_DTO.SI_INC_GST_Amount = Convert.ToDouble(Income.SIH_INC_GST_Amount);
                                            SI_DTO.SI_INC_SAC_Number = Convert.ToInt64(Income.SIH_INC_SAC_Number);
                                            SI_DTO.SI_INC_WHT_Percent = Convert.ToDouble(Income.SIH_INC_WHT_Percent);
                                            SI_DTO.SI_INC_WHT_Amount = Convert.ToDouble(Income.SIH_INC_WHT_Amount);
                                            SI_DTO.SI_Id = 23;
                                            SI_DAO.SaleInvoiceDB(SI_DTO);
                                        }
                                        foreach (var BuyerAddress in BuyerAddress_DTO)
                                        {
                                            SI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SIH_ADD_ADTP_Number);
                                            SI_DTO.SI_ADD_AddressID = Convert.ToString(BuyerAddress.SIH_ADD_AddressID);
                                            SI_DTO.SI_ADD_Address = Convert.ToString(BuyerAddress.SIH_ADD_Address);
                                            SI_DTO.SI_ADD_City = Convert.ToString(BuyerAddress.SIH_ADD_City);
                                            SI_DTO.SI_ADD_State = Convert.ToString(BuyerAddress.SIH_ADD_State);
                                            SI_DTO.SI_ADD_Country = Convert.ToString(BuyerAddress.SIH_ADD_Country);
                                            SI_DTO.SI_ADD_Pin = Convert.ToString(BuyerAddress.SIH_ADD_Pin);
                                            SI_DTO.SI_ADD_GSTIN = Convert.ToString(BuyerAddress.SIH_ADD_GSTIN);
                                            SI_DTO.SI_Id = 26;
                                            SI_DAO.SaleInvoiceDB(SI_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    if (SIHOrderNoOld != SIHOrderNoNew)
                                    {
                                        ViewBag.ErrorCode = 2;
                                        ViewBag.ErrorMessage = "Sale Invoice number " + SIHOrderNoOld + " used by another user. Next number will be allotted to you.";
                                    }
                                    return RedirectToAction("CreateSaleInvoice");
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
            else if (Mode == "SOTOITEM")
            {
                SOToSaleInvoiceHead_DTO SHSO_DTO = new SOToSaleInvoiceHead_DTO();

                SHSO_DTO.SIH_InvoiceNo = S_DTO.SIH_InvoiceNo;
                SHSO_DTO.SIH_InvoiceDate = S_DTO.SIH_InvoiceDate;
                SHSO_DTO.SIH_BUY_Number = Convert.ToString(S_DTO.SIH_BUY_Number);
                SHSO_DTO.SIH_BUY_LOC_Number = Convert.ToString(S_DTO.SIH_BUY_LOC_Number);
                SHSO_DTO.SIH_ExportOrder = Convert.ToString(S_DTO.SIH_ExportOrder);
                SHSO_DTO.SIH_CUR_Number = Convert.ToString(S_DTO.SIH_CUR_Number);
                SHSO_DTO.SIH_MS_Number = Convert.ToString(S_DTO.SIH_MS_Number);
                SHSO_DTO.SIH_ExchangeRate = Convert.ToString(S_DTO.SIH_ExchangeRate);
                SHSO_DTO.SIH_TCT_Number = Convert.ToString(S_DTO.SIH_TCT_Number);
                SHSO_DTO.SIH_WHT_Number = Convert.ToString(S_DTO.SIH_WHT_Number);
                SHSO_DTO.SIH_WHT_Tax = Convert.ToString(S_DTO.SIH_WHT_Tax);
                SHSO_DTO.SIH_WHT_Percent = Convert.ToString(S_DTO.SIH_WHT_Percent);
                SHSO_DTO.SIH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SIH_CUR_DecimalPlaces);

                TempData["SHSO_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSO_DTO);

                return RedirectToAction("SOToCreateSaleInvoice");
            }
            else if (Mode == "SOITEMTO")
            {
                SOToSaleInvoiceHead_DTO SHSO_DTO = new SOToSaleInvoiceHead_DTO();

                SHSO_DTO.SIH_InvoiceNo = S_DTO.SIH_InvoiceNo;
                SHSO_DTO.SIH_InvoiceDate = S_DTO.SIH_InvoiceDate;
                SHSO_DTO.SIH_BUY_Number = Convert.ToString(S_DTO.SIH_BUY_Number);
                SHSO_DTO.SIH_BUY_LOC_Number = Convert.ToString(S_DTO.SIH_BUY_LOC_Number);
                SHSO_DTO.SIH_ExportOrder = Convert.ToString(S_DTO.SIH_ExportOrder);
                SHSO_DTO.SIH_CUR_Number = Convert.ToString(S_DTO.SIH_CUR_Number);
                SHSO_DTO.SIH_MS_Number = Convert.ToString(S_DTO.SIH_MS_Number);
                SHSO_DTO.SIH_ExchangeRate = Convert.ToString(S_DTO.SIH_ExchangeRate);
                SHSO_DTO.SIH_TCT_Number = Convert.ToString(S_DTO.SIH_TCT_Number);
                SHSO_DTO.SIH_WHT_Number = Convert.ToString(S_DTO.SIH_WHT_Number);
                SHSO_DTO.SIH_WHT_Tax = Convert.ToString(S_DTO.SIH_WHT_Tax);
                SHSO_DTO.SIH_WHT_Percent = Convert.ToString(S_DTO.SIH_WHT_Percent);
                SHSO_DTO.SIH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SIH_CUR_DecimalPlaces);

                TempData["SHSO_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SHSO_DTO);

                return RedirectToAction("SOItemToCreateSaleInvoice");
            }
            SaleGetData();
            return View(Original_DTO);
        }

        [Route("sale/transactions/sale-invoice/item")]
        public IActionResult SaleItem(String? ItemCode, String MS)
        {
            if (ItemCode == null)
            {
                ItemCode = "";
            }
            if (MS == null)
            {
                MS = "";
            }
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_ITM_Code = Convert.ToString(ItemCode).Trim();
            SI_DTO.SI_MS_Number = Convert.ToInt64(MS.Trim());
            SI_DTO.SI_Id = 2;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            var Item = S_DL.ItemList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("sale/transactions/sale-invoice/uom")]
        public String SaleUoM(String? UoM)
        {
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_UoM_Number = Convert.ToInt64(UoM);
            SI_DTO.SI_Id = 4;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("sale/transactions/sale-invoice/income/des")]
        public IActionResult SaleExpensiveDes(String? Title)
        {
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_INC_MIC_Number = Convert.ToInt64(Title);
            SI_DTO.SI_Id = 3;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            var Expensive = S_DL.IncomeList(DS.Tables[0]).FirstOrDefault();
            return Json(Expensive);
        }

        [Route("sale/transactions/sale-invoice/buyer")]
        public IActionResult SaleBuyer(String? Buyer, String? Export, String? SIHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            if (Export == null)
            {
                Export = "";
            }
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_ExportOrder = Convert.ToInt16(Convert.ToBoolean(Export) == true ? 2 : 1);
            SI_DTO.SI_ITM_Code = Convert.ToString(Buyer).Trim();
            SI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SI_DTO.SI_Id = 5;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            var Ven = S_DL.BuyerList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("sale/transactions/sale-invoice/gst")]
        public String SaleInvoiceGst(String? Cluster, String? SIHDate, String? HSN, String? BaseAmount)
        {
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SI_DTO.SI_HSN_Number = Convert.ToInt64(HSN);
            SI_DTO.SI_Id = 6;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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

        [Route("sale/transactions/sale-invoice/gst/view")]
        public IActionResult SaleInvoiceGstView(String? Cluster, String? SIHDate, String? HSN, String? BaseAmount)
        {
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SI_DTO.SI_HSN_Number = Convert.ToInt64(HSN);
            SI_DTO.SI_Id = 9;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleInvoiceGst> PurGST = new List<SaleInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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
                        new SaleInvoiceGst
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

        [Route("sale/transactions/sale-invoice/income/gst")]
        public String SaleInvoiceHeaderGst(String? Cluster, String? SIHDate, String? SAC, String? BaseAmount)
        {
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SI_DTO.SI_INC_SAC_Number = Convert.ToInt64(SAC);
            SI_DTO.SI_Id = 11;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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

        [Route("sale/transactions/sale-invoice/income/gst/view")]
        public IActionResult SaleInvoiceGstHeaderView(String? Cluster, String? SIHDate, String? SAC, String? BaseAmount)
        {
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SI_DTO.SI_INC_SAC_Number = Convert.ToInt64(SAC);
            SI_DTO.SI_Id = 12;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleInvoiceGst> PurGST = new List<SaleInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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
                        new SaleInvoiceGst
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

        [Route("sale/transactions/sale-invoice/wht")]
        public IActionResult SaleInvoiceWHT(String? Buyer, String? WHTNumber, String? SIHDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Buyer == null)
            {
                Buyer = "0";
            }
            if (SIHDate == null)
            {
                SIHDate = "0";
            }
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SI_DTO.SI_WHT_Number = Convert.ToInt64(WHTNumber);
            SI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SI_DTO.SI_Id = 7;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            var WHT = S_DL.SaleInvWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("sale/transactions/sale-invoice/cluster")]
        public IActionResult SaleInvoiceCluster(String? Buyer, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SI_DTO.SI_Search = Cluster;
            SI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SI_DTO.SI_Id = 8;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            var InvCluster = S_DL.SaleCluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("sale/transactions/sale-invoice/buyer/address")]
        public IActionResult SaleBuyerAddressID(String? Buyer, String ADTPNumber)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SI_DTO.SI_Id = 13;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            SaleInvoiceAddress SIA = new SaleInvoiceAddress();
            SIA.BuyerAddressId = S_DL.BuyerAddressID(DS.Tables[0]);
            SIA.BuyerAddress = S_DL.BuyerAddress(DS.Tables[1]).FirstOrDefault();
            return Json(SIA);
        }

        [Route("sale/transactions/sale-invoice/buyer/address/addressid")]
        public IActionResult SaleBuyerAddress(String? Buyer, String ADTPNumber, String AddressID)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SI_DTO.SI_ADD_AddressID = Convert.ToString(AddressID);
            SI_DTO.SI_Id = 14;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            var Address = S_DL.BuyerAddress(DS.Tables[0]).FirstOrDefault();
            return Json(Address);
        }

        [Route("sale/transactions/sale-invoice/batch/get")]
        public IActionResult SaleInvoiceBatchGet(String? ItemNumber, String? Index, String? Warehouse)
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

            SI_DTO.SI_ITM_Number = Convert.ToInt64(ItemNumber);
            SI_DTO.SI_WH_Number = Convert.ToInt64(Warehouse);
            SI_DTO.SI_BCH_Index = Convert.ToInt32(Index);
            SI_DTO.SI_BCH_Mode = Convert.ToInt32(11);
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SI_DTO.SI_Id = 151;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);

            SIBatchItem_DTO PRB = new SIBatchItem_DTO();
            PRB.SIBatch = S_DL.SIBatchList(DS.Tables[0]);
            PRB.SIView = S_DL.SIBatchOverallList(DS.Tables[1]);
            return Json(PRB);
        }

        [Route("sale/transactions/sale-invoice/batch/post")]
        [HttpPost]
        public IActionResult SaleInvoiceBatchGet([FromBody] SaleInvoiceItemBatch ItemBatch)
        {
            SI_DTO.SI_BCH_Mode = Convert.ToInt32(11);
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);

            foreach (var Item in ItemBatch.ItemBatch)
            {
                SI_DTO.SI_BCH_Number = Convert.ToInt64(Item.SII_BCH_Number);
                SI_DTO.SI_BCH_BCH_Number = Convert.ToInt64(Item.SII_BCH_BCH_Number);
                SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_BCH_ITM_Number);
                SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_BCH_ITM_Index);
                SI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_BCH_WH_Number);
                SI_DTO.SI_BCH_Date = Convert.ToString(Convert.ToDateTime(Item.SII_BCH_Date).ToString("yyyyMMdd"));
                SI_DTO.SI_BCH_No = Convert.ToString(Item.SII_BCH_No);
                SI_DTO.SI_BCH_Qty = Convert.ToDouble(Item.SII_BCH_Qty);
                SI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(Item.SII_BCH_UnitPrice);
                SI_DTO.SI_BCH_Value = Convert.ToDouble(Item.SII_BCH_Value);
                SI_DTO.SI_Id = 153;
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (Item.SII_BCH_Qty > 0)
                    {
                        SI_DTO.SI_Id = 154;
                        SI_DAO.SaleInvoiceDB(SI_DTO);
                    }
                    else
                    {
                        SI_DTO.SI_Id = 155;
                        SI_DAO.SaleInvoiceDB(SI_DTO);
                    }
                }
                else
                {
                    if (Item.SII_BCH_Qty > 0)
                    {
                        SI_DTO.SI_Id = 152;
                        SI_DAO.SaleInvoiceDB(SI_DTO);
                    }
                }
            }

            PRBatchItem_DTO PRB = new PRBatchItem_DTO();
            return Json(PRB);
        }

        Boolean BatchValidation(List<SaleInvoiceItem_DTO> Item_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                Double BatchAmount = 0;

                SI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_WH_Number);
                SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                SI_DTO.SI_BCH_Mode = Convert.ToInt32(11);
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                SI_DTO.SI_Id = 156;
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                Double Qty = Convert.ToDouble(Item.SII_Qty);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    BatchQty = Convert.ToDouble(DS.Tables[0].Rows[0][0].ToString());
                }

                if (Qty < 0)
                {
                    Qty = Qty * -1;
                }

                if (BatchQty == Qty) { }
                else
                {
                    Message += Item.SII_ITM_Code + " Batch Qty  Mismatched <br/>";
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

        void SITempClear(String Mode)
        {
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SI_DTO.SI_BCH_Mode = Convert.ToInt32(Mode);
            SI_DTO.SI_Id = 161;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
        }



        void SaleGetData()
        {
            SI_DTO.SI_InvoiceDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            SI_DTO.SI_Id = 1;
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);

            ViewBag.IncomeCode = Help.GetCat(DS.Tables[0]);
            ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            ViewBag.Currency = Help.GetCat(DS.Tables[4]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[5]);
            ViewBag.UoM = Help.GetCat(DS.Tables[6]);
            ViewBag.LedgerAccount = Help.GetCat(DS.Tables[7]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[8]);
            ViewBag.HSN = Help.GetCat(DS.Tables[9]);
            ViewBag.WHTax = Help.GetCat(DS.Tables[10]);
            ViewBag.IsCalculate = Help.GetCat(DS.Tables[11]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[12]);
        }
        void OnSaleNumberGen(Int32 SIDate)
        {
            DataSet DS1 = new DataSet();
            SIN_DTO.SIN_Date = SIDate.ToString();
            SIN_DTO.SIN_Id = 101;
            DS1 = SIN_DAO.SINumberDB(SIN_DTO);
            if (DS1.Tables[0].Rows.Count > 0)
            {
                Int32 Order = Convert.ToInt32(DS1.Tables[0].Rows[0]["SIN_Method"].ToString());
                DateTime SIN_Date = Convert.ToDateTime(DateTime.ParseExact(SIDate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));

                DateTime StartDate = new DateTime();
                DateTime EndDate = new DateTime();

                if (Order == 2)
                {
                    if (DS1.Tables[1].Rows.Count > 0)
                    {
                        Int32 Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["SI_StartingNumber"].ToString());

                        SIN_DTO.SIN_Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["SI_Number"].ToString());
                        SIN_DTO.SIN_StartingNumber = Convert.ToString(Convert.ToInt32(Number + 1));
                        SIN_DTO.SIN_Id = 103;
                        SIN_DAO.SINumberDB(SIN_DTO);
                    }
                    else
                    {
                        Int32 Frequency = 0;
                        Int32 Start = 0;
                        DateTime Date = new DateTime();

                        if (DS1.Tables[2].Rows.Count > 0)
                        {
                            Date = Convert.ToDateTime(DS1.Tables[2].Rows[0]["SIR_Date"].ToString());
                            Start = Convert.ToInt32(DS1.Tables[2].Rows[0]["SIR_StartingNumber"].ToString());
                            Frequency = Convert.ToInt32(DS1.Tables[2].Rows[0]["SIR_Frequency"].ToString());
                        }

                        if (Frequency == 4)
                        {
                            if (Date.Month == SIN_Date.Month)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(SIN_Date.Year, SIN_Date.Month, 1);
                                EndDate = new DateTime(SIN_Date.Year, SIN_Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                        }
                        else if (Frequency == 5)
                        {
                            if (Date.Month == SIN_Date.Month && Date.Year == SIN_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else if (Date.Month != SIN_Date.Month && Date.Year == SIN_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(SIN_Date.Year, Date.Month, 1);
                                EndDate = new DateTime(SIN_Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                        }

                        SIN_DTO.SIN_Number = Convert.ToInt32(DS1.Tables[2].Rows[0]["SIR_Number"].ToString());
                        SIN_DTO.SIN_StartingNumber = Convert.ToString(Start);
                        SIN_DTO.SIN_Date = Convert.ToString(StartDate.ToString("yyyyMMdd"));
                        SIN_DTO.SIN_Method = Convert.ToString(EndDate.ToString("yyyyMMdd"));
                        SIN_DTO.SIN_Id = 102;
                        SIN_DAO.SINumberDB(SIN_DTO);
                    }
                }
            }
        }


        [Route("sale/transactions/sale-invoice/numbering")]
        public String OnSaleNumber(Int32 SIDate)
        {
            SI_DTO.SI_InvoiceDate = Convert.ToInt32(SIDate);
            SI_DTO.SI_Id = 0;
            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);

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
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["SIN_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["SIP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["SIS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0)
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["SI_StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["SIR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["SIR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["SIR_Frequency"].ToString());

                            Number = OrNum + 1;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                        }
                    }
                    else
                    {
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["SIR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["SIR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["SIR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["SIR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["SIR_Frequency"].ToString());

                            Number = RNumber;
                            if (RZero == 1)
                            {
                                Prefil = "D" + RDigit;
                            }
                        }
                    }
                    OrderNum = Prefix + "" + Number.ToString(Prefil) + "" + Surfix;

                    return OrderNum;
                }
            }
            else
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = "SI number Not assigned for Given date";
            }
            return "";
        }





        //Sale Preview
        [Route("sale/transactions/sale-invoice/{SI_No}/view")]
        public IActionResult PreviewSaleInvoice(String? SI_No)
        {
            String Active = GetSIPreview(SI_No);
            if (Active != "1")
            {
                return RedirectToAction("SaleInvoiceRegisterSummary");
            }
            ViewBag.SI_No = SI_No;

            SaleGetData();
            GetSIEditData(SI_No);
            return View(SIH_DTO);
        }

        [Route("sale/transactions/sale-invoice/{SI_No}/batch/view")]
        public IActionResult SaleInvoiceViewBatchGet(String? SII_Number)
        {
            if (SII_Number == null)
            {
                SII_Number = "0";
            }

            SI_DTO.SI_I_Number = Convert.ToInt64(SII_Number);
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SI_DTO.SI_Id = 171;
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);

            SIBatchItem_DTO PRB = new SIBatchItem_DTO();
            PRB.SIView = S_DL.SIBatchViewList(DS.Tables[0]);
            return Json(PRB);
        }

        [Route("sale/transactions/sale-invoice/{SI_No}/view")]
        [HttpPost]
        public IActionResult PreviewSaleInvoice(String Mode, String? SI_No)
        {
            if (Mode == "PDF" || Mode == "Ascii" || Mode == "Excel")
            {
                SI_DTO.SI_Number = Convert.ToInt64(SI_No);
                SI_DTO.SI_Id = 41;
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

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
                                        <td colspan='6'>Sale Invoice</td>
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
                                        <td colspan='3'>Sale Order Number</td>
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
                            var fileDownloadName = "Sale-order-download.xlsx";

                            return File(content, contentType, fileDownloadName);
                        }
                    }
                }
            }
            else if (Mode == "Delete")
            {
                SI_DTO.SI_Number = Convert.ToInt64(SI_No);
                SI_DTO.SI_Id = 31;
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);
                return RedirectToAction("SaleInvoiceRegisterSummary");
            }
            String Active = GetSIPreview(SI_No);
            if (Active != "1")
            {
                return RedirectToAction("SaleInvoiceRegisterSummary");
            }
            ViewBag.SI_No = SI_No;

            SaleGetData();
            GetSIEditData(SI_No);
            return View(SIH_DTO);
        }
        String GetSIPreview(String SI_No)
        {
            SI_DTO.SI_Number = Convert.ToInt64(SI_No);
            SI_DTO.SI_Id = 71;
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                ViewBag.HeadPreview = S_DL.SIHeadEditList(DS.Tables[0]).FirstOrDefault();
                ViewBag.ItemPreview = S_DL.SIItemEditList(DS.Tables[1]);
                ViewBag.BuyerPreview = S_DL.BuyerList(DS.Tables[2]).FirstOrDefault();
                return "1";
            }
            else
            {
                return "0";
            }
        }





        //Sale Edit
        [Route("sale/transactions/sale-invoice/{SI_No}/edit")]
        public IActionResult EditSaleInvoice(String? SI_No)
        {
            SITempClear("12");
            SaleGetData();
            String Active = GetSIEditData(SI_No);
            if (Active != "1")
            {
                return RedirectToAction("SaleInvoiceRegisterSummary");
            }
            ViewBag.SI_No = SI_No;
            return View(SIH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-invoice/{SI_No}/edit")]
        public IActionResult EditSaleInvoice(SaleInvoiceHead_DTO S_DTO, String? Mode, String? SI_No)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SaleInvoiceHead_DTO S_Head_DTO = new SaleInvoiceHead_DTO();

            List<SaleInvoiceItem_DTO>? ITM_DTO = new List<SaleInvoiceItem_DTO>();
            List<SaleInvoiceIncome_DTO>? Income_DTO = new List<SaleInvoiceIncome_DTO>();
            List<SaleInvoiceIIncome_DTO>? ItemIncome_DTO = new List<SaleInvoiceIIncome_DTO>();
            List<SaleInvoiceAddress_DTO>? BuyerAddress_DTO = new List<SaleInvoiceAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.InvoiceItem != null)
                ITM_DTO = S_DTO.InvoiceItem!.Where(K => K.SII_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SIH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SII_INC_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SIH_ADD_IsDeleted == 0).ToList();

            SI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SII_MS_Number) != Convert.ToInt64(S_DTO.SIH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SII_Qty) == 0 || Convert.ToDouble(x.SII_UnitPrice) == 0 || Convert.ToDouble(x.SII_Amount) == 0);

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
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SIH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SIH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.InvoiceItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        using (var transaction = new TransactionScope())
                        {
                            try
                            {
                                SI_DTO.SI_Number = Convert.ToInt64(S_DTO.SIH_Number);
                                SI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd"));
                                SI_DTO.SI_InvoiceNo = S_DTO.SIH_InvoiceNo;
                                SI_DTO.SI_BUY_Number = Convert.ToInt64(S_DTO.SIH_BUY_Number);
                                SI_DTO.SI_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 1 : 0);
                                SI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                SI_DTO.SI_MS_Number = Convert.ToInt64(S_DTO.SIH_MS_Number);
                                SI_DTO.SI_TCT_Number = Convert.ToInt64(S_DTO.SIH_TCT_Number);
                                SI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                SI_DTO.SI_WHT_Number = Convert.ToInt64(S_DTO.SIH_WHT_Number);
                                SI_DTO.SI_ExchangeRate = Convert.ToDouble(S_DTO.SIH_ExchangeRate) == 0 ? "1" : S_DTO.SIH_ExchangeRate;
                                SI_DTO.SI_MaterialCost = Convert.ToDouble(S_DTO.SIH_MaterialCost);
                                SI_DTO.SI_ItemMiscIncome = Convert.ToDouble(S_DTO.SIH_ItemMiscIncome);
                                SI_DTO.SI_HeaderMiscIncome = Convert.ToDouble(S_DTO.SIH_HeaderMiscIncome);
                                SI_DTO.SI_GST_Amount = Convert.ToDouble(S_DTO.SIH_GST_Amount);
                                SI_DTO.SI_InvoiceAmount = Convert.ToDouble(S_DTO.SIH_InvoiceAmount);
                                SI_DTO.SI_WHT_Amount = Convert.ToDouble(S_DTO.SIH_WHT_Amount);
                                SI_DTO.SI_RoundOff = Convert.ToDouble(S_DTO.SIH_RoundOff);
                                SI_DTO.SI_BuyerReceivable = Convert.ToDouble(S_DTO.SIH_BuyerReceivable);
                                SI_DTO.SI_Id = 121;
                                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                                String ItemDTO = string.Join(", ", ITM_DTO.Where(x => Convert.ToInt64(x.SII_Number) != 0).Select(x => x.SII_Number));
                                String ItemIncomeDTO = string.Join(", ", ItemIncome_DTO.Where(x => Convert.ToInt64(x.SII_INC_Number) != 0).Select(x => x.SII_INC_Number));
                                String IncomeDTO = string.Join(", ", Income_DTO.Where(x => Convert.ToInt64(x.SIH_INC_Number) != 0).Select(x => x.SIH_INC_Number));
                                String AddressDTO = string.Join(", ", BuyerAddress_DTO.Where(x => Convert.ToInt64(x.SIH_ADD_Number) != 0).Select(x => x.SIH_ADD_Number));

                                SI_DTO.SI_DeleteNumbers = Convert.ToString(ItemDTO);
                                SI_DTO.SI_Id = 101;
                                SI_DAO.SaleInvoiceDB(SI_DTO);

                                SI_DTO.SI_DeleteNumbers = Convert.ToString(IncomeDTO);
                                SI_DTO.SI_Id = 102;
                                SI_DAO.SaleInvoiceDB(SI_DTO);

                                SI_DTO.SI_DeleteNumbers = Convert.ToString(ItemIncomeDTO);
                                SI_DTO.SI_Id = 103;
                                SI_DAO.SaleInvoiceDB(SI_DTO);

                                SI_DTO.SI_DeleteNumbers = Convert.ToString(AddressDTO);
                                SI_DTO.SI_Id = 105;
                                SI_DAO.SaleInvoiceDB(SI_DTO);


                                foreach (var Item in ITM_DTO)
                                {
                                    DataSet D = new DataSet();
                                    SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                    SI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_WH_Number);
                                    SI_DTO.SI_UoM_Number = Convert.ToInt64(Item.SII_UoM_Number);
                                    SI_DTO.SI_Qty = Convert.ToDouble(Item.SII_Qty);
                                    SI_DTO.SI_UnitPrice = Convert.ToDouble(Item.SII_UnitPrice);
                                    SI_DTO.SI_Amount = Convert.ToDouble(Item.SII_Amount);
                                    SI_DTO.SI_IncomeValue = Convert.ToDouble(Item.SII_IncomeValue);
                                    SI_DTO.SI_HSN_Number = Convert.ToInt64(Item.SII_HSN_Number);
                                    SI_DTO.SI_GST_Amount = Convert.ToDouble(Item.SII_GST_Amount);
                                    SI_DTO.SI_WHT_Percent = Convert.ToDouble(Item.SII_WHT_Percent);
                                    SI_DTO.SI_WHT_Amount = Convert.ToDouble(Item.SII_WHT_Amount);
                                    if (Item.SII_Number == 0)
                                    {
                                        SI_DTO.SI_Id = 22;
                                        D = SI_DAO.SaleInvoiceDB(SI_DTO);

                                        var ItemIncome = ItemIncome_DTO.Where(x => (Convert.ToInt64(x.SII_INC_ITM_Number) == Item.SII_ITM_Number) && (x.SII_INC_ITM_Index == Item.SII_Index));

                                        foreach (var ItemInc in ItemIncome)
                                        {
                                            SI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                            SI_DTO.SI_ITM_Number = Convert.ToInt64(ItemInc.SII_INC_ITM_Number);
                                            SI_DTO.SI_INC_MIC_Number = Convert.ToInt64(ItemInc.SII_INC_MIC_Number);
                                            SI_DTO.SI_INC_Remarks = ItemInc.SII_INC_Remarks;
                                            SI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(ItemInc.SII_INC_OCRN_Number);
                                            SI_DTO.SI_INC_CM_Number = Convert.ToInt64(ItemInc.SII_INC_CM_Number);
                                            SI_DTO.SI_INC_IncomeBase = Convert.ToDouble(ItemInc.SII_INC_IncomeBase);
                                            SI_DTO.SI_INC_IncomeValue = Convert.ToDouble(ItemInc.SII_INC_IncomeValue);
                                            SI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(ItemInc.SII_INC_ALCT_Number);
                                            SI_DTO.SI_INC_LA_Number = Convert.ToInt64(ItemInc.SII_INC_LA_Number);
                                            if (ItemInc.SII_INC_Number == 0)
                                            {
                                                SI_DTO.SI_Id = 24;
                                            }
                                            else
                                            {
                                                SI_DTO.SI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_Number);
                                                SI_DTO.SI_Id = 124;
                                            }
                                            SI_DAO.SaleInvoiceDB(SI_DTO);
                                        }

                                        //SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                        //SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                        //SI_DTO.SI_BCH_Mode = Convert.ToInt32(11);
                                        //SI_DTO.SI_Id = 157;
                                        //DS = SI_DAO.SaleInvoiceDB(SI_DTO);
                                        //if (DS.Tables[0].Rows.Count > 0)
                                        //{
                                        //    DataTable dt = DS.Tables[0];
                                        //    foreach (DataRow row in dt.Rows)
                                        //    {
                                        //        SI_DTO.SI_BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                        //        SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                        //        SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                        //        SI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                        //        SI_DTO.SI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                        //        SI_DTO.SI_BCH_No = Convert.ToString(row["BCH_No"]);
                                        //        SI_DTO.SI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                        //        SI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                        //        SI_DTO.SI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                        //        SI_DTO.SI_Id = 25;
                                        //        SI_DAO.SaleInvoiceDB(SI_DTO);
                                        //    }
                                        //}
                                    }
                                    else
                                    {
                                        SI_DTO.SI_I_Number = Convert.ToInt64(Item.SII_Number);
                                        SI_DTO.SI_Id = 122;
                                        D = SI_DAO.SaleInvoiceDB(SI_DTO);

                                        var ItemIncome = ItemIncome_DTO.Where(x => (Convert.ToInt64(x.SII_INC_SII_Number) == Item.SII_Number));

                                        foreach (var ItemInc in ItemIncome)
                                        {
                                            SI_DTO.SI_ITM_Number = Convert.ToInt64(ItemInc.SII_INC_ITM_Number);
                                            SI_DTO.SI_INC_MIC_Number = Convert.ToInt64(ItemInc.SII_INC_MIC_Number);
                                            SI_DTO.SI_INC_Remarks = ItemInc.SII_INC_Remarks;
                                            SI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(ItemInc.SII_INC_OCRN_Number);
                                            SI_DTO.SI_INC_CM_Number = Convert.ToInt64(ItemInc.SII_INC_CM_Number);
                                            SI_DTO.SI_INC_IncomeBase = Convert.ToDouble(ItemInc.SII_INC_IncomeBase);
                                            SI_DTO.SI_INC_IncomeValue = Convert.ToDouble(ItemInc.SII_INC_IncomeValue);
                                            SI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(ItemInc.SII_INC_ALCT_Number);
                                            SI_DTO.SI_INC_LA_Number = Convert.ToInt64(ItemInc.SII_INC_LA_Number);
                                            if (ItemInc.SII_INC_Number == 0)
                                            {
                                                SI_DTO.SI_Id = 24;
                                            }
                                            else
                                            {
                                                SI_DTO.SI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_Number);
                                                SI_DTO.SI_Id = 124;
                                            }
                                            SI_DAO.SaleInvoiceDB(SI_DTO);
                                        }

                                        //SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                        //SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                        //SI_DTO.SI_BCH_Mode = Convert.ToInt32(11);
                                        //SI_DTO.SI_Id = 157;
                                        //DS = SI_DAO.SaleInvoiceDB(SI_DTO);
                                        //if (DS.Tables[0].Rows.Count > 0)
                                        //{
                                        //    DataTable dt = DS.Tables[0];
                                        //    foreach (DataRow row in dt.Rows)
                                        //    {
                                        //        SI_DTO.SI_BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                        //        SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                        //        SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                        //        SI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                        //        SI_DTO.SI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                        //        SI_DTO.SI_BCH_No = Convert.ToString(row["BCH_No"]);
                                        //        SI_DTO.SI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                        //        SI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                        //        SI_DTO.SI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                        //        SI_DTO.SI_Id = 25;
                                        //        SI_DAO.SaleInvoiceDB(SI_DTO);
                                        //    }
                                        //}
                                    }
                                }
                                foreach (var Income in Income_DTO)
                                {
                                    SI_DTO.SI_INC_MIC_Number = Convert.ToInt64(Income.SIH_INC_MIC_Number);
                                    SI_DTO.SI_INC_Remarks = Income.SIH_INC_Remarks;
                                    SI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(Income.SIH_INC_OCRN_Number);
                                    SI_DTO.SI_INC_CM_Number = Convert.ToInt64(Income.SIH_INC_CM_Number);
                                    SI_DTO.SI_INC_IncomeBase = Convert.ToDouble(Income.SIH_INC_IncomeBase);
                                    SI_DTO.SI_INC_IncomeValue = Convert.ToDouble(Income.SIH_INC_IncomeValue);
                                    SI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(Income.SIH_INC_ALCT_Number);
                                    SI_DTO.SI_INC_LA_Number = Convert.ToInt64(Income.SIH_INC_LA_Number);
                                    SI_DTO.SI_INC_CalculateGST = Convert.ToInt64(Income.SIH_INC_CalculateGST);
                                    SI_DTO.SI_INC_GST_Amount = Convert.ToDouble(Income.SIH_INC_GST_Amount);
                                    SI_DTO.SI_INC_SAC_Number = Convert.ToInt64(Income.SIH_INC_SAC_Number);
                                    SI_DTO.SI_INC_WHT_Percent = Convert.ToDouble(Income.SIH_INC_WHT_Percent);
                                    SI_DTO.SI_INC_WHT_Amount = Convert.ToDouble(Income.SIH_INC_WHT_Amount);
                                    if (Income.SIH_INC_Number == 0)
                                    {
                                        SI_DTO.SI_Id = 23;
                                    }
                                    else
                                    {
                                        SI_DTO.SI_INC_Number = Convert.ToInt64(Income.SIH_INC_Number);
                                        SI_DTO.SI_Id = 123;
                                    }
                                    SI_DAO.SaleInvoiceDB(SI_DTO);
                                }
                                foreach (var BuyerAddress in BuyerAddress_DTO)
                                {
                                    SI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SIH_ADD_ADTP_Number);
                                    SI_DTO.SI_ADD_AddressID = Convert.ToString(BuyerAddress.SIH_ADD_AddressID);
                                    SI_DTO.SI_ADD_Address = Convert.ToString(BuyerAddress.SIH_ADD_Address);
                                    SI_DTO.SI_ADD_City = Convert.ToString(BuyerAddress.SIH_ADD_City);
                                    SI_DTO.SI_ADD_State = Convert.ToString(BuyerAddress.SIH_ADD_State);
                                    SI_DTO.SI_ADD_Country = Convert.ToString(BuyerAddress.SIH_ADD_Country);
                                    SI_DTO.SI_ADD_Pin = Convert.ToString(BuyerAddress.SIH_ADD_Pin);
                                    SI_DTO.SI_ADD_GSTIN = Convert.ToString(BuyerAddress.SIH_ADD_GSTIN);
                                    if (BuyerAddress.SIH_ADD_Number == 0)
                                    {
                                        SI_DTO.SI_Id = 26;
                                    }
                                    else
                                    {
                                        SI_DTO.SI_ADD_Number = Convert.ToInt64(BuyerAddress.SIH_ADD_Number);
                                        SI_DTO.SI_Id = 126;
                                    }
                                    SI_DAO.SaleInvoiceDB(SI_DTO);
                                }

                                transaction.Complete();

                                S_Head_DTO.Reset();
                                Income_DTO = null;
                                ITM_DTO = null;
                                ItemIncome_DTO = null;
                                S_DTO.Reset();
                                Original_DTO = Help.JsonClone(S_DTO);
                                return RedirectToAction("SaleInvoiceRegisterSummary");
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
            }

            SaleGetData();
            return View(Original_DTO);
        }

        String GetSIEditData(String SI_No)
        {
            SI_DTO.SI_Number = Convert.ToInt64(SI_No);
            SI_DTO.SI_Id = 61;
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SIH_DTO = S_DL.SIHeadEditList(DS.Tables[0]).FirstOrDefault();
                SIH_DTO.InvoiceItem = S_DL.SIItemEditList(DS.Tables[1]);
                SIH_DTO.Income = S_DL.SIIncomeEditList(DS.Tables[2]);
                SIH_DTO.ItemIncome = S_DL.SIIIncomeEditList(DS.Tables[3]);
                SIH_DTO.BuyerAddress = S_DL.SIHAddressEditList(DS.Tables[5]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["SIH_Mode"].ToString();
                return "1";
            }
            else
            {
                return "0";
            }
        }





        //Sale Invoice summary
        [Route("sale/transactions/sale-invoice-register-summary")]
        public IActionResult SaleInvoiceRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            SIR_List = SISummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<SaleInvoiceRegister_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("sale/transactions/sale-invoice-register-summary")]
        [HttpPost]
        public IActionResult SaleInvoiceRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? SI_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            if (Mode == "DeleteAll")
            {
                SI_DTO.SI_DeleteNumbers = DeleteNumbers;
                SI_DTO.SI_Id = 31;
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            }
            else if (Mode == "Ascii" || Mode == "Excel" || Mode == "PDF")
            {
                //List<Int64> AllowedPoNumbers = new List<Int64>();
                //if (DeleteNumber.Length > 0)
                //{
                //    try
                //    {
                //        AllowedPoNumbers = DeleteNumber
                //            .Select(s => s.Trim())
                //            .Where(s => !string.IsNullOrEmpty(s))
                //            .Select(Int64.Parse)
                //            .ToList();
                //    }
                //    catch
                //    {
                //    }
                //}
                //HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

                //SI_DTO.SIH_Id = 21;
                //SI_DTO.SIH_CreatorCode = Convert.ToInt32(UserCode);
                //DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                //if (Mode == "Ascii")
                //{
                //    List<SaleInvoiceAscii> SIR_List = P_DL.POAscii(DS.Tables[0]);

                //    var Key = SIR_List;
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SIH_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    Decimal TotalQtySum = 0;
                //    Decimal TotalMaterialValueSum = 0;
                //    Decimal TotalItemMiscIncomeSum = 0;
                //    Decimal TotalHeadMiscIncomeSum = 0;

                //    var HeaderRow = typeof(SaleInvoiceAscii)
                //            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                //            .Where(prop => prop.Name != nameof(SaleInvoiceAscii.SIH_Number))
                //            .Select(prop =>
                //                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                //             )
                //            .ToList();

                //    var AsciiData = new StringBuilder();
                //    AsciiData.AppendLine(string.Join("\t", HeaderRow));



                //    PropertyInfo[] PropertiesToInclude = typeof(SaleInvoiceAscii)
                //        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                //        .Where(prop => prop.Name != nameof(SaleInvoiceAscii.SIH_Number))
                //        .ToArray();

                //    foreach (var item in Key)
                //    {
                //        var rowValues = PropertiesToInclude
                //            .Select(prop => prop.GetValue(item)?.ToString() ?? "")
                //            .ToList();

                //        var escapedRowValues = rowValues
                //            .Select(val => val.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "))
                //            .ToList();

                //        AsciiData.AppendLine(string.Join("\t", escapedRowValues));

                //        if (Decimal.TryParse(item.TotalQty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                //        {
                //            TotalQtySum += QtyValue;
                //        }
                //        if (Decimal.TryParse(item.MaterialValue, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                //        {
                //            TotalMaterialValueSum += MaterialValue;
                //        }
                //        if (Decimal.TryParse(item.ItemMiscIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                //        {
                //            TotalItemMiscIncomeSum += ItemMiscValue;
                //        }
                //        if (Decimal.TryParse(item.HeadMiscIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                //        {
                //            TotalHeadMiscIncomeSum += HeadMiscValue;
                //        }
                //    }

                //    List<String> FooterCells = new List<String>();
                //    Boolean TotalLabelAdded = false;
                //    foreach (var prop in PropertiesToInclude)
                //    {
                //        string FooterCellValue = "";

                //        if (!TotalLabelAdded && prop.Name != nameof(SaleInvoiceAscii.TotalQty) &&
                //                                 prop.Name != nameof(SaleInvoiceAscii.MaterialValue) &&
                //                                 prop.Name != nameof(SaleInvoiceAscii.ItemMiscIncome) &&
                //                                 prop.Name != nameof(SaleInvoiceAscii.HeadMiscIncome))
                //        {
                //            if (FooterCells.Count == 0)
                //            {
                //                FooterCellValue = "Total:";
                //                TotalLabelAdded = true;
                //            }
                //        }


                //        switch (prop.Name)
                //        {
                //            case nameof(SaleInvoiceAscii.TotalQty):
                //                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleInvoiceAscii.MaterialValue):
                //                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleInvoiceAscii.ItemMiscIncome):
                //                FooterCellValue = TotalItemMiscIncomeSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleInvoiceAscii.HeadMiscIncome):
                //                FooterCellValue = TotalHeadMiscIncomeSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //        }

                //        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                //    }

                //    AsciiData.AppendLine(string.Join("\t", FooterCells));

                //    String FileName = "PO-download";
                //    byte[] fileBytes = Encoding.UTF8.GetBytes(AsciiData.ToString());
                //    var contentType = "text/plain";
                //    var fileDownloadName = $"{FileName}.txt";
                //    return File(fileBytes, contentType, fileDownloadName);

                //}
                //else if (Mode == "Excel")
                //{
                //    SIR_List = S_DL.SIList(DS.Tables[0]);

                //    var Key = SIR_List.ToList();
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SIH_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    String FileName = "PO-download";
                //    using (var wb = new XLWorkbook())
                //    {
                //        var ws = wb.Worksheets.Add(SummaryDownload(Key.ToList()), "Sheet1");

                //        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //        ws.Style.Font.Bold = true;
                //        ws.Columns().AdjustToContents();

                //        using (var stream = new MemoryStream())
                //        {
                //            wb.SaveAs(stream);

                //            var content = stream.ToArray();
                //            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //            var fileDownloadName = $"{FileName}.xlsx";

                //            return File(content, contentType, fileDownloadName);
                //        }
                //    }
                //}
                //else if (Mode == "PDF")
                //{
                //    SIR_List = P_DL.POList(DS.Tables[0]);

                //    var Key = SIR_List.ToList();
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SIH_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.SIH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.SIH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    String PDFDownload = $@"<html>
                //    <head></head>
                //    <body>
                //        <style>
                //            html,body{{
                //                font-family: ""Noto Sans"", sans-serif;
                //                width: 100%;
                //                margin: 0;
                //                background-color: #fff;
                //            }}

                //            .table {{
                //                border-collapse: collapse;
                //                border: 1px SIlid #ccc;
                //                font-size: 0.6rem;
                //                font-family: ""Noto Sans"", sans-serif;
                //            }}
                //                .table th {{
                //                    border: 1px SIlid #ccc;
                //                    padding: 5px 8px;
                //                    vertical-align: top;
                //                background-color: #000;
                //                    color:#fff;
                //                }}

                //                .table td {{
                //                    border: 1px SIlid #ccc;
                //                    padding: 5px 8px;
                //                    vertical-align: top;
                //                }}

                //                .table th {{
                //                    font-weight: bold;
                //                }}

                //                .table tr td {{
                //                    height: 25px;
                //                }}
                //        </style>";

                //    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Buyer Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Export Order</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Amount</th><th style='width:70px'>Item Misc.Income Value</th><th style='width:70px'>Header Misc.Income Value</th></tr>";

                //    Decimal TotalQtySum = 0;
                //    Decimal TotalMaterialValueSum = 0;
                //    Decimal TotalItemMiscIncomeSum = 0;
                //    Decimal TotalHeadMiscIncomeSum = 0;

                //    if (Key.ToList().Count > 0)
                //    {
                //        foreach (var Row in Key.ToList())
                //        {
                //            String Export = "";
                //            Export = (Row.SI_ExportOrder.ToString() == "1") ? "Yes" : "No"; 

                //            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.MaterialValue));
                //            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.ItemMiscIncome));
                //            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.HeadMiscIncome));
                //            PDFDownload += $@"<tr>
                //            <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.SI_OrderDate).ToString("dd-MMM-yyyy")}</td>
                //            <td>{Row.SI_OrderNo}</td>
                //            <td>{Row.SI_BUY_Name}</td>
                //            <td style='width:30px;text-align:center'>{Row.SI_CUR_Name}</td>
                //            <td style='width:30px;text-align:center'>{Export}</td>
                //            <td>{Row.SI_MS_Name}</td>
                //            <td style='width:30px;text-align:center'>{Row.TotalItem}</td>
                //            <td style='width:30px;text-align:center'>{Row.TotalQty}</td>
                //            <td style='width:70px;text-align:right'>{Matrial}</td>
                //            <td style='width:70px;text-align:right'>{ItemExp}</td>
                //            <td style='width:70px;text-align:right'>{HeadExp}</td>
                //            </tr>";


                //            if (Decimal.TryParse(Row.SI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                //            {
                //                TotalQtySum += QtyValue;
                //            }
                //            if (Decimal.TryParse(Row.SI_TotalAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                //            {
                //                TotalMaterialValueSum += MaterialValue;
                //            }
                //            if (Decimal.TryParse(Row.SI_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                //            {
                //                TotalItemMiscIncomeSum += ItemMiscValue;
                //            }
                //            if (Decimal.TryParse(Row.SI_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                //            {
                //                TotalHeadMiscIncomeSum += HeadMiscValue;
                //            }
                //        }
                //    }

                //    PDFDownload += $@"<tr>
                //            <td>Total</td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscIncomeSum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscIncomeSum))}</td>
                //            </tr>";


                //    PDFDownload += $@"</table></body></html>";

                //    HtmlToPdf converter = new HtmlToPdf();
                //    converter.Options.PdfPageSize = PdfPageSize.A4;
                //    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                //    converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                //    converter.Options.MarginLeft = 10;
                //    converter.Options.MarginRight = 10;
                //    converter.Options.MarginTop = 10;
                //    converter.Options.MarginBottom = 10;

                //    PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                //    MemoryStream memoryStream = new MemoryStream();
                //    doc.Save(memoryStream);
                //    doc.Close();

                //    return File(memoryStream.ToArray(), "application/pdf", "SI_Download.pdf");
                //}
            }
            else if (Mode == "View")
            {
                SI_DTO.SI_Number = Convert.ToInt32(SI_No);
                SI_DTO.SI_Id = 42;
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    return RedirectToAction("PreviewSaleInvoice", new { SI_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }
            else if (Mode == "Edit")
            {
                SI_DTO.SI_Number = Convert.ToInt32(SI_No);
                SI_DTO.SI_Id = 42;
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (DS.Tables[0].Rows[0]["SIH_Mode"].ToString() == "1")
                    {
                        return RedirectToAction("EditSaleInvoice", new { SI_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["SIH_Mode"].ToString() == "2")
                    {
                        return RedirectToAction("SOToEditSaleInvoice", new { SI_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["SIH_Mode"].ToString() == "3")
                    {
                        return RedirectToAction("SOItemToEditSaleInvoice", new { SI_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                }
            }

            SIR_List = SISummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<SaleInvoiceRegister_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }
        DataTable SummaryDownload(List<SaleInvoiceRegister_DTO> SIR_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "sale-invoice-summary";

            TableDown.Clear();
            TableDown.Columns.Add("Date");
            TableDown.Columns.Add("Order Number");
            TableDown.Columns.Add("Buyer Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Export Order");
            TableDown.Columns.Add("Material Segregation");
            TableDown.Columns.Add("No. of Item");
            TableDown.Columns.Add("Qty");
            TableDown.Columns.Add("Amount");
            TableDown.Columns.Add("Item Misc Income Value");
            TableDown.Columns.Add("Head Misc Income Value");

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscIncomeSum = 0;
            Decimal TotalHeadMiscIncomeSum = 0;

            foreach (var Product in SIR_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["Date"] = Product.SI_InvoiceDate;
                NewRow["Invoice Number"] = Product.SI_InvoiceNo;
                NewRow["Buyer Name"] = Product.SI_BUY_Name;
                NewRow["Currency"] = Product.SI_CUR_Name;
                NewRow["Export Order"] = (Product.SI_ExportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Material Segregation"] = Product.SI_MS_Name;
                NewRow["No. of Item"] = Product.SI_NoOfItem;
                NewRow["Qty"] = Product.SI_Qty;
                NewRow["Amount"] = Product.SI_TotalAmount;
                NewRow["Item Misc Income Value"] = Product.SI_TotalItemIncome;
                NewRow["Head Misc Income Value"] = Product.SI_TotalHeadIncome;

                TableDown.Rows.Add(NewRow);


                if (Decimal.TryParse(Product.SI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.SI_TotalAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.SI_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscIncomeSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.SI_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                {
                    TotalHeadMiscIncomeSum += HeadMiscValue;
                }
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["Date"] = "Total";
            NewRows["Order Number"] = "";
            NewRows["Buyer Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Export Order"] = "";
            NewRows["Material Segregation"] = "";
            NewRows["No. of Item"] = "";
            NewRows["Qty"] = TotalQtySum;
            NewRows["Amount"] = TotalMaterialValueSum;
            NewRows["Item Misc Income Value"] = TotalItemMiscIncomeSum;
            NewRows["Head Misc Income Value"] = TotalHeadMiscIncomeSum;
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }
        List<SaleInvoiceRegister_DTO> SISummaryGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            SI_DTO.SI_Id = 51;
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            SIR_List = S_DL.SISummaryList(DS.Tables[0]);

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

            ViewData["CurrentSIrt"] = SortOrder;
            ViewData["KeySIrt"] = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["CurrentFilter"] = Search;

            var Key = SIR_List.OrderByDescending(Cs => Cs.SI_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.SI_InvoiceDate.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SI_InvoiceNo.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SI_BUY_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SI_CUR_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SI_MS_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SI_NoOfItem.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SI_Qty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SI_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.SI_InvoiceDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.SI_InvoiceDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.SI_Number);
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

            ViewBag.SumOfItem = Key.Sum(item => Convert.ToDouble(item.SI_NoOfItem));
            ViewBag.SumOfQty = Key.Sum(item => Convert.ToDouble(item.SI_Qty));
            ViewBag.SumOfMatrialValue = Key.Sum(item => Convert.ToDouble(item.SI_MaterialValue));
            ViewBag.SumOfItemIncome = Key.Sum(item => Convert.ToDouble(item.SI_TotalItemIncome));
            ViewBag.SumOfHeadIncome = Key.Sum(item => Convert.ToDouble(item.SI_TotalHeadIncome));
            ViewBag.SumOfAmount = Key.Sum(item => Convert.ToDouble(item.SI_TotalAmount));
            ViewBag.SumOfItemGst = Key.Sum(item => Convert.ToDouble(item.SI_ItemGST_Amount));
            ViewBag.SumOfHeadGst = Key.Sum(item => Convert.ToDouble(item.SI_HeadGST_Amount));
            ViewBag.SumOfInvoice = Key.Sum(item => Convert.ToDouble(item.SI_InvoiceAmount));
            ViewBag.SumOfItemWHT = Key.Sum(item => Convert.ToDouble(item.SI_ItemWHT_Amount));
            ViewBag.SumOfHeadWHT = Key.Sum(item => Convert.ToDouble(item.SI_HeadWHT_Amount));
            ViewBag.SumOfRound = Key.Sum(item => Convert.ToDouble(item.SI_RoundOff));
            ViewBag.SumOfReceivable = Key.Sum(item => Convert.ToDouble(item.SI_BuyerReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }


        [Route("sale/transactions/sale-invoice-register-summary/print")]
        public String SISummaryPrint(String Search, String SelectedItem, bool AllItem)
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
            SI_DTO.SI_Id = 21;
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            SIR_List = S_DL.SISummaryList(DS.Tables[0]);

            var Key = SIR_List.OrderByDescending(Cs => Cs.SI_Number);

            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.SI_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_NoOfItem!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_Qty!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_TotalAmount!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SI_Number);
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SI_Number)).OrderByDescending(K => K.SI_Number);
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.SI_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_NoOfItem!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_Qty!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_TotalAmount!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                     K.SI_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SI_Number);
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
                                        border: 1px SIlid #ccc;
                                        font-size: 0.6rem;
                                        font-family: ""Noto Sans"", sans-serif;
                                    }}
                                        .table th {{
                                            border: 1px SIlid #ccc;
                                            padding: 5px 8px;
                                            vertical-align: top;
                                        background-color: #000;
                                            color:#fff;
                                        }}

                                        .table td {{
                                            border: 1px SIlid #ccc;
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

            PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Buyer Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Export Order</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Amount</th><th style='width:70px'>Item Misc.Income Value</th><th style='width:70px'>Header Misc.Income Value</th></tr>";

            Decimal TotalQtySum = 0;
            Decimal TotalAmountSum = 0;
            Decimal TotalItemMiscIncomeSum = 0;
            Decimal TotalHeadMiscIncomeSum = 0;

            if (Key.ToList().Count > 0)
            {
                foreach (var Row in Key.ToList())
                {
                    String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SI_TotalAmount));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SI_TotalItemIncome));
                    String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SI_TotalHeadIncome));
                    PDFDownload += $@"<tr>
                                    <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.SI_InvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                    <td>{Row.SI_InvoiceNo}</td>
                                    <td>{Row.SI_BUY_Name}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_CUR_Name}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_ExportOrder}</td>
                                    <td>{Row.SI_MS_Name}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_NoOfItem}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_Qty}</td>
                                    <td style='width:70px;text-align:right'>{Amount}</td>
                                    <td style='width:70px;text-align:right'>{ItemExp}</td>
                                    <td style='width:70px;text-align:right'>{HeadExp}</td>
                                    </tr>";


                    if (Decimal.TryParse(Row.SI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.SI_TotalAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalAmountSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.SI_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscIncomeSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.SI_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                    {
                        TotalHeadMiscIncomeSum += HeadMiscValue;
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
                                    <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                    <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalAmountSum))}</td>
                                    <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscIncomeSum))}</td>
                                    <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscIncomeSum))}</td>
                                    </tr>";

            PDFDownload += $@"</table></body></html>";

            return PDFDownload;
        }






        //Sale Invoice Detailed
        [Route("sale/transactions/sale-invoice-register-detailed")]
        public IActionResult SaleInvoiceRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            SIR_List = SIDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<SaleInvoiceRegister_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("sale/transactions/sale-invoice-register-detailed")]
        [HttpPost]
        public IActionResult SaleInvoiceRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? SI_No, String[] DeleteNumber, String selectAllCheckbox)
        {
            if (Mode == "Ascii" || Mode == "Excel" || Mode == "PDF")
            {
                //List<Int64> AllowedPoNumbers = new List<Int64>();
                //if (DeleteNumber.Length > 0)
                //{
                //    try
                //    {
                //        AllowedPoNumbers = DeleteNumber
                //            .Select(s => s.Trim())
                //            .Where(s => !string.IsNullOrEmpty(s))
                //            .Select(Int64.Parse)
                //            .ToList();
                //    }
                //    catch
                //    {
                //    }
                //}
                //HashSet<Int64> AllowedPoNumbersSet = new HashSet<Int64>(AllowedPoNumbers);

                //SI_DTO.Id = 22;
                //SI_DTO.CreatorCode = Convert.ToInt32(UserCode);
                //DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                //if (Mode == "Ascii")
                //{
                //    List<SaleInvoiceDetailAscii> SIR_List = P_DL.PODetailAscii(DS.Tables[0]);

                //    var Key = SIR_List;
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POH_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    Decimal TotalQtySum = 0;
                //    Decimal TotalMaterialValueSum = 0;
                //    Decimal TotalItemMiscExpenseSum = 0;
                //    Decimal TotalHeadMiscExpenseSum = 0;

                //    var HeaderRow = typeof(SaleInvoiceDetailAscii)
                //            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                //            .Where(prop => prop.Name != nameof(SaleInvoiceDetailAscii.POH_Number))
                //            .Select(prop =>
                //                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                //             )
                //            .ToList();

                //    var AsciiData = new StringBuilder();
                //    AsciiData.AppendLine(string.Join("\t", HeaderRow));


                //    PropertyInfo[] PropertiesToInclude = typeof(SaleInvoiceDetailAscii)
                //        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                //        .Where(prop => prop.Name != nameof(SaleInvoiceDetailAscii.POH_Number))
                //        .ToArray();

                //    foreach (var item in Key)
                //    {
                //        var rowValues = PropertiesToInclude
                //            .Select(prop => prop.GetValue(item)?.ToString() ?? "")
                //            .ToList();

                //        var escapedRowValues = rowValues
                //            .Select(val => val.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "))
                //            .ToList();

                //        AsciiData.AppendLine(string.Join("\t", escapedRowValues));

                //        if (Decimal.TryParse(item.POI_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                //        {
                //            TotalQtySum += QtyValue;
                //        }
                //        if (Decimal.TryParse(item.POI_Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                //        {
                //            TotalMaterialValueSum += MaterialValue;
                //        }
                //        if (Decimal.TryParse(item.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                //        {
                //            TotalItemMiscExpenseSum += ItemMiscValue;
                //        }
                //        if (Decimal.TryParse(item.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                //        {
                //            TotalHeadMiscExpenseSum += HeadMiscValue;
                //        }
                //    }

                //    List<String> FooterCells = new List<String>();
                //    Boolean TotalLabelAdded = false;
                //    foreach (var prop in PropertiesToInclude)
                //    {
                //        string FooterCellValue = "";

                //        if (!TotalLabelAdded && prop.Name != nameof(SaleInvoiceDetailAscii.POI_Qty) &&
                //                                 prop.Name != nameof(SaleInvoiceDetailAscii.POI_Amount) &&
                //                                 prop.Name != nameof(SaleInvoiceDetailAscii.ItemMiscExpense) &&
                //                                 prop.Name != nameof(SaleInvoiceDetailAscii.HeadMiscExpense))
                //        {
                //            if (FooterCells.Count == 0)
                //            {
                //                FooterCellValue = "Total:";
                //                TotalLabelAdded = true;
                //            }
                //        }

                //        switch (prop.Name)
                //        {
                //            case nameof(SaleInvoiceDetailAscii.POI_Qty):
                //                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleInvoiceDetailAscii.POI_Amount):
                //                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleInvoiceDetailAscii.ItemMiscExpense):
                //                FooterCellValue = TotalItemMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //            case nameof(SaleInvoiceDetailAscii.HeadMiscExpense):
                //                FooterCellValue = TotalHeadMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                //                break;
                //        }

                //        FooterCells.Add(FooterCellValue.Replace("\t", " ").Replace("\r", " ").Replace("\n", " "));
                //    }

                //    AsciiData.AppendLine(string.Join("\t", FooterCells));

                //    String FileName = "PO-download";
                //    byte[] fileBytes = Encoding.UTF8.GetBytes(AsciiData.ToString());
                //    var contentType = "text/plain";
                //    var fileDownloadName = $"{FileName}.txt";
                //    return File(fileBytes, contentType, fileDownloadName);

                //}
                //else if (Mode == "Excel")
                //{
                //    SIR_List = P_DL.PODetailList(DS.Tables[0]);

                //    var Key = SIR_List.ToList();
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POI_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    String FileName = "PO-download";
                //    using (var wb = new XLWorkbook())
                //    {
                //        var ws = wb.Worksheets.Add(PODetailsDownload(Key.ToList()), "Sheet1");

                //        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //        ws.Style.Font.Bold = true;
                //        ws.Columns().AdjustToContents();

                //        using (var stream = new MemoryStream())
                //        {
                //            wb.SaveAs(stream);

                //            var content = stream.ToArray();
                //            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //            var fileDownloadName = $"{FileName}.xlsx";

                //            return File(content, contentType, fileDownloadName);
                //        }
                //    }
                //}
                //else if (Mode == "PDF")
                //{
                //    SIR_List = P_DL.PODetailList(DS.Tables[0]);

                //    var Key = SIR_List.ToList();
                //    if (selectAllCheckbox == "on")
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }
                //    else if (DeleteNumber.Length > 0)
                //    {
                //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.POI_Number)).ToList();
                //    }
                //    else
                //    {
                //        if (!String.IsNullOrEmpty(Search))
                //        {
                //            Key = Key.Where(K => K.POH_Date!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UoM!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_Amount!.ToString().ToLower().Contains(Search.ToLower()) ||
                //                 K.ItemMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.HeadMiscExpense!.ToLower().Contains(Search.ToLower()) ||
                //                 K.POI_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                //        }
                //    }

                //    String PDFDownload = $@"<html>
                //    <head></head>
                //    <body>
                //        <style>
                //            html,body{{
                //                font-family: ""Noto Sans"", sans-serif;
                //                width: 100%;
                //                margin: 0;
                //                background-color: #fff;
                //            }}

                //            .table {{
                //                border-collapse: collapse;
                //                border: 1px SIlid #ccc;
                //                font-size: 0.5rem;
                //                font-family: ""Noto Sans"", sans-serif;
                //            }}
                //                .table th {{
                //                    border: 1px SIlid #ccc;
                //                    padding: 5px 8px;
                //                    vertical-align: top;
                //                background-color: #000;
                //                    color:#fff;
                //                }}

                //                .table td {{
                //                    border: 1px SIlid #ccc;
                //                    padding: 5px 8px;
                //                    vertical-align: top;
                //                }}

                //                .table th {{
                //                    font-weight: bold;
                //                }}

                //                .table tr td {{
                //                    height: 25px;
                //                }}
                //        </style>";

                //    PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Buyer Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Import Order</th><th>Material Segregation</th><th>Item Group</th><th>Item Code</th><th>Item Description</th><th style='width:30px;text-align:center'>UoM</th><th style='width:30px;text-align:center'>Qty</th><th style='width:30px;text-align:center'>Unit Price</th><th style='width:70px'>Amount</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th style='width:70px'>Delivery Date</th></tr>";

                //    Decimal TotalQtySum = 0;
                //    Decimal TotalMaterialValueSum = 0;
                //    Decimal TotalItemMiscExpenseSum = 0;
                //    Decimal TotalHeadMiscExpenseSum = 0;

                //    if (Key.ToList().Count > 0)
                //    {
                //        foreach (var Row in Key.ToList())
                //        {
                //            String Import = "";
                //            if (Row.POH_ImportOrder == 1)
                //            {
                //                Import = "Yes";
                //            }
                //            else
                //            {
                //                Import = "No";
                //            }

                //            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.POI_Amount));
                //            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.ItemMiscExpense));
                //            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.HeadMiscExpense));
                //            PDFDownload += $@"<tr>
                //            <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.POH_Date).ToString("dd-MMM-yyyy")}</td>
                //            <td>{Row.POH_OrderNo}</td>
                //            <td>{Row.POH_Buyer_Number}</td>
                //            <td style='width:30px;text-align:center'>{Row.POH_Currency_Number}</td>
                //            <td style='width:30px;text-align:center'>{Import}</td>
                //            <td>{Row.POH_MS_Number}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_ITM_Group}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_ITM_Code}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_ITM_Description}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_UoM}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_Qty}</td>
                //            <td style='width:30px;text-align:center'>{Row.POI_UnitPrice}</td>
                //            <td style='width:70px;text-align:right'>{Matrial}</td>
                //            <td style='width:70px;text-align:right'>{ItemExp}</td>
                //            <td style='width:70px;text-align:right'>{HeadExp}</td>
                //            <td style='width:70px;text-align:right'>{Row.POI_DeliveryDate}</td>
                //            </tr>";


                //            if (Decimal.TryParse(Row.POI_Qty.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                //            {
                //                TotalQtySum += QtyValue;
                //            }
                //            if (Decimal.TryParse(Row.POI_Amount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                //            {
                //                TotalMaterialValueSum += MaterialValue;
                //            }
                //            if (Decimal.TryParse(Row.ItemMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                //            {
                //                TotalItemMiscExpenseSum += ItemMiscValue;
                //            }
                //            if (Decimal.TryParse(Row.HeadMiscExpense, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                //            {
                //                TotalHeadMiscExpenseSum += HeadMiscValue;
                //            }
                //        }
                //    }

                //    PDFDownload += $@"<tr>
                //            <td>Total</td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td></td>
                //            <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                //            <td></td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                //            <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                //            <td></td>
                //            </tr>";


                //    PDFDownload += $@"</table></body></html>";

                //    HtmlToPdf converter = new HtmlToPdf();
                //    converter.Options.PdfPageSize = PdfPageSize.A4;
                //    converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                //    converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                //    converter.Options.MarginLeft = 10;
                //    converter.Options.MarginRight = 10;
                //    converter.Options.MarginTop = 10;
                //    converter.Options.MarginBottom = 10;

                //    PdfDocument doc = converter.ConvertHtmlString(PDFDownload);

                //    MemoryStream memoryStream = new MemoryStream();
                //    doc.Save(memoryStream);
                //    doc.Close();

                //    return File(memoryStream.ToArray(), "application/pdf", "Print_Download.pdf");
                //}
            }
            else if (Mode == "View")
            {
                SI_DTO.SI_Number = Convert.ToInt32(SI_No);
                SI_DTO.SI_Id = 42;
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    return RedirectToAction("PreviewSaleInvoice", new { SI_No = DS.Tables[0].Rows[0][0].ToString() });
                }
            }
            else if (Mode == "Edit")
            {
                SI_DTO.SI_Number = Convert.ToInt32(SI_No);
                SI_DTO.SI_Id = 42;
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (DS.Tables[0].Rows[0]["SIH_Mode"].ToString() == "1")
                    {
                        return RedirectToAction("EditSaleInvoice", new { SI_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["SIH_Mode"].ToString() == "2")
                    {
                        return RedirectToAction("SOToEditSaleInvoice", new { SI_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                    else if (DS.Tables[0].Rows[0]["SIH_Mode"].ToString() == "3")
                    {
                        return RedirectToAction("SOItemToEditSaleInvoice", new { SI_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                }
            }

            SIR_List = SIDetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList_DTO<SaleInvoiceRegister_DTO>.CreateAsync(SIR_List, DPageNumber ?? 1, DPageSize));
        }
        List<SaleInvoiceRegister_DTO> SIDetailedGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            SI_DTO.SI_Id = 52;
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            SIR_List = S_DL.SIDetailedList(DS.Tables[0]);

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

            ViewData["CurrentSIrt"] = SortOrder;
            ViewData["KeySIrt"] = SortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["CurrentFilter"] = Search;

            var Key = SIR_List.OrderByDescending(Cs => Cs.SI_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.SI_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_UoM_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SI_TotalAmount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SI_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SI_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.SI_InvoiceDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.SI_InvoiceDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.SI_Number);
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

            ViewBag.SumOfQty = Key.Sum(item => Convert.ToDouble(item.SI_Qty));
            ViewBag.SumOfMaterialValue = Key.Sum(item => Convert.ToDouble(item.SI_MaterialValue));
            ViewBag.SumOfItemIncome = Key.Sum(item => Convert.ToDouble(item.SI_TotalItemIncome));
            ViewBag.SumOfHeadIncome = Key.Sum(item => Convert.ToDouble(item.SI_TotalHeadIncome));
            ViewBag.SumOfAmount = Key.Sum(item => Convert.ToDouble(item.SI_TotalAmount));
            ViewBag.SumOfItemGST = Key.Sum(item => Convert.ToDouble(item.SI_ItemGST_Amount));
            ViewBag.SumOfHeadGST = Key.Sum(item => Convert.ToDouble(item.SI_HeadGST_Amount));
            ViewBag.SumOfInvoiceAmount = Key.Sum(item => Convert.ToDouble(item.SI_InvoiceAmount));
            ViewBag.SumOfItemWHT = Key.Sum(item => Convert.ToDouble(item.SI_ItemWHT_Amount));
            ViewBag.SumOfHeadWHT = Key.Sum(item => Convert.ToDouble(item.SI_HeadWHT_Amount));
            ViewBag.SumOfRoundOff = Key.Sum(item => Convert.ToDouble(item.SI_RoundOff));
            ViewBag.SumOfReceivable = Key.Sum(item => Convert.ToDouble(item.SI_BuyerReceivable));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }
        DataTable SIDetailsDownload(List<SaleInvoiceRegister_DTO> SIR_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "sale-invoice-detailed";

            TableDown.Clear();
            TableDown.Columns.Add("Date");
            TableDown.Columns.Add("Invoice Number");
            TableDown.Columns.Add("Buyer Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Export Order");
            TableDown.Columns.Add("Material Segregation");
            TableDown.Columns.Add("Item Group");
            TableDown.Columns.Add("Item Code");
            TableDown.Columns.Add("Item Description");
            TableDown.Columns.Add("UoM");
            TableDown.Columns.Add("Qty");
            TableDown.Columns.Add("Unit Price");
            TableDown.Columns.Add("Amount");
            TableDown.Columns.Add("Item Misc Expense Value");
            TableDown.Columns.Add("Head Misc Expense Value");
            TableDown.Columns.Add("Delivery Date");


            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscIncomeSum = 0;
            Decimal TotalHeadMiscIncomeSum = 0;

            foreach (var Product in SIR_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["Date"] = Product.SI_InvoiceDate;
                NewRow["Invoice Number"] = Product.SI_InvoiceNo;
                NewRow["Buyer Name"] = Product.SI_BUY_Name;
                NewRow["Currency"] = Product.SI_CUR_Name;
                NewRow["Export Order"] = (Product.SI_ExportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Material Segregation"] = Product.SI_MS_Name;
                NewRow["Item Group"] = Product.SI_ITM_Group;
                NewRow["Item Code"] = Product.SI_ITM_Code;
                NewRow["Item Description"] = Product.SI_ITM_Description;
                NewRow["UoM"] = Product.SI_UoM_Name;
                NewRow["Qty"] = Product.SI_Qty;
                NewRow["Unit Price"] = Product.SI_UnitPrice;
                NewRow["Amount"] = Product.SI_TotalAmount;
                NewRow["Item Misc Expense Value"] = Product.SI_TotalItemIncome;
                NewRow["Head Misc Expense Value"] = Product.SI_TotalHeadIncome;

                TableDown.Rows.Add(NewRow);

                if (Decimal.TryParse(Product.SI_Qty.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.SI_TotalAmount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.SI_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscIncomeSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.SI_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                {
                    TotalHeadMiscIncomeSum += HeadMiscValue;
                }
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["Date"] = "Total";
            NewRows["Invoice Number"] = "";
            NewRows["Buyer Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Export Order"] = "";
            NewRows["Material Segregation"] = "";
            NewRows["Item Group"] = "";
            NewRows["Item Code"] = "";
            NewRows["Item Description"] = "";
            NewRows["UoM"] = "";
            NewRows["Qty"] = TotalQtySum;
            NewRows["Unit Price"] = "";
            NewRows["Amount"] = TotalMaterialValueSum;
            NewRows["Item Misc.Income Value"] = TotalItemMiscIncomeSum;
            NewRows["Head Misc.Income Value"] = TotalHeadMiscIncomeSum;
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }

        [Route("sale/transactions/sale-invoice-register-detailed/print")]
        public String SIDetailedPrint(String Search, String SelectedItem, bool AllItem)
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
            SI_DTO.SI_Id = 22;
            SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            DS = SI_DAO.SaleInvoiceDB(SI_DTO);
            SIR_List = S_DL.SIDetailedList(DS.Tables[0]);

            var Key = SIR_List.ToList();

            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.SI_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_UoM_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SI_TotalAmount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SI_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).ToList();
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SI_Number)).ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.SI_InvoiceDate!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_InvoiceNo!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_UoM_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SI_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SI_TotalAmount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SI_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SI_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).ToList();
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
                                        border: 1px SIlid #ccc;
                                        font-size: 0.5rem;
                                        font-family: ""Noto Sans"", sans-serif;
                                    }}
                                        .table th {{
                                            border: 1px SIlid #ccc;
                                            padding: 5px 8px;
                                            vertical-align: top;
                                        background-color: #000;
                                            color:#fff;
                                        }}

                                        .table td {{
                                            border: 1px SIlid #ccc;
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

            PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Buyer Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Import Order</th><th>Material Segregation</th><th>Item Group</th><th>Item Code</th><th>Item Description</th><th style='width:30px;text-align:center'>UoM</th><th style='width:30px;text-align:center'>Qty</th><th style='width:30px;text-align:center'>Unit Price</th><th style='width:70px'>Amount</th><th style='width:70px'>Item Misc.Expense Value</th><th style='width:70px'>Header Misc.Expense Value</th><th style='width:70px'>Delivery Date</th></tr>";

            Decimal TotalQtySum = 0;
            Decimal TotalMaterialValueSum = 0;
            Decimal TotalItemMiscExpenseSum = 0;
            Decimal TotalHeadMiscExpenseSum = 0;

            if (Key.ToList().Count > 0)
            {
                foreach (var Row in Key.ToList())
                {
                    String Import = "";

                    String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SI_TotalAmount));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SI_TotalItemIncome));
                    String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SI_TotalHeadIncome));

                    PDFDownload += $@"<tr>
                                    <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.SI_InvoiceDate).ToString("dd-MMM-yyyy")}</td>
                                    <td>{Row.SI_InvoiceNo}</td>
                                    <td>{Row.SI_BUY_Name}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_CUR_Name}</td>
                                    <td style='width:30px;text-align:center'>{Import}</td>
                                    <td>{Row.SI_MS_Name}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_ITM_Code}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_ITM_Group}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_ITM_Description}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_UoM_Name}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_Qty}</td>
                                    <td style='width:30px;text-align:center'>{Row.SI_UnitPrice}</td>
                                    <td style='width:70px;text-align:right'>{Matrial}</td>
                                    <td style='width:70px;text-align:right'>{ItemExp}</td>
                                    <td style='width:70px;text-align:right'>{HeadExp}</td>
                                    </tr>";


                    if (Decimal.TryParse(Row.SI_Qty.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.SI_TotalAmount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalMaterialValueSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.SI_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscExpenseSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.SI_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                    {
                        TotalHeadMiscExpenseSum += HeadMiscValue;
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
                                    <td style='width:30px;text-align:center'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalQtySum))}</td>
                                    <td></td>
                                    <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalMaterialValueSum))}</td>
                                    <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalItemMiscExpenseSum))}</td>
                                    <td style='width:70px;text-align:right'>{string.Format(India, "{0:N2}", Convert.ToDouble(TotalHeadMiscExpenseSum))}</td>
                                    </tr>";


            PDFDownload += $@"</table></body></html>";

            return PDFDownload;
        }






        //Sale Invoice SO To Invoice Create
        [Route("sale/transactions/sale-invoice/so/create")]
        public IActionResult SOToCreateSaleInvoice()
        {
            SITempClear("13");
            SOToSaleInvoiceHead_DTO SHSO_DTO = new SOToSaleInvoiceHead_DTO();
            if (TempData["SHSO_DTO_Json"] is string SHSODto)
            {
                SHSO_DTO = System.Text.Json.JsonSerializer.Deserialize<SOToSaleInvoiceHead_DTO>(SHSODto);
            }

            if (SHSO_DTO.SIH_InvoiceDate == null)
            {
                SHSO_DTO.SIH_InvoiceDate = DateTime.Now.ToString("dd-MMM-yy");
                SHSO_DTO.SIH_InvoiceNo = OnSaleNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            }
            else
            {
                SHSO_DTO.SIH_InvoiceNo = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(SHSO_DTO.SIH_InvoiceDate).ToString("yyyyMMdd")));
            }
            SOToSaleInvoiceGetData();

            SHSO_DTO.SIH_InvoiceNo = OnSaleNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            return View(SHSO_DTO);
        }
        void SOToSaleInvoiceGetData()
        {
            SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            SOSI_DTO.SI_Id = 1;
            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

            ViewBag.IncomeCode = Help.GetCat(DS.Tables[0]);
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
            ViewBag.AddressType = Help.GetCat(DS.Tables[11]);
        }

        [HttpPost]
        [Route("sale/transactions/sale-invoice/so/create")]
        public IActionResult SOToCreateSaleInvoice(SOToSaleInvoiceHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SOToSaleInvoiceHead_DTO S_Head_DTO = new SOToSaleInvoiceHead_DTO();

            List<SOToSaleInvoiceItem_DTO>? ITM_DTO = new List<SOToSaleInvoiceItem_DTO>();
            List<SOToSaleInvoiceIncome_DTO>? Income_DTO = new List<SOToSaleInvoiceIncome_DTO>();
            List<SOToSaleInvoiceIIncome_DTO>? ItemIncome_DTO = new List<SOToSaleInvoiceIIncome_DTO>();
            List<SOToSaleInvoiceAddress_DTO>? BuyerAddress_DTO = new List<SOToSaleInvoiceAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.InvoiceItem != null)
                ITM_DTO = S_DTO.InvoiceItem!.Where(K => K.SII_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SIH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SII_INC_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SIH_ADD_IsDeleted == 0).ToList();

            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SII_MS_Number) != Convert.ToInt64(S_DTO.SIH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SII_Qty) == 0 || Convert.ToDouble(x.SII_UnitPrice) == 0 || Convert.ToDouble(x.SII_Amount) == 0);

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
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SIH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SIH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.InvoiceItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        if (SOToBatchValidation(ITM_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String SIHOrderNoOld = S_DTO.SIH_InvoiceNo;
                                    String SIHOrderNoNew = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd")));

                                    SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd"));
                                    SOSI_DTO.SI_InvoiceNo = SIHOrderNoNew;
                                    SOSI_DTO.SI_BUY_Number = Convert.ToInt64(S_DTO.SIH_BUY_Number);
                                    SOSI_DTO.SI_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 1 : 0);
                                    SOSI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                    SOSI_DTO.SI_MS_Number = Convert.ToInt64(S_DTO.SIH_MS_Number);
                                    SOSI_DTO.SI_TCT_Number = Convert.ToInt64(S_DTO.SIH_TCT_Number);
                                    SOSI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                    SOSI_DTO.SI_WHT_Number = Convert.ToInt64(S_DTO.SIH_WHT_Number);
                                    SOSI_DTO.SI_ExchangeRate = Convert.ToDouble(S_DTO.SIH_ExchangeRate) == 0 ? "1" : S_DTO.SIH_ExchangeRate;
                                    SOSI_DTO.SI_MaterialCost = Convert.ToDouble(S_DTO.SIH_MaterialCost);
                                    SOSI_DTO.SI_ItemMiscIncome = Convert.ToDouble(S_DTO.SIH_ItemMiscIncome);
                                    SOSI_DTO.SI_HeaderMiscIncome = Convert.ToDouble(S_DTO.SIH_HeaderMiscIncome);
                                    SOSI_DTO.SI_GST_Amount = Convert.ToDouble(S_DTO.SIH_GST_Amount);
                                    SOSI_DTO.SI_InvoiceAmount = Convert.ToDouble(S_DTO.SIH_InvoiceAmount);
                                    SOSI_DTO.SI_WHT_Amount = Convert.ToDouble(S_DTO.SIH_WHT_Amount);
                                    SOSI_DTO.SI_RoundOff = Convert.ToDouble(S_DTO.SIH_RoundOff);
                                    SOSI_DTO.SI_BuyerReceivable = Convert.ToDouble(S_DTO.SIH_BuyerReceivable);
                                    SOSI_DTO.SI_Id = 21;
                                    DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                                    OnSaleNumberGen(Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        SOSI_DTO.SI_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in ITM_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            SOSI_DTO.SI_SOH_Number = Convert.ToInt64(Item.SII_SOH_Number);
                                            SOSI_DTO.SI_SOI_Number = Convert.ToInt64(Item.SII_SOI_Number);
                                            SOSI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                            SOSI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_WH_Number);
                                            SOSI_DTO.SI_UoM_Number = Convert.ToInt64(Item.SII_UoM_Number);
                                            SOSI_DTO.SI_Qty = Convert.ToDouble(Item.SII_Qty);
                                            SOSI_DTO.SI_UnitPrice = Convert.ToDouble(Item.SII_UnitPrice);
                                            SOSI_DTO.SI_Amount = Convert.ToDouble(Item.SII_Amount);
                                            SOSI_DTO.SI_IncomeValue = Convert.ToDouble(Item.SII_IncomeValue);
                                            SOSI_DTO.SI_HSN_Number = Convert.ToInt64(Item.SII_HSN_Number);
                                            SOSI_DTO.SI_GST_Amount = Convert.ToDouble(Item.SII_GST_Amount);
                                            SOSI_DTO.SI_WHT_Percent = Convert.ToDouble(Item.SII_WHT_Percent);
                                            SOSI_DTO.SI_WHT_Amount = Convert.ToDouble(Item.SII_WHT_Amount);
                                            SOSI_DTO.SI_Id = 22;
                                            D = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (x.SII_INC_SOI_Number == Item.SII_SOI_Number && x.SII_INC_SOH_Number == Item.SII_SOH_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SOSI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SOSI_DTO.SI_SOI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_SOI_INC_Number);
                                                //SOSI_DTO.SI_ITM_Number = Convert.ToInt64(ItemInc.SII_INC_ITM_Number);
                                                SOSI_DTO.SI_INC_MIC_Number = Convert.ToInt64(ItemInc.SII_INC_MIC_Number);
                                                SOSI_DTO.SI_INC_Remarks = ItemInc.SII_INC_Remarks;
                                                SOSI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(ItemInc.SII_INC_OCRN_Number);
                                                SOSI_DTO.SI_INC_CM_Number = Convert.ToInt64(ItemInc.SII_INC_CM_Number);
                                                SOSI_DTO.SI_INC_IncomeBase = Convert.ToDouble(ItemInc.SII_INC_IncomeBase);
                                                SOSI_DTO.SI_INC_IncomeValue = Convert.ToDouble(ItemInc.SII_INC_IncomeValue);
                                                SOSI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(ItemInc.SII_INC_ALCT_Number);
                                                SOSI_DTO.SI_INC_LA_Number = Convert.ToInt64(ItemInc.SII_INC_LA_Number);
                                                SOSI_DTO.SI_Id = 24;
                                                SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                            }

                                            SOSI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                            SOSI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                            SOSI_DTO.SI_BCH_Mode = Convert.ToInt32(13);
                                            SOSI_DTO.SI_Id = 157;
                                            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                            if (DS.Tables[0].Rows.Count > 0)
                                            {
                                                DataTable dt = DS.Tables[0];
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    SOSI_DTO.SI_BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                                    SOSI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                                    SOSI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                                    SOSI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                    SOSI_DTO.SI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                                    SOSI_DTO.SI_BCH_No = Convert.ToString(row["BCH_No"]);
                                                    SOSI_DTO.SI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                                    SOSI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                                    SOSI_DTO.SI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                                    SOSI_DTO.SI_Id = 25;
                                                    SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                                }
                                            }
                                        }
                                        foreach (var Income in Income_DTO)
                                        {
                                            SOSI_DTO.SI_SOH_INC_Number = Convert.ToInt64(Income.SIH_INC_SOH_INC_Number);
                                            SOSI_DTO.SI_INC_MIC_Number = Convert.ToInt64(Income.SIH_INC_MIC_Number);
                                            SOSI_DTO.SI_INC_Remarks = Income.SIH_INC_Remarks;
                                            SOSI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(Income.SIH_INC_OCRN_Number);
                                            SOSI_DTO.SI_INC_CM_Number = Convert.ToInt64(Income.SIH_INC_CM_Number);
                                            SOSI_DTO.SI_INC_IncomeBase = Convert.ToDouble(Income.SIH_INC_IncomeBase);
                                            SOSI_DTO.SI_INC_IncomeValue = Convert.ToDouble(Income.SIH_INC_IncomeValue);
                                            SOSI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(Income.SIH_INC_ALCT_Number);
                                            SOSI_DTO.SI_INC_LA_Number = Convert.ToInt64(Income.SIH_INC_LA_Number);
                                            SOSI_DTO.SI_INC_CalculateGST = Convert.ToInt64(Income.SIH_INC_CalculateGST);
                                            SOSI_DTO.SI_INC_GST_Amount = Convert.ToDouble(Income.SIH_INC_GST_Amount);
                                            SOSI_DTO.SI_INC_SAC_Number = Convert.ToInt64(Income.SIH_INC_SAC_Number);
                                            SOSI_DTO.SI_INC_WHT_Percent = Convert.ToDouble(Income.SIH_INC_WHT_Percent);
                                            SOSI_DTO.SI_INC_WHT_Amount = Convert.ToDouble(Income.SIH_INC_WHT_Amount);
                                            SOSI_DTO.SI_Id = 23;
                                            SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                        }
                                        foreach (var BuyerAddress in BuyerAddress_DTO)
                                        {
                                            SOSI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SIH_ADD_ADTP_Number);
                                            SOSI_DTO.SI_ADD_AddressID = Convert.ToString(BuyerAddress.SIH_ADD_AddressID);
                                            SOSI_DTO.SI_ADD_Address = Convert.ToString(BuyerAddress.SIH_ADD_Address);
                                            SOSI_DTO.SI_ADD_City = Convert.ToString(BuyerAddress.SIH_ADD_City);
                                            SOSI_DTO.SI_ADD_State = Convert.ToString(BuyerAddress.SIH_ADD_State);
                                            SOSI_DTO.SI_ADD_Country = Convert.ToString(BuyerAddress.SIH_ADD_Country);
                                            SOSI_DTO.SI_ADD_Pin = Convert.ToString(BuyerAddress.SIH_ADD_Pin);
                                            SOSI_DTO.SI_ADD_GSTIN = Convert.ToString(BuyerAddress.SIH_ADD_GSTIN);
                                            SOSI_DTO.SI_Id = 26;
                                            SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    if (SIHOrderNoOld != SIHOrderNoNew)
                                    {
                                        ViewBag.ErrorCode = 2;
                                        ViewBag.ErrorMessage = "Sale Invoice number " + SIHOrderNoOld + " used by another user. Next number will be allotted to you.";
                                    }
                                    return RedirectToAction("SOToCreateSaleInvoice");
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
                SaleInvoiceHead_DTO SH_DTO = new SaleInvoiceHead_DTO();

                SH_DTO.SIH_InvoiceNo = S_DTO.SIH_InvoiceNo;
                SH_DTO.SIH_InvoiceDate = S_DTO.SIH_InvoiceDate;
                SH_DTO.SIH_BUY_Number = Convert.ToString(S_DTO.SIH_BUY_Number);
                SH_DTO.SIH_BUY_LOC_Number = Convert.ToString(S_DTO.SIH_BUY_LOC_Number);
                SH_DTO.SIH_ExportOrder = Convert.ToString(S_DTO.SIH_ExportOrder);
                SH_DTO.SIH_CUR_Number = Convert.ToString(S_DTO.SIH_CUR_Number);
                SH_DTO.SIH_MS_Number = Convert.ToString(S_DTO.SIH_MS_Number);
                SH_DTO.SIH_ExchangeRate = Convert.ToString(S_DTO.SIH_ExchangeRate);
                SH_DTO.SIH_TCT_Number = Convert.ToString(S_DTO.SIH_TCT_Number);
                SH_DTO.SIH_WHT_Number = Convert.ToString(S_DTO.SIH_WHT_Number);
                SH_DTO.SIH_WHT_Tax = Convert.ToString(S_DTO.SIH_WHT_Tax);
                SH_DTO.SIH_WHT_Percent = Convert.ToString(S_DTO.SIH_WHT_Percent);
                SH_DTO.SIH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SIH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("CreateSaleInvoice");
            }
            else if (Mode == "SOITEMTO")
            {
                SaleInvoiceHead_DTO SH_DTO = new SaleInvoiceHead_DTO();

                SH_DTO.SIH_InvoiceNo = S_DTO.SIH_InvoiceNo;
                SH_DTO.SIH_InvoiceDate = S_DTO.SIH_InvoiceDate;
                SH_DTO.SIH_BUY_Number = Convert.ToString(S_DTO.SIH_BUY_Number);
                SH_DTO.SIH_BUY_LOC_Number = Convert.ToString(S_DTO.SIH_BUY_LOC_Number);
                SH_DTO.SIH_ExportOrder = Convert.ToString(S_DTO.SIH_ExportOrder);
                SH_DTO.SIH_CUR_Number = Convert.ToString(S_DTO.SIH_CUR_Number);
                SH_DTO.SIH_MS_Number = Convert.ToString(S_DTO.SIH_MS_Number);
                SH_DTO.SIH_ExchangeRate = Convert.ToString(S_DTO.SIH_ExchangeRate);
                SH_DTO.SIH_TCT_Number = Convert.ToString(S_DTO.SIH_TCT_Number);
                SH_DTO.SIH_WHT_Number = Convert.ToString(S_DTO.SIH_WHT_Number);
                SH_DTO.SIH_WHT_Tax = Convert.ToString(S_DTO.SIH_WHT_Tax);
                SH_DTO.SIH_WHT_Percent = Convert.ToString(S_DTO.SIH_WHT_Percent);
                SH_DTO.SIH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SIH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("SOItemToCreateSaleInvoice");
            }
            SaleGetData();
            return View(Original_DTO);
        }

        [Route("sale/transactions/sale-invoice/so/uom")]
        public String SOToSaleUoM(String? UoM)
        {
            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOSI_DTO.SI_UoM_Number = Convert.ToInt64(UoM);
            SOSI_DTO.SI_Id = 4;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("sale/transactions/sale-invoice/so/buyer")]
        public IActionResult SOToSaleBuyer(String? Buyer, String? Export, String? SIHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            if (Export == null)
            {
                Export = "";
            }
            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOSI_DTO.SI_ExportOrder = Convert.ToInt16(Convert.ToBoolean(Export) == true ? 2 : 1);
            SOSI_DTO.SI_ITM_Code = Convert.ToString(Buyer).Trim();
            SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOSI_DTO.SI_Id = 5;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
            var Ven = S_DL.BuyerList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("sale/transactions/sale-invoice/so/gst")]
        public String SOToSaleInvoiceGst(String? Cluster, String? SIHDate, String? HSN, String? BaseAmount)
        {
            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOSI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SOSI_DTO.SI_HSN_Number = Convert.ToInt64(HSN);
            SOSI_DTO.SI_Id = 6;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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

        [Route("sale/transactions/sale-invoice/so/gst/view")]
        public IActionResult SOToSaleInvoiceGstView(String? Cluster, String? SIHDate, String? HSN, String? BaseAmount)
        {
            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOSI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SOSI_DTO.SI_HSN_Number = Convert.ToInt64(HSN);
            SOSI_DTO.SI_Id = 9;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleInvoiceGst> PurGST = new List<SaleInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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
                        new SaleInvoiceGst
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

        [Route("sale/transactions/sale-invoice/so/wht")]
        public IActionResult SOToSaleInvoiceWHT(String? Buyer, String? WHTNumber, String? SIHDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Buyer == null)
            {
                Buyer = "0";
            }
            if (SIHDate == null)
            {
                SIHDate = "0";
            }
            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOSI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOSI_DTO.SI_WHT_Number = Convert.ToInt64(WHTNumber);
            SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOSI_DTO.SI_Id = 7;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
            var WHT = S_DL.SaleInvWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("sale/transactions/sale-invoice/so/cluster")]
        public IActionResult SOToSaleInvoiceCluster(String? Buyer, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            SOSI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOSI_DTO.SI_Search = Cluster;
            SOSI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOSI_DTO.SI_Id = 8;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
            var InvCluster = S_DL.SaleCluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("sale/transactions/sale-invoice/so/order")]
        public IActionResult SOToSaleInvoiceSO(String? Buyer, String? Export, String? MSNumber, String? SIHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }

            SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOSI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOSI_DTO.SI_MS_Number = Convert.ToInt64(MSNumber);
            SOSI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOSI_DTO.SI_ExportOrder = Convert.ToInt32(Convert.ToBoolean(Export) == true ? 1 : 0);
            SOSI_DTO.SI_Id = 11;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
            var POOrder = S_DL.SOToSIOrder(DS.Tables[0]);
            return Json(POOrder);
        }

        [Route("sale/transactions/sale-invoice/so/order/item")]
        public IActionResult SOToSaleInvoiceSOItem(String? SONumber)
        {
            if (SONumber == null)
            {
                SONumber = "";
            }

            SOSI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOSI_DTO.SI_Search = SONumber;
            SOSI_DTO.SI_Id = 12;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

            SOToSI_DTO SOSI = new SOToSI_DTO();
            var SOItem = S_DL.SOToSIOrderItem(DS.Tables[0]);
            var SOIncome = S_DL.SOToSIIncome(DS.Tables[1]);
            var SOItemIncome = S_DL.SOToSIIIncome(DS.Tables[2]);

            SOSI.SOItems = SOItem;
            SOSI.SOIncomes = SOIncome;
            SOSI.SOItemIncomes = SOItemIncome;

            return Json(SOSI);
        }

        [Route("sale/transactions/sale-invoice/so/income/gst")]
        public String SOToSaleInvoiceHeaderGst(String? Cluster, String? SIHDate, String? SAC, String? BaseAmount)
        {
            SOSI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOSI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SOSI_DTO.SI_INC_SAC_Number = Convert.ToInt64(SAC);
            SOSI_DTO.SI_Id = 13;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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

        [Route("sale/transactions/sale-invoice/so/income/gst/view")]
        public IActionResult SOToSaleInvoiceGstHeaderView(String? Cluster, String? SIHDate, String? SAC, String? BaseAmount)
        {
            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOSI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SOSI_DTO.SI_INC_SAC_Number = Convert.ToInt64(SAC);
            SOSI_DTO.SI_Id = 14;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleInvoiceGst> PurGST = new List<SaleInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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
                        new SaleInvoiceGst
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

        [Route("sale/transactions/sale-invoice/so/batch/get")]
        public IActionResult SOToSaleInvoiceBatchGet(String? ItemNumber, String? Index, String? Warehouse)
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

            SOSI_DTO.SI_ITM_Number = Convert.ToInt64(ItemNumber);
            SOSI_DTO.SI_WH_Number = Convert.ToInt64(Warehouse);
            SOSI_DTO.SI_BCH_Index = Convert.ToInt32(Index);
            SOSI_DTO.SI_BCH_Mode = Convert.ToInt32(13);
            SOSI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOSI_DTO.SI_Id = 151;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

            SIBatchItem_DTO PRB = new SIBatchItem_DTO();
            PRB.SIBatch = S_DL.SIBatchList(DS.Tables[0]);
            PRB.SIView = S_DL.SIBatchOverallList(DS.Tables[1]);
            return Json(PRB);
        }

        [Route("sale/transactions/sale-invoice/so/batch/post")]
        [HttpPost]
        public IActionResult SOToSaleInvoiceBatchGet([FromBody] SOToSaleInvoiceItemBatch ItemBatch)
        {
            SOSI_DTO.SI_BCH_Mode = Convert.ToInt32(13);
            SOSI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);

            foreach (var Item in ItemBatch.ItemBatch)
            {
                SOSI_DTO.SI_BCH_Number = Convert.ToInt64(Item.SII_BCH_Number);
                SOSI_DTO.SI_BCH_BCH_Number = Convert.ToInt64(Item.SII_BCH_BCH_Number);
                SOSI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_BCH_ITM_Number);
                SOSI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_BCH_ITM_Index);
                SOSI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_BCH_WH_Number);
                SOSI_DTO.SI_BCH_Date = Convert.ToString(Convert.ToDateTime(Item.SII_BCH_Date).ToString("yyyyMMdd"));
                SOSI_DTO.SI_BCH_No = Convert.ToString(Item.SII_BCH_No);
                SOSI_DTO.SI_BCH_Qty = Convert.ToDouble(Item.SII_BCH_Qty);
                SOSI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(Item.SII_BCH_UnitPrice);
                SOSI_DTO.SI_BCH_Value = Convert.ToDouble(Item.SII_BCH_Value);
                SOSI_DTO.SI_Id = 153;
                DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (Item.SII_BCH_Qty > 0)
                    {
                        SOSI_DTO.SI_Id = 154;
                        SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                    }
                    else
                    {
                        SOSI_DTO.SI_Id = 155;
                        SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                    }
                }
                else
                {
                    if (Item.SII_BCH_Qty > 0)
                    {
                        SOSI_DTO.SI_Id = 152;
                        SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                    }
                }
            }

            PRBatchItem_DTO PRB = new PRBatchItem_DTO();
            return Json(PRB);
        }
        Boolean SOToBatchValidation(List<SOToSaleInvoiceItem_DTO> Item_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                Double BatchAmount = 0;

                SI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_WH_Number);
                SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                SI_DTO.SI_BCH_Mode = Convert.ToInt32(13);
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                SI_DTO.SI_Id = 156;
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                Double Qty = Convert.ToDouble(Item.SII_Qty);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    BatchQty = Convert.ToDouble(DS.Tables[0].Rows[0][0].ToString());
                }

                if (Qty < 0)
                {
                    Qty = Qty * -1;
                }

                if (BatchQty == Qty) { }
                else
                {
                    Message += Item.SII_ITM_Code + " Batch Qty  Mismatched <br/>";
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

        [Route("sale/transactions/sale-invoice/so/buyer/address")]
        public IActionResult SOToSaleBuyerAddressID(String? Buyer, String ADTPNumber)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOSI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOSI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SOSI_DTO.SI_Id = 15;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
            SOToSaleInvoiceAddress SIA = new SOToSaleInvoiceAddress();
            SIA.BuyerAddressId = S_DL.BuyerAddressID(DS.Tables[0]);
            SIA.BuyerAddress = S_DL.BuyerAddress(DS.Tables[1]).FirstOrDefault();
            return Json(SIA);
        }

        [Route("sale/transactions/sale-invoice/buyer/so/address/addressid")]
        public IActionResult SOToSaleBuyerAddress(String? Buyer, String ADTPNumber, String AddressID)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOSI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOSI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SOSI_DTO.SI_ADD_AddressID = Convert.ToString(AddressID);
            SOSI_DTO.SI_Id = 16;
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
            var Address = S_DL.BuyerAddress(DS.Tables[0]).FirstOrDefault();
            return Json(Address);
        }






        //Sale Invoice SO To Invoice Edit
        [Route("sale/transactions/sale-invoice/so/{SI_No}/edit")]
        public IActionResult SOToEditSaleInvoice(String SI_No)
        {
            SITempClear("14");
            SOToSaleInvoiceGetData();
            String Active = GetSOSIEditData(SI_No);
            if (Active != "1")
            {
                return RedirectToAction("SaleInvoiceRegisterSummary");
            }
            ViewBag.SI_No = SI_No;

            return View(SOSIH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-invoice/so/{SI_No}/edit")]
        public IActionResult SOToEditSaleInvoice(SOToSaleInvoiceHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SOToSaleInvoiceHead_DTO S_Head_DTO = new SOToSaleInvoiceHead_DTO();

            List<SOToSaleInvoiceItem_DTO>? ITM_DTO = new List<SOToSaleInvoiceItem_DTO>();
            List<SOToSaleInvoiceIncome_DTO>? Income_DTO = new List<SOToSaleInvoiceIncome_DTO>();
            List<SOToSaleInvoiceIIncome_DTO>? ItemIncome_DTO = new List<SOToSaleInvoiceIIncome_DTO>();
            List<SOToSaleInvoiceAddress_DTO>? BuyerAddress_DTO = new List<SOToSaleInvoiceAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.InvoiceItem != null)
                ITM_DTO = S_DTO.InvoiceItem!.Where(K => K.SII_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SIH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SII_INC_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SIH_ADD_IsDeleted == 0).ToList();

            SOSI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SII_MS_Number) != Convert.ToInt64(S_DTO.SIH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SII_Qty) == 0 || Convert.ToDouble(x.SII_UnitPrice) == 0 || Convert.ToDouble(x.SII_Amount) == 0);

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
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SIH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SIH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.InvoiceItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        //if (SOToBatchValidation(ITM_DTO))
                        //{
                        using (var transaction = new TransactionScope())
                        {
                            try
                            {
                                SOSI_DTO.SI_Number = Convert.ToInt64(S_DTO.SIH_Number);
                                SOSI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd"));
                                SOSI_DTO.SI_InvoiceNo = Convert.ToString(S_DTO.SIH_InvoiceNo);
                                SOSI_DTO.SI_BUY_Number = Convert.ToInt64(S_DTO.SIH_BUY_Number);
                                SOSI_DTO.SI_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 1 : 0);
                                SOSI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                SOSI_DTO.SI_MS_Number = Convert.ToInt64(S_DTO.SIH_MS_Number);
                                SOSI_DTO.SI_TCT_Number = Convert.ToInt64(S_DTO.SIH_TCT_Number);
                                SOSI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                SOSI_DTO.SI_WHT_Number = Convert.ToInt64(S_DTO.SIH_WHT_Number);
                                SOSI_DTO.SI_ExchangeRate = Convert.ToDouble(S_DTO.SIH_ExchangeRate) == 0 ? "1" : S_DTO.SIH_ExchangeRate;
                                SOSI_DTO.SI_MaterialCost = Convert.ToDouble(S_DTO.SIH_MaterialCost);
                                SOSI_DTO.SI_ItemMiscIncome = Convert.ToDouble(S_DTO.SIH_ItemMiscIncome);
                                SOSI_DTO.SI_HeaderMiscIncome = Convert.ToDouble(S_DTO.SIH_HeaderMiscIncome);
                                SOSI_DTO.SI_GST_Amount = Convert.ToDouble(S_DTO.SIH_GST_Amount);
                                SOSI_DTO.SI_InvoiceAmount = Convert.ToDouble(S_DTO.SIH_InvoiceAmount);
                                SOSI_DTO.SI_WHT_Amount = Convert.ToDouble(S_DTO.SIH_WHT_Amount);
                                SOSI_DTO.SI_RoundOff = Convert.ToDouble(S_DTO.SIH_RoundOff);
                                SOSI_DTO.SI_BuyerReceivable = Convert.ToDouble(S_DTO.SIH_BuyerReceivable);
                                SOSI_DTO.SI_Id = 121;
                                DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                                String ItemDTO = string.Join(", ", ITM_DTO.Where(x => Convert.ToInt64(x.SII_Number) != 0).Select(x => x.SII_Number));
                                String ItemIncomeDTO = string.Join(", ", ItemIncome_DTO.Where(x => Convert.ToInt64(x.SII_INC_Number) != 0).Select(x => x.SII_INC_Number));
                                String IncomeDTO = string.Join(", ", Income_DTO.Where(x => Convert.ToInt64(x.SIH_INC_Number) != 0).Select(x => x.SIH_INC_Number));
                                String AddressDTO = string.Join(", ", BuyerAddress_DTO.Where(x => Convert.ToInt64(x.SIH_ADD_Number) != 0).Select(x => x.SIH_ADD_Number));

                                SOSI_DTO.SI_DeleteNumbers = Convert.ToString(ItemDTO);
                                SOSI_DTO.SI_Id = 101;
                                SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                                SOSI_DTO.SI_DeleteNumbers = Convert.ToString(IncomeDTO);
                                SOSI_DTO.SI_Id = 102;
                                SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                                SOSI_DTO.SI_DeleteNumbers = Convert.ToString(ItemIncomeDTO);
                                SOSI_DTO.SI_Id = 103;
                                SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                                SOSI_DTO.SI_DeleteNumbers = Convert.ToString(AddressDTO);
                                SOSI_DTO.SI_Id = 105;
                                SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                                foreach (var Item in ITM_DTO)
                                {
                                    DataSet D = new DataSet();
                                    SOSI_DTO.SI_SOH_Number = Convert.ToInt64(Item.SII_SOH_Number);
                                    SOSI_DTO.SI_SOI_Number = Convert.ToInt64(Item.SII_SOI_Number);
                                    SOSI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                    SOSI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_WH_Number);
                                    SOSI_DTO.SI_UoM_Number = Convert.ToInt64(Item.SII_UoM_Number);
                                    SOSI_DTO.SI_Qty = Convert.ToDouble(Item.SII_Qty);
                                    SOSI_DTO.SI_UnitPrice = Convert.ToDouble(Item.SII_UnitPrice);
                                    SOSI_DTO.SI_Amount = Convert.ToDouble(Item.SII_Amount);
                                    SOSI_DTO.SI_IncomeValue = Convert.ToDouble(Item.SII_IncomeValue);
                                    SOSI_DTO.SI_HSN_Number = Convert.ToInt64(Item.SII_HSN_Number);
                                    SOSI_DTO.SI_GST_Amount = Convert.ToDouble(Item.SII_GST_Amount);
                                    SOSI_DTO.SI_WHT_Percent = Convert.ToDouble(Item.SII_WHT_Percent);
                                    SOSI_DTO.SI_WHT_Amount = Convert.ToDouble(Item.SII_WHT_Amount);
                                    if (Item.SII_Number == 0)
                                    {
                                        SOSI_DTO.SI_Id = 22;
                                        D = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                                        var ItemIncome = ItemIncome_DTO.Where(x => (x.SII_INC_SOI_Number == Item.SII_SOI_Number && x.SII_INC_SOH_Number == Item.SII_SOH_Number));

                                        foreach (var ItemInc in ItemIncome)
                                        {
                                            SOSI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                            SOSI_DTO.SI_SOI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_SOI_INC_Number);
                                            SOSI_DTO.SI_INC_MIC_Number = Convert.ToInt64(ItemInc.SII_INC_MIC_Number);
                                            SOSI_DTO.SI_INC_Remarks = ItemInc.SII_INC_Remarks;
                                            SOSI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(ItemInc.SII_INC_OCRN_Number);
                                            SOSI_DTO.SI_INC_CM_Number = Convert.ToInt64(ItemInc.SII_INC_CM_Number);
                                            SOSI_DTO.SI_INC_IncomeBase = Convert.ToDouble(ItemInc.SII_INC_IncomeBase);
                                            SOSI_DTO.SI_INC_IncomeValue = Convert.ToDouble(ItemInc.SII_INC_IncomeValue);
                                            SOSI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(ItemInc.SII_INC_ALCT_Number);
                                            SOSI_DTO.SI_INC_LA_Number = Convert.ToInt64(ItemInc.SII_INC_LA_Number);
                                            if (ItemInc.SII_INC_Number == 0)
                                            {
                                                SOSI_DTO.SI_Id = 24;
                                            }
                                            else
                                            {
                                                SOSI_DTO.SI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_Number);
                                                SOSI_DTO.SI_Id = 124;
                                            }
                                            SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                        }
                                    }
                                    else
                                    {
                                        SOSI_DTO.SI_Number = Convert.ToInt64(Item.SII_Number);
                                        SOSI_DTO.SI_Id = 106;
                                        D = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);

                                        var ItemIncome = ItemIncome_DTO.Where(x => (x.SII_INC_SOI_Number == Item.SII_SOI_Number && x.SII_INC_SOH_Number == Item.SII_SOH_Number));

                                        foreach (var ItemInc in ItemIncome)
                                        {
                                            SOSI_DTO.SI_I_Number = Convert.ToInt64(Item.SII_Number);
                                            SOSI_DTO.SI_SOI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_SOI_INC_Number);
                                            SOSI_DTO.SI_INC_MIC_Number = Convert.ToInt64(ItemInc.SII_INC_MIC_Number);
                                            SOSI_DTO.SI_INC_Remarks = ItemInc.SII_INC_Remarks;
                                            SOSI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(ItemInc.SII_INC_OCRN_Number);
                                            SOSI_DTO.SI_INC_CM_Number = Convert.ToInt64(ItemInc.SII_INC_CM_Number);
                                            SOSI_DTO.SI_INC_IncomeBase = Convert.ToDouble(ItemInc.SII_INC_IncomeBase);
                                            SOSI_DTO.SI_INC_IncomeValue = Convert.ToDouble(ItemInc.SII_INC_IncomeValue);
                                            SOSI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(ItemInc.SII_INC_ALCT_Number);
                                            SOSI_DTO.SI_INC_LA_Number = Convert.ToInt64(ItemInc.SII_INC_LA_Number);
                                            if (ItemInc.SII_INC_Number == 0)
                                            {
                                                SOSI_DTO.SI_Id = 24;
                                            }
                                            else
                                            {
                                                SOSI_DTO.SI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_Number);
                                                SOSI_DTO.SI_Id = 124;
                                            }
                                            SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                        }
                                    }

                                    //SOSI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                    //SOSI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                    //SOSI_DTO.SI_BCH_Mode = Convert.ToInt32(13);
                                    //SOSI_DTO.SI_Id = 157;
                                    //DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                    //if (DS.Tables[0].Rows.Count > 0)
                                    //{
                                    //    DataTable dt = DS.Tables[0];
                                    //    foreach (DataRow row in dt.Rows)
                                    //    {
                                    //        SOSI_DTO.SI_BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                    //        SOSI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                    //        SOSI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                    //        SOSI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                    //        SOSI_DTO.SI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                    //        SOSI_DTO.SI_BCH_No = Convert.ToString(row["BCH_No"]);
                                    //        SOSI_DTO.SI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                    //        SOSI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                    //        SOSI_DTO.SI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                    //        SOSI_DTO.SI_Id = 25;
                                    //        SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                    //    }
                                    //}
                                }
                                foreach (var Income in Income_DTO)
                                {
                                    SOSI_DTO.SI_SOH_INC_Number = Convert.ToInt64(Income.SIH_INC_SOH_INC_Number);
                                    SOSI_DTO.SI_INC_MIC_Number = Convert.ToInt64(Income.SIH_INC_MIC_Number);
                                    SOSI_DTO.SI_INC_Remarks = Income.SIH_INC_Remarks;
                                    SOSI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(Income.SIH_INC_OCRN_Number);
                                    SOSI_DTO.SI_INC_CM_Number = Convert.ToInt64(Income.SIH_INC_CM_Number);
                                    SOSI_DTO.SI_INC_IncomeBase = Convert.ToDouble(Income.SIH_INC_IncomeBase);
                                    SOSI_DTO.SI_INC_IncomeValue = Convert.ToDouble(Income.SIH_INC_IncomeValue);
                                    SOSI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(Income.SIH_INC_ALCT_Number);
                                    SOSI_DTO.SI_INC_LA_Number = Convert.ToInt64(Income.SIH_INC_LA_Number);
                                    SOSI_DTO.SI_INC_CalculateGST = Convert.ToInt64(Income.SIH_INC_CalculateGST);
                                    SOSI_DTO.SI_INC_GST_Amount = Convert.ToDouble(Income.SIH_INC_GST_Amount);
                                    SOSI_DTO.SI_INC_SAC_Number = Convert.ToInt64(Income.SIH_INC_SAC_Number);
                                    SOSI_DTO.SI_INC_WHT_Percent = Convert.ToDouble(Income.SIH_INC_WHT_Percent);
                                    SOSI_DTO.SI_INC_WHT_Amount = Convert.ToDouble(Income.SIH_INC_WHT_Amount);
                                    if (Income.SIH_INC_Number == 0)
                                    {
                                        SOSI_DTO.SI_Id = 23;
                                    }
                                    else
                                    {
                                        SOSI_DTO.SI_INC_Number = Convert.ToInt64(Income.SIH_INC_Number);
                                        SOSI_DTO.SI_Id = 123;
                                    }
                                    SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                }
                                foreach (var BuyerAddress in BuyerAddress_DTO)
                                {
                                    SOSI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SIH_ADD_ADTP_Number);
                                    SOSI_DTO.SI_ADD_AddressID = Convert.ToString(BuyerAddress.SIH_ADD_AddressID);
                                    SOSI_DTO.SI_ADD_Address = Convert.ToString(BuyerAddress.SIH_ADD_Address);
                                    SOSI_DTO.SI_ADD_City = Convert.ToString(BuyerAddress.SIH_ADD_City);
                                    SOSI_DTO.SI_ADD_State = Convert.ToString(BuyerAddress.SIH_ADD_State);
                                    SOSI_DTO.SI_ADD_Country = Convert.ToString(BuyerAddress.SIH_ADD_Country);
                                    SOSI_DTO.SI_ADD_Pin = Convert.ToString(BuyerAddress.SIH_ADD_Pin);
                                    SOSI_DTO.SI_ADD_GSTIN = Convert.ToString(BuyerAddress.SIH_ADD_GSTIN);
                                    if (BuyerAddress.SIH_ADD_Number == 0)
                                    {
                                        SOSI_DTO.SI_Id = 26;
                                    }
                                    else
                                    {
                                        SOSI_DTO.SI_ADD_Number = Convert.ToInt64(BuyerAddress.SIH_ADD_Number);
                                        SOSI_DTO.SI_Id = 126;
                                    }
                                    SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
                                }

                                transaction.Complete();

                                S_Head_DTO.Reset();
                                Income_DTO = null;
                                ITM_DTO = null;
                                ItemIncome_DTO = null;
                                S_DTO.Reset();
                                Original_DTO = Help.JsonClone(S_DTO);

                                return RedirectToAction("SaleInvoiceRegisterSummary");
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
            }
            else if (Mode == "Direct")
            {
                SaleInvoiceHead_DTO SH_DTO = new SaleInvoiceHead_DTO();

                SH_DTO.SIH_InvoiceNo = S_DTO.SIH_InvoiceNo;
                SH_DTO.SIH_InvoiceDate = S_DTO.SIH_InvoiceDate;
                SH_DTO.SIH_BUY_Number = Convert.ToString(S_DTO.SIH_BUY_Number);
                SH_DTO.SIH_BUY_LOC_Number = Convert.ToString(S_DTO.SIH_BUY_LOC_Number);
                SH_DTO.SIH_ExportOrder = Convert.ToString(S_DTO.SIH_ExportOrder);
                SH_DTO.SIH_CUR_Number = Convert.ToString(S_DTO.SIH_CUR_Number);
                SH_DTO.SIH_MS_Number = Convert.ToString(S_DTO.SIH_MS_Number);
                SH_DTO.SIH_ExchangeRate = Convert.ToString(S_DTO.SIH_ExchangeRate);
                SH_DTO.SIH_TCT_Number = Convert.ToString(S_DTO.SIH_TCT_Number);
                SH_DTO.SIH_WHT_Number = Convert.ToString(S_DTO.SIH_WHT_Number);
                SH_DTO.SIH_WHT_Tax = Convert.ToString(S_DTO.SIH_WHT_Tax);
                SH_DTO.SIH_WHT_Percent = Convert.ToString(S_DTO.SIH_WHT_Percent);
                SH_DTO.SIH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SIH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("CreateSaleInvoice");
            }
            else if (Mode == "SOITEMTO")
            {
                //SOItemTOSaleInvoiceHead_DTO PHPO_DTO = new SOItemTOSaleInvoiceHead_DTO();

                //PHPO_DTO.SIH_InvoiceNo = SIH_DTO.SIH_InvoiceNo;
                //PHPO_DTO.SIH_InvoiceDate = SIH_DTO.SIH_InvoiceDate;
                //PHPO_DTO.SIH_DueDate = SIH_DTO.SIH_DueDate;
                //PHPO_DTO.SIH_SupplierInvoiceNo = SIH_DTO.SIH_SupplierInvoiceNo;
                //PHPO_DTO.SIH_SupplierInvoiceDate = SIH_DTO.SIH_SupplierInvoiceDate;
                //PHPO_DTO.SIH_Vendor_Number = Convert.ToString(SIH_DTO.SIH_Vendor_Number);
                //PHPO_DTO.SIH_VendorLocation = Convert.ToString(SIH_DTO.SIH_VendorLocation);
                //PHPO_DTO.SIH_CreditDays = Convert.ToString(SIH_DTO.SIH_CreditDays);
                //PHPO_DTO.SIH_PaymentBase = Convert.ToString(SIH_DTO.SIH_PaymentBase);
                //PHPO_DTO.SIH_Vendor = Convert.ToString(SIH_DTO.SIH_Vendor);
                //PHPO_DTO.SIH_ImportOrder = SIH_DTO.SIH_ImportOrder.ToString();
                //PHPO_DTO.SIH_Currency_Number = Convert.ToString(SIH_DTO.SIH_Currency_Number);
                //PHPO_DTO.SIH_MS_Number = Convert.ToString(SIH_DTO.SIH_MS_Number);
                //PHPO_DTO.SIH_ExchangeRate = Convert.ToString(SIH_DTO.SIH_ExchangeRate);
                //PHPO_DTO.SIH_TaxCluster_Number = Convert.ToString(SIH_DTO.SIH_TaxCluster_Number);
                //PHPO_DTO.SIH_WHT_Number = Convert.ToString(SIH_DTO.SIH_WHT_Number);
                //PHPO_DTO.SIH_WHT_Tax = Convert.ToString(SIH_DTO.SIH_WHT_Tax);
                //PHPO_DTO.SIH_WHT_Percent = Convert.ToString(SIH_DTO.SIH_WHT_Percent);
                //PHPO_DTO.SIH_Currency = Convert.ToString(SIH_DTO.SIH_Currency);
                //PHPO_DTO.SIH_DecimalPlaces = Convert.ToString(SIH_DTO.SIH_DecimalPlaces);

                //TempData["PHPO_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(PHPO_DTO);

                //return RedirectToAction("SOItemToSaleInvoiceCreate");
            }
            SaleGetData();
            return View(Original_DTO);
        }
        String GetSOSIEditData(String SI_No)
        {
            SOSI_DTO.SI_Number = Convert.ToInt64(SI_No);
            SOSI_DTO.SI_Id = 61;
            SOSI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            DS = SOSI_DAO.SOToSaleInvoiceDB(SOSI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SOSIH_DTO = S_DL.SOToSIHeadEditList(DS.Tables[0]).FirstOrDefault();
                SOSIH_DTO.InvoiceItem = S_DL.SOToSIItemEditList(DS.Tables[1]);
                SOSIH_DTO.Income = S_DL.SOToSIIncomeEditList(DS.Tables[2]);
                SOSIH_DTO.ItemIncome = S_DL.SOToSIIIncomeEditList(DS.Tables[3]);
                SOSIH_DTO.BuyerAddress = S_DL.SOToSIHAddressEditList(DS.Tables[5]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["SIH_Mode"].ToString();
                return "1";
            }
            else
            {
                return "0";
            }
        }







        //Sale Invoice SO Item to Invoice Create
        [Route("sale/transactions/sale-invoice/soitem/create")]
        public IActionResult SOItemToCreateSaleInvoice()
        {
            SITempClear("15");
            SOItemToSaleInvoiceHead_DTO SOISIH_DTO = new SOItemToSaleInvoiceHead_DTO();
            if (TempData["SHSO_DTO_Json"] is string PHPODto)
            {
                SOISIH_DTO = System.Text.Json.JsonSerializer.Deserialize<SOItemToSaleInvoiceHead_DTO>(PHPODto);
            }

            if (SOISIH_DTO.SIH_InvoiceDate == null)
            {
                SOISIH_DTO.SIH_InvoiceDate = DateTime.Now.ToString("dd-MMM-yy");
                SOISIH_DTO.SIH_InvoiceNo = OnSaleNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
            }
            else
            {
                SOISIH_DTO.SIH_InvoiceNo = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(SOISIH_DTO.SIH_InvoiceDate).ToString("yyyyMMdd")));
            }
            SOItemToSaleInvoiceGetData();

            return View(SOISIH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-invoice/soitem/create")]
        public IActionResult SOItemToCreateSaleInvoice(SOItemToSaleInvoiceHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SOItemToSaleInvoiceHead_DTO S_Head_DTO = new SOItemToSaleInvoiceHead_DTO();

            List<SOItemToSaleInvoiceItem_DTO>? ITM_DTO = new List<SOItemToSaleInvoiceItem_DTO>();
            List<SOItemToSaleInvoiceIncome_DTO>? Income_DTO = new List<SOItemToSaleInvoiceIncome_DTO>();
            List<SOItemToSaleInvoiceIIncome_DTO>? ItemIncome_DTO = new List<SOItemToSaleInvoiceIIncome_DTO>();
            List<SOItemToSaleInvoiceAddress_DTO>? BuyerAddress_DTO = new List<SOItemToSaleInvoiceAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.InvoiceItem != null)
                ITM_DTO = S_DTO.InvoiceItem!.Where(K => K.SII_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SIH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SII_INC_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SIH_ADD_IsDeleted == 0).ToList();

            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SII_MS_Number) != Convert.ToInt64(S_DTO.SIH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SII_Qty) == 0 || Convert.ToDouble(x.SII_UnitPrice) == 0 || Convert.ToDouble(x.SII_Amount) == 0);

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
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SIH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SIH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.InvoiceItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    S_Head_DTO.BuyerAddress = BuyerAddress_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        if (SOItemToBatchValidation(ITM_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    String SIHOrderNoOld = S_DTO.SIH_InvoiceNo;
                                    String SIHOrderNoNew = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd")));

                                    SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd"));
                                    SOISI_DTO.SI_InvoiceNo = SIHOrderNoNew;
                                    SOISI_DTO.SI_BUY_Number = Convert.ToInt64(S_DTO.SIH_BUY_Number);
                                    SOISI_DTO.SI_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 1 : 0);
                                    SOISI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                    SOISI_DTO.SI_MS_Number = Convert.ToInt64(S_DTO.SIH_MS_Number);
                                    SOISI_DTO.SI_TCT_Number = Convert.ToInt64(S_DTO.SIH_TCT_Number);
                                    SOISI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                    SOISI_DTO.SI_WHT_Number = Convert.ToInt64(S_DTO.SIH_WHT_Number);
                                    SOISI_DTO.SI_ExchangeRate = Convert.ToDouble(S_DTO.SIH_ExchangeRate) == 0 ? "1" : S_DTO.SIH_ExchangeRate;
                                    SOISI_DTO.SI_MaterialCost = Convert.ToDouble(S_DTO.SIH_MaterialCost);
                                    SOISI_DTO.SI_ItemMiscIncome = Convert.ToDouble(S_DTO.SIH_ItemMiscIncome);
                                    SOISI_DTO.SI_HeaderMiscIncome = Convert.ToDouble(S_DTO.SIH_HeaderMiscIncome);
                                    SOISI_DTO.SI_GST_Amount = Convert.ToDouble(S_DTO.SIH_GST_Amount);
                                    SOISI_DTO.SI_InvoiceAmount = Convert.ToDouble(S_DTO.SIH_InvoiceAmount);
                                    SOISI_DTO.SI_WHT_Amount = Convert.ToDouble(S_DTO.SIH_WHT_Amount);
                                    SOISI_DTO.SI_RoundOff = Convert.ToDouble(S_DTO.SIH_RoundOff);
                                    SOISI_DTO.SI_BuyerReceivable = Convert.ToDouble(S_DTO.SIH_BuyerReceivable);
                                    SOISI_DTO.SI_Id = 21;
                                    DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                                    OnSaleNumberGen(Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd")));

                                    if (DS.Tables[0].Rows.Count > 0)
                                    {
                                        SOISI_DTO.SI_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                        foreach (var Item in ITM_DTO)
                                        {
                                            DataSet D = new DataSet();
                                            SOISI_DTO.SI_SOH_Number = Convert.ToInt64(Item.SII_SOH_Number);
                                            SOISI_DTO.SI_SOI_Number = Convert.ToInt64(Item.SII_SOI_Number);
                                            SOISI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                            SOISI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_WH_Number);
                                            SOISI_DTO.SI_UoM_Number = Convert.ToInt64(Item.SII_UoM_Number);
                                            SOISI_DTO.SI_Qty = Convert.ToDouble(Item.SII_Qty);
                                            SOISI_DTO.SI_UnitPrice = Convert.ToDouble(Item.SII_UnitPrice);
                                            SOISI_DTO.SI_Amount = Convert.ToDouble(Item.SII_Amount);
                                            SOISI_DTO.SI_IncomeValue = Convert.ToDouble(Item.SII_IncomeValue);
                                            SOISI_DTO.SI_HSN_Number = Convert.ToInt64(Item.SII_HSN_Number);
                                            SOISI_DTO.SI_GST_Amount = Convert.ToDouble(Item.SII_GST_Amount);
                                            SOISI_DTO.SI_WHT_Percent = Convert.ToDouble(Item.SII_WHT_Percent);
                                            SOISI_DTO.SI_WHT_Amount = Convert.ToDouble(Item.SII_WHT_Amount);
                                            SOISI_DTO.SI_Id = 22;
                                            D = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (x.SII_INC_SOI_Number == Item.SII_SOI_Number && x.SII_INC_SOH_Number == Item.SII_SOH_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SOISI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SOISI_DTO.SI_SOI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_SOI_INC_Number);
                                                //SOISI_DTO.SI_ITM_Number = Convert.ToInt64(ItemInc.SII_INC_ITM_Number);
                                                SOISI_DTO.SI_INC_MIC_Number = Convert.ToInt64(ItemInc.SII_INC_MIC_Number);
                                                SOISI_DTO.SI_INC_Remarks = ItemInc.SII_INC_Remarks;
                                                SOISI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(ItemInc.SII_INC_OCRN_Number);
                                                SOISI_DTO.SI_INC_CM_Number = Convert.ToInt64(ItemInc.SII_INC_CM_Number);
                                                SOISI_DTO.SI_INC_IncomeBase = Convert.ToDouble(ItemInc.SII_INC_IncomeBase);
                                                SOISI_DTO.SI_INC_IncomeValue = Convert.ToDouble(ItemInc.SII_INC_IncomeValue);
                                                SOISI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(ItemInc.SII_INC_ALCT_Number);
                                                SOISI_DTO.SI_INC_LA_Number = Convert.ToInt64(ItemInc.SII_INC_LA_Number);
                                                SOISI_DTO.SI_Id = 24;
                                                SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                            }

                                            SOISI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                            SOISI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                            SOISI_DTO.SI_BCH_Mode = Convert.ToInt32(15);
                                            SOISI_DTO.SI_Id = 157;
                                            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                            if (DS.Tables[0].Rows.Count > 0)
                                            {
                                                DataTable dt = DS.Tables[0];
                                                foreach (DataRow row in dt.Rows)
                                                {
                                                    SOISI_DTO.SI_BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                                    SOISI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                                    SOISI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                                    SOISI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                    SOISI_DTO.SI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                                    SOISI_DTO.SI_BCH_No = Convert.ToString(row["BCH_No"]);
                                                    SOISI_DTO.SI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                                    SOISI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                                    SOISI_DTO.SI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                                    SOISI_DTO.SI_Id = 25;
                                                    SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                                }
                                            }
                                        }
                                        foreach (var Income in Income_DTO)
                                        {
                                            SOISI_DTO.SI_SOH_INC_Number = Convert.ToInt64(Income.SIH_INC_SOH_INC_Number);
                                            SOISI_DTO.SI_INC_MIC_Number = Convert.ToInt64(Income.SIH_INC_MIC_Number);
                                            SOISI_DTO.SI_INC_Remarks = Income.SIH_INC_Remarks;
                                            SOISI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(Income.SIH_INC_OCRN_Number);
                                            SOISI_DTO.SI_INC_CM_Number = Convert.ToInt64(Income.SIH_INC_CM_Number);
                                            SOISI_DTO.SI_INC_IncomeBase = Convert.ToDouble(Income.SIH_INC_IncomeBase);
                                            SOISI_DTO.SI_INC_IncomeValue = Convert.ToDouble(Income.SIH_INC_IncomeValue);
                                            SOISI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(Income.SIH_INC_ALCT_Number);
                                            SOISI_DTO.SI_INC_LA_Number = Convert.ToInt64(Income.SIH_INC_LA_Number);
                                            SOISI_DTO.SI_INC_CalculateGST = Convert.ToInt64(Income.SIH_INC_CalculateGST);
                                            SOISI_DTO.SI_INC_GST_Amount = Convert.ToDouble(Income.SIH_INC_GST_Amount);
                                            SOISI_DTO.SI_INC_SAC_Number = Convert.ToInt64(Income.SIH_INC_SAC_Number);
                                            SOISI_DTO.SI_INC_WHT_Percent = Convert.ToDouble(Income.SIH_INC_WHT_Percent);
                                            SOISI_DTO.SI_INC_WHT_Amount = Convert.ToDouble(Income.SIH_INC_WHT_Amount);
                                            SOISI_DTO.SI_Id = 23;
                                            SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                        }
                                        foreach (var BuyerAddress in BuyerAddress_DTO)
                                        {
                                            SOISI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SIH_ADD_ADTP_Number);
                                            SOISI_DTO.SI_ADD_AddressID = Convert.ToString(BuyerAddress.SIH_ADD_AddressID);
                                            SOISI_DTO.SI_ADD_Address = Convert.ToString(BuyerAddress.SIH_ADD_Address);
                                            SOISI_DTO.SI_ADD_City = Convert.ToString(BuyerAddress.SIH_ADD_City);
                                            SOISI_DTO.SI_ADD_State = Convert.ToString(BuyerAddress.SIH_ADD_State);
                                            SOISI_DTO.SI_ADD_Country = Convert.ToString(BuyerAddress.SIH_ADD_Country);
                                            SOISI_DTO.SI_ADD_Pin = Convert.ToString(BuyerAddress.SIH_ADD_Pin);
                                            SOISI_DTO.SI_ADD_GSTIN = Convert.ToString(BuyerAddress.SIH_ADD_GSTIN);
                                            SOISI_DTO.SI_Id = 26;
                                            SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                        }
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    if (SIHOrderNoOld != SIHOrderNoNew)
                                    {
                                        ViewBag.ErrorCode = 2;
                                        ViewBag.ErrorMessage = "Sale Invoice number " + SIHOrderNoOld + " used by another user. Next number will be allotted to you.";
                                    }
                                    return RedirectToAction("SOItemToCreateSaleInvoice");
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
                SaleInvoiceHead_DTO SH_DTO = new SaleInvoiceHead_DTO();

                SH_DTO.SIH_InvoiceNo = S_DTO.SIH_InvoiceNo;
                SH_DTO.SIH_InvoiceDate = S_DTO.SIH_InvoiceDate;
                SH_DTO.SIH_BUY_Number = Convert.ToString(S_DTO.SIH_BUY_Number);
                SH_DTO.SIH_BUY_LOC_Number = Convert.ToString(S_DTO.SIH_BUY_LOC_Number);
                SH_DTO.SIH_ExportOrder = Convert.ToString(S_DTO.SIH_ExportOrder);
                SH_DTO.SIH_CUR_Number = Convert.ToString(S_DTO.SIH_CUR_Number);
                SH_DTO.SIH_MS_Number = Convert.ToString(S_DTO.SIH_MS_Number);
                SH_DTO.SIH_ExchangeRate = Convert.ToString(S_DTO.SIH_ExchangeRate);
                SH_DTO.SIH_TCT_Number = Convert.ToString(S_DTO.SIH_TCT_Number);
                SH_DTO.SIH_WHT_Number = Convert.ToString(S_DTO.SIH_WHT_Number);
                SH_DTO.SIH_WHT_Tax = Convert.ToString(S_DTO.SIH_WHT_Tax);
                SH_DTO.SIH_WHT_Percent = Convert.ToString(S_DTO.SIH_WHT_Percent);
                SH_DTO.SIH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SIH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("CreateSaleInvoice");
            }
            else if (Mode == "SOITEMTO")
            {
                SaleInvoiceHead_DTO SH_DTO = new SaleInvoiceHead_DTO();

                SH_DTO.SIH_InvoiceNo = S_DTO.SIH_InvoiceNo;
                SH_DTO.SIH_InvoiceDate = S_DTO.SIH_InvoiceDate;
                SH_DTO.SIH_BUY_Number = Convert.ToString(S_DTO.SIH_BUY_Number);
                SH_DTO.SIH_BUY_LOC_Number = Convert.ToString(S_DTO.SIH_BUY_LOC_Number);
                SH_DTO.SIH_ExportOrder = Convert.ToString(S_DTO.SIH_ExportOrder);
                SH_DTO.SIH_CUR_Number = Convert.ToString(S_DTO.SIH_CUR_Number);
                SH_DTO.SIH_MS_Number = Convert.ToString(S_DTO.SIH_MS_Number);
                SH_DTO.SIH_ExchangeRate = Convert.ToString(S_DTO.SIH_ExchangeRate);
                SH_DTO.SIH_TCT_Number = Convert.ToString(S_DTO.SIH_TCT_Number);
                SH_DTO.SIH_WHT_Number = Convert.ToString(S_DTO.SIH_WHT_Number);
                SH_DTO.SIH_WHT_Tax = Convert.ToString(S_DTO.SIH_WHT_Tax);
                SH_DTO.SIH_WHT_Percent = Convert.ToString(S_DTO.SIH_WHT_Percent);
                SH_DTO.SIH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SIH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("SOItemToCreateSaleInvoice");
            }
            SOItemToSaleInvoiceGetData();
            return View(Original_DTO);
        }

        void SOItemToSaleInvoiceGetData()
        {
            SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            SOISI_DTO.SI_Id = 1;
            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

            ViewBag.IncomeCode = Help.GetCat(DS.Tables[0]);
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
            ViewBag.AddressType = Help.GetCat(DS.Tables[11]);
        }

        [Route("sale/transactions/sale-invoice/soitem/buyer")]
        public IActionResult SOItemToSaleBuyer(String? Buyer, String? Export, String? SIHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            if (Export == null)
            {
                Export = "";
            }
            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOISI_DTO.SI_ExportOrder = Convert.ToInt16(Convert.ToBoolean(Export) == true ? 2 : 1);
            SOISI_DTO.SI_ITM_Code = Convert.ToString(Buyer).Trim();
            SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOISI_DTO.SI_Id = 5;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
            var Ven = S_DL.BuyerList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("sale/transactions/sale-invoice/soitem/gst")]
        public String SOItemToSaleInvoiceGst(String? Cluster, String? SIHDate, String? HSN, String? BaseAmount)
        {
            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOISI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SOISI_DTO.SI_HSN_Number = Convert.ToInt64(HSN);
            SOISI_DTO.SI_Id = 6;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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

        [Route("sale/transactions/sale-invoice/soitem/gst/view")]
        public IActionResult SOItemToSaleInvoiceGstView(String? Cluster, String? SIHDate, String? HSN, String? BaseAmount)
        {
            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOISI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SOISI_DTO.SI_HSN_Number = Convert.ToInt64(HSN);
            SOISI_DTO.SI_Id = 9;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleInvoiceGst> PurGST = new List<SaleInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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
                        new SaleInvoiceGst
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

        [Route("sale/transactions/sale-invoice/soitem/wht")]
        public IActionResult SOItemToSaleInvoiceWHT(String? Buyer, String? WHTNumber, String? SIHDate)
        {
            if (WHTNumber == null)
            {
                WHTNumber = "0";
            }
            if (Buyer == null)
            {
                Buyer = "0";
            }
            if (SIHDate == null)
            {
                SIHDate = "0";
            }
            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOISI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOISI_DTO.SI_WHT_Number = Convert.ToInt64(WHTNumber);
            SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOISI_DTO.SI_Id = 7;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
            var WHT = S_DL.SaleInvWHT(DS.Tables[0]).FirstOrDefault();
            return Json(WHT);
        }

        [Route("sale/transactions/sale-invoice/soitem/cluster")]
        public IActionResult SOItemToSaleInvoiceCluster(String? Buyer, String? Cluster)
        {
            if (Cluster == null)
            {
                Cluster = "";
            }

            SOISI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOISI_DTO.SI_Search = Cluster;
            SOISI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOISI_DTO.SI_Id = 8;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
            var InvCluster = S_DL.SaleCluster(DS.Tables[0]);
            return Json(InvCluster);
        }

        [Route("sale/transactions/sale-invoice/soitem/order")]
        public IActionResult SOItemToSaleInvoiceSO(String? Buyer, String? Export, String? MSNumber, String? SIHDate)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }

            SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOISI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOISI_DTO.SI_MS_Number = Convert.ToInt64(MSNumber);
            SOISI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOISI_DTO.SI_ExportOrder = Convert.ToInt32(Convert.ToBoolean(Export) == true ? 1 : 0);
            SOISI_DTO.SI_Id = 11;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
            var POOrder = S_DL.SOItemToSIOrder(DS.Tables[0]);
            return Json(POOrder);
        }

        [Route("sale/transactions/sale-invoice/soitem/order/item")]
        public IActionResult SOItemToSaleInvoiceSOItem(String? SONumber)
        {
            if (SONumber == null)
            {
                SONumber = "";
            }

            SOISI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOISI_DTO.SI_Search = SONumber;
            SOISI_DTO.SI_Id = 12;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

            SOItemToSI_DTO SOISI = new SOItemToSI_DTO();
            var SOItem = S_DL.SOItemToSIOrderItem(DS.Tables[0]);
            var SOIncome = S_DL.SOItemToSIIncome(DS.Tables[1]);
            var SOItemIncome = S_DL.SOItemToSIIIncome(DS.Tables[2]);

            SOISI.SOItems = SOItem;
            SOISI.SOIncomes = SOIncome;
            SOISI.SOItemIncomes = SOItemIncome;

            return Json(SOISI);
        }

        [Route("sale/transactions/sale-invoice/soitem/income/gst")]
        public String SOItemToSaleInvoiceHeaderGst(String? Cluster, String? SIHDate, String? SAC, String? BaseAmount)
        {
            SOISI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOISI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SOISI_DTO.SI_INC_SAC_Number = Convert.ToInt64(SAC);
            SOISI_DTO.SI_Id = 13;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            var GroupTotals = new Dictionary<Int64, Double>();

            var TaxIndex = S_DL.SaleInvGst(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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

        [Route("sale/transactions/sale-invoice/soitem/income/gst/view")]
        public IActionResult SOItemToSaleInvoiceGstHeaderView(String? Cluster, String? SIHDate, String? SAC, String? BaseAmount)
        {
            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(SIHDate).ToString("yyyyMMdd"));
            SOISI_DTO.SI_TCT_Number = Convert.ToInt64(Cluster);
            SOISI_DTO.SI_INC_SAC_Number = Convert.ToInt64(SAC);
            SOISI_DTO.SI_Id = 14;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

            Double BaseValue = Convert.ToDouble(BaseAmount);

            List<SaleInvoiceGst> PurGST = new List<SaleInvoiceGst>();

            var GroupTotals = new Dictionary<Int64, Double>();
            var TaxIndex = S_DL.SaleInvGstView(DS.Tables[0]).GroupBy(gst => gst.TaxIndex);

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
                        new SaleInvoiceGst
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

        [Route("sale/transactions/sale-invoice/soitem/batch/get")]
        public IActionResult SOItemToSaleInvoiceBatchGet(String? ItemNumber, String? Index, String? Warehouse)
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

            SOISI_DTO.SI_ITM_Number = Convert.ToInt64(ItemNumber);
            SOISI_DTO.SI_WH_Number = Convert.ToInt64(Warehouse);
            SOISI_DTO.SI_BCH_Index = Convert.ToInt32(Index);
            SOISI_DTO.SI_BCH_Mode = Convert.ToInt32(15);
            SOISI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
            SOISI_DTO.SI_Id = 151;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

            SIBatchItem_DTO PRB = new SIBatchItem_DTO();
            PRB.SIBatch = S_DL.SIBatchList(DS.Tables[0]);
            PRB.SIView = S_DL.SIBatchOverallList(DS.Tables[1]);
            return Json(PRB);
        }

        [Route("sale/transactions/sale-invoice/soitem/batch/post")]
        [HttpPost]
        public IActionResult SOItemToSaleInvoiceBatchGet([FromBody] SOItemToSaleInvoiceItemBatch ItemBatch)
        {
            SOISI_DTO.SI_BCH_Mode = Convert.ToInt32(15);
            SOISI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);

            foreach (var Item in ItemBatch.ItemBatch)
            {
                SOISI_DTO.SI_BCH_Number = Convert.ToInt64(Item.SII_BCH_Number);
                SOISI_DTO.SI_BCH_BCH_Number = Convert.ToInt64(Item.SII_BCH_BCH_Number);
                SOISI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_BCH_ITM_Number);
                SOISI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_BCH_ITM_Index);
                SOISI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_BCH_WH_Number);
                SOISI_DTO.SI_BCH_Date = Convert.ToString(Convert.ToDateTime(Item.SII_BCH_Date).ToString("yyyyMMdd"));
                SOISI_DTO.SI_BCH_No = Convert.ToString(Item.SII_BCH_No);
                SOISI_DTO.SI_BCH_Qty = Convert.ToDouble(Item.SII_BCH_Qty);
                SOISI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(Item.SII_BCH_UnitPrice);
                SOISI_DTO.SI_BCH_Value = Convert.ToDouble(Item.SII_BCH_Value);
                SOISI_DTO.SI_Id = 153;
                DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                if (DS.Tables[0].Rows.Count > 0)
                {
                    if (Item.SII_BCH_Qty > 0)
                    {
                        SOISI_DTO.SI_Id = 154;
                        SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                    }
                    else
                    {
                        SOISI_DTO.SI_Id = 155;
                        SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                    }
                }
                else
                {
                    if (Item.SII_BCH_Qty > 0)
                    {
                        SOISI_DTO.SI_Id = 152;
                        SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                    }
                }
            }

            PRBatchItem_DTO PRB = new PRBatchItem_DTO();
            return Json(PRB);
        }
        Boolean SOItemToBatchValidation(List<SOItemToSaleInvoiceItem_DTO> Item_DTO)
        {
            Boolean Result = true;
            String Message = "";
            foreach (var Item in Item_DTO)
            {
                DataSet D = new DataSet();
                Double BatchQty = 0;
                Double BatchAmount = 0;

                SI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_WH_Number);
                SI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                SI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                SI_DTO.SI_BCH_Mode = Convert.ToInt32(15);
                SI_DTO.SI_CreatorCode = Convert.ToInt32(UserCode);
                SI_DTO.SI_Id = 156;
                DS = SI_DAO.SaleInvoiceDB(SI_DTO);

                Double Qty = Convert.ToDouble(Item.SII_Qty);
                if (DS.Tables[0].Rows.Count > 0)
                {
                    BatchQty = Convert.ToDouble(DS.Tables[0].Rows[0][0].ToString());
                }

                if (Qty < 0)
                {
                    Qty = Qty * -1;
                }

                if (BatchQty == Qty) { }
                else
                {
                    Message += Item.SII_ITM_Code + " Batch Qty  Mismatched <br/>";
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

        [Route("sale/transactions/sale-invoice/soitem/buyer/address")]
        public IActionResult SOItemToSaleBuyerAddressID(String? Buyer, String ADTPNumber)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOISI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOISI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SOISI_DTO.SI_Id = 15;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
            SOItemToSaleInvoiceAddress SIA = new SOItemToSaleInvoiceAddress();
            SIA.BuyerAddressId = S_DL.BuyerAddressID(DS.Tables[0]);
            SIA.BuyerAddress = S_DL.BuyerAddress(DS.Tables[1]).FirstOrDefault();
            return Json(SIA);
        }

        [Route("sale/transactions/sale-invoice/buyer/soitem/address/addressid")]
        public IActionResult SOItemToSaleBuyerAddress(String? Buyer, String ADTPNumber, String AddressID)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            SOISI_DTO.SI_BUY_Number = Convert.ToInt64(Buyer);
            SOISI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SOISI_DTO.SI_ADD_AddressID = Convert.ToString(AddressID);
            SOISI_DTO.SI_Id = 16;
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
            var Address = S_DL.BuyerAddress(DS.Tables[0]).FirstOrDefault();
            return Json(Address);
        }









        //Sale Invoice SO Item to Invoice Edit
        [Route("sale/transactions/sale-invoice/soitem/{SI_No}/edit")]
        public IActionResult SOItemToEditSaleInvoice(Int64 SI_No)
        {
            SITempClear("15");
            SOItemToSaleInvoiceGetData();
            String Active = GetSOSIItemEditData(SI_No);
            if (Active != "1")
            {
                return RedirectToAction("SaleInvoiceRegisterSummary");
            }
            ViewBag.SI_No = SI_No;

            return View(SOISIH_DTO);
        }
        String GetSOSIItemEditData(Int64 SI_No)
        {
            SOISI_DTO.SI_Number = SI_No;
            SOISI_DTO.SI_Id = 61;
            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SOISIH_DTO = S_DL.SOItemToSIHeadEditList(DS.Tables[0]).FirstOrDefault();
                SOISIH_DTO.InvoiceItem = S_DL.SOItemToSIItemEditList(DS.Tables[1]);
                SOISIH_DTO.Income = S_DL.SOItemToSIIncomeEditList(DS.Tables[2]);
                SOISIH_DTO.ItemIncome = S_DL.SOItemToSIIIncomeEditList(DS.Tables[3]);
                SOISIH_DTO.BuyerAddress = S_DL.SOItemToSIHAddressEditList(DS.Tables[5]);

                ViewBag.Mode = DS.Tables[0].Rows[0]["SIH_Mode"].ToString();
                return "1";
            }
            else
            {
                return "0";
            }
        }


        [HttpPost]
        [Route("sale/transactions/sale-invoice/soitem/{SI_No}/edit")]
        public IActionResult SOItemToEditSaleInvoice(SOItemToSaleInvoiceHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SOItemToSaleInvoiceHead_DTO S_Head_DTO = new SOItemToSaleInvoiceHead_DTO();

            List<SOItemToSaleInvoiceItem_DTO>? ITM_DTO = new List<SOItemToSaleInvoiceItem_DTO>();
            List<SOItemToSaleInvoiceIncome_DTO>? Income_DTO = new List<SOItemToSaleInvoiceIncome_DTO>();
            List<SOItemToSaleInvoiceIIncome_DTO>? ItemIncome_DTO = new List<SOItemToSaleInvoiceIIncome_DTO>();
            List<SOItemToSaleInvoiceAddress_DTO>? BuyerAddress_DTO = new List<SOItemToSaleInvoiceAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.InvoiceItem != null)
                ITM_DTO = S_DTO.InvoiceItem!.Where(K => K.SII_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SIH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SII_INC_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SIH_ADD_IsDeleted == 0).ToList();

            SOISI_DTO.SI_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SII_MS_Number) != Convert.ToInt64(S_DTO.SIH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SII_Qty) == 0 || Convert.ToDouble(x.SII_UnitPrice) == 0 || Convert.ToDouble(x.SII_Amount) == 0);

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
                else if (ITM_DTO.Count == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Item Atleast, One Row Required";
                }
                else if (Convert.ToInt32(S_DTO.SIH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SIH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.InvoiceItem = ITM_DTO;
                    S_Head_DTO.Income = Income_DTO;
                    S_Head_DTO.ItemIncome = ItemIncome_DTO;
                    IsValid = TryValidateModel(S_Head_DTO);

                    if (IsValid)
                    {
                        //if (SOItemToBatchValidation(ITM_DTO))
                        {
                            using (var transaction = new TransactionScope())
                            {
                                try
                                {
                                    SOISI_DTO.SI_Number = Convert.ToInt64(S_DTO.SIH_Number);
                                    SOISI_DTO.SI_InvoiceDate = Convert.ToInt32(Convert.ToDateTime(S_DTO.SIH_InvoiceDate).ToString("yyyyMMdd"));
                                    SOISI_DTO.SI_InvoiceNo = Convert.ToString(S_DTO.SIH_InvoiceNo);
                                    SOISI_DTO.SI_BUY_Number = Convert.ToInt64(S_DTO.SIH_BUY_Number);
                                    SOISI_DTO.SI_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SIH_ExportOrder) ? 1 : 0);
                                    SOISI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                    SOISI_DTO.SI_MS_Number = Convert.ToInt64(S_DTO.SIH_MS_Number);
                                    SOISI_DTO.SI_TCT_Number = Convert.ToInt64(S_DTO.SIH_TCT_Number);
                                    SOISI_DTO.SI_CUR_Number = Convert.ToInt64(S_DTO.SIH_CUR_Number);
                                    SOISI_DTO.SI_WHT_Number = Convert.ToInt64(S_DTO.SIH_WHT_Number);
                                    SOISI_DTO.SI_ExchangeRate = Convert.ToDouble(S_DTO.SIH_ExchangeRate) == 0 ? "1" : S_DTO.SIH_ExchangeRate;
                                    SOISI_DTO.SI_MaterialCost = Convert.ToDouble(S_DTO.SIH_MaterialCost);
                                    SOISI_DTO.SI_ItemMiscIncome = Convert.ToDouble(S_DTO.SIH_ItemMiscIncome);
                                    SOISI_DTO.SI_HeaderMiscIncome = Convert.ToDouble(S_DTO.SIH_HeaderMiscIncome);
                                    SOISI_DTO.SI_GST_Amount = Convert.ToDouble(S_DTO.SIH_GST_Amount);
                                    SOISI_DTO.SI_InvoiceAmount = Convert.ToDouble(S_DTO.SIH_InvoiceAmount);
                                    SOISI_DTO.SI_WHT_Amount = Convert.ToDouble(S_DTO.SIH_WHT_Amount);
                                    SOISI_DTO.SI_RoundOff = Convert.ToDouble(S_DTO.SIH_RoundOff);
                                    SOISI_DTO.SI_BuyerReceivable = Convert.ToDouble(S_DTO.SIH_BuyerReceivable);
                                    SOISI_DTO.SI_Id = 121;
                                    DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                                    String ItemDTO = string.Join(", ", ITM_DTO.Where(x => Convert.ToInt64(x.SII_Number) != 0).Select(x => x.SII_Number));
                                    String ItemIncomeDTO = string.Join(", ", ItemIncome_DTO.Where(x => Convert.ToInt64(x.SII_INC_Number) != 0).Select(x => x.SII_INC_Number));
                                    String IncomeDTO = string.Join(", ", Income_DTO.Where(x => Convert.ToInt64(x.SIH_INC_Number) != 0).Select(x => x.SIH_INC_Number));
                                    String AddressDTO = string.Join(", ", BuyerAddress_DTO.Where(x => Convert.ToInt64(x.SIH_ADD_Number) != 0).Select(x => x.SIH_ADD_Number));

                                    SOISI_DTO.SI_DeleteNumbers = Convert.ToString(ItemDTO);
                                    SOISI_DTO.SI_Id = 101;
                                    SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                                    SOISI_DTO.SI_DeleteNumbers = Convert.ToString(IncomeDTO);
                                    SOISI_DTO.SI_Id = 102;
                                    SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                                    SOISI_DTO.SI_DeleteNumbers = Convert.ToString(ItemIncomeDTO);
                                    SOISI_DTO.SI_Id = 103;
                                    SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                                    SOISI_DTO.SI_DeleteNumbers = Convert.ToString(AddressDTO);
                                    SOISI_DTO.SI_Id = 105;
                                    SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                                    foreach (var Item in ITM_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        SOISI_DTO.SI_SOH_Number = Convert.ToInt64(Item.SII_SOH_Number);
                                        SOISI_DTO.SI_SOI_Number = Convert.ToInt64(Item.SII_SOI_Number);
                                        SOISI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                        SOISI_DTO.SI_WH_Number = Convert.ToInt64(Item.SII_WH_Number);
                                        SOISI_DTO.SI_UoM_Number = Convert.ToInt64(Item.SII_UoM_Number);
                                        SOISI_DTO.SI_Qty = Convert.ToDouble(Item.SII_Qty);
                                        SOISI_DTO.SI_UnitPrice = Convert.ToDouble(Item.SII_UnitPrice);
                                        SOISI_DTO.SI_Amount = Convert.ToDouble(Item.SII_Amount);
                                        SOISI_DTO.SI_IncomeValue = Convert.ToDouble(Item.SII_IncomeValue);
                                        SOISI_DTO.SI_HSN_Number = Convert.ToInt64(Item.SII_HSN_Number);
                                        SOISI_DTO.SI_GST_Amount = Convert.ToDouble(Item.SII_GST_Amount);
                                        SOISI_DTO.SI_WHT_Percent = Convert.ToDouble(Item.SII_WHT_Percent);
                                        SOISI_DTO.SI_WHT_Amount = Convert.ToDouble(Item.SII_WHT_Amount);
                                        if (Item.SII_Number == 0)
                                        {
                                            SOISI_DTO.SI_Id = 22;
                                            D = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (x.SII_INC_SOI_Number == Item.SII_SOI_Number && x.SII_INC_SOH_Number == Item.SII_SOH_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SOISI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                                SOISI_DTO.SI_SOI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_SOI_INC_Number);
                                                SOISI_DTO.SI_INC_MIC_Number = Convert.ToInt64(ItemInc.SII_INC_MIC_Number);
                                                SOISI_DTO.SI_INC_Remarks = ItemInc.SII_INC_Remarks;
                                                SOISI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(ItemInc.SII_INC_OCRN_Number);
                                                SOISI_DTO.SI_INC_CM_Number = Convert.ToInt64(ItemInc.SII_INC_CM_Number);
                                                SOISI_DTO.SI_INC_IncomeBase = Convert.ToDouble(ItemInc.SII_INC_IncomeBase);
                                                SOISI_DTO.SI_INC_IncomeValue = Convert.ToDouble(ItemInc.SII_INC_IncomeValue);
                                                SOISI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(ItemInc.SII_INC_ALCT_Number);
                                                SOISI_DTO.SI_INC_LA_Number = Convert.ToInt64(ItemInc.SII_INC_LA_Number);
                                                if (ItemInc.SII_INC_Number == 0)
                                                {
                                                    SOISI_DTO.SI_Id = 24;
                                                }
                                                else
                                                {
                                                    SOISI_DTO.SI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_Number);
                                                    SOISI_DTO.SI_Id = 124;
                                                }
                                                SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                            }
                                        }
                                        else
                                        {
                                            SOISI_DTO.SI_I_Number = Convert.ToInt64(Item.SII_Number);
                                            SOISI_DTO.SI_Id = 106;
                                            D = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);

                                            var ItemIncome = ItemIncome_DTO.Where(x => (x.SII_INC_SOI_Number == Item.SII_SOI_Number && x.SII_INC_SOH_Number == Item.SII_SOH_Number));

                                            foreach (var ItemInc in ItemIncome)
                                            {
                                                SOISI_DTO.SI_I_Number = Convert.ToInt64(Item.SII_Number);
                                                SOISI_DTO.SI_SOI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_SOI_INC_Number);
                                                SOISI_DTO.SI_INC_MIC_Number = Convert.ToInt64(ItemInc.SII_INC_MIC_Number);
                                                SOISI_DTO.SI_INC_Remarks = ItemInc.SII_INC_Remarks;
                                                SOISI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(ItemInc.SII_INC_OCRN_Number);
                                                SOISI_DTO.SI_INC_CM_Number = Convert.ToInt64(ItemInc.SII_INC_CM_Number);
                                                SOISI_DTO.SI_INC_IncomeBase = Convert.ToDouble(ItemInc.SII_INC_IncomeBase);
                                                SOISI_DTO.SI_INC_IncomeValue = Convert.ToDouble(ItemInc.SII_INC_IncomeValue);
                                                SOISI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(ItemInc.SII_INC_ALCT_Number);
                                                SOISI_DTO.SI_INC_LA_Number = Convert.ToInt64(ItemInc.SII_INC_LA_Number);
                                                if (ItemInc.SII_INC_Number == 0)
                                                {
                                                    SOISI_DTO.SI_Id = 24;
                                                }
                                                else
                                                {
                                                    SOISI_DTO.SI_INC_Number = Convert.ToInt64(ItemInc.SII_INC_Number);
                                                    SOISI_DTO.SI_Id = 124;
                                                }
                                                SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                            }
                                        }

                                        //SOISI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                        //SOISI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                        //SOISI_DTO.SI_BCH_Mode = Convert.ToInt32(15);
                                        //SOISI_DTO.SI_Id = 157;
                                        //DS = SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                        //if (DS.Tables[0].Rows.Count > 0)
                                        //{
                                        //    DataTable dt = DS.Tables[0];
                                        //    foreach (DataRow row in dt.Rows)
                                        //    {
                                        //        SOISI_DTO.SI_BCH_Number = Convert.ToInt64(row["BCH_ICB_Number"]);
                                        //        SOISI_DTO.SI_ITM_Number = Convert.ToInt64(Item.SII_ITM_Number);
                                        //        SOISI_DTO.SI_BCH_Index = Convert.ToInt32(Item.SII_Index);
                                        //        SOISI_DTO.SI_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                        //        SOISI_DTO.SI_BCH_Date = Convert.ToString(row["BCH_Date"]);
                                        //        SOISI_DTO.SI_BCH_No = Convert.ToString(row["BCH_No"]);
                                        //        SOISI_DTO.SI_BCH_Qty = Convert.ToDouble(row["BCH_Qty"]);
                                        //        SOISI_DTO.SI_BCH_UnitPrice = Convert.ToDouble(row["BCH_UnitPrice"]);
                                        //        SOISI_DTO.SI_BCH_Value = Convert.ToDouble(row["BCH_Value"]);
                                        //        SOISI_DTO.SI_Id = 25;
                                        //        SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                        //    }
                                        //}
                                    }
                                    foreach (var Income in Income_DTO)
                                    {
                                        SOISI_DTO.SI_SOH_INC_Number = Convert.ToInt64(Income.SIH_INC_SOH_INC_Number);
                                        SOISI_DTO.SI_INC_MIC_Number = Convert.ToInt64(Income.SIH_INC_MIC_Number);
                                        SOISI_DTO.SI_INC_Remarks = Income.SIH_INC_Remarks;
                                        SOISI_DTO.SI_INC_OCRN_Number = Convert.ToInt64(Income.SIH_INC_OCRN_Number);
                                        SOISI_DTO.SI_INC_CM_Number = Convert.ToInt64(Income.SIH_INC_CM_Number);
                                        SOISI_DTO.SI_INC_IncomeBase = Convert.ToDouble(Income.SIH_INC_IncomeBase);
                                        SOISI_DTO.SI_INC_IncomeValue = Convert.ToDouble(Income.SIH_INC_IncomeValue);
                                        SOISI_DTO.SI_INC_ALCT_Number = Convert.ToInt64(Income.SIH_INC_ALCT_Number);
                                        SOISI_DTO.SI_INC_LA_Number = Convert.ToInt64(Income.SIH_INC_LA_Number);
                                        SOISI_DTO.SI_INC_CalculateGST = Convert.ToInt64(Income.SIH_INC_CalculateGST);
                                        SOISI_DTO.SI_INC_GST_Amount = Convert.ToDouble(Income.SIH_INC_GST_Amount);
                                        SOISI_DTO.SI_INC_SAC_Number = Convert.ToInt64(Income.SIH_INC_SAC_Number);
                                        SOISI_DTO.SI_INC_WHT_Percent = Convert.ToDouble(Income.SIH_INC_WHT_Percent);
                                        SOISI_DTO.SI_INC_WHT_Amount = Convert.ToDouble(Income.SIH_INC_WHT_Amount);
                                        if (Income.SIH_INC_Number == 0)
                                        {
                                            SOISI_DTO.SI_Id = 23;
                                        }
                                        else
                                        {
                                            SOISI_DTO.SI_INC_Number = Convert.ToInt64(Income.SIH_INC_Number);
                                            SOISI_DTO.SI_Id = 123;
                                        }
                                        SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                    }
                                    foreach (var BuyerAddress in BuyerAddress_DTO)
                                    {
                                        SOISI_DTO.SI_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SIH_ADD_ADTP_Number);
                                        SOISI_DTO.SI_ADD_AddressID = Convert.ToString(BuyerAddress.SIH_ADD_AddressID);
                                        SOISI_DTO.SI_ADD_Address = Convert.ToString(BuyerAddress.SIH_ADD_Address);
                                        SOISI_DTO.SI_ADD_City = Convert.ToString(BuyerAddress.SIH_ADD_City);
                                        SOISI_DTO.SI_ADD_State = Convert.ToString(BuyerAddress.SIH_ADD_State);
                                        SOISI_DTO.SI_ADD_Country = Convert.ToString(BuyerAddress.SIH_ADD_Country);
                                        SOISI_DTO.SI_ADD_Pin = Convert.ToString(BuyerAddress.SIH_ADD_Pin);
                                        SOISI_DTO.SI_ADD_GSTIN = Convert.ToString(BuyerAddress.SIH_ADD_GSTIN);
                                        if (BuyerAddress.SIH_ADD_Number == 0)
                                        {
                                            SOISI_DTO.SI_Id = 26;
                                        }
                                        else
                                        {
                                            SOISI_DTO.SI_ADD_Number = Convert.ToInt64(BuyerAddress.SIH_ADD_Number);
                                            SOISI_DTO.SI_Id = 126;
                                        }
                                        SOISI_DAO.SOItemToSaleInvoiceDB(SOISI_DTO);
                                    }

                                    transaction.Complete();

                                    S_Head_DTO.Reset();
                                    Income_DTO = null;
                                    ITM_DTO = null;
                                    ItemIncome_DTO = null;
                                    S_DTO.Reset();
                                    Original_DTO = Help.JsonClone(S_DTO);

                                    return RedirectToAction("SaleInvoiceRegisterSummary");
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
                SaleInvoiceHead_DTO SH_DTO = new SaleInvoiceHead_DTO();

                SH_DTO.SIH_InvoiceNo = S_DTO.SIH_InvoiceNo;
                SH_DTO.SIH_InvoiceDate = S_DTO.SIH_InvoiceDate;
                SH_DTO.SIH_BUY_Number = Convert.ToString(S_DTO.SIH_BUY_Number);
                SH_DTO.SIH_BUY_LOC_Number = Convert.ToString(S_DTO.SIH_BUY_LOC_Number);
                SH_DTO.SIH_ExportOrder = Convert.ToString(S_DTO.SIH_ExportOrder);
                SH_DTO.SIH_CUR_Number = Convert.ToString(S_DTO.SIH_CUR_Number);
                SH_DTO.SIH_MS_Number = Convert.ToString(S_DTO.SIH_MS_Number);
                SH_DTO.SIH_ExchangeRate = Convert.ToString(S_DTO.SIH_ExchangeRate);
                SH_DTO.SIH_TCT_Number = Convert.ToString(S_DTO.SIH_TCT_Number);
                SH_DTO.SIH_WHT_Number = Convert.ToString(S_DTO.SIH_WHT_Number);
                SH_DTO.SIH_WHT_Tax = Convert.ToString(S_DTO.SIH_WHT_Tax);
                SH_DTO.SIH_WHT_Percent = Convert.ToString(S_DTO.SIH_WHT_Percent);
                SH_DTO.SIH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SIH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("CreateSaleInvoice");
            }
            else if (Mode == "SOITEMTO")
            {
                SaleInvoiceHead_DTO SH_DTO = new SaleInvoiceHead_DTO();

                SH_DTO.SIH_InvoiceNo = S_DTO.SIH_InvoiceNo;
                SH_DTO.SIH_InvoiceDate = S_DTO.SIH_InvoiceDate;
                SH_DTO.SIH_BUY_Number = Convert.ToString(S_DTO.SIH_BUY_Number);
                SH_DTO.SIH_BUY_LOC_Number = Convert.ToString(S_DTO.SIH_BUY_LOC_Number);
                SH_DTO.SIH_ExportOrder = Convert.ToString(S_DTO.SIH_ExportOrder);
                SH_DTO.SIH_CUR_Number = Convert.ToString(S_DTO.SIH_CUR_Number);
                SH_DTO.SIH_MS_Number = Convert.ToString(S_DTO.SIH_MS_Number);
                SH_DTO.SIH_ExchangeRate = Convert.ToString(S_DTO.SIH_ExchangeRate);
                SH_DTO.SIH_TCT_Number = Convert.ToString(S_DTO.SIH_TCT_Number);
                SH_DTO.SIH_WHT_Number = Convert.ToString(S_DTO.SIH_WHT_Number);
                SH_DTO.SIH_WHT_Tax = Convert.ToString(S_DTO.SIH_WHT_Tax);
                SH_DTO.SIH_WHT_Percent = Convert.ToString(S_DTO.SIH_WHT_Percent);
                SH_DTO.SIH_CUR_DecimalPlaces = Convert.ToString(S_DTO.SIH_CUR_DecimalPlaces);

                TempData["SH_DTO_Json"] = System.Text.Json.JsonSerializer.Serialize(SH_DTO);

                return RedirectToAction("SOItemToCreateSaleInvoice");
            }
            SOItemToSaleInvoiceGetData();
            return View(Original_DTO);
        }









        //Sale Order Numbering
        [Route("sale/setup/sale-invoice-numbering")]
        public IActionResult SINumbering()
        {
            GetSINumber();
            return View(SIN_DTO);
        }

        [Route("sale/setup/sale-invoice-numbering")]
        [HttpPost]
        public IActionResult SINumbering(SINumber_DTO PN_DTO)
        {
            bool IsValid = false;
            SINumber_DTO P_Head_DTO = new SINumber_DTO();

            List<SINumberReset_DTO>? Reset_DTO = new List<SINumberReset_DTO>();
            List<SINumberPrefix_DTO>? Prefix_DTO = new List<SINumberPrefix_DTO>();
            List<SINumberSuffix_DTO>? Suffix_DTO = new List<SINumberSuffix_DTO>();

            P_Head_DTO = SIN_DTO;

            if (PN_DTO.SINumberReset != null)
                Reset_DTO = PN_DTO.SINumberReset!.Where(K => !K.SIR_IsDeleted).ToList();

            if (PN_DTO.SINumberPrefix != null)
                Prefix_DTO = PN_DTO.SINumberPrefix!.Where(K => !K.SIP_IsDeleted).ToList();

            if (PN_DTO.SINumberSuffix != null)
                Suffix_DTO = PN_DTO.SINumberSuffix!.Where(K => !K.SIS_IsDeleted).ToList();

            if (PN_DTO.SIN_Method == "2")
            {
                String ResetDTO = string.Join(", ", Reset_DTO.Where(x => Convert.ToInt64(x.SIR_Number) != 0).Select(x => x.SIR_Number));
                String PrefixDTO = string.Join(", ", Prefix_DTO.Where(x => Convert.ToInt64(x.SIP_Number) != 0).Select(x => x.SIP_Number));
                String SuffixDTO = string.Join(", ", Suffix_DTO.Where(x => Convert.ToInt64(x.SIS_Number) != 0).Select(x => x.SIS_Number));

                SIN_DTO.SIN_DeleteNumbers = Convert.ToString(ResetDTO);
                SIN_DTO.SIN_Id = 31;
                SIN_DAO.SINumberDB(SIN_DTO);

                SIN_DTO.SIN_DeleteNumbers = Convert.ToString(PrefixDTO);
                SIN_DTO.SIN_Id = 32;
                SIN_DAO.SINumberDB(SIN_DTO);

                SIN_DTO.SIN_DeleteNumbers = Convert.ToString(SuffixDTO);
                SIN_DTO.SIN_Id = 33;
                SIN_DAO.SINumberDB(SIN_DTO);

                SIN_DTO.SIN_CreatorCode = Convert.ToInt32(UserCode);

                SIN_DTO.SIN_Method = PN_DTO.SIN_Method;
                if (PN_DTO.SIN_Number == 0)
                {
                    SIN_DTO.SIN_Id = 11;
                }
                else
                {
                    SIN_DTO.SIN_Id = 41;
                    SIN_DTO.SIN_Number = PN_DTO.SIN_Number;
                }
                SIN_DAO.SINumberDB(SIN_DTO);

                foreach (var Reset in Reset_DTO)
                {
                    SIN_DTO.SIN_Date = Convert.ToString(Convert.ToDateTime(Reset.SIR_Date).ToString("yyyyMMdd"));
                    SIN_DTO.SIN_StartingNumber = Convert.ToInt32(Reset.SIR_StartingNumber).ToString();
                    SIN_DTO.SIN_NumberofDigits = Convert.ToInt32(Reset.SIR_NumberofDigits).ToString();
                    SIN_DTO.SIN_PrefilZero = Convert.ToInt64(Reset.SIR_PrefilZero).ToString();
                    SIN_DTO.SIN_Frequency = Convert.ToInt64(Reset.SIR_Frequency).ToString();
                    if (Reset.SIR_Number == 0)
                    {
                        SIN_DTO.SIN_Id = 12;
                    }
                    else
                    {
                        SIN_DTO.SIN_Id = 42;
                        SIN_DTO.SIN_Number = Reset.SIR_Number;
                    }
                    SIN_DAO.SINumberDB(SIN_DTO);
                }

                foreach (var Prefix in Prefix_DTO)
                {
                    SIN_DTO.SIN_Date = Convert.ToString(Convert.ToDateTime(Prefix.SIP_Date).ToString("yyyyMMdd"));
                    SIN_DTO.SIN_Particulars = Convert.ToString(Prefix.SIP_Particulars);
                    if (Prefix.SIP_Number == 0)
                    {
                        SIN_DTO.SIN_Id = 13;
                    }
                    else
                    {
                        SIN_DTO.SIN_Id = 43;
                        SIN_DTO.SIN_Number = Prefix.SIP_Number;
                    }
                    SIN_DAO.SINumberDB(SIN_DTO);
                }

                foreach (var Suffix in Suffix_DTO)
                {
                    SIN_DTO.SIN_Date = Convert.ToString(Convert.ToDateTime(Suffix.SIS_Date).ToString("yyyyMMdd"));
                    SIN_DTO.SIN_Particulars = Convert.ToString(Suffix.SIS_Particulars);
                    if (Suffix.SIS_Number == 0)
                    {
                        SIN_DTO.SIN_Id = 14;
                    }
                    else
                    {
                        SIN_DTO.SIN_Id = 44;
                        SIN_DTO.SIN_Number = Suffix.SIS_Number;
                    }
                    SIN_DAO.SINumberDB(SIN_DTO);
                }
                SIN_DTO.Reset();
                Reset_DTO = null;
                Prefix_DTO = null;
                Suffix_DTO = null;
                ModelState.Clear();
            }
            else if (PN_DTO.SIN_Method == "3")
            {
                SIN_DTO.SIN_Method = PN_DTO.SIN_Method;
                if (PN_DTO.SIN_Number == 0)
                {
                    SIN_DTO.SIN_Id = 21;
                }
                else
                {
                    SIN_DTO.SIN_Id = 22;
                    SIN_DTO.SIN_Number = PN_DTO.SIN_Number;
                }
                SIN_DAO.SINumberDB(SIN_DTO);
            }

            GetSINumber();
            return View(SIN_DTO);
        }
        void GetSINumber()
        {
            SIN_DTO.SIN_CreatorCode = Convert.ToInt32(UserCode);
            SIN_DTO.SIN_Id = 1;
            DS = SIN_DAO.SINumberDB(SIN_DTO);

            ViewBag.Method = Help.GetCat(DS.Tables[0]);
            ViewBag.Frequency = Help.GetCat(DS.Tables[1]);
            ViewBag.Prefil = Help.GetCat(DS.Tables[2]);

            if (DS.Tables[3].Rows.Count > 0)
            {
                SIN_DTO.SIN_Number = Convert.ToInt64(DS.Tables[3].Rows[0]["SIN_Number"]);
                SIN_DTO.SIN_Method = DS.Tables[3].Rows[0]["SIN_Method"].ToString();
            }

            SIN_DTO.SINumberReset = SIN_DL.SIRList(DS.Tables[4]);
            SIN_DTO.SINumberPrefix = SIN_DL.SIPList(DS.Tables[5]);
            SIN_DTO.SINumberSuffix = SIN_DL.SISList(DS.Tables[6]);
        }
    }
}
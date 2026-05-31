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
    public class SaleOrderController : Controller
    {
        CultureInfo India = new CultureInfo("hi-IN");
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();

        //SO
        SaleOrder_DL S_DL = new SaleOrder_DL();
        SaleOrderHead_DTO SOH_DTO = new SaleOrderHead_DTO();
        SaleOrder_DAO SO_DAO = new SaleOrder_DAO();
        SaleOrder_DTO SO_DTO = new SaleOrder_DTO();
        SalesOrderRegister_DTO SOR_DTO = new SalesOrderRegister_DTO();
        List<SalesOrderRegister_DTO> SOR_List = new List<SalesOrderRegister_DTO>();


        //SO NUMBERING
        SONumber_DTO SON_DTO = new SONumber_DTO();
        SONumber_DAO SON_DAO = new SONumber_DAO();
        SONumbering_DL SON_DL = new SONumbering_DL();
        List<SONumberPrefix_DTO> SOP_List = new List<SONumberPrefix_DTO>();
        List<SONumberSuffix_DTO> SOS_List = new List<SONumberSuffix_DTO>();



        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;



        //Sale Order Create
        [Route("sale/transactions/sale-order/create")]
        public IActionResult CreateSaleOrder()
        {
                SaleOrderHead_DTO PH_DTO = new SaleOrderHead_DTO();
                PH_DTO.SOH_RegDate = DateTime.Now.ToString("dd-MMM-yy");
                PH_DTO.SOH_OrderDate = DateTime.Now.ToString("dd-MMM-yy");
                PH_DTO.SOH_RegNo = OnSaleNumber(Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")));
                SaleGetData();
                return View(PH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-order/create")]
        public IActionResult CreateSaleOrder(SaleOrderHead_DTO S_DTO, String? Mode)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SaleOrderHead_DTO S_Head_DTO = new SaleOrderHead_DTO();

            List<SaleOrderItem_DTO>? ITM_DTO = new List<SaleOrderItem_DTO>();
            List<SaleOrderIncome_DTO>? Income_DTO = new List<SaleOrderIncome_DTO>();
            List<SaleOrderIIncome_DTO>? ItemIncome_DTO = new List<SaleOrderIIncome_DTO>();
            List<SaleOrderAddress_DTO>? BuyerAddress_DTO = new List<SaleOrderAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.SaleItems != null)
                ITM_DTO = S_DTO.SaleItems!.Where(K => K.SOI_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SOH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SOI_INC_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SOH_ADD_IsDeleted == 0).ToList();

            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Save")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SOI_MS_Number) != Convert.ToInt64(S_DTO.SOH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SOI_Qty) == 0 || Convert.ToDouble(x.SOI_UnitPrice) == 0 || Convert.ToDouble(x.SOI_Amount) == 0);

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
                else if (Convert.ToInt32(S_DTO.SOH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SOH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SOH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.SaleItems = ITM_DTO;
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
                                String SOHOrderNoOld = S_DTO.SOH_RegNo;
                                String SOHOrderNoNew = OnSaleNumber(Convert.ToInt32(Convert.ToDateTime(S_DTO.SOH_RegDate).ToString("yyyyMMdd")));

                                SO_DTO.SO_RegNo = S_DTO.SOH_RegNo;
                                SO_DTO.SO_RegDate = Convert.ToString(Convert.ToDateTime(S_DTO.SOH_RegDate).ToString("yyyyMMdd"));
                                SO_DTO.SO_OrderNo = S_DTO.SOH_OrderNo;
                                SO_DTO.SO_OrderDate = Convert.ToString(Convert.ToDateTime(S_DTO.SOH_OrderDate).ToString("yyyyMMdd"));
                                SO_DTO.SO_BUY_Number = Convert.ToInt64(S_DTO.SOH_BUY_Number);
                                SO_DTO.SO_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SOH_ExportOrder) ? 1 : 0);
                                SO_DTO.SO_CUR_Number = Convert.ToInt64(S_DTO.SOH_CUR_Number);
                                SO_DTO.SO_MS_Number = Convert.ToInt64(S_DTO.SOH_MS_Number);
                                SO_DTO.SO_PaymentTerms = S_DTO.SOH_PaymentTerms;
                                SO_DTO.SO_PaymentMethod = S_DTO.SOH_PaymentMethod;
                                SO_DTO.SO_DeliveryTerms = S_DTO.SOH_DeliveryTerms;
                                SO_DTO.SO_DeliveryMethod = S_DTO.SOH_DeliveryMethod;
                                SO_DTO.SO_QCR = S_DTO.SOH_QCR;
                                SO_DTO.SO_TDC = S_DTO.SOH_TDC;
                                SO_DTO.SO_OtherRemarks = S_DTO.SOH_OtherRemarks;
                                SO_DTO.SO_ExchangeRate = Convert.ToDouble(S_DTO.SOH_ExchangeRate) == 0 ? "1" : S_DTO.SOH_ExchangeRate;
                                SO_DTO.SO_OtherRemarks = S_DTO.SOH_OtherRemarks;
                                SO_DTO.SO_TotalAmount = Convert.ToDouble(S_DTO.SOH_TotalAmount);
                                SO_DTO.SO_TotalItemIncome = Convert.ToDouble(S_DTO.SOH_TotalItemIncome);
                                SO_DTO.SO_TotalHeadIncome = Convert.ToDouble(S_DTO.SOH_TotalHeadIncome);
                                SO_DTO.SO_OrderValue = Convert.ToDouble(S_DTO.SOH_OrderValue);
                                SO_DTO.SO_Id = 11;
                                DS = SO_DAO.SaleOrderDB(SO_DTO);

                                OnSaleNumberGen(Convert.ToInt32(Convert.ToDateTime(S_DTO.SOH_OrderDate).ToString("yyyyMMdd")));

                                if (DS.Tables[0].Rows.Count > 0)
                                {
                                    SO_DTO.SO_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                    foreach (var Item in ITM_DTO)
                                    {
                                        DataSet D = new DataSet();
                                        SO_DTO.SO_ITM_Number = Convert.ToInt64(Item.SOI_ITM_Number);
                                        SO_DTO.SO_UoM_Number = Convert.ToInt64(Item.SOI_UoM_Number);
                                        SO_DTO.SO_Qty = Convert.ToDouble(Item.SOI_Qty);
                                        SO_DTO.SO_UnitPrice = Convert.ToDouble(Item.SOI_UnitPrice);
                                        SO_DTO.SO_Amount = Convert.ToDouble(Item.SOI_Amount);
                                        SO_DTO.SO_IncomeValue = Convert.ToDouble(Item.SOI_IncomeValue);
                                        SO_DTO.SO_DeliveryDate = Convert.ToInt32(Convert.ToDateTime(Item.SOI_DeliveryDate).ToString("yyyyMMdd"));
                                        SO_DTO.SO_Id = 12;
                                        D = SO_DAO.SaleOrderDB(SO_DTO);

                                        var ItemIncome = ItemIncome_DTO.Where(x => (x.SOI_INC_ITM_Number == Item.SOI_ITM_Number) && (x.SOI_INC_ITM_Index == Item.SOI_Index));

                                        foreach (var ItemInc in ItemIncome)
                                        {
                                            SO_DTO.SO_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                            SO_DTO.SO_ITM_Number = Convert.ToInt64(ItemInc.SOI_INC_ITM_Number);
                                            SO_DTO.SO_INC_MIC_Number = Convert.ToInt64(ItemInc.SOI_INC_MIC_Number);
                                            SO_DTO.SO_INC_Remarks = ItemInc.SOI_INC_Remarks;
                                            SO_DTO.SO_INC_OCRN_Number = Convert.ToInt64(ItemInc.SOI_INC_OCRN_Number);
                                            SO_DTO.SO_INC_CM_Number = Convert.ToInt64(ItemInc.SOI_INC_CM_Number);
                                            SO_DTO.SO_INC_IncomeBase = Convert.ToDouble(ItemInc.SOI_INC_IncomeBase);
                                            SO_DTO.SO_INC_IncomeValue = Convert.ToDouble(ItemInc.SOI_INC_IncomeValue);
                                            SO_DTO.SO_INC_ALCT_Number = Convert.ToInt64(ItemInc.SOI_INC_ALCT_Number);
                                            SO_DTO.SO_INC_LA_Number = Convert.ToInt64(ItemInc.SOI_INC_LA_Number);
                                            SO_DTO.SO_Id = 14;
                                            SO_DAO.SaleOrderDB(SO_DTO);
                                        }
                                    }
                                    foreach (var Income in Income_DTO)
                                    {
                                        SO_DTO.SO_INC_MIC_Number = Convert.ToInt64(Income.SOH_INC_MIC_Number);
                                        SO_DTO.SO_INC_Remarks = Income.SOH_INC_Remarks;
                                        SO_DTO.SO_INC_OCRN_Number = Convert.ToInt64(Income.SOH_INC_OCRN_Number);
                                        SO_DTO.SO_INC_CM_Number = Convert.ToInt64(Income.SOH_INC_CM_Number);
                                        SO_DTO.SO_INC_IncomeBase = Convert.ToDouble(Income.SOH_INC_IncomeBase);
                                        SO_DTO.SO_INC_IncomeValue = Convert.ToDouble(Income.SOH_INC_IncomeValue);
                                        SO_DTO.SO_INC_ALCT_Number = Convert.ToInt64(Income.SOH_INC_ALCT_Number);
                                        SO_DTO.SO_INC_LA_Number = Convert.ToInt64(Income.SOH_INC_LA_Number);
                                        SO_DTO.SO_Id = 13;
                                        SO_DAO.SaleOrderDB(SO_DTO);
                                    }


                                    foreach (var BuyerAddress in BuyerAddress_DTO)
                                    {
                                        SO_DTO.SO_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SOH_ADD_ADTP_Number);
                                        SO_DTO.SO_ADD_AddressID = Convert.ToString(BuyerAddress.SOH_ADD_AddressID);
                                        SO_DTO.SO_ADD_Address = Convert.ToString(BuyerAddress.SOH_ADD_Address);
                                        SO_DTO.SO_ADD_City = Convert.ToString(BuyerAddress.SOH_ADD_City);
                                        SO_DTO.SO_ADD_State = Convert.ToString(BuyerAddress.SOH_ADD_State);
                                        SO_DTO.SO_ADD_Country = Convert.ToString(BuyerAddress.SOH_ADD_Country);
                                        SO_DTO.SO_ADD_Pin = Convert.ToString(BuyerAddress.SOH_ADD_Pin);
                                        SO_DTO.SO_ADD_GSTIN = Convert.ToString(BuyerAddress.SOH_ADD_GSTIN);
                                        SO_DTO.SO_Id = 15;
                                        SO_DAO.SaleOrderDB(SO_DTO);
                                    }
                                }

                                transaction.Complete();

                                S_Head_DTO.Reset();
                                Income_DTO = null;
                                ITM_DTO = null;
                                ItemIncome_DTO = null;
                                S_DTO.Reset();
                                Original_DTO = Help.JsonClone(S_DTO);

                                if (SOHOrderNoOld != SOHOrderNoNew)
                                {
                                    ViewBag.ErrorCode = 2;
                                    ViewBag.ErrorMessage = "Sale Order number " + SOHOrderNoOld + " used by another user. Next number will be allotted to you.";
                                }
                                return RedirectToAction("CreateSaleOrder");
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

        [Route("sale/transactions/sale-order/item")]
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
            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            SO_DTO.SO_ITM_Code = Convert.ToString(ItemCode).Trim();
            SO_DTO.SO_MS_Number = Convert.ToInt64(MS.Trim());
            SO_DTO.SO_Id = 2;
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            var Item = S_DL.ItemList(DS.Tables[0]);
            return Json(Item);
        }

        [Route("sale/transactions/sale-order/uom")]
        public String SaleUoM(String? UoM)
        {
            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            SO_DTO.SO_UoM_Number = Convert.ToInt64(UoM);
            SO_DTO.SO_Id = 4;
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return DS.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        [Route("sale/transactions/sale-order/income/des")]
        public IActionResult SaleExpensiveDes(String? Title)
        {
            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            SO_DTO.SO_INC_MIC_Number = Convert.ToInt64(Title);
            SO_DTO.SO_Id = 3;
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            var Expensive = S_DL.IncomeList(DS.Tables[0]).FirstOrDefault();
            return Json(Expensive);
        }

        [Route("sale/transactions/sale-order/buyer")]
        public IActionResult SaleBuyer(String? Buyer, String? Export)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            if (Export == null)
            {
                Export = "";
            }
            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            SO_DTO.SO_ExportOrder = Convert.ToInt16(Convert.ToBoolean(Export) == true ? 2 : 1);
            SO_DTO.SO_ITM_Code = Convert.ToString(Buyer).Trim();
            SO_DTO.SO_Id = 5;
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            var Ven = S_DL.BuyerList(DS.Tables[0]);
            return Json(Ven);
        }

        [Route("sale/transactions/sale-order/buyer/address")]
        public IActionResult SaleBuyerAddressID(String? Buyer, String ADTPNumber)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            SO_DTO.SO_BUY_Number = Convert.ToInt64(Buyer);
            SO_DTO.SO_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SO_DTO.SO_Id = 6;
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            SaleRejectionAddress SRA = new SaleRejectionAddress();
            SRA.BuyerAddressId = S_DL.BuyerAddressID(DS.Tables[0]);
            SRA.BuyerAddress = S_DL.BuyerAddress(DS.Tables[1]).FirstOrDefault();
            return Json(SRA);
        }

        [Route("sale/transactions/sale-order/buyer/address/addressid")]
        public IActionResult SaleBuyerAddress(String? Buyer, String ADTPNumber, String AddressID)
        {
            if (Buyer == null)
            {
                Buyer = "";
            }
            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            SO_DTO.SO_BUY_Number = Convert.ToInt64(Buyer);
            SO_DTO.SO_ADD_ADTP_Number = Convert.ToInt64(ADTPNumber);
            SO_DTO.SO_ADD_AddressID = Convert.ToString(AddressID);
            SO_DTO.SO_Id = 7;
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            var Address = S_DL.BuyerAddress(DS.Tables[0]).FirstOrDefault();
            return Json(Address);
        }

        void SaleGetData()
        {
            SO_DTO.SO_OrderDate = Convert.ToString(DateTime.Now.ToString("yyyyMMdd"));
            SO_DTO.SO_Id = 1;
            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            DS = SO_DAO.SaleOrderDB(SO_DTO);

            ViewBag.IncomeCode = Help.GetCat(DS.Tables[0]);
            ViewBag.Occurrence = Help.GetCat(DS.Tables[1]);
            ViewBag.ChargeableMethod = Help.GetCat(DS.Tables[2]);
            ViewBag.Allocate = Help.GetCat(DS.Tables[3]);
            ViewBag.Currency = Help.GetCat(DS.Tables[4]);
            ViewBag.MaterialSegregation = Help.GetCat(DS.Tables[5]);
            ViewBag.UoM = Help.GetCat(DS.Tables[6]);
            ViewBag.LedgerAccount = Help.GetCat(DS.Tables[7]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[8]);
        }
        void OnSaleNumberGen(Int32 SODate)
        {
            DataSet DS1 = new DataSet();
            SON_DTO.SON_Date = SODate.ToString();
            SON_DTO.SON_Id = 101;
            DS1 = SON_DAO.SONumberDB(SON_DTO);
            if (DS1.Tables[0].Rows.Count > 0)
            {
                Int32 Order = Convert.ToInt32(DS1.Tables[0].Rows[0]["SON_Method"].ToString());
                DateTime SON_Date = Convert.ToDateTime(DateTime.ParseExact(SODate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture));

                DateTime StartDate = new DateTime();
                DateTime EndDate = new DateTime();

                if (Order == 2)
                {
                    if (DS1.Tables[1].Rows.Count > 0)
                    {
                        Int32 Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["SO_StartingNumber"].ToString());

                        SON_DTO.SON_Number = Convert.ToInt32(DS1.Tables[1].Rows[0]["SO_Number"].ToString());
                        SON_DTO.SON_StartingNumber = Convert.ToString(Convert.ToInt32(Number + 1));
                        SON_DTO.SON_Id = 103;
                        SON_DAO.SONumberDB(SON_DTO);
                    }
                    else
                    {
                        Int32 Frequency = 0;
                        Int32 Start = 0;
                        DateTime Date = new DateTime();

                        if (DS1.Tables[2].Rows.Count > 0)
                        {
                            Date = Convert.ToDateTime(DS1.Tables[2].Rows[0]["SOR_Date"].ToString());
                            Start = Convert.ToInt32(DS1.Tables[2].Rows[0]["SOR_StartingNumber"].ToString());
                            Frequency = Convert.ToInt32(DS1.Tables[2].Rows[0]["SOR_Frequency"].ToString());
                        }

                        if (Frequency == 4)
                        {
                            if (Date.Month == SON_Date.Month)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(SON_Date.Year, SON_Date.Month, 1);
                                EndDate = new DateTime(SON_Date.Year, SON_Date.Month, 1).AddMonths(1).AddDays(-1);
                            }
                        }
                        else if (Frequency == 5)
                        {
                            if (Date.Month == SON_Date.Month && Date.Year == SON_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else if (Date.Month != SON_Date.Month && Date.Year == SON_Date.Year)
                            {
                                StartDate = Date;
                                EndDate = new DateTime(Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                            else
                            {
                                StartDate = new DateTime(SON_Date.Year, Date.Month, 1);
                                EndDate = new DateTime(SON_Date.Year, Date.Month, 1).AddMonths(12).AddDays(-1);
                            }
                        }

                        SON_DTO.SON_Number = Convert.ToInt32(DS1.Tables[2].Rows[0]["SOR_Number"].ToString());
                        SON_DTO.SON_StartingNumber = Convert.ToString(Start);
                        SON_DTO.SON_Date = Convert.ToString(StartDate.ToString("yyyyMMdd"));
                        SON_DTO.SON_Method = Convert.ToString(EndDate.ToString("yyyyMMdd"));
                        SON_DTO.SON_Id = 102;
                        SON_DAO.SONumberDB(SON_DTO);
                    }
                }
            }
        }


        [Route("sale/transactions/sale-order/numbering")]
        public String OnSaleNumber(Int32 SODate)
        {
            SO_DTO.SO_OrderDate = Convert.ToString(SODate);
            SO_DTO.SO_Id = 0;
            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            DS = SO_DAO.SaleOrderDB(SO_DTO);

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
                    Order = Convert.ToInt32(DS.Tables[0].Rows[0]["SON_Method"].ToString());
                }
                if (Order == 2)
                {
                    if (DS.Tables[2].Rows.Count > 0)
                    {
                        Prefix = DS.Tables[2].Rows[0]["SOP_Particulars"].ToString();
                    }
                    if (DS.Tables[3].Rows.Count > 0)
                    {
                        Surfix = DS.Tables[3].Rows[0]["SOS_Particulars"].ToString();
                    }
                    if (DS.Tables[4].Rows.Count > 0)
                    {
                        Int32 OrNum = Convert.ToInt32(DS.Tables[4].Rows[0]["SO_StartingNumber"].ToString());
                        if (DS.Tables[1].Rows.Count > 0)
                        {
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["SOR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["SOR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["SOR_Frequency"].ToString());

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
                            //DateTime RDate = Convert.ToDateTime(DS.Tables[1].Rows[0]["SOR_Date"]);
                            Int32 RNumber = Convert.ToInt32(DS.Tables[1].Rows[0]["SOR_StartingNumber"].ToString());
                            Int32 RZero = Convert.ToInt32(DS.Tables[1].Rows[0]["SOR_PrefilZero"].ToString());
                            Int32 RDigit = Convert.ToInt32(DS.Tables[1].Rows[0]["SOR_NumberofDigits"].ToString());
                            Int32 RFre = Convert.ToInt32(DS.Tables[1].Rows[0]["SOR_Frequency"].ToString());

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
                ViewBag.ErrorMessage = "SO number Not assigned for Given date";
            }
            return "";
        }





        //Sale Preview
        [Route("sale/transactions/sale-order/{SO_No}/view")]
        public IActionResult PreviewSaleOrder(String? SO_No)
        {
                String Active = GetSOPreview(SO_No);
                if (Active != "1")
                {
                    return RedirectToAction("SaleOrderRegisterSummary");
                }
                ViewBag.SO_No = SO_No;

                SaleGetData();
                GetSOEditData(SO_No);
                return View(SOH_DTO);
        }

        [Route("sale/transactions/sale-order/{SO_No}/view")]
        [HttpPost]
        public IActionResult PreviewSaleOrder(String Mode, String? SO_No)
        {
                if (Mode == "PDF")
                {
                    SO_DTO.SO_Number = Convert.ToInt64(SO_No);
                    SO_DTO.SO_Id = 41;
                    SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
                    DS = SO_DAO.SaleOrderDB(SO_DTO);

                    String Total = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[0].Rows[0]["SOH_TotalAmount"]));

                    String Item = "";
                    if (DS.Tables[1].Rows.Count > 0)
                    {
                        for (Int32 i = 0; i < DS.Tables[1].Rows.Count; i++)
                        {
                            String UnitPrice = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["SOI_UnitPrice"]));
                            String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["SOI_Amount"]));
                            Item += $@"<tr>
                                <td colspan='2'>{DS.Tables[1].Rows[i]["SOI_ITM_Code"]}</td>
                                <td colspan='3'>{DS.Tables[1].Rows[i]["SOI_ITM_Description"]}</td>
                                <td class='align-center'>{DS.Tables[1].Rows[i]["SOI_UoM_Name"]}</td>
                                <td colspan='2' class='align-center'>{DS.Tables[1].Rows[i]["SOI_Qty"]}</td>
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
                                    <td colspan='2'></td>
                                    <td colspan='3'></td>
                                    <td class='align-center'></td>
                                    <td colspan='2' class='align-center'></td>
                                    <td colspan='2' class='align-right'></td>
                                    <td colspan='2' class='align-right'></td>
                                    </tr>";
                            }
                        }
                    }

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
                                .SO-container {{
                                    max-width: 100%;
                                    margin: auto;
                                    background-color: #fff;
                                    padding: 20px;
                                    height: 100%;
                                    font-size: 1rem;
                                }}

                                .SO-table {{
                                    width: 100%;
                                    border-collapse: collapse;
                                    border: 1px solid #ccc;
                                    font-family: ""Noto Sans"", sans-serif;
                                }}

                                    .SO-table th, .SO-table td {{
                                        border: 1px solid #ccc;
                                        padding: 5px 8px;
                                        vertical-align: top;
                                        line-height: 1.4;
                                    }}

                                    .SO-table th {{
                                        font-weight: bold;
                                    }}

                                    .SO-table tr td {{
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

                                .SO-title {{
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
                                    border: none !imSOrtant;
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
                            </style>
                            <table class='SO-table' style='background-color: #fff;color:#000'>
                                <tbody>
                                    <tr>
                                        <td colspan='2' style='vertical-align: middle; text-align: center;'>[LOGO]</td>
                                        <td colspan='10' class='company-title'>ABC PRIVATE LIMITED</td>
                                    </tr>
                                    <tr class='header-main'>
                                        <td colspan='4' class='SO-title'>Sale ORDER</td>
                                        <td class='align-right' style='vertical-align: middle;'>SO #</td>
                                        <td colspan='3' style='vertical-align: middle;'>{DS.Tables[0].Rows[0]["SOH_OrderNo"]}</td>
                                        <td></td>
                                        <td class='align-right' style='vertical-align: middle;'>Date</td>
                                        <td colspan='2' style='vertical-align: middle;'>{DS.Tables[0].Rows[0]["SOH_OrderDate"]}</td>
                                    </tr>
                                    <tr class='header-section'>
                                        <td colspan='6'>Buyer</td>
                                        <td colspan='6'>SHIP TO</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='6'></td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='6'></td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'></td>
                                        <td colspan='6'></td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>GST: </td>
                                        <td colspan='6'>GST: </td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>PAN: </td>
                                        <td colspan='6'>PAN: </td>
                                    </tr>
                                    <tr class='header-section'>
                                        <th>Currency</th>
                                        <th colspan='3'>Payment Terms</th>
                                        <th colspan='2'>Method of payment</th>
                                        <th colspan='4'>Delivery terms</th>
                                        <th colspan='2'>Mode of delivery</th>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["SOH_PaymentTerms"]}</td>
                                        <td colspan='3'>{DS.Tables[0].Rows[0]["SOH_PaymentTerms"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["SOH_PaymentMethod"]}</td>
                                        <td colspan='4'>{DS.Tables[0].Rows[0]["SOH_DeliveryTerms"]}</td>
                                        <td colspan='2'>{DS.Tables[0].Rows[0]["SOH_DeliveryMethod"]}</td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'><a href='#' class='details-link'>Inspection Details</a></td>
                                        <td colspan='6'><a href='#' class='details-link'>Technical delivery conditions</a></td>
                                    </tr>
                                    <tr>
                                        <td colspan='6'>{DS.Tables[0].Rows[0]["SOH_Inspection"]}</td>
                                        <td colspan='6'>{DS.Tables[0].Rows[0]["SOH_TDC"]}</td>
                                    </tr>
                                    <tr class='header-section'>
                                        <th colspan='2'>Item Code</th>
                                        <th colspan='3'>Description</th>
                                        <th class='align-center'>Uom</th>
                                        <th colspan='2' class='align-center'>Qty</th>
                                        <th colspan='2' class='align-right'>Unit Price</th>
                                        <th colspan='2' class='align-right'>Value</th>
                                    </tr>";
                    PDFDownload += Item;
                    PDFDownload += $@"<tr class='total-row'>
                                        <td colspan='6' class='align-right'>Total</td>
                                        <td colspan='2' class='align-center'>{DS.Tables[0].Rows[0]["TotalQty"]}</td>
                                        <td colspan='2'></td>
                                    <td colspan='2' class='align-right'>{Total}</td>
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
                            </table>
                        </body>
                    </html>";

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
                    SO_DTO.SO_Number = Convert.ToInt64(SO_No);
                    SO_DTO.SO_Id = 41;
                    SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
                    DS = SO_DAO.SaleOrderDB(SO_DTO);

                    String Total = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[0].Rows[0]["SOH_TotalAmount"]));

                    String Item = "";
                    if (DS.Tables[1].Rows.Count > 0)
                    {
                        for (Int32 i = 0; i < DS.Tables[1].Rows.Count; i++)
                        {
                            String UnitPrice = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["SOI_UnitPrice"]));
                            String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["SOI_Amount"]));
                            Item += $@"<tr>
                                <td>{DS.Tables[1].Rows[i]["SOI_ITM_Code"]}</td>
                                <td>{DS.Tables[1].Rows[i]["SOI_ITM_Description"]}</td>
                                <td>{DS.Tables[1].Rows[i]["SOI_UoM_Name"]}</td>
                                <td>{DS.Tables[1].Rows[i]["SOI_Qty"]}</td>
                                <td>{UnitPrice}</td>
                                <td>{Amount}</td>
                                </tr>";
                        }

                        Int32 Count = 10 - Convert.ToInt32(DS.Tables[1].Rows.Count);
                        if (Count > 0)
                        {
                            for (Int32 i = 0; i < Count; i++)
                            {
                                Item += $@"<tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    </tr>";
                            }
                        }
                    }

                    string htmlString = $@"
                            <table>
                                <tbody>
                                    <tr>
                                        <td>[LOGO]</td>
                                        <td>ABC PRIVATE LIMITED</td>
                                    </tr>
                                    <tr>
                                        <td class='SO-title'>Sale ORDER</td>
                                        <td>SO #</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_OrderNo"]}</td>
                                        <td></td>
                                        <td>Date</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_OrderDate"]}</td>
                                    </tr>
                                    <tr>
                                        <td>Buyer</td>
                                        <td>SHIP TO</td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>GST:</td>
                                        <td>GST:</td>
                                    </tr>
                                    <tr>
                                        <td>PAN:</td>
                                        <td>PAN:</td>
                                    </tr>
                                    <tr>
                                        <th>Currency</th>
                                        <th>Payment Terms</th>
                                        <th>Method of payment</th>
                                        <th>Delivery terms</th>
                                        <th>Mode of delivery</th>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["SOH_PaymentTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_PaymentTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_PaymentMethod"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_DeliveryTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_DeliveryMethod"]}</td>
                                    </tr>
                                    <tr>
                                        <td><a href='#'>Inspection Details</a></td>
                                        <td><a href='#'>Technical delivery conditions</a></td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["SOH_Inspection"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_TDC"]}</td>
                                    </tr>
                                    <tr>
                                        <th>Item Code</th>
                                        <th>Description</th>
                                        <th>Uom</th>
                                        <th>Qty</th>
                                        <th>Unit Price</th>
                                        <th>Value</th>
                                    </tr>";
                    htmlString += Item;
                    htmlString += $@"<tr>
                                        <td>Total</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_TotalAmount"]}</td>
                                        <td></td>
                                    <td>{Total}</td>
                                    </tr>
                                    <tr>
                                        <td>Tax</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>Remarks</td>
                                        <td></td>
                                    </tr>
                                    <tr> <td> </td> </tr>
                                    <tr> <td> </td> </tr>
                                    <tr>
                                        <td></td>
                                        <td>for ABC Private Limited</td>
                                    </tr>
                                    <tr> <td> </td> </tr>
                                    <tr>
                                        <td></td>
                                        <td>Authorized Signatory</td>
                                    </tr>
                                </tbody>
                            </table>";

                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlString);

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
                    SO_DTO.SO_Number = Convert.ToInt64(SO_No);
                    SO_DTO.SO_Id = 41;
                    SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
                    DS = SO_DAO.SaleOrderDB(SO_DTO);

                    String Total = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[0].Rows[0]["SOH_TotalAmount"]));

                    String Item = "";
                    if (DS.Tables[1].Rows.Count > 0)
                    {
                        for (Int32 i = 0; i < DS.Tables[1].Rows.Count; i++)
                        {
                            String UnitPrice = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["SOI_UnitPrice"]));
                            String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(DS.Tables[1].Rows[0]["SOI_Amount"]));
                            Item += $@"<tr>
                                <td>{DS.Tables[1].Rows[i]["SOI_ITM_Code"]}</td>
                                <td>{DS.Tables[1].Rows[i]["SOI_ITM_Description"]}</td>
                                <td>{DS.Tables[1].Rows[i]["SOI_UoM_Name"]}</td>
                                <td>{DS.Tables[1].Rows[i]["SOI_Qty"]}</td>
                                <td>{UnitPrice}</td>
                                <td>{Amount}</td>
                                </tr>";
                        }

                        Int32 Count = 10 - Convert.ToInt32(DS.Tables[1].Rows.Count);
                        if (Count > 0)
                        {
                            for (Int32 i = 0; i < Count; i++)
                            {
                                Item += $@"<tr>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    </tr>";
                            }
                        }
                    }

                    string htmlString = $@"
                            <table>
                                <tbody>
                                    <tr>
                                        <td>[LOGO]</td>
                                        <td>ABC PRIVATE LIMITED</td>
                                    </tr>
                                    <tr>
                                        <td class='SO-title'>Sale ORDER</td>
                                        <td>SO #</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_OrderNo"]}</td>
                                        <td></td>
                                        <td>Date</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_OrderDate"]}</td>
                                    </tr>
                                    <tr>
                                        <td>Buyer</td>
                                        <td>SHIP TO</td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                   <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                   <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>GST:</td>
                                        <td>GST:</td>
                                    </tr>
                                    <tr>
                                        <td>PAN:</td>
                                        <td>PAN:</td>
                                    </tr>
                                    <tr>
                                        <th>Currency</th>
                                        <th>Payment Terms</th>
                                        <th>Method of payment</th>
                                        <th>Delivery terms</th>
                                        <th>Mode of delivery</th>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["SOH_PaymentTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_PaymentTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_PaymentMethod"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_DeliveryTerms"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_DeliveryMethod"]}</td>
                                    </tr>
                                    <tr>
                                        <td><a href='#'>Inspection Details</a></td>
                                        <td><a href='#'>Technical delivery conditions</a></td>
                                    </tr>
                                    <tr>
                                        <td>{DS.Tables[0].Rows[0]["SOH_Inspection"]}</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_TDC"]}</td>
                                    </tr>
                                    <tr>
                                        <th>Item Code</th>
                                        <th>Description</th>
                                        <th>Uom</th>
                                        <th>Qty</th>
                                        <th>Unit Price</th>
                                        <th>Value</th>
                                    </tr>";
                    htmlString += Item;
                    htmlString += $@"<tr>
                                        <td>Total</td>
                                        <td>{DS.Tables[0].Rows[0]["SOH_TotalAmount"]}</td>
                                        <td></td>
                                    <td>{Total}</td>
                                    </tr>
                                    <tr>
                                        <td>Tax</td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>Remarks</td>
                                        <td></td>
                                    </tr>
                                    <tr> <td> </td> </tr>
                                    <tr> <td> </td> </tr>
                                    <tr>
                                        <td></td>
                                        <td>for ABC Private Limited</td>
                                    </tr>
                                    <tr> <td> </td> </tr>
                                    <tr>
                                        <td></td>
                                        <td>Authorized Signatory</td>
                                    </tr>
                                </tbody>
                            </table>";

                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlString);

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
                            var fileDownloadName = "sale-order-download.xlsx";

                            return File(content, contentType, fileDownloadName);
                        }
                    }

                }
                else if (Mode == "Delete")
                {
                    SO_DTO.SO_Number = Convert.ToInt64(SO_No);
                    SO_DTO.SO_Id = 32;
                    SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
                    DS = SO_DAO.SaleOrderDB(SO_DTO);
                    return RedirectToAction("SaleOrderRegisterSummary");
                }
                String Active = GetSOPreview(SO_No);
                if (Active != "1")
                {
                    return RedirectToAction("SaleOrderRegisterSummary");
                }
                ViewBag.SO_No = SO_No;

                SaleGetData();
                GetSOEditData(SO_No);
                return View(SOH_DTO);
        }
        String GetSOPreview(String SO_No)
        {
            SO_DTO.SO_Number = Convert.ToInt64(SO_No);
            SO_DTO.SO_Id = 41;
            SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                ViewBag.HeadPreview = S_DL.SOHeadEditList(DS.Tables[0]).FirstOrDefault();
                ViewBag.ItemPreview = S_DL.SOItemPreviewList(DS.Tables[1]);
                ViewBag.BuyerPreview = S_DL.BuyerList(DS.Tables[2]).FirstOrDefault();
                return "1";
            }
            else
            {
                return "0";
            }
        }
        String GetSOEditData(String SO_No)
        {
            SO_DTO.SO_Number = Convert.ToInt64(SO_No);
            SO_DTO.SO_Id = 51;
            SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SOH_DTO = S_DL.SOHeadEditList(DS.Tables[0]).FirstOrDefault();
                SOH_DTO.SaleItems = S_DL.SOItemEditList(DS.Tables[1]);
                SOH_DTO.Income = S_DL.SOIncomeEditList(DS.Tables[2]);
                SOH_DTO.ItemIncome = S_DL.SOIIncomeEditList(DS.Tables[3]);
                SOH_DTO.BuyerAddress = S_DL.SOHAddressEditList(DS.Tables[4]);
                return "1";
            }
            else
            {
                return "0";
            }
        }





        //Sale Edit
        [Route("sale/transactions/sale-order/{SO_No}/edit")]
        public IActionResult EditSaleOrder(String? SO_No)
        {
                SaleGetData();
                String Active = GetSOEditData(SO_No);
                if (Active != "1")
                {
                    return RedirectToAction("SaleOrderRegisterSummary");
                }
                ViewBag.SO_No = SO_No;
                return View(SOH_DTO);
        }

        [HttpPost]
        [Route("sale/transactions/sale-order/{SO_No}/edit")]
        public IActionResult EditSaleOrder(SaleOrderHead_DTO S_DTO, String? Mode, String? SO_No)
        {
            var Original_DTO = Help.JsonClone(S_DTO);

            bool IsValid = false;
            SaleOrderHead_DTO S_Head_DTO = new SaleOrderHead_DTO();

            List<SaleOrderItem_DTO>? ITM_DTO = new List<SaleOrderItem_DTO>();
            List<SaleOrderIncome_DTO>? Income_DTO = new List<SaleOrderIncome_DTO>();
            List<SaleOrderIIncome_DTO>? ItemIncome_DTO = new List<SaleOrderIIncome_DTO>();
            List<SaleOrderAddress_DTO>? BuyerAddress_DTO = new List<SaleOrderAddress_DTO>();

            S_Head_DTO = S_DTO;

            if (S_DTO.SaleItems != null)
                ITM_DTO = S_DTO.SaleItems!.Where(K => K.SOI_IsDeleted == 0).ToList();

            if (S_DTO.Income != null)
                Income_DTO = S_DTO.Income!.Where(K => K.SOH_INC_IsDeleted == 0).ToList();

            if (S_DTO.ItemIncome != null)
                ItemIncome_DTO = S_DTO.ItemIncome!.Where(K => K.SOI_INC_IsDeleted == 0).ToList();

            if (S_DTO.BuyerAddress != null)
                BuyerAddress_DTO = S_DTO.BuyerAddress!.Where(K => K.SOH_ADD_IsDeleted == 0).ToList();

            SO_DTO.SO_CreatorCode = Convert.ToInt64(UserCode);
            if (Mode == "Update")
            {
                var CheckItem = ITM_DTO.Where(x => Convert.ToInt64(x.SOI_MS_Number) != Convert.ToInt64(S_DTO.SOH_MS_Number));
                var ValueItem = ITM_DTO.Where(x => Convert.ToDouble(x.SOI_Qty) == 0 || Convert.ToDouble(x.SOI_UnitPrice) == 0 || Convert.ToDouble(x.SOI_Amount) == 0);

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
                else if (Convert.ToInt32(S_DTO.SOH_BUY_Number) == 0)
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Buyer is Required";
                }
                else if (Convert.ToInt32(Convert.ToBoolean(S_DTO.SOH_ExportOrder) ? 2 : 1) != Convert.ToInt32(S_DTO.SOH_BUY_LOC_Number))
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Export Order and Buyer not match";
                }
                else
                {
                    ModelState.Clear();
                    S_Head_DTO.SaleItems = ITM_DTO;
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
                                SO_DTO.SO_Number = Convert.ToInt64(SO_No);
                                SO_DTO.SO_RegDate = Convert.ToString(Convert.ToDateTime(S_DTO.SOH_RegDate).ToString("yyyyMMdd"));
                                SO_DTO.SO_OrderNo = S_DTO.SOH_OrderNo;
                                SO_DTO.SO_OrderDate = Convert.ToString(Convert.ToDateTime(S_DTO.SOH_OrderDate).ToString("yyyyMMdd"));
                                SO_DTO.SO_BUY_Number = Convert.ToInt64(S_DTO.SOH_BUY_Number);
                                SO_DTO.SO_ExportOrder = Convert.ToInt16(Convert.ToBoolean(S_DTO.SOH_ExportOrder) ? 1 : 0);
                                SO_DTO.SO_CUR_Number = Convert.ToInt64(S_DTO.SOH_CUR_Number);
                                SO_DTO.SO_MS_Number = Convert.ToInt64(S_DTO.SOH_MS_Number);
                                SO_DTO.SO_PaymentTerms = S_DTO.SOH_PaymentTerms;
                                SO_DTO.SO_PaymentMethod = S_DTO.SOH_PaymentMethod;
                                SO_DTO.SO_DeliveryTerms = S_DTO.SOH_DeliveryTerms;
                                SO_DTO.SO_DeliveryMethod = S_DTO.SOH_DeliveryMethod;
                                SO_DTO.SO_QCR = S_DTO.SOH_QCR;
                                SO_DTO.SO_TDC = S_DTO.SOH_TDC;
                                SO_DTO.SO_OtherRemarks = S_DTO.SOH_OtherRemarks;
                                SO_DTO.SO_ExchangeRate = Convert.ToDouble(S_DTO.SOH_ExchangeRate) == 0 ? "1" : S_DTO.SOH_ExchangeRate;
                                SO_DTO.SO_OtherRemarks = S_DTO.SOH_OtherRemarks;
                                SO_DTO.SO_TotalAmount = Convert.ToDouble(S_DTO.SOH_TotalAmount);
                                SO_DTO.SO_TotalItemIncome = Convert.ToDouble(S_DTO.SOH_TotalItemIncome);
                                SO_DTO.SO_TotalHeadIncome = Convert.ToDouble(S_DTO.SOH_TotalHeadIncome);
                                SO_DTO.SO_OrderValue = Convert.ToDouble(S_DTO.SOH_OrderValue);
                                SO_DTO.SO_Id = 121;
                                DS = SO_DAO.SaleOrderDB(SO_DTO);

                                String ItemDTO = string.Join(", ", ITM_DTO.Where(x => Convert.ToInt64(x.SOI_Number) != 0).Select(x => x.SOI_Number));
                                String ItemIncomeDTO = string.Join(", ", ItemIncome_DTO.Where(x => Convert.ToInt64(x.SOI_INC_Number) != 0).Select(x => x.SOI_INC_Number));
                                String IncomeDTO = string.Join(", ", Income_DTO.Where(x => Convert.ToInt64(x.SOH_INC_Number) != 0).Select(x => x.SOH_INC_Number));
                                String AddressDTO = string.Join(", ", BuyerAddress_DTO.Where(x => Convert.ToInt64(x.SOH_ADD_Number) != 0).Select(x => x.SOH_ADD_Number));

                                SO_DTO.SO_DeleteNumbers = Convert.ToString(ItemDTO);
                                SO_DTO.SO_Id = 101;
                                SO_DAO.SaleOrderDB(SO_DTO);

                                SO_DTO.SO_DeleteNumbers = Convert.ToString(IncomeDTO);
                                SO_DTO.SO_Id = 102;
                                SO_DAO.SaleOrderDB(SO_DTO);

                                SO_DTO.SO_DeleteNumbers = Convert.ToString(ItemIncomeDTO);
                                SO_DTO.SO_Id = 103;
                                SO_DAO.SaleOrderDB(SO_DTO);

                                SO_DTO.SO_DeleteNumbers = Convert.ToString(AddressDTO);
                                SO_DTO.SO_Id = 104;
                                SO_DAO.SaleOrderDB(SO_DTO);

                                foreach (var Item in ITM_DTO)
                                {
                                    DataSet D = new DataSet();
                                    SO_DTO.SO_ITM_Number = Convert.ToInt64(Item.SOI_ITM_Number);
                                    SO_DTO.SO_UoM_Number = Convert.ToInt64(Item.SOI_UoM_Number);
                                    SO_DTO.SO_Qty = Convert.ToDouble(Item.SOI_Qty);
                                    SO_DTO.SO_UnitPrice = Convert.ToDouble(Item.SOI_UnitPrice);
                                    SO_DTO.SO_Amount = Convert.ToDouble(Item.SOI_Amount);
                                    SO_DTO.SO_IncomeValue = Convert.ToDouble(Item.SOI_IncomeValue);
                                    SO_DTO.SO_DeliveryDate = Convert.ToInt32(Convert.ToDateTime(Item.SOI_DeliveryDate).ToString("yyyyMMdd"));
                                    if (Item.SOI_Number == 0)
                                    {
                                        SO_DTO.SO_Id = 12;
                                        D = SO_DAO.SaleOrderDB(SO_DTO);

                                        var ItemIncome = ItemIncome_DTO.Where(x => (x.SOI_INC_ITM_Number == Item.SOI_ITM_Number) && (x.SOI_INC_ITM_Index == Item.SOI_Index));

                                        foreach (var ItemInc in ItemIncome)
                                        {
                                            SO_DTO.SO_I_Number = Convert.ToInt64(D.Tables[0].Rows[0][0]);
                                            SO_DTO.SO_ITM_Number = Convert.ToInt64(ItemInc.SOI_INC_ITM_Number);
                                            SO_DTO.SO_INC_MIC_Number = Convert.ToInt64(ItemInc.SOI_INC_MIC_Number);
                                            SO_DTO.SO_INC_Remarks = ItemInc.SOI_INC_Remarks;
                                            SO_DTO.SO_INC_OCRN_Number = Convert.ToInt64(ItemInc.SOI_INC_OCRN_Number);
                                            SO_DTO.SO_INC_CM_Number = Convert.ToInt64(ItemInc.SOI_INC_CM_Number);
                                            SO_DTO.SO_INC_IncomeBase = Convert.ToDouble(ItemInc.SOI_INC_IncomeBase);
                                            SO_DTO.SO_INC_IncomeValue = Convert.ToDouble(ItemInc.SOI_INC_IncomeValue);
                                            SO_DTO.SO_INC_ALCT_Number = Convert.ToInt64(ItemInc.SOI_INC_ALCT_Number);
                                            SO_DTO.SO_INC_LA_Number = Convert.ToInt64(ItemInc.SOI_INC_LA_Number);
                                            if (ItemInc.SOI_INC_Number == 0)
                                            {
                                                SO_DTO.SO_Id = 14;
                                            }
                                            else
                                            {
                                                SO_DTO.SO_INC_Number = Convert.ToInt64(ItemInc.SOI_INC_Number);
                                                SO_DTO.SO_Id = 106;
                                            }
                                            SO_DAO.SaleOrderDB(SO_DTO);
                                        }
                                    }
                                    else
                                    {
                                        SO_DTO.SO_I_Number = Convert.ToInt64(Item.SOI_Number);
                                        SO_DTO.SO_Id = 105;
                                        D = SO_DAO.SaleOrderDB(SO_DTO);

                                        var ItemIncome = ItemIncome_DTO.Where(x => (x.SOI_INC_SOI_Number == Item.SOI_Number));

                                        foreach (var ItemInc in ItemIncome)
                                        {
                                            SO_DTO.SO_ITM_Number = Convert.ToInt64(ItemInc.SOI_INC_ITM_Number);
                                            SO_DTO.SO_INC_MIC_Number = Convert.ToInt64(ItemInc.SOI_INC_MIC_Number);
                                            SO_DTO.SO_INC_Remarks = ItemInc.SOI_INC_Remarks;
                                            SO_DTO.SO_INC_OCRN_Number = Convert.ToInt64(ItemInc.SOI_INC_OCRN_Number);
                                            SO_DTO.SO_INC_CM_Number = Convert.ToInt64(ItemInc.SOI_INC_CM_Number);
                                            SO_DTO.SO_INC_IncomeBase = Convert.ToDouble(ItemInc.SOI_INC_IncomeBase);
                                            SO_DTO.SO_INC_IncomeValue = Convert.ToDouble(ItemInc.SOI_INC_IncomeValue);
                                            SO_DTO.SO_INC_ALCT_Number = Convert.ToInt64(ItemInc.SOI_INC_ALCT_Number);
                                            SO_DTO.SO_INC_LA_Number = Convert.ToInt64(ItemInc.SOI_INC_LA_Number);
                                            if (ItemInc.SOI_INC_Number == 0)
                                            {
                                                SO_DTO.SO_Id = 14;
                                            }
                                            else
                                            {
                                                SO_DTO.SO_INC_Number = Convert.ToInt64(ItemInc.SOI_INC_Number);
                                                SO_DTO.SO_Id = 106;
                                            }
                                            SO_DAO.SaleOrderDB(SO_DTO);
                                        }
                                    }
                                }
                                foreach (var Income in Income_DTO)
                                {
                                    SO_DTO.SO_INC_MIC_Number = Convert.ToInt64(Income.SOH_INC_MIC_Number);
                                    SO_DTO.SO_INC_Remarks = Income.SOH_INC_Remarks;
                                    SO_DTO.SO_INC_OCRN_Number = Convert.ToInt64(Income.SOH_INC_OCRN_Number);
                                    SO_DTO.SO_INC_CM_Number = Convert.ToInt64(Income.SOH_INC_CM_Number);
                                    SO_DTO.SO_INC_IncomeBase = Convert.ToDouble(Income.SOH_INC_IncomeBase);
                                    SO_DTO.SO_INC_IncomeValue = Convert.ToDouble(Income.SOH_INC_IncomeValue);
                                    SO_DTO.SO_INC_ALCT_Number = Convert.ToInt64(Income.SOH_INC_ALCT_Number);
                                    SO_DTO.SO_INC_LA_Number = Convert.ToInt64(Income.SOH_INC_LA_Number);
                                    if (Income.SOH_INC_Number == 0)
                                    {
                                        SO_DTO.SO_Id = 13;
                                    }
                                    else
                                    {
                                        SO_DTO.SO_INC_Number = Convert.ToInt64(Income.SOH_INC_Number);
                                        SO_DTO.SO_Id = 107;
                                    }
                                    SO_DAO.SaleOrderDB(SO_DTO);
                                }

                                foreach (var BuyerAddress in BuyerAddress_DTO)
                                {
                                    SO_DTO.SO_ADD_ADTP_Number = Convert.ToInt64(BuyerAddress.SOH_ADD_ADTP_Number);
                                    SO_DTO.SO_ADD_AddressID = Convert.ToString(BuyerAddress.SOH_ADD_AddressID);
                                    SO_DTO.SO_ADD_Address = Convert.ToString(BuyerAddress.SOH_ADD_Address);
                                    SO_DTO.SO_ADD_City = Convert.ToString(BuyerAddress.SOH_ADD_City);
                                    SO_DTO.SO_ADD_State = Convert.ToString(BuyerAddress.SOH_ADD_State);
                                    SO_DTO.SO_ADD_Country = Convert.ToString(BuyerAddress.SOH_ADD_Country);
                                    SO_DTO.SO_ADD_Pin = Convert.ToString(BuyerAddress.SOH_ADD_Pin);
                                    SO_DTO.SO_ADD_GSTIN = Convert.ToString(BuyerAddress.SOH_ADD_GSTIN);
                                    if (BuyerAddress.SOH_ADD_Number == 0)
                                    {
                                        SO_DTO.SO_Id = 15;
                                    }
                                    else
                                    {
                                        SO_DTO.SO_ADD_Number = Convert.ToInt64(BuyerAddress.SOH_ADD_Number);
                                        SO_DTO.SO_Id = 125;
                                    }
                                    SO_DAO.SaleOrderDB(SO_DTO);
                                }

                                transaction.Complete();

                                S_Head_DTO.Reset();
                                Income_DTO = null;
                                ITM_DTO = null;
                                ItemIncome_DTO = null;
                                S_DTO.Reset();
                                Original_DTO = Help.JsonClone(S_DTO);
                                return RedirectToAction("SaleOrderRegisterSummary");
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








        //Sale Order summary
        [Route("sale/transactions/sale-order-register-summary")]
        public IActionResult SaleOrderRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                SOR_List = SOSummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList_DTO<SalesOrderRegister_DTO>.CreateAsync(SOR_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("sale/transactions/sale-order-register-summary")]
        [HttpPost]
        public IActionResult SaleOrderRegisterSummary(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? DeleteNumbers, String? SO_No, String[] DeleteNumber, String selectAllCheckbox)
        {
                if (Mode == "DeleteAll")
                {
                    SO_DTO.SO_DeleteNumbers = DeleteNumbers;
                    SO_DTO.SO_Id = 31;
                    SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
                    DS = SO_DAO.SaleOrderDB(SO_DTO);
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

                    //SO_DTO.SO_Id = 21;
                    //SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
                    //DS = SO_DAO.SaleOrderDB(SO_DTO);

                    //if (Mode == "Ascii")
                    //{
                    //    List<SaleOrderAscii> SOR_List = P_DL.POAscii(DS.Tables[0]);

                    //    var Key = SOR_List;
                    //    if (selectAllCheckbox == "on")
                    //    {
                    //        if (!String.IsNullOrEmpty(Search))
                    //        {
                    //            Key = Key.Where(K => K.SOH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Currency!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                    //        }
                    //    }
                    //    else if (DeleteNumber.Length > 0)
                    //    {
                    //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SOH_Number)).ToList();
                    //    }
                    //    else
                    //    {
                    //        if (!String.IsNullOrEmpty(Search))
                    //        {
                    //            Key = Key.Where(K => K.SOH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Buyer!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Currency!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
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

                    //    var HeaderRow = typeof(SaleOrderAscii)
                    //            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    //            .Where(prop => prop.Name != nameof(SaleOrderAscii.SOH_Number))
                    //            .Select(prop =>
                    //                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                    //             )
                    //            .ToList();

                    //    var AsciiData = new StringBuilder();
                    //    AsciiData.AppendLine(string.Join("\t", HeaderRow));



                    //    PropertyInfo[] PropertiesToInclude = typeof(SaleOrderAscii)
                    //        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    //        .Where(prop => prop.Name != nameof(SaleOrderAscii.SOH_Number))
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

                    //        if (!TotalLabelAdded && prop.Name != nameof(SaleOrderAscii.TotalQty) &&
                    //                                 prop.Name != nameof(SaleOrderAscii.MaterialValue) &&
                    //                                 prop.Name != nameof(SaleOrderAscii.ItemMiscIncome) &&
                    //                                 prop.Name != nameof(SaleOrderAscii.HeadMiscIncome))
                    //        {
                    //            if (FooterCells.Count == 0)
                    //            {
                    //                FooterCellValue = "Total:";
                    //                TotalLabelAdded = true;
                    //            }
                    //        }


                    //        switch (prop.Name)
                    //        {
                    //            case nameof(SaleOrderAscii.TotalQty):
                    //                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                    //                break;
                    //            case nameof(SaleOrderAscii.MaterialValue):
                    //                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                    //                break;
                    //            case nameof(SaleOrderAscii.ItemMiscIncome):
                    //                FooterCellValue = TotalItemMiscIncomeSum.ToString("N2", CultureInfo.InvariantCulture);
                    //                break;
                    //            case nameof(SaleOrderAscii.HeadMiscIncome):
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
                    //    SOR_List = S_DL.SOList(DS.Tables[0]);

                    //    var Key = SOR_List.ToList();
                    //    if (selectAllCheckbox == "on")
                    //    {
                    //        if (!String.IsNullOrEmpty(Search))
                    //        {
                    //            Key = Key.Where(K => K.SOH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                    //        }
                    //    }
                    //    else if (DeleteNumber.Length > 0)
                    //    {
                    //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SOH_Number)).ToList();
                    //    }
                    //    else
                    //    {
                    //        if (!String.IsNullOrEmpty(Search))
                    //        {
                    //            Key = Key.Where(K => K.SOH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
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
                    //    SOR_List = P_DL.POList(DS.Tables[0]);

                    //    var Key = SOR_List.ToList();
                    //    if (selectAllCheckbox == "on")
                    //    {
                    //        if (!String.IsNullOrEmpty(Search))
                    //        {
                    //            Key = Key.Where(K => K.SOH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.TotalItem!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.TotalQty!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.MaterialValue!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.ItemMiscIncome!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.HeadMiscIncome!.ToLower().Contains(Search.ToLower())).ToList();
                    //        }
                    //    }
                    //    else if (DeleteNumber.Length > 0)
                    //    {
                    //        Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SOH_Number)).ToList();
                    //    }
                    //    else
                    //    {
                    //        if (!String.IsNullOrEmpty(Search))
                    //        {
                    //            Key = Key.Where(K => K.SOH_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Buyer_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_Currency_Number!.ToLower().Contains(Search.ToLower()) ||
                    //                 K.SOH_MS_Number!.ToLower().Contains(Search.ToLower()) ||
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
                    //                border: 1px solid #ccc;
                    //                font-size: 0.6rem;
                    //                font-family: ""Noto Sans"", sans-serif;
                    //            }}
                    //                .table th {{
                    //                    border: 1px solid #ccc;
                    //                    padding: 5px 8px;
                    //                    vertical-align: top;
                    //                background-color: #000;
                    //                    color:#fff;
                    //                }}

                    //                .table td {{
                    //                    border: 1px solid #ccc;
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
                    //            Export = (Row.SO_ExportOrder.ToString() == "1") ? "Yes" : "No"; 

                    //            String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.MaterialValue));
                    //            String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.ItemMiscIncome));
                    //            String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.HeadMiscIncome));
                    //            PDFDownload += $@"<tr>
                    //            <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.SO_OrderDate).ToString("dd-MMM-yyyy")}</td>
                    //            <td>{Row.SO_OrderNo}</td>
                    //            <td>{Row.SO_BUY_Name}</td>
                    //            <td style='width:30px;text-align:center'>{Row.SO_CUR_Name}</td>
                    //            <td style='width:30px;text-align:center'>{Export}</td>
                    //            <td>{Row.SO_MS_Name}</td>
                    //            <td style='width:30px;text-align:center'>{Row.TotalItem}</td>
                    //            <td style='width:30px;text-align:center'>{Row.TotalQty}</td>
                    //            <td style='width:70px;text-align:right'>{Matrial}</td>
                    //            <td style='width:70px;text-align:right'>{ItemExp}</td>
                    //            <td style='width:70px;text-align:right'>{HeadExp}</td>
                    //            </tr>";


                    //            if (Decimal.TryParse(Row.SO_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    //            {
                    //                TotalQtySum += QtyValue;
                    //            }
                    //            if (Decimal.TryParse(Row.SO_TotalAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    //            {
                    //                TotalMaterialValueSum += MaterialValue;
                    //            }
                    //            if (Decimal.TryParse(Row.SO_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    //            {
                    //                TotalItemMiscIncomeSum += ItemMiscValue;
                    //            }
                    //            if (Decimal.TryParse(Row.SO_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
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

                    //    return File(memoryStream.ToArray(), "application/pdf", "SO_Download.pdf");
                    //}
                }
                else if (Mode == "View")
                {
                    return RedirectToAction("PreviewSaleOrder", new { SO_No = SO_No });
                }
                else if (Mode == "Edit")
                {
                    return RedirectToAction("EditSaleOrder", new { SO_No = SO_No });
                }

                SOR_List = SOSummaryGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList_DTO<SalesOrderRegister_DTO>.CreateAsync(SOR_List, DPageNumber ?? 1, DPageSize));
        }
        DataTable SummaryDownload(List<SalesOrderRegister_DTO> SOR_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "sale-order-summary";

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

            foreach (var Product in SOR_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["Date"] = Product.SO_OrderDate;
                NewRow["Order Number"] = Product.SO_OrderNo;
                NewRow["Buyer Name"] = Product.SO_BUY_Name;
                NewRow["Currency"] = Product.SO_CUR_Name;
                NewRow["Export Order"] = (Product.SO_ExportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Material Segregation"] = Product.SO_MS_Name;
                NewRow["No. of Item"] = Product.SO_NoOfItem;
                NewRow["Qty"] = Product.SO_Qty;
                NewRow["Amount"] = Product.SO_TotalAmount;
                NewRow["Item Misc Income Value"] = Product.SO_TotalItemIncome;
                NewRow["Head Misc Income Value"] = Product.SO_TotalHeadIncome;

                TableDown.Rows.Add(NewRow);


                if (Decimal.TryParse(Product.SO_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.SO_TotalAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.SO_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscIncomeSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.SO_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
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
        List<SalesOrderRegister_DTO> SOSummaryGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            SO_DTO.SO_Id = 21;
            SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            SOR_List = S_DL.SOSummaryList(DS.Tables[0]);

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

            var Key = SOR_List.OrderByDescending(Cs => Cs.SO_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.SO_OrderDate.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SO_OrderNo.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SO_BUY_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SO_CUR_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SO_MS_Name.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SO_NoOfItem.ToString().ToLower().Contains(Search.ToLower()) ||
                 K.SO_Qty.ToString().ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SO_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.SO_OrderDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.SO_OrderDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.SO_Number);
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

            ViewBag.SumOfItem = Key.Sum(item => Convert.ToDouble(item.SO_NoOfItem));
            ViewBag.SumOfQty = Key.Sum(item => Convert.ToDouble(item.SO_Qty));
            ViewBag.SumOfAmount = Key.Sum(item => Convert.ToDouble(item.SO_TotalAmount));
            ViewBag.SumOfItemIncome = Key.Sum(item => Convert.ToDouble(item.SO_TotalItemIncome));
            ViewBag.SumOfHeadIncome = Key.Sum(item => Convert.ToDouble(item.SO_TotalHeadIncome));
            ViewBag.SumOfOrderValue = Key.Sum(item => Convert.ToDouble(item.SO_OrderValue));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }


        [Route("sale/transactions/sale-order-register-summary/print")]
        public String SOSummaryPrint(String Search, String SelectedItem, bool AllItem)
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
            SO_DTO.SO_Id = 21;
            SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            SOR_List = S_DL.SOSummaryList(DS.Tables[0]);

            var Key = SOR_List.OrderByDescending(Cs => Cs.SO_Number);

            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.SO_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_NoOfItem!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_Qty!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_TotalAmount!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SO_Number);
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SO_Number)).OrderByDescending(K => K.SO_Number);
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.SO_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_NoOfItem!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_Qty!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_TotalAmount!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                     K.SO_TotalHeadIncome!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SO_Number);
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

            PDFDownload += $@"<table class='table'><tr><th style='width:70px;text-align:center'>Date</th><th>Order Number</th><th>Buyer Name</th><th style='width:30px;text-align:center'>Currency</th><th style='width:30px;text-align:center'>Export Order</th><th>Material Segregation</th><th style='width:30px;text-align:center'>No. of Line Item</th><th style='width:30px;text-align:center'>Qty</th><th style='width:70px'>Amount</th><th style='width:70px'>Item Misc.Income Value</th><th style='width:70px'>Header Misc.Income Value</th></tr>";

            Decimal TotalQtySum = 0;
            Decimal TotalAmountSum = 0;
            Decimal TotalItemMiscIncomeSum = 0;
            Decimal TotalHeadMiscIncomeSum = 0;

            if (Key.ToList().Count > 0)
            {
                foreach (var Row in Key.ToList())
                {
                    String Amount = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SO_TotalAmount));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SO_TotalItemIncome));
                    String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SO_TotalHeadIncome));
                    PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.SO_OrderDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.SO_OrderNo}</td>
                                <td>{Row.SO_BUY_Name}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_CUR_Name}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_ExportOrder}</td>
                                <td>{Row.SO_MS_Name}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_NoOfItem}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_Qty}</td>
                                <td style='width:70px;text-align:right'>{Amount}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                </tr>";


                    if (Decimal.TryParse(Row.SO_Qty, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.SO_TotalAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalAmountSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.SO_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscIncomeSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.SO_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
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






        //Sale Order Detailed
        [Route("sale/transactions/sale-order-register-detailed")]
        public IActionResult SaleOrderRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
                SOR_List = SODetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList_DTO<SalesOrderRegister_DTO>.CreateAsync(SOR_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("sale/transactions/sale-order-register-detailed")]
        [HttpPost]
        public IActionResult SaleOrderRegisterDetailed(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, String? Mode, String? SOR_No, String[] DeleteNumber, String selectAllCheckbox)
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

                    //SO_DTO.Id = 22;
                    //SO_DTO.CreatorCode = Convert.ToInt32(UserCode);
                    //DS = SO_DAO.SaleOrderDB(SO_DTO);

                    //if (Mode == "Ascii")
                    //{
                    //    List<SaleOrderDetailAscii> SOR_List = P_DL.PODetailAscii(DS.Tables[0]);

                    //    var Key = SOR_List;
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

                    //    var HeaderRow = typeof(SaleOrderDetailAscii)
                    //            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    //            .Where(prop => prop.Name != nameof(SaleOrderDetailAscii.POH_Number))
                    //            .Select(prop =>
                    //                prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name
                    //             )
                    //            .ToList();

                    //    var AsciiData = new StringBuilder();
                    //    AsciiData.AppendLine(string.Join("\t", HeaderRow));


                    //    PropertyInfo[] PropertiesToInclude = typeof(SaleOrderDetailAscii)
                    //        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    //        .Where(prop => prop.Name != nameof(SaleOrderDetailAscii.POH_Number))
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

                    //        if (!TotalLabelAdded && prop.Name != nameof(SaleOrderDetailAscii.POI_Qty) &&
                    //                                 prop.Name != nameof(SaleOrderDetailAscii.POI_Amount) &&
                    //                                 prop.Name != nameof(SaleOrderDetailAscii.ItemMiscExpense) &&
                    //                                 prop.Name != nameof(SaleOrderDetailAscii.HeadMiscExpense))
                    //        {
                    //            if (FooterCells.Count == 0)
                    //            {
                    //                FooterCellValue = "Total:";
                    //                TotalLabelAdded = true;
                    //            }
                    //        }

                    //        switch (prop.Name)
                    //        {
                    //            case nameof(SaleOrderDetailAscii.POI_Qty):
                    //                FooterCellValue = TotalQtySum.ToString("N0", CultureInfo.InvariantCulture);
                    //                break;
                    //            case nameof(SaleOrderDetailAscii.POI_Amount):
                    //                FooterCellValue = TotalMaterialValueSum.ToString("N2", CultureInfo.InvariantCulture);
                    //                break;
                    //            case nameof(SaleOrderDetailAscii.ItemMiscExpense):
                    //                FooterCellValue = TotalItemMiscExpenseSum.ToString("N2", CultureInfo.InvariantCulture);
                    //                break;
                    //            case nameof(SaleOrderDetailAscii.HeadMiscExpense):
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
                    //    SOR_List = P_DL.PODetailList(DS.Tables[0]);

                    //    var Key = SOR_List.ToList();
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
                    //    SOR_List = P_DL.PODetailList(DS.Tables[0]);

                    //    var Key = SOR_List.ToList();
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
                    //                border: 1px solid #ccc;
                    //                font-size: 0.5rem;
                    //                font-family: ""Noto Sans"", sans-serif;
                    //            }}
                    //                .table th {{
                    //                    border: 1px solid #ccc;
                    //                    padding: 5px 8px;
                    //                    vertical-align: top;
                    //                background-color: #000;
                    //                    color:#fff;
                    //                }}

                    //                .table td {{
                    //                    border: 1px solid #ccc;
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
                    SO_DTO.SO_Number = Convert.ToInt32(SOR_No);
                    SO_DTO.SO_Id = 42;
                    SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
                    DS = SO_DAO.SaleOrderDB(SO_DTO);

                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        return RedirectToAction("PreviewSaleOrder", new { SOR_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                }
                else if (Mode == "Edit")
                {
                    SO_DTO.SO_Number = Convert.ToInt32(SOR_No);
                    SO_DTO.SO_Id = 42;
                    SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
                    DS = SO_DAO.SaleOrderDB(SO_DTO);

                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        return RedirectToAction("EditSaleOrder", new { SOR_No = DS.Tables[0].Rows[0][0].ToString() });
                    }
                }

                SOR_List = SODetailedGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
                return View(PaginatedList_DTO<SalesOrderRegister_DTO>.CreateAsync(SOR_List, DPageNumber ?? 1, DPageSize));
        }
        List<SalesOrderRegister_DTO> SODetailedGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            SO_DTO.SO_Id = 22;
            SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            SOR_List = S_DL.SODetailedList(DS.Tables[0]);

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

            var Key = SOR_List.OrderByDescending(Cs => Cs.SO_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.SO_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_UOM_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SO_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SO_TotalAmount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SO_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_TotalHeadIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_DeliveryDate!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.SO_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => Convert.ToDateTime(K.SO_OrderDate)!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => Convert.ToDateTime(K.SO_OrderDate)!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.SO_Number);
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

            ViewBag.SumOfQty = Key.Sum(item => Convert.ToDouble(item.SO_Qty));
            ViewBag.SumOfAmount = Key.Sum(item => Convert.ToDouble(item.SO_TotalAmount));
            ViewBag.SumOfItemIncome = Key.Sum(item => Convert.ToDouble(item.SO_TotalItemIncome));
            ViewBag.SumOfHeadIncome = Key.Sum(item => Convert.ToDouble(item.SO_TotalHeadIncome));
            ViewBag.SumOfOrderValue = Key.Sum(item => Convert.ToDouble(item.SO_OrderValue));

            ViewBag.Page = Help.PageSize(PSize.ToString());
            ViewData["PageNumber"] = DPageNumber;
            ViewData["PageSize"] = DPageSize;
            ViewData["PageCount"] = PageCounts;
            ViewData["TotalSize"] = Key.ToList().Count;

            return Key.ToList();
        }
        DataTable SODetailsDownload(List<SalesOrderRegister_DTO> SOR_List)
        {
            DataTable Dt = DS.Tables[0];
            DataTable TableDown = new DataTable();
            TableDown.TableName = "sale-order-detailed";

            TableDown.Clear();
            TableDown.Columns.Add("Date");
            TableDown.Columns.Add("Order Number");
            TableDown.Columns.Add("Buyer Name");
            TableDown.Columns.Add("Currency");
            TableDown.Columns.Add("Import Order");
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

            foreach (var Product in SOR_List)
            {
                DataRow NewRow = TableDown.NewRow();
                NewRow["Date"] = Product.SO_OrderDate;
                NewRow["Order Number"] = Product.SO_OrderNo;
                NewRow["Buyer Name"] = Product.SO_BUY_Name;
                NewRow["Currency"] = Product.SO_CUR_Name;
                NewRow["Import Order"] = (Product.SO_ExportOrder.ToString() == "1") ? "Yes" : "No";
                NewRow["Material Segregation"] = Product.SO_MS_Name;
                NewRow["Item Group"] = Product.SO_ITM_Group;
                NewRow["Item Code"] = Product.SO_ITM_Code;
                NewRow["Item Description"] = Product.SO_ITM_Description;
                NewRow["UoM"] = Product.SO_UOM_Name;
                NewRow["Qty"] = Product.SO_Qty;
                NewRow["Unit Price"] = Product.SO_UnitPrice;
                NewRow["Amount"] = Product.SO_TotalAmount;
                NewRow["Item Misc Expense Value"] = Product.SO_TotalItemIncome;
                NewRow["Head Misc Expense Value"] = Product.SO_TotalHeadIncome;
                NewRow["Delivery Date"] = Product.SO_DeliveryDate;

                TableDown.Rows.Add(NewRow);

                if (Decimal.TryParse(Product.SO_Qty.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                {
                    TotalQtySum += QtyValue;
                }
                if (Decimal.TryParse(Product.SO_TotalAmount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                {
                    TotalMaterialValueSum += MaterialValue;
                }
                if (Decimal.TryParse(Product.SO_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                {
                    TotalItemMiscIncomeSum += ItemMiscValue;
                }
                if (Decimal.TryParse(Product.SO_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
                {
                    TotalHeadMiscIncomeSum += HeadMiscValue;
                }
            }

            DataRow NewRows = TableDown.NewRow();
            NewRows["Date"] = "Total";
            NewRows["Order Number"] = "";
            NewRows["Buyer Name"] = "";
            NewRows["Currency"] = "";
            NewRows["Import Order"] = "";
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
            NewRows["Delivery Date"] = "";
            TableDown.Rows.Add(NewRows);

            return TableDown;
        }

        [Route("sale/transactions/sale-order-register-detailed/print")]
        public String SODetailedPrint(String Search, String SelectedItem, bool AllItem)
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
            SO_DTO.SO_Id = 22;
            SO_DTO.SO_CreatorCode = Convert.ToInt32(UserCode);
            DS = SO_DAO.SaleOrderDB(SO_DTO);
            SOR_List = S_DL.SODetailedList(DS.Tables[0]);

            var Key = SOR_List.ToList();

            if (AllItem)
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.SO_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_UOM_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SO_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SO_TotalAmount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SO_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_TotalHeadIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                Key = Key.Where(K => AllowedPoNumbersSet.Contains(K.SO_Number)).ToList();
            }
            else
            {
                if (!String.IsNullOrEmpty(Search))
                {
                    Key = Key.Where(K => K.SO_OrderDate!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_OrderNo!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_BUY_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_CUR_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_MS_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_ITM_Code!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_ITM_Group!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_ITM_Description!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_UOM_Name!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_Qty!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SO_UnitPrice!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SO_TotalAmount!.ToString().ToLower().Contains(Search.ToLower()) ||
                         K.SO_TotalItemIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_TotalHeadIncome!.ToLower().Contains(Search.ToLower()) ||
                         K.SO_DeliveryDate!.ToLower().Contains(Search.ToLower())).ToList();
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
                                    font-size: 0.5rem;
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
                    
                    String Matrial = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SO_TotalAmount));
                    String ItemExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SO_TotalItemIncome));
                    String HeadExp = string.Format(India, "{0:N2}", Convert.ToDouble(Row.SO_TotalHeadIncome));

                    PDFDownload += $@"<tr>
                                <td style='width:60px;text-align:center'>{Convert.ToDateTime(Row.SO_OrderDate).ToString("dd-MMM-yyyy")}</td>
                                <td>{Row.SO_OrderNo}</td>
                                <td>{Row.SO_BUY_Name}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_CUR_Name}</td>
                                <td style='width:30px;text-align:center'>{Import}</td>
                                <td>{Row.SO_MS_Name}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_ITM_Code}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_ITM_Group}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_ITM_Description}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_UOM_Name}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_Qty}</td>
                                <td style='width:30px;text-align:center'>{Row.SO_UnitPrice}</td>
                                <td style='width:70px;text-align:right'>{Matrial}</td>
                                <td style='width:70px;text-align:right'>{ItemExp}</td>
                                <td style='width:70px;text-align:right'>{HeadExp}</td>
                                <td style='width:70px;text-align:right'>{Row.SO_DeliveryDate}</td>
                                </tr>";


                    if (Decimal.TryParse(Row.SO_Qty.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal QtyValue))
                    {
                        TotalQtySum += QtyValue;
                    }
                    if (Decimal.TryParse(Row.SO_TotalAmount.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal MaterialValue))
                    {
                        TotalMaterialValueSum += MaterialValue;
                    }
                    if (Decimal.TryParse(Row.SO_TotalItemIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal ItemMiscValue))
                    {
                        TotalItemMiscExpenseSum += ItemMiscValue;
                    }
                    if (Decimal.TryParse(Row.SO_TotalHeadIncome, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal HeadMiscValue))
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
                                <td></td>
                                </tr>";


            PDFDownload += $@"</table></body></html>";

            return PDFDownload;
        }








        //Sale Order Numbering
        [Route("sale/setup/sale-order-numbering")]
        public IActionResult SONumbering()
        {
                GetSONumber();
                return View(SON_DTO);
        }

        [Route("sale/setup/Sale-order-numbering")]
        [HttpPost]
        public IActionResult SONumbering(SONumber_DTO PN_DTO)
        {
                bool IsValid = false;
                SONumber_DTO P_Head_DTO = new SONumber_DTO();

                List<SONumberReset_DTO>? Reset_DTO = new List<SONumberReset_DTO>();
                List<SONumberPrefix_DTO>? Prefix_DTO = new List<SONumberPrefix_DTO>();
                List<SONumberSuffix_DTO>? Suffix_DTO = new List<SONumberSuffix_DTO>();

                P_Head_DTO = SON_DTO;

                if (PN_DTO.SONumberReset != null)
                    Reset_DTO = PN_DTO.SONumberReset!.Where(K => !K.SOR_IsDeleted).ToList();

                if (PN_DTO.SONumberPrefix != null)
                    Prefix_DTO = PN_DTO.SONumberPrefix!.Where(K => !K.SOP_IsDeleted).ToList();

                if (PN_DTO.SONumberSuffix != null)
                    Suffix_DTO = PN_DTO.SONumberSuffix!.Where(K => !K.SOS_IsDeleted).ToList();

                if (PN_DTO.SON_Method == "2")
                {
                    String ResetDTO = string.Join(", ", Reset_DTO.Where(x => Convert.ToInt64(x.SOR_Number) != 0).Select(x => x.SOR_Number));
                    String PrefixDTO = string.Join(", ", Prefix_DTO.Where(x => Convert.ToInt64(x.SOP_Number) != 0).Select(x => x.SOP_Number));
                    String SuffixDTO = string.Join(", ", Suffix_DTO.Where(x => Convert.ToInt64(x.SOS_Number) != 0).Select(x => x.SOS_Number));

                    SON_DTO.SON_DeleteNumbers = Convert.ToString(ResetDTO);
                    SON_DTO.SON_Id = 31;
                    SON_DAO.SONumberDB(SON_DTO);

                    SON_DTO.SON_DeleteNumbers = Convert.ToString(PrefixDTO);
                    SON_DTO.SON_Id = 32;
                    SON_DAO.SONumberDB(SON_DTO);

                    SON_DTO.SON_DeleteNumbers = Convert.ToString(SuffixDTO);
                    SON_DTO.SON_Id = 33;
                    SON_DAO.SONumberDB(SON_DTO);

                    SON_DTO.SON_CreatorCode = Convert.ToInt32(UserCode);

                    SON_DTO.SON_Method = PN_DTO.SON_Method;
                    if (PN_DTO.SON_Number == 0)
                    {
                        SON_DTO.SON_Id = 11;
                    }
                    else
                    {
                        SON_DTO.SON_Id = 41;
                        SON_DTO.SON_Number = PN_DTO.SON_Number;
                    }
                    SON_DAO.SONumberDB(SON_DTO);

                    foreach (var Reset in Reset_DTO)
                    {
                        SON_DTO.SON_Date = Convert.ToString(Convert.ToDateTime(Reset.SOR_Date).ToString("yyyyMMdd"));
                        SON_DTO.SON_StartingNumber = Convert.ToInt32(Reset.SOR_StartingNumber).ToString();
                        SON_DTO.SON_NumberofDigits = Convert.ToInt32(Reset.SOR_NumberofDigits).ToString();
                        SON_DTO.SON_PrefilZero = Convert.ToInt64(Reset.SOR_PrefilZero).ToString();
                        SON_DTO.SON_Frequency = Convert.ToInt64(Reset.SOR_Frequency).ToString();
                        if (Reset.SOR_Number == 0)
                        {
                            SON_DTO.SON_Id = 12;
                        }
                        else
                        {
                            SON_DTO.SON_Id = 42;
                            SON_DTO.SON_Number = Reset.SOR_Number;
                        }
                        SON_DAO.SONumberDB(SON_DTO);
                    }

                    foreach (var Prefix in Prefix_DTO)
                    {
                        SON_DTO.SON_Date = Convert.ToString(Convert.ToDateTime(Prefix.SOP_Date).ToString("yyyyMMdd"));
                        SON_DTO.SON_Particulars = Convert.ToString(Prefix.SOP_Particulars);
                        if (Prefix.SOP_Number == 0)
                        {
                            SON_DTO.SON_Id = 13;
                        }
                        else
                        {
                            SON_DTO.SON_Id = 43;
                            SON_DTO.SON_Number = Prefix.SOP_Number;
                        }
                        SON_DAO.SONumberDB(SON_DTO);
                    }

                    foreach (var Suffix in Suffix_DTO)
                    {
                        SON_DTO.SON_Date = Convert.ToString(Convert.ToDateTime(Suffix.SOS_Date).ToString("yyyyMMdd"));
                        SON_DTO.SON_Particulars = Convert.ToString(Suffix.SOS_Particulars);
                        if (Suffix.SOS_Number == 0)
                        {
                            SON_DTO.SON_Id = 14;
                        }
                        else
                        {
                            SON_DTO.SON_Id = 44;
                            SON_DTO.SON_Number = Suffix.SOS_Number;
                        }
                        SON_DAO.SONumberDB(SON_DTO);
                    }
                    SON_DTO.Reset();
                    Reset_DTO = null;
                    Prefix_DTO = null;
                    Suffix_DTO = null;
                    ModelState.Clear();
                }
                else if (PN_DTO.SON_Method == "3")
                {
                    SON_DTO.SON_Method = PN_DTO.SON_Method;
                    if (PN_DTO.SON_Number == 0)
                    {
                        SON_DTO.SON_Id = 21;
                    }
                    else
                    {
                        SON_DTO.SON_Id = 22;
                        SON_DTO.SON_Number = PN_DTO.SON_Number;
                    }
                    SON_DAO.SONumberDB(SON_DTO);
                }

                GetSONumber();
                return View(SON_DTO);
        }
        void GetSONumber()
        {
            SON_DTO.SON_CreatorCode = Convert.ToInt32(UserCode);
            SON_DTO.SON_Id = 1;
            DS = SON_DAO.SONumberDB(SON_DTO);

            ViewBag.Method = Help.GetCat(DS.Tables[0]);
            ViewBag.Frequency = Help.GetCat(DS.Tables[1]);
            ViewBag.Prefil = Help.GetCat(DS.Tables[2]);

            if (DS.Tables[3].Rows.Count > 0)
            {
                SON_DTO.SON_Number = Convert.ToInt64(DS.Tables[3].Rows[0]["SON_Number"]);
                SON_DTO.SON_Method = DS.Tables[3].Rows[0]["SON_Method"].ToString();
            }

            SON_DTO.SONumberReset = SON_DL.SORList(DS.Tables[4]);
            SON_DTO.SONumberPrefix = SON_DL.SOPList(DS.Tables[5]);
            SON_DTO.SONumberSuffix = SON_DL.SOSList(DS.Tables[6]);
        }
    }
}

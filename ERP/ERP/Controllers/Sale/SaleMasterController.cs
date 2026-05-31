using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DL;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;
using System.Transactions;

namespace ERP.Controllers.sale
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class SaleMasterController : Controller
    {
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        DataSet DS = new DataSet();
        SaleMaster_DL SM_DL = new SaleMaster_DL();


        Buyer_DAO B_DAO = new Buyer_DAO();
        Buyer_DTO B_DTO = new Buyer_DTO();
        List<Buyer_DTO> B_List = new List<Buyer_DTO>();


        BuyerSegment_DAO BS_DAO = new BuyerSegment_DAO();
        BuyerSegment_DTO BS_DTO = new BuyerSegment_DTO();
        List<BuyerSegment_DTO> BS_List = new List<BuyerSegment_DTO>();


        BuyerSubsegment_DAO BSS_DAO = new BuyerSubsegment_DAO();
        BuyerSubsegment_DTO BSS_DTO = new BuyerSubsegment_DTO();
        List<BuyerSubsegment_DTO> BSS_List = new List<BuyerSubsegment_DTO>();


        BuyerGroup_DAO BG_DAO = new BuyerGroup_DAO();
        BuyerGroup_DTO BG_DTO = new BuyerGroup_DTO();
        List<BuyerGroup_DTO> BG_List = new List<BuyerGroup_DTO>();


        BuyerCategory_DAO BC_DAO = new BuyerCategory_DAO();
        BuyerCategory_DTO BC_DTO = new BuyerCategory_DTO();
        List<BuyerCategory_DTO> BC_List = new List<BuyerCategory_DTO>();


        IncomeCode_DAO IC_DAO = new IncomeCode_DAO();
        IncomeCode_DTO IC_DTO = new IncomeCode_DTO();
        List<IncomeCode_DTO> IC_List = new List<IncomeCode_DTO>();


        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;




        [Route("sale/master/buyer")]
        public IActionResult Buyer(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<BuyerList_DTO> BList = BUYGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            var PageList = PaginatedList_DTO<BuyerList_DTO>.CreateAsync(BList, DPageNumber ?? 1, DPageSize);

            var Model = new BuyerHead_DTO()
            {
                Buyer_List = PageList,
            };
            return View(Model);
        }

        [Route("sale/master/buyer")]
        [HttpPost]
        public IActionResult Buyer(BuyerHead_DTO BH_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            var Original_BH_DTO = JsonClone(BH_DTO);

            bool IsValid = false;
            BuyerHead_DTO B_Head_DTO = new BuyerHead_DTO();
            List<BuyerGST_DTO>? GST_DTO = new List<BuyerGST_DTO>();
            List<BuyerWHT_DTO>? WHT_DTO = new List<BuyerWHT_DTO>();
            List<BuyerAdd_DTO>? Add_DTO = new List<BuyerAdd_DTO>();

            B_Head_DTO = BH_DTO;

            if (BH_DTO.BUY_GST_List != null)
                GST_DTO = BH_DTO.BUY_GST_List!.Where(K => K.BUY_GST_IsDeleted == 0).ToList();

            if (BH_DTO.BUY_WHT_List != null)
                WHT_DTO = BH_DTO.BUY_WHT_List!.Where(K => K.BUY_WHT_IsDeleted == 0).ToList();

            if (BH_DTO.BUY_Add_List != null)
                Add_DTO = BH_DTO.BUY_Add_List!.Where(K => K.BUY_ADD_IsDeleted == 0).ToList();

            B_DTO.BUY_CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                ModelState.Clear();

                B_Head_DTO.BUY_GST_List = GST_DTO;
                B_Head_DTO.BUY_WHT_List = WHT_DTO;
                B_Head_DTO.BUY_Add_List = Add_DTO;
                IsValid = TryValidateModel(B_Head_DTO);

                if (IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        B_DTO.BUY_Name = B_Head_DTO.BUY_Name;
                        B_DTO.BUY_Id = 1;
                        DS = B_DAO.BuyerDB(B_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            B_DTO.BUY_Name = BH_DTO.BUY_Name;
                            B_DTO.BUY_ContactPerson = BH_DTO.BUY_ContactPerson;
                            B_DTO.BUY_ContactTelephone = BH_DTO.BUY_ContactTelephone;
                            B_DTO.BUY_ContactMobile = BH_DTO.BUY_ContactMobile;
                            B_DTO.BUY_ContactEmail = BH_DTO.BUY_ContactEmail;
                            B_DTO.BUY_AccountPerson = BH_DTO.BUY_AccountPerson;
                            B_DTO.BUY_AccountTelephone = BH_DTO.BUY_AccountTelephone;
                            B_DTO.BUY_AccountMobile = BH_DTO.BUY_AccountMobile;
                            B_DTO.BUY_AccountEmail = BH_DTO.BUY_AccountEmail;
                            B_DTO.BUY_LOC_Number = Convert.ToInt64(BH_DTO.BUY_LOC_Number);
                            B_DTO.BUY_BYG_Number = Convert.ToInt64(BH_DTO.BUY_BYG_Number);
                            B_DTO.BUY_BYC_Number = Convert.ToInt64(BH_DTO.BUY_BYC_Number);
                            B_DTO.BUY_BYS_Number = Convert.ToInt64(BH_DTO.BUY_BYS_Number);
                            B_DTO.BUY_BSS_Number = Convert.ToInt64(BH_DTO.BUY_BSS_Number);
                            B_DTO.BUY_PaymentTerms = BH_DTO.BUY_PaymentTerms;
                            B_DTO.BUY_PaymentMode = BH_DTO.BUY_PaymentMode;
                            B_DTO.BUY_CreditDays = Convert.ToInt16(BH_DTO.BUY_CreditDays);
                            B_DTO.BUY_CreditLimit = Convert.ToDouble(BH_DTO.BUY_CreditLimit);
                            B_DTO.BUY_CUR_Number = Convert.ToInt64(BH_DTO.BUY_CUR_Number == null ? 1 : BH_DTO.BUY_CUR_Number);
                            B_DTO.BUY_AccountName = BH_DTO.BUY_AccountName;
                            B_DTO.BUY_AccountNumber = BH_DTO.BUY_AccountNumber;
                            B_DTO.BUY_IFSC = BH_DTO.BUY_IFSC;
                            B_DTO.BUY_BankName = BH_DTO.BUY_BankName;
                            B_DTO.BUY_DeliveryTerms = BH_DTO.BUY_DeliveryTerms;
                            B_DTO.BUY_DeliveryMode = BH_DTO.BUY_DeliveryMode;
                            B_DTO.BUY_RT_Number = Convert.ToInt64(BH_DTO.BUY_RT_Number == null ? 1 : BH_DTO.BUY_RT_Number);
                            B_DTO.BUY_GSTIN = BH_DTO.BUY_GSTIN;
                            B_DTO.BUY_AT_Number = Convert.ToInt64(BH_DTO.BUY_AT_Number);
                            B_DTO.BUY_PAN = BH_DTO.BUY_PAN;
                            B_DTO.BUY_YN_Number = Convert.ToInt64(BH_DTO.BUY_YN_Number == null ? 1 : BH_DTO.BUY_YN_Number);
                            B_DTO.BUY_AN_Number = Convert.ToInt64(BH_DTO.BUY_AN_Number == null ? 1 : BH_DTO.BUY_AN_Number);
                            B_DTO.BUY_Id = 2;
                            DS = B_DAO.BuyerDB(B_DTO);

                            if (DS.Tables[0].Rows.Count > 0)
                            {
                                B_DTO.BUY_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                foreach (var WHT in WHT_DTO)
                                {
                                    B_DTO.BUY_WHT_WHTC_Number = Convert.ToInt64(WHT.BUY_WHT_WHTC_Number);
                                    B_DTO.BUY_WHT_WHTT_Number = Convert.ToInt64(WHT.BUY_WHT_WHTT_Number);
                                    B_DTO.BUY_WHT_WHT_Number = Convert.ToInt64(WHT.BUY_WHT_WHT_Number);
                                    B_DTO.BUY_WHT_FromDate = Convert.ToString(Convert.ToDateTime(WHT.BUY_WHT_FromDate).ToString("yyyyMMdd"));
                                    B_DTO.BUY_WHT_ToDate = Convert.ToString(Convert.ToDateTime(WHT.BUY_WHT_ToDate).ToString("yyyyMMdd"));
                                    B_DTO.BUY_Id = 3;
                                    B_DAO.BuyerDB(B_DTO);
                                }
                                foreach (var GST in GST_DTO)
                                {
                                    B_DTO.BUY_GST_GSTC_Number = Convert.ToInt64(GST.BUY_GST_GSTC_Number);
                                    B_DTO.BUY_GST_GSTT_Number = Convert.ToInt64(GST.BUY_GST_GSTT_Number);
                                    B_DTO.BUY_GST_TCT_Number = Convert.ToInt64(GST.BUY_GST_TCT_Number);
                                    B_DTO.BUY_GST_FromDate = Convert.ToString(Convert.ToDateTime(GST.BUY_GST_FromDate).ToString("yyyyMMdd"));
                                    B_DTO.BUY_GST_ToDate = Convert.ToString(Convert.ToDateTime(GST.BUY_GST_ToDate).ToString("yyyyMMdd"));
                                    B_DTO.BUY_Id = 4;
                                    B_DAO.BuyerDB(B_DTO);
                                }
                                foreach (var ADD in Add_DTO)
                                {
                                    B_DTO.BUY_ADD_ADTP_Number = Convert.ToInt64(ADD.BUY_ADD_ADTP_Number);
                                    B_DTO.BUY_ADD_AddressID = Convert.ToString(ADD.BUY_ADD_AddressID);
                                    B_DTO.BUY_ADD_Address = Convert.ToString(ADD.BUY_ADD_Address);
                                    B_DTO.BUY_ADD_City = Convert.ToString(ADD.BUY_ADD_City);
                                    B_DTO.BUY_ADD_State = Convert.ToString(ADD.BUY_ADD_State);
                                    B_DTO.BUY_ADD_Country = Convert.ToString(ADD.BUY_ADD_Country);
                                    B_DTO.BUY_ADD_Pin = Convert.ToString(ADD.BUY_ADD_Pin);
                                    B_DTO.BUY_ADD_GSTIN = Convert.ToString(ADD.BUY_ADD_GSTIN);
                                    B_DTO.BUY_ADD_Primary = Convert.ToInt16(ADD.BUY_ADD_Primary);
                                    B_DTO.BUY_Id = 10;
                                    B_DAO.BuyerDB(B_DTO);
                                }
                                transaction.Complete();

                                BH_DTO.Reset();
                                B_Head_DTO.Reset();
                                GST_DTO = BH_DTO.BUY_GST_List!;
                                WHT_DTO = BH_DTO.BUY_WHT_List!;
                                Add_DTO = BH_DTO.BUY_Add_List!;
                                B_DTO.Reset();
                                Original_BH_DTO = JsonClone(BH_DTO);
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
                    B_DTO.BUY_DeleteNumbers = DeleteNumbers;
                    B_DTO.BUY_Id = 23;
                    B_DAO.BuyerDB(B_DTO);

                    BH_DTO.Reset();
                    B_Head_DTO.Reset();
                    GST_DTO = BH_DTO.BUY_GST_List!;
                    WHT_DTO = BH_DTO.BUY_WHT_List!;
                    B_DTO.Reset();
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
                    B_DTO.BUY_Number = Convert.ToInt64(Number);
                    B_DTO.BUY_Id = 22;
                    B_DAO.BuyerDB(B_DTO);

                    BH_DTO.Reset();
                    B_Head_DTO.Reset();
                    GST_DTO = BH_DTO.BUY_GST_List!;
                    WHT_DTO = BH_DTO.BUY_WHT_List!;
                    B_DTO.Reset();
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
                BH_DTO.Reset();
                B_Head_DTO.Reset();
                GST_DTO = BH_DTO.BUY_GST_List!;
                WHT_DTO = BH_DTO.BUY_WHT_List!;
                B_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                B_DTO.BUY_Number = Convert.ToInt64(BH_DTO.BUY_Number);
                B_DTO.BUY_Id = 11;
                DS = B_DAO.BuyerDB(B_DTO);
                B_Head_DTO = SM_DL.BuyerHeadList(DS.Tables[0]).FirstOrDefault();
                WHT_DTO = SM_DL.BuyerWHTList(DS.Tables[1]);
                GST_DTO = SM_DL.BuyerGSTList(DS.Tables[2]);
                Add_DTO = SM_DL.BuyerAddList(DS.Tables[3]);
            }
            else if (Mode == "Update")
            {
                ModelState.Clear();

                B_Head_DTO.BUY_GST_List = GST_DTO;
                B_Head_DTO.BUY_WHT_List = WHT_DTO;
                B_Head_DTO.BUY_Add_List = Add_DTO;

                IsValid = TryValidateModel(B_Head_DTO);
                if (IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            String GSTCheck = string.Join(", ", GST_DTO.Where(x => Convert.ToInt64(x.BUY_GST_Number) != 0).Select(x => x.BUY_GST_Number));
                            String WHTCheck = string.Join(", ", WHT_DTO.Where(x => Convert.ToInt64(x.BUY_WHT_Number) != 0).Select(x => x.BUY_WHT_Number));
                            String AddCheck = string.Join(", ", Add_DTO.Where(x => Convert.ToInt64(x.BUY_ADD_Number) != 0).Select(x => x.BUY_ADD_Number));

                            B_DTO.BUY_Number = BH_DTO.BUY_Number;
                            B_DTO.BUY_DeleteNumbers = GSTCheck;
                            B_DTO.BUY_Id = 12;
                            B_DAO.BuyerDB(B_DTO);

                            B_DTO.BUY_Number = BH_DTO.BUY_Number;
                            B_DTO.BUY_DeleteNumbers = WHTCheck;
                            B_DTO.BUY_Id = 13;
                            B_DAO.BuyerDB(B_DTO);

                            B_DTO.BUY_Number = BH_DTO.BUY_Number;
                            B_DTO.BUY_DeleteNumbers = AddCheck;
                            B_DTO.BUY_Id = 18;
                            B_DAO.BuyerDB(B_DTO);

                            B_DTO.BUY_Number = BH_DTO.BUY_Number;
                            B_DTO.BUY_Name = BH_DTO.BUY_Name;
                            B_DTO.BUY_ContactPerson = BH_DTO.BUY_ContactPerson;
                            B_DTO.BUY_ContactTelephone = BH_DTO.BUY_ContactTelephone;
                            B_DTO.BUY_ContactMobile = BH_DTO.BUY_ContactMobile;
                            B_DTO.BUY_ContactEmail = BH_DTO.BUY_ContactEmail;
                            B_DTO.BUY_AccountPerson = BH_DTO.BUY_AccountPerson;
                            B_DTO.BUY_AccountTelephone = BH_DTO.BUY_AccountTelephone;
                            B_DTO.BUY_AccountMobile = BH_DTO.BUY_AccountMobile;
                            B_DTO.BUY_AccountEmail = BH_DTO.BUY_AccountEmail;
                            B_DTO.BUY_LOC_Number = Convert.ToInt64(BH_DTO.BUY_LOC_Number);
                            B_DTO.BUY_BYG_Number = Convert.ToInt64(BH_DTO.BUY_BYG_Number);
                            B_DTO.BUY_BYC_Number = Convert.ToInt64(BH_DTO.BUY_BYC_Number);
                            B_DTO.BUY_BYS_Number = Convert.ToInt64(BH_DTO.BUY_BYS_Number);
                            B_DTO.BUY_BSS_Number = Convert.ToInt64(BH_DTO.BUY_BSS_Number);
                            B_DTO.BUY_PaymentTerms = BH_DTO.BUY_PaymentTerms;
                            B_DTO.BUY_PaymentMode = BH_DTO.BUY_PaymentMode;
                            B_DTO.BUY_CreditDays = Convert.ToInt16(BH_DTO.BUY_CreditDays);
                            B_DTO.BUY_CreditLimit = Convert.ToDouble(BH_DTO.BUY_CreditLimit);
                            B_DTO.BUY_CUR_Number = Convert.ToInt64(BH_DTO.BUY_CUR_Number == null ? 1 : BH_DTO.BUY_CUR_Number);
                            B_DTO.BUY_AccountName = BH_DTO.BUY_AccountName;
                            B_DTO.BUY_AccountNumber = BH_DTO.BUY_AccountNumber;
                            B_DTO.BUY_IFSC = BH_DTO.BUY_IFSC;
                            B_DTO.BUY_BankName = BH_DTO.BUY_BankName;
                            B_DTO.BUY_DeliveryTerms = BH_DTO.BUY_DeliveryTerms;
                            B_DTO.BUY_DeliveryMode = BH_DTO.BUY_DeliveryMode;
                            B_DTO.BUY_RT_Number = Convert.ToInt64(BH_DTO.BUY_RT_Number == null ? 1 : BH_DTO.BUY_RT_Number);
                            B_DTO.BUY_GSTIN = BH_DTO.BUY_GSTIN;
                            B_DTO.BUY_AT_Number = Convert.ToInt64(BH_DTO.BUY_AT_Number == null ? 1 : BH_DTO.BUY_AT_Number);
                            B_DTO.BUY_PAN = BH_DTO.BUY_PAN;
                            B_DTO.BUY_YN_Number = Convert.ToInt64(BH_DTO.BUY_YN_Number == null ? 1 : BH_DTO.BUY_YN_Number);
                            B_DTO.BUY_AN_Number = Convert.ToInt64(BH_DTO.BUY_AN_Number == null ? 1 : BH_DTO.BUY_AN_Number);
                            B_DTO.BUY_Id = 14;
                            DS = B_DAO.BuyerDB(B_DTO);

                            foreach (var WHTDetail in WHT_DTO)
                            {
                                B_DTO.BUY_WHT_WHTC_Number = Convert.ToInt64(WHTDetail.BUY_WHT_WHTC_Number);
                                B_DTO.BUY_WHT_WHTT_Number = Convert.ToInt64(WHTDetail.BUY_WHT_WHTT_Number);
                                B_DTO.BUY_WHT_WHT_Number = Convert.ToInt64(WHTDetail.BUY_WHT_WHT_Number);
                                B_DTO.BUY_WHT_FromDate = Convert.ToString(Convert.ToDateTime(WHTDetail.BUY_WHT_FromDate).ToString("yyyyMMdd"));
                                B_DTO.BUY_WHT_ToDate = Convert.ToString(Convert.ToDateTime(WHTDetail.BUY_WHT_ToDate).ToString("yyyyMMdd"));
                                if (WHTDetail.BUY_WHT_Number == 0)
                                {
                                    B_DTO.BUY_Id = 3;
                                }
                                else
                                {
                                    B_DTO.BUY_WHT_Number = WHTDetail.BUY_WHT_Number;
                                    B_DTO.BUY_Id = 15;
                                }
                                B_DAO.BuyerDB(B_DTO);
                            }
                            foreach (var GSTDetail in GST_DTO)
                            {
                                B_DTO.BUY_GST_GSTC_Number = Convert.ToInt64(GSTDetail.BUY_GST_GSTC_Number);
                                B_DTO.BUY_GST_GSTT_Number = Convert.ToInt64(GSTDetail.BUY_GST_GSTT_Number);
                                B_DTO.BUY_GST_TCT_Number = Convert.ToInt64(GSTDetail.BUY_GST_TCT_Number);
                                B_DTO.BUY_GST_FromDate = Convert.ToString(Convert.ToDateTime(GSTDetail.BUY_GST_FromDate).ToString("yyyyMMdd"));
                                B_DTO.BUY_GST_ToDate = Convert.ToString(Convert.ToDateTime(GSTDetail.BUY_GST_ToDate).ToString("yyyyMMdd"));
                                if (GSTDetail.BUY_GST_Number == 0)
                                {
                                    B_DTO.BUY_Id = 4;
                                }
                                else
                                {
                                    B_DTO.BUY_GST_Number = GSTDetail.BUY_GST_Number;
                                    B_DTO.BUY_Id = 16;
                                }
                                B_DAO.BuyerDB(B_DTO);
                            }
                            foreach (var ADD in Add_DTO)
                            {
                                B_DTO.BUY_ADD_ADTP_Number = Convert.ToInt64(ADD.BUY_ADD_ADTP_Number);
                                B_DTO.BUY_ADD_AddressID = Convert.ToString(ADD.BUY_ADD_AddressID);
                                B_DTO.BUY_ADD_Address = Convert.ToString(ADD.BUY_ADD_Address);
                                B_DTO.BUY_ADD_City = Convert.ToString(ADD.BUY_ADD_City);
                                B_DTO.BUY_ADD_State = Convert.ToString(ADD.BUY_ADD_State);
                                B_DTO.BUY_ADD_Country = Convert.ToString(ADD.BUY_ADD_Country);
                                B_DTO.BUY_ADD_Pin = Convert.ToString(ADD.BUY_ADD_Pin);
                                B_DTO.BUY_ADD_GSTIN = Convert.ToString(ADD.BUY_ADD_GSTIN);
                                B_DTO.BUY_ADD_Primary = Convert.ToInt16(ADD.BUY_ADD_Primary);
                                if (ADD.BUY_ADD_Number == 0)
                                {
                                    B_DTO.BUY_Id = 10;
                                }
                                else
                                {
                                    B_DTO.BUY_ADD_Number = ADD.BUY_ADD_Number;
                                    B_DTO.BUY_Id = 17;
                                }
                                B_DAO.BuyerDB(B_DTO);
                            }
                            transaction.Complete();

                            BH_DTO.Reset();
                            B_Head_DTO.Reset();
                            GST_DTO = BH_DTO.BUY_GST_List!;
                            WHT_DTO = BH_DTO.BUY_WHT_List!;
                            Add_DTO = BH_DTO.BUY_Add_List!;
                            B_DTO.Reset();
                            Original_BH_DTO = JsonClone(BH_DTO);
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

            List<BuyerList_DTO> BList = BUYGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            var PageList = PaginatedList_DTO<BuyerList_DTO>.CreateAsync(BList, DPageNumber ?? 1, DPageSize);

            if (Mode == "Save" || Mode == "Update")
            {
                var JsonBuyer = JsonSerializer.Serialize(Original_BH_DTO);
                var BuyHead_DTO = JsonSerializer.Deserialize<BuyerHead_DTO>(JsonBuyer);
                BuyHead_DTO.Buyer_List = PageList;
                return View(BuyHead_DTO);
            }
            else
            {
                B_Head_DTO.Buyer_List = PageList;
                B_Head_DTO.BUY_GST_List = GST_DTO;
                B_Head_DTO.BUY_WHT_List = WHT_DTO;
                B_Head_DTO.BUY_Add_List = Add_DTO;
                return View(B_Head_DTO);
            }
        }

        List<BuyerList_DTO> BUYGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            B_DTO.BUY_Id = 21;
            B_DTO.BUY_CreatorCode = Convert.ToInt32(UserCode);
            DS = B_DAO.BuyerDB(B_DTO);
            List<BuyerList_DTO> B_List = SM_DL.BuyerList(DS.Tables[0]);

            ViewBag.BuyerGroup = Help.GetCat(DS.Tables[1]);
            ViewBag.BuyerCategory = Help.GetCat(DS.Tables[2]);
            ViewBag.BuyerSegment = Help.GetCat(DS.Tables[3]);
            ViewBag.BuyerSubsegment = Help.GetCat(DS.Tables[4]);
            ViewBag.Registration = Help.GetCat(DS.Tables[5]);
            ViewBag.Currency = Help.GetCat(DS.Tables[6]);
            ViewBag.NatureOfAssessee = Help.GetCat(DS.Tables[7]);
            ViewBag.YesNo = Help.GetUnder(DS.Tables[8]);

            ViewBag.WHT_Category = Help.GetCat(DS.Tables[9]);
            ViewBag.WHT_Type = Help.GetCat(DS.Tables[10]);
            ViewBag.WHT_Tax = Help.GetCat(DS.Tables[11]);
            ViewBag.GST_Type = Help.GetCat(DS.Tables[12]);
            ViewBag.Location = Help.GetUnder(DS.Tables[13]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[14]);

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

            var Key = B_List.OrderByDescending(Cs => Cs.BUY_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.BUY_Name!.ToLower().Contains(Search.ToLower()) || K.BUY_Name!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.BUY_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.BUY_Name!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.BUY_Name!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.BUY_Number);
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

        [Route("sale/master/buyer/duplicate")]
        public Boolean BuyerDuplicate(String? Title, String? Number)
        {
            B_DTO.BUY_CreatorCode = Convert.ToInt32(UserCode);
            B_DTO.BUY_Name = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                B_DTO.BUY_Id = 5;
            }
            else
            {
                B_DTO.BUY_Number = Convert.ToInt32(Number);
                B_DTO.BUY_Id = 6;
            }
            DS = B_DAO.BuyerDB(B_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("sale/master/buyer/category")]
        public IActionResult BuyerCatetory(String? Category, String? Location)
        {
            if (Category == null)
            {
                Category = "";
            }
            B_DTO.BUY_CreatorCode = Convert.ToInt32(UserCode);
            B_DTO.BUY_Name = Category;
            B_DTO.BUY_LOC_Number = Convert.ToInt64(Location);
            B_DTO.BUY_Id = 8;
            DS = B_DAO.BuyerDB(B_DTO);

            var GstCategory = SM_DL.BuyerTaxCategory(DS.Tables[0]);
            return Json(GstCategory);
        }

        [Route("sale/master/buyer/cluster")]
        public IActionResult BuyerCluster(String? Tax, String? Category, String? Type)
        {
            if (Tax == null)
            {
                Tax = "";
            }

            B_DTO.BUY_CreatorCode = Convert.ToInt32(UserCode);
            B_DTO.BUY_Name = Convert.ToString(Tax);
            B_DTO.BUY_GST_GSTC_Number = Convert.ToInt32(Category);
            B_DTO.BUY_GST_GSTT_Number = Convert.ToInt32(Type);
            B_DTO.BUY_Id = 7;
            DS = B_DAO.BuyerDB(B_DTO);

            var Cluster = SM_DL.BuyerCluster(DS.Tables[0]);
            return Json(Cluster);
        }

        [Route("sale/master/buyer/assessee")]
        public IActionResult BuyerAssessee(String? Location)
        {
            B_DTO.BUY_CreatorCode = Convert.ToInt32(UserCode);
            B_DTO.BUY_LOC_Number = Convert.ToInt64(Location);
            B_DTO.BUY_Id = 9;
            DS = B_DAO.BuyerDB(B_DTO);

            var Assessee = SM_DL.BuyerTaxCategory(DS.Tables[0]);
            return Json(Assessee);
        }






        //buyer group
        [Route("sale/master/buyer-group")]
        public IActionResult BuyerGroup(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<BuyerGroup_DTO> BG_List = BGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BSS_DTO.BSS_Number);
            return View(PaginatedList<BuyerGroup_DTO>.CreateAsync(BG_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("sale/master/buyer-group")]
        [HttpPost]
        public IActionResult BuyerGroup(BuyerGroup_DTO BG_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                BG_DTO.BYG_CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        BG_DTO.BYG_Id = 6;
                        DS = BG_DAO.BuyerGroupDB(BG_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            BG_DTO.BYG_Id = 1;
                            BG_DAO.BuyerGroupDB(BG_DTO);

                            BG_DTO.Reset();
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
                        BG_DTO.BYG_DeleteNumbers = DeleteNumbers;
                        BG_DTO.BYG_Id = 3;
                        BG_DAO.BuyerGroupDB(BG_DTO);

                        BG_DTO.Reset();
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
                        BG_DTO.BYG_Number = Number;
                        BG_DTO.BYG_Id = 8;
                        BG_DAO.BuyerGroupDB(BG_DTO);

                        BG_DTO.Reset();
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
                    BG_DTO.Reset();
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    BG_DTO.BYG_Id = 4;
                    DS = BG_DAO.BuyerGroupDB(BG_DTO);
                    ViewBag.BuyerGroupEdit = SM_DL.GroupList(DS.Tables[0]).FirstOrDefault();
                }
                else if (Mode == "Update")
                {
                    BG_DTO.BYG_Id = 7;
                    DS = BG_DAO.BuyerGroupDB(BG_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            BG_DTO.BYG_Id = 5;
                            BG_DAO.BuyerGroupDB(BG_DTO);

                            BG_DTO.Reset();
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


                List<BuyerGroup_DTO> BG_List = BGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BG_DTO.BYG_Number);
                return View(PaginatedList<BuyerGroup_DTO>.CreateAsync(BG_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<BuyerGroup_DTO> BGGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                BG_DTO.BYG_Number = Convert.ToInt64(Number);
            }

            BG_DTO.BYG_Id = 2;
            BG_DTO.BYG_CreatorCode = Convert.ToInt32(UserCode);
            DS = BG_DAO.BuyerGroupDB(BG_DTO);
            BG_List = SM_DL.GroupList(DS.Tables[0]);
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

            var Key = BG_List.OrderByDescending(Cs => Cs.BYG_Group);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.BYG_Group!.ToLower().Contains(Search.ToLower()) || K.BYG_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.BYG_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.BYG_Group!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.BYG_Group!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.BYG_Number);
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

        [Route("sale/master/buyer-group/duplicate")]
        public Boolean BuyerGroupDuplicate(String? Title, String? Number)
        {
            BG_DTO.BYG_CreatorCode = Convert.ToInt32(UserCode);
            BG_DTO.BYG_Group = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                BG_DTO.BYG_Id = 6;
            }
            else
            {
                BG_DTO.BYG_Number = Convert.ToInt32(Number);
                BG_DTO.BYG_Id = 7;
            }
            DS = BG_DAO.BuyerGroupDB(BG_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //buyer segment
        [Route("sale/master/buyer-segment")]
        public IActionResult BuyerSegment(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<BuyerSegment_DTO> BS_LIST = BSGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BS_DTO.BYS_Number);
            return View(PaginatedList<BuyerSegment_DTO>.CreateAsync(BS_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("sale/master/buyer-segment")]
        [HttpPost]
        public IActionResult BuyerSegment(BuyerSegment_DTO BS_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                BS_DTO.BYS_CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        BS_DTO.BYS_Id = 6;
                        DS = BS_DAO.BuyerSegmentDB(BS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            BS_DTO.BYS_Id = 1;
                            BS_DAO.BuyerSegmentDB(BS_DTO);

                            BS_DTO.Reset();
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
                        BS_DTO.BYS_DeleteNumbers = DeleteNumbers;
                        BS_DTO.BYS_Id = 3;
                        BS_DAO.BuyerSegmentDB(BS_DTO);

                        BS_DTO.Reset();
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
                        BS_DTO.BYS_Number = Number;
                        BS_DTO.BYS_Id = 8;
                        BS_DAO.BuyerSegmentDB(BS_DTO);

                        BS_DTO.Reset();
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
                    BS_DTO.Reset();
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    BS_DTO.BYS_Id = 4;
                    DS = BS_DAO.BuyerSegmentDB(BS_DTO);
                    ViewBag.BuyerSegmentEdit = SM_DL.SegmentList(DS.Tables[0]).FirstOrDefault();
                }
                else if (Mode == "Update")
                {
                    BS_DTO.BYS_Id = 7;
                    DS = BS_DAO.BuyerSegmentDB(BS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            BS_DTO.BYS_Id = 5;
                            BS_DAO.BuyerSegmentDB(BS_DTO);

                            BS_DTO.Reset();
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


                List<BuyerSegment_DTO> BYS_List = BSGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BS_DTO.BYS_Number);
                return View(PaginatedList<BuyerSegment_DTO>.CreateAsync(BYS_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<BuyerSegment_DTO> BSGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                BS_DTO.BYS_Number = Convert.ToInt64(Number);
            }

            BS_DTO.BYS_Id = 2;
            BS_DTO.BYS_CreatorCode = Convert.ToInt32(UserCode);
            DS = BS_DAO.BuyerSegmentDB(BS_DTO);
            BS_List = SM_DL.SegmentList(DS.Tables[0]);
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

            var Key = BS_List.OrderByDescending(Cs => Cs.BYS_Segment);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.BYS_Segment!.ToLower().Contains(Search.ToLower()) || K.BYS_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.BYS_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.BYS_Segment!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.BYS_Segment!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.BYS_Number);
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

        [Route("sale/master/buyer-segment/duplicate")]
        public Boolean BuyerSegmentDuplicate(String? Title, String? Number)
        {
            BS_DTO.BYS_CreatorCode = Convert.ToInt32(UserCode);
            BS_DTO.BYS_Segment = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                BS_DTO.BYS_Id = 6;
            }
            else
            {
                BS_DTO.BYS_Number = Convert.ToInt32(Number);
                BS_DTO.BYS_Id = 7;
            }
            DS = BS_DAO.BuyerSegmentDB(BS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //buyer Subsegment
        [Route("sale/master/buyer-subsegment")]
        public IActionResult BuyerSubsegment(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<BuyerSubsegment_DTO> BSS_List = BSSGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BSS_DTO.BSS_Number);
            return View(PaginatedList<BuyerSubsegment_DTO>.CreateAsync(BSS_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("sale/master/buyer-subsegment")]
        [HttpPost]
        public IActionResult BuyerSubsegment(BuyerSubsegment_DTO BSS_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                BSS_DTO.BSS_CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        BSS_DTO.BSS_Id = 6;
                        DS = BSS_DAO.BuyerSubsegmentDB(BSS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            BSS_DTO.BSS_Id = 1;
                            BSS_DAO.BuyerSubsegmentDB(BSS_DTO);

                            BSS_DTO.Reset();
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
                        BSS_DTO.BSS_DeleteNumbers = DeleteNumbers;
                        BSS_DTO.BSS_Id = 3;
                        BSS_DAO.BuyerSubsegmentDB(BSS_DTO);

                        BSS_DTO.Reset();
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
                        BSS_DTO.BSS_Number = Number;
                        BSS_DTO.BSS_Id = 8;
                        BSS_DAO.BuyerSubsegmentDB(BSS_DTO);

                        BSS_DTO.Reset();
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
                    BSS_DTO.Reset();
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    BSS_DTO.BSS_Id = 4;
                    DS = BSS_DAO.BuyerSubsegmentDB(BSS_DTO);
                    ViewBag.BuyerSubsegmentEdit = SM_DL.SubsegmentList(DS.Tables[0]).FirstOrDefault();
                }
                else if (Mode == "Update")
                {
                    BSS_DTO.BSS_Id = 7;
                    DS = BSS_DAO.BuyerSubsegmentDB(BSS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            BSS_DTO.BSS_Id = 5;
                            BSS_DAO.BuyerSubsegmentDB(BSS_DTO);

                            BSS_DTO.Reset();
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

                List<BuyerSubsegment_DTO> BSS_List = BSSGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BSS_DTO.BSS_Number);
                return View(PaginatedList<BuyerSubsegment_DTO>.CreateAsync(BSS_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<BuyerSubsegment_DTO> BSSGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                BSS_DTO.BSS_Number = Convert.ToInt64(Number);
            }

            BSS_DTO.BSS_Id = 2;
            BSS_DTO.BSS_CreatorCode = Convert.ToInt32(UserCode);
            DS = BSS_DAO.BuyerSubsegmentDB(BSS_DTO);
            BSS_List = SM_DL.SubsegmentList(DS.Tables[0]);
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

            var Key = BSS_List.OrderByDescending(Cs => Cs.BSS_SubSegment);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.BSS_SubSegment!.ToLower().Contains(Search.ToLower()) || K.BSS_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.BSS_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.BSS_SubSegment!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.BSS_SubSegment!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.BSS_Number);
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

        [Route("sale/master/buyer-subsegment/duplicate")]
        public Boolean BuyerSubsegmentDuplicate(String? Title, String? Number)
        {
            BSS_DTO.BSS_CreatorCode = Convert.ToInt32(UserCode);
            BSS_DTO.BSS_SubSegment = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                BSS_DTO.BSS_Id = 6;
            }
            else
            {
                BSS_DTO.BSS_Number = Convert.ToInt32(Number);
                BSS_DTO.BSS_Id = 7;
            }
            DS = BSS_DAO.BuyerSubsegmentDB(BSS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }






        //Income Code
        [Route("sale/master/income-code")]
        public IActionResult IncomeCode(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<IncomeCode_DTO> IC_List = ICGetData(SortOrder, Search, PageNumber, PSize, PageFilter, IC_DTO.MIC_Number);
            return View(PaginatedList<IncomeCode_DTO>.CreateAsync(IC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("sale/master/income-code")]
        [HttpPost]
        public IActionResult IncomeCode(IncomeCode_DTO IC_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            IC_DTO.MIC_CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    IC_DTO.MIC_Id = 6;
                    DS = IC_DAO.IncomeCodeDB(IC_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        IC_DTO.MIC_Id = 1;
                        IC_DAO.IncomeCodeDB(IC_DTO);

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
                    IC_DTO.MIC_DeleteNumbers = DeleteNumbers;
                    IC_DTO.MIC_Id = 3;
                    IC_DAO.IncomeCodeDB(IC_DTO);

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
                    IC_DTO.MIC_Number = Number;
                    IC_DTO.MIC_Id = 8;
                    IC_DAO.IncomeCodeDB(IC_DTO);

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
                IC_DTO.MIC_Id = 4;
                DS = IC_DAO.IncomeCodeDB(IC_DTO);
                ViewBag.IncomeCodeEdit = SM_DL.IncomeList(DS.Tables[0]).FirstOrDefault();
            }
            else if (Mode == "Update")
            {
                IC_DTO.MIC_Id = 7;
                DS = IC_DAO.IncomeCodeDB(IC_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        IC_DTO.MIC_Id = 5;
                        IC_DAO.IncomeCodeDB(IC_DTO);

                        IC_DTO.Reset();
                        ModelState.Clear();
                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;

                        IC_DTO.MIC_Id = 4;
                        DS = IC_DAO.IncomeCodeDB(IC_DTO);
                        ViewBag.IncomeCodeEdit = SM_DL.IncomeList(DS.Tables[0]).FirstOrDefault();
                    }
                }
            }

            List<IncomeCode_DTO> IC_List = ICGetData(SortOrder, Search, PageNumber, PSize, PageFilter, IC_DTO.MIC_Number);
            return View(PaginatedList<IncomeCode_DTO>.CreateAsync(IC_List, DPageNumber ?? 1, DPageSize));
        }
        List<IncomeCode_DTO> ICGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                IC_DTO.MIC_Number = Convert.ToInt64(Number);
            }

            IC_DTO.MIC_Id = 2;
            IC_DTO.MIC_CreatorCode = Convert.ToInt32(UserCode);
            DS = IC_DAO.IncomeCodeDB(IC_DTO);
            IC_List = SM_DL.IncomeList(DS.Tables[0]);
            ViewBag.Occ = Help.GetCat(DS.Tables[1]);
            ViewBag.Cha = Help.GetCat(DS.Tables[2]);
            ViewBag.All = Help.GetCat(DS.Tables[3]);
            ViewBag.COA = Help.GetCat(DS.Tables[4]);
            ViewBag.SAC = Help.GetCat(DS.Tables[5]);

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

            var Key = IC_List.OrderByDescending(Cs => Cs.MIC_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.MIC_Code!.ToLower().Contains(Search.ToLower()) || K.MIC_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.MIC_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.MIC_Code!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.MIC_Code!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.MIC_Number);
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

        [Route("sale/master/income-code/duplicate")]
        public Boolean IncomeCodeDuplicate(String? Title, String? Number)
        {
            IC_DTO.MIC_CreatorCode = Convert.ToInt32(UserCode);
            IC_DTO.MIC_Code = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                IC_DTO.MIC_Id = 6;
            }
            else
            {
                IC_DTO.MIC_Number = Convert.ToInt32(Number);
                IC_DTO.MIC_Id = 7;
            }
            DS = IC_DAO.IncomeCodeDB(IC_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //buyer Category
        [Route("sale/master/buyer-category")]
        public IActionResult BuyerCategory(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<BuyerCategory_DTO> BYC_List = BuyerGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BC_DTO.BYC_Number);
            return View(PaginatedList<BuyerCategory_DTO>.CreateAsync(BYC_List, DPageNumber ?? 1, DPageSize));
        }


        [Route("sale/master/buyer-category")]
        [HttpPost]
        public IActionResult BuyerCategory(BuyerCategory_DTO BC_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            BC_DTO.BYC_CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    BC_DTO.BYC_Id = 6;
                    DS = BC_DAO.BuyerCategoryDB(BC_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        BC_DTO.BYC_Id = 1;
                        BC_DAO.BuyerCategoryDB(BC_DTO);

                        BC_DTO.Reset();
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
                    BC_DTO.BYC_DeleteNumbers = DeleteNumbers;
                    BC_DTO.BYC_Id = 3;
                    BC_DAO.BuyerCategoryDB(BC_DTO);

                    BC_DTO.Reset();
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
                    BC_DTO.BYC_Number = Number;
                    BC_DTO.BYC_Id = 8;
                    BC_DAO.BuyerCategoryDB(BC_DTO);

                    BC_DTO.Reset();
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
                BC_DTO.BYC_Id = 4;
                DS = BC_DAO.BuyerCategoryDB(BC_DTO);
                BC_DTO = SM_DL.BuyerCategoryList(DS.Tables[0]).FirstOrDefault();
                ViewBag.BuyerCategoryEdit = BC_DTO;
            }
            else if (Mode == "Update")
            {
                BC_DTO.BYC_Id = 7;
                DS = BC_DAO.BuyerCategoryDB(BC_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        BC_DTO.BYC_Id = 5;
                        BC_DAO.BuyerCategoryDB(BC_DTO);

                        BC_DTO.Reset();
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

            List<BuyerCategory_DTO> BYC_List = BuyerGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BC_DTO.BYC_Number);
            return View(PaginatedList<BuyerCategory_DTO>.CreateAsync(BYC_List, DPageNumber ?? 1, DPageSize));
        }
        List<BuyerCategory_DTO> BuyerGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;
            if (Number != null)
            {
                BC_DTO.BYC_Number = Convert.ToInt64(Number);
            }

            BC_DTO.BYC_Id = 2;
            BC_DTO.BYC_CreatorCode = Convert.ToInt32(UserCode);
            DS = BC_DAO.BuyerCategoryDB(BC_DTO);
            BC_List = SM_DL.BuyerCategoryList(DS.Tables[0]);
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

            var Key = BC_List.OrderByDescending(Cs => Cs.BYC_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.BYC_Category!.ToLower().Contains(Search.ToLower()) || K.BYC_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.BYC_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.BYC_Category!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.BYC_Category!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.BYC_Number);
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

        [Route("sale/master/buyer-category/duplicate")]
        public Boolean BuyerCategoryDuplicate(String? Title, String? Number)
        {
            BC_DTO.BYC_CreatorCode = Convert.ToInt32(UserCode);
            BC_DTO.BYC_Category = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                BC_DTO.BYC_Id = 6;
            }
            else
            {
                BC_DTO.BYC_Number = Convert.ToInt32(Number);
                BC_DTO.BYC_Id = 7;
            }
            DS = BC_DAO.BuyerCategoryDB(BC_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        private T JsonClone<T>(T obj)
        {
            if (obj == null) return default(T);
            var json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
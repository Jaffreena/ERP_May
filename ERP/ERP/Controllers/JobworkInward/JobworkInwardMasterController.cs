using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DL;
using ERP_DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using SelectPdf;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Transactions;
namespace ERP.Controllers.JobworkInward
{
    public class JobworkInwardMasterController : Controller
    {
        Int32? DPageNumber;
        Int32 DPageSize;
        Validation Valid = new Validation();
        DataSet DS = new DataSet();
        JobworkInwardMaster_DL SM_DL = new JobworkInwardMaster_DL();
        Help Help = new Help();

        Customer_DAO B_DAO = new Customer_DAO();
        Customer_DTO B_DTO = new Customer_DTO();
        List<Customer_DTO> B_List = new List<Customer_DTO>();


        JW_CustomerGroup_DAO BG_DAO = new JW_CustomerGroup_DAO();
        CustomerGroup_DTO BG_DTO = new CustomerGroup_DTO();
        List<CustomerGroup_DTO> BG_List = new List<CustomerGroup_DTO>();


        CustomerCategory_DAO BC_DAO = new CustomerCategory_DAO();
        CustomerCategory_DTO BC_DTO = new CustomerCategory_DTO();
        List<CustomerCategory_DTO> BC_List = new List<CustomerCategory_DTO>();

        Shift_DTO PS_DTO = new Shift_DTO();
        Shift_DAO PS_DAO = new Shift_DAO();
        List<Shift_DTO> PS_List = new List<Shift_DTO>();
        Shift_DL PS_DL = new Shift_DL();

        JW_Process_DAO W_DAO = new JW_Process_DAO();
        JW_Process_DTO W_DTO = new JW_Process_DTO();
        List<JW_Process_DTO> W_List = new List<JW_Process_DTO>();

        WorkCentreGroup_DAO WG_DAO = new WorkCentreGroup_DAO();
        WorkCentreGroup_DTO WG_DTO = new WorkCentreGroup_DTO();
        List<WorkCentreGroup_DTO> WG_List = new List<WorkCentreGroup_DTO>();

        WorkCentre_DAO WC_DAO = new WorkCentre_DAO();
        WorkCentre_DTO WC_DTO = new WorkCentre_DTO();
        List<WorkCentre_DTO> WC_List = new List<WorkCentre_DTO>();

        CustomerContact_DTO C_DTO = new CustomerContact_DTO();

        #region Customer
        [Route("jobwork/master/customer/assessee")]
        public IActionResult CustomerAssessee(string? Location)
        {
            B_DTO.CUS_CreatorCode = Convert.ToInt32(UserCode);
            B_DTO.CUS_LOC_Number = string.IsNullOrEmpty(Location) ? 0 : Convert.ToInt64(Location);
            B_DTO.CUS_Id = 28;

            DS = B_DAO.CustomerDB(B_DTO);

            var Assessee = SM_DL.CustomerAssess(DS.Tables[0]);

            #region JSON LOG
            try
            {
                string folderPath = @"C:\Temp";
                Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, "CustomerJsonLog.txt");

                // 🔥 Convert object to JSON string
                string jsonString = System.Text.Json.JsonSerializer.Serialize(
                    Assessee,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true // pretty format
                    });

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("===== CUSTOMER ASSESSEE JSON START =====");
                sb.AppendLine("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine(jsonString);
                sb.AppendLine("===== CUSTOMER ASSESSEE JSON END =====");
                sb.AppendLine("");

                System.IO.File.AppendAllText(filePath, sb.ToString());

                System.Diagnostics.Debug.WriteLine("JSON FILE WRITTEN SUCCESS");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JSON FILE ERROR: " + ex.ToString());
            }
            #endregion

            return Json(Assessee);
        }

        [Route("jobwork/master/customer/whtgrid")]
        public IActionResult CustomerGridWHT(string? categoryId,string? typeId)
        {
            B_DTO.CUS_CreatorCode = Convert.ToInt32(UserCode);
           
            B_DTO.CUS_Id = 26;
            B_DTO.WH_TaxCategory= string.IsNullOrEmpty(categoryId) ? "" : categoryId;
            B_DTO.WH_TaxType = string.IsNullOrEmpty(typeId) ? "" : typeId;
            DS = B_DAO.CustomerDB(B_DTO);

            var Assessee = SM_DL.CustomerWHTGridTaxCode(DS.Tables[0]);

      

            return Json(Assessee);
        }

        [Route("jobwork/master/customer/gstgrid")]
        public IActionResult CustomerGridGST(string? categoryId, string? typeId)
        {
            B_DTO.CUS_CreatorCode = Convert.ToInt32(UserCode);

            B_DTO.CUS_Id = 27;
            B_DTO.GST_Category = string.IsNullOrEmpty(categoryId) ? "" : categoryId;
            B_DTO.GST_Type = string.IsNullOrEmpty(typeId) ? "" : typeId;
            DS = B_DAO.CustomerDB(B_DTO);

            var Assessee = SM_DL.CustomerGSTGridTaxCluster(DS.Tables[0]);



            return Json(Assessee);
        }
        [Route("jobwork/master/customer/category")]
        public IActionResult CustomerCategory1(string? Category, string? Location)
        {
            try
            {
                if (Category == null)
                {
                    Category = "";
                }

                B_DTO.CUS_CreatorCode = Convert.ToInt32(UserCode);
                B_DTO.CUS_Name = Category;
                B_DTO.CUS_LOC_Number = Convert.ToInt64(Location);
                B_DTO.CUS_Id = 8; // same logic like Buyer (fetch category based on location)

                DS = B_DAO.CustomerDB(B_DTO);

                var GstCategory = SM_DL.CustomerTaxCategory(DS.Tables[0]);

                return Json(GstCategory);
            }
            catch (Exception ex)
            {
                // Optional debug (you already logging DB params)
                System.Diagnostics.Debug.WriteLine("CustomerCategory Error: " + ex.ToString());

                return Json(new { success = false, message = ex.Message });
            }
        }

        [Route("jobwork/master/customer/cluster")]
        public IActionResult CustomerCluster(string? Tax, string? Category, string? Type)
        {
            try
            {
                if (Tax == null)
                {
                    Tax = "";
                }

                B_DTO.CUS_CreatorCode = Convert.ToInt32(UserCode);
                B_DTO.CUS_Name = Tax;
                B_DTO.CUS_GST_GSTC_Number = string.IsNullOrEmpty(Category) ? 0 : Convert.ToInt32(Category);
                B_DTO.CUS_GST_GSTT_Number = string.IsNullOrEmpty(Type) ? 0 : Convert.ToInt32(Type);
                B_DTO.CUS_Id = 7;

                DS = B_DAO.CustomerDB(B_DTO);

                var Cluster = SM_DL.CustomerCluster(DS.Tables[0]);

                return Json(Cluster);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CustomerCluster Error: " + ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Route("jobwork/master/customer")]
        public IActionResult Customer(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<CustomerList_DTO> BList = CUSTGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            var PageList = PaginatedList_DTO<CustomerList_DTO>.CreateAsync(BList, DPageNumber ?? 1, DPageSize);

            var Model = new CustomerHead_DTO()
            {
                Customer_List = PageList,
            };
            return View(Model);
        }

        [Route("jobwork/master/customer")]
        [HttpPost]
        public IActionResult Customer(CustomerHead_DTO BH_DTO, Int64? Number, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            var Original_BH_DTO = JsonClone(BH_DTO);

            bool IsValid = false;
            CustomerHead_DTO B_Head_DTO = new CustomerHead_DTO();
            List<CustomerGST_DTO>? GST_DTO = new List<CustomerGST_DTO>();
            List<CustomerWHT_DTO>? WHT_DTO = new List<CustomerWHT_DTO>();
            List<CustomerAdd_DTO>? Add_DTO = new List<CustomerAdd_DTO>();
            List<CustomerContact_DTO>? Contact_DTO = new List<CustomerContact_DTO>();

            B_Head_DTO = BH_DTO;

            if (BH_DTO.CUS_GST_List != null)
                GST_DTO = BH_DTO.CUS_GST_List!.Where(K => K.CUS_GST_IsDeleted == 0).ToList();

            if (BH_DTO.CUS_WHT_List != null)
                WHT_DTO = BH_DTO.CUS_WHT_List!.Where(K => K.CUS_WHT_IsDeleted == 0).ToList();

            if (BH_DTO.CUS_Add_List != null)
                Add_DTO = BH_DTO.CUS_Add_List!.Where(K => K.CUS_ADD_IsDeleted == 0).ToList();

            if (BH_DTO.CUS_Contact_List != null)
                Contact_DTO = BH_DTO.CUS_Contact_List!.Where(K => K.CUS_CNT_IsDeleted == 0).ToList();

            B_DTO.CUS_CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                ModelState.Clear();

                B_Head_DTO.CUS_GST_List = GST_DTO;
                B_Head_DTO.CUS_WHT_List = WHT_DTO;
                B_Head_DTO.CUS_Add_List = Add_DTO;
                IsValid = TryValidateModel(B_Head_DTO);

                if (IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        B_DTO.CUS_Name = B_Head_DTO.CUS_Name;
                        B_DTO.CUS_Id = 1;
                        DS = B_DAO.CustomerDB(B_DTO);
                       // if (DS == null && DS.Tables.Count > 0 && DS.Tables[0].Rows.Count == 0)
                            if (DS.Tables[0].Rows.Count == 0)
                            {
                            B_DTO.CUS_Name = BH_DTO.CUS_Name;
                            B_DTO.CUS_ContactPerson = BH_DTO.CUS_ContactPerson;
                            B_DTO.CUS_ContactTelephone = BH_DTO.CUS_ContactTelephone;
                            B_DTO.CUS_ContactMobile = BH_DTO.CUS_ContactMobile;
                            B_DTO.CUS_ContactEmail = BH_DTO.CUS_ContactEmail;
                            B_DTO.CUS_AccountPerson = BH_DTO.CUS_AccountPerson;
                            B_DTO.CUS_AccountTelephone = BH_DTO.CUS_AccountTelephone;
                            B_DTO.CUS_AccountMobile = BH_DTO.CUS_AccountMobile;
                            B_DTO.CUS_AccountEmail = BH_DTO.CUS_AccountEmail;
                            B_DTO.CUS_LOC_Number = Convert.ToInt64(BH_DTO.CUS_LOC_Number);
                            B_DTO.CUS_BYG_Number = Convert.ToInt64(BH_DTO.CUS_BYG_Number);
                            B_DTO.CUS_BYC_Number = Convert.ToInt64(BH_DTO.CUS_BYC_Number);
                           B_DTO.JWC_WH_Number = Convert.ToInt64(BH_DTO.CUS_WareHouse_Number);
                            B_DTO.CUS_BSS_Number = Convert.ToInt64(BH_DTO.CUS_BSS_Number);
                            B_DTO.CUS_PaymentTerms = BH_DTO.CUS_PaymentTerms;
                            B_DTO.CUS_PaymentMode = BH_DTO.CUS_PaymentMode;
                            B_DTO.CUS_CreditDays = Convert.ToInt16(BH_DTO.CUS_CreditDays);
                            B_DTO.CUS_CreditLimit = Convert.ToDouble(BH_DTO.CUS_CreditLimit);
                            B_DTO.CUS_CUR_Number = Convert.ToInt64(BH_DTO.CUS_CUR_Number == null ? 1 : BH_DTO.CUS_CUR_Number);
                            B_DTO.CUS_AccountName = BH_DTO.CUS_AccountName;
                            B_DTO.CUS_AccountNumber = BH_DTO.CUS_AccountNumber;
                            B_DTO.CUS_IFSC = BH_DTO.CUS_IFSC;
                            B_DTO.CUS_BankName = BH_DTO.CUS_BankName;
                            B_DTO.CUS_DeliveryTerms = BH_DTO.CUS_DeliveryTerms;
                            B_DTO.CUS_DeliveryMode = BH_DTO.CUS_DeliveryMode;
                            B_DTO.CUS_RT_Number = Convert.ToInt64(BH_DTO.CUS_RT_Number == null ? 1 : BH_DTO.CUS_RT_Number);
                            B_DTO.CUS_GSTIN = BH_DTO.CUS_GSTIN;
                            B_DTO.CUS_AT_Number = Convert.ToInt64(BH_DTO.CUS_AT_Number);
                            B_DTO.CUS_PAN = BH_DTO.CUS_PAN;
                            B_DTO.CUS_YN_Number = Convert.ToInt64(BH_DTO.CUS_YN_Number == null ? 1 : BH_DTO.CUS_YN_Number);
                            B_DTO.CUS_AN_Number = Convert.ToInt64(BH_DTO.CUS_AN_Number == null ? 1 : BH_DTO.CUS_AN_Number);
                            B_DTO.CUS_Id = 2;
                            DS = B_DAO.CustomerDB(B_DTO);

                            if (DS.Tables[0].Rows.Count > 0)
                            {
                                B_DTO.CUS_Number = Convert.ToInt64(DS.Tables[0].Rows[0][0]);

                                foreach (var WHT in WHT_DTO)
                                {
                                    B_DTO.CUS_WHT_WHTC_Number = Convert.ToInt64(WHT.CUS_WHT_WHTC_Number);
                                    B_DTO.CUS_WHT_WHTT_Number = Convert.ToInt64(WHT.CUS_WHT_WHTT_Number);
                                    B_DTO.CUS_WHT_WHT_Number = Convert.ToInt64(WHT.CUS_WHT_WHT_Number);
                                    B_DTO.CUS_WHT_FromDate = Convert.ToString(Convert.ToDateTime(WHT.CUS_WHT_FromDate).ToString("yyyyMMdd"));
                                    B_DTO.CUS_WHT_ToDate = Convert.ToString(Convert.ToDateTime(WHT.CUS_WHT_ToDate).ToString("yyyyMMdd"));
                                    B_DTO.CUS_Id = 3;
                                    B_DAO.CustomerDB(B_DTO);
                                }
                                foreach (var GST in GST_DTO)
                                {
                                    B_DTO.CUS_GST_GSTC_Number = Convert.ToInt64(GST.CUS_GST_GSTC_Number);
                                    B_DTO.CUS_GST_GSTT_Number = Convert.ToInt64(GST.CUS_GST_GSTT_Number);
                                    B_DTO.CUS_GST_TCT_Number = Convert.ToInt64(GST.CUS_GST_TCT_Number);
                                    B_DTO.CUS_GST_FromDate = Convert.ToString(Convert.ToDateTime(GST.CUS_GST_FromDate).ToString("yyyyMMdd"));
                                    B_DTO.CUS_GST_ToDate = Convert.ToString(Convert.ToDateTime(GST.CUS_GST_ToDate).ToString("yyyyMMdd"));
                                    B_DTO.CUS_Id = 4;
                                    B_DAO.CustomerDB(B_DTO);
                                }
                                foreach (var ADD in Add_DTO)
                                {
                                    B_DTO.CUS_ADD_ADTP_Number = Convert.ToInt64(ADD.CUS_ADD_ADTP_Number);
                                    B_DTO.CUS_ADD_AddressID = Convert.ToString(ADD.CUS_ADD_AddressID);
                                    B_DTO.CUS_ADD_Address = Convert.ToString(ADD.CUS_ADD_Address);
                                    B_DTO.CUS_ADD_City = Convert.ToString(ADD.CUS_ADD_City);
                                    B_DTO.CUS_ADD_State = Convert.ToString(ADD.CUS_ADD_State);
                                    B_DTO.CUS_ADD_Country = Convert.ToString(ADD.CUS_ADD_Country);
                                    B_DTO.CUS_ADD_Pin = Convert.ToString(ADD.CUS_ADD_Pin);
                                    B_DTO.CUS_ADD_GSTIN = Convert.ToString(ADD.CUS_ADD_GSTIN);
                                    B_DTO.CUS_ADD_Primary = Convert.ToInt16(ADD.CUS_ADD_Primary);
                                    B_DTO.CUS_Id = 10;
                                    B_DAO.CustomerDB(B_DTO);
                                }
                                transaction.Complete();

                                BH_DTO.Reset();
                                B_Head_DTO.Reset();
                                GST_DTO = BH_DTO.CUS_GST_List!;
                                WHT_DTO = BH_DTO.CUS_WHT_List!;
                                Add_DTO = BH_DTO.CUS_Add_List!;
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
                            ViewBag.ErrorMessage = "(Customer Name Already Exists )Already assigned. Please check";
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
                    B_DTO.CUS_DeleteNumbers = DeleteNumbers;
                    B_DTO.CUS_Id = 23;
                    B_DAO.CustomerDB(B_DTO);

                    BH_DTO.Reset();
                    B_Head_DTO.Reset();
                    GST_DTO = BH_DTO.CUS_GST_List!;
                    WHT_DTO = BH_DTO.CUS_WHT_List!;
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
                    B_DTO.CUS_Number = Convert.ToInt64(Number);
                    B_DTO.CUS_Id = 22;
                    B_DAO.CustomerDB(B_DTO);

                    BH_DTO.Reset();
                    B_Head_DTO.Reset();
                    GST_DTO = BH_DTO.CUS_GST_List!;
                    WHT_DTO = BH_DTO.CUS_WHT_List!;
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
                GST_DTO = BH_DTO.CUS_GST_List!;
                WHT_DTO = BH_DTO.CUS_WHT_List!;
                B_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                B_DTO.CUS_Number = Convert.ToInt64(BH_DTO.CUS_Number);
                B_DTO.CUS_Id = 11;
                DS = B_DAO.CustomerDB(B_DTO);
                B_Head_DTO = SM_DL.CustomerHeadList(DS.Tables[0]).FirstOrDefault();
                Add_DTO = SM_DL.CustomerAddList(DS.Tables[1]);
                Contact_DTO= SM_DL.CustomerContactList(DS.Tables[2]);
                 WHT_DTO = SM_DL.CustomerWHTList(DS.Tables[3]);
                GST_DTO = SM_DL.CustomerGSTList(DS.Tables[4]);
                //Add_DTO = SM_DL.CustomerAddList(DS.Tables[3]);
            }
            else if (Mode == "Update")
            {
                ModelState.Clear();

                B_Head_DTO.CUS_GST_List = GST_DTO;
                B_Head_DTO.CUS_WHT_List = WHT_DTO;
                B_Head_DTO.CUS_Add_List = Add_DTO;
                B_Head_DTO.CUS_Contact_List = Contact_DTO;

                IsValid = TryValidateModel(B_Head_DTO);
                if (IsValid)
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            String GSTCheck = string.Join(", ", GST_DTO.Where(x => Convert.ToInt64(x.CUS_GST_Number) != 0).Select(x => x.CUS_GST_Number));
                            String WHTCheck = string.Join(", ", WHT_DTO.Where(x => Convert.ToInt64(x.CUS_WHT_Number) != 0).Select(x => x.CUS_WHT_Number));
                            String AddCheck = string.Join(", ", Add_DTO.Where(x => Convert.ToInt64(x.CUS_ADD_Number) != 0).Select(x => x.CUS_ADD_Number));

                            B_DTO.CUS_Number = BH_DTO.CUS_Number;
                            B_DTO.CUS_DeleteNumbers = GSTCheck;
                            B_DTO.CUS_Id = 12;
                            B_DAO.CustomerDB(B_DTO);

                            B_DTO.CUS_Number = BH_DTO.CUS_Number;
                            B_DTO.CUS_DeleteNumbers = WHTCheck;
                            B_DTO.CUS_Id = 13;
                            B_DAO.CustomerDB(B_DTO);

                            B_DTO.CUS_Number = BH_DTO.CUS_Number;
                            B_DTO.CUS_DeleteNumbers = AddCheck;
                            B_DTO.CUS_Id = 18;
                            B_DAO.CustomerDB(B_DTO);

                            B_DTO.CUS_Number = BH_DTO.CUS_Number;
                            B_DTO.CUS_Name = BH_DTO.CUS_Name;
                            B_DTO.CUS_ContactPerson = BH_DTO.CUS_ContactPerson;
                            B_DTO.CUS_ContactTelephone = BH_DTO.CUS_ContactTelephone;
                            B_DTO.CUS_ContactMobile = BH_DTO.CUS_ContactMobile;
                            B_DTO.CUS_ContactEmail = BH_DTO.CUS_ContactEmail;
                            B_DTO.CUS_AccountPerson = BH_DTO.CUS_AccountPerson;
                            B_DTO.CUS_AccountTelephone = BH_DTO.CUS_AccountTelephone;
                            B_DTO.CUS_AccountMobile = BH_DTO.CUS_AccountMobile;
                            B_DTO.CUS_AccountEmail = BH_DTO.CUS_AccountEmail;
                            B_DTO.CUS_LOC_Number = Convert.ToInt64(BH_DTO.CUS_LOC_Number);
                            B_DTO.CUS_BYG_Number = Convert.ToInt64(BH_DTO.CUS_BYG_Number);
                            B_DTO.CUS_BYC_Number = Convert.ToInt64(BH_DTO.CUS_BYC_Number);
                           B_DTO.JWC_WH_Number = Convert.ToInt64(BH_DTO.CUS_WareHouse_Number);
                            B_DTO.CUS_BSS_Number = Convert.ToInt64(BH_DTO.CUS_BSS_Number);
                            B_DTO.CUS_PaymentTerms = BH_DTO.CUS_PaymentTerms;
                            B_DTO.CUS_PaymentMode = BH_DTO.CUS_PaymentMode;
                            B_DTO.CUS_CreditDays = Convert.ToInt16(BH_DTO.CUS_CreditDays);
                            B_DTO.CUS_CreditLimit = Convert.ToDouble(BH_DTO.CUS_CreditLimit);
                            B_DTO.CUS_CUR_Number = Convert.ToInt64(BH_DTO.CUS_CUR_Number == null ? 0 : BH_DTO.CUS_CUR_Number);
                            B_DTO.CUS_AccountName = BH_DTO.CUS_AccountName;
                            B_DTO.CUS_AccountNumber = BH_DTO.CUS_AccountNumber;
                            B_DTO.CUS_IFSC = BH_DTO.CUS_IFSC;
                            B_DTO.CUS_BankName = BH_DTO.CUS_BankName;
                            B_DTO.CUS_DeliveryTerms = BH_DTO.CUS_DeliveryTerms;
                            B_DTO.CUS_DeliveryMode = BH_DTO.CUS_DeliveryMode;
                            B_DTO.CUS_RT_Number = Convert.ToInt64(BH_DTO.CUS_RT_Number == null ? 0 : BH_DTO.CUS_RT_Number);
                            B_DTO.CUS_GSTIN = BH_DTO.CUS_GSTIN;
                            B_DTO.CUS_AT_Number = Convert.ToInt64(BH_DTO.CUS_AT_Number == null ? 0 : BH_DTO.CUS_AT_Number);
                            B_DTO.CUS_PAN = BH_DTO.CUS_PAN;
                            B_DTO.CUS_YN_Number = Convert.ToInt64(BH_DTO.CUS_YN_Number == null ? 0 : BH_DTO.CUS_YN_Number);
                            B_DTO.CUS_AN_Number = Convert.ToInt64(BH_DTO.CUS_AN_Number == null ? 0 : BH_DTO.CUS_AN_Number);
                            B_DTO.CUS_Id = 14;
                            DS = B_DAO.CustomerDB(B_DTO);
                            if (WHT_DTO != null && WHT_DTO.Count > 0)
                            {
                                foreach (var WHTDetail in WHT_DTO)
                                {
                                    // 🔄 reset
                                    B_DTO.CUS_WHT_Number = 0;

                                    B_DTO.CUS_WHT_WHTC_Number = string.IsNullOrEmpty(Convert.ToString(WHTDetail.CUS_WHT_WHTC_Number))
                                        ? 0 : Convert.ToInt64(WHTDetail.CUS_WHT_WHTC_Number);

                                    B_DTO.CUS_WHT_WHTT_Number = string.IsNullOrEmpty(Convert.ToString(WHTDetail.CUS_WHT_WHTT_Number))
                                        ? 0 : Convert.ToInt64(WHTDetail.CUS_WHT_WHTT_Number);

                                    B_DTO.CUS_WHT_WHT_Number = string.IsNullOrEmpty(Convert.ToString(WHTDetail.CUS_WHT_WHT_Number))
                                        ? 0 : Convert.ToInt64(WHTDetail.CUS_WHT_WHT_Number);
                                    // ✅ Convert STRING → DateTime (IMPORTANT)
                                    B_DTO.CUS_WHT_FromDate = string.IsNullOrEmpty(WHTDetail.CUS_WHT_FromDate)
                                        ? null
                                        : Convert.ToDateTime(WHTDetail.CUS_WHT_FromDate).ToString("yyyy-MM-dd");

                                    B_DTO.CUS_WHT_ToDate = string.IsNullOrEmpty(WHTDetail.CUS_WHT_ToDate)
                                        ? null
                                        : Convert.ToDateTime(WHTDetail.CUS_WHT_ToDate).ToString("yyyy-MM-dd");



                                    if (WHTDetail.CUS_WHT_Number == 0)
                                    {
                                        B_DTO.CUS_Id = 3;   // INSERT
                                    }
                                    else
                                    {
                                        B_DTO.CUS_WHT_Number = WHTDetail.CUS_WHT_Number;
                                        B_DTO.CUS_Id = 15;  // UPDATE
                                    }
                                    string json = JsonSerializer.Serialize(
    B_DTO,
    new JsonSerializerOptions { WriteIndented = true }
);

                                    System.Diagnostics.Debug.WriteLine(json);
                                    B_DAO.CustomerDB(B_DTO);
                                }
                            }
                            if (GST_DTO != null && GST_DTO.Count > 0)
                            {
                                foreach (var GSTDetail in GST_DTO)
                                {
                                    B_DTO.CUS_GST_GSTC_Number = Convert.ToInt64(GSTDetail.CUS_GST_GSTC_Number);
                                    B_DTO.CUS_GST_GSTT_Number = Convert.ToInt64(GSTDetail.CUS_GST_GSTT_Number);
                                    B_DTO.CUS_GST_TCT_Number = Convert.ToInt64(GSTDetail.CUS_GST_TCT_Number);
                                    B_DTO.CUS_GST_FromDate = Convert.ToString(Convert.ToDateTime(GSTDetail.CUS_GST_FromDate).ToString("yyyyMMdd"));
                                    B_DTO.CUS_GST_ToDate = Convert.ToString(Convert.ToDateTime(GSTDetail.CUS_GST_ToDate).ToString("yyyyMMdd"));
                                    if (GSTDetail.CUS_GST_Number == 0)
                                    {
                                        B_DTO.CUS_Id = 4;
                                    }
                                    else
                                    {
                                        B_DTO.CUS_GST_Number = GSTDetail.CUS_GST_Number;
                                        B_DTO.CUS_Id = 16;
                                    }
                                    B_DAO.CustomerDB(B_DTO);
                                }
                            }
                            if (Add_DTO != null && Add_DTO.Count > 0)
                            {
                                foreach (var ADD in Add_DTO)
                                {
                                    B_DTO.CUS_ADD_ADTP_Number = Convert.ToInt64(ADD.CUS_ADD_ADTP_Number);
                                    B_DTO.CUS_ADD_AddressID = Convert.ToString(ADD.CUS_ADD_AddressID);
                                    B_DTO.CUS_ADD_Address = Convert.ToString(ADD.CUS_ADD_Address);
                                    B_DTO.CUS_ADD_City = Convert.ToString(ADD.CUS_ADD_City);
                                    B_DTO.CUS_ADD_State = Convert.ToString(ADD.CUS_ADD_State);
                                    B_DTO.CUS_ADD_Country = Convert.ToString(ADD.CUS_ADD_Country);
                                    B_DTO.CUS_ADD_Pin = Convert.ToString(ADD.CUS_ADD_Pin);
                                    B_DTO.CUS_ADD_GSTIN = Convert.ToString(ADD.CUS_ADD_GSTIN);
                                    B_DTO.CUS_ADD_Primary = Convert.ToInt16(ADD.CUS_ADD_Primary);
                                    if (ADD.CUS_ADD_Number == 0)
                                    {
                                        B_DTO.CUS_Id = 10;
                                    }
                                    else
                                    {
                                        B_DTO.CUS_ADD_Number = ADD.CUS_ADD_Number;
                                        B_DTO.CUS_Id = 17;
                                    }
                                    B_DAO.CustomerDB(B_DTO);
                                }
                            }
                            if (Contact_DTO != null && Contact_DTO.Count > 0)
                            {
                                foreach (var CNT in Contact_DTO)
                                {
                                    B_DTO.CUS_CNT_ContactName = Convert.ToString(CNT.CUS_CNT_ContactName);
                                    B_DTO.CUS_CNT_Department = Convert.ToString(CNT.CUS_CNT_Department);
                                    B_DTO.CUS_CNT_Mobile = Convert.ToString(CNT.CUS_CNT_Mobile);
                                    B_DTO.CUS_CNT_Telephone = Convert.ToString(CNT.CUS_CNT_Telephone);
                                    B_DTO.CUS_CNT_Email = Convert.ToString(CNT.CUS_CNT_Email);

                                    if (CNT.CUS_CNT_Number == 0)
                                    {
                                        B_DTO.CUS_Id = 24; // INSERT
                                    }
                                    else
                                    {
                                        B_DTO.CUS_CNT_Number = CNT.CUS_CNT_Number;
                                        B_DTO.CUS_Id = 25; // UPDATE
                                    }

                                    B_DAO.CustomerDB(B_DTO);
                                }
                            }
                            transaction.Complete();

                            BH_DTO.Reset();
                            B_Head_DTO.Reset();
                            GST_DTO = BH_DTO.CUS_GST_List!;
                            WHT_DTO = BH_DTO.CUS_WHT_List!;
                            Add_DTO = BH_DTO.CUS_Add_List!;
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

            List<CustomerList_DTO> BList = CUSTGetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            var PageList = PaginatedList_DTO<CustomerList_DTO>.CreateAsync(BList, DPageNumber ?? 1, DPageSize);

            if (Mode == "Save" || Mode == "Update")
            {
                var JsonCUSer = JsonSerializer.Serialize(Original_BH_DTO);
                var CUSHead_DTO = JsonSerializer.Deserialize<CustomerHead_DTO>(JsonCUSer);
                CUSHead_DTO.Customer_List = PageList;
                return View(CUSHead_DTO);
            }
            else
            {
                B_Head_DTO.Customer_List = PageList;
                B_Head_DTO.CUS_GST_List = GST_DTO;
                B_Head_DTO.CUS_WHT_List = WHT_DTO;
                B_Head_DTO.CUS_Add_List = Add_DTO;
                B_Head_DTO.CUS_Contact_List = Contact_DTO;
                return View(B_Head_DTO);
            }
        }


        //add here post----
        List<CustomerList_DTO> CUSTGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            B_DTO.CUS_Id = 21;
            B_DTO.CUS_CreatorCode = Convert.ToInt32(UserCode);
            DS = B_DAO.CustomerDB(B_DTO);
            List<CustomerList_DTO> B_List = SM_DL.CustomerList(DS.Tables[0]);

            ViewBag.CustomerGroup = Help.GetCat(DS.Tables[1]);
            ViewBag.CustomerCategory = Help.GetCat(DS.Tables[2]);
            //ViewBag.CustomerSegment = Help.GetCat(DS.Tables[3]);
            //ViewBag.CustomerSubsegment = Help.GetCat(DS.Tables[4]);
            ViewBag.Registration = Help.GetCat(DS.Tables[3]);
            ViewBag.Currency = Help.GetCat(DS.Tables[4]);
            ViewBag.NatureOfAssessee = Help.GetCat(DS.Tables[5]);
            ViewBag.YesNo = Help.GetUnder(DS.Tables[6]);

            ViewBag.WHT_Category = Help.GetCat(DS.Tables[7]);
            ViewBag.WHT_Type = Help.GetCat(DS.Tables[8]);
            ViewBag.WHT_Tax = Help.GetCat(DS.Tables[9]);
            ViewBag.GST_Type = Help.GetCat(DS.Tables[10]);
            ViewBag.Location = Help.GetUnder(DS.Tables[11]);
            ViewBag.AddressType = Help.GetCat(DS.Tables[12]);
            ViewBag.Warehouse = Help.GetCat(DS.Tables[13]);
            ViewBag.GST_Category = Help.GetCat(DS.Tables[14]);
            ViewBag.GST_TaxCluster = Help.GetCat(DS.Tables[15]);
        

            ViewBag.AssesseeTerritory =  Help.GetCat(DS.Tables[16]);
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

            var Key = B_List.OrderByDescending(Cs => Cs.CUS_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.CUS_Name!.ToLower().Contains(Search.ToLower()) || K.CUS_Name!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.CUS_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.CUS_Name!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.CUS_Name!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.CUS_Number);
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

        [Route("sale/master/CUSer/duplicate")]
        public Boolean CustomerDuplicate(String? Title, String? Number)
        {
            B_DTO.CUS_CreatorCode = Convert.ToInt32(UserCode);
            B_DTO.CUS_Name = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                B_DTO.CUS_Id = 5;
            }
            else
            {
                B_DTO.CUS_Number = Convert.ToInt32(Number);
                B_DTO.CUS_Id = 6;
            }
            DS = B_DAO.CustomerDB(B_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region customer group
        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;
        [Route("jobwork/master/customer-group")]
        public IActionResult CustomerGroup(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
              List<CustomerGroup_DTO> BG_List = CGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BG_DTO.JCG_Number);
           // List<CustomerGroup_DTO> BG_List = CGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, 1);
            return View(PaginatedList<CustomerGroup_DTO>.CreateAsync(BG_List, DPageNumber ?? 1, DPageSize));
        }
        [Route("jobwork/master/customer-group")]
        [HttpPost]
        public IActionResult CustomerGroup(CustomerGroup_DTO BG_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                BG_DTO.JCG_CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        BG_DTO.JCG_Id = 6;
                        DS = BG_DAO.JWCustomerGroupDB(BG_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            BG_DTO.JCG_Id = 1;
                            BG_DAO.JWCustomerGroupDB(BG_DTO);

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
                        BG_DTO.JCG_DeleteNumbers = DeleteNumbers;
                        BG_DTO.JCG_Id = 3;
                        BG_DAO.JWCustomerGroupDB(BG_DTO);

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
                        BG_DTO.JCG_Number = Number;
                        BG_DTO.JCG_Id = 8;
                        BG_DAO.JWCustomerGroupDB(BG_DTO);

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
                    BG_DTO.JCG_Id = 4;
                    DS = BG_DAO.JWCustomerGroupDB(BG_DTO);
                    ViewBag.BuyerGroupEdit = SM_DL.GroupList(DS.Tables[0]).FirstOrDefault();
                }
                else if (Mode == "Update")
                {
                    BG_DTO.JCG_Id = 7;
                    DS = BG_DAO.JWCustomerGroupDB(BG_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            BG_DTO.JCG_Id = 5;
                            BG_DAO.JWCustomerGroupDB(BG_DTO);

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

                List<CustomerGroup_DTO> BG_List = CGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BG_DTO.JCG_Number);
              //  List<CustomerGroup_DTO> BG_List = CGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, 1);
                return View(PaginatedList<CustomerGroup_DTO>.CreateAsync(BG_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }

        List<CustomerGroup_DTO> CGGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                BG_DTO.JCG_Number = Convert.ToInt64(Number);
            }

            BG_DTO.JCG_Id = 2;
            BG_DTO.JCG_CreatorCode = Convert.ToInt32(UserCode);
            DS = BG_DAO.JWCustomerGroupDB(BG_DTO);
            #region filter status<>1
            DataRow[] filteredRows = DS.Tables[0].Select("JCG_Status <> 1");

            System.Data.DataTable dtFiltered = DS.Tables[0].Clone();
            foreach (DataRow row in filteredRows)
            {
                dtFiltered.ImportRow(row);
            }
            #endregion
            BG_List = SM_DL.GroupList(dtFiltered);
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

            var Key = BG_List.OrderByDescending(Cs => Cs.JCG_JW_CustomerGroup);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.JCG_JW_CustomerGroup!.ToLower().Contains(Search.ToLower()) || K.JCG_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.JCG_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.JCG_JW_CustomerGroup!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.JCG_JW_CustomerGroup!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.JCG_Number);
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

        #endregion

        #region customer category

        //buyer Category
        [Route("jobwork/master/customer-category")]
        public IActionResult CustomerCategory(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<CustomerCategory_DTO> BYC_List = CustomerCategoryGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BC_DTO.JCC_Number);
            return View(PaginatedList<CustomerCategory_DTO>.CreateAsync(BYC_List, DPageNumber ?? 1, DPageSize));
        }


        [Route("jobwork/master/customer-category")]
        [HttpPost]
        public IActionResult CustomerCategory(CustomerCategory_DTO BC_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            BC_DTO.JCC_CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    BC_DTO.JCC_Id = 6;
                    DS = BC_DAO.CustomerCategoryDB(BC_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        BC_DTO.JCC_Id = 1;
                        BC_DAO.CustomerCategoryDB(BC_DTO);

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
                    BC_DTO.JCC_DeleteNumbers = DeleteNumbers;
                    BC_DTO.JCC_Id = 3;
                    BC_DAO.CustomerCategoryDB(BC_DTO);

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
                    BC_DTO.JCC_Number = Number;
                    BC_DTO.JCC_Id = 8;
                    BC_DAO.CustomerCategoryDB(BC_DTO);

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
                //BC_DTO.JCC_Number = Number;
                BC_DTO.JCC_Id = 4;
                DS = BC_DAO.CustomerCategoryDB(BC_DTO);             
                ViewBag.CustomerCategoryEdit = SM_DL.CustomerCategoryList(DS.Tables[0]).FirstOrDefault();

            }
            else if (Mode == "Update")
            {
                BC_DTO.JCC_Id = 7;
                DS = BC_DAO.CustomerCategoryDB(BC_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        BC_DTO.JCC_Id = 5;
                        BC_DAO.CustomerCategoryDB(BC_DTO);

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
           List<CustomerCategory_DTO> BYC_List = CustomerCategoryGetData(SortOrder, Search, PageNumber, PSize, PageFilter, BC_DTO.JCC_Number);

         //   List<CustomerCategory_DTO> BYC_List = CustomerCategoryGetData(SortOrder, Search, PageNumber, PSize, PageFilter, 1);
            return View(PaginatedList<CustomerCategory_DTO>.CreateAsync(BYC_List, DPageNumber ?? 1, DPageSize));
        }
        List<CustomerCategory_DTO> CustomerCategoryGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;
            if (Number != null)
            {
                BC_DTO.JCC_Number = Convert.ToInt64(Number);
            }

            BC_DTO.JCC_Id = 2;
            BC_DTO.JCC_CreatorCode = Convert.ToInt32(UserCode);
            DS = BC_DAO.CustomerCategoryDB(BC_DTO);

            #region filter status<>1
            DataRow[] filteredRows = DS.Tables[0].Select("JCC_Status <> 1");

            System.Data.DataTable dtFiltered = DS.Tables[0].Clone();
            foreach (DataRow row in filteredRows)
            {
                dtFiltered.ImportRow(row);
            }
            #endregion



            BC_List = SM_DL.CustomerCategoryList(dtFiltered);
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

            var Key = BC_List.OrderByDescending(Cs => Cs.JCC_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.JCC_JW_CustomerCategory!.ToLower().Contains(Search.ToLower()) || K.JCC_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.JCC_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.JCC_JW_CustomerCategory!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.JCC_JW_CustomerCategory!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.JCC_Number);
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

        [Route("jobwork/master/customer-category/duplicate")]
        public Boolean BuyerCategoryDuplicate(String? Title, String? Number)
        {
            BC_DTO.JCC_CreatorCode = Convert.ToInt32(UserCode);
            BC_DTO.JCC_JW_CustomerCategory = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                BC_DTO.JCC_Id = 6;
            }
            else
            {
                BC_DTO.JCC_Number = Convert.ToInt32(Number);
                BC_DTO.JCC_Id = 7;
            }
            DS = BC_DAO.CustomerCategoryDB(BC_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion

        #region shift

        [Route("jobwork/master/shift")]
        public IActionResult Shift(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            List<Shift_DTO> PS_List = GetData(SortOrder, Search, PageNumber, PSize, PageFilter);
            return View(PaginatedList<Shift_DTO>.CreateAsync(PS_List, DPageNumber ?? 1, DPageSize));

        }

        [Route("jobwork/master/shift")]
        [HttpPost]
        public IActionResult Shift(Shift_DTO PS_DTO, String? DeleteNumbers, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {

            try
            {
                PS_DTO.CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        PS_DTO.Id = 6;
                        DS = PS_DAO.ShiftDB(PS_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            PS_DTO.Id = 1;
                            PS_DAO.ShiftDB(PS_DTO);

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
                    PS_DAO.ShiftDB(PS_DTO);

                    ModelState.Clear();
                }
                else if (Mode == "Delete")
                {
                    PS_DTO.Id = 8;
                    PS_DAO.ShiftDB(PS_DTO);

                    ModelState.Clear();
                }
                else if (Mode == "Clear")
                {
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    PS_DTO.Id = 4;
                    DS = PS_DAO.ShiftDB(PS_DTO);
                    ViewBag.Submaster = PS_DL.PSList(DS.Tables[0]).FirstOrDefault(); ;
                }
                else if (Mode == "Update")
                {
                    PS_DTO.Id = 7;
                    DS = PS_DAO.ShiftDB(PS_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            PS_DTO.Id = 5;
                            PS_DAO.ShiftDB(PS_DTO);

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
                List<Shift_DTO> PS_List = GetData(SortOrder, Search, PageNumber, PSize, PageFilter);

                return View(PaginatedList<Shift_DTO>.CreateAsync(PS_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }

        }
        List<Shift_DTO> GetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            DPageSize = 10;

            PS_DTO.Id = 2;
            PS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = PS_DAO.ShiftDB(PS_DTO);
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
                Key = Key.Where(K => K.ShiftName!.ToLower().Contains(Search.ToLower()) || K.Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.ShiftName!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.ShiftName!);
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


        [Route("jobwork/master/shift/duplicate")]
        public Boolean ShiftDuplicate(String? Title, String? Number)
        {
            PS_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Title != null)
            {
                PS_DTO.ShiftName = Convert.ToString(Title).Trim();
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
            DS = PS_DAO.ShiftDB(PS_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion



        #region Process



        //Process
        [Route("jobwork/master/process")]
        public IActionResult Process(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<JW_Process_DTO> W_List = WGetData(SortOrder, Search, PageNumber, PSize, PageFilter, W_DTO.ProcessNumber);
            return View(PaginatedList<JW_Process_DTO>.CreateAsync(W_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("jobwork/master/process")]
        [HttpPost]
        public IActionResult Process(JW_Process_DTO W_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            W_DTO.CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    W_DTO.Id = 6;
                    DS = W_DAO.JW_ProcessDB(W_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        W_DTO.Id = 1;
                        W_DAO.JW_ProcessDB(W_DTO);

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
                    W_DAO.JW_ProcessDB(W_DTO);

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
                    W_DTO.ProcessNumber = Number;
                    W_DTO.Id = 8;
                    W_DAO.JW_ProcessDB(W_DTO);

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
                DS = W_DAO.JW_ProcessDB(W_DTO);
                ViewBag.WarehouseEdit = SM_DL.WList(DS.Tables[0]).FirstOrDefault();
            }
            else if (Mode == "Update")
            {
                W_DTO.Id = 7;
                DS = W_DAO.JW_ProcessDB(W_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        W_DTO.Id = 5;
                        W_DAO.JW_ProcessDB(W_DTO);

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
                        DS = W_DAO.JW_ProcessDB(W_DTO);
                        ViewBag.WarehouseEdit = SM_DL.WList(DS.Tables[0]).FirstOrDefault();
                    }
                }
            }


            List<JW_Process_DTO> W_List = WGetData(SortOrder, Search, PageNumber, PSize, PageFilter, W_DTO.ProcessNumber);
            return View(PaginatedList<JW_Process_DTO>.CreateAsync(W_List, DPageNumber ?? 1, DPageSize));
        }
        List<JW_Process_DTO> WGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                W_DTO.ProcessNumber = Convert.ToInt64(Number);
            }

            W_DTO.Id = 2;
            W_DTO.CreatorCode = Convert.ToInt32(UserCode);
            DS = W_DAO.JW_ProcessDB(W_DTO);
            W_List = SM_DL.WList(DS.Tables[0]);
            ViewBag.UOM = Help.GetCat(DS.Tables[1]);
            ViewBag.ScrapItemCode = Help.GetUnder(DS.Tables[2]);
            ViewBag.SAC = Help.GetUnder(DS.Tables[3]);

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

            var Key = W_List.OrderByDescending(Cs => Cs.ProcessNumber);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.ConsumptionUoM!.ToLower().Contains(Search.ToLower()) || K.Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.ProcessNumber);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.ConsumptionUoM!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.ConsumptionUoM!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.ProcessNumber);
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



        [Route("jobwork/master/process/duplicate")]
        public Boolean ProcessDuplicate(String? Title, String? Number)
        {
            W_DTO.CreatorCode = Convert.ToInt32(UserCode);
            W_DTO.ConsumptionUoM = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                W_DTO.Id = 6;
            }
            else
            {
                W_DTO.ProcessNumber = Convert.ToInt32(Number);
                W_DTO.Id = 7;
            }
            DS = W_DAO.JW_ProcessDB(W_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion


        #region work centre group
        //buyer group
        [Route("jobwork/master/workcentre-group")]
        public IActionResult WorkCentreGroup(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<WorkCentreGroup_DTO> WG_List = BGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, WG_DTO.WCG_Number);

           // List<WorkCentreGroup_DTO> WG_List = BGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, 1);
            return View(PaginatedList<WorkCentreGroup_DTO>.CreateAsync(WG_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("jobwork/master/workcentre-group")]
        [HttpPost]
        public IActionResult WorkCentreGroup(WorkCentreGroup_DTO WG_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            try
            {
                WG_DTO.WCG_CreatorCode = Convert.ToInt32(UserCode);
                if (Mode == "Save")
                {
                    if (ModelState.IsValid)
                    {
                        WG_DTO.WCG_Id = 6;
                        DS = WG_DAO.WorkCentreGroupDB(WG_DTO);
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            WG_DTO.WCG_Id = 1;
                            WG_DAO.WorkCentreGroupDB(WG_DTO);

                            WG_DTO.Reset();
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
                        WG_DTO.WCG_DeleteNumbers = DeleteNumbers;
                        WG_DTO.WCG_Id = 3;
                        WG_DAO.WorkCentreGroupDB(WG_DTO);

                        WG_DTO.Reset();
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
                        WG_DTO.WCG_Number = Number;
                        WG_DTO.WCG_Id = 8;
                        WG_DAO.WorkCentreGroupDB(WG_DTO);

                        WG_DTO.Reset();
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
                    WG_DTO.Reset();
                    ModelState.Clear();
                }
                else if (Mode == "Edit")
                {
                    WG_DTO.WCG_Id = 4;
                    DS = WG_DAO.WorkCentreGroupDB(WG_DTO);
                    ViewBag.WorkCentreGroupEdit = SM_DL.WorkCentreGroupList(DS.Tables[0]).FirstOrDefault();
                }
                else if (Mode == "Update")
                {
                    WG_DTO.WCG_Id = 7;
                    DS = WG_DAO.WorkCentreGroupDB(WG_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        if (ModelState.IsValid)
                        {
                            WG_DTO.WCG_Id = 5;
                            WG_DAO.WorkCentreGroupDB(WG_DTO);

                            WG_DTO.Reset();
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
                List<WorkCentreGroup_DTO> WG_List = BGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, WG_DTO.WCG_Number);


              //  List<WorkCentreGroup_DTO> WG_List = BGGetData(SortOrder, Search, PageNumber, PSize, PageFilter, WG_DTO.WCG_Number);
                return View(PaginatedList<WorkCentreGroup_DTO>.CreateAsync(WG_List, DPageNumber ?? 1, DPageSize));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = 2;
                ViewBag.ErrorMessage = Valid.CatchValid(ex.Message);
                return View();
            }
        }
        List<WorkCentreGroup_DTO> BGGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                WG_DTO.WCG_Number = Convert.ToInt64(Number);
            }

            WG_DTO.WCG_Id = 2;
            WG_DTO.WCG_CreatorCode = Convert.ToInt32(UserCode);
            DS = WG_DAO.WorkCentreGroupDB(WG_DTO);
            WG_List = SM_DL.WorkCentreGroupList(DS.Tables[0]);
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

            var Key = WG_List.OrderByDescending(Cs => Cs.WCG_WorkCentreGroup);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.WCG_WorkCentreGroup!.ToLower().Contains(Search.ToLower()) || K.WCG_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.WCG_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.WCG_WorkCentreGroup!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.WCG_WorkCentreGroup!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.WCG_Number);
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

        [Route("jobwork/master/workcentre-group/duplicate")]
        public Boolean WorkCentreGroupDuplicate(String? Title, String? Number)
        {
            WG_DTO.WCG_CreatorCode = Convert.ToInt32(UserCode);
            WG_DTO.WCG_WorkCentreGroup = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                WG_DTO.WCG_Id = 6;
            }
            else
            {
                WG_DTO.WCG_Number = Convert.ToInt32(Number);
                WG_DTO.WCG_Id = 7;
            }
            DS = WG_DAO.WorkCentreGroupDB(WG_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        #endregion



        #region work centre



        //Process
        [Route("jobwork/master/workcentre")]
        public IActionResult WorkCentre(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            List<WorkCentre_DTO> WC_List = WCGetData(SortOrder, Search, PageNumber, PSize, PageFilter, WC_DTO.WC_Number);
            return View(PaginatedList<WorkCentre_DTO>.CreateAsync(WC_List, DPageNumber ?? 1, DPageSize));
        }

        [Route("jobwork/master/workcentre")]
        [HttpPost]
        public IActionResult WorkCentre(WorkCentre_DTO WC_DTO, String? DeleteNumbers, Int64 Number, String? Mode, String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter)
        {
            WC_DTO.WC_CreatorCode = Convert.ToInt32(UserCode);
            if (Mode == "Save")
            {
                if (ModelState.IsValid)
                {
                    WC_DTO.WC_Id = 6;
                    DS = WC_DAO.WorkCentreDB(WC_DTO);
                    if (DS.Tables[0].Rows.Count == 0)
                    {
                        WC_DTO.WC_Id = 1;
                        WC_DAO.WorkCentreDB(WC_DTO);

                        WC_DTO.Reset();
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
                    WC_DTO.WC_DeleteNumbers = DeleteNumbers;
                    WC_DTO.WC_Id = 3;
                    WC_DAO.WorkCentreDB(WC_DTO);

                    WC_DTO.Reset();
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
                    WC_DTO.WC_Number = Number;
                    WC_DTO.WC_Id = 8;
                    WC_DAO.WorkCentreDB(WC_DTO);

                    WC_DTO.Reset();
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
                WC_DTO.Reset();
                ModelState.Clear();
            }
            else if (Mode == "Edit")
            {
                WC_DTO.WC_Id = 4;
                DS = WC_DAO.WorkCentreDB(WC_DTO);
                ViewBag.WarehouseEdit = SM_DL.WorkList(DS.Tables[0]).FirstOrDefault();
            }
            else if (Mode == "Update")
            {
                WC_DTO.WC_Id = 7;
                DS = WC_DAO.WorkCentreDB(WC_DTO);
                if (DS.Tables[0].Rows.Count == 0)
                {
                    if (ModelState.IsValid)
                    {
                        WC_DTO.WC_Id = 5;
                        WC_DAO.WorkCentreDB(WC_DTO);

                        WC_DTO.Reset();
                        ModelState.Clear();
                    }
                    else
                    {
                        var Errors = ModelState.SelectMany(m => m.Value!.Errors.Select(s => new { s.ErrorMessage })).Select(p => p.ErrorMessage);
                        string CombinedString = string.Join("<br/>", Errors);

                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = CombinedString;

                        WC_DTO.WC_Id = 4;
                        DS = WC_DAO.WorkCentreDB(WC_DTO);
                        ViewBag.WarehouseEdit = SM_DL.WList(DS.Tables[0]).FirstOrDefault();
                    }
                }
            }


            List<WorkCentre_DTO> WC_List = WCGetData(SortOrder, Search, PageNumber, PSize, PageFilter, WC_DTO.WC_Number);
            return View(PaginatedList<WorkCentre_DTO>.CreateAsync(WC_List, DPageNumber ?? 1, DPageSize));
        }
        List<WorkCentre_DTO> WCGetData(String? SortOrder, String? Search, Int32? PageNumber, Int32 PSize, String? PageFilter, Int64? Number)
        {
            DPageSize = 10;

            if (Number != null)
            {
                WC_DTO.WC_Number = Convert.ToInt64(Number);
            }

            WC_DTO.WC_Id = 2;
            WC_DTO.WC_CreatorCode = Convert.ToInt32(UserCode);
            DS = WC_DAO.WorkCentreDB(WC_DTO);
            WC_List = SM_DL.WorkCentreList(DS.Tables[0]);
            ViewBag.WorkCentreGroup = Help.GetCat(DS.Tables[1]);
            ViewBag.Warehouse = Help.GetUnder(DS.Tables[2]);
            ViewBag.Process = Help.GetUnder(DS.Tables[3]);


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

            var Key = WC_List.OrderByDescending(Cs => Cs.WC_Number);
            if (!String.IsNullOrEmpty(Search))
            {
                Key = Key.Where(K => K.WC_Description!.ToLower().Contains(Search.ToLower()) || K.WC_Description!.ToLower().Contains(Search.ToLower())).OrderByDescending(Cs => Cs.WC_Number);
            }

            switch (SortOrder)
            {
                case "Title_desc":
                    Key = Key.OrderByDescending(K => K.WC_Description!);
                    break;
                case "Title":
                    Key = Key.OrderBy(K => K.WC_Description!);
                    break;
                default:
                    Key = Key.OrderByDescending(K => K.WC_Number);
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



        [Route("jobwork/master/workcentre/duplicate")]
        public Boolean WorkCentreDuplicate(String? Title, String? Number)
        {
            WC_DTO.WC_CreatorCode = Convert.ToInt32(UserCode);
            WC_DTO.WC_Description = Title;
            if (Convert.ToInt32(Number) == 0)
            {
                WC_DTO.WC_Id = 6;
            }
            else
            {
                WC_DTO.WC_Number = Convert.ToInt32(Number);
                WC_DTO.WC_Id = 7;
            }
            DS = WC_DAO.WorkCentreDB(WC_DTO);
            if (DS.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion


     

        #region JsonClone
        private T JsonClone<T>(T obj)
        {
            if (obj == null) return default(T);
            var json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(json);
        }
        #endregion

        

        public IActionResult Index()
        {
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class Buyer_DTO
    {
        public Int64 BUY_Number { get; set; }
        public String? BUY_Name { get; set; }
        public String? BUY_ContactPerson { get; set; }
        public String? BUY_ContactTelephone { get; set; }
        public String? BUY_ContactMobile { get; set; }
        public String? BUY_ContactEmail { get; set; }
        public String? BUY_AccountPerson { get; set; }
        public String? BUY_AccountTelephone { get; set; }
        public String? BUY_AccountMobile { get; set; }
        public String? BUY_AccountEmail { get; set; }
        public Int64 BUY_LOC_Number { get; set; }
        public Int64 BUY_BYG_Number { get; set; }
        public Int64 BUY_BYC_Number { get; set; }
        public Int64 BUY_BYS_Number { get; set; }
        public Int64 BUY_BSS_Number { get; set; }
        public String? BUY_PaymentTerms { get; set; }
        public String? BUY_PaymentMode { get; set; }
        public Int16 BUY_CreditDays { get; set; }
        public Double BUY_CreditLimit { get; set; }
        public Int64 BUY_CUR_Number { get; set; }
        public String? BUY_AccountName { get; set; }
        public String? BUY_AccountNumber { get; set; }
        public String? BUY_IFSC { get; set; }
        public String? BUY_BankName { get; set; }
        public String? BUY_DeliveryTerms { get; set; }
        public String? BUY_DeliveryMode { get; set; }
        public Int64 BUY_RT_Number { get; set; }
        public String? BUY_GSTIN { get; set; }
        public Int64 BUY_AT_Number { get; set; }
        public String? BUY_PAN { get; set; }
        public Int64 BUY_YN_Number { get; set; }
        public Int64 BUY_AN_Number { get; set; }

        public Int64 BUY_WHT_Number { get; set; }
        public Int64 BUY_WHT_WHTC_Number { get; set; }
        public Int64 BUY_WHT_WHTT_Number { get; set; }
        public Int64 BUY_WHT_WHT_Number { get; set; }
        public String? BUY_WHT_FromDate { get; set; }
        public String? BUY_WHT_ToDate { get; set; }

        public Int64 BUY_GST_Number { get; set; }
        public Int64 BUY_GST_GSTC_Number { get; set; }
        public Int64 BUY_GST_GSTT_Number { get; set; }
        public Int64? BUY_GST_TCT_Number { get; set; }
        public String? BUY_GST_FromDate { get; set; }
        public String? BUY_GST_ToDate { get; set; }

        public Int64 BUY_ADD_Number { get; set; }
        public Int64 BUY_ADD_ADTP_Number { get; set; }
        public String? BUY_ADD_AddressID { get; set; }
        public String? BUY_ADD_Address { get; set; }
        public String? BUY_ADD_City { get; set; }
        public String? BUY_ADD_State { get; set; }
        public String? BUY_ADD_Country { get; set; }
        public String? BUY_ADD_Pin { get; set; }
        public String? BUY_ADD_GSTIN { get; set; }
        public Int16 BUY_ADD_Primary { get; set; }

        public String? BUY_DeleteNumbers { get; set; }
        public Int64 BUY_CreatorCode { get; set; }
        public Int16 BUY_Id { get; set; }

        public void Reset()
        {
            this.BUY_Number = 0;
            this.BUY_Name = "";
            this.BUY_ContactPerson = "";
            this.BUY_ContactTelephone = "";
            this.BUY_ContactMobile = "";
            this.BUY_ContactEmail = "";
            this.BUY_AccountPerson = "";
            this.BUY_AccountTelephone = "";
            this.BUY_AccountMobile = "";
            this.BUY_AccountEmail = "";
            this.BUY_LOC_Number = 0;
            this.BUY_BYG_Number = 0;
            this.BUY_BYC_Number = 0;
            this.BUY_BYS_Number =0;
            this.BUY_BSS_Number = 0;
            this.BUY_PaymentTerms = "";
            this.BUY_PaymentMode = "";
            this.BUY_CreditDays = 0;
            this.BUY_CreditLimit = 0;
            this.BUY_CUR_Number = 0;
            this.BUY_AccountName = "";
            this.BUY_AccountNumber = "";
            this.BUY_IFSC = "";
            this.BUY_BankName = "";
            this.BUY_DeliveryTerms = "";
            this.BUY_DeliveryMode = "";
            this.BUY_RT_Number = 0;
            this.BUY_GSTIN = "";
            this.BUY_AT_Number = 0;
            this.BUY_PAN = "";
            this.BUY_YN_Number = 0;
            this.BUY_AN_Number = 0;

            this.BUY_WHT_Number = 0;
            this.BUY_WHT_WHTC_Number = 0;
            this.BUY_WHT_WHTT_Number = 0;
            this.BUY_WHT_WHT_Number = 0;
            this.BUY_WHT_FromDate = "";
            this.BUY_WHT_ToDate = "";


            this.BUY_GST_Number = 0;
            this.BUY_GST_GSTC_Number = 0;
            this.BUY_GST_GSTT_Number = 0;
            this.BUY_GST_TCT_Number = 0;
            this.BUY_GST_FromDate = "";
            this.BUY_GST_ToDate = "";

            this.BUY_ADD_Number = 0;
            this.BUY_ADD_ADTP_Number = 0;
            this.BUY_ADD_Address = "";
            this.BUY_ADD_City = "";
            this.BUY_ADD_State = "";
            this.BUY_ADD_Country = "";
            this.BUY_ADD_Pin = "";
            this.BUY_ADD_GSTIN = "";
            this.BUY_ADD_Primary = 0;

            this.BUY_DeleteNumbers = "";
        }
    }

    public class BuyerList_DTO
    {
        public Int64 BUY_Number { get; set; }

        [Display(Name = "Buyer Name")]
        public String? BUY_Name { get; set; }

        [Display(Name = "Buyer Location")]
        public String? BUY_LOC_Name { get; set; }

        [Display(Name = "Buyer Group")]
        public String? BUY_BYG_Name { get; set; }

        [Display(Name = "Buyer Category")]
        public String? BUY_BYC_Name { get; set; }

        [Display(Name = "Buyer Segment")]
        public String? BUY_BYS_Name { get; set; }

        [Display(Name = "Buyer Subsegment")]
        public String? BUY_BSS_Name { get; set; }

        [Display(Name = "Currency")]
        public String? BUY_CUR_Name { get; set; }
        public String? BUY_CUR_Number { get; set; }
    }

    public class BuyerOrderList_DTO
    {
        public Int64 BUY_Number { get; set; }
        public String? BUY_Name { get; set; }
        public String? BUY_LOC_Number{ get; set; }
        public String? BUY_CUR_Name { get; set; }
        public String? BUY_CUR_Number { get; set; }
        public String? BUY_CUR_DecimalPlaces { get; set; }
        public Int64 BUY_TCT_Number { get; set; }
        public Int64 BUY_WHT_Number { get; set; }
    }
    public class BuyerHead_DTO
    {
        public Int64 BUY_Number { get; set; }

        [Display(Name = "Buyer Name")]
        [MaxLength(100, ErrorMessage = "Buyer Name be longer than 100 characters.")]
        [Required(ErrorMessage = "Buyer Name is Required")]
        public String? BUY_Name { get; set; }


        [Display(Name = "Contact Person")]
        [MaxLength(50, ErrorMessage = "Contact Person be longer than 50 characters.")]
        public String? BUY_ContactPerson { get; set; }

        [Display(Name = "Telephone")]
        [MaxLength(25, ErrorMessage = "Telephone be longer than 25 characters.")]
        public String? BUY_ContactTelephone { get; set; }

        [Display(Name = "Mobile")]
        [MaxLength(25, ErrorMessage = "Mobile be longer than 25 characters.")]
        public String? BUY_ContactMobile { get; set; }

        [Display(Name = "E-mail")]
        [MaxLength(50, ErrorMessage = "E-mail be longer than 50 characters.")]
        public String? BUY_ContactEmail { get; set; }

        [Display(Name = "Accounts Person")]
        [MaxLength(50, ErrorMessage = "Accounts Person be longer than 50 characters.")]
        public String? BUY_AccountPerson { get; set; }

        [Display(Name = "Telephone")]
        [MaxLength(25, ErrorMessage = "Telephone be longer than 25 characters.")]
        public String? BUY_AccountTelephone { get; set; }

        [Display(Name = "Mobile")]
        [MaxLength(25, ErrorMessage = "Mobile be longer than 25 characters.")]
        public String? BUY_AccountMobile { get; set; }

        [Display(Name = "E-mail")]
        [MaxLength(50, ErrorMessage = "E-mail be longer than 50 characters.")]
        public String? BUY_AccountEmail  { get; set; }


        [Display(Name = "Buyer Location")]
        [Required(ErrorMessage = "Buyer Location is Required")]
        public Int64? BUY_LOC_Number { get; set; }

        [Display(Name = "Buyer Group")]
        [Required(ErrorMessage = "Buyer Group is Required")]
        public Int64? BUY_BYG_Number { get; set; }

        [Display(Name = "Buyer Category")]
        [Required(ErrorMessage = "Buyer Category is Required")]
        public Int64? BUY_BYC_Number { get; set; }

        [Display(Name = "Buyer Segment")]
        [Required(ErrorMessage = "Buyer Segment is Required")]
        public Int64? BUY_BYS_Number { get; set; }

        [Display(Name = "Buyer Subsegment")]
        [Required(ErrorMessage = "Buyer Subsegment is Required")]
        public Int64? BUY_BSS_Number { get; set; }

        [Display(Name = "Terms of Payment")]
        [MaxLength(250, ErrorMessage = "Terms of Payment be longer than 250 characters.")]
        public String? BUY_PaymentTerms { get; set; }

        [Display(Name = "Mode of Payment")]
        [MaxLength(50, ErrorMessage = "Mode of Payment be longer than 50 characters.")]
        public String? BUY_PaymentMode { get; set; }

        [Display(Name = "Credit Days")]
        [MaxLength(3, ErrorMessage = "Credit Days be longer than 3 characters.")]
        public String? BUY_CreditDays { get; set; }

        [Display(Name = "Credit Limit")]
        public String? BUY_CreditLimit { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public Int64? BUY_CUR_Number { get; set; }

        [Display(Name = "Account Name")]
        [MaxLength(100, ErrorMessage = "Account Name be longer than 100 characters.")]
        public String? BUY_AccountName { get; set; }

        [Display(Name = "Account Number")]
        [MaxLength(25, ErrorMessage = "Account Number be longer than 25 characters.")]
        public String? BUY_AccountNumber { get; set; }

        [Display(Name = "IFSC")]
        [MaxLength(25, ErrorMessage = "IFSC be longer than 25 characters.")]
        public String? BUY_IFSC { get; set; }

        [Display(Name = "Bank Name")]
        [MaxLength(100, ErrorMessage = "Bank Name be longer than 100 characters.")]
        public String? BUY_BankName { get; set; }

        [Display(Name = "Terms of delivery")]
        [MaxLength(250, ErrorMessage = "Terms of delivery be longer than 250 characters.")]
        public String? BUY_DeliveryTerms { get; set; }

        [Display(Name = "Mode of delivery")]
        [MaxLength(50, ErrorMessage = "Mode of delivery be longer than 50 characters.")]
        public String? BUY_DeliveryMode { get; set; }

        [Display(Name = "Registration Type")]
        public Int64? BUY_RT_Number { get; set; }

        [Display(Name = "GST Number")]
        [MaxLength(15, ErrorMessage = "GST Number be longer than 15 characters.")]
        public String? BUY_GSTIN { get; set; }

        [Display(Name = "Assessee Territory")]
        [Required(ErrorMessage = "Assessee Territory is Required")]
        public Int64? BUY_AT_Number { get; set; }
        public String? BUY_AT_Name { get; set; }

        [Display(Name = "PAN / IT No.")]
        [MaxLength(10, ErrorMessage = "PAN / IT No. be longer than 10 characters.")]
        public String? BUY_PAN { get; set; }

        [Display(Name = "Calculate withholding tax")]
        public Int64? BUY_YN_Number { get; set; }

        [Display(Name = "Nature of assessee")]
        public Int64? BUY_AN_Number { get; set; }

        public PaginatedList_DTO<BuyerList_DTO>? Buyer_List { get; set; }
        public List<BuyerWHT_DTO>? BUY_WHT_List { get; set; }
        public List<BuyerGST_DTO>? BUY_GST_List { get; set; }
        public List<BuyerAdd_DTO>? BUY_Add_List { get; set; }
        public void Reset()
        {
            this.BUY_Number = 0;
            this.BUY_Name = "";
            this.BUY_ContactPerson = "";
            this.BUY_ContactTelephone = "";
            this.BUY_ContactMobile = "";
            this.BUY_ContactEmail = "";
            this.BUY_AccountPerson = "";
            this.BUY_AccountTelephone = "";
            this.BUY_AccountMobile = "";
            this.BUY_AccountEmail = "";
            this.BUY_LOC_Number = 0;
            this.BUY_BYG_Number = 0;
            this.BUY_BYC_Number = 0;
            this.BUY_BYS_Number = 0;
            this.BUY_BSS_Number = 0;
            this.BUY_PaymentTerms = "";
            this.BUY_PaymentMode = "";
            this.BUY_CreditDays = "";
            this.BUY_CreditLimit = "";
            this.BUY_CUR_Number = 0;
            this.BUY_AccountName = "";
            this.BUY_AccountNumber = "";
            this.BUY_IFSC = "";
            this.BUY_BankName = "";
            this.BUY_DeliveryTerms = "";
            this.BUY_DeliveryMode = "";
            this.BUY_RT_Number = 0;
            this.BUY_GSTIN = "";
            this.BUY_AT_Number = 0;
            this.BUY_PAN = "";
            this.BUY_YN_Number = 0;
            this.BUY_AN_Number = 0;

            this.Buyer_List = null;
            this.BUY_WHT_List = null;
            this.BUY_GST_List = null;
            this.BUY_Add_List = null;
        }
    }
    public class BuyerWHT_DTO
    {
        public Int64 BUY_WHT_Number { get; set; }

        [Display(Name = "WHT Category")]
        [Required(ErrorMessage = "WHT Category is Required")]
        public String? BUY_WHT_WHTC_Number { get; set; }

        [Display(Name = "WHT Type")]
        [Required(ErrorMessage = "WHT Type is Required")]
        public String? BUY_WHT_WHTT_Number { get; set; }

        [Display(Name = "WH Tax Code")]
        [Required(ErrorMessage = "WH Tax Code is Required")]
        public String? BUY_WHT_WHT_Number { get; set; }

        [Display(Name = "Description")]
        public String? BUY_WHT_WHT_Description { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is Required")]
        public String? BUY_WHT_FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is Required")]
        public String? BUY_WHT_ToDate { get; set; }

        public Int16 BUY_WHT_IsDeleted { get; set; }

        public void Reset()
        {
            this.BUY_WHT_Number = 0;
            this.BUY_WHT_WHTC_Number = "0";
            this.BUY_WHT_WHTT_Number = "0";
            this.BUY_WHT_WHT_Number = "0";
            this.BUY_WHT_WHT_Description = "";
            this.BUY_WHT_FromDate = "";
            this.BUY_WHT_ToDate = "";
            this.BUY_WHT_IsDeleted = 0;
        }
    }
    public class BuyerGST_DTO
    {
        public Int64 BUY_GST_Number { get; set; }

        [Display(Name = "GST Category")]
        [Required(ErrorMessage = "GST Category is Required")]
        public String? BUY_GST_GSTC_Number { get; set; }
        public String? BUY_GST_GSTC_Name { get; set; }

        [Display(Name = "GST Type")]
        [Required(ErrorMessage = "GST Type is Required")]
        public String? BUY_GST_GSTT_Number { get; set; }

        [Display(Name = "GST Tax Cluster")]
        [Required(ErrorMessage = "GST Tax Cluster is Required")]
        public String? BUY_GST_TCT_Number { get; set; }

        [Display(Name = "GST Tax Cluster")]
        public String? BUY_GST_TCT_Name { get; set; }

        [Display(Name = "Description")]
        public String? BUY_GST_TCT_Description { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is Required")]
        public String? BUY_GST_FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is Required")]
        public String? BUY_GST_ToDate { get; set; }

        public Int16 BUY_GST_IsDeleted { get; set; }

        public void Reset()
        {
            this.BUY_GST_Number = 0;
            this.BUY_GST_GSTC_Number = "0";
            this.BUY_GST_GSTT_Number = "0";
            this.BUY_GST_TCT_Number = "0";
            this.BUY_GST_TCT_Description = "";
            this.BUY_GST_FromDate = "";
            this.BUY_GST_ToDate = "";
            this.BUY_GST_IsDeleted = 0;
        }
    }
    public class BuyerAdd_DTO
    {
        [Key]
        public Int64 BUY_ADD_Number { get; set; }

        [Display(Name = "Address Type")]
        [Required(ErrorMessage = "Address Type is Required")]
        public Int64 BUY_ADD_ADTP_Number { get; set; }

        [Display(Name = "Address ID")]
        [Required(ErrorMessage = "Address Type is Required")]
        public String? BUY_ADD_AddressID { get; set; }

        [Display(Name = "Address")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        [Required(ErrorMessage = "Address is Required")]
        public String? BUY_ADD_Address { get; set; }

        [Display(Name = "City")]
        [StringLength(25, ErrorMessage = "City cannot exceed 25 characters")]
        [Required(ErrorMessage = "City is Required")]
        public String? BUY_ADD_City { get; set; }

        [Display(Name = "State")]
        [StringLength(25, ErrorMessage = "State cannot exceed 25 characters")]
        [Required(ErrorMessage = "State is Required")]
        public String? BUY_ADD_State { get; set; }

        [Display(Name = "Country")]
        [StringLength(25, ErrorMessage = "Country cannot exceed 25 characters")]
        [Required(ErrorMessage = "Country is Required")]
        public String? BUY_ADD_Country { get; set; }

        [Display(Name = "PIN Code")]
        [StringLength(10, ErrorMessage = "PIN Code cannot exceed 10 characters")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "PIN Code must be 6 digits")]
        [Required(ErrorMessage = "PIN Code is Required")]
        public String? BUY_ADD_Pin { get; set; }

        [Display(Name = "GSTIN")]
        [StringLength(25, ErrorMessage = "GSTIN cannot exceed 25 characters")]
        public String? BUY_ADD_GSTIN { get; set; }

        [Display(Name = "Default")]
        public Boolean BUY_ADD_Primary { get; set; }

        public Int16 BUY_ADD_IsDeleted { get; set; }

        public void Reset()
        {
            this.BUY_ADD_Number = 0;
            this.BUY_ADD_ADTP_Number = 0;
            this.BUY_ADD_Address = "";
            this.BUY_ADD_City = "";
            this.BUY_ADD_State = "";
            this.BUY_ADD_Country = "";
            this.BUY_ADD_Pin = "";
            this.BUY_ADD_GSTIN = "";
            this.BUY_ADD_Primary = false;
            this.BUY_ADD_IsDeleted = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class Customer_DTO
    {
        public Int64 CUS_Number { get; set; }
        public String? CUS_Name { get; set; }
        public String? CUS_ContactPerson { get; set; }
        public String? CUS_ContactTelephone { get; set; }
        public String? CUS_ContactMobile { get; set; }
        public String? CUS_ContactEmail { get; set; }
        public String? CUS_AccountPerson { get; set; }
        public String? CUS_AccountTelephone { get; set; }
        public String? CUS_AccountMobile { get; set; }
        public String? CUS_AccountEmail { get; set; }
        public Int64 CUS_LOC_Number { get; set; }
        public Int64 CUS_BYG_Number { get; set; }

        public Int64 JWC_WH_Number { get; set; }
        public Int64 CUS_BYC_Number { get; set; }
        public Int64 CUS_BYS_Number { get; set; }
        public Int64 CUS_BSS_Number { get; set; }
        public String? CUS_PaymentTerms { get; set; }
        public String? CUS_PaymentMode { get; set; }
        public Int16 CUS_CreditDays { get; set; }
        public Double CUS_CreditLimit { get; set; }
        public Int64 CUS_CUR_Number { get; set; }
        public String? CUS_AccountName { get; set; }
        public String? CUS_AccountNumber { get; set; }
        public String? CUS_IFSC { get; set; }
        public String? CUS_BankName { get; set; }
        public String? CUS_DeliveryTerms { get; set; }
        public String? CUS_DeliveryMode { get; set; }
        public Int64 CUS_RT_Number { get; set; }
        public String? CUS_GSTIN { get; set; }
        public Int64 CUS_AT_Number { get; set; }
        public String? CUS_PAN { get; set; }
        public Int64 CUS_YN_Number { get; set; }
        public Int64 CUS_AN_Number { get; set; }

        public Int64 CUS_WHT_Number { get; set; }
        public Int64 CUS_WHT_WHTC_Number { get; set; }
        public Int64 CUS_WHT_WHTT_Number { get; set; }
        public Int64 CUS_WHT_WHT_Number { get; set; }
        public String? CUS_WHT_FromDate { get; set; }
        public String? CUS_WHT_ToDate { get; set; }

        public Int64 CUS_GST_Number { get; set; }
        public Int64 CUS_GST_GSTC_Number { get; set; }
        public Int64 CUS_GST_GSTT_Number { get; set; }
        public Int64? CUS_GST_TCT_Number { get; set; }
        public String? CUS_GST_FromDate { get; set; }
        public String? CUS_GST_ToDate { get; set; }

        public Int64 CUS_ADD_Number { get; set; }
        public Int64 CUS_ADD_ADTP_Number { get; set; }
        public String? CUS_ADD_AddressID { get; set; }
        public String? CUS_ADD_Address { get; set; }
        public String? CUS_ADD_City { get; set; }
        public String? CUS_ADD_State { get; set; }
        public String? CUS_ADD_Country { get; set; }
        public String? CUS_ADD_Pin { get; set; }
        public String? CUS_ADD_GSTIN { get; set; }
        public Int16 CUS_ADD_Primary { get; set; }

 
        public Int64 CUS_CNT_Number { get; set; }
 
        public string? CUS_CNT_ContactName { get; set; }
 
        public string? CUS_CNT_Department { get; set; }
 
        public string? CUS_CNT_Mobile { get; set; }
 
        public string? CUS_CNT_Telephone { get; set; }
 
        public string? CUS_CNT_Email { get; set; }



        public String? CUS_DeleteNumbers { get; set; }
        public Int64 CUS_CreatorCode { get; set; }
        public Int16 CUS_Id { get; set; }

        public String? WH_TaxCategory { get; set; }

        public String? WH_TaxType { get; set; }

        public String? GST_Category { get; set; }
        public String? GST_Type { get; set; }

        public String? WH_Number { get; set; }

        public String? WH_TaxCode { get; set; }
        public String? WH_TaxDescription { get; set; }
        public String? TCT_Number { get; set; }
        public String? TCT_Name { get; set; }
        public String? TCT_Description { get; set; }
        public void Reset()
        {
            this.CUS_Number = 0;
            this.CUS_Name = "";
            this.CUS_ContactPerson = "";
            this.CUS_ContactTelephone = "";
            this.CUS_ContactMobile = "";
            this.CUS_ContactEmail = "";
            this.CUS_AccountPerson = "";
            this.CUS_AccountTelephone = "";
            this.CUS_AccountMobile = "";
            this.CUS_AccountEmail = "";
            this.CUS_LOC_Number = 0;
            this.CUS_BYG_Number = 0;
            this.JWC_WH_Number = 0;
            this.CUS_BYC_Number = 0;
            this.CUS_BYS_Number = 0;
            this.CUS_BSS_Number = 0;
            this.CUS_PaymentTerms = "";
            this.CUS_PaymentMode = "";
            this.CUS_CreditDays = 0;
            this.CUS_CreditLimit = 0;
            this.CUS_CUR_Number = 0;
            this.CUS_AccountName = "";
            this.CUS_AccountNumber = "";
            this.CUS_IFSC = "";
            this.CUS_BankName = "";
            this.CUS_DeliveryTerms = "";
            this.CUS_DeliveryMode = "";
            this.CUS_RT_Number = 0;
            this.CUS_GSTIN = "";
            this.CUS_AT_Number = 0;
            this.CUS_PAN = "";
            this.CUS_YN_Number = 0;
            this.CUS_AN_Number = 0;

            this.CUS_WHT_Number = 0;
            this.CUS_WHT_WHTC_Number = 0;
            this.CUS_WHT_WHTT_Number = 0;
            this.CUS_WHT_WHT_Number = 0;
            this.CUS_WHT_FromDate = "";
            this.CUS_WHT_ToDate = "";


            this.CUS_GST_Number = 0;
            this.CUS_GST_GSTC_Number = 0;
            this.CUS_GST_GSTT_Number = 0;
            this.CUS_GST_TCT_Number = 0;
            this.CUS_GST_FromDate = "";
            this.CUS_GST_ToDate = "";

            this.CUS_ADD_Number = 0;
            this.CUS_ADD_ADTP_Number = 0;
            this.CUS_ADD_Address = "";
            this.CUS_ADD_City = "";
            this.CUS_ADD_State = "";
            this.CUS_ADD_Country = "";
            this.CUS_ADD_Pin = "";
            this.CUS_ADD_GSTIN = "";
            this.CUS_ADD_Primary = 0;

            this.CUS_DeleteNumbers = "";
            this.WH_Number = "";
            this.WH_TaxCode = "";
            this.WH_TaxDescription = "";

        }
    }

    public class CustomerList_DTO
    {
        public Int64 CUS_Number { get; set; }
        
        [Display(Name = "Customer Name", Order = 1)]
        public String? CUS_Name { get; set; }

        [Display(Name = "Customer Location")]
        public String? CUS_LOC_Name { get; set; }

        [Display(Name = "Customer Group", Order = 2)]
        public String? CUS_BYG_Name { get; set; }

        [Display(Name = "Customer Category", Order = 3)]
        public String? CUS_BYC_Name { get; set; }

        [Display(Name = "Customer Segment")]
        public String? CUS_BYS_Name { get; set; }

        [Display(Name = "Customer Subsegment")]
        public String? CUS_BSS_Name { get; set; }

        [Display(Name = "Currency", Order = 4)]
        public String? CUS_CUR_Name { get; set; }
        public String? CUS_CUR_Number { get; set; }
    }

    public class CustomerOrderList_DTO
    {
        public Int64 CUS_Number { get; set; }
        public String? CUS_Name { get; set; }
        public String? CUS_LOC_Number { get; set; }
        public String? CUS_CUR_Name { get; set; }
        public String? CUS_CUR_Number { get; set; }
        public String? CUS_CUR_DecimalPlaces { get; set; }
        public Int64 CUS_TCT_Number { get; set; }
        public Int64 CUS_WHT_Number { get; set; }
        public Int64 CUS_WH_Number { get; set; }
    }
    public class CustomerHead_DTO : IValidatableObject
    {
        public Int64 CUS_Number { get; set; }

        [Display(Name = "Customer Name")]
        [MaxLength(100, ErrorMessage = "Customer Name be longer than 100 characters.")]
        [Required(ErrorMessage = "Customer Name is Required")]
        public String? CUS_Name { get; set; }


        [Display(Name = "Contact Person")]
        [MaxLength(50, ErrorMessage = "Contact Person be longer than 50 characters.")]
        public String? CUS_ContactPerson { get; set; }

        [Display(Name = "Telephone")]
        [MaxLength(25, ErrorMessage = "Telephone be longer than 25 characters.")]
        public String? CUS_ContactTelephone { get; set; }

        [Display(Name = "Mobile")]
        [MaxLength(25, ErrorMessage = "Mobile be longer than 25 characters.")]
        public String? CUS_ContactMobile { get; set; }

        [Display(Name = "E-mail")]
        [MaxLength(50, ErrorMessage = "E-mail be longer than 50 characters.")]
        public String? CUS_ContactEmail { get; set; }

        [Display(Name = "Accounts Person")]
        [MaxLength(50, ErrorMessage = "Accounts Person be longer than 50 characters.")]
        public String? CUS_AccountPerson { get; set; }

        [Display(Name = "Telephone")]
        [MaxLength(25, ErrorMessage = "Telephone be longer than 25 characters.")]
        public String? CUS_AccountTelephone { get; set; }

        [Display(Name = "Mobile")]
        [MaxLength(25, ErrorMessage = "Mobile be longer than 25 characters.")]
        public String? CUS_AccountMobile { get; set; }

        [Display(Name = "E-mail")]
        [MaxLength(50, ErrorMessage = "E-mail be longer than 50 characters.")]
        public String? CUS_AccountEmail { get; set; }


        [Display(Name = "Customer Location")]
      //  [Required(ErrorMessage = "Customer Location is Required")]
        public Int64? CUS_LOC_Number { get; set; }

        [Display(Name = "Customer Group")]
        [Required(ErrorMessage = "Customer Group is Required")]
        public Int64? CUS_BYG_Number { get; set; }

        [Display(Name = "Customer Category")]
        [Required(ErrorMessage = "Customer Category is Required")]
        public Int64? CUS_BYC_Number { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public Int64? CUS_WareHouse_Number { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public Int64? CUS_CUR_Number { get; set; }

     

        [Display(Name = "Customer Subsegment")]
       // [Required(ErrorMessage = "Customer Subsegment is Required")]
        public Int64? CUS_BSS_Number { get; set; }

        [Display(Name = "Terms of Payment")]
        [MaxLength(250, ErrorMessage = "Terms of Payment be longer than 250 characters.")]
        public String? CUS_PaymentTerms { get; set; }

        [Display(Name = "Mode of Payment")]
        [MaxLength(50, ErrorMessage = "Mode of Payment be longer than 50 characters.")]
        public String? CUS_PaymentMode { get; set; }

        [Display(Name = "Credit Days")]
        [MaxLength(3, ErrorMessage = "Credit Days be longer than 3 characters.")]
        public String? CUS_CreditDays { get; set; }

        [Display(Name = "Credit Limit")]
        public String? CUS_CreditLimit { get; set; }

      

        [Display(Name = "Account Name")]
        [MaxLength(100, ErrorMessage = "Account Name be longer than 100 characters.")]
        public String? CUS_AccountName { get; set; }

        [Display(Name = "Account Number")]
        [MaxLength(25, ErrorMessage = "Account Number be longer than 25 characters.")]
        public String? CUS_AccountNumber { get; set; }

        [Display(Name = "IFSC")]
        [MaxLength(25, ErrorMessage = "IFSC be longer than 25 characters.")]
        public String? CUS_IFSC { get; set; }

        [Display(Name = "Bank Name")]
        [MaxLength(100, ErrorMessage = "Bank Name be longer than 100 characters.")]
        public String? CUS_BankName { get; set; }

        [Display(Name = "Terms of delivery")]
        [MaxLength(250, ErrorMessage = "Terms of delivery be longer than 250 characters.")]
        public String? CUS_DeliveryTerms { get; set; }

        [Display(Name = "Mode of delivery")]
        [MaxLength(50, ErrorMessage = "Mode of delivery be longer than 50 characters.")]
        public String? CUS_DeliveryMode { get; set; }

        [Display(Name = "Registration Type")]
        public Int64? CUS_RT_Number { get; set; }

        [Display(Name = "GST Number")]
        [MaxLength(15, ErrorMessage = "GST Number be longer than 15 characters.")]
        public String? CUS_GSTIN { get; set; }

        [Display(Name = "Assessee Territory")]
      //  [Required(ErrorMessage = "Assessee Territory is Required")]
        public Int64? CUS_AT_Number { get; set; }
        public String? CUS_AT_Name { get; set; }

        [Display(Name = "PAN / IT No.")]
        [MaxLength(10, ErrorMessage = "PAN / IT No. be longer than 10 characters.")]
        public String? CUS_PAN { get; set; }

        [Display(Name = "Calculate withholding tax")]
        public Int64? CUS_YN_Number { get; set; }

        [Display(Name = "Nature of assessee")]
        public Int64? CUS_AN_Number { get; set; }

        public PaginatedList_DTO<CustomerList_DTO>? Customer_List { get; set; }
        public List<CustomerWHT_DTO>? CUS_WHT_List { get; set; }
        public List<CustomerGST_DTO>? CUS_GST_List { get; set; }
        public List<CustomerAdd_DTO>? CUS_Add_List { get; set; }
        public List<CustomerContact_DTO>? CUS_Contact_List { get; set; }
        public void Reset()
        {
            this.CUS_Number = 0;
            this.CUS_Name = "";
            this.CUS_ContactPerson = "";
            this.CUS_ContactTelephone = "";
            this.CUS_ContactMobile = "";
            this.CUS_ContactEmail = "";
            this.CUS_AccountPerson = "";
            this.CUS_AccountTelephone = "";
            this.CUS_AccountMobile = "";
            this.CUS_AccountEmail = "";
            this.CUS_LOC_Number = 0;
            this.CUS_BYG_Number = 0;
            this.CUS_BYC_Number = 0;
            this.CUS_WareHouse_Number = 0;
            this.CUS_BSS_Number = 0;
            this.CUS_PaymentTerms = "";
            this.CUS_PaymentMode = "";
            this.CUS_CreditDays = "";
            this.CUS_CreditLimit = "";
            this.CUS_CUR_Number = 0;
            this.CUS_AccountName = "";
            this.CUS_AccountNumber = "";
            this.CUS_IFSC = "";
            this.CUS_BankName = "";
            this.CUS_DeliveryTerms = "";
            this.CUS_DeliveryMode = "";
            this.CUS_RT_Number = 0;
            this.CUS_GSTIN = "";
            this.CUS_AT_Number = 0;
            this.CUS_PAN = "";
            this.CUS_YN_Number = 0;
            this.CUS_AN_Number = 0;

            this.Customer_List = null;
            this.CUS_WHT_List = null;
            this.CUS_GST_List = null;
            this.CUS_Add_List = null;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool hasRT = CUS_RT_Number != null && CUS_RT_Number != 0;
            bool hasGST = !string.IsNullOrWhiteSpace(CUS_GSTIN);
            bool hasAT = CUS_AT_Number != null && CUS_AT_Number != 0;

            int filledCount = (hasRT ? 1 : 0) + (hasGST ? 1 : 0) + (hasAT ? 1 : 0);

            if (filledCount > 0 && filledCount < 3)
            {
                if (!hasRT)
                    yield return new ValidationResult("Registration Type is required.", new[] { nameof(CUS_RT_Number) });

                if (!hasGST)
                    yield return new ValidationResult("GST Number is required.", new[] { nameof(CUS_GSTIN) });

                if (!hasAT)
                    yield return new ValidationResult("Assessee Territory is required.", new[] { nameof(CUS_AT_Number) });
            }
        }

    }
    public class CustomerWHT_DTO
    {
        public Int64 CUS_WHT_Number { get; set; }

        [Display(Name = "WHT Category")]
        [Required(ErrorMessage = "WHT Category is Required")]
        public String? CUS_WHT_WHTC_Number { get; set; }

        [Display(Name = "WHT Type")]
        [Required(ErrorMessage = "WHT Type is Required")]
        public String? CUS_WHT_WHTT_Number { get; set; }

        [Display(Name = "WH Tax Code")]
        [Required(ErrorMessage = "WH Tax Code is Required")]
        public String? CUS_WHT_WHT_Number { get; set; }

        [Display(Name = "Description")]
        public String? CUS_WHT_WHT_Description { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is Required")]
        public String? CUS_WHT_FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is Required")]
        public String? CUS_WHT_ToDate { get; set; }

        public Int16 CUS_WHT_IsDeleted { get; set; }

        public void Reset()
        {
            this.CUS_WHT_Number = 0;
            this.CUS_WHT_WHTC_Number = "0";
            this.CUS_WHT_WHTT_Number = "0";
            this.CUS_WHT_WHT_Number = "0";
            this.CUS_WHT_WHT_Description = "";
            this.CUS_WHT_FromDate = "";
            this.CUS_WHT_ToDate = "";
            this.CUS_WHT_IsDeleted = 0;
        }
    }
    public class CustomerGST_DTO
    {
        public Int64 CUS_GST_Number { get; set; }

        [Display(Name = "GST Category")]
        [Required(ErrorMessage = "GST Category is Required")]
        public String? CUS_GST_GSTC_Number { get; set; }
        public String? CUS_GST_GSTC_Name { get; set; }

        [Display(Name = "GST Type")]
        [Required(ErrorMessage = "GST Type is Required")]
        public String? CUS_GST_GSTT_Number { get; set; }

        [Display(Name = "GST Tax Cluster")]
        [Required(ErrorMessage = "GST Tax Cluster is Required")]
        public String? CUS_GST_TCT_Number { get; set; }

        [Display(Name = "GST Tax Cluster")]
        public String? CUS_GST_TCT_Name { get; set; }

        [Display(Name = "Description")]
        public String? CUS_GST_TCT_Description { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is Required")]
        public String? CUS_GST_FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is Required")]
        public String? CUS_GST_ToDate { get; set; }

        public Int16 CUS_GST_IsDeleted { get; set; }

        public void Reset()
        {
            this.CUS_GST_Number = 0;
            this.CUS_GST_GSTC_Number = "0";
            this.CUS_GST_GSTT_Number = "0";
            this.CUS_GST_TCT_Number = "0";
            this.CUS_GST_TCT_Description = "";
            this.CUS_GST_FromDate = "";
            this.CUS_GST_ToDate = "";
            this.CUS_GST_IsDeleted = 0;
        }
    }
    public class CustomerAdd_DTO
    {
        [Key]
        public Int64 CUS_ADD_Number { get; set; }

        [Display(Name = "Address Type")]
      //  [Required(ErrorMessage = "Address Type is Required")]
        public Int64 CUS_ADD_ADTP_Number { get; set; }

        [Display(Name = "Address ID")]
        //[Required(ErrorMessage = "Address ID is Required")]
        public String? CUS_ADD_AddressID { get; set; }

        [Display(Name = "Address")]
        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
       // [Required(ErrorMessage = "Address is Required")]
        public String? CUS_ADD_Address { get; set; }

        [Display(Name = "City")]
        [StringLength(25, ErrorMessage = "City cannot exceed 25 characters")]
       // [Required(ErrorMessage = "City is Required")]
        public String? CUS_ADD_City { get; set; }

        [Display(Name = "State")]
        [StringLength(25, ErrorMessage = "State cannot exceed 25 characters")]
      //  [Required(ErrorMessage = "State is Required")]
        public String? CUS_ADD_State { get; set; }

        [Display(Name = "Country")]
        [StringLength(25, ErrorMessage = "Country cannot exceed 25 characters")]
      //  [Required(ErrorMessage = "Country is Required")]
        public String? CUS_ADD_Country { get; set; }

        [Display(Name = "PIN Code")]
        [StringLength(10, ErrorMessage = "PIN Code cannot exceed 10 characters")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "PIN Code must be 6 digits")]
      //  [Required(ErrorMessage = "PIN Code is Required")]
        public String? CUS_ADD_Pin { get; set; }

        [Display(Name = "GSTIN")]
        [StringLength(25, ErrorMessage = "GSTIN cannot exceed 25 characters")]
        public String? CUS_ADD_GSTIN { get; set; }

        [Display(Name = "Default")]
        public Boolean CUS_ADD_Primary { get; set; }

        public Int16 CUS_ADD_IsDeleted { get; set; }

        public void Reset()
        {
            this.CUS_ADD_Number = 0;
            this.CUS_ADD_ADTP_Number = 0;
            this.CUS_ADD_Address = "";
            this.CUS_ADD_City = "";
            this.CUS_ADD_State = "";
            this.CUS_ADD_Country = "";
            this.CUS_ADD_Pin = "";
            this.CUS_ADD_GSTIN = "";
            this.CUS_ADD_Primary = false;
            this.CUS_ADD_IsDeleted = 0;
        }
   

    }

    public class CustomerContact_DTO
    {
        [Key]
        public Int64 CUS_CNT_Number { get; set; }

        [Display(Name = "Contact Name")]
        [StringLength(25, ErrorMessage = "Contact Name cannot exceed 25 characters")]
        public string? CUS_CNT_ContactName { get; set; }

        [Display(Name = "Department")]
        [StringLength(25, ErrorMessage = "Department cannot exceed 25 characters")]
        public string? CUS_CNT_Department { get; set; }

        [Display(Name = "Mobile")]
        [StringLength(50, ErrorMessage = "Mobile cannot exceed 50 characters")]
        public string? CUS_CNT_Mobile { get; set; }

        [Display(Name = "Telephone")]
        [StringLength(50, ErrorMessage = "Telephone cannot exceed 50 characters")]
        public string? CUS_CNT_Telephone { get; set; }

        [Display(Name = "Email")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? CUS_CNT_Email { get; set; }

        public Int16 CUS_CNT_IsDeleted { get; set; }
        public Int16 CUS_Id { get; set; }

        public void Reset()
        {
            this.CUS_CNT_Number = 0;
            this.CUS_CNT_ContactName = "";
            this.CUS_CNT_Department = "";
            this.CUS_CNT_Mobile = "";
            this.CUS_CNT_Telephone = "";
            this.CUS_CNT_Email = "";
            this.CUS_CNT_IsDeleted = 0;
        }
    }

}

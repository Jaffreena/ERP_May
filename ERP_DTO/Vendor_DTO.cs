using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class Vendor_DTO
    {
        public Int64 VendorNumber { get; set; }
        public String? VendorCode { get; set; }
        public String? VendorName { get; set; }
        public String? Address { get; set; }
        public String? City { get; set; }
        public String? State { get; set; }
        public String? Country { get; set; }
        public String? Pincode { get; set; }
        public String? ContactPerson { get; set; }
        public String? CP_Telephone { get; set; }
        public String? CP_Mobile { get; set; }
        public String? CP_Email { get; set; }
        public String? AccountsPerson { get; set; }
        public String? AP_Telephone { get; set; }
        public String? AP_Mobile { get; set; }
        public String? AP_Email { get; set; }
        public String? VendorLocation { get; set; }
        public String? VendorGroup { get; set; }
        public String? VendorCategory { get; set; }
        public Int64 RegistrationType { get; set; }
        public String? GSTIN { get; set; }
        public Int64 AssesseeTerritory { get; set; } 
        public Int32 TransportAgency { get; set; }
        public String? TransporterID { get; set; }
        public String? PaymentTerms { get; set; }
        public String? PaymentMode { get; set; }
        public Int64 CreditDays { get; set; }
        public Int64 PaymentBase { get; set; }
        public String? Currency { get; set; }
        public String? AccountName { get; set; }
        public String? AccountNumber { get; set; }
        public String? IFSC { get; set; }
        public String? BankName { get; set; }
        public String? PAN { get; set; }
        public Int64 WithholdTax { get; set; }
        public Int64 AssesseeNature { get; set; }

        public Int64 TaxNumber { get; set; }
        public Int64 Category { get; set; }
        public Int64 Type { get; set; }
        public Int64 Tax { get; set; }
        public Int32 FromDate { get; set; }
        public Int32 ToDate { get; set; }
        public String? Cluster { get; set; }


        public String? CurrencyCode { get; set; }
        public String? DecimalPlaces { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.VendorNumber = 0;
            this.VendorCode = "";
            this.VendorName = "";
            this.Address = "";
            this.City = "";
            this.State = "";
            this.Country = "";
            this.Pincode = "";
            this.ContactPerson = "";
            this.CP_Telephone = "";
            this.CP_Mobile = "";
            this.CP_Email = "";
            this.AccountsPerson = "";
            this.AP_Telephone = "";
            this.AP_Mobile = "";
            this.AP_Email = "";
            this.VendorLocation = "0";
            this.VendorGroup = "0";
            this.VendorCategory = "0";
            this.RegistrationType = 0;
            this.GSTIN = "";
            this.AssesseeTerritory = 0;
            this.TransportAgency = 0;
            this.TransporterID = "";
            this.PaymentTerms = "";
            this.PaymentMode = "";
            this.CreditDays = 0;
            this.PaymentBase = 0;
            this.Currency = "0";
            this.AccountName = "";
            this.AccountNumber = "";
            this.IFSC = "";
            this.BankName = "";
            this.PAN = "";
            this.WithholdTax = 0;
            this.AssesseeNature = 0;

            this.TaxNumber = 0;
            this.Category = 0;
            this.Type = 0;
            this.Tax = 0;
            this.FromDate = 0;
            this.ToDate = 0;

            this.DeleteNumbers = "";
        }
    }
    public class VendorHead_DTO
    {
        public Int64 VendorNumber { get; set; }

        [Display(Name = "Vendor Code")]
        [MaxLength(10, ErrorMessage = "Vendor Code be longer than 10 characters.")]
        [Required(ErrorMessage = "Vendor Code is Required")]
        public String? VendorCode { get; set; }

        [Display(Name = "Vendor Name")]
        [MaxLength(100, ErrorMessage = "Vendor Name be longer than 100 characters.")]
        [Required(ErrorMessage = "Vendor Name is Required")]
        public String? VendorName { get; set; }

        [Display(Name = "Address")]
        [MaxLength(250, ErrorMessage = "Address be longer than 250 characters.")]
        public String? Address { get; set; }

        [Display(Name = "City")]
        [MaxLength(25, ErrorMessage = "City be longer than 25 characters.")]
        public String? City { get; set; }

        [Display(Name = "State")]
        [MaxLength(25, ErrorMessage = "State be longer than 25 characters.")]
        public String? State { get; set; }

        [Display(Name = "Country")]
        [MaxLength(25, ErrorMessage = "Country be longer than 25 characters.")]
        public String? Country { get; set; }

        [Display(Name = "Pincode")]
        [MaxLength(10, ErrorMessage = "Pincode be longer than 10 characters.")]
        public String? Pincode { get; set; }

        [Display(Name = "Contact Person")]
        [MaxLength(50, ErrorMessage = "Contact Person be longer than 50 characters.")]
        public String? ContactPerson { get; set; }

        [Display(Name = "Telephone")]
        [MaxLength(25, ErrorMessage = "Contact Person Telephone be longer than 25 characters.")]
        public String? CP_Telephone { get; set; }

        [Display(Name = "Mobile")]
        [MaxLength(25, ErrorMessage = "Contact Person Mobile be longer than 25 characters.")]
        public String? CP_Mobile { get; set; }

        [Display(Name = "E-mail")]
        [MaxLength(50, ErrorMessage = "Contact Person E-mail be longer than 50 characters.")]
        public String? CP_Email { get; set; }

        [Display(Name = "Accounts Person")]
        [MaxLength(50, ErrorMessage = "Accounts Person be longer than 50 characters.")]
        public String? AccountsPerson { get; set; }

        [Display(Name = "Telephone")]
        [MaxLength(25, ErrorMessage = "Accounts Person Telephone be longer than 25 characters.")]
        public String? AP_Telephone { get; set; }

        [Display(Name = "Mobile")]
        [MaxLength(25, ErrorMessage = "Accounts Person Mobile be longer than 25 characters.")]
        public String? AP_Mobile { get; set; }

        [Display(Name = "E-mail")]
        [MaxLength(50, ErrorMessage = "Accounts Person E-mail be longer than 50 characters.")]
        public String? AP_Email { get; set; }


        [Display(Name = "Vendor Location")]
        [Required(ErrorMessage = "Vendor Location is Required")]
        public String? VendorLocation { get; set; }

        [Display(Name = "Vendor Group")]
        [Required(ErrorMessage = "Vendor Group is Required")]
        public String? VendorGroup { get; set; }

        [Display(Name = "Vendor Category")]
        [Required(ErrorMessage = "Vendor Category is Required")]
        public String? VendorCategory { get; set; }

        [Display(Name = "Registration Type")]
        public String? RegistrationType { get; set; }

        [Display(Name = "GST Number")]
        public String? GSTIN { get; set; }

        [Display(Name = "Assessee Territory")]
        [Required(ErrorMessage = "Assessee Territory is Required")]
        public String? AssesseeTerritory { get; set; }
        public String? AssesseeTerritoryName { get; set; }

        [Display(Name = "Transport Agency")]
        public String? TransportAgency { get; set; }

        [Display(Name = "Transporter ID")]
        [MaxLength(15, ErrorMessage = "Transporter ID be longer than 15 characters.")]
        public String? TransporterID { get; set; }


        [Display(Name = "Payment Terms")]
        [MaxLength(250, ErrorMessage = "Payment Terms be longer than 250 characters.")]
        public String? PaymentTerms { get; set; }

        [Display(Name = "Payment Mode")]
        [MaxLength(50, ErrorMessage = "Payment Mode be longer than 50 characters.")]
        public String? PaymentMode { get; set; }

        [Display(Name = "Credit Days")]
        [MaxLength(3, ErrorMessage = "Credit Days be longer than 3 characters.")]
        public String? CreditDays { get; set; }

        [Display(Name = "Payment Base")]
        public String? PaymentBase { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public String? Currency { get; set; }


        [Display(Name = "Account Name")]
        [MaxLength(100, ErrorMessage = "Account Name be longer than 100 characters.")]
        public String? AccountName { get; set; }

        [Display(Name = "Account Number")]
        [MaxLength(25, ErrorMessage = "Account Number be longer than 25 characters.")]
        public String? AccountNumber { get; set; }

        [Display(Name = "IFSC")]
        [MaxLength(25, ErrorMessage = "IFSC be longer than 25 characters.")]
        public String? IFSC { get; set; }

        [Display(Name = "Bank Name")]
        [MaxLength(100, ErrorMessage = "Bank Name be longer than 100 characters.")]
        public String? BankName { get; set; }

        [Display(Name = "PAN")]
        [MaxLength(10, ErrorMessage = "PAN be longer than 10 characters.")]
        public String? PAN { get; set; }

        [Display(Name = "Calculate Withhold Tax")]
        public String? WithholdTax { get; set; }

        [Display(Name = "Assessee Nature")]
        public String? AssesseeNature { get; set; }

        public PaginatedList_DTO<Vendor_DTO>? Vendor_List { get; set; }
        public List<VendorWHT_DTO>? WHT_List { get; set; }
        public List<VendorGST_DTO>? GST_List { get; set; }
        public void Reset()
        {
            this.VendorNumber = 0;
            this.VendorCode = "";
            this.VendorName = "";
            this.Address = "";
            this.City = "";
            this.State = "";
            this.Country = "";
            this.Pincode = "";
            this.ContactPerson = "";
            this.CP_Telephone = "";
            this.CP_Mobile = "";
            this.CP_Email = "";
            this.AccountsPerson = "";
            this.AP_Telephone = "";
            this.AP_Mobile = "";
            this.AP_Email = "";
            this.VendorLocation = "";
            this.VendorGroup = "";
            this.VendorCategory = "";
            this.RegistrationType = "";
            this.GSTIN = "";
            this.AssesseeTerritory = "";
            this.TransportAgency = "";
            this.TransporterID = "";
            this.PaymentTerms = "";
            this.PaymentMode = "";
            this.CreditDays = "";
            this.PaymentBase = "";
            this.Currency = "";
            this.AccountName = "";
            this.AccountNumber = "";
            this.IFSC = "";
            this.BankName = "";
            this.PAN = "";
            this.WithholdTax = "";
            this.AssesseeNature = "";

            this.Vendor_List = null;
            this.WHT_List = null;
            this.GST_List = null;
        }

    }
    public class VendorWHT_DTO
    {
        public Int64 Number { get; set; }

        [Display(Name = "WHT Category")]
        [Required(ErrorMessage = "WHT Category is Required")]
        public String? Category { get; set; }

        [Display(Name = "WHT Type")]
        [Required(ErrorMessage = "WHT Type is Required")]
        public String? Type { get; set; }

        [Display(Name = "Tax Code")]
        [Required(ErrorMessage = "Tax Code is Required")]
        public String? Tax { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is Required")]
        public String? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is Required")]
        public String? ToDate { get; set; }

        public Boolean IsDeleted { get; set; }

        public void Reset()
        {
            this.Number = 0;
            this.Category = "0";
            this.Type = "0";
            this.Tax = "0";
            this.FromDate = "";
            this.ToDate = "";
            this.IsDeleted = false;
        }
    }
    public class VendorGST_DTO
    {
        public Int64 Number { get; set; }

        [Display(Name = "GST Category")]
        [Required(ErrorMessage = "GST Category is Required")]
        public String? Category { get; set; }
        public String? CategoryName { get; set; }

        [Display(Name = "GST Type")]
        [Required(ErrorMessage = "GST Type is Required")]
        public String? Type { get; set; }

        [Display(Name = "Tax Code")]
        [Required(ErrorMessage = "Tax Code is Required")]
        public String? TaxNumber { get; set; }

        [Display(Name = "Tax Code")]
        [Required(ErrorMessage = "Tax Code is Required")]
        public String? Tax { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is Required")]
        public String? FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is Required")]
        public String? ToDate { get; set; }

        public Boolean IsDeleted { get; set; }

        public void Reset()
        {
            this.Number = 0;
            this.Category = "0";
            this.Type = "0";
            this.Tax = "0";
            this.FromDate = "";
            this.ToDate = "";
            this.IsDeleted = false;
        }
    }
}

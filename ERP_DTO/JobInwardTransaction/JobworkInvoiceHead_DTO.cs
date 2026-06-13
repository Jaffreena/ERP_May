using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO.JobInwardTransaction
{
    public class JobworkInvoiceHead_DTO
    {
        public long JISVIH_Number { get; set; }

        [Display(Name = "Invoice No.")]
        [StringLength(25)]
        public string JISVIH_InvoiceNo { get; set; }

        [Display(Name = "Invoice Date")]
        [DataType(DataType.Date)]
        public DateTime JISVIH_InvoiceDate { get; set; }

        [Display(Name = "JW Customer")]
        public long JISVIH_JW_Customer_Number { get; set; }
        public string JISVIH_JW_Customer_Name { get; set; }
        

        [Display(Name = "Currency")]
        public long JISVIH_Currency_Number { get; set; }

        [Display(Name = "Tax Template")]
        public long JISVIH_TCT_Number { get; set; }

        [Display(Name = "Payment Terms")]
        [StringLength(50)]
        public string JISVIH_PaymentTerms { get; set; }

        [Display(Name = "Payment Method")]
        [StringLength(50)]
        public string JISVIH_PaymentMethod { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(250)]
        public string JISVIH_Remarks { get; set; }
        public int? JW_Inv_Id { get; set; }
    }
    public class JobworkInvoiceItem_DTO
    {
        public long JISVII_JISVIH_Number { get; set; }

        public long JISVII_Number { get; set; }

        [Display(Name = "JW Order")]
        public long JISVII_JISVOH_Number { get; set; }

        [Display(Name = "Delivery Note")]
        public long JISVII_JIDNH_Number { get; set; }

        [Display(Name = "PRS Number")]
        public long JISVII_PRS_Number { get; set; }

        [Display(Name = "Item")]
        public long JISVII_Item_Number { get; set; }

        [Display(Name = "UOM")]
        public long JISVII_UoM_Number { get; set; }

        [Display(Name = "Qty")]
        public double JISVII_Qty { get; set; }

        [Display(Name = "Unit Price")]
        public double JISVII_UnitPrice { get; set; }

        [Display(Name = "Amount")]
        public double JISVII_Amount { get; set; }

        [Display(Name = "SAC")]
        public long JISVII_SAC_Number { get; set; }

        [Display(Name = "GST Amount")]
        public double JISVII_GST_Amount { get; set; }
    }

    public class JobworkInvoiceAddress_DTO
    {
        public long JISVIA_JISVIH_Number { get; set; }

        public long JISVIA_Number { get; set; }

        [Display(Name = "Address Type")]
        public long JISVIA_ADTP_Number { get; set; }

        [Display(Name = "Address ID")]
        [StringLength(25)]
        public string JISVIA_Address_ID { get; set; }

        [Display(Name = "Address")]
        [StringLength(250)]
        public string JISVIA_Address { get; set; }

        [Display(Name = "City")]
        [StringLength(25)]
        public string JISVIA_City { get; set; }

        [Display(Name = "State")]
        [StringLength(25)]
        public string JISVIA_State { get; set; }

        [Display(Name = "Country")]
        [StringLength(25)]
        public string JISVIA_Country { get; set; }

        [Display(Name = "PIN")]
        [StringLength(10)]
        public string JISVIA_PIN { get; set; }

        [Display(Name = "GSTIN")]
        [StringLength(15)]
        public string JISVIA_GSTIN { get; set; }
    }

    public class JobworkInvoiceGST_DTO
    {
        public long JISVIG_JISVIH_Number { get; set; }

        public long JISVIG_JISVII_Number { get; set; }

        public long JISVIG_Number { get; set; }

        [Display(Name = "Index")]
        public int JISVIG_Index { get; set; }

        [Display(Name = "GST Category")]
        public long JISVIG_GSTC_Number { get; set; }

        [Display(Name = "GST Type")]
        public long JISVIG_GSTT_Number { get; set; }

        [Display(Name = "GST Element")]
        public long JISVIG_GSTE_Number { get; set; }

        [Display(Name = "Assessable Value")]
        public double JISVIG_AssessableValue { get; set; }

        [Display(Name = "Percent")]
        public double JISVIG_Percent { get; set; }

        [Display(Name = "GST Amount")]
        public double JISVIG_GST_Amount { get; set; }
    }
    public class JobworkInvoiceCreate_DTO
    {
        public JobworkInvoiceCreate_DTO()
        {
            Header = new JobworkInvoiceHead_DTO();

            Items = new List<JobworkInvoiceItem_DTO>();

            Addresses = new List<JobworkInvoiceAddress_DTO>();

            GST = new List<JobworkInvoiceGST_DTO>();
        }

        public JobworkInvoiceHead_DTO Header { get; set; }

        public List<JobworkInvoiceItem_DTO> Items { get; set; }

        public List<JobworkInvoiceAddress_DTO> Addresses { get; set; }

        public List<JobworkInvoiceGST_DTO> GST { get; set; }
    }

}

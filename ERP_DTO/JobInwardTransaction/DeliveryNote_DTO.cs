using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO.JobInwardTransaction
{
    public class DeletedRowInfo_DTO
    {
        public int ItemGridindex { get; set; }

        public long JIDNI_Number { get; set; }

        public long JIDNH_Number { get; set; }

        public long DBCH_Item_Number { get; set; }

        public long DBCH_DBCH_Number { get; set; }
    }
    public class DeliveryNoteCreate_DTO
    {
        public DeliveryNoteHeader_DTO Header { get; set; }

        public List<DeliveryNoteItem_DTO> Items { get; set; }

        public List<DeliveryNoteBatch_DTO> deliveryNoteBatches { get; set; }

        public List<DeliveryNoteAddress_DTO> Addresses { get; set; }

        public DeliveryNoteCreate_DTO()
        {
            Header = new DeliveryNoteHeader_DTO();

            Items = new List<DeliveryNoteItem_DTO>();

            Addresses = new List<DeliveryNoteAddress_DTO>();
            deliveryNoteBatches = new List<DeliveryNoteBatch_DTO>();
        }

    }
    public class DeliveryNoteHeader_DTO
    {
        public long JIDNH_Number { get; set; }

        [Display(Name = "Delivery Note No.")]
        [StringLength(25)]
        public string JIDNH_DN_No { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime JIDNH_DN_Date { get; set; }

        [Display(Name = "Material Segregation")]
        public long JIDNH_MS_Number { get; set; }

        [Display(Name = "JW Customer")]
        public long JIDNH_JW_Customer_Number { get; set; }
        public string? JIDNI_Item_Code { get; set; } = null;
        public string? JIDNH_JW_Customer_Name { get; set; }

        [Display(Name = "Currency")]
        public long JIDNH_Currency_Number { get; set; }

        [Display(Name = "Warehouse")]
        public long JIDNH_WH_Number { get; set; }

        [Display(Name = "Terms of Payment")]
        [StringLength(50)]
        public string? JIDNH_PaymentTerms { get; set; }

        [Display(Name = "Terms of Delivery")]
        [StringLength(50)]
        public string? JIDNH_DeliveryTerms { get; set; }

        [Display(Name = "Mode of Delivery")]
        [StringLength(50)]
        public string? JIDNH_DeliveryMode { get; set; }

        [Display(Name = "Despatch Document No.")]
        [StringLength(50)]
        public string? JIDNH_DespatchDocumentNo { get; set; }

        [Display(Name = "Despatched Through")]
        [StringLength(50)]
        public string? JIDNH_DespatchedThrough { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(250)]
        public string? JIDNH_Remarks { get; set; }

        public int? DN_Id { get; set; }
        public int? DN_CUS_Number { get; set; }
        public int? DN_ADD_ADTP_Number { get; set; }
        public int? DN_CreatorCode { get; set; }

        public string? JIDNH_Warehouse { get; set; }
        public string? JIDNH_CustomerName { get; set; }
        public string? JIDNH_CurrencyCode { get; set; }
        public string? JIDNH_Segregation { get; set; }

        

    }

    public class DeliveryNoteItem_DTO
    {
        public long JIDNI_JIDNH_Number { get; set; }

        public long JIDNI_Number { get; set; }

        [Display(Name = "PRS Number")]
        public long JIDNI_PRS_Number { get; set; }

        [Display(Name = "Item")]
        public long JIDNI_Item_Number { get; set; }

        [Display(Name = "Warehouse")]
        public long JIDNI_WH_Number { get; set; }

        [Display(Name = "UoM")]
        public long JIDNI_UoM_Number { get; set; }

        [Display(Name = "Quantity")]
        public double JIDNI_Qty { get; set; }

        [Display(Name = "Unit Price")]
        public double JIDNI_UnitPrice { get; set; }

        [Display(Name = "Amount")]
        public double JIDNI_Amount { get; set; }

        [Display(Name = "JW Invoice Tracking")]
        [StringLength(3)]
        public string JIDNI_JW_InvoiceTracking { get; set; }

        public string? JIDNI_JW_ProcessName { get; set; }
        public string? JIDNI_JW_ItemName { get; set; }
        public string? JIDNI_JW_ItemDescription { get; set; }

        public string? JIDNI_JW_OuterDia { get; set; }

        public string? JIDNI_JW_Thickness { get; set; }

        public string? JIDNI_JW_Length { get; set; }

        public string? JIDNI_JW_ITM_Width { get; set; }

        public string? JIDNI_JW_MaterialGrade { get; set; }

        public string? JIDNI_UOM { get; set; }
        public string? JIDNI_Warehouse { get; set; }

        public string? JIDNI_Item_Code { get; set; }
        public string? JIDNI_Width { get; set; }
        public string? JIDNI_Item_Description { get; set; }
        public string? JIDNI_OuterDia { get; set; }
        public string? JIDNI_Thickness { get; set; }
        public string? JIDNI_Length { get; set; }
        public string? JIDNI_MaterialGrade { get; set; }
        public string? JIDNI_ItemGroup { get; set; }
        public string? JIDNI_IsDeleted { get; set; }
        public long? CustomerNumber { get; set; }
        public long? JISVOH_Number { get; set; }
        public long? JISVOI_Number { get; set; }
       



    }

    public class DeliveryNoteAddress_DTO
    {
        public long JIDNA_JIDNH_Number { get; set; }

        public long JIDNA_Number { get; set; }

        [Display(Name = "Address Type")]
        public long JIDNA_ADTP_Number { get; set; }

        [Display(Name = "Address ID")]
        [StringLength(25)]
        public string JIDNA_Address_ID { get; set; }

        [Display(Name = "Address")]
        [StringLength(250)]
        public string? JIDNA_Address { get; set; }

        [Display(Name = "City")]
        [StringLength(25)]
        public string? JIDNA_City { get; set; }

        [Display(Name = "State")]
        [StringLength(25)]
        public string? JIDNA_State { get; set; }

        [Display(Name = "Country")]
        [StringLength(25)]
        public string? JIDNA_Country { get; set; }

        [Display(Name = "PIN")]
        [StringLength(10)]
        public string? JIDNA_PIN { get; set; }

        [Display(Name = "GSTIN")]
        [StringLength(15)]
        public string? JIDNA_GSTIN { get; set; }
    }

    public class DeliveryNoteSummary_DTO
    {
        public long JIDNH_Number { get; set; }

        public int DN_Id { get; set; }

        public int DN_CreatorCode { get; set; }
        public string? JIDNH_DN_No { get; set; }

        public DateTime JIDNH_DN_Date { get; set; }

        public long JIDNH_JW_Customer_Number { get; set; }

        public long CUS_JCG_Number { get; set; }

        public string? JCG_JW_CustomerGroup { get; set; }

        public string? JCC_JW_CustomerCategory { get; set; }

        public string? CUS_Name { get; set; }

        public string? CurrencyCode { get; set; }

        public string? WarehouseCode { get; set; }

        public long JIDNH_MS_Number { get; set; }

        public string? Segregation { get; set; }

        public int NoOfLineItems { get; set; }

        public string Qty { get; set; }

        public double Amount { get; set; }

        public string? CustomerWareHouse { get; set; }

        public string? ItemWareHouse { get; set; }
    }

    public class DeliveryNoteDetailed_DTO
    {
        public long JIDNH_Number { get; set; }
        public string JIDNH_DN_No { get; set; }
        public DateTime JIDNH_DN_Date { get; set; }
        public long JIDNH_JW_Customer_Number { get; set; }

        public long CUS_JCG_Number { get; set; }
        public string JCG_JW_CustomerGroup { get; set; }
        public string JCC_JW_CustomerCategory { get; set; }
        public string CUS_Name { get; set; }

        public string CurrencyCode { get; set; }
        public string WarehouseCode { get; set; }

        public long JIDNH_MS_Number { get; set; }

        public string CustomerWareHouse { get; set; }
        public string ItemWareHouse { get; set; }
        public string Segregation { get; set; }

        public long JIDNI_PRS_Number { get; set; }
        public long JIDNI_Item_Number { get; set; }

        public string Process { get; set; }
        public string ItemGroup { get; set; }

        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }

        public decimal OuterDia { get; set; }
        public decimal Thickness { get; set; }
        public decimal ItemLength { get; set; }
        public decimal? ITM_Width { get; set; }

        public string MaterialGrade { get; set; }
        public string Warehouse { get; set; }
        public string UOM { get; set; }

        public decimal JIDNI_Qty { get; set; }
        public decimal JIDNI_Amount { get; set; }

        public string JIDNI_JW_InvoiceTracking { get; set; }
    }
    public class OutCommonBatch_DTO
    {
        public string TransType { get; set; }
        public long Header_Number { get; set; }
        public long LineItem_Number { get; set; }
        public long LineBatch_Number { get; set; }
        public long Warehouse { get; set; }
        public DateTime BatchDate { get; set; }
        public string BatchNo { get; set; }
        public string ItemStatus { get; set; }
        public decimal BatchQty { get; set; }
        public decimal BatchUnitPrice { get; set; }
        public decimal BatchValue { get; set; }
    }
    public class DeliveryNoteBatch_DTO
    {
        public long JIDNI_BCH_Number { get; set; }

        public long JIDNI_BCH_JIDNH_Number { get; set; }

        public long JIDNI_BCH_JIDNI_Number { get; set; }

        public long JIDNI_BCH_WH_Number { get; set; }

        public DateTime JIDNI_BCH_BatchDate { get; set; }

        public string JIDNI_BCH_BatchNo { get; set; }

        

        public decimal JIDNI_BCH_BatchQty { get; set; }

        public decimal JIDNI_BCH_BatchUnitPrice { get; set; }

        public decimal JIDNI_BCH_BatchValue { get; set; }

        public decimal RefBatch_Number { get; set; }
        public decimal JIDNH_Number { get; set; }
        public decimal JIDNI_Number { get; set; }
    }

    public class TempDeliveryBatch_DTO
    {
        public string? DBCH_RowGuid { get; set; }
        public long DBCH_Number { get; set; }

        public int DBCH_Index { get; set; }

        public long? DBCH_DBCH_Number { get; set; }

        public long DBCH_Item_Number { get; set; }

        public long? JIDNI_NUMBER { get; set; }
        public long? JIDNH_NUMBER { get; set; }
        public long? RefBatch_Number { get; set; }

        public long DBCH_Warehouse_Number { get; set; }

        public DateTime DBCH_Date { get; set; }

        public string DBCH_No { get; set; }

        public decimal DBCH_Qty { get; set; }

        public decimal? DBCH_UnitPrice { get; set; }

        public decimal? DBCH_Value { get; set; }

        public int? Mode { get; set; }

        public int CreatorCode { get; set; }

        public DateTime CreatorDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO.JobInwardTransaction
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class ServiceOrderDetailed_DTO
    {
        public long JISVOH_Number { get; set; }

        public int SO_Id { get; set; }

        public int SO_CreatorCode { get; set; }

        // SVO Register
        public string? JISVOH_RegNo { get; set; }
        public DateTime JISVOH_RegDate { get; set; }

        // Service Order
        public string? JISVOH_ServiceOrderNo { get; set; }
        public DateTime JISVOH_ServiceOrderDate { get; set; }

        // Customer
        public long JISVOH_JW_Customer_Number { get; set; }
        public long CUS_JCG_Number { get; set; }
        public string? JCG_JW_CustomerGroup { get; set; }
        public string? JCC_JW_CustomerCategory { get; set; }
        public string? CUS_Name { get; set; }
        public string? CurrencyCode { get; set; }

        // Item Details
        public long JISVOI_PRS_Number { get; set; }
        public long JISVOI_Item_Number { get; set; }

        public string? PRS_ProcessName { get; set; }
        public string? Segregation { get; set; }
        public string? ItemGroup { get; set; }

        public string? ItemCode { get; set; }
        public string? ItemDescription { get; set; }

        public string? OuterDia { get; set; }
        public string? Thickness { get; set; }
        public string? ItemLength { get; set; }
        public string? ITM_Width { get; set; }

        public string? MaterialGrade { get; set; }

        public string? WarehouseCode { get; set; }
        public string? UOM { get; set; }

        public string Qty { get; set; }

        public double UnitPrice { get; set; }

        public double Amount { get; set; }

        public DateTime? DeliveryDate { get; set; }
    }
    public class ServiceOrderSummary_DTO
    {
        public long JISVOH_Number { get; set; }

        public int SO_Id { get; set; }

        public int SO_CreatorCode { get; set; }

        public string? JISVOH_SO_No { get; set; }

        public DateTime JISVOH_SO_Date { get; set; }

        public long JISVOH_JW_Customer_Number { get; set; }

        public long CUS_JCG_Number { get; set; }

        public string? JCG_JW_CustomerGroup { get; set; }

        public string? JCC_JW_CustomerCategory { get; set; }

        public string? CUS_Name { get; set; }

        public string? CurrencyCode { get; set; }

        public string? PRS_ProcessName { get; set; }

        public string? Segregation { get; set; }

        public int NoOfLineItems { get; set; }

        public string Qty { get; set; }

        public double Amount { get; set; }
        public string? JISVOH_RegNo { get; set; }
        public DateTime JISVOH_RegDate { get; set; }
        public string? JISVOH_ServiceOrderNo { get; set; }
        public DateTime JISVOH_ServiceOrderDate { get; set; }
    }
    public class JI_ServiceOrderHead_DTO
    {
        public long JISVOH_Number { get; set; }

        [Display(Name = "SVO Register No.")]
        public string JISVOH_RegNo { get; set; }

        [Display(Name = "Date")]
        public DateTime JISVOH_RegDate { get; set; }

        [Display(Name = "Service Order No.")]
        public string JISVOH_ServiceOrderNo { get; set; }

        [Display(Name = "Service Order Date")]
        public DateTime JISVOH_ServiceOrderDate { get; set; }

        [Display(Name = "JW Customer")]
        public long JISVOH_JW_Customer_Number { get; set; }

        public string JISVOH_JW_Customer_Name { get; set; }   // Display only

        [Display(Name = "Currency")]
        public long JISVOH_Currency_Number { get; set; }

        [Display(Name = "Terms of Payment")]
        public string JISVOH_PaymentTerms { get; set; }

        [Display(Name = "Terms of Delivery")]
        public string JISVOH_DeliveryTerms { get; set; }

        [Display(Name = "Mode of Delivery")]
        public string JISVOH_DeliveryMode { get; set; }

        [Display(Name = "Tax")]
        public string JISVOH_Tax { get; set; }

        [Display(Name = "Technical delivery conditions")]
        public string JISVOH_TDC { get; set; }

        [Display(Name = "Remarks")]
        public string JISVOH_Remarks { get; set; }

        public long SVO_Id { get; set; }
        public string JISVOI_Item_Code { get; set; }
        [Display(Name = "Material Seggregation")]
        public long? JISVOH_MS_Number { get; set; }
    }
    public class JI_ServiceOrder_DTO
    {
        public JI_ServiceOrderHead_DTO Header { get; set; } = new();
        public List<JI_ServiceOrderItem_DTO> Items { get; set; } = new();
    }
    public class JI_ServiceOrderItem_DTO
    {
        public long JISVOI_Number { get; set; }
        public long JISVOI_JISVOH_Number { get; set; }

        public long JISVOI_PRS_Number { get; set; }
        public long JISVOI_Item_Number { get; set; }

        public string JISVOI_Item_Code { get; set; }
        public string JISVOI_Item_Description { get; set; }

        public decimal JISVOI_OuterDia { get; set; }
        public decimal JISVOI_Thickness { get; set; }
        public decimal JISVOI_Length { get; set; }
        public decimal JISVOI_Width { get; set; }

        public string JISVOI_MaterialGrade { get; set; }
        public string JISVOI_ItemGroup { get; set; }

        public long JISVOI_WH_Number { get; set; }
        public long JISVOI_UoM_Number { get; set; }

        public double JISVOI_Qty { get; set; }
        public double JISVOI_UnitPrice { get; set; }
        public double JISVOI_Amount { get; set; }

        public DateTime? JISVOI_DeliveryDate { get; set; }

        public bool JISVOI_IsDeleted { get; set; }
    }
}

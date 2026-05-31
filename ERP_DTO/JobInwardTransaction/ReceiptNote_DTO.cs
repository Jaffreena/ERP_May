using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO.JobInwardTransaction
{
    #region DTO Create

  
    public class ReceiptNoteBatch_DTO
    {
        [Display(Name = "Batch Number")]
        public Int64 RNI_BCH_No { get; set; }

        public string? RNI_BCH_Num { get; set; }


        public Int64 RNI_BCH_RNI_Number { get; set; }



        public Int64 JIRNI_BCH_JIRNH_Number { get; set; }
        public Int64 JIRNI_BCH_JIRNI_Number { get; set; }
        public string? RNI_BCH_WH_Name { get; set; }

        [Display(Name = "Warehouse")]
        //  [Required(ErrorMessage = "Warehouse is Required")]
        public string? WH_Number { get; set; }

        [Display(Name = "Batch Date")]
        [Required(ErrorMessage = "Batch Date is Required")]
        public string? RNI_BCH_Date { get; set; }

        [Display(Name = "Batch Ware house Number")]
        //   [Required(ErrorMessage = "Batch Number is Required")]
        public string? RNI_BCH_WH_Number { get; set; }

        [Display(Name = "Qty")]
        //  [Required(ErrorMessage = "Qty is Required")]
        public string? RNI_BCH_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public string? RNI_BCH_UnitPrice { get; set; }

        [Display(Name = "Batch Value")]
        // [Required(ErrorMessage = "Batch Value is Required")]
        public string? RNI_BCH_Value { get; set; }

        public string? RNI_BCH_IsDeleted { get; set; }
        public string? RNI_BCH_Item_Number { get; set; }
        public Int64 RNI_BCH_Item_Index { get; set; }

        public string? RNI_BCH_Number { get; set; }
        public string? JIRNI_Number { get; set; }

        public void Reset()
        {
            this.RNI_BCH_No = 0;
            this.JIRNI_BCH_JIRNH_Number = 0;
            this.JIRNI_BCH_JIRNI_Number = 0;
            this.WH_Number = "";
            this.RNI_BCH_Date = "";
            this.RNI_BCH_WH_Number = "";
            this.RNI_BCH_Qty = "";
            this.RNI_BCH_UnitPrice = "";
            this.RNI_BCH_Value = "";
            this.RNI_BCH_IsDeleted = "false";
            this.RNI_BCH_Item_Number = "";
        }
    }
    public class ReceiptNoteItem_DTO
    {
        public Int64 JIRNI_Number { get; set; }
        public Int64 JIRNI_JIRNH_Number { get; set; }

        [Display(Name = "Process")]
        [Required(ErrorMessage = "Process is Required")]
        public string? PRS_Number { get; set; }

        public string? PRS_Name { get; set; }

        //[Display(Name = "Item Code")]
        //[Required(ErrorMessage = "Item Code is Required")]
        public string? Item_Number { get; set; }
        public string? Item_Code { get; set; }

        public string? RNI_Index { get; set; }
        public string? RNI_MS_Number { get; set; }
        //[Display(Name = "Item Code")]
        //[Required(ErrorMessage = "Item Code is Required")]
        public string? RNI_Item_Number { get; set; }
        //[Display(Name = "Item Code")]
        //[Required(ErrorMessage = "Item Code is Required")]
        public string? RNI_ItemCode { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Description")]
        public string? RNI_Description { get; set; }
        [Display(Name = "Item Group")]
        public string? RNI_ItemGroup { get; set; }
        //[Display(Name = "Warehouse")]
        //[Required(ErrorMessage = "Warehouse is Required")]
        public string? RNI_Warehouse_Number { get; set; }

        //[Display(Name = "UoM")]
        //[Required(ErrorMessage = "UoM is Required")]
        public String? RNI_UoM_Number { get; set; }
        public String? RNI_UoM { get; set; }
        public String? RNI_DecimalPlaces { get; set; }
        //[Display(Name = "Qty")]
        //[Required(ErrorMessage = "Qty is Required")]
        public String? RNI_Qty { get; set; }

        //[Display(Name = "Unit Price")]
        //[Required(ErrorMessage = "Unit Price is Required")]
        public String? RNI_UnitPrice { get; set; }

        //[Display(Name = "Amount")]
        //[Required(ErrorMessage = "Amount is Required")]
        public String? RNI_Amount { get; set; }

        public string? OuterDia { get; set; }
        public string? Thickness { get; set; }
        public string? Length { get; set; }
        public string? Width { get; set; }
        public string? MaterialGrade { get; set; }
        public string? ItemGroup { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public string? WH_Number { get; set; }

        [Display(Name = "UoM")]
        [Required(ErrorMessage = "UoM is Required")]
        public string? UoM_Number { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Qty is Required")]
        public string? Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public string? UnitPrice { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        public string? Amount { get; set; }

        public string? IsDeleted { get; set; }

        public void Reset()
        {
            this.JIRNI_Number = 0;
            this.JIRNI_JIRNH_Number = 0;
            this.PRS_Number = "";
            this.Item_Number = "";
            this.Description = "";
            this.OuterDia = "";
            this.Thickness = "";
            this.Length = "";
            this.Width = "";
            this.MaterialGrade = "";
            this.ItemGroup = "";
            this.WH_Number = "";
            this.UoM_Number = "";
            this.Qty = "";
            this.UnitPrice = "";
            this.Amount = "";
            this.IsDeleted = "";
        }
    }
    public class ReceiptNoteHead_DTO
    {
        public Int64 JIRNH_Number { get; set; }

        [Display(Name = "Receipt Note No")]
        [Required(ErrorMessage = "Receipt Note No is Required")]
        public string? RN_No { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Date is Required")]
        public string? RN_Date { get; set; }

        [Display(Name = "JW Customer DC No")]
        [Required(ErrorMessage = "Customer DC No is Required")]
        public string? JW_CustomerDC_No { get; set; }

        [Display(Name = "Customer DC Date")]
        [Required(ErrorMessage = "Customer DC Date is Required")]
        public string? JW_CustomerDC_Date { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public string? MS_Number { get; set; }

        [Display(Name = "JW Customer")]
        [Required(ErrorMessage = "Customer is Required")]
        public string? JWC_Number { get; set; }

        [Display(Name = "JW Customer")]
        [Required(ErrorMessage = "Customer is Required")]
        public string? JWC_Name { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public string? Currency_Number { get; set; }

        public string? Currency_Name { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public string? WH_Number { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        // Child Lists
        public List<ReceiptNoteItem_DTO>? Items { get; set; } = new();
        public List<ReceiptNoteBatch_DTO>? ItemBatch { get; set; } = new();
        public int? Mode_ID { get; set; }
        public int Mode { get; set; }

        public void Reset()
        {
            this.JIRNH_Number = 0;
            this.RN_No = null;
            this.RN_Date = "";
            this.JW_CustomerDC_No = "";
            this.JW_CustomerDC_Date = "";
            this.MS_Number = null;
            this.JWC_Number = null;
            this.Currency_Number = null;
            this.WH_Number = null;
            this.Remarks = "";
            this.Mode_ID = 0;
            this.Items = null;
            this.ItemBatch = null;
            this.Items = new List<ReceiptNoteItem_DTO>();
            this.ItemBatch = new List<ReceiptNoteBatch_DTO>();
        }
    }
    #endregion


    #region DTO Edit

    public class ReceiptNoteEdit_DTO
    {
        // =======================
        // HEADER (JIRNH)
        // =======================
        public Int64 JIRNH_Number { get; set; }
        public String? JIRNH_RN_No { get; set; }
        public DateTime? JIRNH_RN_Date { get; set; }
        public String? JIRNH_Date { get; set; }
        public Int64 RNH_Date { get; set; }
        public String? JIRNH_JW_CustomerDC_No { get; set; }
        public DateTime? JIRNH_JW_CustomerDC_Date { get; set; }
        public Int64 JIRNH_MS_Number { get; set; }
        public Int64 JIRNH_JWC_Number { get; set; }
        public Int64 JIRNH_Currency_Number { get; set; }
        public Int64 JIRNH_WH_Number { get; set; }
        public String? JIRNH_Remarks { get; set; }

        //public int RNH_Date { get; set; } // For Numbering Purpose Only (Important ❗)

        // =======================
        // ITEM (JIRNI)
        // =======================
        public Int64 JIRNI_JIRNH_Number { get; set; }
        public Int64 JIRNI_Number { get; set; }
        public Int64 JIRNI_PRS_Number { get; set; }
        public string JIRNI_PRS_Name { get; set; }
        public Int64 JIRNI_Item_Number { get; set; }
        public string JIRNI_Item_Number1 { get; set; }
        public String? JIRNI_Description { get; set; }
        public String? JIRNI_OuterDia { get; set; }
        public String? JIRNI_Thickness { get; set; }
        public String? JIRNI_Length { get; set; }
        public String? JIRNI_Width { get; set; }
        public String? JIRNI_MaterialGrade { get; set; }
        public String? JIRNI_ItemGroup { get; set; }

        public Int64 JIRNI_WH_Number { get; set; }
        public String? JIRNI_ITM_Code { get; set; }
        public Int64 JIRNI_UoM_Number { get; set; }
        public Double JIRNI_OriginalQty { get; set; }
        public Double JIRNI_AmendedQty { get; set; }
        public Double JIRNI_UnitPrice { get; set; }
        public Double JIRNI_Amount { get; set; }

        // =======================
        // BATCH (JIRNI_BCH)
        // =======================
        public Int64 JIRNI_BCH_JIRNH_Number { get; set; }
        public Int64 JIRNI_BCH_JIRNI_Number { get; set; }
        public Int64 JIRNI_BCH_Number { get; set; }

        public Int64 JIRNI_BCH_WH_Number { get; set; }
        public DateTime? JIRNI_BCH_BatchDate { get; set; }
        public String? JIRNI_BCH_BatchNo { get; set; }
        public Double JIRNI_BCH_BatchOriginalQty { get; set; }
        public Double JIRNI_BCH_BatchUsedQty { get; set; }
        public Double JIRNI_BCH_BatchAmendedQty { get; set; }
        public Double JIRNI_BCH_BatchUnitPrice { get; set; }
        public Double JIRNI_BCH_BatchValue { get; set; }

        // =======================
        // COMMON
        // =======================
        public String? JIRN_Search { get; set; }
        public String? JIRN_DeleteNumbers { get; set; }
        public Int64 JIRN_CreatorCode { get; set; }
        public Int16 JIRN_Id { get; set; }

        // =======================
        // RESET METHOD
        // =======================
        public void Reset()
        {
            // Header
            JIRNH_Number = 0;
            JIRNH_RN_No = "";
            JIRNH_RN_Date = null;
            JIRNH_JW_CustomerDC_No = "";
            JIRNH_JW_CustomerDC_Date = null;
            JIRNH_MS_Number = 0;
            JIRNH_JWC_Number = 0;
            JIRNH_Currency_Number = 0;
            JIRNH_WH_Number = 0;
            JIRNH_Remarks = "";

            // Item
            JIRNI_JIRNH_Number = 0;
            JIRNI_Number = 0;
            JIRNI_PRS_Number = 0;
            JIRNI_Item_Number = 0;
            JIRNI_Description = "";
            JIRNI_OuterDia = "";
            JIRNI_Thickness = "";
            JIRNI_Length = "";
            JIRNI_Width = "";
            JIRNI_MaterialGrade = "";
            JIRNI_ItemGroup = "";
            JIRNI_WH_Number = 0;
            JIRNI_UoM_Number = 0;
            JIRNI_AmendedQty = 0;
            JIRNI_UnitPrice = 0;
            JIRNI_Amount = 0;

            // Batch
            JIRNI_BCH_JIRNH_Number = 0;
            JIRNI_BCH_JIRNI_Number = 0;
            JIRNI_BCH_Number = 0;
            JIRNI_BCH_WH_Number = 0;
            JIRNI_BCH_BatchDate = null;
            JIRNI_BCH_BatchNo = "";
            JIRNI_BCH_BatchOriginalQty = 0;
            JIRNI_BCH_BatchUsedQty = 0;
            JIRNI_BCH_BatchAmendedQty = 0;

            JIRNI_BCH_BatchUnitPrice = 0;
            JIRNI_BCH_BatchValue = 0;

            // Common
            JIRN_Search = "";
            JIRN_DeleteNumbers = "";
            JIRN_CreatorCode = 0;
            JIRN_Id = 0;
        }
    }
    public class ReceiptNoteBatchEdit_DTO
    {
        [Display(Name = "Batch Number")]
        public Int64 RNI_BCH_No { get; set; }

        public string? RNI_BCH_Num { get; set; }


        public Int64 RNI_BCH_RNI_Number { get; set; }



        public Int64 JIRNI_BCH_JIRNH_Number { get; set; }
        public Int64 JIRNI_BCH_JIRNI_Number { get; set; }
        public string? RNI_BCH_WH_Name { get; set; }

        [Display(Name = "Warehouse")]
        //  [Required(ErrorMessage = "Warehouse is Required")]
        public string? WH_Number { get; set; }

        [Display(Name = "Batch Date")]
        [Required(ErrorMessage = "Batch Date is Required")]
        public string? RNI_BCH_Date { get; set; }

        [Display(Name = "Batch Ware house Number")]
        //   [Required(ErrorMessage = "Batch Number is Required")]
        public string? RNI_BCH_WH_Number { get; set; }

        [Display(Name = "Original Qty")]
        //  [Required(ErrorMessage = "Qty is Required")]
        public string? RNI_BCH_OriginalQty { get; set; }
        [Display(Name = "Used Qty")]
        //  [Required(ErrorMessage = "Qty is Required")]
        public string? RNI_BCH_UsedQty { get; set; }
        [Display(Name = "Amended Qty")]
        //  [Required(ErrorMessage = "Qty is Required")]
        public string? RNI_BCH_AmendedQty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public string? RNI_BCH_UnitPrice { get; set; }

        [Display(Name = "Batch Value")]
        // [Required(ErrorMessage = "Batch Value is Required")]
        public string? RNI_BCH_Value { get; set; }

        public string? RNI_BCH_IsDeleted { get; set; }
        public string? RNI_BCH_Item_Number { get; set; }
        public Int64 RNI_BCH_Item_Index { get; set; }

        public string? RNI_BCH_Number { get; set; }
        public string? JIRNI_Number { get; set; }

        public void Reset()
        {
            this.RNI_BCH_No = 0;
            this.JIRNI_BCH_JIRNH_Number = 0;
            this.JIRNI_BCH_JIRNI_Number = 0;
            this.WH_Number = "";
            this.RNI_BCH_Date = "";
            this.RNI_BCH_WH_Number = "";
            this.RNI_BCH_OriginalQty = "";
            this.RNI_BCH_UsedQty = "";
            this.RNI_BCH_AmendedQty = "";
            this.RNI_BCH_UnitPrice = "";
            this.RNI_BCH_Value = "";
            this.RNI_BCH_IsDeleted = "false";
            this.RNI_BCH_Item_Number = "";
        }
    }
  
    public class ReceiptNoteItemEdit_DTO
    {
        public Int64 JIRNI_Number { get; set; }
        public Int64 JIRNI_JIRNH_Number { get; set; }

        [Display(Name = "Process")]
        [Required(ErrorMessage = "Process is Required")]
        public string? PRS_Number { get; set; }

        public string? PRS_Name { get; set; }

        //[Display(Name = "Item Code")]
        //[Required(ErrorMessage = "Item Code is Required")]
        public string? Item_Number { get; set; }
        public string? Item_Code { get; set; }

        public string? RNI_Index { get; set; }
        public string? RNI_MS_Number { get; set; }
        //[Display(Name = "Item Code")]
        //[Required(ErrorMessage = "Item Code is Required")]
        public string? RNI_Item_Number { get; set; }
        //[Display(Name = "Item Code")]
        //[Required(ErrorMessage = "Item Code is Required")]
        public string? RNI_ItemCode { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Description")]
        public string? RNI_Description { get; set; }
        [Display(Name = "Item Group")]
        public string? RNI_ItemGroup { get; set; }
        //[Display(Name = "Warehouse")]
        //[Required(ErrorMessage = "Warehouse is Required")]
        public string? RNI_Warehouse_Number { get; set; }

        //[Display(Name = "UoM")]
        //[Required(ErrorMessage = "UoM is Required")]
        public String? RNI_UoM_Number { get; set; }
        public String? RNI_UoM { get; set; }
        public String? RNI_DecimalPlaces { get; set; }
        //[Display(Name = "Qty")]
        //[Required(ErrorMessage = "Qty is Required")]
        public String? RNI_Qty { get; set; }

        //[Display(Name = "Unit Price")]
        //[Required(ErrorMessage = "Unit Price is Required")]
        public String? RNI_UnitPrice { get; set; }

        //[Display(Name = "Amount")]
        //[Required(ErrorMessage = "Amount is Required")]
        public String? RNI_Amount { get; set; }

        public string? OuterDia { get; set; }
        public string? Thickness { get; set; }
        public string? Length { get; set; }
        public string? Width { get; set; }
        public string? MaterialGrade { get; set; }
        public string? ItemGroup { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public string? WH_Number { get; set; }

        [Display(Name = "UoM")]
        [Required(ErrorMessage = "UoM is Required")]
        public string? UoM_Number { get; set; }

        [Display(Name = "Original Qty")]
     //   [Required(ErrorMessage = "Qty is Required")]
        public string? OriginalQty { get; set; }

        public string? UsedQty { get; set; }
        public string? AmendQty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public string? UnitPrice { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        public string? Amount { get; set; }

        public string? IsDeleted { get; set; }

        public void Reset()
        {
            this.JIRNI_Number = 0;
            this.JIRNI_JIRNH_Number = 0;
            this.PRS_Number = "";
            this.Item_Number = "";
            this.Description = "";
            this.OuterDia = "";
            this.Thickness = "";
            this.Length = "";
            this.Width = "";
            this.MaterialGrade = "";
            this.ItemGroup = "";
            this.WH_Number = "";
            this.UoM_Number = "";
            this.OriginalQty = "";
            this.UsedQty = "";
            this.AmendQty = "";
            this.UnitPrice = "";
            this.Amount = "";
            this.IsDeleted = "";
        }
    }
  
    public class ReceiptNoteHeadEdit_DTO
    {
        public Int64 JIRNH_Number { get; set; }

        [Display(Name = "Receipt Note No")]
        [Required(ErrorMessage = "Receipt Note No is Required")]
        public string? RN_No { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Date is Required")]
        public string? RN_Date { get; set; }

        [Display(Name = "JW Customer DC No")]
        [Required(ErrorMessage = "Customer DC No is Required")]
        public string? JW_CustomerDC_No { get; set; }

        [Display(Name = "Customer DC Date")]
        [Required(ErrorMessage = "Customer DC Date is Required")]
        public string? JW_CustomerDC_Date { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public string? MS_Number { get; set; }

        [Display(Name = "JW Customer")]
        [Required(ErrorMessage = "Customer is Required")]
        public string? JWC_Number { get; set; }

        [Display(Name = "JW Customer")]
        [Required(ErrorMessage = "Customer is Required")]
        public string? JWC_Name { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public string? Currency_Number { get; set; }

        public string? Currency_Name { get; set; }

        [Display(Name = "Warehouse")]
        [Required(ErrorMessage = "Warehouse is Required")]
        public string? WH_Number { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        // Child Lists
        public List<ReceiptNoteItemEdit_DTO>? Items { get; set; } = new();
        public List<ReceiptNoteBatchEdit_DTO>? ItemBatch { get; set; } = new();
        public int? Mode_ID { get; set; }
        public int Mode { get; set; }

        public void Reset()
        {
            this.JIRNH_Number = 0;
            this.RN_No = null;
            this.RN_Date = "";
            this.JW_CustomerDC_No = "";
            this.JW_CustomerDC_Date = "";
            this.MS_Number = null;
            this.JWC_Number = null;
            this.Currency_Number = null;
            this.WH_Number = null;
            this.Remarks = "";
            this.Mode_ID = 0;
            this.Items = null;
            this.ItemBatch = null;
            this.Items = new List<ReceiptNoteItemEdit_DTO>();
            this.ItemBatch = new List<ReceiptNoteBatchEdit_DTO>();
        }
    }

    #endregion
    public class ReceiptNote_DTO
    {
        // =======================
        // HEADER (JIRNH)
        // =======================
        public Int64 JIRNH_Number { get; set; }
        public String? JIRNH_RN_No { get; set; }
        public DateTime? JIRNH_RN_Date { get; set; }
        public String? JIRNH_Date { get; set; }
        public Int64 RNH_Date { get; set; }
        public String? JIRNH_JW_CustomerDC_No { get; set; }
        public DateTime? JIRNH_JW_CustomerDC_Date { get; set; }
        public Int64 JIRNH_MS_Number { get; set; }
        public Int64 JIRNH_JWC_Number { get; set; }
        public Int64 JIRNH_Currency_Number { get; set; }
        public Int64 JIRNH_WH_Number { get; set; }
        public String? JIRNH_Remarks { get; set; }

        //public int RNH_Date { get; set; } // For Numbering Purpose Only (Important ❗)

        // =======================
        // ITEM (JIRNI)
        // =======================
        public Int64 JIRNI_JIRNH_Number { get; set; }
        public Int64 JIRNI_Number { get; set; }
        public Int64 JIRNI_PRS_Number { get; set; }
        public string JIRNI_PRS_Name { get; set; }
        public Int64 JIRNI_Item_Number { get; set; }
        public string JIRNI_Item_Number1 { get; set; }
        public String? JIRNI_Description { get; set; }
        public String? JIRNI_OuterDia { get; set; }
        public String? JIRNI_Thickness { get; set; }
        public String? JIRNI_Length { get; set; }
        public String? JIRNI_Width { get; set; }
        public String? JIRNI_MaterialGrade { get; set; }
        public String? JIRNI_ItemGroup { get; set; }

        public Int64 JIRNI_WH_Number { get; set; }
        public String? JIRNI_ITM_Code { get; set; }
        public Int64 JIRNI_UoM_Number { get; set; }
        public Double JIRNI_Qty { get; set; }
        public Double JIRNI_UnitPrice { get; set; }
        public Double JIRNI_Amount { get; set; }

        // =======================
        // BATCH (JIRNI_BCH)
        // =======================
        public Int64 JIRNI_BCH_JIRNH_Number { get; set; }
        public Int64 JIRNI_BCH_JIRNI_Number { get; set; }
        public Int64 JIRNI_BCH_Number { get; set; }

        public Int64 JIRNI_BCH_WH_Number { get; set; }
        public DateTime? JIRNI_BCH_BatchDate { get; set; }
        public String? JIRNI_BCH_BatchNo { get; set; }
        public Double JIRNI_BCH_BatchQty { get; set; }
        public Double JIRNI_BCH_BatchUnitPrice { get; set; }
        public Double JIRNI_BCH_BatchValue { get; set; }

        // =======================
        // COMMON
        // =======================
        public String? JIRN_Search { get; set; }
        public String? JIRN_DeleteNumbers { get; set; }
        public Int64 JIRN_CreatorCode { get; set; }
        public Int16 JIRN_Id { get; set; }

        // =======================
        // RESET METHOD
        // =======================
        public void Reset()
        {
            // Header
            JIRNH_Number = 0;
            JIRNH_RN_No = "";
            JIRNH_RN_Date = null;
            JIRNH_JW_CustomerDC_No = "";
            JIRNH_JW_CustomerDC_Date = null;
            JIRNH_MS_Number = 0;
            JIRNH_JWC_Number = 0;
            JIRNH_Currency_Number = 0;
            JIRNH_WH_Number = 0;
            JIRNH_Remarks = "";

            // Item
            JIRNI_JIRNH_Number = 0;
            JIRNI_Number = 0;
            JIRNI_PRS_Number = 0;
            JIRNI_Item_Number = 0;
            JIRNI_Description = "";
            JIRNI_OuterDia = "";
            JIRNI_Thickness = "";
            JIRNI_Length = "";
            JIRNI_Width = "";
            JIRNI_MaterialGrade = "";
            JIRNI_ItemGroup = "";
            JIRNI_WH_Number = 0;
            JIRNI_UoM_Number = 0;
            JIRNI_Qty = 0;
            JIRNI_UnitPrice = 0;
            JIRNI_Amount = 0;

            // Batch
            JIRNI_BCH_JIRNH_Number = 0;
            JIRNI_BCH_JIRNI_Number = 0;
            JIRNI_BCH_Number = 0;
            JIRNI_BCH_WH_Number = 0;
            JIRNI_BCH_BatchDate = null;
            JIRNI_BCH_BatchNo = "";
            JIRNI_BCH_BatchQty = 0;
            JIRNI_BCH_BatchUnitPrice = 0;
            JIRNI_BCH_BatchValue = 0;

            // Common
            JIRN_Search = "";
            JIRN_DeleteNumbers = "";
            JIRN_CreatorCode = 0;
            JIRN_Id = 0;
        }
    }

    public class ReceiptNoteSummary_DTO
    {
        public Int64 RN_Number { get; set; }

        public String? RN_No { get; set; }
        public String? RN_Date { get; set; }

        public String? RN_JW_CustomerDC_No { get; set; }
        public String? RN_JW_CustomerDC_Date { get; set; }

        public String? RN_MS_Number { get; set; }

        public String? RN_JWC_Name { get; set; }

        public String? RN_CUR_Name { get; set; }

        public String? RN_WH_Name { get; set; }

        public String? RN_ITM_Group { get; set; }

        public String? RN_ITM_Category { get; set; }

        
        public String? RN_ITM_Code { get; set; }
        public String? RN_ITM_Description { get; set; }

        public String? RN_ITM_OuterDia { get; set; }
        public String? RN_ITM_Thickness { get; set; }
        public String? RN_ITM_Length { get; set; }
        public String? RN_ITM_Width { get; set; }
        public String? RN_ITM_MaterialGrade { get; set; }

        public String? RN_Qty { get; set; }
        public String? RN_UoM_Name { get; set; }

        public String? RN_NoOfItem { get; set; }

        public String? RN_UnitPrice { get; set; }
        public String? RN_MaterialValue { get; set; }

        public String? RN_TotalItemExpense { get; set; }
        public String? RN_TotalHeadExpense { get; set; }

        public String? RN_TotalAmount { get; set; }

        public String? RN_ExchangeRate { get; set; }

        public String? RN_HeadGST_Amount { get; set; }
        public String? RN_ItemGST_Amount { get; set; }

        public String? RN_InvoiceAmount { get; set; }

        public String? RN_HeadWHT_Amount { get; set; }
        public String? RN_ItemWHT_Amount { get; set; }

        public String? RN_RoundOff { get; set; }

        public String? RN_SupplierPayable { get; set; }

        public String? RN_Remarks { get; set; }

        public String? RN_Process{ get; set; }

        public string? RN_JWC_WH_Name { get; set; }
        public string? RN_ITM_GroupInfo { get; set; }
    }

    public class ReceiptNoteInfo_DTO
    {
        public Int64 RN_Number { get; set; }
        public String? RN_No { get; set; }
        public Int32 RN_Date { get; set; }
        public String? RN_ExchangeRate { get; set; }

        public Int64 RN_JWC_Number { get; set; }
        public Int64 RN_JWC_LOC_Number { get; set; }

        public Int64 RN_CUR_Number { get; set; }
        public Int32 RN_CUR_DecimalPlaces { get; set; }

        public Int64 RN_TCT_Number { get; set; }
        public Int64 RN_WHT_Number { get; set; }

        public Int64 RN_MS_Number { get; set; }

        public Double RN_WHT_Tax { get; set; }
        public Double RN_WHT_Percent { get; set; }

        public Double RN_MaterialCost { get; set; }
        public Double RN_ItemMiscExpense { get; set; }
        public Double RN_HeaderMiscExpense { get; set; }

        public Double RN_GST_Amount { get; set; }
        public Double RN_InvoiceAmount { get; set; }

        public Double RN_WHT_Amount { get; set; }

        public Double RN_RoundOff { get; set; }

        public Double RN_SupplierPayable { get; set; }


        // ================= ITEM =================

        public Int64 RN_I_Number { get; set; }

        public Int64 RN_WH_Number { get; set; }

        public Int64 RN_ITM_Number { get; set; }

        public String? RN_ITM_Code { get; set; }

        public Int64 RN_UoM_Number { get; set; }

        public Double RN_Qty { get; set; }

        public Double RN_UnitPrice { get; set; }

        public Double RN_Amount { get; set; }

        public Double RN_ExpenseValue { get; set; }

        public Int64 RN_HSN_Number { get; set; }


        // ================= EXPENSE =================

        public Int64 RN_EXP_Number { get; set; }

        public Int64 RN_EXP_MEC_Number { get; set; }

        public String? RN_EXP_Remarks { get; set; }

        public Int64 RN_EXP_OCRN_Number { get; set; }

        public Int64 RN_EXP_CM_Number { get; set; }

        public Double RN_EXP_ExpenseBase { get; set; }

        public Double RN_EXP_ExpenseValue { get; set; }

        public Int64 RN_EXP_ALCT_Number { get; set; }

        public Int64 RN_EXP_LA_Number { get; set; }

        public Int64 RN_EXP_CalculateGST { get; set; }

        public Double RN_EXP_GST_Amount { get; set; }

        public Int64 RN_EXP_SAC_Number { get; set; }

        public Double RN_EXP_WHT_Percent { get; set; }

        public Double RN_EXP_WHT_Amount { get; set; }


        // ================= BATCH =================

        public Int64 RN_BCH_Number { get; set; }

        public Int64 RN_BCH_BCH_Number { get; set; }

        public String? RN_BCH_Date { get; set; }

        public String? RN_BCH_No { get; set; }

        public Double RN_BCH_Qty { get; set; }

        public Double RN_BCH_UnitPrice { get; set; }

        public Double RN_BCH_Value { get; set; }

        public Int32 RN_BCH_Mode { get; set; }

        public Int32 RN_BCH_Index { get; set; }


        // ================= ADDRESS =================

        public Int64 RN_ADD_Number { get; set; }

        public Int64 RN_ADD_ADTP_Number { get; set; }

        public String? RN_ADD_AddressID { get; set; }

        public String? RN_ADD_Address { get; set; }

        public String? RN_ADD_City { get; set; }

        public String? RN_ADD_State { get; set; }

        public String? RN_ADD_Country { get; set; }

        public String? RN_ADD_Pin { get; set; }

        public String? RN_ADD_GSTIN { get; set; }


        // ================= COMMON =================

        public String? RN_Search { get; set; }

        public String? RN_DeleteNumbers { get; set; }

        public Int64 RN_CreatorCode { get; set; }

        public Int16 RN_Id { get; set; }


        public void Reset()
        {
            this.RN_Number = 0;
            this.RN_No = "";
            this.RN_Date = 0;
            this.RN_ExchangeRate = "";

            this.RN_JWC_Number = 0;
            this.RN_JWC_LOC_Number = 0;

            this.RN_CUR_Number = 0;
            this.RN_CUR_DecimalPlaces = 0;

            this.RN_TCT_Number = 0;
            this.RN_WHT_Number = 0;

            this.RN_MS_Number = 0;

            this.RN_WHT_Tax = 0;
            this.RN_WHT_Percent = 0;

            this.RN_MaterialCost = 0;
            this.RN_ItemMiscExpense = 0;
            this.RN_HeaderMiscExpense = 0;

            this.RN_GST_Amount = 0;
            this.RN_InvoiceAmount = 0;

            this.RN_WHT_Amount = 0;

            this.RN_RoundOff = 0;

            this.RN_SupplierPayable = 0;


            // ITEM
            this.RN_I_Number = 0;
            this.RN_WH_Number = 0;
            this.RN_ITM_Number = 0;
            this.RN_ITM_Code = "";
            this.RN_UoM_Number = 0;
            this.RN_Qty = 0;
            this.RN_UnitPrice = 0;
            this.RN_Amount = 0;
            this.RN_ExpenseValue = 0;
            this.RN_HSN_Number = 0;


            // EXPENSE
            this.RN_EXP_Number = 0;
            this.RN_EXP_MEC_Number = 0;
            this.RN_EXP_Remarks = "";
            this.RN_EXP_OCRN_Number = 0;
            this.RN_EXP_CM_Number = 0;
            this.RN_EXP_ExpenseBase = 0;
            this.RN_EXP_ExpenseValue = 0;
            this.RN_EXP_ALCT_Number = 0;
            this.RN_EXP_LA_Number = 0;
            this.RN_EXP_CalculateGST = 0;
            this.RN_EXP_GST_Amount = 0;
            this.RN_EXP_SAC_Number = 0;
            this.RN_EXP_WHT_Percent = 0;
            this.RN_EXP_WHT_Amount = 0;


            // BATCH
            this.RN_BCH_Number = 0;
            this.RN_BCH_BCH_Number = 0;
            this.RN_BCH_Date = "";
            this.RN_BCH_No = "";
            this.RN_BCH_Qty = 0;
            this.RN_BCH_UnitPrice = 0;
            this.RN_BCH_Value = 0;
            this.RN_BCH_Mode = 0;
            this.RN_BCH_Index = 0;


            // ADDRESS
            this.RN_ADD_Number = 0;
            this.RN_ADD_ADTP_Number = 0;
            this.RN_ADD_AddressID = "";
            this.RN_ADD_Address = "";
            this.RN_ADD_City = "";
            this.RN_ADD_State = "";
            this.RN_ADD_Country = "";
            this.RN_ADD_Pin = "";
            this.RN_ADD_GSTIN = "";


            // COMMON
            this.RN_Search = "";
            this.RN_DeleteNumbers = "";
            this.RN_CreatorCode = 0;
            this.RN_Id = 0;
        }
    }


}

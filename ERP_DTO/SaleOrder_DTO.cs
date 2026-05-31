using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class SaleOrder_DTO
    {
        public Int64 SO_Number { get; set; }
        public String? SO_RegNo { get; set; }
        public String? SO_RegDate { get; set; }
        public String? SO_OrderNo { get; set; }
        public String? SO_OrderDate { get; set; }
        public Int64 SO_BUY_Number { get; set; }
        public String? SO_BUY_Name { get; set; }
        public Int64 SO_BUY_LOC_Number { get; set; }
        public Int16 SO_ExportOrder { get; set; }
        public Int64 SO_CUR_Number { get; set; }
        public String? SO_ExchangeRate { get; set; }
        public Int64 SO_MS_Number { get; set; }
        public String? SO_PaymentTerms { get; set; }
        public String? SO_PaymentMethod { get; set; }
        public String? SO_DeliveryTerms { get; set; }
        public String? SO_DeliveryMethod { get; set; }
        public String? SO_QCR { get; set; }
        public String? SO_TDC { get; set; }
        public String? SO_OtherRemarks { get; set; }

        public Double SO_TotalAmount { get; set; }
        public Double SO_TotalItemIncome { get; set; }
        public Double SO_TotalHeadIncome { get; set; }
        public Double SO_OrderValue { get; set; }

        public Int64 SO_I_Number { get; set; }
        public Int64 SO_ITM_Number { get; set; }
        public String? SO_ITM_Code { get; set; }
        public Int64 SO_UoM_Number { get; set; }
        public Double SO_Qty { get; set; }
        public Double SO_UnitPrice { get; set; }
        public Double SO_Amount { get; set; }
        public Double SO_IncomeValue { get; set; }
        public Int32 SO_DeliveryDate { get; set; }

        public Int64 SO_INC_Number { get; set; }
        public Int64 SO_INC_MIC_Number { get; set; }
        public String? SO_INC_Remarks { get; set; }
        public Int64 SO_INC_OCRN_Number { get; set; }
        public Int64 SO_INC_CM_Number { get; set; }
        public Double SO_INC_IncomeBase { get; set; }
        public Double SO_INC_IncomeValue { get; set; }
        public Int64 SO_INC_ALCT_Number { get; set; }
        public Int64 SO_INC_LA_Number { get; set; }

        public Int64 SO_ADD_Number { get; set; }
        public Int64 SO_ADD_ADTP_Number { get; set; }
        public String? SO_ADD_AddressID { get; set; }
        public String? SO_ADD_Address { get; set; }
        public String? SO_ADD_City { get; set; }
        public String? SO_ADD_State { get; set; }
        public String? SO_ADD_Country { get; set; }
        public String? SO_ADD_Pin { get; set; }
        public String? SO_ADD_GSTIN { get; set; }


        public String? SO_DeleteNumbers { get; set; }
        public Int64 SO_CreatorCode { get; set; }
        public Int16 SO_Id { get; set; }

        public void Reset()
        {
            this.SO_Number = 0;
            this.SO_RegNo = null;
            this.SO_RegDate = null;
            this.SO_OrderNo = null;
            this.SO_OrderDate = null;
            this.SO_BUY_Number = 0;
            this.SO_BUY_Name = null;
            this.SO_BUY_LOC_Number = 0;
            this.SO_ExportOrder = 0;
            this.SO_CUR_Number = 0;
            this.SO_ExchangeRate = null;
            this.SO_MS_Number = 0;
            this.SO_PaymentTerms = null;
            this.SO_PaymentMethod = null;
            this.SO_DeliveryTerms = null;
            this.SO_DeliveryMethod = null;
            this.SO_QCR = null;
            this.SO_TDC = null;
            this.SO_OtherRemarks = null;

            this.SO_TotalAmount = 0;
            this.SO_TotalItemIncome = 0;
            this.SO_TotalHeadIncome = 0;
            this.SO_OrderValue = 0;

            this.SO_I_Number = 0;
            this.SO_ITM_Number = 0;
            this.SO_UoM_Number = 0;
            this.SO_Qty = 0;
            this.SO_UnitPrice = 0;
            this.SO_Amount = 0;
            this.SO_DeliveryDate = 0;

            this.SO_INC_Number = 0;
            this.SO_INC_MIC_Number = 0;
            this.SO_INC_Remarks = null;
            this.SO_INC_OCRN_Number = 0;
            this.SO_INC_CM_Number = 0;
            this.SO_INC_IncomeBase = 0;
            this.SO_INC_IncomeValue = 0;
            this.SO_INC_ALCT_Number = 0;
            this.SO_INC_LA_Number = 0;

            this.SO_DeleteNumbers = null;
            this.SO_CreatorCode = 0;
            this.SO_Id = 0;
        }
    }


    public class SaleOrderHead_DTO
    {
        public Int64 SOH_Number { get; set; }

        [Display(Name = "SO Register No.")]
        [Required(ErrorMessage = "SO Register No. is Required")]
        public String? SOH_RegNo { get; set; }

        [Display(Name = "SO Register Date")]
        [MaxLength(15, ErrorMessage = "SO Register Date be longer than 15 characters.")]
        [Required(ErrorMessage = "SO Register Date is Required")]
        public String? SOH_RegDate { get; set; }

        [Display(Name = "Sale Order No")]
        [Required(ErrorMessage = "Sale Order No is Required")]
        public String? SOH_OrderNo { get; set; }

        [Display(Name = "Sale Order Date")]
        [MaxLength(15, ErrorMessage = "Sale Order Date be longer than 15 characters.")]
        [Required(ErrorMessage = "Sale Order Date is Required")]
        public String? SOH_OrderDate { get; set; }

        [Display(Name = "Buyer Name")]
        [Required(ErrorMessage = "Buyer Name is Required")]
        public String? SOH_BUY_Number { get; set; }
        public String? SOH_BUY_Name { get; set; }
        public String? SOH_BUY_LOC_Number { get; set; }

        [Display(Name = "Export Order")]
        [Required(ErrorMessage = "Import Order is Required")]
        public String? SOH_ExportOrder { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is Required")]
        public String? SOH_CUR_Number { get; set; }
        public String? SOH_CUR_Name { get; set; }
        public String? SOH_CUR_DecimalPlaces { get; set; }

        [Display(Name = "Exchange Rate")]
        public String? SOH_ExchangeRate { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public String? SOH_MS_Number { get; set; }

        [Display(Name = "Payment terms")]
        [MaxLength(50, ErrorMessage = "Payment terms be longer than 50 characters.")]
        public String? SOH_PaymentTerms { get; set; }

        [Display(Name = "Method of payment")]
        [MaxLength(50, ErrorMessage = "Method of payment be longer than 50 characters.")]
        public String? SOH_PaymentMethod { get; set; }

        [Display(Name = "Delivery terms")]
        [MaxLength(50, ErrorMessage = "Delivery terms be longer than 50 characters.")]
        public String? SOH_DeliveryTerms { get; set; }

        [Display(Name = "Mode of delivery")]
        [MaxLength(50, ErrorMessage = "Mode of delivery be longer than 50 characters.")]
        public String? SOH_DeliveryMethod { get; set; }

        [Display(Name = "Quality Check Remarks")]
        [MaxLength(250, ErrorMessage = "Quality Check Remarks be longer than 250 characters.")]
        public String? SOH_QCR { get; set; }

        [Display(Name = "Technical delivery conditions")]
        [MaxLength(250, ErrorMessage = "Technical Delivery Conditions be longer than 250 characters.")]
        public String? SOH_TDC { get; set; }

        [Display(Name = "Other Remarks")]
        [MaxLength(250, ErrorMessage = "Other Remarks be longer than 250 characters.")]
        public String? SOH_OtherRemarks { get; set; }

        [Display(Name = "Material Value")]
        public String? SOH_TotalAmount { get; set; }

        [Display(Name = "Item Misc.Income")]
        public String? SOH_TotalItemIncome { get; set; }

        [Display(Name = "Header Misc.Income")]
        public String? SOH_TotalHeadIncome { get; set; }

        [Display(Name = "Sale Order Value")]
        public String? SOH_OrderValue { get; set; }

        public List<SaleOrderItem_DTO>? SaleItems { get; set; }
        public List<SaleOrderIncome_DTO>? Income { get; set; }
        public List<SaleOrderIIncome_DTO>? ItemIncome { get; set; }
        public List<SaleOrderAddress_DTO>? BuyerAddress { get; set; }
        public void Reset()
        {
            this.SOH_Number = 0;
            this.SOH_RegNo = null;
            this.SOH_RegDate = null;
            this.SOH_OrderNo = null;
            this.SOH_OrderDate = null;
            this.SOH_BUY_Number = null;
            this.SOH_BUY_Name = null;
            this.SOH_BUY_LOC_Number = null;
            this.SOH_ExportOrder = null;
            this.SOH_CUR_Number = null;
            this.SOH_CUR_Name = null;
            this.SOH_ExchangeRate = null;
            this.SOH_MS_Number = null;
            this.SOH_PaymentTerms = null;
            this.SOH_PaymentMethod = null;
            this.SOH_DeliveryTerms = null;
            this.SOH_DeliveryMethod = null;
            this.SOH_QCR = null;
            this.SOH_TDC = null;
            this.SOH_OtherRemarks = null;
            //this.TotalItem = null;
            //this.TotalQty = null;
            //this.MaterialValue = null;
            //this.MiscExpense = null;
            this.SOH_TotalAmount = null;
            this.SOH_TotalItemIncome = null;
            this.SOH_TotalHeadIncome = null;
            this.SOH_OrderValue = null;
            this.SaleItems = null;
            this.Income = null;
            this.ItemIncome = null;
            this.BuyerAddress = null;
        }

    }
    public class SaleOrderItem_DTO
    {
        public Int64 SOI_Number { get; set; }
        public Int64 SOI_SOH_Number { get; set; }
        public Int16 SOI_Index { get; set; }
        public Int64 SOI_MS_Number { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public Int64 SOI_ITM_Number { get; set; }

        [Display(Name = "Description")]
        public String? SOI_ITM_Description { get; set; }

        [Display(Name = "Item Group")]
        public String? SOI_ITM_Group { get; set; }

        [Display(Name = "Item Code")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? SOI_ITM_Code { get; set; }

        [Display(Name = "Outer Dia")]
        public String? SOI_ITM_OuterDia { get; set; }

        [Display(Name = "Thickness")]
        public String? SOI_ITM_Thickness { get; set; }

        [Display(Name = "Length")]
        public String? SOI_ITM_Length { get; set; }

        [Display(Name = "Material Grade")]
        public String? SOI_ITM_MaterialGrade { get; set; }

        [Display(Name = "UoM")]
        [Required(ErrorMessage = "UoM is Required")]
        public String? SOI_UoM_Number { get; set; }
        public String? SOI_UoM_Name { get; set; }
        public String? SOI_UoM_OldNumber { get; set; }
        public String? SOI_UoM_DecimalPlaces { get; set; }

        [Display(Name = "Qty")]
        [Required(ErrorMessage = "Qty is Required")]
        public String? SOI_Qty { get; set; }

        [Display(Name = "Invoice Qty")]
        public String? SOI_SII_Qty { get; set; }

        [Display(Name = "SO Qty")]
        public String? SOI_OLD_Qty { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public String? SOI_UnitPrice { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        public String? SOI_Amount { get; set; }

        [Display(Name = "Income Value")]
        [Required(ErrorMessage = "Income value is Required")]
        public String? SOI_IncomeValue { get; set; }

        [Display(Name = "Delivery Date")]
        [Required(ErrorMessage = "Delivery Date is Required")]
        public String? SOI_DeliveryDate { get; set; }
        
        public Int16? SOI_IsDeleted { get; set; }
        
        public void Reset()
        {
            this.SOI_Number = 0;
            this.SOI_Index = 0;
            this.SOI_MS_Number = 0;
            this.SOI_ITM_Number = 0;
            this.SOI_ITM_Code = "";
            this.SOI_ITM_Group = "";
            this.SOI_ITM_Description = "";
            this.SOI_UoM_Number = "";
            this.SOI_UoM_OldNumber = "";
            this.SOI_Qty = "";
            this.SOI_UnitPrice = "";
            this.SOI_Amount = "";
            this.SOI_DeliveryDate = "";
            this.SOI_UoM_DecimalPlaces = "";
            this.SOI_IsDeleted = 0;
        }
    }
    public class SaleOrderIncome_DTO
    {
        public Int64 SOH_INC_Number { get; set; }
        public Int64 SOH_INC_SOH_Number { get; set; }

        [Display(Name = "Income Code")]
        [Required(ErrorMessage = "Income Code is Required")]
        public String? SOH_INC_MIC_Number { get; set; }

        [Display(Name = "Description")]
        public String? SOH_INC_MIC_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? SOH_INC_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? SOH_INC_OCRN_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? SOH_INC_CM_Number { get; set; }

        [Display(Name = "Income Base")]
        [Required(ErrorMessage = "Income Base is Required")]
        public String? SOH_INC_IncomeBase { get; set; }

        [Display(Name = "Income Value")]
        [Required(ErrorMessage = "Income Value is Required")]
        public String? SOH_INC_IncomeValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? SOH_INC_ALCT_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? SOH_INC_LA_Number { get; set; }
        public Int16? SOH_INC_IsDeleted { get; set; }

        public void Reset()
        {
            this.SOH_INC_Number = 0;
            this.SOH_INC_MIC_Number = "";
            this.SOH_INC_MIC_Description = "";
            this.SOH_INC_Remarks = "";
            this.SOH_INC_OCRN_Number = "";
            this.SOH_INC_CM_Number = "";
            this.SOH_INC_IncomeBase = "";
            this.SOH_INC_IncomeValue = "";
            this.SOH_INC_ALCT_Number = "";
            this.SOH_INC_LA_Number = "";
            this.SOH_INC_IsDeleted = 0;
        }
    }
    public class SaleOrderIIncome_DTO
    {
        public Int64 SOI_INC_Number { get; set; }
        public Int64 SOI_INC_SOH_Number { get; set; }
        public Int64 SOI_INC_SOI_Number { get; set; }
        public Int64 SOI_INC_ITM_Number { get; set; }
        public Int16 SOI_INC_ITM_Index { get; set; }

        [Display(Name = "Income Code")]
        [Required(ErrorMessage = "Income Code is Required")]
        public String? SOI_INC_MIC_Number { get; set; }

        [Display(Name = "Description")]
        public String? SOI_INC_MIC_Description { get; set; }

        [Display(Name = "Remarks")]
        public String? SOI_INC_Remarks { get; set; }

        [Display(Name = "Occurrence")]
        public String? SOI_INC_OCRN_Number { get; set; }

        [Display(Name = "Chargeable Method")]
        [Required(ErrorMessage = "Chargeable Method is Required")]
        public String? SOI_INC_CM_Number { get; set; }

        [Display(Name = "Income Base")]
        [Required(ErrorMessage = "Income Base is Required")]
        public String? SOI_INC_IncomeBase { get; set; }

        [Display(Name = "Income Value")]
        [Required(ErrorMessage = "Income Value is Required")]
        public String? SOI_INC_IncomeValue { get; set; }

        [Display(Name = "Allocate")]
        [Required(ErrorMessage = "Allocate is Required")]
        public String? SOI_INC_ALCT_Number { get; set; }

        [Display(Name = "Ledger Account")]
        [Required(ErrorMessage = "Ledger Account is Required")]
        public String? SOI_INC_LA_Number { get; set; }
        public Int16? SOI_INC_IsDeleted { get; set; }

        public void Reset()
        {
            this.SOI_INC_Number = 0;
            this.SOI_INC_MIC_Number = "";
            this.SOI_INC_MIC_Description = "";
            this.SOI_INC_Remarks = "";
            this.SOI_INC_OCRN_Number = "";
            this.SOI_INC_CM_Number = "";
            this.SOI_INC_IncomeBase = "";
            this.SOI_INC_IncomeValue = "";
            this.SOI_INC_ALCT_Number = "";
            this.SOI_INC_LA_Number = "";
            this.SOI_INC_IsDeleted = 0;
        }
    }
    public class SaleOrderAddress_DTO
    {
        public Int64 SOH_ADD_Number { get; set; }
        public Int64 SOH_ADD_SOH_Number { get; set; }
        public Int64 SOH_ADD_BUY_ADD_Number { get; set; }

        [Display(Name = "Address Type")]
        public Int64 SOH_ADD_ADTP_Number { get; set; }

        [Display(Name = "Address ID")]
        public String? SOH_ADD_AddressID { get; set; }

        [Display(Name = "Address")]
        public String? SOH_ADD_Address { get; set; }

        [Display(Name = "City")]
        public String? SOH_ADD_City { get; set; }

        [Display(Name = "State")]
        public String? SOH_ADD_State { get; set; }

        [Display(Name = "Country")]
        public String? SOH_ADD_Country { get; set; }

        [Display(Name = "PIN Code")]
        public String? SOH_ADD_Pin { get; set; }

        [Display(Name = "GSTIN")]
        public String? SOH_ADD_GSTIN { get; set; }

        public Int16 SOH_ADD_IsDeleted { get; set; }

        public void Reset()
        {
            this.SOH_ADD_Number = 0;
            this.SOH_ADD_SOH_Number = 0;
            this.SOH_ADD_Number = 0;
            this.SOH_ADD_ADTP_Number = 0;
            this.SOH_ADD_AddressID = "";
            this.SOH_ADD_Address = "";
            this.SOH_ADD_City = "";
            this.SOH_ADD_State = "";
            this.SOH_ADD_Country = "";
            this.SOH_ADD_Pin = "";
            this.SOH_ADD_GSTIN = "";
            this.SOH_ADD_IsDeleted = 0;
        }
    }


    public class SalesOrderRegister_DTO
    {
        public Int64 SO_Number { get; set; }
        public String? SO_RegNo { get; set; }
        public String? SO_RegDate { get; set; }
        public String? SO_OrderNo { get; set; }
        public String? SO_OrderDate { get; set; }
        public String? SO_ExportOrder { get; set; }
        public String? SO_BYC_Category { get; set; }
        public String? SO_BYG_Group { get; set; }
        public String? SO_BUY_Name { get; set; }
        public String? SO_MS_Name { get; set; }
        public String? SO_ITM_Group { get; set; }
        public String? SO_ITM_Code { get; set; }
        public String? SO_ITM_Description { get; set; }
        public String? SO_ITM_OuterDia { get; set; }
        public String? SO_ITM_MaterialGrade { get; set; }
        public String? SO_ITM_Thickness { get; set; }
        public String? SO_ITM_Length { get; set; }
        public String? SO_NoOfItem { get; set; }
        public String? SO_Qty { get; set; }
        public String? SO_UOM_Name { get; set; }
        public String? SO_DeliveryDate { get; set; }

        public String? SO_CUR_Name { get; set; }
        public String? SO_UnitPrice { get; set; }
        public String? SO_TotalAmount { get; set; }
        public String? SO_TotalItemIncome { get; set; }
        public String? SO_TotalHeadIncome { get; set; }
        public String? SO_OrderValue { get; set; }
        public String? SO_ExchangeRate { get; set; }
    }
}

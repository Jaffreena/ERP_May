using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class Item_DTO
    {
        public Int64 ItemNumber { get; set; }

        public String? ItemCode { get; set; }
        public String? ItemDescription { get; set; }
        public String? ItemPartNumber { get; set; }

        public String? InnerDia { get; set; }
        public String? OuterDia { get; set; }
        public String? Thickness { get; set; }
        public String? Length { get; set; }
        public String? Spec { get; set; }
        public String? MaterialGrade { get; set; }
        public String? ITM_Width { get; set; }

        public String? ItemGroup { get; set; }
        public String? ItemCategory { get; set; }
        public String? ItemSubCategory { get; set; }
        public String? MaterialSegregation { get; set; }

        public Int64 PurchaseWarehouse { get; set; }
        public Int64 SaleWarehouse { get; set; }

        public String? HSN_Code { get; set; }

        public String? UoM { get; set; }
        public Int64 PurchaseUnit { get; set; }
        public Int64 ProductionUnit { get; set; }
        public Int64 SaleUnit { get; set; }

        public Int64 UnitNumber { get; set; }
        public Double FromQty { get; set; }
        public Int64 FromUnit { get; set; }
        public Double ToQty { get; set; }
        public Int64 ToUnit { get; set; }

        public Int32 DecimalPlaces { get; set; }


        public Int32 CreatorCode { get; set; }
        public String? DeleteNumbers { get; set; }
        public Int32 Id { get; set; }


        public void Reset()
        {
            this.ItemNumber = 0;
            this.ItemCode = string.Empty;
            this.ItemDescription = string.Empty;
            this.ItemPartNumber = string.Empty;
            this.InnerDia = "0";
            this.OuterDia = string.Empty;
            this.Thickness = string.Empty;
            this.Length = string.Empty;
            this.Spec = string.Empty;
            this.MaterialGrade = string.Empty;
            this.ItemGroup = "0";
            this.ItemCategory = "0";
            this.ItemSubCategory = "0";
            this.MaterialSegregation = "0";
            this.PurchaseWarehouse = 0;
            this.SaleWarehouse = 0;
            this.HSN_Code = "0";
            this.UoM = "0";
            this.PurchaseUnit = 0;
            this.ProductionUnit = 0;
            this.SaleUnit = 0;
            this.UnitNumber = 0;
            this.FromQty = 0;
            this.FromUnit = 0;
            this.ToQty = 0;
            this.ToUnit = 0;
            this.DeleteNumbers = string.Empty;
        }
    }
    public class ItemHead_DTO
    {
        public Int64 ItemNumber { get; set; }

        [Display(Name = "Item Code")]
        [MaxLength(15, ErrorMessage = "Item Code be longer than 15 characters.")]
        [Required(ErrorMessage = "Item Code is Required")]
        public String? ItemCode { get; set; }

        [Display(Name = "Description")]
        [MaxLength(100, ErrorMessage = "Item Description be longer than 100 characters.")]
        [Required(ErrorMessage = "Item Description is Required")]
        public String? ItemDescription { get; set; }

        [Display(Name = "Part Number")]
        [MaxLength(25, ErrorMessage = "Item Part Number be longer than 25 characters.")]
        public String? ItemPartNumber { get; set; }

        [Display(Name = "Inner Dia")]
        [MaxLength(10, ErrorMessage = "Inner Dia be longer than 10 characters.")]
        public String? InnerDia { get; set; }

        [Display(Name = "Outer Dia")]
        [MaxLength(10, ErrorMessage = "Outer Dia be longer than 10 characters.")]
        public String? OuterDia { get; set; }

        [Display(Name = "Thickness")]
        [MaxLength(10, ErrorMessage = "Thickness be longer than 10 characters.")]
        public String? Thickness { get; set; }

        [Display(Name = "Length")]
        [MaxLength(10, ErrorMessage = "Length be longer than 10 characters.")]
        public String? Length { get; set; }

        [Display(Name = "Spec")]
        [MaxLength(25, ErrorMessage = "Spec be longer than 25 characters.")]
        public String? Spec { get; set; }

        [Display(Name = "Material Grade")]
        [MaxLength(25, ErrorMessage = "Material Grade be longer than 25 characters.")]
        public String? MaterialGrade { get; set; }

        [Display(Name = "Width")]
        [MaxLength(10, ErrorMessage = "Width be longer than 10 characters.")]
        public String? ITM_Width { get; set; }


        [Display(Name = "Item Group")]
        [Required(ErrorMessage = "Item Group is Required")]
        public String? ItemGroup { get; set; }

        [Display(Name = "Item Category")]
        public String? ItemCategory { get; set; }

        [Display(Name = "Item SubCategory")]
        public String? ItemSubCategory { get; set; }

        [Display(Name = "Material Segregation")]
        [Required(ErrorMessage = "Material Segregation is Required")]
        public String? MaterialSegregation { get; set; }


        [Display(Name = "Purchase Warehouse")]
        public String? PurchaseWarehouse { get; set; }

        [Display(Name = "Sale Warehouse")]
        public String? SaleWarehouse { get; set; }


        [Display(Name = "HSN Code")]
        public String? HSN_Code { get; set; }


        [Display(Name = "UoM")]
        [Required(ErrorMessage = "UoM is Required")]
        public String? UoM { get; set; }

        [Display(Name = "Purchase Unit")]
        public String? PurchaseUnit { get; set; }

        [Display(Name = "Production Unit")]
        public String? ProductionUnit { get; set; }

        [Display(Name = "Sale Unit")]
        public String? SaleUnit { get; set; }

        public PaginatedList_DTO<Item_DTO>? Item_List { get; set; }
        public List<ItemDetail_DTO>? ItemDetail { get; set; }

        public void Reset()
        {
            this.ItemNumber = 0;
            this.ItemCode = string.Empty;
            this.ItemDescription = string.Empty;
            this.ItemPartNumber = string.Empty;
            this.InnerDia = "0";
            this.OuterDia = string.Empty;
            this.Thickness = string.Empty;
            this.Length = string.Empty;
            this.Spec = string.Empty;
            this.MaterialGrade = string.Empty;
            this.ItemGroup = "0";
            this.ItemCategory = "0";
            this.ItemSubCategory = "0";
            this.MaterialSegregation = "0";
            this.PurchaseWarehouse = "0";
            this.SaleWarehouse = "0";
            this.HSN_Code = "0";
            this.UoM = "0";
            this.PurchaseUnit = "0";
            this.ProductionUnit = "0";
            this.SaleUnit = "0";
            this.Item_List = null;
            this.ItemDetail = null;
        }
    }
    public class ItemDetail_DTO
    {
        public Int64 UnitNumber { get; set; }

        [Display(Name = "From Qty Description")]
        [Required(ErrorMessage = "From Qty is Required")]
        public String? FromQty { get; set; }

        [Display(Name = "From Unit Description")]
        [Required(ErrorMessage = "From Unit is Required")]
        public String? FromUnit { get; set; }

        [Display(Name = "To Qty Description")]
        [Required(ErrorMessage = "To Qty is Required")]
        public String? ToQty { get; set; }

        [Display(Name = "To Unit Description")]
        [Required(ErrorMessage = "To Unit is Required")]
        public String? ToUnit { get; set; }
        public Boolean IsDeleted { get; set; }

        public void Reset()
        {
            this.UnitNumber = 0;
            this.FromQty = "0";
            this.FromUnit = "0";
            this.ToQty = "0";
            this.ToUnit = "0";
            this.IsDeleted = false;
        }
    }





    public class ItemOrder_DTO
    {
        public Int64 ItemNumber { get; set; }
        public String? ItemCode { get; set; }
        public String? ItemDescription { get; set; }

        //public String? InnerDia { get; set; }
        public String? OuterDia { get; set; }
        public String? Thickness { get; set; }
        public String? Length { get; set; }
        //public String? Spec { get; set; }
        public String? MaterialGrade { get; set; }

        public Int32 DecimalPlaces { get; set; }
        public String? ItemGroup { get; set; }
        public String? ItemCategory { get; set; }
        public String? ItemSubCategory { get; set; }
        public String? MaterialSegregation { get; set; }
        public String? HSN_Code { get; set; }
        public String? UoM { get; set; }
        public Int32 SaleWarehouse { get; set; }
    }
}

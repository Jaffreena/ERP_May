using ERP_DTO;
using System.Data;

namespace ERP.DataList
{
    public class InventoryMaster_DL
    {
        public List<Item_DTO> IList(DataTable Dt)
        {
            List<Item_DTO> IList = new List<Item_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IList.Add(
                    new Item_DTO
                    {
                        ItemNumber = Convert.ToInt64(dr["ItemNumber"]),
                        ItemCode = Convert.ToString(dr["ItemCode"]),
                        ItemDescription = Convert.ToString(dr["ItemDescription"]),
                        ItemPartNumber = Convert.ToString(dr["ItemPartNumber"]),
                        ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        ItemCategory = Convert.ToString(dr["ItemCategory"]),
                        ItemSubCategory = Convert.ToString(dr["ItemSubCategory"]),
                        MaterialSegregation = Convert.ToString(dr["MaterialSegregation"]),
                        HSN_Code = Convert.ToString(dr["HSN_Code"]),
                        UoM = Convert.ToString(dr["UoM"]),
                    });
            }
            return IList;
        }
        public List<ItemHead_DTO> IEList(DataTable Dt)
        {
            List<ItemHead_DTO> IHList = new List<ItemHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IHList.Add(
                    new ItemHead_DTO
                    {
                        ItemNumber = Convert.ToInt64(dr["ItemNumber"]),
                        ItemCode = Convert.ToString(dr["ItemCode"]),
                        ItemDescription = Convert.ToString(dr["ItemDescription"]),
                        ItemPartNumber = Convert.ToString(dr["ItemPartNumber"]),

                        InnerDia = Convert.ToString(dr["InnerDia"]),
                        OuterDia = Convert.ToString(dr["OuterDia"]),
                        Thickness = Convert.ToString(dr["Thickness"]),
                        Length = Convert.ToString(dr["Length"]),
                        Spec = Convert.ToString(dr["Spec"]),
                        MaterialGrade = Convert.ToString(dr["MaterialGrade"]),

                        ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        ItemCategory = Convert.ToString(dr["ItemCategory"]),
                        ItemSubCategory = Convert.ToString(dr["ItemSubCategory"]),
                        MaterialSegregation = Convert.ToString(dr["MaterialSegregation"]),

                        PurchaseWarehouse = Convert.ToString(dr["PurchaseWarehouse"]),
                        SaleWarehouse = Convert.ToString(dr["SaleWarehouse"]),
                        HSN_Code = Convert.ToString(dr["HSN_Code"]),

                        UoM = Convert.ToString(dr["UoM"]),
                        PurchaseUnit = Convert.ToString(dr["PurchaseUnit"]),
                        ProductionUnit = Convert.ToString(dr["ProductionUnit"]),
                        SaleUnit = Convert.ToString(dr["SaleUnit"])
                    });
            }
            return IHList;
        }
        public List<ItemDetail_DTO> IDList(DataTable Dt)
        {
            List<ItemDetail_DTO> IDList = new List<ItemDetail_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IDList.Add(
                    new ItemDetail_DTO
                    {
                        UnitNumber = Convert.ToInt64(dr["UnitNumber"]),
                        FromQty = Convert.ToString(dr["FromQty"]),
                        FromUnit = Convert.ToString(dr["FromUnit"]),
                        ToQty = Convert.ToString(dr["ToQty"]),
                        ToUnit = Convert.ToString(dr["ToUnit"]),
                    });
            }
            return IDList;
        }

        public List<ItemCategory_DTO> ICList(DataTable Dt)
        {
            List<ItemCategory_DTO> ICList = new List<ItemCategory_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ICList.Add(
                    new ItemCategory_DTO
                    {
                        ItemCategoryNumber = Convert.ToInt64(dr["ItemCategoryNumber"]),
                        ItemCategory = Convert.ToString(dr["ItemCategory"]),
                        IC_Description = Convert.ToString(dr["IC_Description"]),
                        UnderICategory = Convert.ToString(dr["UnderICategory"])
                    });
            }
            return ICList;
        }

        public List<ItemSubcategory_DTO> ISCList(DataTable Dt)
        {
            List<ItemSubcategory_DTO> ISCList = new List<ItemSubcategory_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ISCList.Add(
                    new ItemSubcategory_DTO
                    {
                        ItemSubcategoryNumber = Convert.ToInt64(dr["ItemSubcategoryNumber"]),
                        ItemSubcategory = Convert.ToString(dr["ItemSubcategory"]),
                        ISC_Description = Convert.ToString(dr["ISC_Description"]),
                        UnderISubcategory = Convert.ToString(dr["UnderISubcategory"])
                    });
            }
            return ISCList;
        }

        public List<UoM_DTO> UoMList(DataTable Dt)
        {
            List<UoM_DTO> ISCList = new List<UoM_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ISCList.Add(
                    new UoM_DTO
                    {
                        UnitNumber = Convert.ToInt64(dr["UnitNumber"]),
                        UnitCode = Convert.ToString(dr["UnitCode"]),
                        UnitDescription = Convert.ToString(dr["UnitDescription"]),
                        DecimalPlaces = Convert.ToString(dr["DecimalPlaces"])
                    });
            }
            return ISCList;
        }

        public List<Warehouse_DTO> WList(DataTable Dt)
        {
            List<Warehouse_DTO> WList = new List<Warehouse_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                WList.Add(
                    new Warehouse_DTO
                    {
                        WarehouseNumber = Convert.ToInt64(dr["WarehouseNumber"]),
                        WarehouseCode = Convert.ToString(dr["WarehouseCode"]),
                        WarehouseDescription = Convert.ToString(dr["WarehouseDescription"]),
                        WarehouseGroup = Convert.ToString(dr["WarehouseGroup"]),
                        WarehouseCategory = Convert.ToString(dr["WarehouseCategory"])
                    });
            }
            return WList;
        }

        public List<ItemGroup_DTO> IGList(DataTable Dt)
        {
            List<ItemGroup_DTO> IGList = new List<ItemGroup_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                IGList.Add(
                    new ItemGroup_DTO
                    {
                        ItemGroupNumber = Convert.ToInt64(dr["ItemGroupNumber"]),
                        ItemGroup = Convert.ToString(dr["ItemGroup"]),
                        IG_Description = Convert.ToString(dr["IG_Description"]),
                        UnderIGroup = Convert.ToString(dr["UnderIGroup"]),
                        MaterialSegregation = Convert.ToString(dr["MaterialSegregation"]),
                        MaterialOwnership = Convert.ToString(dr["MaterialOwnership"]),
                        PurchaseWarehouse = Convert.ToString(dr["PurchaseWarehouse"]),
                        SaleWarehouse = Convert.ToString(dr["SaleWarehouse"])
                    });
            }
            return IGList;
        }

        public List<ExpenseCode_DTO> ECList(DataTable Dt)
        {
            List<ExpenseCode_DTO> ECList = new List<ExpenseCode_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ECList.Add(
                    new ExpenseCode_DTO
                    {
                        ExpenseCodeNumber = Convert.ToInt64(dr["ExpenseCodeNumber"]),
                        ExpenseCode = Convert.ToString(dr["ExpenseCode"]),
                        EC_Description = Convert.ToString(dr["EC_Description"]),
                        LedgerAccount = Convert.ToString(dr["LedgerAccount"]),
                        EC_SAC_Number = Convert.ToString(dr["EC_SAC_Number"])
                    });
            }
            return ECList;
        }
    }
}

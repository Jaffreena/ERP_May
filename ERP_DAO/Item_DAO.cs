using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_DTO;

namespace ERP_DAO
{
    public class Item_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet ItemDB(Item_DTO I_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("Item_SP");
            Db.AddInParameter(DbC, "@ItemNumber", DbType.Int64, I_DTO.ItemNumber);
            Db.AddInParameter(DbC, "@ItemCode", DbType.String, I_DTO.ItemCode);
            Db.AddInParameter(DbC, "@ItemDescription", DbType.String, I_DTO.ItemDescription);

            Db.AddInParameter(DbC, "@ItemPartNumber", DbType.String, I_DTO.ItemPartNumber);
            Db.AddInParameter(DbC, "@InnerDia", DbType.String, I_DTO.InnerDia);
            Db.AddInParameter(DbC, "@OuterDia", DbType.String, I_DTO.OuterDia);
            Db.AddInParameter(DbC, "@Thickness", DbType.String, I_DTO.Thickness);
            Db.AddInParameter(DbC, "@Length", DbType.String, I_DTO.Length);
            Db.AddInParameter(DbC, "@Spec", DbType.String, I_DTO.Spec);
            Db.AddInParameter(DbC, "@MaterialGrade", DbType.String, I_DTO.MaterialGrade);

            Db.AddInParameter(DbC, "@ItemGroup", DbType.Int64, I_DTO.ItemGroup);
            Db.AddInParameter(DbC, "@ItemCategory", DbType.Int64, I_DTO.ItemCategory);
            Db.AddInParameter(DbC, "@ItemSubCategory", DbType.Int64, I_DTO.ItemSubCategory);
            Db.AddInParameter(DbC, "@MaterialSegregation", DbType.Int64, I_DTO.MaterialSegregation);
            Db.AddInParameter(DbC, "@PurchaseWarehouse", DbType.Int64, I_DTO.PurchaseWarehouse);
            Db.AddInParameter(DbC, "@SaleWarehouse", DbType.Int64, I_DTO.SaleWarehouse);
            Db.AddInParameter(DbC, "@HSN_Code", DbType.Int64, I_DTO.HSN_Code);
            Db.AddInParameter(DbC, "@UoM", DbType.Int64, I_DTO.UoM);
            Db.AddInParameter(DbC, "@PurchaseUnit", DbType.Int64, I_DTO.PurchaseUnit);
            Db.AddInParameter(DbC, "@ProductionUnit", DbType.Int64, I_DTO.ProductionUnit);
            Db.AddInParameter(DbC, "@SaleUnit", DbType.Int64, I_DTO.SaleUnit);

            Db.AddInParameter(DbC, "@UnitNumber", DbType.Int64, I_DTO.UnitNumber);
            Db.AddInParameter(DbC, "@FromQty ", DbType.Double, I_DTO.FromQty);
            Db.AddInParameter(DbC, "@FromUnit ", DbType.Int64, I_DTO.FromUnit);
            Db.AddInParameter(DbC, "@ToQty ", DbType.Double, I_DTO.ToQty);
            Db.AddInParameter(DbC, "@ToUnit ", DbType.Int64, I_DTO.ToUnit);

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, I_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, I_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, I_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

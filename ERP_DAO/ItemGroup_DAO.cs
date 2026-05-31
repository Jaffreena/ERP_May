using ERP_DTO;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO
{
    public class ItemGroup_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet ItemGroupDB(ItemGroup_DTO IG_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("ItemGroup_SP");
            Db.AddInParameter(DbC, "@ItemGroupNumber", DbType.Int64, IG_DTO.ItemGroupNumber);
            Db.AddInParameter(DbC, "@ItemGroup", DbType.String, IG_DTO.ItemGroup);
            Db.AddInParameter(DbC, "@IG_Description", DbType.String, IG_DTO.IG_Description);
            Db.AddInParameter(DbC, "@UnderIGroup", DbType.String, IG_DTO.UnderIGroup);
            Db.AddInParameter(DbC, "@MaterialOwnership", DbType.String, IG_DTO.MaterialOwnership);
            Db.AddInParameter(DbC, "@PurchaseWarehouse", DbType.String, IG_DTO.PurchaseWarehouse);
            Db.AddInParameter(DbC, "@SaleWarehouse", DbType.String, IG_DTO.SaleWarehouse);
            Db.AddInParameter(DbC, "@MaterialSegregation", DbType.String, IG_DTO.MaterialSegregation);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, IG_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, IG_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, IG_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }

    }
}

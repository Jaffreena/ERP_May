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
    public class ItemCategory_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet ItemCategoryDB(ItemCategory_DTO IC_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("ItemCategory_SP");
            Db.AddInParameter(DbC, "@ItemCategoryNumber", DbType.Int64, IC_DTO.ItemCategoryNumber);
            Db.AddInParameter(DbC, "@ItemCategory", DbType.String, IC_DTO.ItemCategory);
            Db.AddInParameter(DbC, "@IC_Description", DbType.String, IC_DTO.IC_Description);
            Db.AddInParameter(DbC, "@UnderICategory", DbType.Int64, IC_DTO.UnderICategory);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, IC_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, IC_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, IC_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_DTO;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ERP_DAO
{
    public class InventorySubmaster_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet InventorySubmasterDB(InventorySubmaster_DTO IS_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("InventorySubmaster_SP");
            Db.AddInParameter(DbC, "@Number", DbType.Int64, IS_DTO.Number);
            Db.AddInParameter(DbC, "@Title", DbType.String, IS_DTO.Title);
            Db.AddInParameter(DbC, "@Notes", DbType.String, IS_DTO.Notes);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, IS_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, IS_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, IS_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

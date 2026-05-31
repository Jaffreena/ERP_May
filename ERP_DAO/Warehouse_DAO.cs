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
    public class Warehouse_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet WarehouseDB(Warehouse_DTO WH_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("Warehouse_SP");
            Db.AddInParameter(DbC, "@WarehouseNumber", DbType.Int64, WH_DTO.WarehouseNumber);
            Db.AddInParameter(DbC, "@WarehouseCategory", DbType.Int64, WH_DTO.WarehouseCategory);
            Db.AddInParameter(DbC, "@WarehouseCode", DbType.String, WH_DTO.WarehouseCode);
            Db.AddInParameter(DbC, "@WarehouseDescription", DbType.String, WH_DTO.WarehouseDescription);
            Db.AddInParameter(DbC, "@WarehouseGroup", DbType.Int64, WH_DTO.WarehouseGroup);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, WH_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, WH_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, WH_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }


    }
}

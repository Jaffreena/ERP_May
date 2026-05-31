using ERP_DTO;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO
{
    public class HSN_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet HSNDB(HSN_DTO H_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("HSN_SP");
            Db.AddInParameter(DbC, "@HSN_Number", DbType.Int64, H_DTO.HSN_Number);
            Db.AddInParameter(DbC, "@HSN_Code", DbType.String, H_DTO.HSN_Code);
            Db.AddInParameter(DbC, "@Description", DbType.String, H_DTO.Description);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, H_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, H_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, H_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

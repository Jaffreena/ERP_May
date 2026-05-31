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
    public class UserLog_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet UserDb(UserLog_DTO UL_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("User_Sp");
            Db.AddInParameter(DbC, "@Number", DbType.Int64, UL_DTO.Number);
            Db.AddInParameter(DbC, "@Username", DbType.String, UL_DTO.Username);
            Db.AddInParameter(DbC, "@Password", DbType.String, UL_DTO.Password);
            Db.AddInParameter(DbC, "@Theme", DbType.Int32, UL_DTO.Theme);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, UL_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, UL_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

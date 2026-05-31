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
    public class SAC_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet SACDB(SAC_DTO S_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("SAC_SP");
            Db.AddInParameter(DbC, "@SAC_Number", DbType.Int64, S_DTO.SAC_Number);
            Db.AddInParameter(DbC, "@SAC_Code", DbType.String, S_DTO.SAC_Code);
            Db.AddInParameter(DbC, "@Description", DbType.String, S_DTO.Description);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, S_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, S_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, S_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

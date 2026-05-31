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
    public class ExpenseCode_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet ExpenseCodeDB(ExpenseCode_DTO E_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("ExpenseCode_SP");
            Db.AddInParameter(DbC, "@ExpenseCodeNumber", DbType.Int64, E_DTO.ExpenseCodeNumber);
            Db.AddInParameter(DbC, "@ExpenseCode", DbType.String, E_DTO.ExpenseCode);
            Db.AddInParameter(DbC, "@EC_Description", DbType.String, E_DTO.EC_Description);
            Db.AddInParameter(DbC, "@LedgerAccount", DbType.Int64, E_DTO.LedgerAccount);
            Db.AddInParameter(DbC, "@EC_SAC_Number", DbType.Int64, E_DTO.EC_SAC_Number);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, E_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, E_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, E_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

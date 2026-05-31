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
    public class COA_Ledger_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet COALedgerDB(COA_Ledger_DTO CA_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("COA_Ledger_SP");
            Db.AddInParameter(DbC, "@LedgerNumber", DbType.Int64, CA_DTO.LedgerNumber);
            Db.AddInParameter(DbC, "@LedgerAccount", DbType.String, CA_DTO.LedgerAccount);
            Db.AddInParameter(DbC, "@LedgerName", DbType.String, CA_DTO.LedgerName);
            Db.AddInParameter(DbC, "@LedgerGroup ", DbType.String, CA_DTO.LedgerGroup);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, CA_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, CA_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, CA_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }

    }
}

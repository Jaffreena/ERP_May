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
    public class GeneralLedgerSubmaster_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet GeneralLedgerSubmasterDB(GeneralLedgerSubmaster_DTO GL_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("GeneralLedgerSubmaster_SP");
            Db.AddInParameter(DbC, "@Number", DbType.Int64, GL_DTO.Number);
            Db.AddInParameter(DbC, "@Title", DbType.String, GL_DTO.Title);
            Db.AddInParameter(DbC, "@Notes", DbType.String, GL_DTO.Notes);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, GL_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, GL_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, GL_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

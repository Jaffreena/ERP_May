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
    public class ProcurementSubmaster_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet ProcurementSubmasterDB(ProcurementSubmaster_DTO PS_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("ProcurementSubmaster_SP");
            Db.AddInParameter(DbC, "@Number", DbType.Int64, PS_DTO.Number);
            Db.AddInParameter(DbC, "@Title", DbType.String, PS_DTO.Title);
            Db.AddInParameter(DbC, "@Notes", DbType.String, PS_DTO.Notes);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, PS_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, PS_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, PS_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

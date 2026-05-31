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
    public class TaxationSubmaster_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet TaxationSubmasterDB(TaxationSubmaster_DTO TS_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("TaxationSubmaster_SP");
            Db.AddInParameter(DbC, "@Number", DbType.Int64, TS_DTO.Number);
            Db.AddInParameter(DbC, "@Location", DbType.Int64, TS_DTO.Location);
            Db.AddInParameter(DbC, "@Title", DbType.String, TS_DTO.Title);
            Db.AddInParameter(DbC, "@Notes", DbType.String, TS_DTO.Notes);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, TS_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, TS_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, TS_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

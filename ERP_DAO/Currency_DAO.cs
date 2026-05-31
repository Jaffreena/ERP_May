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
    public class Currency_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet CurrencyDB(Currency_DTO C_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("Currency_SP");
            Db.AddInParameter(DbC, "@CurrencyNumber", DbType.Int64, C_DTO.CurrencyNumber);
            Db.AddInParameter(DbC, "@CurrencyCode", DbType.String, C_DTO.CurrencyCode);
            Db.AddInParameter(DbC, "@FormalName", DbType.String, C_DTO.FormalName);
            Db.AddInParameter(DbC, "@Symbol", DbType.String, C_DTO.Symbol);
            Db.AddInParameter(DbC, "@DecimalPlaces", DbType.Int32, C_DTO.DecimalPlaces);
            Db.AddInParameter(DbC, "@DecimalPortionName", DbType.String, C_DTO.DecimalPortionName);
            Db.AddInParameter(DbC, "@CurrencyLocation", DbType.Int64, Convert.ToInt64(C_DTO.CurrencyLocation));
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, C_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, C_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, C_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

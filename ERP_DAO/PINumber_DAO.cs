using ERP_DTO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO
{
    public class PINumber_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet PINumberDB(PINumber_DTO P_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("PINumbering_SP");
            Db.AddInParameter(DbC, "@PIN_Number", DbType.Int64, P_DTO.PIN_Number);
            Db.AddInParameter(DbC, "@PIN_Method", DbType.Int64, P_DTO.PIN_Method);
            Db.AddInParameter(DbC, "@PIN_Date", DbType.Int32, P_DTO.PIN_Date);
            Db.AddInParameter(DbC, "@PIN_StartingNumber", DbType.Int32, P_DTO.PIN_StartingNumber);
            Db.AddInParameter(DbC, "@PIN_NumberofDigits", DbType.Int32, P_DTO.PIN_NumberofDigits);
            Db.AddInParameter(DbC, "@PIN_PrefilZero", DbType.Int64, P_DTO.PIN_PrefilZero);
            Db.AddInParameter(DbC, "@PIN_Frequency", DbType.Int64, P_DTO.PIN_Frequency);
            Db.AddInParameter(DbC, "@PIN_Particulars", DbType.String, P_DTO.PIN_Particulars);

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, P_DTO.DeleteNumbers);

            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, P_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, P_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

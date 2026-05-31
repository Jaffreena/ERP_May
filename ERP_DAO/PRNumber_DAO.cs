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
    public class PRNumber_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet PRNumberDB(PRNumber_DTO P_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("PRNumbering_SP");
            Db.AddInParameter(DbC, "@PRN_Number", DbType.Int64, P_DTO.PRN_Number);
            Db.AddInParameter(DbC, "@PRN_Method", DbType.Int64, P_DTO.PRN_Method);
            Db.AddInParameter(DbC, "@PRN_Date", DbType.Int32, P_DTO.PRN_Date);
            Db.AddInParameter(DbC, "@PRN_StartingNumber", DbType.Int32, P_DTO.PRN_StartingNumber);
            Db.AddInParameter(DbC, "@PRN_NumberofDigits", DbType.Int32, P_DTO.PRN_NumberofDigits);
            Db.AddInParameter(DbC, "@PRN_PrefilZero", DbType.Int64, P_DTO.PRN_PrefilZero);
            Db.AddInParameter(DbC, "@PRN_Frequency", DbType.Int64, P_DTO.PRN_Frequency);
            Db.AddInParameter(DbC, "@PRN_Particulars", DbType.String, P_DTO.PRN_Particulars);

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, P_DTO.DeleteNumbers);

            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, P_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, P_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

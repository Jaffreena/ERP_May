using ERP_DTO;
using ERP_DTO.JobInwardTransaction;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO.JobInwardTransaction
{
    public class RNNumber_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet RNNumberDB(RNNumber_DTO P_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("RNNumbering_SP");
            Db.AddInParameter(DbC, "@RNN_Number", DbType.Int64, P_DTO.RNN_Number);
            Db.AddInParameter(DbC, "@RNN_Method", DbType.Int64, P_DTO.RNN_Method);
            Db.AddInParameter(DbC, "@RNN_Date", DbType.Int32, P_DTO.RNN_Date);
            Db.AddInParameter(DbC, "@RNN_StartingNumber", DbType.Int32, P_DTO.RNN_StartingNumber);
            Db.AddInParameter(DbC, "@RNN_NumberofDigits", DbType.Int32, P_DTO.RNN_NumberofDigits);
            Db.AddInParameter(DbC, "@RNN_PrefilZero", DbType.Int64, P_DTO.RNN_PrefilZero);
            Db.AddInParameter(DbC, "@RNN_Frequency", DbType.Int64, P_DTO.RNN_Frequency);
            Db.AddInParameter(DbC, "@RNN_Particulars", DbType.String, P_DTO.RNN_Particulars);

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, P_DTO.DeleteNumbers);

            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, P_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, P_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

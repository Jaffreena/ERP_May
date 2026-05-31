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
    public class PONumber_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet PONumberDB(PONumber_DTO P_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("PONumbering_SP");
            Db.AddInParameter(DbC, "@PON_Number", DbType.Int64, P_DTO.PON_Number);
            Db.AddInParameter(DbC, "@PON_Method", DbType.Int64, P_DTO.PON_Method);
            Db.AddInParameter(DbC, "@PON_Date", DbType.Int32, P_DTO.PON_Date);
            Db.AddInParameter(DbC, "@PON_StartingNumber", DbType.Int32, P_DTO.PON_StartingNumber);
            Db.AddInParameter(DbC, "@PON_NumberofDigits", DbType.Int32, P_DTO.PON_NumberofDigits);
            Db.AddInParameter(DbC, "@PON_PrefilZero", DbType.Int64, P_DTO.PON_PrefilZero);
            Db.AddInParameter(DbC, "@PON_Frequency", DbType.Int64, P_DTO.PON_Frequency);
            Db.AddInParameter(DbC, "@PON_Particulars", DbType.String, P_DTO.PON_Particulars);

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, P_DTO.DeleteNumbers);

            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, P_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, P_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

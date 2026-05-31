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
    public class SRNumber_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet SRNumberDB(SRNumber_DTO P_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("SRNumbering_SP");
            Db.AddInParameter(DbC, "@SRN_Number", DbType.Int64, P_DTO.SRN_Number);
            Db.AddInParameter(DbC, "@SRN_Method", DbType.Int64, P_DTO.SRN_Method);
            Db.AddInParameter(DbC, "@SRN_Date", DbType.Int32, P_DTO.SRN_Date);
            Db.AddInParameter(DbC, "@SRN_StartingNumber", DbType.Int32, P_DTO.SRN_StartingNumber);
            Db.AddInParameter(DbC, "@SRN_NumberofDigits", DbType.Int32, P_DTO.SRN_NumberofDigits);
            Db.AddInParameter(DbC, "@SRN_PrefilZero", DbType.Int64, P_DTO.SRN_PrefilZero);
            Db.AddInParameter(DbC, "@SRN_Frequency", DbType.Int64, P_DTO.SRN_Frequency);
            Db.AddInParameter(DbC, "@SRN_Particulars", DbType.String, P_DTO.SRN_Particulars);

            Db.AddInParameter(DbC, "@SRN_DeleteNumbers", DbType.String, P_DTO.SRN_DeleteNumbers);

            Db.AddInParameter(DbC, "@SRN_CreatorCode", DbType.Int32, P_DTO.SRN_CreatorCode);
            Db.AddInParameter(DbC, "@SRN_Id", DbType.Int32, P_DTO.SRN_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

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
    public class SINumber_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet SINumberDB(SINumber_DTO P_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("SINumbering_SP");
            Db.AddInParameter(DbC, "@SIN_Number", DbType.Int64, P_DTO.SIN_Number);
            Db.AddInParameter(DbC, "@SIN_Method", DbType.Int64, P_DTO.SIN_Method);
            Db.AddInParameter(DbC, "@SIN_Date", DbType.Int32, P_DTO.SIN_Date);
            Db.AddInParameter(DbC, "@SIN_StartingNumber", DbType.Int32, P_DTO.SIN_StartingNumber);
            Db.AddInParameter(DbC, "@SIN_NumberofDigits", DbType.Int32, P_DTO.SIN_NumberofDigits);
            Db.AddInParameter(DbC, "@SIN_PrefilZero", DbType.Int64, P_DTO.SIN_PrefilZero);
            Db.AddInParameter(DbC, "@SIN_Frequency", DbType.Int64, P_DTO.SIN_Frequency);
            Db.AddInParameter(DbC, "@SIN_Particulars", DbType.String, P_DTO.SIN_Particulars);

            Db.AddInParameter(DbC, "@SIN_DeleteNumbers", DbType.String, P_DTO.SIN_DeleteNumbers);

            Db.AddInParameter(DbC, "@SIN_CreatorCode", DbType.Int32, P_DTO.SIN_CreatorCode);
            Db.AddInParameter(DbC, "@SIN_Id", DbType.Int32, P_DTO.SIN_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

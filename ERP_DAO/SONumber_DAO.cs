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
    public class SONumber_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet SONumberDB(SONumber_DTO P_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("SONumbering_SP");
            Db.AddInParameter(DbC, "@SON_Number", DbType.Int64, P_DTO.SON_Number);
            Db.AddInParameter(DbC, "@SON_Method", DbType.Int64, P_DTO.SON_Method);
            Db.AddInParameter(DbC, "@SON_Date", DbType.Int32, P_DTO.SON_Date);
            Db.AddInParameter(DbC, "@SON_StartingNumber", DbType.Int32, P_DTO.SON_StartingNumber);
            Db.AddInParameter(DbC, "@SON_NumberofDigits", DbType.Int32, P_DTO.SON_NumberofDigits);
            Db.AddInParameter(DbC, "@SON_PrefilZero", DbType.Int64, P_DTO.SON_PrefilZero);
            Db.AddInParameter(DbC, "@SON_Frequency", DbType.Int64, P_DTO.SON_Frequency);
            Db.AddInParameter(DbC, "@SON_Particulars", DbType.String, P_DTO.SON_Particulars);

            Db.AddInParameter(DbC, "@SON_DeleteNumbers", DbType.String, P_DTO.SON_DeleteNumbers);

            Db.AddInParameter(DbC, "@SON_CreatorCode", DbType.Int32, P_DTO.SON_CreatorCode);
            Db.AddInParameter(DbC, "@SON_Id", DbType.Int32, P_DTO.SON_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

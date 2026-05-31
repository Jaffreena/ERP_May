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
    public class WorkCentreGroup_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet WorkCentreGroupDB(WorkCentreGroup_DTO WCG_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("JW_WorkCentreGroup_SP");

            // 🔽 Parameters mapping
            Db.AddInParameter(DbC, "@WCG_Number", DbType.Int64, WCG_DTO.WCG_Number);
            Db.AddInParameter(DbC, "@WCG_WorkCentreGroup", DbType.String, WCG_DTO.WCG_WorkCentreGroup);
            Db.AddInParameter(DbC, "@WCG_Description", DbType.String, WCG_DTO.WCG_Description);
            Db.AddInParameter(DbC, "@WCG_Under_WCG_Number", DbType.Int64, WCG_DTO.WCG_Under_WCG_Number);
            Db.AddInParameter(DbC, "@WCG_DeleteNumbers", DbType.String, WCG_DTO.WCG_DeleteNumbers);
            Db.AddInParameter(DbC, "@WCG_CreatorCode", DbType.Int64, WCG_DTO.WCG_CreatorCode);
            Db.AddInParameter(DbC, "@WCG_Id", DbType.Int32, WCG_DTO.WCG_Id);

            // 🔽 Execute SP
            DS = Db.ExecuteDataSet(DbC);

            return DS;
        }
    }
}

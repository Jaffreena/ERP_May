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
    public class WorkCentre_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet WorkCentreDB(WorkCentre_DTO WC_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("WorkCentre_SP");

            // 🔽 Parameters mapping
            Db.AddInParameter(DbC, "@WC_Number", DbType.Int64, WC_DTO.WC_Number);
            Db.AddInParameter(DbC, "@WC_WorkCentre", DbType.String, WC_DTO.WC_WorkCentre);
            Db.AddInParameter(DbC, "@WC_Description", DbType.String, WC_DTO.WC_Description);

            Db.AddInParameter(DbC, "@WC_WCG_Number", DbType.Int64, WC_DTO.WC_WCG_Number);
            Db.AddInParameter(DbC, "@WC_Warehouse_Number", DbType.Int64, WC_DTO.WC_Warehouse_Number);
            Db.AddInParameter(DbC, "@WC_PRS_Number", DbType.Int64, WC_DTO.WC_PRS_Number);

            Db.AddInParameter(DbC, "@WC_DeleteNumbers", DbType.String, WC_DTO.WC_DeleteNumbers);
            Db.AddInParameter(DbC, "@WC_CreatorCode", DbType.Int64, WC_DTO.WC_CreatorCode);
            Db.AddInParameter(DbC, "@WC_Id", DbType.Int32, WC_DTO.WC_Id);

            // 🔽 Execute SP
            DS = Db.ExecuteDataSet(DbC);

            return DS;
        }
    }
}

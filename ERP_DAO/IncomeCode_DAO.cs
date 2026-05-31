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
    public class IncomeCode_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet IncomeCodeDB(IncomeCode_DTO IC_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("IncomeCode_SP");
            Db.AddInParameter(DbC, "@MIC_Number",DbType.Int64, IC_DTO.MIC_Number);
            Db.AddInParameter(DbC, "@MIC_Code", DbType.String, IC_DTO.MIC_Code);
            Db.AddInParameter(DbC, "@MIC_Description", DbType.String, IC_DTO.MIC_Description);
            Db.AddInParameter(DbC, "@MIC_OCRN_Number", DbType.Int64,IC_DTO.MIC_OCRN_Number);
            Db.AddInParameter(DbC, "@MIC_CM_Number", DbType.Int64,IC_DTO.MIC_CM_Number);
            Db.AddInParameter(DbC, "@MIC_ALCT_Number", DbType.Int64,IC_DTO.MIC_ALCT_Number);
            Db.AddInParameter(DbC, "@MIC_LA_Number", DbType.Int64, IC_DTO.MIC_LA_Number);
            Db.AddInParameter(DbC, "@MIC_SAC_Number", DbType.Int64, IC_DTO.MIC_SAC_Number);
            Db.AddInParameter(DbC, "@MIC_DeleteNumbers", DbType.String, IC_DTO.MIC_DeleteNumbers);
            Db.AddInParameter(DbC, "@MIC_CreatorCode", DbType.Int64, IC_DTO.MIC_CreatorCode);
            Db.AddInParameter(DbC, "@MIC_Id", DbType.Int16, IC_DTO.MIC_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;


     }  }
}

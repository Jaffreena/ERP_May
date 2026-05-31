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
    public class JW_Process_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet JW_ProcessDB(JW_Process_DTO WH_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("JW_Process_SP");
            Db.AddInParameter(DbC, "@ProcessNumber", DbType.Int64, WH_DTO.ProcessNumber);
         
            Db.AddInParameter(DbC, "@ProcessName", DbType.String, WH_DTO.ProcessName);
            Db.AddInParameter(DbC, "@Description", DbType.String, WH_DTO.Description);
            Db.AddInParameter(DbC, "@ProductionUoM", DbType.Int64, WH_DTO.ProductionUoM);
            Db.AddInParameter(DbC, "@ConsumptionUoM", DbType.Int64, WH_DTO.ConsumptionUoM);
            Db.AddInParameter(DbC, "@ScrapUoM", DbType.Int64, WH_DTO.ScrapUoM);
            Db.AddInParameter(DbC, "@ScrapItemCode", DbType.Int64, WH_DTO.ScrapItemCode);
            Db.AddInParameter(DbC, "@SAC", DbType.Int64, WH_DTO.SAC);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, WH_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, WH_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, WH_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

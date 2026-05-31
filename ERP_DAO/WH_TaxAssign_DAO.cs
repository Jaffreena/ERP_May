using ERP_DTO;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO
{
    public class WH_TaxAssign_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet WHTaxAssignDB(WH_TaxAssign_DTO WTA_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("WH_TaxAssign_SP");
            Db.AddInParameter(DbC, "@WH_TaxNumber", DbType.Int64, WTA_DTO.WH_TaxNumber);
            Db.AddInParameter(DbC, "@WH_TaxCode", DbType.Int64,Convert.ToInt64(WTA_DTO.WH_TaxCode));
            Db.AddInParameter(DbC, "@WH_TaxDescription", DbType.String, WTA_DTO.WH_TaxDescription);

            Db.AddInParameter(DbC, "@WH_TaxAssignNumber", DbType.Int64, Convert.ToInt64(WTA_DTO.WH_TaxAssignNumber));
            Db.AddInParameter(DbC, "@AssesseeNature", DbType.Int64, Convert.ToInt64(WTA_DTO.AssesseeNature));
            Db.AddInParameter(DbC, "@FromDate", DbType.Int32, Convert.ToInt64(WTA_DTO.FromDate));
            Db.AddInParameter(DbC, "@ToDate", DbType.Int32, Convert.ToInt64(WTA_DTO.ToDate));
            Db.AddInParameter(DbC, "@SingleTransLimit", DbType.Double, Convert.ToDouble(WTA_DTO.SingleTransLimit));
            Db.AddInParameter(DbC, "@AggregateTransLimit", DbType.Double, Convert.ToDouble(WTA_DTO.AggregateTransLimit));
            Db.AddInParameter(DbC, "@IncludeTax", DbType.Int64, Convert.ToInt64(WTA_DTO.IncludeTax));
            Db.AddInParameter(DbC, "@PAN_TaxPercent", DbType.Double, Convert.ToDouble(WTA_DTO.PAN_TaxPercent));
            Db.AddInParameter(DbC, "@NON_PAN_TaxPercent", DbType.Double, Convert.ToDouble(WTA_DTO.NON_PAN_TaxPercent));
            Db.AddInParameter(DbC, "@COA", DbType.Int64, Convert.ToInt64(WTA_DTO.COA));

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, WTA_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, WTA_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, WTA_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

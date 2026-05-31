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
    public class TaxElement_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet TaxElementDB(TaxElement_DTO TE_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("TaxElement_SP");
            Db.AddInParameter(DbC, "@TaxElementNumber", DbType.Int64, TE_DTO.TaxElementNumber);
            Db.AddInParameter(DbC, "@TaxNumber", DbType.Int64, TE_DTO.TaxNumber);
            Db.AddInParameter(DbC, "@TaxElement", DbType.String, TE_DTO.TaxElement);
            Db.AddInParameter(DbC, "@ElementDescription", DbType.String, TE_DTO.ElementDescription);
            Db.AddInParameter(DbC, "@TaxCategory", DbType.Int64, Convert.ToInt64(TE_DTO.TaxCategory));
            Db.AddInParameter(DbC, "@TaxType", DbType.Int64, Convert.ToInt64(TE_DTO.TaxType));

            Db.AddInParameter(DbC, "@LoadonInventory", DbType.Int32, TE_DTO.LoadonInventory);
            Db.AddInParameter(DbC, "@LoadonInventoryPercent", DbType.Double, Convert.ToDouble(TE_DTO.LoadonInventoryPercent));
            Db.AddInParameter(DbC, "@COA_LedgerAccount", DbType.Int64, Convert.ToInt64(TE_DTO.COA_LedgerAccount));
            Db.AddInParameter(DbC, "@GST_Abatement", DbType.Int64, Convert.ToInt64(TE_DTO.GST_Abatement));
            Db.AddInParameter(DbC, "@GST_TaxNature", DbType.Int64, Convert.ToInt64(TE_DTO.GST_TaxNature));

            Db.AddInParameter(DbC, "@FromDate", DbType.Int32, TE_DTO.FromDate);
            Db.AddInParameter(DbC, "@ToDate", DbType.Int32, TE_DTO.ToDate);
            Db.AddInParameter(DbC, "@FixedPercent", DbType.Double, TE_DTO.FixedPercent);

            Db.AddInParameter(DbC, "@HSN", DbType.Int64, TE_DTO.HSN);
            Db.AddInParameter(DbC, "@HSNPercent", DbType.Double, TE_DTO.HSNPercent);
            Db.AddInParameter(DbC, "@SAC", DbType.Int64, TE_DTO.SAC);
            Db.AddInParameter(DbC, "@SACPercent", DbType.Double, TE_DTO.SACPercent);

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, TE_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, TE_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, TE_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

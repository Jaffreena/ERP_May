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
    public class TaxCluster_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet TaxClusterDB(TaxCluster_DTO TC_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("TaxCluster_SP");
            Db.AddInParameter(DbC, "@TaxClusterNumber", DbType.Int64, TC_DTO.TaxClusterNumber);
            Db.AddInParameter(DbC, "@TaxCluster", DbType.String, TC_DTO.TaxCluster);
            Db.AddInParameter(DbC, "@ClusterDescription", DbType.String, TC_DTO.ClusterDescription);
            Db.AddInParameter(DbC, "@GST_Category", DbType.Int64, Convert.ToInt64(TC_DTO.GST_Category));
            Db.AddInParameter(DbC, "@GST_Type", DbType.Int64, Convert.ToInt64(TC_DTO.GST_Type));

            Db.AddInParameter(DbC, "@TaxIndex", DbType.Int32, Convert.ToInt64(TC_DTO.TaxIndex));
            Db.AddInParameter(DbC, "@TaxClusterDetailsNumber", DbType.Int64, Convert.ToInt64(TC_DTO.@TaxClusterDetailsNumber));
            Db.AddInParameter(DbC, "@TaxElement", DbType.Int64, Convert.ToInt64(TC_DTO.TaxElement));
            Db.AddInParameter(DbC, "@ChargeableBasis", DbType.Int64, Convert.ToInt64(TC_DTO.ChargeableBasis));
            Db.AddInParameter(DbC, "@CalculationFactors", DbType.String, TC_DTO.CalculationFactors);

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, TC_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, TC_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, TC_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

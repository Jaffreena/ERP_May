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
    public class COA_Group_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet COAGroupDB(COA_Group_DTO COA_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("COA_Group_SP");
            Db.AddInParameter(DbC, "@LedgerGroupNumber", DbType.Int64, COA_DTO.LedgerGroupNumber);
            Db.AddInParameter(DbC, "@LedgerGroup", DbType.String, COA_DTO.LedgerGroup);
            Db.AddInParameter(DbC, "@UnderLGroup", DbType.Int64, COA_DTO.UnderLGroup);
            Db.AddInParameter(DbC, "@GroupNature", DbType.Int64, COA_DTO.GroupNature);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, COA_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, COA_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, COA_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

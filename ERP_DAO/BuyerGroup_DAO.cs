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
    public class BuyerGroup_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet BuyerGroupDB(BuyerGroup_DTO BG_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("BuyerGroup_SP");
            Db.AddInParameter(DbC, "@BYG_Number", DbType.Int64, BG_DTO.BYG_Number);
            Db.AddInParameter(DbC, "@BYG_Group", DbType.String, BG_DTO.BYG_Group);
            Db.AddInParameter(DbC, "@BYG_Description", DbType.String, BG_DTO.BYG_Description);
            Db.AddInParameter(DbC, "@BYG_Under_BYG_Number", DbType.Int64, BG_DTO.BYG_Under_BYG_Number);
            Db.AddInParameter(DbC, "@BYG_DeleteNumbers", DbType.String, BG_DTO.BYG_DeleteNumbers);
            Db.AddInParameter(DbC, "@BYG_CreatorCode", DbType.Int64, BG_DTO.BYG_CreatorCode);
            Db.AddInParameter(DbC, "@BYG_Id", DbType.Int16, BG_DTO.BYG_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

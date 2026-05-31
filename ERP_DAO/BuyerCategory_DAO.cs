using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_DTO;

namespace ERP_DAO
{
    public class BuyerCategory_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet BuyerCategoryDB(BuyerCategory_DTO BYC_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("BuyerCategory_SP");
            Db.AddInParameter(DbC, "@BYC_Number", DbType.Int64, BYC_DTO.BYC_Number);
            Db.AddInParameter(DbC, "@BYC_Category", DbType.String, BYC_DTO.BYC_Category);
            Db.AddInParameter(DbC, "@BYC_Description", DbType.String, BYC_DTO.BYC_Description);
            Db.AddInParameter(DbC, "@BYC_Under_BYC_Number", DbType.Int64, BYC_DTO.BYC_Under_BYC_Number);
            Db.AddInParameter(DbC, "@BYC_DeleteNumbers", DbType.String, BYC_DTO.BYC_DeleteNumbers);
            Db.AddInParameter(DbC, "@BYC_CreatorCode", DbType.Int64, BYC_DTO.BYC_CreatorCode);
            Db.AddInParameter(DbC, "@BYC_Id", DbType.Int16, BYC_DTO.BYC_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

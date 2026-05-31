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
    public class CustomerCategory_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet CustomerCategoryDB(CustomerCategory_DTO BYC_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("JW_CustomerCategory_SP");
            Db.AddInParameter(DbC, "@JCC_Number", DbType.Int64, BYC_DTO.JCC_Number);
            Db.AddInParameter(DbC, "@JCC_Category", DbType.String, BYC_DTO.JCC_JW_CustomerCategory);
            Db.AddInParameter(DbC, "@JCC_Description", DbType.String, BYC_DTO.JCC_Description);
            Db.AddInParameter(DbC, "@JCC_Under_JCC_Number", DbType.Int64, BYC_DTO.JCC_Under_JCC_Number);
            Db.AddInParameter(DbC, "@JCC_DeleteNumbers", DbType.String, BYC_DTO.JCC_DeleteNumbers);
            Db.AddInParameter(DbC, "@JCC_CreatorCode", DbType.Int64, BYC_DTO.JCC_CreatorCode);
            Db.AddInParameter(DbC, "@JCC_Id", DbType.Int16, BYC_DTO.JCC_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

using ERP_DTO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;

namespace ERP_DAO
{
    public class JW_CustomerGroup_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet JWCustomerGroupDB(CustomerGroup_DTO JCG_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("JW_CustomerGroup_SP");

            // 🔽 Parameters mapping
            Db.AddInParameter(DbC, "@JCG_Number", DbType.Int64, JCG_DTO.JCG_Number);
            Db.AddInParameter(DbC, "@JCG_JW_CustomerGroup", DbType.String, JCG_DTO.JCG_JW_CustomerGroup);
            Db.AddInParameter(DbC, "@JCG_Description", DbType.String, JCG_DTO.JCG_Description);
            Db.AddInParameter(DbC, "@JCG_Under_JCG_Number", DbType.Int64, JCG_DTO.JCG_Under_JCG_Number);
            Db.AddInParameter(DbC, "@JCG_DeleteNumbers", DbType.String, JCG_DTO.JCG_DeleteNumbers);
            Db.AddInParameter(DbC, "@JCG_CreatorCode", DbType.Int32, JCG_DTO.JCG_CreatorCode);
            Db.AddInParameter(DbC, "@JCG_Id", DbType.Int32, JCG_DTO.JCG_Id);

            // 🔽 Execute SP
            DS = Db.ExecuteDataSet(DbC);

            return DS;
        }
    }
}
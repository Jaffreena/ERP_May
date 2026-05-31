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
    public class ItemSubcategory_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet ItemSubcategoryDB(ItemSubcategory_DTO ISC_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("ItemSubcategory_SP");
            Db.AddInParameter(DbC, "@ItemSubcategoryNumber", DbType.Int64, ISC_DTO.ItemSubcategoryNumber);
            Db.AddInParameter(DbC, "@ItemSubcategory", DbType.String, ISC_DTO.ItemSubcategory);
            Db.AddInParameter(DbC, "@ISC_Description", DbType.String, ISC_DTO.ISC_Description);
            Db.AddInParameter(DbC, "@UnderISubcategory", DbType.Int64, ISC_DTO.UnderISubcategory);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, ISC_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, ISC_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, ISC_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

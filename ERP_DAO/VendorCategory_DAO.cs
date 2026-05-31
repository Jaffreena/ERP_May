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
    public class VendorCategory_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet VendorCategoryDB(VendorCategory_DTO VC_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("VendorCategory_SP");
            Db.AddInParameter(DbC, "@VendorCategoryNumber", DbType.Int64, VC_DTO.VendorCategoryNumber);
            Db.AddInParameter(DbC, "@VendorCategory", DbType.String, VC_DTO.VendorCategory);
            Db.AddInParameter(DbC, "@VC_Description", DbType.String, VC_DTO.VC_Description);
            Db.AddInParameter(DbC, "@UnderVCategory", DbType.Int64, VC_DTO.UnderVCategory);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, VC_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, VC_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, VC_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

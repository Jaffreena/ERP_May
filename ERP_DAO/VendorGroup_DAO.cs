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
    public class VendorGroup_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet VendorGroupDB(VendorGroup_DTO VG_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("VendorGroup_SP");
            Db.AddInParameter(DbC, "@VendorGroupNumber", DbType.Int64, VG_DTO.VendorGroupNumber);
            Db.AddInParameter(DbC, "@VendorGroup", DbType.String, VG_DTO.VendorGroup);
            Db.AddInParameter(DbC, "@VG_Description", DbType.String, VG_DTO.VG_Description);
            Db.AddInParameter(DbC, "@UnderVGroup", DbType.Int64, VG_DTO.UnderVGroup);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, VG_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, VG_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, VG_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

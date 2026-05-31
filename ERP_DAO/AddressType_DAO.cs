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
    public class AddressType_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet AddressTypeDB(AddressType_DTO AT_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("AddressType_SP");
            Db.AddInParameter(DbC, "@ADTP_Number", DbType.Int64, AT_DTO.ADTP_Number);
            Db.AddInParameter(DbC, "@ADTP_Name", DbType.String, AT_DTO.ADTP_Name);
            Db.AddInParameter(DbC, "@ADTP_Notes", DbType.String, AT_DTO.ADTP_Notes);
            Db.AddInParameter(DbC, "@ADTP_DeleteNumbers", DbType.String, AT_DTO.ADTP_DeleteNumbers);
            Db.AddInParameter(DbC, "@ADTP_CreatorCode", DbType.Int64, AT_DTO.ADTP_CreatorCode);
            Db.AddInParameter(DbC, "@ADTP_Id", DbType.Int16, AT_DTO.ADTP_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

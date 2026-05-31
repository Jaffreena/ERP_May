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
 

    public class Shift_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet ShiftDB(Shift_DTO PS_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("Shift_SP");
            Db.AddInParameter(DbC, "@SFT_Number", DbType.Int64, PS_DTO.Number);
            Db.AddInParameter(DbC, "@ShiftName", DbType.String, PS_DTO.ShiftName);
            Db.AddInParameter(DbC, "@Description", DbType.String, PS_DTO.Description);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, PS_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, PS_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, PS_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }
    }
}

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
  public   class BuyerSubsegment_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet BuyerSubsegmentDB(BuyerSubsegment_DTO BS_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("BuyerSubsegment_SP");
            Db.AddInParameter(DbC, "@BSS_Number", DbType.Int64, BS_DTO.BSS_Number);
            Db.AddInParameter(DbC, "@BSS_SubSegment", DbType.String, BS_DTO.BSS_SubSegment);
            Db.AddInParameter(DbC, "@BSS_Description", DbType.String, BS_DTO.BSS_Description);
            Db.AddInParameter(DbC, "@BSS_Under_BSS_Number", DbType.Int64, BS_DTO.BSS_Under_BSS_Number);
            Db.AddInParameter(DbC, "@BSS_DeleteNumbers", DbType.String, BS_DTO.BSS_DeleteNumbers);
            Db.AddInParameter(DbC, "@BSS_CreatorCode", DbType.Int64, BS_DTO.BSS_CreatorCode);
            Db.AddInParameter(DbC, "@BSS_Id", DbType.Int16, BS_DTO.BSS_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;

        }


    }
}

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
    public class BuyerSegment_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet BuyerSegmentDB(BuyerSegment_DTO BST_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("BuyerSegment_SP");
            Db.AddInParameter(DbC, "@BYS_Number", DbType.Int64, BST_DTO.BYS_Number);
            Db.AddInParameter(DbC, "@BYS_Segment", DbType.String, BST_DTO.BYS_Segment);
            Db.AddInParameter(DbC, "@BYS_Description", DbType.String, BST_DTO.BYS_Description);
            Db.AddInParameter(DbC, "@BYS_Under_BYS_Number", DbType.Int64, BST_DTO.BYS_Under_BYS_Number);
            Db.AddInParameter(DbC, "@BYS_DeleteNumbers", DbType.String, BST_DTO.BYS_DeleteNumbers);
            Db.AddInParameter(DbC, "@BYS_CreatorCode", DbType.Int64, BST_DTO.BYS_CreatorCode);
            Db.AddInParameter(DbC, "@BYS_Id", DbType.Int16, BST_DTO.BYS_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

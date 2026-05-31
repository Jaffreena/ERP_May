using ERP_DTO;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO
{
    public class WithholdTax_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet WithholdTaxDB(WithholdTax_DTO WH_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("WithholdTax_SP");
            Db.AddInParameter(DbC, "@WH_Number", DbType.Int64, WH_DTO.WH_Number);
            Db.AddInParameter(DbC, "@WH_TaxCode", DbType.String, WH_DTO.WH_TaxCode);
            Db.AddInParameter(DbC, "@WH_TaxDescription", DbType.String, WH_DTO.WH_TaxDescription);
            Db.AddInParameter(DbC, "@WH_TaxImpact", DbType.Int64, WH_DTO.WH_TaxImpact);
            Db.AddInParameter(DbC, "@WH_TaxCategory", DbType.Int64, WH_DTO.WH_TaxCategory);
            Db.AddInParameter(DbC, "@WH_TaxType", DbType.Int64, WH_DTO.WH_TaxType);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.Int32, WH_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, WH_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, WH_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

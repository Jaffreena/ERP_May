using ERP_DTO.JobInwardTransaction;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO.JobInwardTransaction
{
    public class JobworkInvoice_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet JobworkInvoice(JobworkInvoiceCreate_DTO DN_DTO)
        {
            Database db = new SqlDatabase(DB.Connection());
            DbCommand cmd = db.GetStoredProcCommand("JI_JobworkInvoice_SP");

            //   int DN_Id = 10; // INSERT MODE

            // 🔹 Mode
            db.AddInParameter(cmd, "@JW_Inv_Id", DbType.Int32, DN_DTO.Header.JW_Inv_Id);

            DN_DTO.Header.JISVIH_InvoiceDate = DateTime.Now;
            db.AddInParameter(cmd, "@JISVIH_InvoiceDate", DbType.Date, DN_DTO.Header.JISVIH_InvoiceDate);
            //db.AddInParameter(cmd, "@JIDNI_Item_Code", DbType.String, DN_DTO.Header.it);
            //db.AddInParameter(cmd, "@DN_CUS_Number", DbType.Int32, DN_DTO.Header.DN_CUS_Number);
            //db.AddInParameter(cmd, "@DN_ADD_ADTP_Number", DbType.Int32, DN_DTO.Header.DN_ADD_ADTP_Number);


            return db.ExecuteDataSet(cmd);
        }

        public DataSet GetDeliveryNoteItemsDB(long CustomerNumber)
        {
            Database db = new SqlDatabase(DB.Connection());

            DbCommand cmd = db.GetStoredProcCommand("JI_GetDeliveryNoteItems_SP");

            db.AddInParameter(cmd,
                              "@CustomerNumber",
                              DbType.Int64,
                              CustomerNumber);

            return db.ExecuteDataSet(cmd);
        }
    }
}

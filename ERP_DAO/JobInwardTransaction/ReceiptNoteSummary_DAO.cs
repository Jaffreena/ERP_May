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
    public class ReceiptNoteSummary_DAO
    {

        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();

        public DataSet JI_ReceiptNoteDB(ReceiptNote_DTO DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("ReceiptNoteSummary_SP");

            // =======================
            // 🔹 HEADER (JIRNH)
            // =======================
            //Db.AddInParameter(DbC, "@JIRNH_Number", DbType.Int64, DTO.JIRNH_Number);
            //Db.AddInParameter(DbC, "@JIRNH_RN_No", DbType.String, DTO.JIRNH_RN_No);
            //Db.AddInParameter(DbC, "@JIRNH_RN_Date", DbType.Date, DTO.JIRNH_RN_Date);
            //Db.AddInParameter(DbC, "@JIRNH_JW_CustomerDC_No", DbType.String, DTO.JIRNH_JW_CustomerDC_No);
            //Db.AddInParameter(DbC, "@JIRNH_JW_CustomerDC_Date", DbType.Date, DTO.JIRNH_JW_CustomerDC_Date);
            //Db.AddInParameter(DbC, "@JIRNH_MS_Number", DbType.Int64, DTO.JIRNH_MS_Number);
            //Db.AddInParameter(DbC, "@JIRNH_JWC_Number", DbType.Int64, DTO.JIRNH_JWC_Number);
            //Db.AddInParameter(DbC, "@JIRNH_Currency_Number", DbType.Int64, DTO.JIRNH_Currency_Number);
            //Db.AddInParameter(DbC, "@JIRNH_WH_Number", DbType.Int64, DTO.JIRNH_WH_Number);
            //Db.AddInParameter(DbC, "@JIRNH_Remarks", DbType.String, DTO.JIRNH_Remarks);

            //// =======================
            //// 🔹 ITEM (JIRNI)
            //// =======================
            //Db.AddInParameter(DbC, "@JIRNI_Number", DbType.Int64, DTO.JIRNI_Number);
            //Db.AddInParameter(DbC, "@JIRNI_PRS_Number", DbType.Int64, DTO.JIRNI_PRS_Number);
            //Db.AddInParameter(DbC, "@JIRNI_PRS_Name", DbType.String, DTO.JIRNI_PRS_Name);
            //Db.AddInParameter(DbC, "@JIRNI_Item_Number", DbType.Int64, DTO.JIRNI_Item_Number);
            //Db.AddInParameter(DbC, "@JIRNI_WH_Number", DbType.Int64, DTO.JIRNI_WH_Number);
            //Db.AddInParameter(DbC, "@JIRNI_ITM_Code", DbType.String, DTO.JIRNI_ITM_Code);
            //Db.AddInParameter(DbC, "@JIRNI_UoM_Number", DbType.Int64, DTO.JIRNI_UoM_Number);
            //Db.AddInParameter(DbC, "@JIRNI_Qty", DbType.Decimal, DTO.JIRNI_Qty);
            //Db.AddInParameter(DbC, "@JIRNI_UnitPrice", DbType.Decimal, DTO.JIRNI_UnitPrice);
            //Db.AddInParameter(DbC, "@JIRNI_Amount", DbType.Decimal, DTO.JIRNI_Amount);

            //// =======================
            //// 🔹 BATCH (JIRNI_BCH)
            //// =======================
            //Db.AddInParameter(DbC, "@JIRNI_BCH_Number", DbType.Int64, DTO.JIRNI_BCH_Number);
            //Db.AddInParameter(DbC, "@JIRNI_BCH_WH_Number", DbType.Int64, DTO.JIRNI_BCH_WH_Number);
            //Db.AddInParameter(DbC, "@JIRNI_BCH_BatchDate", DbType.Date, DTO.JIRNI_BCH_BatchDate);
            //Db.AddInParameter(DbC, "@JIRNI_BCH_BatchNo", DbType.String, DTO.JIRNI_BCH_BatchNo);
            //Db.AddInParameter(DbC, "@JIRNI_BCH_BatchQty", DbType.Decimal, DTO.JIRNI_BCH_BatchQty);
            //Db.AddInParameter(DbC, "@JIRNI_BCH_BatchUnitPrice", DbType.Decimal, DTO.JIRNI_BCH_BatchUnitPrice);
            //Db.AddInParameter(DbC, "@JIRNI_BCH_BatchValue", DbType.Decimal, DTO.JIRNI_BCH_BatchValue);

            //// =======================
            //// 🔹 COMMON (IMPORTANT ❗)
            //// =======================
            //Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, DTO.JIRN_DeleteNumbers);
            //Db.AddInParameter(DbC, "@CreatorCode", DbType.Int64, DTO.JIRN_CreatorCode);
            Db.AddInParameter(DbC, "@RI_Id", DbType.Int32, DTO.JIRN_Id);

            //---numbering
            //Db.AddInParameter(DbC, "@RNH_Date", DbType.String, DTO.RNH_Date);

            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }


    }
}

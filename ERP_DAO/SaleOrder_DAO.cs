using ERP_DAO;
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
    public class SaleOrder_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet SaleOrderDB(SaleOrder_DTO S_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("SaleOrder_SP");
            Db.AddInParameter(DbC, "@SO_Number", DbType.Int64, S_DTO.SO_Number);
            Db.AddInParameter(DbC, "@SO_RegNo", DbType.String, S_DTO.SO_RegNo);
            Db.AddInParameter(DbC, "@SO_RegDate", DbType.String, S_DTO.SO_RegDate);
            Db.AddInParameter(DbC, "@SO_OrderNo", DbType.String, S_DTO.SO_OrderNo);
            Db.AddInParameter(DbC, "@SO_OrderDate", DbType.String, S_DTO.SO_OrderDate);
            Db.AddInParameter(DbC, "@SO_BUY_Number", DbType.Int64, S_DTO.SO_BUY_Number);
            Db.AddInParameter(DbC, "@SO_ExchangeRate", DbType.String, S_DTO.SO_ExchangeRate);
            Db.AddInParameter(DbC, "@SO_ExportOrder", DbType.String, S_DTO.SO_ExportOrder);
            Db.AddInParameter(DbC, "@SO_CUR_Number", DbType.Int64, S_DTO.SO_CUR_Number);
            Db.AddInParameter(DbC, "@SO_MS_Number", DbType.Int64, S_DTO.SO_MS_Number);
            Db.AddInParameter(DbC, "@SO_PaymentTerms", DbType.String, S_DTO.SO_PaymentTerms);
            Db.AddInParameter(DbC, "@SO_PaymentMethod", DbType.String, S_DTO.SO_PaymentMethod);
            Db.AddInParameter(DbC, "@SO_DeliveryTerms", DbType.String, S_DTO.SO_DeliveryTerms);
            Db.AddInParameter(DbC, "@SO_DeliveryMethod", DbType.String, S_DTO.SO_DeliveryMethod);
            Db.AddInParameter(DbC, "@SO_QCR", DbType.String, S_DTO.SO_QCR);
            Db.AddInParameter(DbC, "@SO_TDC", DbType.String, S_DTO.SO_TDC);
            Db.AddInParameter(DbC, "@SO_OtherRemarks", DbType.String, S_DTO.SO_OtherRemarks);

            Db.AddInParameter(DbC, "@SO_TotalAmount", DbType.Double, S_DTO.SO_TotalAmount);
            Db.AddInParameter(DbC, "@SO_TotalItemIncome", DbType.Double, S_DTO.SO_TotalItemIncome);
            Db.AddInParameter(DbC, "@SO_TotalHeadIncome", DbType.Double, S_DTO.SO_TotalHeadIncome);
            Db.AddInParameter(DbC, "@SO_OrderValue", DbType.Double, S_DTO.SO_OrderValue);

            Db.AddInParameter(DbC, "@SO_INC_Number", DbType.Int64, S_DTO.SO_INC_Number);
            Db.AddInParameter(DbC, "@SO_INC_MIC_Number", DbType.Int64, S_DTO.SO_INC_MIC_Number);
            Db.AddInParameter(DbC, "@SO_INC_Remarks", DbType.String, S_DTO.SO_INC_Remarks);
            Db.AddInParameter(DbC, "@SO_INC_OCRN_Number", DbType.Int64, S_DTO.SO_INC_OCRN_Number);
            Db.AddInParameter(DbC, "@SO_INC_CM_Number", DbType.Int64, S_DTO.SO_INC_CM_Number);
            Db.AddInParameter(DbC, "@SO_INC_IncomeBase", DbType.Double, S_DTO.SO_INC_IncomeBase);
            Db.AddInParameter(DbC, "@SO_INC_IncomeValue", DbType.Double, S_DTO.SO_INC_IncomeValue);
            Db.AddInParameter(DbC, "@SO_INC_ALCT_Number", DbType.Int64, S_DTO.SO_INC_ALCT_Number);
            Db.AddInParameter(DbC, "@SO_INC_LA_Number", DbType.String, S_DTO.SO_INC_LA_Number);

            Db.AddInParameter(DbC, "@SO_I_Number", DbType.Int64, S_DTO.SO_I_Number);
            Db.AddInParameter(DbC, "@SO_ITM_Number", DbType.Int64, S_DTO.SO_ITM_Number);
            Db.AddInParameter(DbC, "@SO_ITM_Code", DbType.String, S_DTO.SO_ITM_Code);
            Db.AddInParameter(DbC, "@SO_UoM_Number", DbType.Int64, S_DTO.SO_UoM_Number);
            Db.AddInParameter(DbC, "@SO_Qty", DbType.Double, S_DTO.SO_Qty);
            Db.AddInParameter(DbC, "@SO_UnitPrice", DbType.Double, S_DTO.SO_UnitPrice);
            Db.AddInParameter(DbC, "@SO_Amount", DbType.Double, S_DTO.SO_Amount);
            Db.AddInParameter(DbC, "@SO_IncomeValue", DbType.Double, S_DTO.SO_IncomeValue);
            Db.AddInParameter(DbC, "@SO_DeliveryDate", DbType.Int32, S_DTO.SO_DeliveryDate);

            Db.AddInParameter(DbC, "@SO_ADD_Number", DbType.Int64, S_DTO.SO_ADD_Number);
            Db.AddInParameter(DbC, "@SO_ADD_ADTP_Number", DbType.Int64, S_DTO.SO_ADD_ADTP_Number);
            Db.AddInParameter(DbC, "@SO_ADD_AddressID", DbType.String, S_DTO.SO_ADD_AddressID);
            Db.AddInParameter(DbC, "@SO_ADD_Address", DbType.String, S_DTO.SO_ADD_Address);
            Db.AddInParameter(DbC, "@SO_ADD_City", DbType.String, S_DTO.SO_ADD_City);
            Db.AddInParameter(DbC, "@SO_ADD_State", DbType.String, S_DTO.SO_ADD_State);
            Db.AddInParameter(DbC, "@SO_ADD_Country", DbType.String, S_DTO.SO_ADD_Country);
            Db.AddInParameter(DbC, "@SO_ADD_Pin", DbType.String, S_DTO.SO_ADD_Pin);
            Db.AddInParameter(DbC, "@SO_ADD_GSTIN", DbType.String, S_DTO.SO_ADD_GSTIN);

            Db.AddInParameter(DbC, "@SO_DeleteNumbers", DbType.String, S_DTO.SO_DeleteNumbers);
            Db.AddInParameter(DbC, "@SO_CreatorCode", DbType.Int64, S_DTO.SO_CreatorCode);
            Db.AddInParameter(DbC, "@SO_Id", DbType.Int16, S_DTO.SO_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

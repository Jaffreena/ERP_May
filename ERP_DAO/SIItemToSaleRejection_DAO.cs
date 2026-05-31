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
    public class SIItemToSaleRejection_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet SIItemToSaleRejectionDB(SIItemToSaleRejection_DTO SR_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("SIItemToSaleRejection_SP");
            Db.AddInParameter(DbC, "@SR_Number", DbType.Int64, SR_DTO.SR_Number);
            Db.AddInParameter(DbC, "@SR_SIH_Number", DbType.Int64, SR_DTO.SR_SIH_Number);
            Db.AddInParameter(DbC, "@SR_SII_Number", DbType.Int64, SR_DTO.SR_SII_Number);
            Db.AddInParameter(DbC, "@SR_RejectionNo", DbType.String, SR_DTO.SR_RejectionNo);
            Db.AddInParameter(DbC, "@SR_RejectionDate", DbType.Int32, SR_DTO.SR_RejectionDate);
            Db.AddInParameter(DbC, "@SR_ExportOrder", DbType.String, SR_DTO.SR_ExportOrder);
            Db.AddInParameter(DbC, "@SR_ExchangeRate", DbType.String, SR_DTO.SR_ExchangeRate);
            Db.AddInParameter(DbC, "@SR_MS_Number", DbType.Int64, SR_DTO.SR_MS_Number);
            Db.AddInParameter(DbC, "@SR_BUY_Number", DbType.Int64, SR_DTO.SR_BUY_Number);
            Db.AddInParameter(DbC, "@SR_CUR_Number", DbType.Int64, SR_DTO.SR_CUR_Number);
            Db.AddInParameter(DbC, "@SR_TCT_Number", DbType.Int64, SR_DTO.SR_TCT_Number);
            Db.AddInParameter(DbC, "@SR_WHT_Number", DbType.Int64, SR_DTO.SR_WHT_Number);
            Db.AddInParameter(DbC, "@SR_MaterialCost", DbType.Double, SR_DTO.SR_MaterialCost);
            Db.AddInParameter(DbC, "@SR_ItemMiscIncome", DbType.Double, SR_DTO.SR_ItemMiscIncome);
            Db.AddInParameter(DbC, "@SR_HeaderMiscIncome", DbType.Double, SR_DTO.SR_HeaderMiscIncome);
            Db.AddInParameter(DbC, "@SR_GST_Amount", DbType.Double, SR_DTO.SR_GST_Amount);
            Db.AddInParameter(DbC, "@SR_RejectionAmount", DbType.Double, SR_DTO.SR_RejectionAmount);
            Db.AddInParameter(DbC, "@SR_WHT_Amount", DbType.Double, SR_DTO.SR_WHT_Amount);
            Db.AddInParameter(DbC, "@SR_RoundOff", DbType.Double, SR_DTO.SR_RoundOff);
            Db.AddInParameter(DbC, "@SR_BuyerReceivable", DbType.Double, SR_DTO.SR_BuyerReceivable);

            Db.AddInParameter(DbC, "@SR_INC_Number", DbType.Int64, SR_DTO.SR_INC_Number);
            Db.AddInParameter(DbC, "@SR_SII_INC_Number", DbType.Int64, SR_DTO.SR_SII_INC_Number);
            Db.AddInParameter(DbC, "@SR_SIH_INC_Number", DbType.Int64, SR_DTO.SR_SIH_INC_Number);
            Db.AddInParameter(DbC, "@SR_INC_MIC_Number", DbType.Int64, SR_DTO.SR_INC_MIC_Number);
            Db.AddInParameter(DbC, "@SR_INC_Remarks", DbType.String, SR_DTO.SR_INC_Remarks);
            Db.AddInParameter(DbC, "@SR_INC_OCRN_Number", DbType.Int64, SR_DTO.SR_INC_OCRN_Number);
            Db.AddInParameter(DbC, "@SR_INC_CM_Number", DbType.Int64, SR_DTO.SR_INC_CM_Number);
            Db.AddInParameter(DbC, "@SR_INC_IncomeBase", DbType.String, SR_DTO.SR_INC_IncomeBase);
            Db.AddInParameter(DbC, "@SR_INC_IncomeValue", DbType.String, SR_DTO.SR_INC_IncomeValue);
            Db.AddInParameter(DbC, "@SR_INC_ALCT_Number", DbType.Int64, SR_DTO.SR_INC_ALCT_Number);
            Db.AddInParameter(DbC, "@SR_INC_LA_Number", DbType.Int64, SR_DTO.SR_INC_LA_Number);
            Db.AddInParameter(DbC, "@SR_INC_SAC_Number", DbType.Int64, SR_DTO.SR_INC_SAC_Number);
            Db.AddInParameter(DbC, "@SR_INC_CalculateGST", DbType.Int64, SR_DTO.SR_INC_CalculateGST);
            Db.AddInParameter(DbC, "@SR_INC_GST_Amount", DbType.Double, SR_DTO.SR_INC_GST_Amount);
            Db.AddInParameter(DbC, "@SR_INC_WHT_Percent", DbType.Double, SR_DTO.SR_INC_WHT_Percent);
            Db.AddInParameter(DbC, "@SR_INC_WHT_Amount", DbType.Double, SR_DTO.SR_INC_WHT_Amount);

            Db.AddInParameter(DbC, "@SR_I_Number", DbType.Int64, SR_DTO.SR_I_Number);
            Db.AddInParameter(DbC, "@SR_ITM_Number", DbType.Int64, SR_DTO.SR_ITM_Number);
            Db.AddInParameter(DbC, "@SR_ITM_Code", DbType.String, SR_DTO.SR_ITM_Code);
            Db.AddInParameter(DbC, "@SR_WH_Number", DbType.Int64, SR_DTO.SR_WH_Number);
            Db.AddInParameter(DbC, "@SR_UoM_Number", DbType.Int64, SR_DTO.SR_UoM_Number);
            Db.AddInParameter(DbC, "@SR_Qty", DbType.Double, SR_DTO.SR_Qty);
            Db.AddInParameter(DbC, "@SR_UnitPrice", DbType.Double, SR_DTO.SR_UnitPrice);
            Db.AddInParameter(DbC, "@SR_Amount", DbType.Double, SR_DTO.SR_Amount);
            Db.AddInParameter(DbC, "@SR_HSN_Number", DbType.Int64, SR_DTO.SR_HSN_Number);
            Db.AddInParameter(DbC, "@SR_IncomeValue", DbType.Double, SR_DTO.SR_IncomeValue);
            Db.AddInParameter(DbC, "@SR_WHT_Percent", DbType.Double, SR_DTO.SR_WHT_Percent);

            Db.AddInParameter(DbC, "@SR_BCH_Number", DbType.Int64, SR_DTO.SR_BCH_Number);
            Db.AddInParameter(DbC, "@SR_BCH_BCH_Number", DbType.Int64, SR_DTO.SR_BCH_BCH_Number);
            Db.AddInParameter(DbC, "@SR_BCH_Date", DbType.String, SR_DTO.SR_BCH_Date);
            Db.AddInParameter(DbC, "@SR_BCH_No", DbType.String, SR_DTO.SR_BCH_No);
            Db.AddInParameter(DbC, "@SR_BCH_Qty", DbType.Double, SR_DTO.SR_BCH_Qty);
            Db.AddInParameter(DbC, "@SR_BCH_UnitPrice", DbType.Double, SR_DTO.SR_BCH_UnitPrice);
            Db.AddInParameter(DbC, "@SR_BCH_Value", DbType.Double, SR_DTO.SR_BCH_Value);
            Db.AddInParameter(DbC, "@SR_BCH_Index", DbType.Int32, SR_DTO.SR_BCH_Index);
            Db.AddInParameter(DbC, "@SR_BCH_Mode", DbType.Int32, SR_DTO.SR_BCH_Mode);

            Db.AddInParameter(DbC, "@SR_ADD_Number", DbType.Int64, SR_DTO.SR_ADD_Number);
            Db.AddInParameter(DbC, "@SR_ADD_ADTP_Number", DbType.Int64, SR_DTO.SR_ADD_ADTP_Number);
            Db.AddInParameter(DbC, "@SR_ADD_AddressID", DbType.String, SR_DTO.SR_ADD_AddressID);
            Db.AddInParameter(DbC, "@SR_ADD_Address", DbType.String, SR_DTO.SR_ADD_Address);
            Db.AddInParameter(DbC, "@SR_ADD_City", DbType.String, SR_DTO.SR_ADD_City);
            Db.AddInParameter(DbC, "@SR_ADD_State", DbType.String, SR_DTO.SR_ADD_State);
            Db.AddInParameter(DbC, "@SR_ADD_Country", DbType.String, SR_DTO.SR_ADD_Country);
            Db.AddInParameter(DbC, "@SR_ADD_Pin", DbType.String, SR_DTO.SR_ADD_Pin);
            Db.AddInParameter(DbC, "@SR_ADD_GSTIN", DbType.String, SR_DTO.SR_ADD_GSTIN);

            Db.AddInParameter(DbC, "@SR_Search", DbType.String, SR_DTO.SR_Search);
            Db.AddInParameter(DbC, "@SR_DeleteNumbers", DbType.String, SR_DTO.SR_DeleteNumbers);
            Db.AddInParameter(DbC, "@SR_CreatorCode", DbType.Int32, SR_DTO.SR_CreatorCode);
            Db.AddInParameter(DbC, "@SR_Id", DbType.Int16, SR_DTO.SR_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

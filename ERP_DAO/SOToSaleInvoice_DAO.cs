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
    public class SOToSaleInvoice_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet SOToSaleInvoiceDB(SOToSaleInvoice_DTO SI_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("SOToSaleInvoice_SP");
            Db.AddInParameter(DbC, "@SI_Number", DbType.Int64, SI_DTO.SI_Number);
            Db.AddInParameter(DbC, "@SI_SOH_Number", DbType.Int64, SI_DTO.SI_SOH_Number);
            Db.AddInParameter(DbC, "@SI_SOI_Number", DbType.Int64, SI_DTO.SI_SOI_Number);
            Db.AddInParameter(DbC, "@SI_InvoiceNo", DbType.String, SI_DTO.SI_InvoiceNo);
            Db.AddInParameter(DbC, "@SI_InvoiceDate", DbType.Int32, SI_DTO.SI_InvoiceDate);
            Db.AddInParameter(DbC, "@SI_ExportOrder", DbType.String, SI_DTO.SI_ExportOrder);
            Db.AddInParameter(DbC, "@SI_ExchangeRate", DbType.String, SI_DTO.SI_ExchangeRate);
            Db.AddInParameter(DbC, "@SI_MS_Number", DbType.Int64, SI_DTO.SI_MS_Number);
            Db.AddInParameter(DbC, "@SI_BUY_Number", DbType.Int64, SI_DTO.SI_BUY_Number);
            Db.AddInParameter(DbC, "@SI_CUR_Number", DbType.Int64, SI_DTO.SI_CUR_Number);
            Db.AddInParameter(DbC, "@SI_TCT_Number", DbType.Int64, SI_DTO.SI_TCT_Number);
            Db.AddInParameter(DbC, "@SI_WHT_Number", DbType.Int64, SI_DTO.SI_WHT_Number);
            Db.AddInParameter(DbC, "@SI_MaterialCost", DbType.Double, SI_DTO.SI_MaterialCost);
            Db.AddInParameter(DbC, "@SI_ItemMiscIncome", DbType.Double, SI_DTO.SI_ItemMiscIncome);
            Db.AddInParameter(DbC, "@SI_HeaderMiscIncome", DbType.Double, SI_DTO.SI_HeaderMiscIncome);
            Db.AddInParameter(DbC, "@SI_GST_Amount", DbType.Double, SI_DTO.SI_GST_Amount);
            Db.AddInParameter(DbC, "@SI_InvoiceAmount", DbType.Double, SI_DTO.SI_InvoiceAmount);
            Db.AddInParameter(DbC, "@SI_WHT_Amount", DbType.Double, SI_DTO.SI_WHT_Amount);
            Db.AddInParameter(DbC, "@SI_RoundOff", DbType.Double, SI_DTO.SI_RoundOff);
            Db.AddInParameter(DbC, "@SI_BuyerReceivable", DbType.Double, SI_DTO.SI_BuyerReceivable);

            Db.AddInParameter(DbC, "@SI_INC_Number", DbType.Int64, SI_DTO.SI_INC_Number);
            Db.AddInParameter(DbC, "@SI_SOI_INC_Number", DbType.Int64, SI_DTO.SI_SOI_INC_Number);
            Db.AddInParameter(DbC, "@SI_SOH_INC_Number", DbType.Int64, SI_DTO.SI_SOH_INC_Number);
            Db.AddInParameter(DbC, "@SI_INC_MIC_Number", DbType.Int64, SI_DTO.SI_INC_MIC_Number);
            Db.AddInParameter(DbC, "@SI_INC_Remarks", DbType.String, SI_DTO.SI_INC_Remarks);
            Db.AddInParameter(DbC, "@SI_INC_OCRN_Number", DbType.Int64, SI_DTO.SI_INC_OCRN_Number);
            Db.AddInParameter(DbC, "@SI_INC_CM_Number", DbType.Int64, SI_DTO.SI_INC_CM_Number);
            Db.AddInParameter(DbC, "@SI_INC_IncomeBase", DbType.String, SI_DTO.SI_INC_IncomeBase);
            Db.AddInParameter(DbC, "@SI_INC_IncomeValue", DbType.String, SI_DTO.SI_INC_IncomeValue);
            Db.AddInParameter(DbC, "@SI_INC_ALCT_Number", DbType.Int64, SI_DTO.SI_INC_ALCT_Number);
            Db.AddInParameter(DbC, "@SI_INC_LA_Number", DbType.Int64, SI_DTO.SI_INC_LA_Number);
            Db.AddInParameter(DbC, "@SI_INC_SAC_Number", DbType.Int64, SI_DTO.SI_INC_SAC_Number);
            Db.AddInParameter(DbC, "@SI_INC_CalculateGST", DbType.Int64, SI_DTO.SI_INC_CalculateGST);
            Db.AddInParameter(DbC, "@SI_INC_GST_Amount", DbType.Double, SI_DTO.SI_INC_GST_Amount);
            Db.AddInParameter(DbC, "@SI_INC_WHT_Percent", DbType.Double, SI_DTO.SI_INC_WHT_Percent);
            Db.AddInParameter(DbC, "@SI_INC_WHT_Amount", DbType.Double, SI_DTO.SI_INC_WHT_Amount);

            Db.AddInParameter(DbC, "@SI_I_Number", DbType.Int64, SI_DTO.SI_I_Number);
            Db.AddInParameter(DbC, "@SI_ITM_Number", DbType.Int64, SI_DTO.SI_ITM_Number);
            Db.AddInParameter(DbC, "@SI_ITM_Code", DbType.String, SI_DTO.SI_ITM_Code);
            Db.AddInParameter(DbC, "@SI_WH_Number", DbType.Int64, SI_DTO.SI_WH_Number);
            Db.AddInParameter(DbC, "@SI_UoM_Number", DbType.Int64, SI_DTO.SI_UoM_Number);
            Db.AddInParameter(DbC, "@SI_Qty", DbType.Double, SI_DTO.SI_Qty);
            Db.AddInParameter(DbC, "@SI_UnitPrice", DbType.Double, SI_DTO.SI_UnitPrice);
            Db.AddInParameter(DbC, "@SI_Amount", DbType.Double, SI_DTO.SI_Amount);
            Db.AddInParameter(DbC, "@SI_HSN_Number", DbType.Int64, SI_DTO.SI_HSN_Number);
            Db.AddInParameter(DbC, "@SI_IncomeValue", DbType.Double, SI_DTO.SI_IncomeValue);
            Db.AddInParameter(DbC, "@SI_WHT_Percent", DbType.Double, SI_DTO.SI_WHT_Percent);

            Db.AddInParameter(DbC, "@SI_BCH_Number", DbType.Int64, SI_DTO.SI_BCH_Number);
            Db.AddInParameter(DbC, "@SI_BCH_BCH_Number", DbType.Int64, SI_DTO.SI_BCH_BCH_Number);
            Db.AddInParameter(DbC, "@SI_BCH_Date", DbType.String, SI_DTO.SI_BCH_Date);
            Db.AddInParameter(DbC, "@SI_BCH_No", DbType.String, SI_DTO.SI_BCH_No);
            Db.AddInParameter(DbC, "@SI_BCH_Qty", DbType.Double, SI_DTO.SI_BCH_Qty);
            Db.AddInParameter(DbC, "@SI_BCH_UnitPrice", DbType.Double, SI_DTO.SI_BCH_UnitPrice);
            Db.AddInParameter(DbC, "@SI_BCH_Value", DbType.Double, SI_DTO.SI_BCH_Value);
            Db.AddInParameter(DbC, "@SI_BCH_Index", DbType.Int32, SI_DTO.SI_BCH_Index);
            Db.AddInParameter(DbC, "@SI_BCH_Mode", DbType.Int32, SI_DTO.SI_BCH_Mode);

            Db.AddInParameter(DbC, "@SI_ADD_Number", DbType.Int64, SI_DTO.SI_ADD_Number);
            Db.AddInParameter(DbC, "@SI_ADD_ADTP_Number", DbType.Int64, SI_DTO.SI_ADD_ADTP_Number);
            Db.AddInParameter(DbC, "@SI_ADD_AddressID", DbType.String, SI_DTO.SI_ADD_AddressID);
            Db.AddInParameter(DbC, "@SI_ADD_Address", DbType.String, SI_DTO.SI_ADD_Address);
            Db.AddInParameter(DbC, "@SI_ADD_City", DbType.String, SI_DTO.SI_ADD_City);
            Db.AddInParameter(DbC, "@SI_ADD_State", DbType.String, SI_DTO.SI_ADD_State);
            Db.AddInParameter(DbC, "@SI_ADD_Country", DbType.String, SI_DTO.SI_ADD_Country);
            Db.AddInParameter(DbC, "@SI_ADD_Pin", DbType.String, SI_DTO.SI_ADD_Pin);
            Db.AddInParameter(DbC, "@SI_ADD_GSTIN", DbType.String, SI_DTO.SI_ADD_GSTIN);

            Db.AddInParameter(DbC, "@SI_Search", DbType.String, SI_DTO.SI_Search);
            Db.AddInParameter(DbC, "@SI_DeleteNumbers", DbType.String, SI_DTO.SI_DeleteNumbers);
            Db.AddInParameter(DbC, "@SI_CreatorCode", DbType.Int32, SI_DTO.SI_CreatorCode);
            Db.AddInParameter(DbC, "@SI_Id", DbType.Int16, SI_DTO.SI_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

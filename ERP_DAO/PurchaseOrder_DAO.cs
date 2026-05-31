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
    public class PurchaseOrder_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet PurchaseOrderDB(PurchaseOrder_DTO P_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("PurchaseOrder_SP");
            Db.AddInParameter(DbC, "@POH_Number", DbType.Int64, P_DTO.POH_Number);
            Db.AddInParameter(DbC, "@POH_OrderNo", DbType.String, P_DTO.POH_OrderNo);
            Db.AddInParameter(DbC, "@POH_Date", DbType.String, P_DTO.POH_Date);
            Db.AddInParameter(DbC, "@POH_Vendor_Number", DbType.String, P_DTO.POH_Vendor_Number);
            Db.AddInParameter(DbC, "@POH_ExchangeRate", DbType.String, P_DTO.POH_ExchangeRate);
            Db.AddInParameter(DbC, "@POH_ImportOrder", DbType.String, P_DTO.POH_ImportOrder);
            Db.AddInParameter(DbC, "@POH_Currency_Number", DbType.String, P_DTO.POH_Currency_Number);
            Db.AddInParameter(DbC, "@POH_MS_Number", DbType.String, P_DTO.POH_MS_Number);
            Db.AddInParameter(DbC, "@POH_PaymentTerms", DbType.String, P_DTO.POH_PaymentTerms);
            Db.AddInParameter(DbC, "@POH_MOP", DbType.String, P_DTO.POH_MOP);
            Db.AddInParameter(DbC, "@POH_DeliveryTerms", DbType.String, P_DTO.POH_DeliveryTerms);
            Db.AddInParameter(DbC, "@POH_MOD", DbType.String, P_DTO.POH_MOD);
            Db.AddInParameter(DbC, "@POH_Tax", DbType.String, P_DTO.POH_Tax);
            Db.AddInParameter(DbC, "@POH_Inspection", DbType.String, P_DTO.POH_Inspection);
            Db.AddInParameter(DbC, "@POH_TDC", DbType.String, P_DTO.POH_TDC);
            Db.AddInParameter(DbC, "@POH_Remarks", DbType.String, P_DTO.POH_Remarks);

            Db.AddInParameter(DbC, "@POH_MaterialValue", DbType.String, P_DTO.POH_MaterialValue);
            Db.AddInParameter(DbC, "@POH_ItemMiscExpense", DbType.String, P_DTO.POH_ItemMiscExpense);
            Db.AddInParameter(DbC, "@POH_HeadMiscExpense", DbType.String, P_DTO.POH_HeadMiscExpense);
            Db.AddInParameter(DbC, "@POH_OrderValue", DbType.String, P_DTO.POH_OrderValue);

            Db.AddInParameter(DbC, "@EXP_Number", DbType.Int32, P_DTO.EXP_Number);
            Db.AddInParameter(DbC, "@EXP_Expense_Number", DbType.String, P_DTO.EXP_Expense_Number);
            Db.AddInParameter(DbC, "@EXP_Remarks", DbType.String, P_DTO.EXP_Remarks);
            Db.AddInParameter(DbC, "@EXP_Occurrence_Number", DbType.Int32, P_DTO.EXP_Occurrence_Number);
            Db.AddInParameter(DbC, "@EXP_CM_Number", DbType.Int64, P_DTO.EXP_CM_Number);
            Db.AddInParameter(DbC, "@EXP_ExpenseBase", DbType.Double, P_DTO.EXP_ExpenseBase);
            Db.AddInParameter(DbC, "@EXP_ExpenseValue", DbType.Double, P_DTO.EXP_ExpenseValue);
            Db.AddInParameter(DbC, "@EXP_Allocate_Number", DbType.Int64, P_DTO.EXP_Allocate_Number);
            Db.AddInParameter(DbC, "@EXP_LA_Number", DbType.String, P_DTO.EXP_LA_Number);

            Db.AddInParameter(DbC, "@POI_Number", DbType.Int64, P_DTO.POI_Number);
            Db.AddInParameter(DbC, "@POI_Item_Number", DbType.Int64, P_DTO.POI_Item_Number);
            Db.AddInParameter(DbC, "@POI_Item_Code", DbType.String, P_DTO.POI_Item_Code);
            Db.AddInParameter(DbC, "@POI_UoM_Number", DbType.String, P_DTO.POI_UoM_Number);
            Db.AddInParameter(DbC, "@POI_Qty", DbType.Double, P_DTO.POI_Qty);
            Db.AddInParameter(DbC, "@POI_UnitPrice", DbType.String, P_DTO.POI_UnitPrice);
            Db.AddInParameter(DbC, "@POI_Amount", DbType.String, P_DTO.POI_Amount);
            Db.AddInParameter(DbC, "@POI_ExpenseValue", DbType.String, P_DTO.POI_ExpenseValue);
            Db.AddInParameter(DbC, "@POI_DeliveryDate", DbType.Int32, P_DTO.POI_DeliveryDate);

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, P_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, P_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, P_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

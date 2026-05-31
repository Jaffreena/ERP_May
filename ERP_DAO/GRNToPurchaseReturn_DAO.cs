using ERP_DTO;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO
{
    public class GRNToPurchaseReturn_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet GRNToPurchaseReturnDB(GRNToPurchaseReturn_DTO PR_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("GRNToPurchaseReturn_SP");
            Db.AddInParameter(DbC, "@PRH_Number", DbType.Int64, PR_DTO.PRH_Number);
            Db.AddInParameter(DbC, "@PRH_ReturnNo", DbType.String, PR_DTO.PRH_ReturnNo);
            Db.AddInParameter(DbC, "@PRH_ReturnDate", DbType.String, PR_DTO.PRH_ReturnDate);
            Db.AddInParameter(DbC, "@PRH_DueDate", DbType.String, PR_DTO.PRH_DueDate);
            Db.AddInParameter(DbC, "@PRH_ImportOrder", DbType.String, PR_DTO.PRH_ImportOrder);
            Db.AddInParameter(DbC, "@PRH_ExchangeRate", DbType.String, PR_DTO.PRH_ExchangeRate);
            Db.AddInParameter(DbC, "@PRH_MS_Number", DbType.String, PR_DTO.PRH_MS_Number);
            Db.AddInParameter(DbC, "@PRH_Vendor_Number", DbType.String, PR_DTO.PRH_Vendor_Number);
            Db.AddInParameter(DbC, "@PRH_Currency_Number", DbType.String, PR_DTO.PRH_Currency_Number);
            Db.AddInParameter(DbC, "@PRH_TaxCluster_Number", DbType.String, PR_DTO.PRH_TaxCluster_Number);
            Db.AddInParameter(DbC, "@PRH_WHT_Number", DbType.String, PR_DTO.PRH_WHT_Number);
            Db.AddInParameter(DbC, "@PRH_MaterialCost", DbType.String, PR_DTO.PRH_MaterialCost);
            Db.AddInParameter(DbC, "@PRH_ItemMiscExpense", DbType.String, PR_DTO.PRH_ItemMiscExpense);
            Db.AddInParameter(DbC, "@PRH_HeaderMiscExpense", DbType.String, PR_DTO.PRH_HeaderMiscExpense);
            Db.AddInParameter(DbC, "@PRH_GST_Amount", DbType.String, PR_DTO.PRH_GST_Amount);
            Db.AddInParameter(DbC, "@PRH_ReturnAmount", DbType.String, PR_DTO.PRH_ReturnAmount);
            Db.AddInParameter(DbC, "@PRH_WHT_Amount", DbType.String, PR_DTO.PRH_WHT_Amount);
            Db.AddInParameter(DbC, "@PRH_RoundOff", DbType.String, PR_DTO.PRH_RoundOff);
            Db.AddInParameter(DbC, "@PRH_VendorReceivable", DbType.String, PR_DTO.PRH_VendorReceivable);
            Db.AddInParameter(DbC, "@PRH_DeliveryTerms", DbType.String, PR_DTO.PRH_DeliveryTerms);
            Db.AddInParameter(DbC, "@PRH_DeliveryMode", DbType.String, PR_DTO.PRH_DeliveryMode);

            Db.AddInParameter(DbC, "@EXP_Number", DbType.Int64, PR_DTO.EXP_Number);
            Db.AddInParameter(DbC, "@EXP_Expense_Number", DbType.String, PR_DTO.EXP_Expense_Number);
            Db.AddInParameter(DbC, "@EXP_Remarks", DbType.String, PR_DTO.EXP_Remarks);
            Db.AddInParameter(DbC, "@EXP_Occurrence_Number", DbType.Int64, PR_DTO.EXP_Occurrence_Number);
            Db.AddInParameter(DbC, "@EXP_CM_Number", DbType.Int64, PR_DTO.EXP_CM_Number);
            Db.AddInParameter(DbC, "@EXP_ExpenseBase", DbType.Int64, PR_DTO.EXP_ExpenseBase);
            Db.AddInParameter(DbC, "@EXP_ExpenseValue", DbType.Int64, PR_DTO.EXP_ExpenseValue);
            Db.AddInParameter(DbC, "@EXP_Allocate_Number", DbType.Int64, PR_DTO.EXP_Allocate_Number);
            Db.AddInParameter(DbC, "@EXP_LA_Number", DbType.String, PR_DTO.EXP_LA_Number);
            Db.AddInParameter(DbC, "@EXP_TaxCalculate", DbType.Int64, PR_DTO.EXP_TaxCalculate);
            Db.AddInParameter(DbC, "@EXP_TaxValue", DbType.String, PR_DTO.EXP_TaxValue);
            Db.AddInParameter(DbC, "@EXP_SAC_Number", DbType.Int64, PR_DTO.EXP_SAC_Number);

            Db.AddInParameter(DbC, "@PRI_PII_Number", DbType.Int64, PR_DTO.PRI_PII_Number);
            Db.AddInParameter(DbC, "@PRI_PIH_Number", DbType.Int64, PR_DTO.PRI_PIH_Number);
            Db.AddInParameter(DbC, "@PRI_Number", DbType.Int64, PR_DTO.PRI_Number);
            Db.AddInParameter(DbC, "@PRI_Item_Number", DbType.Int64, PR_DTO.PRI_Item_Number);
            Db.AddInParameter(DbC, "@PRI_Warehouse_Number", DbType.Int64, PR_DTO.PRI_Warehouse_Number);
            Db.AddInParameter(DbC, "@PRI_ItemCode", DbType.String, PR_DTO.PRI_ItemCode);
            Db.AddInParameter(DbC, "@PRI_UoM_Number", DbType.String, PR_DTO.PRI_UoM_Number);
            Db.AddInParameter(DbC, "@PRI_Qty", DbType.String, PR_DTO.PRI_Qty);
            Db.AddInParameter(DbC, "@PRI_UnitPrice", DbType.String, PR_DTO.PRI_UnitPrice);
            Db.AddInParameter(DbC, "@PRI_Amount", DbType.String, PR_DTO.PRI_Amount);
            Db.AddInParameter(DbC, "@PRI_HSN_Number", DbType.String, PR_DTO.PRI_HSN_Number);
            Db.AddInParameter(DbC, "@PRI_ExpenseValue", DbType.String, PR_DTO.PRI_ExpenseValue);
            Db.AddInParameter(DbC, "@PRI_GST_Amount", DbType.String, PR_DTO.PRI_GST_Amount);
            Db.AddInParameter(DbC, "@PRI_WHT_Percent", DbType.String, PR_DTO.PRI_WHT_Percent);
            Db.AddInParameter(DbC, "@PRI_WHT_Amount", DbType.String, PR_DTO.PRI_WHT_Amount);

            Db.AddInParameter(DbC, "@BCH_Number", DbType.String, PR_DTO.BCH_Number);
            Db.AddInParameter(DbC, "@PRI_BCH_Number", DbType.String, PR_DTO.PRI_BCH_Number);
            Db.AddInParameter(DbC, "@PRI_BCH_Date", DbType.String, PR_DTO.PRI_BCH_Date);
            Db.AddInParameter(DbC, "@PRI_BCH_No", DbType.String, PR_DTO.PRI_BCH_No);
            Db.AddInParameter(DbC, "@PRI_BCH_Qty", DbType.String, PR_DTO.PRI_BCH_Qty);
            Db.AddInParameter(DbC, "@PRI_BCH_UnitPrice", DbType.String, PR_DTO.PRI_BCH_UnitPrice);
            Db.AddInParameter(DbC, "@PRI_BCH_Value", DbType.String, PR_DTO.PRI_BCH_Value);

            Db.AddInParameter(DbC, "@PRI_Op1", DbType.String, PR_DTO.PRI_Op1);
            Db.AddInParameter(DbC, "@PRI_Op2", DbType.String, PR_DTO.PRI_Op2);

            Db.AddInParameter(DbC, "@Search", DbType.String, PR_DTO.Search);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, PR_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, PR_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, PR_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

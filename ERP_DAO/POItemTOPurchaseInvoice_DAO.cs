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
    public class POItemTOPurchaseInvoice_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet POItemTOPurchaseInvoiceDB(POItemTOPurchaseInvoice_DTO PI_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("POItemTOPurchaseInvoice_SP");
            Db.AddInParameter(DbC, "@PIH_Number", DbType.Int64, PI_DTO.PIH_Number);
            Db.AddInParameter(DbC, "@PIH_InvoiceNo", DbType.String, PI_DTO.PIH_InvoiceNo);
            Db.AddInParameter(DbC, "@PIH_InvoiceDate", DbType.String, PI_DTO.PIH_InvoiceDate);
            Db.AddInParameter(DbC, "@PIH_ImportOrder", DbType.String, PI_DTO.PIH_ImportOrder);
            Db.AddInParameter(DbC, "@PIH_SupplierInvoiceNo", DbType.String, PI_DTO.PIH_SupplierInvoiceNo);
            Db.AddInParameter(DbC, "@PIH_SupplierInvoiceDate", DbType.String, PI_DTO.PIH_SupplierInvoiceDate);
            Db.AddInParameter(DbC, "@PIH_DueDate", DbType.String, PI_DTO.PIH_DueDate);
            Db.AddInParameter(DbC, "@PIH_ExchangeRate", DbType.String, PI_DTO.PIH_ExchangeRate);
            Db.AddInParameter(DbC, "@PIH_MS_Number", DbType.String, PI_DTO.PIH_MS_Number);
            Db.AddInParameter(DbC, "@PIH_Vendor_Number", DbType.String, PI_DTO.PIH_Vendor_Number);
            Db.AddInParameter(DbC, "@PIH_Currency_Number", DbType.String, PI_DTO.PIH_Currency_Number);
            Db.AddInParameter(DbC, "@PIH_TaxCluster_Number", DbType.String, PI_DTO.PIH_TaxCluster_Number);
            Db.AddInParameter(DbC, "@PIH_WHT_Number", DbType.String, PI_DTO.PIH_WHT_Number);
            Db.AddInParameter(DbC, "@PIH_MaterialCost", DbType.String, PI_DTO.PIH_MaterialCost);
            Db.AddInParameter(DbC, "@PIH_ItemMiscExpense", DbType.String, PI_DTO.PIH_ItemMiscExpense);
            Db.AddInParameter(DbC, "@PIH_HeaderMiscExpense", DbType.String, PI_DTO.PIH_HeaderMiscExpense);
            Db.AddInParameter(DbC, "@PIH_GST_Amount", DbType.String, PI_DTO.PIH_GST_Amount);
            Db.AddInParameter(DbC, "@PIH_InvoiceAmount", DbType.String, PI_DTO.PIH_InvoiceAmount);
            Db.AddInParameter(DbC, "@PIH_WHT_Amount", DbType.String, PI_DTO.PIH_WHT_Amount);
            Db.AddInParameter(DbC, "@PIH_RoundOff", DbType.String, PI_DTO.PIH_RoundOff);
            Db.AddInParameter(DbC, "@PIH_VendorPayable", DbType.String, PI_DTO.PIH_VendorPayable);

            Db.AddInParameter(DbC, "@EXP_Number", DbType.Int32, PI_DTO.EXP_Number);
            Db.AddInParameter(DbC, "@EXP_Expense_Number", DbType.String, PI_DTO.EXP_Expense_Number);
            Db.AddInParameter(DbC, "@EXP_Remarks", DbType.String, PI_DTO.EXP_Remarks);
            Db.AddInParameter(DbC, "@EXP_Occurrence_Number", DbType.Int32, PI_DTO.EXP_Occurrence_Number);
            Db.AddInParameter(DbC, "@EXP_CM_Number", DbType.Int64, PI_DTO.EXP_CM_Number);
            Db.AddInParameter(DbC, "@EXP_ExpenseBase", DbType.String, PI_DTO.EXP_ExpenseBase);
            Db.AddInParameter(DbC, "@EXP_ExpenseValue", DbType.String, PI_DTO.EXP_ExpenseValue);
            Db.AddInParameter(DbC, "@EXP_Allocate_Number", DbType.Int64, PI_DTO.EXP_Allocate_Number);
            Db.AddInParameter(DbC, "@EXP_LA_Number", DbType.String, PI_DTO.EXP_LA_Number);
            Db.AddInParameter(DbC, "@EXP_SAC_Number", DbType.String, PI_DTO.EXP_SAC_Number);
            Db.AddInParameter(DbC, "@EXP_TaxCalculate", DbType.String, PI_DTO.EXP_TaxCalculate);
            Db.AddInParameter(DbC, "@EXP_TaxValue", DbType.String, PI_DTO.EXP_TaxValue);


            Db.AddInParameter(DbC, "@PII_Number", DbType.Int64, PI_DTO.PII_Number);
            Db.AddInParameter(DbC, "@PII_POH_Number", DbType.Int64, PI_DTO.PII_POH_Number);
            Db.AddInParameter(DbC, "@PII_POI_Number", DbType.Int64, PI_DTO.PII_POI_Number);
            Db.AddInParameter(DbC, "@PII_Item_Number", DbType.Int64, PI_DTO.PII_Item_Number);
            Db.AddInParameter(DbC, "@PII_Warehouse_Number", DbType.Int64, PI_DTO.PII_Warehouse_Number);
            Db.AddInParameter(DbC, "@PII_ItemCode", DbType.String, PI_DTO.PII_ItemCode);
            Db.AddInParameter(DbC, "@PII_UoM_Number", DbType.String, PI_DTO.PII_UoM_Number);
            Db.AddInParameter(DbC, "@PII_Qty", DbType.String, PI_DTO.PII_Qty);
            Db.AddInParameter(DbC, "@PII_UnitPrice", DbType.String, PI_DTO.PII_UnitPrice);
            Db.AddInParameter(DbC, "@PII_Amount", DbType.String, PI_DTO.PII_Amount);
            Db.AddInParameter(DbC, "@PII_HSN_Number", DbType.String, PI_DTO.PII_HSN_Number);
            Db.AddInParameter(DbC, "@PII_ExpenseValue", DbType.String, PI_DTO.PII_ExpenseValue);
            Db.AddInParameter(DbC, "@PII_GST_Amount", DbType.String, PI_DTO.PII_GST_Amount);
            Db.AddInParameter(DbC, "@PII_WHT_Percent", DbType.String, PI_DTO.PII_WHT_Percent);
            Db.AddInParameter(DbC, "@PII_WHT_Amount", DbType.String, PI_DTO.PII_WHT_Amount);

            Db.AddInParameter(DbC, "@PII_BCH_Number", DbType.String, PI_DTO.PII_BCH_Number);
            Db.AddInParameter(DbC, "@PII_BCH_Date", DbType.String, PI_DTO.PII_BCH_Date);
            Db.AddInParameter(DbC, "@PII_BCH_No", DbType.String, PI_DTO.PII_BCH_No);
            Db.AddInParameter(DbC, "@PII_BCH_Qty", DbType.String, PI_DTO.PII_BCH_Qty);
            Db.AddInParameter(DbC, "@PII_BCH_UnitPrice", DbType.String, PI_DTO.PII_BCH_UnitPrice);
            Db.AddInParameter(DbC, "@PII_BCH_Value", DbType.String, PI_DTO.PII_BCH_Value);

            Db.AddInParameter(DbC, "@Search", DbType.String, PI_DTO.Search);
            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, PI_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, PI_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, PI_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

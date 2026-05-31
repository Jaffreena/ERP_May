using ERP_DTO;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO
{
    public class Customer_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet CustomerDB(Customer_DTO B_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("JW_Customer_SP");
            Db.AddInParameter(DbC, "@CUS_Number", DbType.Int64, B_DTO.CUS_Number);
            Db.AddInParameter(DbC, "@CUS_Name", DbType.String, B_DTO.CUS_Name);
            Db.AddInParameter(DbC, "@CUS_ContactPerson", DbType.String, B_DTO.CUS_ContactPerson);
            Db.AddInParameter(DbC, "@CUS_ContactTelephone", DbType.String, B_DTO.CUS_ContactTelephone);
            Db.AddInParameter(DbC, "@CUS_ContactMobile", DbType.String, B_DTO.CUS_ContactMobile);
            Db.AddInParameter(DbC, "@CUS_ContactEmail", DbType.String, B_DTO.CUS_ContactEmail);
            Db.AddInParameter(DbC, "@CUS_AccountPerson", DbType.String, B_DTO.CUS_AccountPerson);
            Db.AddInParameter(DbC, "@CUS_AccountTelephone", DbType.String, B_DTO.CUS_AccountTelephone);
            Db.AddInParameter(DbC, "@CUS_AccountMobile", DbType.String, B_DTO.CUS_AccountMobile);
            Db.AddInParameter(DbC, "@CUS_AccountEmail", DbType.String, B_DTO.CUS_AccountEmail);
            Db.AddInParameter(DbC, "@CUS_LOC_Number", DbType.Int64, B_DTO.CUS_LOC_Number);
            Db.AddInParameter(DbC, "@CUS_JCG_Number", DbType.Int64, B_DTO.CUS_BYG_Number);

            Db.AddInParameter(DbC, "@JWC_WH_Number", DbType.Int64, B_DTO.JWC_WH_Number);

            Db.AddInParameter(DbC, "@CUS_BYC_Number", DbType.Int64, B_DTO.CUS_BYC_Number);
            Db.AddInParameter(DbC, "@CUS_BYS_Number", DbType.Int64, B_DTO.CUS_BYS_Number);
            Db.AddInParameter(DbC, "@CUS_BSS_Number", DbType.Int64, B_DTO.CUS_BSS_Number);
            Db.AddInParameter(DbC, "@CUS_PaymentTerms", DbType.String, B_DTO.CUS_PaymentTerms);
            Db.AddInParameter(DbC, "@CUS_PaymentMode", DbType.String, B_DTO.CUS_PaymentMode);
            Db.AddInParameter(DbC, "@CUS_CreditDays", DbType.Int16, B_DTO.CUS_CreditDays);
            Db.AddInParameter(DbC, "@CUS_CreditLimit", DbType.Double, B_DTO.CUS_CreditLimit);
            Db.AddInParameter(DbC, "@CUS_CUR_Number", DbType.Int64, B_DTO.CUS_CUR_Number);
            Db.AddInParameter(DbC, "@CUS_AccountName", DbType.String, B_DTO.CUS_AccountName);
            Db.AddInParameter(DbC, "@CUS_AccountNumber", DbType.String, B_DTO.CUS_AccountNumber);
            Db.AddInParameter(DbC, "@CUS_IFSC", DbType.String, B_DTO.CUS_IFSC);
            Db.AddInParameter(DbC, "@CUS_BankName", DbType.String, B_DTO.CUS_BankName);
            Db.AddInParameter(DbC, "@CUS_DeliveryTerms", DbType.String, B_DTO.CUS_DeliveryTerms);
            Db.AddInParameter(DbC, "@CUS_DeliveryMode", DbType.String, B_DTO.CUS_DeliveryMode);
            Db.AddInParameter(DbC, "@CUS_RT_Number", DbType.Int64, B_DTO.CUS_RT_Number);
            Db.AddInParameter(DbC, "@CUS_GSTIN", DbType.String, B_DTO.CUS_GSTIN);
            Db.AddInParameter(DbC, "@CUS_AT_Number", DbType.Int64, B_DTO.CUS_AT_Number);
            Db.AddInParameter(DbC, "@CUS_PAN", DbType.String, B_DTO.CUS_PAN);
            Db.AddInParameter(DbC, "@CUS_YN_Number", DbType.Int64, B_DTO.CUS_YN_Number);
            Db.AddInParameter(DbC, "@CUS_AN_Number", DbType.Int64, B_DTO.CUS_AN_Number);

            Db.AddInParameter(DbC, "@CUS_WHT_Number", DbType.Int64, B_DTO.CUS_WHT_Number);
            Db.AddInParameter(DbC, "@CUS_WHT_WHTC_Number ", DbType.Int64, B_DTO.CUS_WHT_WHTC_Number);
            Db.AddInParameter(DbC, "@CUS_WHT_WHTT_Number  ", DbType.Int64, B_DTO.CUS_WHT_WHTT_Number);
            Db.AddInParameter(DbC, "@CUS_WHT_WHT_Number ", DbType.Int64, B_DTO.CUS_WHT_WHT_Number);
            Db.AddInParameter(DbC, "@CUS_WHT_FromDate  ", DbType.String, B_DTO.CUS_WHT_FromDate);
            Db.AddInParameter(DbC, "@CUS_WHT_ToDate ", DbType.String, B_DTO.CUS_WHT_ToDate);

            Db.AddInParameter(DbC, "@CUS_GST_Number", DbType.Int64, B_DTO.CUS_GST_Number);
            Db.AddInParameter(DbC, "@CUS_GST_GSTC_Number ", DbType.Int64, B_DTO.CUS_GST_GSTC_Number);
            Db.AddInParameter(DbC, "@CUS_GST_GSTT_Number  ", DbType.Int64, B_DTO.CUS_GST_GSTT_Number);
            Db.AddInParameter(DbC, "@CUS_GST_TCT_Number ", DbType.Int64, B_DTO.CUS_GST_TCT_Number);
            Db.AddInParameter(DbC, "@CUS_GST_FromDate  ", DbType.String, B_DTO.CUS_GST_FromDate);
            Db.AddInParameter(DbC, "@CUS_GST_ToDate ", DbType.String, B_DTO.CUS_GST_ToDate);

            Db.AddInParameter(DbC, "@CUS_ADD_Number", DbType.Int64, B_DTO.CUS_ADD_Number);
            Db.AddInParameter(DbC, "@CUS_ADD_ADTP_Number", DbType.Int64, B_DTO.CUS_ADD_ADTP_Number);
            Db.AddInParameter(DbC, "@CUS_ADD_AddressID", DbType.String, B_DTO.CUS_ADD_AddressID);
            Db.AddInParameter(DbC, "@CUS_ADD_Address", DbType.String, B_DTO.CUS_ADD_Address);
            Db.AddInParameter(DbC, "@CUS_ADD_City", DbType.String, B_DTO.CUS_ADD_City);
            Db.AddInParameter(DbC, "@CUS_ADD_State", DbType.String, B_DTO.CUS_ADD_State);
            Db.AddInParameter(DbC, "@CUS_ADD_Country", DbType.String, B_DTO.CUS_ADD_Country);
            Db.AddInParameter(DbC, "@CUS_ADD_Pin", DbType.String, B_DTO.CUS_ADD_Pin);
            Db.AddInParameter(DbC, "@CUS_ADD_GSTIN", DbType.String, B_DTO.CUS_ADD_GSTIN);
            Db.AddInParameter(DbC, "@CUS_ADD_Primary", DbType.Int64, B_DTO.CUS_ADD_Primary);

            Db.AddInParameter(DbC, "@CUS_CNT_Number", DbType.Int64, B_DTO.CUS_CNT_Number);
            Db.AddInParameter(DbC, "@CUS_CNT_ContactName", DbType.String, B_DTO.CUS_CNT_ContactName);
            Db.AddInParameter(DbC, "@CUS_CNT_Department", DbType.String, B_DTO.CUS_CNT_Department);
            Db.AddInParameter(DbC, "@CUS_CNT_Mobile", DbType.String, B_DTO.CUS_CNT_Mobile);
            Db.AddInParameter(DbC, "@CUS_CNT_Telephone", DbType.String, B_DTO.CUS_CNT_Telephone);
            Db.AddInParameter(DbC, "@CUS_CNT_Email", DbType.String, B_DTO.CUS_CNT_Email);

            Db.AddInParameter(DbC, "@CUS_DeleteNumbers", DbType.String, B_DTO.CUS_DeleteNumbers);
            Db.AddInParameter(DbC, "@CUS_CreatorCode", DbType.Int64, B_DTO.CUS_CreatorCode);

            Db.AddInParameter(DbC, "@WH_TaxCategory", DbType.String, B_DTO.WH_TaxCategory);
            Db.AddInParameter(DbC, "@WH_TaxType", DbType.String, B_DTO.WH_TaxType);

            Db.AddInParameter(DbC, "@GST_Category", DbType.String, B_DTO.GST_Category);
            Db.AddInParameter(DbC, "@GST_Type", DbType.String, B_DTO.GST_Type);


            Db.AddInParameter(DbC, "@CUS_Id", DbType.Int16, B_DTO.CUS_Id);
            #region MyRegion
            try
            {
                string folderPath = @"C:\Temp";
                Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, "DbLog.txt");

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("===== DB PARAMETERS START =====");
                sb.AppendLine("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                foreach (DbParameter param in DbC.Parameters)
                {
                    var value = param.Value == null || param.Value == DBNull.Value
                        ? "NULL"
                        : param.Value.ToString();

                    sb.AppendLine($"Name: {param.ParameterName}, Value: {value}, Type: {param.DbType}");
                }

                sb.AppendLine("===== DB PARAMETERS END =====");
                sb.AppendLine("");

                File.AppendAllText(filePath, sb.ToString());

                System.Diagnostics.Debug.WriteLine("FILE WRITTEN SUCCESS");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FILE ERROR: " + ex.ToString());
            }


            #endregion
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

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
    public class Buyer_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet BuyerDB (Buyer_DTO B_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("Buyer_SP");
            Db.AddInParameter(DbC, "@BUY_Number", DbType.Int64, B_DTO.BUY_Number);
            Db.AddInParameter(DbC, "@BUY_Name", DbType.String, B_DTO.BUY_Name);
            Db.AddInParameter(DbC, "@BUY_ContactPerson", DbType.String, B_DTO.BUY_ContactPerson);
            Db.AddInParameter(DbC, "@BUY_ContactTelephone", DbType.String, B_DTO.BUY_ContactTelephone);
            Db.AddInParameter(DbC, "@BUY_ContactMobile", DbType.String, B_DTO.BUY_ContactMobile);
            Db.AddInParameter(DbC, "@BUY_ContactEmail", DbType.String, B_DTO.BUY_ContactEmail);
            Db.AddInParameter(DbC, "@BUY_AccountPerson", DbType.String, B_DTO.BUY_AccountPerson);
            Db.AddInParameter(DbC, "@BUY_AccountTelephone", DbType.String, B_DTO.BUY_AccountTelephone);
            Db.AddInParameter(DbC, "@BUY_AccountMobile", DbType.String, B_DTO.BUY_AccountMobile);
            Db.AddInParameter(DbC, "@BUY_AccountEmail", DbType.String, B_DTO.BUY_AccountEmail);
            Db.AddInParameter(DbC, "@BUY_LOC_Number", DbType.Int64, B_DTO.BUY_LOC_Number);
            Db.AddInParameter(DbC, "@BUY_BYG_Number", DbType.Int64, B_DTO.BUY_BYG_Number);
            Db.AddInParameter(DbC, "@BUY_BYC_Number", DbType.Int64, B_DTO.BUY_BYC_Number);
            Db.AddInParameter(DbC, "@BUY_BYS_Number", DbType.Int64, B_DTO.BUY_BYS_Number);
            Db.AddInParameter(DbC, "@BUY_BSS_Number", DbType.Int64, B_DTO.BUY_BSS_Number);
            Db.AddInParameter(DbC, "@BUY_PaymentTerms", DbType.String, B_DTO.BUY_PaymentTerms);
            Db.AddInParameter(DbC, "@BUY_PaymentMode", DbType.String, B_DTO.BUY_PaymentMode);
            Db.AddInParameter(DbC, "@BUY_CreditDays", DbType.Int16, B_DTO.BUY_CreditDays);
            Db.AddInParameter(DbC, "@BUY_CreditLimit", DbType.Double, B_DTO.BUY_CreditLimit);
            Db.AddInParameter(DbC, "@BUY_CUR_Number", DbType.Int64, B_DTO.BUY_CUR_Number);
            Db.AddInParameter(DbC, "@BUY_AccountName", DbType.String, B_DTO.BUY_AccountName);
            Db.AddInParameter(DbC, "@BUY_AccountNumber", DbType.String, B_DTO.BUY_AccountNumber);
            Db.AddInParameter(DbC, "@BUY_IFSC", DbType.String, B_DTO.BUY_IFSC);
            Db.AddInParameter(DbC, "@BUY_BankName", DbType.String, B_DTO.BUY_BankName);
            Db.AddInParameter(DbC, "@BUY_DeliveryTerms", DbType.String, B_DTO.BUY_DeliveryTerms);
            Db.AddInParameter(DbC, "@BUY_DeliveryMode", DbType.String, B_DTO.BUY_DeliveryMode);
            Db.AddInParameter(DbC, "@BUY_RT_Number", DbType.Int64, B_DTO.BUY_RT_Number);
            Db.AddInParameter(DbC, "@BUY_GSTIN", DbType.String, B_DTO.BUY_GSTIN);
            Db.AddInParameter(DbC, "@BUY_AT_Number", DbType.Int64, B_DTO.BUY_AT_Number);
            Db.AddInParameter(DbC, "@BUY_PAN", DbType.String, B_DTO.BUY_PAN);
            Db.AddInParameter(DbC, "@BUY_YN_Number", DbType.Int64, B_DTO.BUY_YN_Number);
            Db.AddInParameter(DbC, "@BUY_AN_Number", DbType.Int64, B_DTO.BUY_AN_Number);

            Db.AddInParameter(DbC, "@BUY_WHT_Number", DbType.Int64, B_DTO.BUY_WHT_Number);
            Db.AddInParameter(DbC, "@BUY_WHT_WHTC_Number ", DbType.Int64, B_DTO.BUY_WHT_WHTC_Number);
            Db.AddInParameter(DbC, "@BUY_WHT_WHTT_Number  ", DbType.Int64, B_DTO.BUY_WHT_WHTT_Number);
            Db.AddInParameter(DbC, "@BUY_WHT_WHT_Number ", DbType.Int64, B_DTO.BUY_WHT_WHT_Number);
            Db.AddInParameter(DbC, "@BUY_WHT_FromDate  ", DbType.String, B_DTO.BUY_WHT_FromDate);
            Db.AddInParameter(DbC, "@BUY_WHT_ToDate ", DbType.String, B_DTO.BUY_WHT_ToDate);

            Db.AddInParameter(DbC, "@BUY_GST_Number", DbType.Int64, B_DTO.BUY_GST_Number);
            Db.AddInParameter(DbC, "@BUY_GST_GSTC_Number ", DbType.Int64, B_DTO.BUY_GST_GSTC_Number);
            Db.AddInParameter(DbC, "@BUY_GST_GSTT_Number  ", DbType.Int64, B_DTO.BUY_GST_GSTT_Number);
            Db.AddInParameter(DbC, "@BUY_GST_TCT_Number ", DbType.Int64, B_DTO.BUY_GST_TCT_Number);
            Db.AddInParameter(DbC, "@BUY_GST_FromDate  ", DbType.String, B_DTO.BUY_GST_FromDate);
            Db.AddInParameter(DbC, "@BUY_GST_ToDate ", DbType.String, B_DTO.BUY_GST_ToDate);

            Db.AddInParameter(DbC, "@BUY_ADD_Number", DbType.Int64, B_DTO.BUY_ADD_Number);
            Db.AddInParameter(DbC, "@BUY_ADD_ADTP_Number", DbType.Int64, B_DTO.BUY_ADD_ADTP_Number);
            Db.AddInParameter(DbC, "@BUY_ADD_AddressID", DbType.String, B_DTO.BUY_ADD_AddressID);
            Db.AddInParameter(DbC, "@BUY_ADD_Address", DbType.String, B_DTO.BUY_ADD_Address);
            Db.AddInParameter(DbC, "@BUY_ADD_City", DbType.String, B_DTO.BUY_ADD_City);
            Db.AddInParameter(DbC, "@BUY_ADD_State", DbType.String, B_DTO.BUY_ADD_State);
            Db.AddInParameter(DbC, "@BUY_ADD_Country", DbType.String, B_DTO.BUY_ADD_Country);
            Db.AddInParameter(DbC, "@BUY_ADD_Pin", DbType.String, B_DTO.BUY_ADD_Pin);
            Db.AddInParameter(DbC, "@BUY_ADD_GSTIN", DbType.String, B_DTO.BUY_ADD_GSTIN);
            Db.AddInParameter(DbC, "@BUY_ADD_Primary", DbType.Int64, B_DTO.BUY_ADD_Primary);

            Db.AddInParameter(DbC, "@BUY_DeleteNumbers", DbType.String, B_DTO.BUY_DeleteNumbers);
            Db.AddInParameter(DbC, "@BUY_CreatorCode", DbType.Int64, B_DTO.BUY_CreatorCode);
            Db.AddInParameter(DbC, "@BUY_Id", DbType.Int16, B_DTO.BUY_Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

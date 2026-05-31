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
    public class Vendor_DAO
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public DataSet VendorDB (Vendor_DTO V_DTO)
        {
            Database Db = new SqlDatabase(DB.Connection());
            DbCommand DbC = Db.GetStoredProcCommand("Vendor_SP");
            Db.AddInParameter(DbC, "@VendorNumber", DbType.Int64, V_DTO.VendorNumber);
            Db.AddInParameter(DbC, "@VendorCode", DbType.String, V_DTO.VendorCode);
            Db.AddInParameter(DbC, "@VendorName", DbType.String, V_DTO.VendorName);
            Db.AddInParameter(DbC, "@Address", DbType.String, V_DTO.Address);
            Db.AddInParameter(DbC, "@City", DbType.String, V_DTO.City);
            Db.AddInParameter(DbC, "@State", DbType.String, V_DTO.State);
            Db.AddInParameter(DbC, "@Country", DbType.String, V_DTO.Country);
            Db.AddInParameter(DbC, "@Pincode", DbType.String, V_DTO.Pincode);
            Db.AddInParameter(DbC, "@ContactPerson", DbType.String, V_DTO.ContactPerson);
            Db.AddInParameter(DbC, "@CP_Telephone", DbType.String, V_DTO.CP_Telephone);
            Db.AddInParameter(DbC, "@CP_Mobile", DbType.String, V_DTO.CP_Mobile);
            Db.AddInParameter(DbC, "@CP_Email", DbType.String, V_DTO.CP_Email);
            Db.AddInParameter(DbC, "@AccountsPerson", DbType.String, V_DTO.AccountsPerson);
            Db.AddInParameter(DbC, "@AP_Telephone", DbType.String, V_DTO.AP_Telephone);
            Db.AddInParameter(DbC, "@AP_Mobile", DbType.String, V_DTO.AP_Mobile);
            Db.AddInParameter(DbC, "@AP_Email", DbType.String, V_DTO.AP_Email);
            Db.AddInParameter(DbC, "@VendorLocation", DbType.Int32, V_DTO.VendorLocation);
            Db.AddInParameter(DbC, "@VendorGroup", DbType.Int64, V_DTO.VendorGroup);
            Db.AddInParameter(DbC, "@VendorCategory", DbType.Int64, V_DTO.VendorCategory);
            Db.AddInParameter(DbC, "@RegistrationType", DbType.Int64, V_DTO.RegistrationType);
            Db.AddInParameter(DbC, "@GSTIN", DbType.String, V_DTO.GSTIN);
            Db.AddInParameter(DbC, "@AssesseeTerritory", DbType.Int64, V_DTO.AssesseeTerritory);
            Db.AddInParameter(DbC, "@TransportAgency", DbType.Int32, V_DTO.TransportAgency);
            Db.AddInParameter(DbC, "@TransporterID", DbType.String, V_DTO.TransporterID);
            Db.AddInParameter(DbC, "@PaymentTerms", DbType.String, V_DTO.PaymentTerms);
            Db.AddInParameter(DbC, "@PaymentMode", DbType.String, V_DTO.PaymentMode);
            Db.AddInParameter(DbC, "@CreditDays", DbType.Int32, V_DTO.CreditDays);
            Db.AddInParameter(DbC, "@PaymentBase", DbType.Int64, V_DTO.PaymentBase);
            Db.AddInParameter(DbC, "@Currency", DbType.Int64, V_DTO.Currency);
            Db.AddInParameter(DbC, "@AccountName", DbType.String, V_DTO.AccountName);
            Db.AddInParameter(DbC, "@AccountNumber", DbType.String, V_DTO.AccountNumber);
            Db.AddInParameter(DbC, "@IFSC", DbType.String, V_DTO.IFSC);
            Db.AddInParameter(DbC, "@BankName", DbType.String, V_DTO.BankName);
            Db.AddInParameter(DbC, "@PAN", DbType.String, V_DTO.PAN);
            Db.AddInParameter(DbC, "@WithholdTax", DbType.Int64, V_DTO.WithholdTax);
            Db.AddInParameter(DbC, "@AssesseeNature", DbType.Int64, V_DTO.AssesseeNature);

            Db.AddInParameter(DbC, "@TaxNumber", DbType.Int64, V_DTO.TaxNumber);
            Db.AddInParameter(DbC, "@Category ", DbType.Int64, V_DTO.Category);
            Db.AddInParameter(DbC, "@Type  ", DbType.Int64, V_DTO.Type);
            Db.AddInParameter(DbC, "@Tax ", DbType.Int64, V_DTO.Tax);
            Db.AddInParameter(DbC, "@FromDate  ", DbType.Int32, V_DTO.FromDate);
            Db.AddInParameter(DbC, "@ToDate ", DbType.Int32, V_DTO.ToDate);

            Db.AddInParameter(DbC, "@DeleteNumbers", DbType.String, V_DTO.DeleteNumbers);
            Db.AddInParameter(DbC, "@CreatorCode", DbType.Int32, V_DTO.CreatorCode);
            Db.AddInParameter(DbC, "@Id", DbType.Int32, V_DTO.Id);
            DS = Db.ExecuteDataSet(DbC);
            return DS;
        }
    }
}

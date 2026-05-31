using ERP_DTO;
using System.Data;

namespace ERP.DataList
{
    public class ProcurementMaster_DL
    {
        public List<Vendor_DTO> VList(DataTable Dt)
        {
            List<Vendor_DTO> VList = new List<Vendor_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VList.Add(
                    new Vendor_DTO
                    {
                        VendorNumber = Convert.ToInt64(dr["VendorNumber"]),
                        VendorCode = Convert.ToString(dr["VendorCode"]),
                        VendorName = Convert.ToString(dr["VendorName"]),
                        Currency = Convert.ToString(dr["Currency"]),
                        VendorLocation = Convert.ToString(dr["VendorLocation"]),
                        VendorGroup = Convert.ToString(dr["VendorGroup"]),
                        VendorCategory = Convert.ToString(dr["VendorCategory"]),
                    });
            }
            return VList;
        }
        public List<VendorHead_DTO> VHList(DataTable Dt)
        {
            List<VendorHead_DTO> VHList = new List<VendorHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VHList.Add(
                    new VendorHead_DTO
                    {
                        VendorNumber = Convert.ToInt64(dr["VendorNumber"]),
                        VendorCode = Convert.ToString(dr["VendorCode"]),
                        VendorName = Convert.ToString(dr["VendorName"]),
                        Address = Convert.ToString(dr["Address"]),
                        City = Convert.ToString(dr["City"]),
                        State = Convert.ToString(dr["State"]),
                        Country = Convert.ToString(dr["Country"]),
                        Pincode = Convert.ToString(dr["Pincode"]),
                        ContactPerson = Convert.ToString(dr["ContactPerson"]),
                        CP_Telephone = Convert.ToString(dr["CP_Telephone"]),
                        CP_Mobile = Convert.ToString(dr["CP_Mobile"]),
                        CP_Email = Convert.ToString(dr["CP_Email"]),
                        AccountsPerson = Convert.ToString(dr["AccountsPerson"]),
                        AP_Telephone = Convert.ToString(dr["AP_Telephone"]),
                        AP_Mobile = Convert.ToString(dr["AP_Mobile"]),
                        AP_Email = Convert.ToString(dr["AP_Email"]),
                        VendorLocation = Convert.ToString(dr["VendorLocation"]),
                        VendorGroup = Convert.ToString(dr["VendorGroup"]),
                        VendorCategory = Convert.ToString(dr["VendorCategory"]),
                        RegistrationType = Convert.ToString(dr["RegistrationType"]),
                        GSTIN = Convert.ToString(dr["GSTIN"]),
                        AssesseeTerritory = Convert.ToString(dr["AssesseeTerritory"]),
                        AssesseeTerritoryName = Convert.ToString(dr["AssesseeTerritoryName"]),
                        TransportAgency = Convert.ToString(dr["TransportAgency"]),
                        TransporterID = Convert.ToString(dr["TransporterID"]),
                        PaymentTerms = Convert.ToString(dr["PaymentTerms"]),
                        PaymentMode = Convert.ToString(dr["PaymentMode"]),
                        CreditDays = Convert.ToString(dr["CreditDays"]),
                        PaymentBase = Convert.ToString(dr["PaymentBase"]),
                        Currency = Convert.ToString(dr["Currency"]),
                        AccountName = Convert.ToString(dr["AccountName"]),
                        AccountNumber = Convert.ToString(dr["AccountNumber"]),
                        IFSC = Convert.ToString(dr["IFSC"]),
                        BankName = Convert.ToString(dr["BankName"]),
                        PAN = Convert.ToString(dr["PAN"]),
                        WithholdTax = Convert.ToString(dr["WithholdTax"]),
                        AssesseeNature = Convert.ToString(dr["AssesseeNature"]),
                    });
            }
            return VHList;
        }
        public List<VendorWHT_DTO> VWHTList(DataTable Dt)
        {
            List<VendorWHT_DTO> VHList = new List<VendorWHT_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VHList.Add(
                    new VendorWHT_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Category = Convert.ToString(dr["WHT_Category"]),
                        Type = Convert.ToString(dr["WHT_Type"]),
                        Tax = Convert.ToString(dr["WHT_TaxCode"]),
                        FromDate = Convert.ToString(dr["FromDate"]),
                        ToDate = Convert.ToString(dr["ToDate"]),
                    });
            }
            return VHList;
        }
        public List<VendorGST_DTO> VGSTList(DataTable Dt)
        {
            List<VendorGST_DTO> VHList = new List<VendorGST_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VHList.Add(
                    new VendorGST_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Category = Convert.ToString(dr["GST_Category"]),
                        CategoryName = Convert.ToString(dr["GST_CategoryName"]),
                        Type = Convert.ToString(dr["GST_Type"]),
                        TaxNumber = Convert.ToString(dr["GST_TaxCluster"]),
                        Tax = Convert.ToString(dr["Tax"]),
                        FromDate = Convert.ToString(dr["FromDate"]),
                        ToDate = Convert.ToString(dr["ToDate"]),
                    });
            }
            return VHList;
        }


        public List<VendorCategory_DTO> VCList(DataTable Dt)
        {
            List<VendorCategory_DTO> VCList = new List<VendorCategory_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VCList.Add(
                    new VendorCategory_DTO
                    {
                        VendorCategoryNumber = Convert.ToInt64(dr["VendorCategoryNumber"]),
                        VendorCategory = Convert.ToString(dr["VendorCategory"]),
                        VC_Description = Convert.ToString(dr["VC_Description"]),
                        UnderVCategory = Convert.ToString(dr["UnderVCategory"])
                    });
            }
            return VCList;
        }
        public List<VendorGroup_DTO> VGList(DataTable Dt)
        {
            List<VendorGroup_DTO> VGList = new List<VendorGroup_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                VGList.Add(
                    new VendorGroup_DTO
                    {
                        VendorGroupNumber = Convert.ToInt64(dr["VendorGroupNumber"]),
                        VendorGroup = Convert.ToString(dr["VendorGroup"]),
                        VG_Description = Convert.ToString(dr["VG_Description"]),
                        UnderVGroup = Convert.ToString(dr["UnderVGroup"])
                    });
            }
            return VGList;
        }

        public List<VendorGST_DTO> VendorTaxCategory(DataTable Dt)
        {
            List<VendorGST_DTO> CList = new List<VendorGST_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CList.Add(
                    new VendorGST_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Category = Convert.ToString(dr["Category"]),
                    });
            }
            return CList;
        }
        public List<VendorGST_DTO> VendorCluster(DataTable Dt)
        {
            List<VendorGST_DTO> CList = new List<VendorGST_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CList.Add(
                    new VendorGST_DTO
                    {
                        Number = Convert.ToInt64(dr["TaxClusterNumber"]),
                        Tax = Convert.ToString(dr["TaxCluster"]),
                    });
            }
            return CList;
        }
    }
}

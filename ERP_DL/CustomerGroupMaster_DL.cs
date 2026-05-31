using ERP_DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DataList
{
    public class JobworkInwardMaster_DL
    {
        //public List<CustomerGST_DTO> CustomerTaxCategory(DataTable Dt)
        //{
        //    List<CustomerGST_DTO> CList = new List<CustomerGST_DTO>();

        //    foreach (DataRow dr in Dt.Rows)
        //    {
        //        CList.Add(
        //            new CustomerGST_DTO
        //            {
        //                CUS_GST_Number = Convert.ToInt64(dr["Number"]),
        //                CUS_GST_GSTC_Number = Convert.ToString(dr["Category"]),
        //            });
        //    }

        //    return CList;
        //}

        public List<CustomerGST_DTO> CustomerTaxCategory(DataTable Dt)
        {
            List<CustomerGST_DTO> CList = new List<CustomerGST_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                CList.Add(
                    new CustomerGST_DTO
                    {
                        CUS_GST_Number = dr["Number"] == DBNull.Value
                                            ? 0
                                            : Convert.ToInt64(dr["Number"]),

                        CUS_GST_GSTC_Number = dr["Category"] == DBNull.Value
                                            ? ""
                                            : Convert.ToString(dr["Category"]),
                    });
            }

            return CList;
        }

        public List<CustomerGST_DTO> CustomerAssess(DataTable Dt)
        {
            List<CustomerGST_DTO> CList = new List<CustomerGST_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                CList.Add(
                    new CustomerGST_DTO
                    {
                        CUS_GST_Number = dr["Number"] == DBNull.Value
                                            ? 0
                                            : Convert.ToInt64(dr["Number"]),

                        CUS_GST_GSTC_Number = dr["Assessee"] == DBNull.Value
                                            ? ""
                                            : Convert.ToString(dr["Assessee"]),
                    });
            }

            return CList;
        }

        public List<Customer_DTO> CustomerWHTGridTaxCode(DataTable Dt)
        {
            List<Customer_DTO> CList = new List<Customer_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                CList.Add(
                    new Customer_DTO
                    {
                        WH_Number = dr["WH_Number"] == DBNull.Value
                                            ? ""
                                            : Convert.ToString(dr["WH_Number"]),

                        WH_TaxCode = dr["WH_TaxCode"] == DBNull.Value
                                            ? ""
                                            : Convert.ToString(dr["WH_TaxCode"]),
                        WH_TaxDescription = dr["WH_TaxDescription"] == DBNull.Value
                                            ? ""
                                            : Convert.ToString(dr["WH_TaxDescription"])
                    });
            }

            return CList;
        }
        public List<Customer_DTO> CustomerGSTGridTaxCluster(DataTable Dt)
        {
            List<Customer_DTO> CList = new List<Customer_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                CList.Add(
                    new Customer_DTO
                    {
                        TCT_Number = dr["TaxClusterNumber"] == DBNull.Value
                                        ? ""
                                        : Convert.ToString(dr["TaxClusterNumber"]),

                        TCT_Name = dr["TaxCluster"] == DBNull.Value
                                        ? ""
                                        : Convert.ToString(dr["TaxCluster"]),

                        TCT_Description = dr["ClusterDescription"] == DBNull.Value
                                        ? ""
                                        : Convert.ToString(dr["ClusterDescription"])
                    });
            }

            return CList;
        }
        public List<CustomerGST_DTO> CustomerCluster(DataTable Dt)
        {
            List<CustomerGST_DTO> CList = new List<CustomerGST_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                CList.Add(new CustomerGST_DTO
                {
                    CUS_GST_Number = dr["TaxClusterNumber"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt64(dr["TaxClusterNumber"]),

                    CUS_GST_TCT_Number = dr["TaxCluster"] == DBNull.Value
                                        ? ""
                                        : Convert.ToString(dr["TaxCluster"])
                });
            }

            return CList;
        }
     
        //Customer
        public List<CustomerList_DTO> CustomerList(DataTable Dt)
        {
            List<CustomerList_DTO> BList = new List<CustomerList_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new CustomerList_DTO
                    {
                        CUS_Number = Convert.ToInt64(dr["CUS_Number"]),
                        CUS_Name = Convert.ToString(dr["CUS_Name"]),
                        CUS_CUR_Name = Convert.ToString(dr["CUS_CUR_Name"]),
                        CUS_LOC_Name = Convert.ToString(dr["CUS_LOC_Name"]),
                        CUS_BYG_Name = Convert.ToString(dr["CUS_BYG_Name"]),
                        CUS_BYC_Name = Convert.ToString(dr["CUS_BYC_Name"]),
                        //CUS_BYS_Name = Convert.ToString(dr["CUS_BYS_Name"]),
                        //CUS_BSS_Name = Convert.ToString(dr["CUS_BSS_Name"]),
                    });
            }
            return BList;
        }
        public List<CustomerHead_DTO> CustomerHeadList(DataTable Dt)
        {
            List<CustomerHead_DTO> CustomerHeadList = new List<CustomerHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CustomerHeadList.Add(
                    new CustomerHead_DTO
                    {
                        CUS_Number = Convert.ToInt64(dr["CUS_Number"]),
                        CUS_Name = Convert.ToString(dr["CUS_Name"]),
                        CUS_ContactPerson = Convert.ToString(dr["CUS_ContactPerson"]),
                        CUS_ContactTelephone = Convert.ToString(dr["CUS_ContactTelephone"]),
                        CUS_ContactMobile = Convert.ToString(dr["CUS_ContactMobile"]),
                        CUS_ContactEmail = Convert.ToString(dr["CUS_ContactEmail"]),
                        CUS_AccountPerson = Convert.ToString(dr["CUS_AccountPerson"]),
                        CUS_AccountTelephone = Convert.ToString(dr["CUS_AccountTelephone"]),
                        CUS_AccountMobile = Convert.ToString(dr["CUS_AccountMobile"]),
                        CUS_AccountEmail = Convert.ToString(dr["CUS_AccountEmail"]),
                        CUS_LOC_Number = Convert.ToInt64(dr["CUS_LOC_Number"]),
                        CUS_BYG_Number = Convert.ToInt64(dr["CUS_JCG_Number"]),
                        CUS_BYC_Number = Convert.ToInt64(dr["CUS_BYC_Number"]),
                        CUS_WareHouse_Number = Convert.ToInt64(dr["JWC_WH_NUMBER"]),
                        CUS_BSS_Number = Convert.ToInt64(dr["CUS_BSS_Number"]),
                        CUS_PaymentTerms = Convert.ToString(dr["CUS_PaymentTerms"]),
                        CUS_PaymentMode = Convert.ToString(dr["CUS_PaymentMode"]),
                        CUS_CreditDays = Convert.ToString(dr["CUS_CreditDays"]),
                        CUS_CreditLimit = Convert.ToString(dr["CUS_CreditLimit"]),
                        CUS_CUR_Number = Convert.ToInt64(dr["CUS_CUR_Number"]),
                        CUS_AccountName = Convert.ToString(dr["CUS_AccountName"]),
                        CUS_AccountNumber = Convert.ToString(dr["CUS_AccountNumber"]),
                        CUS_IFSC = Convert.ToString(dr["CUS_IFSC"]),
                        CUS_BankName = Convert.ToString(dr["CUS_BankName"]),
                        CUS_DeliveryTerms = Convert.ToString(dr["CUS_DeliveryTerms"]),
                        CUS_DeliveryMode = Convert.ToString(dr["CUS_DeliveryMode"]),
                        CUS_RT_Number = Convert.ToInt64(dr["CUS_RT_Number"]),
                        CUS_GSTIN = Convert.ToString(dr["CUS_GSTIN"]),
                        CUS_AT_Number = Convert.ToInt64(dr["CUS_AT_Number"]),
                        CUS_AT_Name = Convert.ToString(dr["CUS_AT_Name"]),
                        CUS_PAN = Convert.ToString(dr["CUS_PAN"]),
                        CUS_YN_Number = Convert.ToInt64(dr["CUS_YN_Number"]),
                        CUS_AN_Number = Convert.ToInt64(dr["CUS_AN_Number"])
                        
                    });
            }
            return CustomerHeadList;
        }
        public List<CustomerWHT_DTO> CustomerWHTList(DataTable Dt)
        {
            List<CustomerWHT_DTO> CustomerHeadList = new List<CustomerWHT_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CustomerHeadList.Add(
                    new CustomerWHT_DTO
                    {
                        CUS_WHT_Number = Convert.ToInt64(dr["JWC_WHT_Number"]),
                        CUS_WHT_WHTC_Number = Convert.ToString(dr["JWC_WHT_WHTC_Number"]),
                        CUS_WHT_WHTT_Number = Convert.ToString(dr["JWC_WHT_WHTT_Number"]),
                        CUS_WHT_WHT_Number = Convert.ToString(dr["JWC_WHT_WHT_Number"]),
                        CUS_WHT_FromDate = Convert.ToString(dr["JWC_WHT_FromDate"]),
                        CUS_WHT_ToDate = Convert.ToString(dr["JWC_WHT_ToDate"]),
                        CUS_WHT_WHT_Description = Convert.ToString(dr["JWC_WHT_Description"])
                    });
            }
            return CustomerHeadList;
        }
        public List<CustomerGST_DTO> CustomerGSTList(DataTable Dt)
        {
            List<CustomerGST_DTO> CustomerHeadList = new List<CustomerGST_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CustomerHeadList.Add(
                    new CustomerGST_DTO
                    {
                        CUS_GST_Number = Convert.ToInt64(dr["JWC_GST_Number"]),
                        CUS_GST_GSTC_Number = Convert.ToString(dr["JWC_GST_GSTC_Number"]),
                        CUS_GST_GSTC_Name = Convert.ToString(dr["CUS_GST_GSTC_Name"]),
                        CUS_GST_GSTT_Number = Convert.ToString(dr["JWC_GST_GSTT_Number"]),
                        CUS_GST_TCT_Number = Convert.ToString(dr["JWC_GST_TCT_Number"]),
                        CUS_GST_TCT_Name = Convert.ToString(dr["CUS_GST_TCT_Name"]),
                        CUS_GST_FromDate = Convert.ToString(dr["JWC_GST_FromDate"]),
                        CUS_GST_ToDate = Convert.ToString(dr["CUS_GST_ToDate"]),
                        CUS_GST_TCT_Description= Convert.ToString(dr["JWC_GST_Description"])
                    });
            }
            return CustomerHeadList;
        }

        public List<CustomerContact_DTO> CustomerContactList(DataTable Dt)
        {
            List<CustomerContact_DTO> CustomerContactList = new List<CustomerContact_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                CustomerContactList.Add(
                    new CustomerContact_DTO
                    {
                        CUS_CNT_Number = Convert.ToInt64(dr["JWC_CNT_Number"]),

                        CUS_CNT_ContactName = Convert.ToString(dr["JWC_CNT_ContactName"]),
                        CUS_CNT_Department = Convert.ToString(dr["JWC_CNT_Department"]),
                        CUS_CNT_Mobile = Convert.ToString(dr["JWC_CNT_Mobile"]),
                        CUS_CNT_Telephone = Convert.ToString(dr["JWC_CNT_Telephone"]),
                        CUS_CNT_Email = Convert.ToString(dr["JWC_CNT_Email"]),

                        // Optional (if you added in DTO)
                        // CUS_CNT_JWC_Number = Convert.ToInt64(dr["JWC_CNT_JWC_Number"]),

                        CUS_CNT_IsDeleted = 0
                    });
            }

            return CustomerContactList;
        }
        public List<CustomerAdd_DTO> CustomerAddList(DataTable Dt)
        {
            List<CustomerAdd_DTO> CustomerAddList = new List<CustomerAdd_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CustomerAddList.Add(
             new CustomerAdd_DTO
             {
                 CUS_ADD_Number = Convert.ToInt64(dr["JWC_ADD_Number"]),
                 CUS_ADD_ADTP_Number = Convert.ToInt64(dr["JWC_ADD_ADTP_Number"]),
                 CUS_ADD_AddressID = Convert.ToString(dr["JWC_ADD_Address_ID"]),
                 CUS_ADD_Address = Convert.ToString(dr["JWC_ADD_Address"]),
                 CUS_ADD_City = Convert.ToString(dr["JWC_ADD_City"]),
                 CUS_ADD_State = Convert.ToString(dr["JWC_ADD_State"]),
                 CUS_ADD_Country = Convert.ToString(dr["JWC_ADD_Country"]),
                 CUS_ADD_Pin = Convert.ToString(dr["JWC_ADD_PIN"]),
                 CUS_ADD_GSTIN = Convert.ToString(dr["JWC_ADD_GSTIN"]),
                 CUS_ADD_Primary = dr["JWC_ADD_Default"]?.ToString() == "1"
             });
            }
            return CustomerAddList;
        }
        //Customer Group List
        public List<CustomerGroup_DTO> GroupList(DataTable Dt)
        {
            List<CustomerGroup_DTO> G_List = new List<CustomerGroup_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                G_List.Add(
                    new CustomerGroup_DTO
                    {
                        JCG_Number = Convert.ToInt64(dr["JCG_Number"]),
                        JCG_JW_CustomerGroup = Convert.ToString(dr["JCG_JW_CustomerGroup"]),
                        JCG_Description = Convert.ToString(dr["JCG_Description"]),
                        JCG_Under_JCG_Number = Convert.ToString(dr["JCG_Under_JCG_Number"])
                    });
            }
            return G_List;
        }



        //Customer Category List
        public List<CustomerCategory_DTO> CustomerCategoryList(DataTable Dt)
        {
            List<CustomerCategory_DTO> JCCList = new List<CustomerCategory_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                JCCList.Add(
                    new CustomerCategory_DTO
                    {
                        JCC_Number = Convert.ToInt64(dr["JCC_Number"]),
                        JCC_JW_CustomerCategory = Convert.ToString(dr["JCC_JW_CustomerCategory"]),
                        JCC_Description = Convert.ToString(dr["JCC_Description"]),
                        JCC_Under_JCC_Number = Convert.ToString(dr["JCC_Under_JCC_Number"])
                    });
            }
            return JCCList;
        }
        public List<WorkCentre_DTO> WorkList(DataTable Dt)
        {
            List<WorkCentre_DTO> list = new List<WorkCentre_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                list.Add(new WorkCentre_DTO
                {
                    WC_Number = dr["WC_Number"] != DBNull.Value ? Convert.ToInt64(dr["WC_Number"]) : 0,
                    WC_WorkCentre = Convert.ToString(dr["WC_WorkCentre"]),
                    WC_Description = Convert.ToString(dr["WC_Description"]),
                    WC_WCG_Number = dr["WC_WCG_Number"] != DBNull.Value ? Convert.ToInt64(dr["WC_WCG_Number"]) : 0,
                    WC_Warehouse_Number = dr["WC_Warehouse_Number"] != DBNull.Value ? Convert.ToInt64(dr["WC_Warehouse_Number"]) : 0,
                    WC_PRS_Number = dr["WC_PRS_Number"] != DBNull.Value ? Convert.ToInt64(dr["WC_PRS_Number"]) : 0,
                   
                });
            }

            return list;
        }

        public List<JW_Process_DTO> WList(DataTable Dt)
        {
            List<JW_Process_DTO> WList = new List<JW_Process_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                WList.Add(
                    new JW_Process_DTO
                    {
                        ProcessNumber = Convert.ToInt64(dr["ProcessNumber"]),
                        ProcessName = Convert.ToString(dr["ProcessName"]),
                        Description = Convert.ToString(dr["Description"]),
                        ProductionUoM = Convert.ToString(dr["ProductionUoM"]),
                        ConsumptionUoM = Convert.ToString(dr["ConsumptionUoM"]),

                        ScrapUoM = Convert.ToString(dr["ScrapUoM"]),
                        ScrapItemCode = Convert.ToString(dr["ScrapItemCode"]),
                        SAC = Convert.ToString(dr["SAC"])

                    });
            }
            return WList;
        }

        // WorkCentre Group List
        public List<WorkCentreGroup_DTO> WorkCentreGroupList(DataTable Dt)
        {
            List<WorkCentreGroup_DTO> G_List = new List<WorkCentreGroup_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                G_List.Add(
                    new WorkCentreGroup_DTO
                    {
                        WCG_Number = Convert.ToInt64(dr["WCG_Number"]),
                        WCG_WorkCentreGroup = Convert.ToString(dr["WCG_WorkCentreGroup"]),
                        WCG_Description = Convert.ToString(dr["WCG_Description"]),
                        WCG_Under_WCG_Number = Convert.ToString(dr["WCG_Under_WCG_Number"])
                    });
            }

            return G_List;
        }
        // WorkCentre List
        public List<WorkCentre_DTO> WorkCentreList(DataTable Dt)
        {
            List<WorkCentre_DTO> WC_List = new List<WorkCentre_DTO>();

            foreach (DataRow dr in Dt.Rows)
            {
                WC_List.Add(
                    new WorkCentre_DTO
                    {
                        WC_Number = Convert.ToInt64(dr["WC_Number"]),
                        WC_WorkCentre = Convert.ToString(dr["WC_WorkCentre"]),
                        WC_Description = Convert.ToString(dr["WC_Description"]),

                        WC_WCG_Number = Convert.ToInt64(dr["WC_WCG_Number"]),
                        WC_Warehouse_Number = Convert.ToInt64(dr["WC_Warehouse_Number"]),
                        WC_PRS_Number = Convert.ToInt64(dr["WC_PRS_Number"]),
                        WC_ProcessName = Convert.ToString(dr["ProcessName"]),
                        WC_WarehouseName = Convert.ToString(dr["WarehouseName"]),
                        WC_WorkcentreGroup = Convert.ToString(dr["WCG_WorkCentreGroup"])
                    });
            }

            return WC_List;
        }
    }
}

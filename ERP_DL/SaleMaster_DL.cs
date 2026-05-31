using ERP_DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DL
{
    public class SaleMaster_DL
    {
        //Buyer
        public List<BuyerList_DTO> BuyerList(DataTable Dt)
        {
            List<BuyerList_DTO> BList = new List<BuyerList_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new BuyerList_DTO
                    {
                        BUY_Number = Convert.ToInt64(dr["BUY_Number"]),
                        BUY_Name = Convert.ToString(dr["BUY_Name"]),
                        BUY_CUR_Name = Convert.ToString(dr["BUY_CUR_Name"]),
                        BUY_LOC_Name = Convert.ToString(dr["BUY_LOC_Name"]),
                        BUY_BYG_Name = Convert.ToString(dr["BUY_BYG_Name"]),
                        BUY_BYC_Name = Convert.ToString(dr["BUY_BYC_Name"]),
                        BUY_BYS_Name = Convert.ToString(dr["BUY_BYS_Name"]),
                        BUY_BSS_Name = Convert.ToString(dr["BUY_BSS_Name"]),
                    });
            }
            return BList;
        }
        public List<BuyerHead_DTO> BuyerHeadList(DataTable Dt)
        {
            List<BuyerHead_DTO> BuyerHeadList = new List<BuyerHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BuyerHeadList.Add(
                    new BuyerHead_DTO
                    {
                        BUY_Number = Convert.ToInt64(dr["BUY_Number"]),
                        BUY_Name = Convert.ToString(dr["BUY_Name"]),
                        BUY_ContactPerson = Convert.ToString(dr["BUY_ContactPerson"]),
                        BUY_ContactTelephone = Convert.ToString(dr["BUY_ContactTelephone"]),
                        BUY_ContactMobile = Convert.ToString(dr["BUY_ContactMobile"]),
                        BUY_ContactEmail = Convert.ToString(dr["BUY_ContactEmail"]),
                        BUY_AccountPerson = Convert.ToString(dr["BUY_AccountPerson"]),
                        BUY_AccountTelephone = Convert.ToString(dr["BUY_AccountTelephone"]),
                        BUY_AccountMobile = Convert.ToString(dr["BUY_AccountMobile"]),
                        BUY_AccountEmail = Convert.ToString(dr["BUY_AccountEmail"]),
                        BUY_LOC_Number = Convert.ToInt64(dr["BUY_LOC_Number"]),
                        BUY_BYG_Number = Convert.ToInt64(dr["BUY_BYG_Number"]),
                        BUY_BYC_Number = Convert.ToInt64(dr["BUY_BYC_Number"]),
                        BUY_BYS_Number = Convert.ToInt64(dr["BUY_BYS_Number"]),
                        BUY_BSS_Number = Convert.ToInt64(dr["BUY_BSS_Number"]),
                        BUY_PaymentTerms = Convert.ToString(dr["BUY_PaymentTerms"]),
                        BUY_PaymentMode = Convert.ToString(dr["BUY_PaymentMode"]),
                        BUY_CreditDays = Convert.ToString(dr["BUY_CreditDays"]),
                        BUY_CreditLimit = Convert.ToString(dr["BUY_CreditLimit"]),
                        BUY_CUR_Number = Convert.ToInt64(dr["BUY_CUR_Number"]),
                        BUY_AccountName = Convert.ToString(dr["BUY_AccountName"]),
                        BUY_AccountNumber = Convert.ToString(dr["BUY_AccountNumber"]),
                        BUY_IFSC = Convert.ToString(dr["BUY_IFSC"]),
                        BUY_BankName = Convert.ToString(dr["BUY_BankName"]),
                        BUY_DeliveryTerms = Convert.ToString(dr["BUY_DeliveryTerms"]),
                        BUY_DeliveryMode = Convert.ToString(dr["BUY_DeliveryMode"]),
                        BUY_RT_Number = Convert.ToInt64(dr["BUY_RT_Number"]),
                        BUY_GSTIN = Convert.ToString(dr["BUY_GSTIN"]),
                        BUY_AT_Number = Convert.ToInt64(dr["BUY_AT_Number"]),
                        BUY_AT_Name = Convert.ToString(dr["BUY_AT_Name"]),
                        BUY_PAN = Convert.ToString(dr["BUY_PAN"]),
                        BUY_YN_Number = Convert.ToInt64(dr["BUY_YN_Number"]),
                        BUY_AN_Number = Convert.ToInt64(dr["BUY_AN_Number"]),
                    });
            }
            return BuyerHeadList;
        }
        public List<BuyerWHT_DTO> BuyerWHTList(DataTable Dt)
        {
            List<BuyerWHT_DTO> BuyerHeadList = new List<BuyerWHT_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BuyerHeadList.Add(
                    new BuyerWHT_DTO
                    {
                        BUY_WHT_Number = Convert.ToInt64(dr["BUY_WHT_Number"]),
                        BUY_WHT_WHTC_Number = Convert.ToString(dr["BUY_WHT_WHTC_Number"]),
                        BUY_WHT_WHTT_Number = Convert.ToString(dr["BUY_WHT_WHTT_Number"]),
                        BUY_WHT_WHT_Number = Convert.ToString(dr["BUY_WHT_WHT_Number"]),
                        BUY_WHT_FromDate = Convert.ToString(dr["BUY_WHT_FromDate"]),
                        BUY_WHT_ToDate = Convert.ToString(dr["BUY_WHT_ToDate"]),
                    });
            }
            return BuyerHeadList;
        }
        public List<BuyerGST_DTO> BuyerGSTList(DataTable Dt)
        {
            List<BuyerGST_DTO> BuyerHeadList = new List<BuyerGST_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BuyerHeadList.Add(
                    new BuyerGST_DTO
                    {
                        BUY_GST_Number = Convert.ToInt64(dr["BUY_GST_Number"]),
                        BUY_GST_GSTC_Number = Convert.ToString(dr["BUY_GST_GSTC_Number"]),
                        BUY_GST_GSTC_Name = Convert.ToString(dr["BUY_GST_GSTC_Name"]),
                        BUY_GST_GSTT_Number = Convert.ToString(dr["BUY_GST_GSTT_Number"]),
                        BUY_GST_TCT_Number = Convert.ToString(dr["BUY_GST_TCT_Number"]),
                        BUY_GST_TCT_Name = Convert.ToString(dr["BUY_GST_TCT_Name"]),
                        BUY_GST_FromDate = Convert.ToString(dr["BUY_GST_FromDate"]),
                        BUY_GST_ToDate = Convert.ToString(dr["BUY_GST_ToDate"]),
                    });
            }
            return BuyerHeadList;
        }
        public List<BuyerAdd_DTO> BuyerAddList(DataTable Dt)
        {
            List<BuyerAdd_DTO> BuyerAddList = new List<BuyerAdd_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BuyerAddList.Add(
                    new BuyerAdd_DTO
                    {
                        BUY_ADD_Number = Convert.ToInt64(dr["BUY_ADD_Number"]),
                        BUY_ADD_ADTP_Number = Convert.ToInt64(dr["BUY_ADD_ADTP_Number"]),
                        BUY_ADD_AddressID = Convert.ToString(dr["BUY_ADD_AddressID"]),
                        BUY_ADD_Address = Convert.ToString(dr["BUY_ADD_Address"]),
                        BUY_ADD_City = Convert.ToString(dr["BUY_ADD_City"]),
                        BUY_ADD_State = Convert.ToString(dr["BUY_ADD_State"]),
                        BUY_ADD_Country = Convert.ToString(dr["BUY_ADD_Country"]),
                        BUY_ADD_Pin = Convert.ToString(dr["BUY_ADD_Pin"]),
                        BUY_ADD_GSTIN = Convert.ToString(dr["BUY_ADD_GSTIN"]),
                        BUY_ADD_Primary = Convert.ToBoolean(dr["BUY_ADD_Primary"]),
                    });
            }
            return BuyerAddList;
        }
        public List<BuyerGST_DTO> BuyerTaxCategory(DataTable Dt)
        {
            List<BuyerGST_DTO> BList = new List<BuyerGST_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new BuyerGST_DTO
                    {
                        BUY_GST_Number = Convert.ToInt64(dr["Number"]),
                        BUY_GST_GSTC_Number = Convert.ToString(dr["Category"]),
                    });
            }
            return BList;
        }
        public List<BuyerGST_DTO> BuyerCluster(DataTable Dt)
        {
            List<BuyerGST_DTO> BList = new List<BuyerGST_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BList.Add(
                    new BuyerGST_DTO
                    {
                        BUY_GST_Number = Convert.ToInt64(dr["TaxClusterNumber"]),
                        BUY_GST_TCT_Number = Convert.ToString(dr["TaxCluster"]),
                    });
            }
            return BList;
        }





        //Buyer Group List
        public List<BuyerGroup_DTO> GroupList(DataTable Dt)
        {
            List<BuyerGroup_DTO> G_List = new List<BuyerGroup_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                G_List.Add(
                    new BuyerGroup_DTO
                    {
                        BYG_Number = Convert.ToInt64(dr["BYG_Number"]),
                        BYG_Group = Convert.ToString(dr["BYG_Group"]),
                        BYG_Description = Convert.ToString(dr["BYG_Description"]),
                        BYG_Under_BYG_Number = Convert.ToString(dr["BYG_Under_BYG_Number"])
                    });
            }
            return G_List;
        }


        //Buyer Segment List
        public List<BuyerSegment_DTO> SegmentList(DataTable Dt)
        {
            List<BuyerSegment_DTO> S_List = new List<BuyerSegment_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                S_List.Add(
                    new BuyerSegment_DTO
                    {
                        BYS_Number = Convert.ToInt64(dr["BYS_Number"]),
                        BYS_Segment = Convert.ToString(dr["BYS_Segment"]),
                        BYS_Description = Convert.ToString(dr["BYS_Description"]),
                        BYS_Under_BYS_Number = Convert.ToString(dr["BYS_Under_BYS_Number"])
                    });
            }
            return S_List;
        }


        //Buyer Subsegment List
        public List<BuyerSubsegment_DTO> SubsegmentList(DataTable Dt)
        {
            List<BuyerSubsegment_DTO> SS_List = new List<BuyerSubsegment_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SS_List.Add(
                    new BuyerSubsegment_DTO
                    {
                        BSS_Number = Convert.ToInt64(dr["BSS_Number"]),
                        BSS_SubSegment = Convert.ToString(dr["BSS_SubSegment"]),
                        BSS_Description = Convert.ToString(dr["BSS_Description"]),
                        BSS_Under_BSS_Number = Convert.ToString(dr["BSS_Under_BSS_Number"])
                    });
            }
            return SS_List;
        }


        //Income Code List
        public List<IncomeCode_DTO> IncomeList(DataTable Dt)
        {
            List<IncomeCode_DTO> I_List = new List<IncomeCode_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                I_List.Add(
                    new IncomeCode_DTO
                    {
                        MIC_Number = Convert.ToInt64(dr["MIC_Number"]),
                        MIC_Code = Convert.ToString(dr["MIC_Code"]),
                        MIC_Description = Convert.ToString(dr["MIC_Description"]),
                        MIC_OCRN_Number = Convert.ToString(dr["MIC_OCRN_Number"]),
                        MIC_CM_Number = Convert.ToString(dr["MIC_CM_Number"]),
                        MIC_ALCT_Number = Convert.ToString(dr["MIC_ALCT_Number"]),
                        MIC_LA_Number = Convert.ToString(dr["MIC_LA_Number"]),
                        MIC_SAC_Number = Convert.ToString(dr["MIC_SAC_Number"])
                    });
            }
            return I_List;
        }


        //Buyer Category List
        public List<BuyerCategory_DTO> BuyerCategoryList(DataTable Dt)
        {
            List<BuyerCategory_DTO> BYCList = new List<BuyerCategory_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                BYCList.Add(
                    new BuyerCategory_DTO
                    {
                        BYC_Number = Convert.ToInt64(dr["BYC_Number"]),
                        BYC_Category = Convert.ToString(dr["BYC_Category"]),
                        BYC_Description = Convert.ToString(dr["BYC_Description"]),
                        BYC_Under_BYC_Number = Convert.ToString(dr["BYC_Under_BYC_Number"])
                    });
            }
            return BYCList;
        }
    }
}

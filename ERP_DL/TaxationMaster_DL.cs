using ERP_DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DL
{
    public class TaxationMaster_DL
    {
        public List<SAC_DTO> SACList(DataTable Dt)
        {
            List<SAC_DTO> SList = new List<SAC_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SList.Add(
                    new SAC_DTO
                    {
                        SAC_Number = Convert.ToInt64(dr["SAC_Number"]),
                        SAC_Code = Convert.ToString(dr["SAC_Code"]),
                        Description = Convert.ToString(dr["Description"]),
                    });
            }
            return SList;
        }

        public List<HSN_DTO>HSNList(DataTable Dt)
        {
            List<HSN_DTO> SList = new List<HSN_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SList.Add(
                    new HSN_DTO
                    {
                        HSN_Number = Convert.ToInt64(dr["HSN_Number"]),
                        HSN_Code = Convert.ToString(dr["HSN_Code"]),
                        Description = Convert.ToString(dr["Description"]),
                    });
            }
            return SList;
        }

        public List<WithholdTax_DTO> WHList(DataTable Dt)
        {
            List<WithholdTax_DTO> WHList = new List<WithholdTax_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                WHList.Add(
                    new WithholdTax_DTO
                    {
                        WH_Number = Convert.ToInt64(dr["WH_Number"]),
                        WH_TaxCode = Convert.ToString(dr["WH_TaxCode"]),
                        WH_TaxDescription = Convert.ToString(dr["WH_TaxDescription"]),
                        WH_TaxCategory = Convert.ToString(dr["WH_TaxCategory"]),
                        WH_TaxType = Convert.ToString(dr["WH_TaxType"]),
                        WH_TaxImpact = Convert.ToString(dr["WH_TaxImpact"]),
                    });
            }
            return WHList;
        }


        public List<WH_TaxAssign_DTO> WHAList(DataTable Dt)
        {
            List<WH_TaxAssign_DTO> WHList = new List<WH_TaxAssign_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                WHList.Add(
                    new WH_TaxAssign_DTO
                    {
                        WH_TaxNumber = Convert.ToInt64(dr["WH_TaxNumber"]),
                        WH_TaxCode = Convert.ToString(dr["WH_TaxCode"]),
                        WH_TaxDescription = Convert.ToString(dr["WH_TaxDescription"]),
                    });
            }
            return WHList;
        }

        public List<WH_TaxAssignHead_DTO> WHEList(DataTable Dt)
        {
            List<WH_TaxAssignHead_DTO> WHList = new List<WH_TaxAssignHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                WHList.Add(
                    new WH_TaxAssignHead_DTO
                    {
                        WH_TaxNumber = Convert.ToInt64(dr["WH_TaxNumber"]),
                        WH_TaxCode = Convert.ToString(dr["WH_TaxCode"]),
                        WH_TaxDescription = Convert.ToString(dr["WH_TaxDescription"]),
                    });
            }
            return WHList;
        }

        public List<WH_TaxAssignDetail_DTO> WHDList(DataTable Dt)
        {
            List<WH_TaxAssignDetail_DTO> WHList = new List<WH_TaxAssignDetail_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                WHList.Add(
                    new WH_TaxAssignDetail_DTO
                    {
                        WH_TaxAssignNumber = Convert.ToInt64(dr["WH_TaxAssignNumber"]),
                        AssesseeNature = Convert.ToString(dr["AssesseeNature"]),
                        FromDate = Convert.ToString(dr["FromDate"]),
                        ToDate = Convert.ToString(dr["ToDate"]),
                        SingleTransLimit = Convert.ToString(dr["SingleTransLimit"]),
                        AggregateTransLimit = Convert.ToString(dr["AggregateTransLimit"]),
                        IncludeTax = Convert.ToString(dr["IncludeTax"]),
                        PAN_TaxPercent = Convert.ToString(dr["PAN_TaxPercent"]),
                        NON_PAN_TaxPercent = Convert.ToString(dr["NON_PAN_TaxPercent"]),
                        COA = Convert.ToString(dr["COA"]),
                });
            }
            return WHList;
        }

        public List<TaxCluster_DTO> TCList(DataTable Dt)
        {
            List<TaxCluster_DTO> TCList = new List<TaxCluster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TCList.Add(
                    new TaxCluster_DTO
                    {
                        TaxClusterNumber = Convert.ToInt64(dr["TaxClusterNumber"]),
                        GST_Category = Convert.ToString(dr["GST_Category"]),
                        GST_Type = Convert.ToString(dr["GST_Type"]),
                        TaxCluster = Convert.ToString(dr["TaxCluster"]),
                        ClusterDescription = Convert.ToString(dr["ClusterDescription"]),
                    });
            }
            return TCList;
        }

        public List<TaxClusterHead_DTO> TCEList(DataTable Dt)
        {
            List<TaxClusterHead_DTO> TCHList = new List<TaxClusterHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TCHList.Add(
                    new TaxClusterHead_DTO
                    {
                        TaxClusterNumber = Convert.ToInt64(dr["TaxClusterNumber"]),
                        GST_Category = Convert.ToString(dr["GST_Category"]),
                        GST_Type = Convert.ToString(dr["GST_Type"]),
                        TaxCluster = Convert.ToString(dr["TaxCluster"]),
                        ClusterDescription = Convert.ToString(dr["ClusterDescription"]),
                    });
            }
            return TCHList;
        }

        public List<TaxClusterDetail_DTO> TCDList(DataTable Dt)
        {
            List<TaxClusterDetail_DTO> TCDList = new List<TaxClusterDetail_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TCDList.Add(
                    new TaxClusterDetail_DTO
                    {
                        TaxIndex = Convert.ToInt64(dr["TaxIndex"]),
                        TaxClusterDetailsNumber = Convert.ToInt64(dr["TaxClusterDetailsNumber"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        TaxElementDescription = Convert.ToString(dr["TaxElementDescription"]),
                        CalculationFactors = Convert.ToString(dr["CalculationFactors"]),
                        ChargeableBasis = Convert.ToString(dr["ChargeableBasis"]),
                    });
            }
            return TCDList;
        }

        public List<TaxElement_DTO> TEList(DataTable Dt)
        {
            List<TaxElement_DTO> TEList = new List<TaxElement_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TEList.Add(
                    new TaxElement_DTO
                    {
                        TaxElementNumber = Convert.ToInt64(dr["TaxElementNumber"]),
                        TaxCategory = Convert.ToString(dr["TaxCategory"]),
                        TaxType = Convert.ToString(dr["TaxType"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        ElementDescription = Convert.ToString(dr["ElementDescription"]),
                        LoadonInventory = Convert.ToBoolean(dr["LoadonInventory"]),
                        LoadonInventoryPercent = Convert.ToString(dr["LoadonInventoryPercent"]),
                        COA_LedgerAccount = Convert.ToString(dr["COA_LedgerAccount"]),
                    });
            }
            return TEList;
        }

        public List<TaxElementHead_DTO> TEEList(DataTable Dt)
        {
            List<TaxElementHead_DTO> TEList = new List<TaxElementHead_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TEList.Add(
                    new TaxElementHead_DTO
                    {
                        TaxElementNumber = Convert.ToInt64(dr["TaxElementNumber"]),
                        TaxCategory = Convert.ToString(dr["TaxCategory"]),
                        TaxType = Convert.ToString(dr["TaxType"]),
                        TaxElement = Convert.ToString(dr["TaxElement"]),
                        ElementDescription = Convert.ToString(dr["ElementDescription"]),

                        LoadonInventory = Convert.ToBoolean(dr["LoadonInventory"]),
                        LoadonInventoryPercent = Convert.ToString(dr["LoadonInventoryPercent"]),
                        GST_TaxNature = Convert.ToString(dr["GST_TaxNature"]),
                        GST_Abatement = Convert.ToString(dr["GST_Abatement"]),
                        COA_LedgerAccount = Convert.ToString(dr["COA_LedgerAccount"]),
                    });
            }
            return TEList;
        }


        public List<TaxElementDetail_DTO> TEDList(DataTable Dt)
        {
            List<TaxElementDetail_DTO> TEList = new List<TaxElementDetail_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TEList.Add(
                    new TaxElementDetail_DTO
                    {
                        TaxNumber = Convert.ToInt64(dr["TaxNumber"]),
                        FromDate = Convert.ToString(dr["FromDate"]),
                        ToDate = Convert.ToString(dr["ToDate"]),
                        FixedPercent = Convert.ToString(dr["FixedPercent"]),
                    });
            }
            return TEList;
        }


        public List<TaxElementVariableDetail_DTO> TEVDList(DataTable Dt)
        {
            List<TaxElementVariableDetail_DTO> TEList = new List<TaxElementVariableDetail_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                TEList.Add(
                    new TaxElementVariableDetail_DTO
                    {
                        TaxNumber = Convert.ToInt64(dr["TaxNumber"]),
                        FromDate = Convert.ToString(dr["FromDate"]),
                        ToDate = Convert.ToString(dr["ToDate"]),
                        HSN = Convert.ToString(dr["HSN"]),
                        HSNPercent = Convert.ToString(dr["HSNPercent"]),
                        SAC = Convert.ToString(dr["SAC"]),
                        SACPercent = Convert.ToString(dr["SACPercent"]),
                    });
            }
            return TEList;
        }
    }
}

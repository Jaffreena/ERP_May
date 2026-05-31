using ERP_DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DL
{
    public class Numbering_DL
    {
        public List<PONumberReset_DTO> PORList(DataTable Dt)
        {
            List<PONumberReset_DTO> PORList = new List<PONumberReset_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PORList.Add(
                    new PONumberReset_DTO
                    {
                        POR_Number = Convert.ToInt64(dr["POR_Number"]),
                        POR_Date = Convert.ToString(dr["POR_Date"]),
                        POR_StartingNumber = Convert.ToString(dr["POR_StartingNumber"]),
                        POR_NumberofDigits = Convert.ToString(dr["POR_NumberofDigits"]),
                        POR_PrefilZero = Convert.ToString(dr["POR_PrefilZero"]),
                        POR_Frequency = Convert.ToString(dr["POR_Frequency"])
                    });
            }
            return PORList;
        }
        public List<PONumberPrefix_DTO> POPList(DataTable Dt)
        {
            List<PONumberPrefix_DTO> PORList = new List<PONumberPrefix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PORList.Add(
                    new PONumberPrefix_DTO
                    {
                        POP_Number = Convert.ToInt64(dr["POP_Number"]),
                        POP_Date = Convert.ToString(dr["POP_Date"]),
                        POP_Particulars = Convert.ToString(dr["POP_Particulars"])
                    });
            }
            return PORList;
        }
        public List<PONumberSuffix_DTO> POSList(DataTable Dt)
        {
            List<PONumberSuffix_DTO> PORList = new List<PONumberSuffix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PORList.Add(
                    new PONumberSuffix_DTO
                    {
                        POS_Number = Convert.ToInt64(dr["POS_Number"]),
                        POS_Date = Convert.ToString(dr["POS_Date"]),
                        POS_Particulars = Convert.ToString(dr["POS_Particulars"])
                    });
            }
            return PORList;
        }




        public List<PINumberReset_DTO> PIRList(DataTable Dt)
        {
            List<PINumberReset_DTO> PIRList = new List<PINumberReset_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIRList.Add(
                    new PINumberReset_DTO
                    {
                        PIR_Number = Convert.ToInt64(dr["PIR_Number"]),
                        PIR_Date = Convert.ToString(dr["PIR_Date"]),
                        PIR_StartingNumber = Convert.ToString(dr["PIR_StartingNumber"]),
                        PIR_NumberofDigits = Convert.ToString(dr["PIR_NumberofDigits"]),
                        PIR_PrefilZero = Convert.ToString(dr["PIR_PrefilZero"]),
                        PIR_Frequency = Convert.ToString(dr["PIR_Frequency"])
                    });
            }
            return PIRList;
        }
        public List<PINumberPrefix_DTO> PIPList(DataTable Dt)
        {
            List<PINumberPrefix_DTO> PIRList = new List<PINumberPrefix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIRList.Add(
                    new PINumberPrefix_DTO
                    {
                        PIP_Number = Convert.ToInt64(dr["PIP_Number"]),
                        PIP_Date = Convert.ToString(dr["PIP_Date"]),
                        PIP_Particulars = Convert.ToString(dr["PIP_Particulars"])
                    });
            }
            return PIRList;
        }
        public List<PINumberSuffix_DTO> PISList(DataTable Dt)
        {
            List<PINumberSuffix_DTO> PIRList = new List<PINumberSuffix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIRList.Add(
                    new PINumberSuffix_DTO
                    {
                        PIS_Number = Convert.ToInt64(dr["PIS_Number"]),
                        PIS_Date = Convert.ToString(dr["PIS_Date"]),
                        PIS_Particulars = Convert.ToString(dr["PIS_Particulars"])
                    });
            }
            return PIRList;
        }




        public List<PRNumberReset_DTO> PRRList(DataTable Dt)
        {
            List<PRNumberReset_DTO> PRRList = new List<PRNumberReset_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRRList.Add(
                    new PRNumberReset_DTO
                    {
                        PRR_Number = Convert.ToInt64(dr["PRR_Number"]),
                        PRR_Date = Convert.ToString(dr["PRR_Date"]),
                        PRR_StartingNumber = Convert.ToString(dr["PRR_StartingNumber"]),
                        PRR_NumberofDigits = Convert.ToString(dr["PRR_NumberofDigits"]),
                        PRR_PrefilZero = Convert.ToString(dr["PRR_PrefilZero"]),
                        PRR_Frequency = Convert.ToString(dr["PRR_Frequency"])
                    });
            }
            return PRRList;
        }
        public List<PRNumberPrefix_DTO> PRPList(DataTable Dt)
        {
            List<PRNumberPrefix_DTO> PRRList = new List<PRNumberPrefix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRRList.Add(
                    new PRNumberPrefix_DTO
                    {
                        PRP_Number = Convert.ToInt64(dr["PRP_Number"]),
                        PRP_Date = Convert.ToString(dr["PRP_Date"]),
                        PRP_Particulars = Convert.ToString(dr["PRP_Particulars"])
                    });
            }
            return PRRList;
        }
        public List<PRNumberSuffix_DTO> PRSList(DataTable Dt)
        {
            List<PRNumberSuffix_DTO> PRRList = new List<PRNumberSuffix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRRList.Add(
                    new PRNumberSuffix_DTO
                    {
                        PRS_Number = Convert.ToInt64(dr["PRS_Number"]),
                        PRS_Date = Convert.ToString(dr["PRS_Date"]),
                        PRS_Particulars = Convert.ToString(dr["PRS_Particulars"])
                    });
            }
            return PRRList;
        }
    }
}

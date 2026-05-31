using ERP_DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DL
{
    public class SINumbering_DL
    {
        public List<SINumberReset_DTO> SIRList(DataTable Dt)
        {
            List<SINumberReset_DTO> SIRList = new List<SINumberReset_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIRList.Add(
                    new SINumberReset_DTO
                    {
                        SIR_Number = Convert.ToInt64(dr["SIR_Number"]),
                        SIR_Date = Convert.ToString(dr["SIR_Date"]),
                        SIR_StartingNumber = Convert.ToString(dr["SIR_StartingNumber"]),
                        SIR_NumberofDigits = Convert.ToString(dr["SIR_NumberofDigits"]),
                        SIR_PrefilZero = Convert.ToString(dr["SIR_PrefilZero"]),
                        SIR_Frequency = Convert.ToString(dr["SIR_Frequency"])
                    });
            }
            return SIRList;
        }
        public List<SINumberPrefix_DTO> SIPList(DataTable Dt)
        {
            List<SINumberPrefix_DTO> SIRList = new List<SINumberPrefix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIRList.Add(
                    new SINumberPrefix_DTO
                    {
                        SIP_Number = Convert.ToInt64(dr["SIP_Number"]),
                        SIP_Date = Convert.ToString(dr["SIP_Date"]),
                        SIP_Particulars = Convert.ToString(dr["SIP_Particulars"])
                    });
            }
            return SIRList;
        }
        public List<SINumberSuffix_DTO> SISList(DataTable Dt)
        {
            List<SINumberSuffix_DTO> SIRList = new List<SINumberSuffix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                SIRList.Add(
                    new SINumberSuffix_DTO
                    {
                        SIS_Number = Convert.ToInt64(dr["SIS_Number"]),
                        SIS_Date = Convert.ToString(dr["SIS_Date"]),
                        SIS_Particulars = Convert.ToString(dr["SIS_Particulars"])
                    });
            }
            return SIRList;
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

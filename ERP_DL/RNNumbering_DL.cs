using ERP_DTO;
using ERP_DTO.JobInwardTransaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DL
{
    public class RNNumbering_DL
    {
        public List<RNNumberReset_DTO> PORList(DataTable Dt)
        {
            List<RNNumberReset_DTO> PORList = new List<RNNumberReset_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PORList.Add(
                    new RNNumberReset_DTO
                    {
                        RNR_Number = Convert.ToInt64(dr["RNR_Number"]),
                        RNR_Date = Convert.ToString(dr["RNR_Date"]),
                        RNR_StartingNumber = Convert.ToString(dr["RNR_StartingNumber"]),
                        RNR_NumberofDigits = Convert.ToString(dr["RNR_NumberofDigits"]),
                        RNR_PrefilZero = Convert.ToString(dr["RNR_PrefilZero"]),
                        RNR_Frequency = Convert.ToString(dr["RNR_Frequency"])
                    });
            }
            return PORList;
        }
        public List<RNNumberPrefix_DTO> POPList(DataTable Dt)
        {
            List<RNNumberPrefix_DTO> PORList = new List<RNNumberPrefix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PORList.Add(
                    new RNNumberPrefix_DTO
                    {
                        RNP_Number = Convert.ToInt64(dr["RNP_Number"]),
                        RNP_Date = Convert.ToString(dr["RNP_Date"]),
                        RNP_Particulars = Convert.ToString(dr["RNP_Particulars"])
                    });
            }
            return PORList;
        }
        public List<RNNumberSuffix_DTO> POSList(DataTable Dt)
        {
            List<RNNumberSuffix_DTO> PORList = new List<RNNumberSuffix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PORList.Add(
                    new RNNumberSuffix_DTO  
                    {
                        RNS_Number = Convert.ToInt64(dr["RNS_Number"]),
                        RNS_Date = Convert.ToString(dr["RNS_Date"]),
                        RNS_Particulars = Convert.ToString(dr["RNS_Particulars"])
                    });
            }
            return PORList;
        }




        public List<RNNumberReset_DTO> PIRList(DataTable Dt)
        {
            List<RNNumberReset_DTO> PIRList = new List<RNNumberReset_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIRList.Add(
                    new RNNumberReset_DTO
                    {
                        RNR_Number = Convert.ToInt64(dr["RNR_Number"]),
                        RNR_Date = Convert.ToString(dr["RNR_Date"]),
                        RNR_StartingNumber = Convert.ToString(dr["RNR_StartingNumber"]),
                        RNR_NumberofDigits = Convert.ToString(dr["RNR_NumberofDigits"]),
                        RNR_PrefilZero = Convert.ToString(dr["RNR_PrefilZero"]),
                        RNR_Frequency = Convert.ToString(dr["RNR_Frequency"])
                    });
            }
            return PIRList;
        }
        public List<RNNumberPrefix_DTO> PIPList(DataTable Dt)
        {
            List<RNNumberPrefix_DTO> PIRList = new List<RNNumberPrefix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIRList.Add(
                    new RNNumberPrefix_DTO
                    {
                        RNP_Number = Convert.ToInt64(dr["RNP_Number"]),
                        RNP_Date = Convert.ToString(dr["RNP_Date"]),
                        RNP_Particulars = Convert.ToString(dr["RNP_Particulars"])
                    });
            }
            return PIRList;
        }
        public List<RNNumberSuffix_DTO> PISList(DataTable Dt)
        {
            List<RNNumberSuffix_DTO> PIRList = new List<RNNumberSuffix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PIRList.Add(
                    new RNNumberSuffix_DTO
                    {
                        RNS_Number = Convert.ToInt64(dr["RNS_Number"]),
                        RNS_Date = Convert.ToString(dr["RNS_Date"]),
                        RNS_Particulars  = Convert.ToString(dr["RNS_Particulars"])
                    });
            }
            return PIRList;
        }




        public List<RNNumberReset_DTO> PRRList(DataTable Dt)
        {
            List<RNNumberReset_DTO> PRRList = new List<RNNumberReset_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRRList.Add(
                    new RNNumberReset_DTO
                    {
                        RNR_Number = Convert.ToInt64(dr["RNR_Number"]),
                        RNR_Date = Convert.ToString(dr["RNR_Date"]),
                        RNR_StartingNumber = Convert.ToString(dr["RNR_StartingNumber"]),
                        RNR_NumberofDigits = Convert.ToString(dr["RNR_NumberofDigits"]),
                        RNR_PrefilZero = Convert.ToString(dr["RNR_PrefilZero"]),
                        RNR_Frequency = Convert.ToString(dr["RNR_Frequency"])
                    });
            }
            return PRRList;
        }
        public List<RNNumberPrefix_DTO> PRPList(DataTable Dt)
        {
            List<RNNumberPrefix_DTO> PRRList = new List<RNNumberPrefix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRRList.Add(
                    new RNNumberPrefix_DTO  
                    {
                        RNP_Number = Convert.ToInt64(dr["RNP_Number"]),
                        RNP_Date = Convert.ToString(dr["RNP_Date"]),
                        RNP_Particulars = Convert.ToString(dr["RNP_Particulars"])
                    });
            }
            return PRRList;
        }
        public List<RNNumberSuffix_DTO> PRSList(DataTable Dt)
        {
            List<RNNumberSuffix_DTO> PRRList = new List<RNNumberSuffix_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PRRList.Add(
                    new RNNumberSuffix_DTO
                    {
                        RNS_Number = Convert.ToInt64(dr["RNS_Number"]),
                        RNS_Date = Convert.ToString(dr["RNS_Date"]),
                        RNS_Particulars = Convert.ToString(dr["RNS_Particulars"])
                    });
            }
            return PRRList;
        }

    }
}

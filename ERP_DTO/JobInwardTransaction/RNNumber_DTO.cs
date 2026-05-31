using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO.JobInwardTransaction
{

    public class RNNumber_DTO
    {
        public Int64 RNN_Number { get; set; }
        public String? RNN_Method { get; set; }
        public String? RNN_Date { get; set; }
        public String? RNN_StartingNumber { get; set; }
        public String? RNN_NumberofDigits { get; set; }
        public String? RNN_PrefilZero { get; set; }
        public String? RNN_Frequency { get; set; }
        public String? RNN_Particulars { get; set; }

        public List<RNNumberReset_DTO>? RNNumberReset { get; set; }
        public List<RNNumberPrefix_DTO>? RNNumberPrefix { get; set; }
        public List<RNNumberSuffix_DTO>? RNNumberSuffix { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.RNN_Number = 0;
            this.RNN_Date = "0";
            this.RNN_Method = "0";
            this.RNN_StartingNumber = "0";
            this.RNN_NumberofDigits = "0";
            this.RNN_PrefilZero = "0";
            this.RNN_Frequency = "0";
            this.DeleteNumbers = "0";
            this.RNNumberReset = null;
            this.RNNumberPrefix = null;
            this.RNNumberSuffix = null;
        }
    }

    public class RNNumberReset_DTO
    {
        public Int64 RNR_Number { get; set; }
        public String? RNR_Date { get; set; }
        public String? RNR_StartingNumber { get; set; }
        public String? RNR_NumberofDigits { get; set; }
        public String? RNR_PrefilZero { get; set; }
        public String? RNR_Frequency { get; set; }
        public Boolean RNR_IsDeleted { get; set; }

        public void Reset()
        {
            this.RNR_Number = 0;
            this.RNR_Date = "";
            this.RNR_StartingNumber = "";
            this.RNR_NumberofDigits = "";
            this.RNR_PrefilZero = "";
            this.RNR_Frequency = "";
            this.RNR_IsDeleted = false;
        }
    }
    public class RNNumberPrefix_DTO
    {
        public Int64 RNP_Number { get; set; }
        public String? RNP_Date { get; set; }
        public String? RNP_Particulars { get; set; }
        public Boolean RNP_IsDeleted { get; set; }

        public void Reset()
        {
            this.RNP_Number = 0;
            this.RNP_Date = "";
            this.RNP_Particulars = "";
            this.RNP_IsDeleted = false;
        }
    }
    public class RNNumberSuffix_DTO
    {
        public Int64 RNS_Number { get; set; }
        public String? RNS_Date { get; set; }
        public String? RNS_Particulars { get; set; }
        public Boolean RNS_IsDeleted { get; set; }

        public void Reset()
        {
            this.RNS_Number = 0;
            this.RNS_Date = "";
            this.RNS_Particulars = "";
            this.RNS_IsDeleted = false;
        }
    }
}

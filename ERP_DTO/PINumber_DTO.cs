using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class PINumber_DTO
    {
        public Int64 PIN_Number { get; set; }
        public String? PIN_Method { get; set; }
        public String? PIN_Date { get; set; }
        public String? PIN_StartingNumber { get; set; }
        public String? PIN_NumberofDigits { get; set; }
        public String? PIN_PrefilZero { get; set; }
        public String? PIN_Frequency { get; set; }
        public String? PIN_Particulars { get; set; }

        public List<PINumberReset_DTO>? PINumberReset { get; set; }
        public List<PINumberPrefix_DTO>? PINumberPrefix { get; set; }
        public List<PINumberSuffix_DTO>? PINumberSuffix { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.PIN_Number = 0;
            this.PIN_Date = "0";
            this.PIN_Method = "0";
            this.PIN_StartingNumber = "0";
            this.PIN_NumberofDigits = "0";
            this.PIN_PrefilZero = "0";
            this.PIN_Frequency = "0";
            this.DeleteNumbers = "0";
            this.PINumberReset = null;
            this.PINumberPrefix = null;
            this.PINumberSuffix = null;
        }
    }

    public class PINumberReset_DTO
    {
        public Int64 PIR_Number { get; set; }
        public String? PIR_Date { get; set; }
        public String? PIR_StartingNumber { get; set; }
        public String? PIR_NumberofDigits { get; set; }
        public String? PIR_PrefilZero { get; set; }
        public String? PIR_Frequency { get; set; }
        public Boolean PIR_IsDeleted { get; set; }

        public void Reset()
        {
            this.PIR_Number = 0;
            this.PIR_Date = "";
            this.PIR_StartingNumber = "";
            this.PIR_NumberofDigits = "";
            this.PIR_PrefilZero = "";
            this.PIR_Frequency = "";
            this.PIR_IsDeleted = false;
        }
    }
    public class PINumberPrefix_DTO
    {
        public Int64 PIP_Number { get; set; }
        public String? PIP_Date { get; set; }
        public String? PIP_Particulars { get; set; }
        public Boolean PIP_IsDeleted { get; set; }

        public void Reset()
        {
            this.PIP_Number = 0;
            this.PIP_Date = "";
            this.PIP_Particulars = "";
            this.PIP_IsDeleted = false;
        }
    }
    public class PINumberSuffix_DTO
    {
        public Int64 PIS_Number { get; set; }
        public String? PIS_Date { get; set; }
        public String? PIS_Particulars { get; set; }
        public Boolean PIS_IsDeleted { get; set; }

        public void Reset()
        {
            this.PIS_Number = 0;
            this.PIS_Date = "";
            this.PIS_Particulars = "";
            this.PIS_IsDeleted = false;
        }
    }
}

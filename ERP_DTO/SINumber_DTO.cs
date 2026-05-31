using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class SINumber_DTO
    {
        public Int64 SIN_Number { get; set; }
        public String? SIN_Method { get; set; }
        public String? SIN_Date { get; set; }
        public String? SIN_StartingNumber { get; set; }
        public String? SIN_NumberofDigits { get; set; }
        public String? SIN_PrefilZero { get; set; }
        public String? SIN_Frequency { get; set; }
        public String? SIN_Particulars { get; set; }

        public List<SINumberReset_DTO>? SINumberReset { get; set; }
        public List<SINumberPrefix_DTO>? SINumberPrefix { get; set; }
        public List<SINumberSuffix_DTO>? SINumberSuffix { get; set; }

        public String? SIN_DeleteNumbers { get; set; }
        public Int64 SIN_CreatorCode { get; set; }
        public Int32 SIN_Id { get; set; }

        public void Reset()
        {
            this.SIN_Number = 0;
            this.SIN_Date = "0";
            this.SIN_Method = "0";
            this.SIN_StartingNumber = "0";
            this.SIN_NumberofDigits = "0";
            this.SIN_PrefilZero = "0";
            this.SIN_Frequency = "0";
            this.SIN_DeleteNumbers = "0";
            this.SINumberReset = null;
            this.SINumberPrefix = null;
            this.SINumberSuffix = null;
        }
    }

    public class SINumberReset_DTO
    {
        public Int64 SIR_Number { get; set; }
        public String? SIR_Date { get; set; }
        public String? SIR_StartingNumber { get; set; }
        public String? SIR_NumberofDigits { get; set; }
        public String? SIR_PrefilZero { get; set; }
        public String? SIR_Frequency { get; set; }
        public Boolean SIR_IsDeleted { get; set; }

        public void Reset()
        {
            this.SIR_Number = 0;
            this.SIR_Date = "";
            this.SIR_StartingNumber = "";
            this.SIR_NumberofDigits = "";
            this.SIR_PrefilZero = "";
            this.SIR_Frequency = "";
            this.SIR_IsDeleted = false;
        }
    }
    public class SINumberPrefix_DTO
    {
        public Int64 SIP_Number { get; set; }
        public String? SIP_Date { get; set; }
        public String? SIP_Particulars { get; set; }
        public Boolean SIP_IsDeleted { get; set; }

        public void Reset()
        {
            this.SIP_Number = 0;
            this.SIP_Date = "";
            this.SIP_Particulars = "";
            this.SIP_IsDeleted = false;
        }
    }
    public class SINumberSuffix_DTO
    {
        public Int64 SIS_Number { get; set; }
        public String? SIS_Date { get; set; }
        public String? SIS_Particulars { get; set; }
        public Boolean SIS_IsDeleted { get; set; }

        public void Reset()
        {
            this.SIS_Number = 0;
            this.SIS_Date = "";
            this.SIS_Particulars = "";
            this.SIS_IsDeleted = false;
        }
    }
}

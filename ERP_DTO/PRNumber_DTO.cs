using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class PRNumber_DTO
    {
        public Int64 PRN_Number { get; set; }
        public String? PRN_Method { get; set; }
        public String? PRN_Date { get; set; }
        public String? PRN_StartingNumber { get; set; }
        public String? PRN_NumberofDigits { get; set; }
        public String? PRN_PrefilZero { get; set; }
        public String? PRN_Frequency { get; set; }
        public String? PRN_Particulars { get; set; }

        public List<PRNumberReset_DTO>? PRNumberReset { get; set; }
        public List<PRNumberPrefix_DTO>? PRNumberPrefix { get; set; }
        public List<PRNumberSuffix_DTO>? PRNumberSuffix { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.PRN_Number = 0;
            this.PRN_Date = "0";
            this.PRN_Method = "0";
            this.PRN_StartingNumber = "0";
            this.PRN_NumberofDigits = "0";
            this.PRN_PrefilZero = "0";
            this.PRN_Frequency = "0";
            this.DeleteNumbers = "0";
            this.PRNumberReset = null;
            this.PRNumberPrefix = null;
            this.PRNumberSuffix = null;
        }
    }

    public class PRNumberReset_DTO
    {
        public Int64 PRR_Number { get; set; }
        public String? PRR_Date { get; set; }
        public String? PRR_StartingNumber { get; set; }
        public String? PRR_NumberofDigits { get; set; }
        public String? PRR_PrefilZero { get; set; }
        public String? PRR_Frequency { get; set; }
        public Boolean PRR_IsDeleted { get; set; }

        public void Reset()
        {
            this.PRR_Number = 0;
            this.PRR_Date = "";
            this.PRR_StartingNumber = "";
            this.PRR_NumberofDigits = "";
            this.PRR_PrefilZero = "";
            this.PRR_Frequency = "";
            this.PRR_IsDeleted = false;
        }
    }
    public class PRNumberPrefix_DTO
    {
        public Int64 PRP_Number { get; set; }
        public String? PRP_Date { get; set; }
        public String? PRP_Particulars { get; set; }
        public Boolean PRP_IsDeleted { get; set; }

        public void Reset()
        {
            this.PRP_Number = 0;
            this.PRP_Date = "";
            this.PRP_Particulars = "";
            this.PRP_IsDeleted = false;
        }
    }
    public class PRNumberSuffix_DTO
    {
        public Int64 PRS_Number { get; set; }
        public String? PRS_Date { get; set; }
        public String? PRS_Particulars { get; set; }
        public Boolean PRS_IsDeleted { get; set; }

        public void Reset()
        {
            this.PRS_Number = 0;
            this.PRS_Date = "";
            this.PRS_Particulars = "";
            this.PRS_IsDeleted = false;
        }
    }
}

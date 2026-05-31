using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class SRNumber_DTO
    {
        public Int64 SRN_Number { get; set; }
        public String? SRN_Method { get; set; }
        public String? SRN_Date { get; set; }
        public String? SRN_StartingNumber { get; set; }
        public String? SRN_NumberofDigits { get; set; }
        public String? SRN_PrefilZero { get; set; }
        public String? SRN_Frequency { get; set; }
        public String? SRN_Particulars { get; set; }

        public List<SRNumberReset_DTO>? SRNumberReset { get; set; }
        public List<SRNumberPrefix_DTO>? SRNumberPrefix { get; set; }
        public List<SRNumberSuffix_DTO>? SRNumberSuffix { get; set; }

        public String? SRN_DeleteNumbers { get; set; }
        public Int64 SRN_CreatorCode { get; set; }
        public Int32 SRN_Id { get; set; }

        public void Reset()
        {
            this.SRN_Number = 0;
            this.SRN_Date = "0";
            this.SRN_Method = "0";
            this.SRN_StartingNumber = "0";
            this.SRN_NumberofDigits = "0";
            this.SRN_PrefilZero = "0";
            this.SRN_Frequency = "0";
            this.SRN_DeleteNumbers = "0";
            this.SRNumberReset = null;
            this.SRNumberPrefix = null;
            this.SRNumberSuffix = null;
        }
    }

    public class SRNumberReset_DTO
    {
        public Int64 SRR_Number { get; set; }
        public String? SRR_Date { get; set; }
        public String? SRR_StartingNumber { get; set; }
        public String? SRR_NumberofDigits { get; set; }
        public String? SRR_PrefilZero { get; set; }
        public String? SRR_Frequency { get; set; }
        public Boolean SRR_IsDeleted { get; set; }

        public void Reset()
        {
            this.SRR_Number = 0;
            this.SRR_Date = "";
            this.SRR_StartingNumber = "";
            this.SRR_NumberofDigits = "";
            this.SRR_PrefilZero = "";
            this.SRR_Frequency = "";
            this.SRR_IsDeleted = false;
        }
    }
    public class SRNumberPrefix_DTO
    {
        public Int64 SRP_Number { get; set; }
        public String? SRP_Date { get; set; }
        public String? SRP_Particulars { get; set; }
        public Boolean SRP_IsDeleted { get; set; }

        public void Reset()
        {
            this.SRP_Number = 0;
            this.SRP_Date = "";
            this.SRP_Particulars = "";
            this.SRP_IsDeleted = false;
        }
    }
    public class SRNumberSuffix_DTO
    {
        public Int64 SRS_Number { get; set; }
        public String? SRS_Date { get; set; }
        public String? SRS_Particulars { get; set; }
        public Boolean SRS_IsDeleted { get; set; }

        public void Reset()
        {
            this.SRS_Number = 0;
            this.SRS_Date = "";
            this.SRS_Particulars = "";
            this.SRS_IsDeleted = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class PONumber_DTO
    {
        public Int64 PON_Number { get; set; }
        public String? PON_Method { get; set; }
        public String? PON_Date { get; set; }
        public String? PON_StartingNumber { get; set; }
        public String? PON_NumberofDigits { get; set; }
        public String? PON_PrefilZero { get; set; }
        public String? PON_Frequency { get; set; }
        public String? PON_Particulars { get; set; }

        public List<PONumberReset_DTO>? PONumberReset { get; set; }
        public List<PONumberPrefix_DTO>? PONumberPrefix { get; set; }
        public List<PONumberSuffix_DTO>? PONumberSuffix { get; set; }

        public String? DeleteNumbers { get; set; }
        public Int32 CreatorCode { get; set; }
        public Int32 Id { get; set; }

        public void Reset()
        {
            this.PON_Number = 0;
            this.PON_Date = "0";
            this.PON_Method = "0";
            this.PON_StartingNumber = "0";
            this.PON_NumberofDigits = "0";
            this.PON_PrefilZero = "0";
            this.PON_Frequency = "0";
            this.DeleteNumbers = "0";
            this.PONumberReset = null;
            this.PONumberPrefix = null;
            this.PONumberSuffix = null;
        }
    }

    public class PONumberReset_DTO
    {
        public Int64 POR_Number { get; set; }
        public String? POR_Date { get; set; }
        public String? POR_StartingNumber { get; set; }
        public String? POR_NumberofDigits { get; set; }
        public String? POR_PrefilZero { get; set; }
        public String? POR_Frequency { get; set; }
        public Boolean POR_IsDeleted { get; set; }

        public void Reset()
        {
            this.POR_Number = 0;
            this.POR_Date = "";
            this.POR_StartingNumber = "";
            this.POR_NumberofDigits = "";
            this.POR_PrefilZero = "";
            this.POR_Frequency = "";
            this.POR_IsDeleted = false;
        }
    }
    public class PONumberPrefix_DTO
    {
        public Int64 POP_Number { get; set; }
        public String? POP_Date { get; set; }
        public String? POP_Particulars { get; set; }
        public Boolean POP_IsDeleted { get; set; }

        public void Reset()
        {
            this.POP_Number = 0;
            this.POP_Date = "";
            this.POP_Particulars = "";
            this.POP_IsDeleted = false;
        }
    }
    public class PONumberSuffix_DTO
    {
        public Int64 POS_Number { get; set; }
        public String? POS_Date { get; set; }
        public String? POS_Particulars { get; set; }
        public Boolean POS_IsDeleted { get; set; }

        public void Reset()
        {
            this.POS_Number = 0;
            this.POS_Date = "";
            this.POS_Particulars = "";
            this.POS_IsDeleted = false;
        }
    }
}

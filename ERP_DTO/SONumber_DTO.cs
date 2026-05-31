using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class SONumber_DTO
    {
        public Int64 SON_Number { get; set; }
        public String? SON_Method { get; set; }
        public String? SON_Date { get; set; }
        public String? SON_StartingNumber { get; set; }
        public String? SON_NumberofDigits { get; set; }
        public String? SON_PrefilZero { get; set; }
        public String? SON_Frequency { get; set; }
        public String? SON_Particulars { get; set; }

        public List<SONumberReset_DTO>? SONumberReset { get; set; }
        public List<SONumberPrefix_DTO>? SONumberPrefix { get; set; }
        public List<SONumberSuffix_DTO>? SONumberSuffix { get; set; }

        public String? SON_DeleteNumbers { get; set; }
        public Int64 SON_CreatorCode { get; set; }
        public Int32 SON_Id { get; set; }

        public void Reset()
        {
            this.SON_Number = 0;
            this.SON_Date = "0";
            this.SON_Method = "0";
            this.SON_StartingNumber = "0";
            this.SON_NumberofDigits = "0";
            this.SON_PrefilZero = "0";
            this.SON_Frequency = "0";
            this.SON_DeleteNumbers = "0";
            this.SONumberReset = null;
            this.SONumberPrefix = null;
            this.SONumberSuffix = null;
        }
    }

    public class SONumberReset_DTO
    {
        public Int64 SOR_Number { get; set; }
        public String? SOR_Date { get; set; }
        public String? SOR_StartingNumber { get; set; }
        public String? SOR_NumberofDigits { get; set; }
        public String? SOR_PrefilZero { get; set; }
        public String? SOR_Frequency { get; set; }
        public Boolean SOR_IsDeleted { get; set; }

        public void Reset()
        {
            this.SOR_Number = 0;
            this.SOR_Date = "";
            this.SOR_StartingNumber = "";
            this.SOR_NumberofDigits = "";
            this.SOR_PrefilZero = "";
            this.SOR_Frequency = "";
            this.SOR_IsDeleted = false;
        }
    }
    public class SONumberPrefix_DTO
    {
        public Int64 SOP_Number { get; set; }
        public String? SOP_Date { get; set; }
        public String? SOP_Particulars { get; set; }
        public Boolean SOP_IsDeleted { get; set; }

        public void Reset()
        {
            this.SOP_Number = 0;
            this.SOP_Date = "";
            this.SOP_Particulars = "";
            this.SOP_IsDeleted = false;
        }
    }
    public class SONumberSuffix_DTO
    {
        public Int64 SOS_Number { get; set; }
        public String? SOS_Date { get; set; }
        public String? SOS_Particulars { get; set; }
        public Boolean SOS_IsDeleted { get; set; }

        public void Reset()
        {
            this.SOS_Number = 0;
            this.SOS_Date = "";
            this.SOS_Particulars = "";
            this.SOS_IsDeleted = false;
        }
    }
}

using ERP_DTO;
using System.Data;

namespace ERP.DataList
{
    public class GeneralLedgerMaster_DL
    {
        public List<Currency_DTO> CList(DataTable Dt)
        {
            List<Currency_DTO> CList = new List<Currency_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CList.Add(
                    new Currency_DTO
                    {
                        CurrencyNumber = Convert.ToInt64(dr["CurrencyNumber"]),
                        CurrencyCode = Convert.ToString(dr["CurrencyCode"]),
                        FormalName = Convert.ToString(dr["FormalName"]),
                        Symbol = Convert.ToString(dr["Symbol"]),
                        DecimalPlaces = Convert.ToString(dr["DecimalPlaces"]),
                        DecimalPortionName = Convert.ToString(dr["DecimalPortionName"]),
                        CurrencyLocation = Convert.ToString(dr["CurrencyLocation"])
                    });
            }
            return CList;
        }

        public List<CurrencyLocation_DTO> Currency(DataTable Dt, String Check)
        {
            List<CurrencyLocation_DTO> CL_DTO = new List<CurrencyLocation_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CL_DTO.Add(
                    new CurrencyLocation_DTO
                    {
                        Number = Convert.ToInt32(dr["Number"]),
                        Location = Convert.ToString(dr["Location"]),
                        Checked = Checked(Convert.ToInt32(dr["Number"]), Check)
                    });
            }
            return CL_DTO;
        }

        Boolean Checked(Int32 Number, String Check)
        {
            if (Number.ToString() == Check)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<COA_Group_DTO> CGList(DataTable Dt)
        {
            List<COA_Group_DTO> CGList = new List<COA_Group_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CGList.Add(
                    new COA_Group_DTO
                    {
                        LedgerGroupNumber = Convert.ToInt64(dr["LedgerGroupNumber"]),
                        LedgerGroup = Convert.ToString(dr["LedgerGroup"]),
                        UnderLGroup = Convert.ToString(dr["UnderLGroup"]),
                        GroupNature = Convert.ToString(dr["GroupNature"])
                    });
            }
            return CGList;
        }

        public List<COA_Ledger_DTO> CLList(DataTable Dt)
        {
            List<COA_Ledger_DTO> CLList = new List<COA_Ledger_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                CLList.Add(
                    new COA_Ledger_DTO
                    {
                        LedgerNumber = Convert.ToInt64(dr["LedgerNumber"]),
                        LedgerAccount = Convert.ToString(dr["LedgerAccount"]),
                        LedgerName = Convert.ToString(dr["LedgerName"]),
                        LedgerGroup = Convert.ToString(dr["LedgerGroup"]),
                    });
            }
            return CLList;
        }

    }
}

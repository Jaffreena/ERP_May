using ERP_DTO;
using System.Data;

namespace ERP.DataList
{
    public class GeneralLedgerSubmaster_DL
    {
        public List<GeneralLedgerSubmaster_DTO> GLList(DataTable Dt)
        {
            List<GeneralLedgerSubmaster_DTO> GLSubList = new List<GeneralLedgerSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                GLSubList.Add(
                    new GeneralLedgerSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Nature"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return GLSubList;
        }

    }
}

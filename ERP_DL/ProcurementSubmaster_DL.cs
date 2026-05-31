using ERP_DTO;
using System.Data;

namespace ERP.DataList
{
    public class ProcurementSubmaster_DL
    {
        public List<ProcurementSubmaster_DTO> PSList(DataTable Dt)
        {
            List<ProcurementSubmaster_DTO> PSubList = new List<ProcurementSubmaster_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PSubList.Add(
                    new ProcurementSubmaster_DTO
                    {
                        Number = Convert.ToInt64(dr["Number"]),
                        Title = Convert.ToString(dr["Payment"]),
                        Notes = Convert.ToString(dr["Notes"])
                    });
            }
            return PSubList;
        }

    }
}

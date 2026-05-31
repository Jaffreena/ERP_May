using ERP_DTO;
using System.Data;

namespace ERP.DataList
{
    public class AddressType_DL
    {
        public List<AddressType_DTO> AddressTList(DataTable Dt)
        {
            List<AddressType_DTO> AT_List = new List<AddressType_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                AT_List.Add(
                    new AddressType_DTO
                    {
                        ADTP_Number = Convert.ToInt64(dr["ADTP_Number"]),
                        ADTP_Name = Convert.ToString(dr["ADTP_Name"]),
                        ADTP_Notes = Convert.ToString(dr["ADTP_Notes"])
                    });
            }
            return AT_List;
        }
    }
}

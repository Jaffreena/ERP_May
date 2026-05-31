using ERP_DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.DataList
{
    public class Shift_DL
    {
        public List<Shift_DTO> PSList(DataTable Dt)
        {
            List<Shift_DTO> PSubList = new List<Shift_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                PSubList.Add(
                    new Shift_DTO
                    {
                        Number = Convert.ToInt64(dr["SFT_Number"]),
                        ShiftName = Convert.ToString(dr["SFT_ShiftName"]),
                        Description = Convert.ToString(dr["SFT_Description"])
                    });
            }
            return PSubList;
        }

    }
}

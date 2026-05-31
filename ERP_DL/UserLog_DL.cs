using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP_DTO;

namespace ERP_DAO
{
    public class UserLog_DL
    {
        DBConnect DB = new DBConnect();
        DataSet DS = new DataSet();
        public List<UserLog_DTO> LogList(DataTable Dt)
        {
            List<UserLog_DTO> ULList = new List<UserLog_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ULList.Add(
                    new UserLog_DTO
                    {
                        Number = Convert.ToInt32(dr["Number"]),
                        Username = Convert.ToString(dr["Username"]),
                        Password = Convert.ToString(dr["Password"]),
                        Theme = Convert.ToInt32(dr["Theme"])
                    });
            }
            return ULList;
        }
        public List<UserLog_DTO> ThemeEdit(DataTable Dt)
        {
            List<UserLog_DTO> ULList = new List<UserLog_DTO>();
            foreach (DataRow dr in Dt.Rows)
            {
                ULList.Add(
                    new UserLog_DTO
                    {
                        Number = Convert.ToInt32(dr["Number"]),
                        Theme = Convert.ToInt32(dr["Theme"]),
                    });
            }
            return ULList;
        }
    }
}

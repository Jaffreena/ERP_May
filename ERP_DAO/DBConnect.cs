using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DAO
{
    public class DBConnect
    {
        public String Connection()
        {
            string constring = "";
            constring = "Server=localhost;Initial Catalog=Job_Inward; Integrated Security=True;TrustServerCertificate=True;";
            //constring = "Server=MRSOFT;Initial Catalog=MRS_ERP; User ID=sa; Password=Mind@123";
            return constring;
        }
    }
}

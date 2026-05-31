using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models
{
    public class Validation
    {
        Alerts Alert=new Alerts();
        public String CatchValid(String Catch)
        {
            if(Catch.Contains("Cannot insert duplicate key"))
            {
                return Alert.Duplicate();
            }
            else if(Catch.Contains("TABLE REFERENCE"))
            {
                return Alert.DeleteReference();
            }
            else if (Catch.Contains("REFERENCE constraint"))
            {
                return Alert.DeleteReference();
            }
            else if (Catch.Contains("Cannot insert the value NULL"))
            {
                return Alert.DeleteReference();
            }
            else
            {
                return Catch;
            }
        }
    }
}

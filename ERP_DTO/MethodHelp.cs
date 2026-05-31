using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_DTO
{
    public class MethodHelp
    {
        CultureInfo India = new CultureInfo("hi-IN");
        bool Boolean(String Get, String Set)
        {
            if (Get.Equals(Set))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public String DecimalConvertQty(Double Qty)
        {
            return String.Format(India, "{0:#,0.##}", Qty);
        }
        public String DecimalConvertUnit(Double UnitPrice)
        {
            return String.Format(India, "{0:#,0.00##}", UnitPrice);
        }
        public String DecimalConvertFixed(Double Amount)
        {
            return String.Format(India, "{0:#,0.00}", Amount);
        }
    }
}

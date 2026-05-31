using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models
{
    public class Alerts
    {
        public String Save()
        {
            return "Saved successfully";
        }
        public String Delete()
        {
            return "Deleted successfully";
        }
        public  String Edit()
        {
            return "Updated successfully";
        }
        public String DeleteConfirm()
        {
            return "Do you want to delete it ?";
        }
        public String EditConfirm()
        {
            return "Do you want to edit it ?";
        }
        public String Duplicate()
        {
            return "Duplicate Entry";
        }
        public String Required(String Form)
        {
            return Form + " is required";
        }
        public String Record()
        {
            return "Record Not Found";
        }
        public String DeleteReference()
        {
            return "This record can't be deleted as it is used in another table !";
        }
    }
}

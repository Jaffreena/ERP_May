using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Globalization;

namespace ERP.Models
{
    public class Help
    {
        CultureInfo India = new CultureInfo("hi-IN");
        public String Web()
        {
            //return "https://erp.ayiarak.co.in/";
            //return "https://erpdesign.ayiarak.co.in/";
            return "https://localhost:7237/";
        }
        public String WebName()
        {
            return "ERP";
        }
        public List<SelectListItem> GetUnder(DataTable Dt)
        {
            List<SelectListItem> Mode = new List<SelectListItem>();
            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                Mode.Add(new SelectListItem { Text = Dt.Rows[i][1].ToString(), Value = Dt.Rows[i][0].ToString() });
            }
            return Mode;
        }
        public List<SelectListItem> GetCat(DataTable Dt)
        {
            List<SelectListItem> Mode = new List<SelectListItem>();
            Mode.Add(new SelectListItem { Text = "", Value = "" });
            for (int i = 0; i < Dt.Rows.Count; i++)
            {
                Mode.Add(new SelectListItem { Text = Dt.Rows[i][1].ToString(), Value = Dt.Rows[i][0].ToString() });
            }
            return Mode;
        }
        public List<SelectListItem> PageSize(String PageSize)
        {
            List<SelectListItem> Page = new List<SelectListItem>();
            Page.Add(new SelectListItem { Text = "10", Value = "10", Selected = Boolean("10", PageSize) });
            Page.Add(new SelectListItem { Text = "25", Value = "25", Selected = Boolean("25", PageSize) });
            Page.Add(new SelectListItem { Text = "50", Value = "50", Selected = Boolean("50", PageSize) });

            return Page;
        }
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

        public T JsonClone<T>(T obj)
        {
            if (obj == null) return default(T);
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public String DecimalConvertQty(Double Qty)
        {
            return String.Format(India, "{0:#,0.##}", Qty);
        }
        public String DecimalConvertUnit(Double UnitPrice)
        {
            return String.Format(India, "{0:#,0.00##}", UnitPrice);
        }
    }
}

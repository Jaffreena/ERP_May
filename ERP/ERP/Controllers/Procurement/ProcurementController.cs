using ERP.DataList;
using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Globalization;

namespace ERP.Controllers.Procurement
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class ProcurementController : Controller
    {
        CultureInfo India = new CultureInfo("hi-IN");
        Alerts Alert = new Alerts();
        Help Help = new Help();
        Validation Valid = new Validation();
        Procurement_DL P_DL = new Procurement_DL();

        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        DataSet DS = new DataSet();
        Int32? DPageNumber;
        Int32 DPageSize;

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;







        [Route("procurement")]
        public IActionResult Index()
        {
            return View();
        }



        [Route("table-test")]
        public IActionResult TableIndex()
        {
            return View();
        }
    }
}

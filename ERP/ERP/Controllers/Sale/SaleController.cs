using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ERP.Controllers.sale
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class SaleController : Controller
    {

        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        DataSet DS = new DataSet();

        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;



        [Route("sale")]
        public IActionResult Index()
        {
            return View();
        }
    } 
}

using ERP.Models;
using ERP_DAO;
using ERP_DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace ERP.Controllers
{
    [Authorize(AuthenticationSchemes = "ERPAdminCookies")]
    public class HomeController : Controller
    {
        UserLog_DTO UL_DTO = new UserLog_DTO();
        UserLog_DAO UL_DAO = new UserLog_DAO();
        UserLog_DL UL_DL = new UserLog_DL();
        DataSet DS = new DataSet();
        private readonly ILogger<HomeController> _logger;
        public Int64 UserCode => Int64.TryParse(User.FindFirst("ERP_ID")?.Value, out var No) ? No : 0;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index(String ReturnUrl)
        {
            if (User.Identity.IsAuthenticated && Request.Cookies["ERPAdmin.Auth"] != null)
            {
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View();
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(UserLog_DTO UL_DTO, String ReturnUrl)
        {
            if (UL_DTO != null)
            {
                UL_DTO.Id = 1;
                DS = UL_DAO.UserDb(UL_DTO);
                List<UserLog_DTO> Log = UL_DL.LogList(DS.Tables[0]);
                if (Log != null)
                {
                    var use = Log.Where(e => e.Username == UL_DTO.Username && e.Password == UL_DTO.Password);
                    if (use != null)
                    {
                        foreach (var item in use)
                        {

                            var Claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, item.Username),
                                new Claim("ERP_ID", item.Number.ToString()),
                                new Claim("ERP_THEME", item.Theme.ToString()),
                                //new Claim("MRS_LogId", DS.Tables[0].Rows[0][0].ToString()),
                                new Claim(ClaimTypes.Authentication, "Admin")
                            };

                            //var authProperties = new AuthenticationProperties
                            //{
                            //    IsPersistent = L_DTO.UL_Rem,
                            //    ExpiresUtc = L_DTO.UL_Rem ? DateTimeOffset.UtcNow.AddDays(30) : null
                            //};

                            var identity = new ClaimsIdentity(Claims, "ERPAdminCookies");
                            await HttpContext.SignInAsync("ERPAdminCookies", new ClaimsPrincipal(identity));


                            if (!string.IsNullOrEmpty(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            return RedirectToAction("Dashboard");
                            //HttpContext.Session.SetInt32("ERPUser", item.Number);
                            //return RedirectToAction("Dashboard");
                        }
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = "Username or Password is wrong";
                    }
                    else
                    {
                        ViewBag.ErrorCode = 2;
                        ViewBag.ErrorMessage = "Username or Password is wrong";
                    }
                }
                else
                {
                    ViewBag.ErrorCode = 2;
                    ViewBag.ErrorMessage = "Username or Password is wrong";
                }
            }
            return View();
        }


        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Route("theme")]
        public IActionResult Theme()
        {
            UL_DTO.Number = Convert.ToInt32(UserCode);
            UL_DTO.Id = 2;
            DS = UL_DAO.UserDb(UL_DTO);
            var User = UL_DL.ThemeEdit(DS.Tables[0]).FirstOrDefault();
            return View(User);
        }
        [Route("theme")]
        [HttpPost]
        public IActionResult Theme(UserLog_DTO UL_DTO)
        {
            UL_DTO.Number = Convert.ToInt32(UserCode);
            UL_DTO.Id = 3;
            DS = UL_DAO.UserDb(UL_DTO);
            var User = UL_DL.ThemeEdit(DS.Tables[0]).FirstOrDefault();
            return View(User);
        }


        [Route("log-out")]
        public ActionResult LogOut()
        {
            HttpContext.SignOutAsync("AdminCookies");
            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

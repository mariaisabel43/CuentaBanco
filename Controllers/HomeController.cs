using Cuenta_Bancaria.Models;
using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Cuenta_Bancaria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController>_logger;

        public UserModel GetSessionInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("userSession")))
                {
                    UserModel? bankuser = JsonConvert.DeserializeObject<UserModel>(HttpContext.Session.GetString("userSession"));

                    return bankuser;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public ActionResult Principal()
        {
            UserModel? bankuser = GetSessionInfo();
            if (bankuser != null)
            {
                ViewBag.User = bankuser;
                return View();
            }
           return View();
        }
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
    }
}

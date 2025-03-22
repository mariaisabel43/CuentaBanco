using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Cuenta_Bancaria.Firebase;
using Cuenta_Bancaria.Models;

namespace Cuenta_Bancaria.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                UserHelper userHelper = new UserHelper();
                UserCredential userCredential = await FirebaseAuthHelper.setFirebaseAuthClient().SignInWithEmailAndPasswordAsync(email, password);
                UserModel bankuser = await userHelper.getUserInfo(email);
                HttpContext.Session.SetString("userSession", JsonConvert.SerializeObject(bankuser));
                return RedirectToAction("Principal", "Home");
            }
            catch
            {
                TempData["Error"] = "Invalid email or password.";
                return RedirectToAction("Index");
            }
        }
        public IActionResult LogOut(int id)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }

}
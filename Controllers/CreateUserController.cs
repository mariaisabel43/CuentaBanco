using Cuenta_Bancaria.Firebase;
using Cuenta_Bancaria.Models;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cuenta_Bancaria.Controllers
{
    public class CreateUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Register(string email, string password, string name, int capital)
		{
			try
			{
				if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
				{
					TempData["Error"] = "All fields are required.";
					return RedirectToAction("Index");
				}
				UserCredential userCredential = await FirebaseAuthHelper.setFirebaseAuthClient().CreateUserWithEmailAndPasswordAsync(email, password);
				string id = userCredential.User.Uid; 
				UserModel BankUser = new UserModel
				{
					Id = id,
					Email = email,
					Name = name,
					Capital = capital
				};
				UserHelper userHelper = new UserHelper();
				bool isUserCreated = await userHelper.CreateUser(BankUser);

				if (!isUserCreated)
				{
					await userCredential.User.DeleteAsync();
					TempData["Error"] = "Error saving user information.";
					return RedirectToAction("Index");
				}
				HttpContext.Session.SetString("userSession", JsonConvert.SerializeObject(BankUser));
				return RedirectToAction("Principal", "Home");
			}
			catch (FirebaseAuthException ex)
			{
				TempData["Error"] = "Authentication error.";

                return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Error registering user.";
				return RedirectToAction("Index");
			}
		}
		
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Cuenta_Bancaria.Models;
using System.Transactions;

namespace Cuenta_Bancaria.Controllers
{
    public class TransactionsController : Controller
    {
        // GET: TransactionsController
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
        public ActionResult Index()
        {
            UserModel? bankuser = GetSessionInfo();

            if (bankuser != null)
            {
                ViewBag.User = bankuser;
                List<TransactionsModel> transactionsList = TransactionsHelper.GetTransactions(bankuser.Email).Result;
                ViewBag.Transactions = transactionsList;
                HttpContext.Session.SetString("transactionList", JsonConvert.SerializeObject(transactionsList));
                return View();
            }

            return RedirectToAction("Create");
        }

        public ActionResult Create()
        {
            UserModel? bankuser = GetSessionInfo();

            if (bankuser != null)
            {
                ViewBag.User = bankuser;
                return View();
            }
            return RedirectToAction("Create");
        }

        public ActionResult Balance()
        {
            UserModel? bankuser = GetSessionInfo();

            if (bankuser != null)
            {
                ViewBag.User = bankuser;
                return View();
            }
            return RedirectToAction("Balance");
        }
//Realización de transacciones.
//Verificación de montos superiores y validación de correos.
        public ActionResult createTransaction(string emailToUser, int txtTotalAmount)
        {
            UserHelper userHelper = new UserHelper();
            UserModel? bankuser = GetSessionInfo();
            if (bankuser != null)
            {
                if (txtTotalAmount < bankuser.Capital)
                {
                    List<UserModel> usersList = userHelper.getAllUsers().Result;
                    foreach (var userData in usersList)
                    {
                        if (emailToUser.Equals(userData.Email))
                        {
                            if (emailToUser.Equals(bankuser.Email))
                            {
                                TempData["SuccessMessage"] = "You cannot perform a transaction to yourself.";
                                return RedirectToAction("Create");
                            }
                            else
                            {
                                string email = bankuser.Email;
                                string name = bankuser.Name;
                                int newTotalAmount = bankuser.Capital - txtTotalAmount;
                                int QuantitySent = userData.Capital + txtTotalAmount;

                                TransactionsHelper transactionsHelper = new TransactionsHelper();                                
                                bool result = transactionsHelper.RegisterTransaction(new TransactionsModel
                                {
                                    FromUser = bankuser.Email,
                                    ToUser = emailToUser,
                                    TotalAmount = txtTotalAmount,
                                    Date = DateTime.Now.ToString(),
                                }).Result;
                                bool result_2 = userHelper.UpdateUserInfo(new UserModel
                                {
                                    Id = bankuser.Id,
                                    Email = email,
                                    Name = name,
                                    Capital = newTotalAmount,
                                }).Result;
                                bool result_3 = userHelper.MakeTransaction(new UserModel
                                {
                                    Id = userData.Id,
                                    Email = emailToUser,
                                    Name = userData.Name,
                                    Capital = QuantitySent,
                                }).Result;
                                if (result && result_2 && result_3)
                                {
                                    TempData["SuccessMessage"] = "Successful transaction.";
                                    return RedirectToAction("Create"); 
                                }
                                else
                                {
                                    TempData["Error"] = "There was an error in the transaction.";
                                    return RedirectToAction("Create");
                                }
                            }
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Insufficient funds to carry out the transaction.";
                    return RedirectToAction("Create");
                }
            }
            TempData["Error"] = "Unauthenticated user.";
            return RedirectToAction("Create");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
       
    }
}

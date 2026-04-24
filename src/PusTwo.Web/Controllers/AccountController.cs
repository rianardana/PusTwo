using Microsoft.AspNetCore.Mvc;


namespace PusTwo.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            
            return RedirectToAction("Login");
        }
    }
}
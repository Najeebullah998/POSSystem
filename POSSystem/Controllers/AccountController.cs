using Microsoft.AspNetCore.Mvc;

namespace POSSystem.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace POSSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountRepository _repo;
        public AccountController(AccountRepository repo)
        {
                _repo = repo;
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string username, string password)
        {
            var user = _repo.Login(username, password);

            if (user != null)
            {
                // User Information
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.UserName);

                // Branch Information
                HttpContext.Session.SetInt32("BranchId", user.BranchId);
                HttpContext.Session.SetString("BranchName", user.BranchName);

                // Company Information
                HttpContext.Session.SetInt32("CompanyId", user.CompanyId);
                HttpContext.Session.SetString("CompanyName", user.CompanyName);

                // Role Information
                HttpContext.Session.SetString("RoleName", user.RoleName);

                // Business Type Information
                HttpContext.Session.SetInt32("BusinessTypeId", user.BusinessTypeId);
                HttpContext.Session.SetString("BusinessTypeName", user.BusinessName);

                return Json(new
                {
                    success = true
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid Username or Password"
                });
            }
        }
    }
}

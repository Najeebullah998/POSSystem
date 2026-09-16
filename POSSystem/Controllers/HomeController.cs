using Microsoft.AspNetCore.Mvc;
using POSSystem.Interfaces;
using POSSystem.Models;
using POSSystem.Repositories;
using System.Diagnostics;

namespace POSSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDashboardRepository _repo;
        public HomeController(IDashboardRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                int companyId =
                    HttpContext.Session.GetInt32("CompanyId") ?? 0;

                int branchId =
                    HttpContext.Session.GetInt32("BranchId") ?? 0;

                int userId =
                    HttpContext.Session.GetInt32("UserId") ?? 0;

                if (companyId <= 0 || branchId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Company or Branch session not found."
                    });
                }

                var result =
                    await _repo.GetDashboardSummaryAsync(
                        companyId,
                        branchId,
                        userId
                    );

                return Json(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
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

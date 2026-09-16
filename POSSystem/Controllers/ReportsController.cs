using Microsoft.AspNetCore.Mvc;
using POSSystem.Interfaces;

namespace POSSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportsRepo _repo;
        public ReportsController(IReportsRepo repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DailySalesReport()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetDailySalesReport(DateTime fromDate,DateTime toDate)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
                int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

                if (companyId == 0 || branchId == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Company or Branch not found in session."
                    });
                }

                var result = await _repo.GetDailySalesReportAsync(companyId,branchId,fromDate,toDate);

                return Json(new
                {
                    success = true,
                    summary = result.Summary,
                    details = result.Details
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
    }
}

using Microsoft.AspNetCore.Mvc;
using POSSystem.Interfaces;
using POSSystem.Repositories;

namespace POSSystem.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryRepository _repo;

        public InventoryController(IInventoryRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetWarehouses()
        {
            try
            {
                int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
                var warehouses = await _repo.GetWarehouseDDAsync();
                return Json(new { success = true, data = warehouses });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
                var items = await _repo.GetItemsDDAsync();
                return Json(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetStock(int? warehouseId = null,int? itemId = null,string batchNo = null)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
                int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

                var stock = await _repo.GetStockAsync(
                    companyId,
                    branchId,
                    warehouseId,
                    itemId,
                    batchNo
                );

                return Json(new
                {
                    success = true,
                    data = stock
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

using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;

namespace POSSystem.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly IWarehouse _repo;

        public WarehousesController(IWarehouse repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWarehouse()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            var warehouse = await _repo.GetAllAsync(companyId, branchId);

            return Json(warehouse);
        }

        [HttpGet]
        public IActionResult AddWarehouse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouse([FromBody] Warehouse warehouse)
        {
            if (warehouse == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            warehouse.CreatedBy = userId;

            // Repository WarehouseId generate karega
            int id = await _repo.AddAsync(
                warehouse,
                companyId,
                branchId
            );

            if (id > 0)
            {
                return Json(new
                {
                    success = true,
                    message = "Warehouse saved successfully",
                    warehouseId = id
                });
            }

            return Json(new
            {
                success = false,
                message = "Error saving Warehouse"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouseById(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            var warehouse = await _repo.GetByIdAsync(
                id,
                companyId,
                branchId
            );

            return Json(warehouse);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWarehouse(
            [FromBody] Warehouse warehouse)
        {
            if (warehouse == null || warehouse.WarehouseId == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            warehouse.ModifiedBy = userId;

            await _repo.UpdateAsync(
                warehouse,
                companyId,
                branchId
            );

            return Json(new
            {
                success = true,
                message = "Warehouse updated successfully"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            await _repo.DeleteAsync(
                id,
                companyId,
                branchId,
                userId
            );

            return Json(new
            {
                success = true,
                message = "Warehouse deleted successfully"
            });
        }
    }
}

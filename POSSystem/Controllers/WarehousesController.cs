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
            var warehouse = await _repo.GetAllAsync();
            return Json(warehouse);
        }
        public IActionResult AddWarehouse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddWarehouse([FromBody] Warehouse warehouse)
        {
            if (warehouse == null)
                return Json(new { success = false, message = "Invalid data" });

            warehouse.CreatedBy = 1; 
            warehouse.IsActive = warehouse.IsActive;

            int id = await _repo.AddAsync(warehouse);

            if (id > 0)
            {
                return Json(new { success = true, message = "Warehouse saved successfully" });
            }

            return Json(new { success = false, message = "Error saving Warehouse" });
        }
    }
}

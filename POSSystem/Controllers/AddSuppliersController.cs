using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;

namespace POSSystem.Controllers
{
    public class AddSuppliersController : Controller
    {
        private readonly ISupplier _repo;

        public AddSuppliersController(ISupplier repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var suppliers = await _repo.GetAllAsync();
            return Json(suppliers);
        }

        [HttpGet]
        public IActionResult AddSupplier()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] Supplier supplier)
        {
            if (supplier == null)
                return Json(new { success = false, message = "Invalid data" });

            supplier.CreatedBy = 1;
            supplier.IsActive = supplier.IsActive;

            int id = await _repo.AddAsync(supplier);

            if (id > 0)
            {
                return Json(new
                {
                    success = true,
                    message = "Supplier saved successfully"
                });
            }

            return Json(new
            {
                success = false,
                message = "Error saving supplier"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);
            return Json(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSupplier([FromBody] Supplier supplier)
        {
            if (supplier == null || supplier.SupplierId == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            supplier.ModifiedBy = 1;

            await _repo.UpdateAsync(supplier);

            return Json(new
            {
                success = true,
                message = "Supplier updated successfully"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            await _repo.DeleteAsync(id);

            return Json(new
            {
                success = true,
                message = "Supplier deleted successfully"
            });
        }
    }
}

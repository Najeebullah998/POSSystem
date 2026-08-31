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
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            var suppliers = await _repo.GetAllAsync(companyId, branchId);

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
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            supplier.CreatedBy = 1;

            int id = await _repo.AddAsync(
                supplier,
                companyId,
                branchId
            );

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
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            var supplier = await _repo.GetByIdAsync(
                id,
                companyId,
                branchId
            );

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

            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            supplier.ModifiedBy = 1;

            await _repo.UpdateAsync(
                supplier,
                companyId,
                branchId
            );

            return Json(new
            {
                success = true,
                message = "Supplier updated successfully"
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            int userId = 1;

            await _repo.DeleteAsync(
                id,
                companyId,
                branchId,
                userId
            );

            return Json(new
            {
                success = true,
                message = "Supplier deleted successfully"
            });
        }
    }
}

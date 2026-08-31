using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;

namespace POSSystem.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategory _repo;

        public CategoriesController(ICategory repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            int branchId = Convert.ToInt32(HttpContext.Session.GetInt32("BranchId"));
            var Categories = await _repo.GetAllAsync(companyId,branchId);
            return Json(Categories);
        }
        public IActionResult AddCategories()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategories([FromBody] ItemCategory Categories)
        {
            if (Categories == null)
                return Json(new { success = false, message = "Invalid data" });

            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            int branchId = Convert.ToInt32(HttpContext.Session.GetInt32("BranchId"));
            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

            Categories.CreatedBy = userId;

            int id = await _repo.AddAsync(Categories, companyId, branchId);

            if (id > 0)
            {
                return Json(new
                {
                    success = true,
                    message = "Category saved successfully"
                });
            }

            return Json(new
            {
                success = false,
                message = "Error saving Category"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            int branchId = Convert.ToInt32(HttpContext.Session.GetInt32("BranchId"));

            var category = await _repo.GetByIdAsync(id, companyId, branchId);

            return Json(category);
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            int branchId = Convert.ToInt32(HttpContext.Session.GetInt32("BranchId"));

            var categories = await _repo.GetAllAsync(companyId, branchId);

            return Json(categories);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory([FromBody] ItemCategory category)
        {
            if (category == null || category.CategoryId == 0)
                return Json(new { success = false, message = "Invalid data" });

            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            int branchId = Convert.ToInt32(HttpContext.Session.GetInt32("BranchId"));
            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

            category.ModifiedBy = userId;

            await _repo.UpdateAsync(category, companyId, branchId);

            return Json(new
            {
                success = true,
                message = "Category updated successfully"
            });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            int companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));
            int branchId = Convert.ToInt32(HttpContext.Session.GetInt32("BranchId"));
            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

            await _repo.DeleteAsync(id, companyId, branchId, userId);

            return Json(new
            {
                success = true,
                message = "Category deleted successfully"
            });
        }
    }
}

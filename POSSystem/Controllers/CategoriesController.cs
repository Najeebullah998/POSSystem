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
            var Categories = await _repo.GetAllAsync();
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

            Categories.CreatedBy = 1; // session se le sakte ho
            Categories.IsActive = Categories.IsActive;

            int id = await _repo.AddAsync(Categories);

            if (id > 0)
            {
                return Json(new { success = true, message = "Category saved successfully" });
            }

            return Json(new { success = false, message = "Error saving Category" });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;

namespace POSSystem.Controllers
{
    public class ItemsInfoController : Controller
    {
        private readonly Iitems _repo;

        public ItemsInfoController(Iitems repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllitems()
        {
            var items = await _repo.GetAllAsync();
            return Json(items);
        }
        public IActionResult Additems()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Additems([FromBody] Item items)
        {
            if (items == null)
                return Json(new { success = false, message = "Invalid data" });
            
            items.CreatedBy = 1; 
            items.IsActive = items.IsActive;

            int id = await _repo.AddAsync(items);

            if (id > 0)
            {
                return Json(new { success = true, message = "items saved successfully" });
            }

            return Json(new { success = false, message = "Error saving items" });
        }
    }
}

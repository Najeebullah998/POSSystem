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

        [HttpGet]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            return Json(item);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateItem([FromBody] Item item)
        {
            if (item == null || item.ItemId == 0)
                return Json(new { success = false, message = "Invalid data" });

            item.ModifiedBy = 1;

            await _repo.UpdateAsync(item);

            return Json(new { success = true, message = "Item updated successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _repo.DeleteAsync(id);
            return Json(new { success = true, message = "Item deleted successfully" });
        }
    }
}

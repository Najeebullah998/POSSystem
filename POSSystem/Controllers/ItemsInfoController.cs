using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

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
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            var items = await _repo.GetAllAsync(companyId, branchId);

            return Json(items);
        }


        [HttpGet]
        public async Task<IActionResult> Additems()
        {
            var model = new Item();

            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            var units = await _repo.GetUnitAsync(companyId, branchId);

            model.UnitList = units.Select(x => new SelectListItem
            {
                Value = x.Value.ToString(),
                Text = x.Text
            }).ToList();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Additems([FromBody] Item items)
        {
            if (items == null)
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

            items.CreatedBy = userId;

            int id = await _repo.AddAsync(
                items,
                companyId,
                branchId
            );

            if (id > 0)
            {
                return Json(new
                {
                    success = true,
                    message = "Item saved successfully",
                    itemId = id
                });
            }

            return Json(new
            {
                success = false,
                message = "Error saving item"
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetItemById(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            var item = await _repo.GetByIdAsync(
                id,
                companyId,
                branchId
            );

            return Json(item);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateItem([FromBody] Item item)
        {
            if (item == null || item.ItemId == 0)
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

            item.ModifiedBy = userId;

            await _repo.UpdateAsync(
                item,
                companyId,
                branchId
            );

            return Json(new
            {
                success = true,
                message = "Item updated successfully"
            });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
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
                message = "Item deleted successfully"
            });
        }
    }
}

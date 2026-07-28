using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;

namespace POSSystem.Controllers
{
    public class CreateGRNController : Controller
    {
        private readonly IGRNRepository _repo;
        public CreateGRNController(IGRNRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrder(int id)
        {
            var result = await _repo.GetPurchaseOrderByIdAsync(id);

            if (result == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Purchase Order not found."
                });
            }

            return Json(new
            {
                success = true,
                data = result
            });
        }
        public async Task<IActionResult> CreateAndEditGRN()
        {
            var model = new GRNHeaderVM();
            model.GRNNumber = await _repo.GenerateGRNNumberAsync();
            model.SupplierList = await _repo.GetSupplierDDAsync();
            model.WarehouseList = await _repo.GetWarehouseDDAsync();
            model.PurchaseOrderList = await _repo.GetPurchaseOrderList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGRN([FromBody] GRNHeaderVM model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid request."
                    });
                }

                // Sirf received items rakho
                model.Details = model.Details
                                     .Where(x => x.ReceivedQty > 0)
                                     .ToList();

                if (!model.Details.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please enter received quantity."
                    });
                }

                var result = await _repo.SaveGRNAsync(model);

                if (result > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "GRN Saved Successfully.",
                        id = result
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Unable to save GRN."
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

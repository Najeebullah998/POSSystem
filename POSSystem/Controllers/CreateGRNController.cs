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

            model.SupplierList = await _repo.GetSupplierDDAsync();
            model.WarehouseList = await _repo.GetWarehouseDDAsync();
            model.PurchaseOrderList = await _repo.GetPurchaseOrderList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(GRNHeaderVM model)
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

                if (model.Details == null || !model.Details.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please add at least one item."
                    });
                }

                if (model.Details.Any(x => x.ReceivedQty <= 0))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Received Quantity must be greater than zero."
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

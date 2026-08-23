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

                // ==========================================
                // Get Logged-in User Information from Session
                // ==========================================

                int? companyId =
                    HttpContext.Session.GetInt32("CompanyId");

                int? branchId =
                    HttpContext.Session.GetInt32("BranchId");

                int? userId =
                    HttpContext.Session.GetInt32("UserId");


                if (!companyId.HasValue || companyId.Value <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Company is not configured in session."
                    });
                }

                if (!branchId.HasValue || branchId.Value <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Branch is not configured in session."
                    });
                }

                if (!userId.HasValue || userId.Value <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User is not configured in session."
                    });
                }


                // ==========================================
                // Override values from frontend
                // ==========================================

                model.CompanyId = companyId.Value;

                model.BranchId = branchId.Value;

                model.CreatedBy = userId.Value;


                // ==========================================
                // Validate Details
                // ==========================================

                if (model.Details == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "GRN details are required."
                    });
                }


                // ==========================================
                // Sirf received items rakho
                // ==========================================

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


                // ==========================================
                // Save GRN
                // ==========================================

                var result =
                    await _repo.SaveGRNAsync(model);


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

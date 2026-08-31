using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;

namespace POSSystem.Controllers
{
    public class PurchaseReturnController : Controller
    {
        private readonly IPurchaseReturnRepository _repo;
        public PurchaseReturnController(IPurchaseReturnRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetGRNItems(int grnId)
        {
            try
            {
                if (grnId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid GRN Id"
                    });
                }

                var result = await _repo.GetGRNForPurchaseReturnAsync(grnId);

                if (result == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "GRN not found"
                    });
                }

                return Json(new
                {
                    success = true,
                    data = result
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
        public async Task<IActionResult> CreatePurchaseReturn()
        {
            var model = new PurchaseReturnHeaderVM();

            model.ReturnNumber = await _repo.GenerateReturnNumberAsync();

            model.GRNList = await _repo.GetGRNListAsync();
            model.SupplierList = await _repo.GetSupplierDDAsync();
            model.WarehouseList = await _repo.GetWarehouseDDAsync();

            return View(model);
        }

        // =========================
        // SAVE PURCHASE RETURN (AJAX)
        // =========================
        [HttpPost]
        public async Task<IActionResult> SavePurchaseReturn([FromBody] PurchaseReturnHeaderVM model)
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
                // Get Session Values
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
                // Override frontend values
                // ==========================================

                model.CompanyId = companyId.Value;
                model.BranchId = branchId.Value;
                model.CreatedBy = userId.Value;


                // ==========================================
                // Validate Details
                // ==========================================

                if (model.Details == null || !model.Details.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Purchase Return details are required."
                    });
                }


                // ==========================================
                // Sirf Return Qty > 0 wale items
                // ==========================================

                model.Details = model.Details
                    .Where(x => x.ReturnQty > 0)
                    .ToList();


                if (!model.Details.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please enter return quantity."
                    });
                }


                // ==========================================
                // Save Purchase Return
                // ==========================================

                var result =
                    await _repo.SavePurchaseReturnAsync(model);


                if (result > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Purchase Return Saved Successfully.",
                        id = result
                    });
                }


                return Json(new
                {
                    success = false,
                    message = "Unable to save Purchase Return."
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

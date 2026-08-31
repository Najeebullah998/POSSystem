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
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            if (companyId == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Company not found in session."
                });
            }

            if (branchId == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Branch not found in session."
                });
            }

            var result = await _repo.GetPurchaseOrderByIdAsync(
                id,
                companyId,
                branchId
            );

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
                // Get Company, Branch & User from Session
                // ==========================================

                int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
                int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

                if (companyId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Company is not configured in session."
                    });
                }

                if (branchId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Branch is not configured in session."
                    });
                }

                if (userId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User is not configured in session."
                    });
                }

                // ==========================================
                // Always use Session values
                // ==========================================

                model.CompanyId = companyId;
                model.BranchId = branchId;
                model.CreatedBy = userId;

                // ==========================================
                // Validate Details
                // ==========================================

                if (model.Details == null || !model.Details.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "GRN details are required."
                    });
                }

                // ==========================================
                // Only received items
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

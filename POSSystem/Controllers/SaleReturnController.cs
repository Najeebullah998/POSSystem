using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;

namespace POSSystem.Controllers
{
    public class SaleReturnController : Controller
    {
        private readonly ISaleReturnRepository _repo;
        public SaleReturnController(ISaleReturnRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> EditBill(int id)
        {
            ViewBag.InvoiceId = id;
            ViewBag.IsEditMode = false;

            var model = new SaleReturnHeaderVM();

            model.ReturnNumber = await _repo.GenerateSaleReturnNumberAsync();
            model.ReturnDate = DateTime.Now;

            model.InvoiceList = (await _repo.GetInvoiceDropdownAsync()).ToList();

            model.WarehouseId = 2;
            model.CustomerId = 1;

            return View("CreateSaleReturn", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSaleReturns()
        {
            var result = await _repo.GetAllSaleReturnsAsync();
            return Json(result);
        }

        public async Task<IActionResult> CreateSaleReturn()
        {
            var model = new SaleReturnHeaderVM();
            model.ReturnNumber =await _repo.GenerateSaleReturnNumberAsync();
            model.ReturnDate = DateTime.Now;
            model.InvoiceList = (await _repo.GetInvoiceDropdownAsync()).ToList();
            model.WarehouseId = 2;
            model.CustomerId = 1;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceForSaleReturn(int invoiceId)
        {
            try
            {
                if (invoiceId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid Invoice Id."
                    });
                }

                var result = await _repo.GetInvoiceForSaleReturnAsync(invoiceId);

                if (result == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invoice not found."
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
        [HttpGet]
        public async Task<IActionResult> GetSaleReturnById(int saleReturnId)
        {
            try
            {
                var result = await _repo.GetSaleReturnByIdAsync(saleReturnId);

                if (result == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Sale Return not found."
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
        [HttpPost]
        public async Task<IActionResult> SaveSaleReturn(
     [FromBody] SaleReturnHeaderVM model)
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
                // Override Frontend Values
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
                        message = "Sale Return details are required."
                    });
                }


                // ==========================================
                // Only ReturnQty > 0
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
                // Save Sale Return
                // ==========================================

                var result =
                    await _repo.SaveSaleReturnAsync(model);


                if (result > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Sale Return Saved Successfully.",
                        id = result
                    });
                }


                return Json(new
                {
                    success = false,
                    message = "Unable to save Sale Return."
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

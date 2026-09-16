using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;

public class PurchaseOrderController : Controller
{
    private readonly IPurchaseOrderRepository _repo;

    public PurchaseOrderController(IPurchaseOrderRepository repo)
    {
        _repo = repo;
    }

    // =========================
    // INDEX (LIST PAGE)
    // =========================
    public IActionResult Index()
    {
        return View();
    }


    // =========================
    // GET PURCHASE ORDER LIST
    // =========================
    public async Task<IActionResult> GetPurchaseList()
    {
        int companyId =
            HttpContext.Session.GetInt32("CompanyId") ?? 0;

        int branchId =
            HttpContext.Session.GetInt32("BranchId") ?? 0;

        if (companyId <= 0 || branchId <= 0)
        {
            return Json(new
            {
                success = false,
                message = "Company or Branch not found in session."
            });
        }

        var result =
            await _repo.GetAllAsync(companyId, branchId);

        return Json(new
        {
            success = true,
            data = result
        });
    }


    // =========================
    // ADD / CREATE SCREEN
    // =========================
    public async Task<IActionResult> CreatePurchseOrder()
    {
        int companyId =
            HttpContext.Session.GetInt32("CompanyId") ?? 0;

        int branchId =
            HttpContext.Session.GetInt32("BranchId") ?? 0;

        if (companyId <= 0 || branchId <= 0)
        {
            return RedirectToAction("Index");
        }

        var model = new PurchaseOrderHeaderVm();

        model.CompanyId = companyId;
        model.BranchId = branchId;

        model.PONumber =
            await _repo.GeneratePONumberAsync(companyId, branchId);

        model.SupplierList =
            await _repo.GetSupplierDDAsync(companyId, branchId);

        model.WarehouseList =
            await _repo.GetWarehouseDDAsync(companyId, branchId);

        return View(model);
    }


    // =========================
    // SAVE PURCHASE ORDER
    // =========================
    [HttpPost]
    public async Task<IActionResult> Save(PurchaseOrderHeaderVm model)
    {
        try
        {
            if (model == null ||
                model.Details == null ||
                model.Details.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "No items found!"
                });
            }


            // ==========================================
            // Get Session IDs
            // ==========================================

            int companyId =
                HttpContext.Session.GetInt32("CompanyId") ?? 0;

            int branchId =
                HttpContext.Session.GetInt32("BranchId") ?? 0;

            int userId =
                HttpContext.Session.GetInt32("UserId") ?? 0;


            // ==========================================
            // Validate Session
            // ==========================================

            if (companyId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Company not found in session."
                });
            }

            if (branchId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Branch not found in session."
                });
            }

            if (userId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "User not found in session."
                });
            }


            // ==========================================
            // Override Frontend Values
            // ==========================================

            model.CompanyId = companyId;
            model.BranchId = branchId;
            model.CreatedBy = userId;


            // ==========================================
            // Save
            // ==========================================

            var result =
                await _repo.SaveAsync(model);


            if (result > 0)
            {
                return Json(new
                {
                    success = true,
                    message = "Purchase Order Saved Successfully",
                    id = result
                });
            }


            return Json(new
            {
                success = false,
                message = "Save failed!"
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


    // =========================
    // EDIT SCREEN
    // =========================
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            int companyId =
                HttpContext.Session.GetInt32("CompanyId") ?? 0;

            int branchId =
                HttpContext.Session.GetInt32("BranchId") ?? 0;

            if (companyId <= 0 || branchId <= 0)
            {
                return RedirectToAction("Index");
            }

            var model = await _repo.GetByIdAsync(
                id,
                companyId,
                branchId
            );

            if (model == null)
            {
                return NotFound();
            }

            // Dropdowns
            model.SupplierList =
                await _repo.GetSupplierDDAsync(
                    companyId,
                    branchId
                );

            model.WarehouseList =
                await _repo.GetWarehouseDDAsync(
                    companyId,
                    branchId
                );

            // Ensure Company/Branch remain session based
            model.CompanyId = companyId;
            model.BranchId = branchId;

            return View("CreatePurchseOrder", model);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // =========================
    // UPDATE PURCHASE ORDER
    // =========================
    [HttpPost]
    public async Task<IActionResult> Update(PurchaseOrderHeaderVm model)
    {
        try
        {
            if (model == null ||
                model.Details == null ||
                model.Details.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "No items found!"
                });
            }


            // ==========================================
            // Get Session IDs
            // ==========================================

            int companyId =
                HttpContext.Session.GetInt32("CompanyId") ?? 0;

            int branchId =
                HttpContext.Session.GetInt32("BranchId") ?? 0;

            int userId =
                HttpContext.Session.GetInt32("UserId") ?? 0;


            // ==========================================
            // Validate Session
            // ==========================================

            if (companyId <= 0 ||
                branchId <= 0 ||
                userId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid session. Please login again."
                });
            }


            // ==========================================
            // Override Frontend Values
            // ==========================================

            model.CompanyId = companyId;
            model.BranchId = branchId;
            model.ModifiedBy = userId;


            // ==========================================
            // Update
            // ==========================================

            var result =
                await _repo.UpdateAsync(model);


            if (result > 0)
            {
                return Json(new
                {
                    success = true,
                    message = "Purchase Order Updated Successfully",
                    id = result
                });
            }


            return Json(new
            {
                success = false,
                message = "Update failed!"
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


    // =========================
    // DELETE PURCHASE ORDER
    // =========================
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            int companyId =
                HttpContext.Session.GetInt32("CompanyId") ?? 0;

            int branchId =
                HttpContext.Session.GetInt32("BranchId") ?? 0;

            int userId =
                HttpContext.Session.GetInt32("UserId") ?? 0;


            if (companyId <= 0 ||
                branchId <= 0 ||
                userId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid session. Please login again."
                });
            }


            var result =
                await _repo.DeleteAsync(
                    id,
                    companyId,
                    branchId,
                    userId);


            return Json(new
            {
                success = result > 0,
                message = result > 0
                    ? "Purchase Order Deleted Successfully"
                    : "Delete failed!"
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


    // =========================
    // ITEM BY BARCODE
    // =========================
    public async Task<IActionResult> GetItemByBarcode(string barcode)
    {
        int companyId =
            HttpContext.Session.GetInt32("CompanyId") ?? 0;

        int branchId =
            HttpContext.Session.GetInt32("BranchId") ?? 0;


        if (companyId <= 0 || branchId <= 0)
        {
            return Json(new
            {
                success = false,
                message = "Company or Branch not found."
            });
        }


        var item =
            await _repo.GetItemByBarcodeAsync(
                barcode,
                companyId,
                branchId);


        return Json(item);
    }


    // =========================
    // ITEM SEARCH
    // =========================
    public async Task<IActionResult> SearchItem(string term)
    {
        int companyId =
            HttpContext.Session.GetInt32("CompanyId") ?? 0;

        int branchId =
            HttpContext.Session.GetInt32("BranchId") ?? 0;


        if (companyId <= 0 || branchId <= 0)
        {
            return Json(new
            {
                success = false,
                message = "Company or Branch not found."
            });
        }


        var items =
            await _repo.SearchItemAsync(
                term,
                companyId,
                branchId);


        return Json(items);
    }
}
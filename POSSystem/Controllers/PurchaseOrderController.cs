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
    public async Task<IActionResult> Index()
    {
        var list = await _repo.GetAllAsync();
        return View(list);
    }

    // =========================
    // ADD / CREATE SCREEN
    // =========================
    public async Task<IActionResult> CreatePurchseOrder()
    {
        var model = new PurchaseOrderHeaderVm();

        model.PONumber = await _repo.GeneratePONumberAsync();

        model.SupplierList = await _repo.GetSupplierDDAsync();
        model.WarehouseList = await _repo.GetWarehouseDDAsync();

        return View(model);
    }

    // =========================
    // SAVE PURCHASE ORDER (AJAX)
    // =========================
    [HttpPost]
    public async Task<IActionResult> Save(PurchaseOrderHeaderVm model)
    {
        if (model == null || model.Details == null || model.Details.Count == 0)
        {
            return Json(new { success = false, message = "No items found!" });
        }

        var result = await _repo.SaveAsync(model);

        if (result > 0)
        {
            return Json(new
            {
                success = true,
                message = "Purchase Order Saved Successfully",
                id = result
            });
        }

        return Json(new { success = false, message = "Save failed!" });
    }

    // =========================
    // EDIT SCREEN
    // =========================
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _repo.GetByIdAsync(id);

        //model.SupplierList = await _repo.GetSupplierDDAsync();
        //model.WarehouseList = await _repo.GetWarehouseDDAsync();

        return View("Create", model);
    }

    // =========================
    // DELETE
    // =========================
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _repo.DeleteAsync(id);

        return Json(new
        {
            success = result > 0,
            message = "Deleted Successfully"
        });
    }

    // =========================
    // ITEM SEARCH (AJAX)
    // =========================
    public async Task<IActionResult> GetItemByBarcode(string barcode)
    {
        var item = await _repo.GetItemByBarcodeAsync(barcode);
        return Json(item);
    }

    public async Task<IActionResult> SearchItem(string term)
    {
        var items = await _repo.SearchItemAsync(term);
        return Json(items);
    }
}
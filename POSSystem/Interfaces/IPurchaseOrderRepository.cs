using Microsoft.AspNetCore.Mvc.Rendering;
using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        // =========================
        // GET METHODS
        // =========================

        Task<List<PurchaseOrderHeaderVm>> GetAllAsync();

        Task<PurchaseOrderHeaderVm> GetByIdAsync(int id);

        Task<string> GeneratePONumberAsync();

        // =========================
        // SAVE / UPDATE
        // =========================

        Task<int> SaveAsync(PurchaseOrderHeaderVm model);

        Task<int> UpdateAsync(PurchaseOrderHeaderVm model);

        Task<int> DeleteAsync(int id);

        // =========================
        // DROPDOWNS
        // =========================

        Task<List<SelectListItem>> GetSupplierDDAsync();

        Task<List<SelectListItem>> GetWarehouseDDAsync();

        Task<List<SelectListItem>> GetItemDDAsync();

        // =========================
        // ITEM HELPERS (AJAX)
        // =========================

        Task<ItemInfoVm> GetItemByBarcodeAsync(string barcode);

        Task<List<ItemInfoVm>> SearchItemAsync(string term);
    }
}

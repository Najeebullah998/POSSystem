using Microsoft.AspNetCore.Mvc.Rendering;
using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        // =========================
        // GET METHODS
        // =========================

        Task<List<PurchaseOrderHeaderVm>> GetAllAsync(
            int companyId,
            int branchId);

        Task<PurchaseOrderHeaderVm> GetByIdAsync(
            int id,
            int companyId,
            int branchId);

        Task<string> GeneratePONumberAsync(
            int companyId,
            int branchId);


        // =========================
        // SAVE / UPDATE / DELETE
        // =========================

        Task<int> SaveAsync(
            PurchaseOrderHeaderVm model);

        Task<int> UpdateAsync(
            PurchaseOrderHeaderVm model);

        Task<int> DeleteAsync(
            int id,
            int companyId,
            int branchId,
            int deletedBy);


        // =========================
        // DROPDOWNS
        // =========================

        Task<List<SelectListItem>> GetSupplierDDAsync(int companyId,int branchId);

        Task<List<SelectListItem>> GetWarehouseDDAsync(int companyId,int branchId);

        Task<List<SelectListItem>> GetItemDDAsync(int companyId,int branchId);


        // =========================
        // ITEM HELPERS (AJAX)
        // =========================

        Task<ItemInfoVm> GetItemByBarcodeAsync(string barcode,int companyId,int branchId);

        Task<List<ItemInfoVm>> SearchItemAsync(string term,int companyId,int branchId);
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IPurchaseReturnRepository
    {
        Task<string> GenerateReturnNumberAsync();

        // Dropdown
        Task<List<SelectListItem>> GetGRNListAsync();
        Task<List<SelectListItem>> GetSupplierDDAsync();

        Task<List<SelectListItem>> GetWarehouseDDAsync();

        // Load GRN Header & Details
        Task<PurchaseReturnHeaderVM> GetGRNForPurchaseReturnAsync(int grnId);

        // Save Purchase Return
        Task<int> SavePurchaseReturnAsync(PurchaseReturnHeaderVM model);
    }
}

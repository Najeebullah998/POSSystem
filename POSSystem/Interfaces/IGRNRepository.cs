using Microsoft.AspNetCore.Mvc.Rendering;
using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IGRNRepository
    {
        Task<GRNHeaderVM> GetPurchaseOrderByIdAsync(int purchaseOrderId);
        Task<List<SelectListItem>> GetPurchaseOrderList();

        Task<List<SelectListItem>> GetSupplierDDAsync();

        Task<List<SelectListItem>> GetWarehouseDDAsync();
        Task<int> SaveGRNAsync(GRNHeaderVM model);
    }
}

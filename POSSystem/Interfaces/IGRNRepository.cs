using Microsoft.AspNetCore.Mvc.Rendering;
using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IGRNRepository
    {
        Task<string> GenerateGRNNumberAsync();
        Task<GRNHeaderVM> GetPurchaseOrderByIdAsync(int purchaseOrderId, int companyId, int branchId);
        Task<List<SelectListItem>> GetPurchaseOrderList();

        Task<List<SelectListItem>> GetSupplierDDAsync();

        Task<List<SelectListItem>> GetWarehouseDDAsync();
        Task<int> SaveGRNAsync(GRNHeaderVM model);
    }
}

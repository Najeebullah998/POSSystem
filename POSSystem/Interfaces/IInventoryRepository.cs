namespace POSSystem.Interfaces
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using POSSystem.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IInventoryRepository
    {
        Task<List<SelectListItem>> GetWarehouseDDAsync();
        Task<List<SelectListItem>> GetItemsDDAsync();
        Task<IEnumerable<StockViewModel>> GetStockAsync(
       int companyId,
       int branchId,
       int? warehouseId = null,
       int? itemId = null,
       string batchNo = null
   );
    }
}

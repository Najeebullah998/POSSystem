using Microsoft.AspNetCore.Mvc.Rendering;
using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface ISaleReturnRepository
    {
        Task<string> GenerateSaleReturnNumberAsync();
        Task<SaleReturnHeaderVM> GetInvoiceForSaleReturnAsync(int invoiceId);
        Task<IEnumerable<SelectListItem>> GetInvoiceDropdownAsync();
        // Save
        Task<int> SaveSaleReturnAsync(SaleReturnHeaderVM model);

        // List
        Task<IEnumerable<SaleReturnHeaderVM>> GetAllSaleReturnsAsync();

        // Get By Id
        Task<SaleReturnHeaderVM> GetSaleReturnByIdAsync(int saleReturnId);

        // Delete (Soft Delete)
        Task<bool> DeleteSaleReturnAsync(int saleReturnId);
    }
}

using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IPosRepository
    {
        Task<string> GenerateInvoiceNoAsync(int branchId);
        bool SaveInvoice(PosInvoiceVm model);
        IEnumerable<ItemSearchVM> SearchItems(string term);
        Task<(PosInvoiceVm Header, List<PosInvoiceDetailVm> Details)> GetBillByIdAsync(int companyId,int branchId,int invoiceId
    );
        Task<IEnumerable<PosInvoiceVm>> GetCompleteBillsAsync(int companyId, int branchId, DateTime? fromDate, DateTime? toDate, string invoiceNo);
        Task<bool> UpdatePosInvoiceAsync(
        int companyId,
        int branchId,
        int invoiceId,
        PosInvoiceVm model,
        List<PosInvoiceDetailVm> details,
        int modifiedBy
    );
        bool DeleteInvoiceDetail(int invoiceDetailId, int companyId, int branchId, int modifiedBy);
        Task<SaleClosingSummary> GetSaleClosingSummaryAsync(int companyId, int branchId, int userId, DateTime closingDate);
        Task<List<Customers>> GetDropdownAsync(int companyId, int branchId);
        Task<SaleClosingSaveResult> SaveAsync(int companyId,int branchId,int userId,DateTime closingDate,
        decimal openingCash,
        decimal totalSales,
        decimal totalReturns,
        decimal netSales,
        decimal cashSales,
        decimal easyPaisaSales,
        decimal otherSales,
        decimal actualCash,
        string remarks
    );
    }
}

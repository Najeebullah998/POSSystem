using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IPosRepository
    {
        bool SaveInvoice(PosInvoiceVm model);
        IEnumerable<ItemSearchVM> SearchItems(string term);
        Task<(PosInvoiceVm Invoice, List<PosInvoiceDetailVm> Details)> GetBillByIdAsync(int invoiceId);
        Task<IEnumerable<PosInvoiceVm>> GetCompleteBillsAsync(int branchId,DateTime? fromDate,DateTime? toDate,string invoiceNo);
        bool UpdateInvoice(PosInvoiceVm model);
        bool DeleteInvoice(int invoiceId, int modifiedBy);
    }
}

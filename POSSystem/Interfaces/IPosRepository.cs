using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IPosRepository
    {
        bool SaveInvoice(PosInvoiceVm model);
        IEnumerable<ItemSearchVM> SearchItems(string term);
    }
}

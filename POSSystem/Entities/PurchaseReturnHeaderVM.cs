using Microsoft.AspNetCore.Mvc.Rendering;

namespace POSSystem.Entities
{
    public class PurchaseReturnHeaderVM
    {
        public int PurchaseReturnId { get; set; }

        public string ReturnNumber { get; set; }

        public DateTime ReturnDate { get; set; }

        public int GRNId { get; set; }

        public int SupplierId { get; set; }

        public int BranchId { get; set; }

        public int WarehouseId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Remarks { get; set; }

        public int CreatedBy { get; set; }

        public List<PurchaseReturnDetailVM> Details { get; set; } = new List<PurchaseReturnDetailVM>();

        // Dropdowns

        public List<SelectListItem> GRNList { get; set; }

        public List<SelectListItem> SupplierList { get; set; }

        public List<SelectListItem> WarehouseList { get; set; }
    }
}

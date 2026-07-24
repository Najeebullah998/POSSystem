using Microsoft.AspNetCore.Mvc.Rendering;

namespace POSSystem.Entities
{
    public class GRNHeaderVM
    {
        public int GRNId { get; set; }

        public string GRNNumber { get; set; }

        public DateTime GRNDate { get; set; }

        public int PurchaseOrderId { get; set; }

        public int SupplierId { get; set; }

        public int BranchId { get; set; }

        public int WarehouseId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Remarks { get; set; }

        public int CreatedBy { get; set; }

        public List<GRNDetailVM> Details { get; set; } = new List<GRNDetailVM>();


        // Dropdowns

        public IEnumerable<SelectListItem> PurchaseOrderList { get; set; }

        public List<SelectListItem> SupplierList { get; set; }

        public List<SelectListItem> WarehouseList { get; set; }
    }
}

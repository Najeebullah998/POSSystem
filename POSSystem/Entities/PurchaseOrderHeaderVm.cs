using Microsoft.AspNetCore.Mvc.Rendering;

namespace POSSystem.Entities
{
    public class PurchaseOrderHeaderVm
    {
        public int PurchaseOrderId { get; set; }

        public string PONumber { get; set; }

        public DateTime PODate { get; set; }

        public int SupplierId { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }

        public int WarehouseId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Discount { get; set; }

        public decimal NetAmount { get; set; }

        public string Status { get; set; }
        public string supplierName { get; set; }

        public int CreatedBy { get; set; }

        public List<PurchaseOrderDetailVm> Details { get; set; }
        public List<SelectListItem> SupplierList { get; set; }
        public List<SelectListItem> WarehouseList { get; set; }

        public PurchaseOrderHeaderVm()
        {
            Details = new List<PurchaseOrderDetailVm>();
        }
    }
}

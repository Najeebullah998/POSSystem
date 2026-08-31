using Microsoft.AspNetCore.Mvc.Rendering;

namespace POSSystem.Entities
{
    public class SaleReturnHeaderVM
    {
        public int SaleReturnId { get; set; }

        public string ReturnNumber { get; set; }

        public DateTime ReturnDate { get; set; }

        public int InvoiceId { get; set; }

        public int CustomerId { get; set; }

        public int BranchId { get; set; }
        public int CompanyId { get; set; }

        public int WarehouseId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Remarks { get; set; }

        public int CreatedBy { get; set; }
        public string CustomerName { get; set; }

        public string WarehouseName { get; set; }

        public List<SaleReturnDetailVM> Details { get; set; } = new();

        // Dropdowns

        public List<SelectListItem> InvoiceList { get; set; }

        public List<SelectListItem> CustomerList { get; set; }

        public List<SelectListItem> WarehouseList { get; set; }
    }
}

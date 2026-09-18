using Microsoft.AspNetCore.Mvc.Rendering;

namespace POSSystem.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public string? Barcode { get; set; }
        public string? ItemName { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? UnitId { get; set; }
        public string? UnitName { get; set; } = string.Empty;
        public decimal? SaleQuantity { get; set; }
        public decimal? SupplierDiscount { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? CostPrice { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public List<SelectListItem> UnitList { get; set; }
    }
}

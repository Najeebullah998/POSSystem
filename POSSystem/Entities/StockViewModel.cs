namespace POSSystem.Entities
{
    public class StockViewModel
    {
        public int StockId { get; set; }

        public int CompanyId { get; set; }

        public int BranchId { get; set; }

        public int WarehouseId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public string BatchNo { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public decimal Quantity { get; set; }
    }
}

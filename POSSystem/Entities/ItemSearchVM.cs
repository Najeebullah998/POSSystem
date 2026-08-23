namespace POSSystem.Entities
{
    public class ItemSearchVM
    {
        public int ItemId { get; set; }

        public string Barcode { get; set; }

        public string ItemName { get; set; }

        public decimal SalePrice { get; set; }

        public decimal StockQty { get; set; }
    }
}

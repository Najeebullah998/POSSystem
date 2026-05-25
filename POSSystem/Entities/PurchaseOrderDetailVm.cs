namespace POSSystem.Entities
{
    public class PurchaseOrderDetailVm
    {
        public int PODetailId { get; set; }

        public int ItemId { get; set; }

        public string Barcode { get; set; }

        public string ItemName { get; set; }

        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }

        public decimal Amount { get; set; }
    }
}

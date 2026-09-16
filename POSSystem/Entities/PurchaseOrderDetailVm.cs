namespace POSSystem.Entities
{
    public class PurchaseOrderDetailVm
    {
        public int PODetailId { get; set; }

        public int CompanyId { get; set; }

        public int BranchId { get; set; }

        public int ItemId { get; set; }

        public string Barcode { get; set; }

        public string ItemName { get; set; }

        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }

        public decimal Amount { get; set; }

        public decimal ReceivedQty { get; set; }

        public decimal PendingQty { get; set; }

        public decimal CurrentStock { get; set; }
    }
}

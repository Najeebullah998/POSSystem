namespace POSSystem.Entities
{
    public class ItemInfoVm
    {
        public int ItemId { get; set; }

        public string? Barcode { get; set; }

        public string? ItemName { get; set; }

        public decimal CurrentStock { get; set; }

        public decimal LastPurchaseRate { get; set; }
    }
}

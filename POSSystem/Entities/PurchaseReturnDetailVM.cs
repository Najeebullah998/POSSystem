namespace POSSystem.Entities
{
    public class PurchaseReturnDetailVM
    {
        public int PurchaseReturnDetailId { get; set; }
        public int GRNDetailId { get; set; } // ✅ Added this property
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal ReceivedQty { get; set; } // Original quantity from GRN
        public decimal ReturnedQty { get; set; } // Already returned quantity
        public decimal AvailableQty { get; set; } // Available to return
        public decimal ReturnQty { get; set; } // Current return quantity
        public decimal Rate { get; set; }
        public string Reason { get; set; } // Return reason
        public decimal Amount
        {
            get
            {
                return ReturnQty * Rate;
            }
        }
    }
}

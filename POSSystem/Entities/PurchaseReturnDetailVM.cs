namespace POSSystem.Entities
{
    public class PurchaseReturnDetailVM
    {
        public int PurchaseReturnDetailId { get; set; }

        public int GRNDetailId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public decimal ReceivedQty { get; set; }

        public decimal ReturnedQty { get; set; }

        public decimal AvailableQty { get; set; }

        public decimal ReturnQty { get; set; }

        public decimal Rate { get; set; }

        public decimal Amount
        {
            get
            {
                return ReturnQty * Rate;
            }
        }
    }
}

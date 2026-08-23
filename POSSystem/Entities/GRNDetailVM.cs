namespace POSSystem.Entities
{
    public class GRNDetailVM
    {
        public int GRNDetailId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public decimal OrderQty { get; set; }

        public decimal ReceivedQty { get; set; }

        public decimal PendingQty { get; set; }

        public decimal Rate { get; set; }

        public decimal Amount
        {
            get
            {
                return ReceivedQty * Rate;
            }
        }

        public string BatchNo { get; set; }

        public DateTime? ManufacturingDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}

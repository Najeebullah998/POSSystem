namespace POSSystem.Entities
{
    public class SaleReturnDetailVM
    {
        public int SaleReturnDetailId { get; set; }

        public int SaleInvoiceDetailId { get; set; }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        // Sold Qty
        public decimal Quantity { get; set; }

        // Already Returned Qty
        public decimal ReturnedQty { get; set; }

        // Remaining Qty
        public decimal AvailableQty { get; set; }

        // User Enter Qty
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

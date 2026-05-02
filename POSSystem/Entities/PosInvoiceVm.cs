namespace POSSystem.Entities
{
    public class PosInvoiceVm
    {
        public string? InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int CustomerId { get; set; }
        public int BranchId { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }

        public List<PosInvoiceDetailVm>? Items { get; set; }
    }

    public class PosInvoiceDetailVm
    {
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}

namespace POSSystem.Entities
{
    public class PosInvoiceVm
    {
        public int InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int CustomerId { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentMode { get; set; }
        public decimal PaidAmount { get; set; }

        public string CustomerName { get; set; }
        public string BranchName { get; set; }
        public List<PosInvoiceDetailVm>? Items { get; set; }
    }

    public class PosInvoiceDetailVm
    {
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string ItemName { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}

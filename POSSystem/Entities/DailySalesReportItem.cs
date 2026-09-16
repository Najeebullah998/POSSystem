namespace POSSystem.Entities
{
    public class DailySalesReportItem
    {
        public int InvoiceId { get; set; }
        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public int? CustomerId { get; set; }
        public int? UserId { get; set; }

        public string PaymentMode { get; set; }

        public decimal? PaidAmount { get; set; }

        public decimal? GrossAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetAmount { get; set; }

        public decimal? ReturnAmount { get; set; }
        public decimal? NetAfterReturn { get; set; }
    }
}

namespace POSSystem.Entities
{
    public class SaleClosingSummary
    {
        public int TotalBills { get; set; }
        public decimal GrossSales { get; set; }
        public decimal Discount { get; set; }
        public decimal NetSales { get; set; }
        public decimal TotalReturns { get; set; }
        public decimal NetAfterReturns { get; set; }

        public decimal Cash { get; set; }
        public decimal EasyPaisa { get; set; }
        public decimal Other { get; set; }

        public decimal TotalPayments { get; set; }
        public decimal ExpectedCash { get; set; }
    }
}

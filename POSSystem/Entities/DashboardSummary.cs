namespace POSSystem.Entities
{
    public class DashboardSummary
    {
        public int TotalBills { get; set; }

        public decimal GrossSales { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal NetSales { get; set; }

        public decimal TotalReturns { get; set; }

        public decimal NetAfterReturns { get; set; }

        public decimal CashSales { get; set; }

        public decimal EasyPaisaSales { get; set; }

        public decimal OtherSales { get; set; }

        public decimal TotalPayments { get; set; }
    }
}

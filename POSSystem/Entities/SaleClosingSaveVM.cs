namespace POSSystem.Entities
{
    public class SaleClosingSaveVM
    {
        public DateTime ClosingDate { get; set; }

        public decimal OpeningCash { get; set; }

        public decimal TotalSales { get; set; }

        public decimal TotalReturns { get; set; }

        public decimal NetSales { get; set; }

        public decimal CashSales { get; set; }

        public decimal EasyPaisaSales { get; set; }

        public decimal OtherSales { get; set; }

        public decimal ActualCash { get; set; }

        public string Remarks { get; set; }
    }
}

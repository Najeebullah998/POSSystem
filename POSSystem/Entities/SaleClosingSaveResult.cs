namespace POSSystem.Entities
{
    public class SaleClosingSaveResult
    {
        public int ClosingId { get; set; }
        public string ClosingNo { get; set; }
        public decimal ExpectedCash { get; set; }
        public decimal ActualCash { get; set; }
        public decimal CashDifference { get; set; }
        public string Message { get; set; }
    }
}

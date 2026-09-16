using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IReportsRepo
    {
        Task<(DailySalesSummary Summary, List<DailySalesReportItem> Details)>  GetDailySalesReportAsync(int companyId,int branchId,DateTime fromDate,DateTime toDate);
    }
}

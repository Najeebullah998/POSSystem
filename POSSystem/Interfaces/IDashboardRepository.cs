using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummary> GetDashboardSummaryAsync(
        int companyId,
        int branchId,
        int userId);
    }
}

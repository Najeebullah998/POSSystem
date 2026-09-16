using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using System.Data;

namespace POSSystem.Repositories
{
    public class DashboardRepository: IDashboardRepository
    {
        private readonly DapperContext _context;

        public DashboardRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<DashboardSummary> GetDashboardSummaryAsync(int companyId,int branchId,int userId)
        {
            using (var con = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyId", companyId);
                parameters.Add("@BranchId", branchId);
                parameters.Add("@UserId", userId);

                var result = await con.QueryFirstOrDefaultAsync<DashboardSummary>(
                    "sp_GetDashboardSummary",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result ?? new DashboardSummary();
            }
        }
    }
}

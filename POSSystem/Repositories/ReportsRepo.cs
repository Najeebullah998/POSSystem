using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using System.Data;

namespace POSSystem.Repositories
{
    public class ReportsRepo:IReportsRepo
    {
        private readonly DapperContext _context;
        public ReportsRepo(DapperContext context)
        {
            _context = context;
        }
        public async Task<(DailySalesSummary Summary, List<DailySalesReportItem> Details)> GetDailySalesReportAsync(int companyId,int branchId,
        DateTime fromDate,
        DateTime toDate)
        {
            using (var con = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyId", companyId);
                parameters.Add("@BranchId", branchId);
                parameters.Add("@FromDate", fromDate.Date);
                parameters.Add("@ToDate", toDate.Date);

                using (var multi = await con.QueryMultipleAsync(
                    "sp_GetDailySalesReport",
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    var summary = await multi.ReadFirstOrDefaultAsync<DailySalesSummary>();

                    var details = (await multi.ReadAsync<DailySalesReportItem>())
                        .ToList();

                    return (summary, details);
                }
            }
        }
    }
}

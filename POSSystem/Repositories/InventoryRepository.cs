using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace POSSystem.Repositories
{
    public class InventoryRepository: IInventoryRepository
    {
        private readonly DapperContext _context;
        public InventoryRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<List<SelectListItem>> GetItemsDDAsync()
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<SelectListItem>(
                    "SELECT ItemId AS Value, ItemName AS Text FROM Items WHERE IsDeleted = 0"
                );
                return result.ToList();
            }
        }
        public async Task<List<SelectListItem>> GetWarehouseDDAsync()
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<SelectListItem>(
                    "SELECT WarehouseId AS Value, WarehouseName AS Text FROM Warehouses WHERE IsDeleted = 0"
                );

                return result.ToList();
            }
        }
        public async Task<IEnumerable<StockViewModel>> GetStockAsync(int companyId,int branchId,int? warehouseId = null,int? itemId = null,string batchNo = null)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyId", companyId);
                parameters.Add("@BranchId", branchId);
                parameters.Add("@WarehouseId", warehouseId);
                parameters.Add("@ItemId", itemId);
                parameters.Add("@BatchNo", batchNo);

                return await connection.QueryAsync<StockViewModel>(
                    "sp_GetStock",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }
    }
}

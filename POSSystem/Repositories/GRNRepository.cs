using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using System.Data;

namespace POSSystem.Repositories
{
    using Dapper;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Data;

    public class GRNRepository : IGRNRepository
    {
        private readonly DapperContext _context;

        public GRNRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateGRNNumberAsync()
        {
            using (var con = _context.CreateConnection())
            {
                var count = await con.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM GRNHeader"
                );

                return $"GRN-{DateTime.Now:yyyyMMdd}-{count + 1}";
            }
        }

        public async Task<int> SaveGRNAsync(GRNHeaderVM model)
        {
            using (var con = _context.CreateConnection())
            {
                DynamicParameters param = new DynamicParameters();

                //===========================
                // Header Parameters
                //===========================

                param.Add("@GRNNumber", model.GRNNumber);
                param.Add("@GRNDate", model.GRNDate);
                param.Add("@PurchaseOrderId", model.PurchaseOrderId);
                param.Add("@SupplierId", model.SupplierId);
                param.Add("@BranchId", model.BranchId);
                param.Add("@WarehouseId", model.WarehouseId);
                param.Add("@TotalAmount", model.TotalAmount);
                param.Add("@Remarks", model.Remarks);
                param.Add("@CreatedBy", model.CreatedBy);

                //===========================
                // Detail TVP
                //===========================

                param.Add("@Details",
                    GetGRNDetailTable(model.Details)
                    .AsTableValuedParameter("GRNDetailType"));

                var grnId = await con.QuerySingleAsync<int>(
                    "sp_SaveGRN",
                    param,
                    commandType: CommandType.StoredProcedure);

                return grnId;
            }
        }

        private DataTable GetGRNDetailTable(List<GRNDetailVM> details)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ItemId", typeof(int));
            dt.Columns.Add("ReceivedQty", typeof(decimal));
            dt.Columns.Add("Rate", typeof(decimal));
            dt.Columns.Add("Amount", typeof(decimal));

            foreach (var item in details)
            {
                dt.Rows.Add(
                    item.ItemId,
                    item.ReceivedQty,
                    item.Rate,
                    item.Amount
                );
            }

            return dt;
        }

        public async Task<GRNHeaderVM> GetPurchaseOrderByIdAsync(int purchaseOrderId)
        {
            using (var con = _context.CreateConnection())
            {
                using (var multi = await con.QueryMultipleAsync(
                    "sp_GetPurchaseOrderForGRN",
                    new
                    {
                        PurchaseOrderId = purchaseOrderId
                    },
                    commandType: CommandType.StoredProcedure))
                {
                    var header = await multi.ReadFirstOrDefaultAsync<GRNHeaderVM>();

                    if (header != null)
                    {
                        header.Details = (await multi.ReadAsync<GRNDetailVM>()).ToList();
                    }

                    return header;
                }
            }
        }
        public async Task<List<SelectListItem>> GetPurchaseOrderList()
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<SelectListItem>(
                    "SELECT PurchaseOrderId AS Value, PONumber AS Text FROM PurchaseOrderHeader WHERE IsDeleted = 0"
                );

                return result.ToList();
            }
        }

        public async Task<List<SelectListItem>> GetSupplierDDAsync()
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<SelectListItem>(
                    "SELECT SupplierId AS Value, SupplierName AS Text FROM Suppliers WHERE IsDeleted = 0"
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
    }
}

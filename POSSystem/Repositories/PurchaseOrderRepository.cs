using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using System.Data;

namespace POSSystem.Repositories
{
    public class PurchaseOrderRepository: IPurchaseOrderRepository
    {
        private readonly DapperContext _context;
        public PurchaseOrderRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> SaveAsync(PurchaseOrderHeaderVm model)
        {
            using (var con = _context.CreateConnection())
            {
                // REMOVE THIS LINE ❌
                // await con.OpenAsync();

                DataTable dt = new DataTable();
                dt.Columns.Add("ItemId", typeof(int));
                dt.Columns.Add("Quantity", typeof(decimal));
                dt.Columns.Add("Rate", typeof(decimal));
                dt.Columns.Add("Amount", typeof(decimal));

                foreach (var item in model.Details)
                {
                    dt.Rows.Add(item.ItemId, item.Quantity, item.Rate, item.Amount);
                }

                var param = new DynamicParameters();
                param.Add("@PONumber", model.PONumber);
                param.Add("@PODate", model.PODate);
                param.Add("@SupplierId", model.SupplierId);
                param.Add("@BranchId", model.BranchId);
                param.Add("@WarehouseId", model.WarehouseId);
                param.Add("@TotalAmount", model.TotalAmount);
                param.Add("@Discount", model.Discount);
                param.Add("@NetAmount", model.NetAmount);
                param.Add("@Remarks", model.Remarks);
                param.Add("@CreatedBy", model.CreatedBy);

                param.Add("@Details", dt.AsTableValuedParameter("PurchaseOrderDetailType"));

                var result = await con.QueryFirstOrDefaultAsync<int>(
                    "sp_SavePurchaseOrder",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<PurchaseOrderHeaderVm>> GetAllAsync()
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<PurchaseOrderHeaderVm>(
                    "SELECT * FROM PurchaseOrderHeader WHERE IsDeleted = 0"
                );

                return result.ToList();
            }
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<PurchaseOrderHeaderVm> GetByIdAsync(int id)
        {
            using (var con = _context.CreateConnection())
            {
                var query = @"
                SELECT * FROM PurchaseOrderHeader WHERE PurchaseOrderId = @Id;

                SELECT * FROM PurchaseOrderDetail WHERE PurchaseOrderId = @Id;
            ";

                using (var multi = await con.QueryMultipleAsync(query, new { Id = id }))
                {
                    var header = await multi.ReadFirstOrDefaultAsync<PurchaseOrderHeaderVm>();
                    var details = await multi.ReadAsync<PurchaseOrderDetailVm>();

                    if (header != null)
                    {
                        header.Details = details.ToList();
                    }

                    return header;
                }
            }
        }

        // =========================
        // DELETE (SOFT DELETE)
        // =========================
        public async Task<int> DeleteAsync(int id)
        {
            using (var con = _context.CreateConnection())
            {
                var query = @"
                UPDATE PurchaseOrderHeader
                SET IsDeleted = 1
                WHERE PurchaseOrderId = @Id
            ";

                return await con.ExecuteAsync(query, new { Id = id });
            }
        }

        // =========================
        // PONUMBER GENERATE (SIMPLE)
        // =========================
        public async Task<string> GeneratePONumberAsync()
        {
            using (var con = _context.CreateConnection())
            {
                var count = await con.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM PurchaseOrderHeader"
                );

                return $"PO-{DateTime.Now:yyyyMMdd}-{count + 1}";
            }
        }

        // =========================
        // DROPDOWNS
        // =========================
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

        public async Task<List<SelectListItem>> GetItemDDAsync()
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<SelectListItem>(
                    "SELECT ItemId AS Value, ItemName AS Text FROM Items WHERE IsDeleted = 0"
                );

                return result.ToList();
            }
        }

        // =========================
        // ITEM SEARCH (BASIC)
        // =========================
        public async Task<ItemInfoVm> GetItemByBarcodeAsync(string barcode)
        {
            using (var con = _context.CreateConnection())
            {
                var query = @"
                SELECT TOP 1 
                    i.ItemId,
                    Barcode,
                    ItemName,
                    s.Quantity AS CurrentStock,
                    CostPrice AS LastPurchaseRate
                FROM Items i
                inner join Stock s on i.ItemId = s.ItemId
                WHERE Barcode = @Barcode
            ";

                return await con.QueryFirstOrDefaultAsync<ItemInfoVm>(query,new { Barcode = barcode });
            }
        }

        public async Task<List<ItemInfoVm>> SearchItemAsync(string term)
        {
            using (var con = _context.CreateConnection())
            {
                var query = @"
                SELECT TOP 20 
                    ItemId,
                    Barcode,
                    ItemName,
                    0 AS CurrentStock,
                    CostPrice AS LastPurchaseRate
                FROM Items
                WHERE ItemName LIKE '%' + @Term + '%'
            ";

                var result = await con.QueryAsync<ItemInfoVm>(query, new { Term = term });

                return result.ToList();
            }
        }

        // =========================
        // UPDATE (OPTIONAL)
        // =========================
        public Task<int> UpdateAsync(PurchaseOrderHeaderVm model)
        {
            throw new NotImplementedException();
        }
    }
}

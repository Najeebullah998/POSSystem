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
                // ==========================================
                // Purchase Order Detail TVP
                // ==========================================

                DataTable dt = new DataTable();

                dt.Columns.Add("ItemId", typeof(int));
                dt.Columns.Add("Quantity", typeof(decimal));
                dt.Columns.Add("Rate", typeof(decimal));
                dt.Columns.Add("Amount", typeof(decimal));


                foreach (var item in model.Details)
                {
                    dt.Rows.Add(
                        item.ItemId,
                        item.Quantity,
                        item.Rate,
                        item.Amount
                    );
                }


                // ==========================================
                // Stored Procedure Parameters
                // ==========================================

                var param = new DynamicParameters();

                param.Add("@PONumber", model.PONumber);
                param.Add("@PODate", model.PODate);
                param.Add("@SupplierId", model.SupplierId);

                // Company + Branch
                param.Add("@CompanyId", model.CompanyId);
                param.Add("@BranchId", model.BranchId);

                param.Add("@WarehouseId", model.WarehouseId);

                param.Add("@TotalAmount", model.TotalAmount);
                param.Add("@Discount", model.Discount);
                param.Add("@NetAmount", model.NetAmount);

                param.Add("@CreatedBy", model.CreatedBy);


                // ==========================================
                // Detail TVP
                // ==========================================

                param.Add(
                    "@Details",
                    dt.AsTableValuedParameter("dbo.PurchaseOrderDetailType")
                );


                // ==========================================
                // Execute Stored Procedure
                // ==========================================

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
        public async Task<List<PurchaseOrderHeaderVm>> GetAllAsync(
            int companyId,
            int branchId)
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<PurchaseOrderHeaderVm>(
                    "sp_GetPurchaseOrders",
                    new
                    {
                        CompanyId = companyId,
                        BranchId = branchId
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result.ToList();
            }
        }


        // =========================
        // GET BY ID
        // =========================
        public async Task<PurchaseOrderHeaderVm> GetByIdAsync(
            int id,
            int companyId,
            int branchId)
        {
            using (var con = _context.CreateConnection())
            {
                using (var multi = await con.QueryMultipleAsync(
                    "sp_GetPurchaseOrderById",
                    new
                    {
                        PurchaseOrderId = id,
                        CompanyId = companyId,
                        BranchId = branchId
                    },
                    commandType: CommandType.StoredProcedure))
                {
                    var header =
                        await multi.ReadFirstOrDefaultAsync<PurchaseOrderHeaderVm>();

                    var details =
                        await multi.ReadAsync<PurchaseOrderDetailVm>();

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
        public async Task<int> DeleteAsync(
            int id,
            int companyId,
            int branchId,
            int deletedBy)
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QuerySingleAsync<dynamic>(
                    "sp_DeletePurchaseOrder",
                    new
                    {
                        PurchaseOrderId = id,
                        CompanyId = companyId,
                        BranchId = branchId,
                        DeletedBy = deletedBy
                    },
                    commandType: CommandType.StoredProcedure
                );

                return result.Success ? 1 : 0;
            }
        }


        // =========================
        // PO NUMBER GENERATE
        // =========================
        public async Task<string> GeneratePONumberAsync(
            int companyId,
            int branchId)
        {
            using (var con = _context.CreateConnection())
            {
                var count = await con.ExecuteScalarAsync<int>(
                    @"
            SELECT COUNT(*)
            FROM PurchaseOrderHeader
            WHERE CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND ISNULL(IsDeleted, 0) = 0
            ",
                    new
                    {
                        CompanyId = companyId,
                        BranchId = branchId
                    }
                );

                return $"PO-{DateTime.Now:yyyyMMdd}-{count + 1}";
            }
        }


        // =========================
        // SUPPLIER DROPDOWN
        // =========================
        public async Task<List<SelectListItem>> GetSupplierDDAsync(
            int companyId,
            int branchId)
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<SelectListItem>(
                    @"
            SELECT
                SupplierId AS Value,
                SupplierName AS Text
            FROM Suppliers
            WHERE CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND ISNULL(IsDeleted, 0) = 0
            ORDER BY SupplierName
            ",
                    new
                    {
                        CompanyId = companyId,
                        BranchId = branchId
                    }
                );

                return result.ToList();
            }
        }


        // =========================
        // WAREHOUSE DROPDOWN
        // =========================
        public async Task<List<SelectListItem>> GetWarehouseDDAsync(
            int companyId,
            int branchId)
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<SelectListItem>(
                    @"
            SELECT
                WarehouseId AS Value,
                WarehouseName AS Text
            FROM Warehouses
            WHERE CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND ISNULL(IsDeleted, 0) = 0
            ORDER BY WarehouseName
            ",
                    new
                    {
                        CompanyId = companyId,
                        BranchId = branchId
                    }
                );

                return result.ToList();
            }
        }


        // =========================
        // ITEM DROPDOWN
        // =========================
        public async Task<List<SelectListItem>> GetItemDDAsync(
            int companyId,
            int branchId)
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<SelectListItem>(
                    @"
            SELECT
                ItemId AS Value,
                ItemName AS Text
            FROM Items
            WHERE CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND ISNULL(IsDeleted, 0) = 0
            ORDER BY ItemName
            ",
                    new
                    {
                        CompanyId = companyId,
                        BranchId = branchId
                    }
                );

                return result.ToList();
            }
        }


        // =========================
        // ITEM BY BARCODE
        // =========================
        public async Task<ItemInfoVm> GetItemByBarcodeAsync(
            string barcode,
            int companyId,
            int branchId)
        {
            using (var con = _context.CreateConnection())
            {
                var query = @"
            SELECT TOP 1
                i.ItemId,
                i.Barcode,
                i.ItemName,

                ISNULL(s.Quantity, 0) AS CurrentStock,

                i.CostPrice AS LastPurchaseRate

            FROM Items i

            LEFT JOIN Stock s
                ON s.ItemId = i.ItemId
                AND s.CompanyId = i.CompanyId
                AND s.BranchId = i.BranchId

            WHERE i.Barcode = @Barcode
              AND i.CompanyId = @CompanyId
              AND i.BranchId = @BranchId
              AND ISNULL(i.IsDeleted, 0) = 0;
        ";

                return await con.QueryFirstOrDefaultAsync<ItemInfoVm>(
                    query,
                    new
                    {
                        Barcode = barcode,
                        CompanyId = companyId,
                        BranchId = branchId
                    }
                );
            }
        }


        // =========================
        // ITEM SEARCH
        // =========================
        public async Task<List<ItemInfoVm>> SearchItemAsync(
            string term,
            int companyId,
            int branchId)
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

              AND CompanyId = @CompanyId
              AND BranchId = @BranchId

              AND ISNULL(IsDeleted, 0) = 0

            ORDER BY ItemName;
        ";

                var result = await con.QueryAsync<ItemInfoVm>(
                    query,
                    new
                    {
                        Term = term,
                        CompanyId = companyId,
                        BranchId = branchId
                    }
                );

                return result.ToList();
            }
        }


        // =========================
        // UPDATE
        // =========================
        public async Task<int> UpdateAsync(
            PurchaseOrderHeaderVm model)
        {
            using (var con = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add(
                    "PurchaseOrderId",
                    model.PurchaseOrderId);

                parameters.Add(
                    "PONumber",
                    model.PONumber);

                parameters.Add(
                    "PODate",
                    model.PODate);

                parameters.Add(
                    "SupplierId",
                    model.SupplierId);

                parameters.Add(
                    "CompanyId",
                    model.CompanyId);

                parameters.Add(
                    "BranchId",
                    model.BranchId);

                parameters.Add(
                    "WarehouseId",
                    model.WarehouseId);

                parameters.Add(
                    "TotalAmount",
                    model.TotalAmount);

                parameters.Add(
                    "Discount",
                    model.Discount);

                parameters.Add(
                    "NetAmount",
                    model.NetAmount);

                parameters.Add(
                    "UpdatedBy",
                    model.ModifiedBy);


                // =========================================
                // Details TVP
                // =========================================

                var detailTable = new DataTable();

                detailTable.Columns.Add(
                    "ItemId",
                    typeof(int));

                detailTable.Columns.Add(
                    "Quantity",
                    typeof(decimal));

                detailTable.Columns.Add(
                    "Rate",
                    typeof(decimal));

                detailTable.Columns.Add(
                    "Amount",
                    typeof(decimal));


                foreach (var item in model.Details)
                {
                    detailTable.Rows.Add(
                        item.ItemId,
                        item.Quantity,
                        item.Rate,
                        item.Amount
                    );
                }


                parameters.Add(
                    "Details",
                    detailTable.AsTableValuedParameter(
                        "dbo.PurchaseOrderDetailType"));


                var result = await con.ExecuteScalarAsync<int>(
                    "sp_UpdatePurchaseOrder",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
        }
    }
}

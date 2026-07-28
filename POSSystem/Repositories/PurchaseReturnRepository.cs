using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using System.Data;

namespace POSSystem.Repositories
{
    public class PurchaseReturnRepository : IPurchaseReturnRepository
    {
        private readonly DapperContext _context;
        public PurchaseReturnRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateReturnNumberAsync()
        {
            const string query = "SELECT COUNT(*) FROM PurchaseReturnHeader";

            using var con = _context.CreateConnection();

            var count = await con.ExecuteScalarAsync<int>(query);

            return $"PR-{DateTime.Now:yyyyMMdd}-{count + 1}";
        }

        public async Task<PurchaseReturnHeaderVM> GetGRNForPurchaseReturnAsync(int grnId)
        {
            using var con = _context.CreateConnection();

            var model = new PurchaseReturnHeaderVM();

            //=========================
            // Header
            //=========================

            const string headerQuery = @"
        SELECT
            GRNId,
            SupplierId,
            WarehouseId,
            BranchId
        FROM GRNHeader
        WHERE GRNId = @GRNId
          AND IsDeleted = 0";

            model = await con.QueryFirstOrDefaultAsync<PurchaseReturnHeaderVM>(
                headerQuery,
                new { GRNId = grnId });

            if (model == null)
                return null;

            //=========================
            // Detail
            //=========================

            const string detailQuery = @"
        SELECT
            GD.GRNDetailId,
            GD.ItemId,
            I.ItemName,
            GD.ReceivedQty,
            ISNULL(GD.ReturnQty,0) AS ReturnedQty,
            (GD.ReceivedQty - ISNULL(GD.ReturnQty,0)) AS AvailableQty,
            GD.Rate,
            GD.Amount
        FROM GRNDetail GD
        INNER JOIN Items I
            ON GD.ItemId = I.ItemId
        WHERE GD.GRNId = @GRNId
          AND (GD.ReceivedQty - ISNULL(GD.ReturnQty,0)) > 0
        ORDER BY I.ItemName;";

            model.Details = (await con.QueryAsync<PurchaseReturnDetailVM>(
                detailQuery,
                new { GRNId = grnId }))
                .ToList();

            return model;
        }

        public async Task<List<SelectListItem>> GetGRNListAsync()
        {
            const string query = @"
        SELECT DISTINCT
            GH.GRNId AS Value,
            GH.GRNNumber AS Text
        FROM GRNHeader GH
        INNER JOIN GRNDetail GD
            ON GH.GRNId = GD.GRNId
        WHERE
            GH.IsDeleted = 0
            AND (GD.ReceivedQty - ISNULL(GD.ReturnQty,0)) > 0
        ORDER BY GH.GRNId DESC;";

            using var con = _context.CreateConnection();

            var data = await con.QueryAsync<SelectListItem>(query);

            return data.ToList();
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
        public async Task<int> SavePurchaseReturnAsync(PurchaseReturnHeaderVM model)
        {
            using var con = _context.CreateConnection();

            DynamicParameters param = new DynamicParameters();

            //===========================
            // Header Parameters
            //===========================

            param.Add("@ReturnNumber", model.ReturnNumber);
            param.Add("@ReturnDate", model.ReturnDate);
            param.Add("@GRNId", model.GRNId);
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
                GetPurchaseReturnDetailTable(model.Details)
                .AsTableValuedParameter("PurchaseReturnDetailType"));

            var purchaseReturnId = await con.QuerySingleAsync<int>(
                "sp_SavePurchaseReturn",
                param,
                commandType: CommandType.StoredProcedure);

            return purchaseReturnId;
        }

        private DataTable GetPurchaseReturnDetailTable(List<PurchaseReturnDetailVM> details)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("GRNDetailId", typeof(int));
            dt.Columns.Add("ItemId", typeof(int));
            dt.Columns.Add("ReturnQty", typeof(decimal));
            dt.Columns.Add("Rate", typeof(decimal));
            dt.Columns.Add("Amount", typeof(decimal));

            foreach (var item in details)
            {
                dt.Rows.Add(
                    item.GRNDetailId,
                    item.ItemId,
                    item.ReturnQty,
                    item.Rate,
                    item.Amount
                );
            }

            return dt;
        }
    }
}

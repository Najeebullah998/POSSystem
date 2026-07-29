using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using System.Data;

namespace POSSystem.Repositories
{
    public class SaleReturnRepository : ISaleReturnRepository
    {
        private readonly DapperContext _context;
        public SaleReturnRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<SelectListItem>> GetInvoiceDropdownAsync()
        {
            using var con = _context.CreateConnection();

            const string query = @"
             SELECT
                 InvoiceId AS Value,
                 InvoiceNo AS Text
             FROM PosInvoice
             WHERE IsDeleted = 0
             ORDER BY InvoiceId DESC;";

            return await con.QueryAsync<SelectListItem>(query);
        }
        public async Task<string> GenerateSaleReturnNumberAsync()
        {
            using var con = _context.CreateConnection();

            const string query = @"
                 DECLARE @Today NVARCHAR(8) = CONVERT(VARCHAR(8), GETDATE(), 112);

                 DECLARE @NextNo INT =
                 (
                     SELECT ISNULL(MAX(
                         CAST(RIGHT(ReturnNumber,
                         LEN(ReturnNumber) - LEN('SR-' + @Today + '-')) AS INT)
                     ),0) + 1
                     FROM SaleReturnHeader
                     WHERE ReturnNumber LIKE 'SR-' + @Today + '-%'
                 );

                 SELECT 'SR-' + @Today + '-' + CAST(@NextNo AS VARCHAR(10));
                 ";

            return await con.ExecuteScalarAsync<string>(query);
        }
        public Task<bool> DeleteSaleReturnAsync(int saleReturnId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SaleReturnHeaderVM>> GetAllSaleReturnsAsync()
        {
            using var con = _context.CreateConnection();

            const string query = @"
                SELECT
                    SRH.SaleReturnId,
                    SRH.ReturnNumber,
                    SRH.ReturnDate,
                    SRH.InvoiceId,
                    SRH.CustomerId,
                    C.CustomerName,
                    SRH.BranchId,
                    SRH.WarehouseId,
                    W.WarehouseName,
                    SRH.TotalAmount,
                    SRH.Remarks
                FROM SaleReturnHeader SRH
                INNER JOIN Customers C
                    ON SRH.CustomerId = C.CustomerId
                INNER JOIN Warehouse W
                    ON SRH.WarehouseId = W.WarehouseId
                WHERE SRH.IsDeleted = 0
                ORDER BY SRH.SaleReturnId DESC;";

            var result = await con.QueryAsync<SaleReturnHeaderVM>(query);

            return result;
        }

        public async Task<SaleReturnHeaderVM> GetInvoiceForSaleReturnAsync(int invoiceId)
        {
            using var con = _context.CreateConnection();

            var model = new SaleReturnHeaderVM();

            //=====================================
            // Header
            //=====================================

            const string headerQuery = @"
    SELECT
        PI.InvoiceId,
        PI.CustomerId,
        PI.WarehouseId,
        PI.BranchId
    FROM PosInvoice PI
    WHERE PI.InvoiceId = @InvoiceId
      AND PI.IsDeleted = 0;";

            model = await con.QueryFirstOrDefaultAsync<SaleReturnHeaderVM>(
                headerQuery,
                new { InvoiceId = invoiceId });

            if (model == null)
                return null;

            //=====================================
            // Detail
            //=====================================

            const string detailQuery = @"
    SELECT
        PID.InvoiceDetailId AS SaleInvoiceDetailId,
        PID.ItemId,
        I.ItemName,
        PID.Quantity,
        ISNULL(PID.ReturnQty,0) AS ReturnedQty,
        (PID.Quantity - ISNULL(PID.ReturnQty,0)) AS AvailableQty,
        PID.Rate,
        PID.Amount
    FROM PosInvoiceDetail PID
    INNER JOIN Items I
        ON PID.ItemId = I.ItemId
    WHERE PID.InvoiceId = @InvoiceId
      AND (PID.Quantity - ISNULL(PID.ReturnQty,0)) > 0
    ORDER BY I.ItemName;";

            model.Details = (await con.QueryAsync<SaleReturnDetailVM>(
                detailQuery,
                new { InvoiceId = invoiceId }))
                .ToList();

            return model;
        }

        public async Task<SaleReturnHeaderVM> GetSaleReturnByIdAsync(int saleReturnId)
        {
            using var con = _context.CreateConnection();

            var model = new SaleReturnHeaderVM();

            //=====================================
            // Header
            //=====================================

            const string headerQuery = @"
    SELECT
        SRH.SaleReturnId,
        SRH.ReturnNumber,
        SRH.ReturnDate,
        SRH.SaleInvoiceId AS InvoiceId,
        SRH.CustomerId,
        SRH.BranchId,
        SRH.WarehouseId,
        SRH.TotalAmount,
        SRH.Remarks,
        SRH.CreatedBy
    FROM SaleReturnHeader SRH
    WHERE SRH.SaleReturnId = @SaleReturnId
      AND SRH.IsDeleted = 0;";

            model = await con.QueryFirstOrDefaultAsync<SaleReturnHeaderVM>(
                headerQuery,
                new { SaleReturnId = saleReturnId });

            if (model == null)
                return null;

            //=====================================
            // Details
            //=====================================

            const string detailQuery = @"
    SELECT
        SRD.SaleReturnDetailId,
        SRD.InvoiceDetailId AS SaleInvoiceDetailId,
        SRD.ItemId,
        I.ItemName,
        PID.Quantity,
        ISNULL(PID.ReturnQty,0) AS ReturnedQty,
        (PID.Quantity - ISNULL(PID.ReturnQty,0)) AS AvailableQty,
        SRD.ReturnQty,
        SRD.Rate,
        SRD.Amount
    FROM SaleReturnDetail SRD
    INNER JOIN PosInvoiceDetail PID
        ON SRD.InvoiceDetailId = PID.InvoiceDetailId
    INNER JOIN Items I
        ON SRD.ItemId = I.ItemId
    WHERE SRD.SaleReturnId = @SaleReturnId
    ORDER BY I.ItemName;";

            model.Details = (await con.QueryAsync<SaleReturnDetailVM>(
                detailQuery,
                new { SaleReturnId = saleReturnId }))
                .ToList();

            return model;
        }

        public async Task<int> SaveSaleReturnAsync(SaleReturnHeaderVM model)
        {
            using var con = _context.CreateConnection();

            var parameters = new DynamicParameters();

            parameters.Add("@ReturnNumber", model.ReturnNumber);
            parameters.Add("@ReturnDate", model.ReturnDate);
            parameters.Add("@InvoiceId", model.InvoiceId);
            parameters.Add("@CustomerId", model.CustomerId);
            parameters.Add("@BranchId", model.BranchId);
            parameters.Add("@WarehouseId", model.WarehouseId);
            parameters.Add("@TotalAmount", model.TotalAmount);
            parameters.Add("@Remarks", model.Remarks);
            parameters.Add("@CreatedBy", model.CreatedBy);

            parameters.Add(
                "@Details",
                GetSaleReturnDetailTable(model.Details).AsTableValuedParameter("SaleReturnDetailType")
            );

            var result = await con.ExecuteScalarAsync<int>(
                "sp_SaveSaleReturn",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        private DataTable GetSaleReturnDetailTable(List<SaleReturnDetailVM> details)
        {
            var table = new DataTable();

            table.Columns.Add("SaleInvoiceDetailId", typeof(int));
            table.Columns.Add("ItemId", typeof(int));
            table.Columns.Add("ReturnQty", typeof(decimal));
            table.Columns.Add("Rate", typeof(decimal));
            table.Columns.Add("Amount", typeof(decimal));

            foreach (var item in details)
            {
                table.Rows.Add(
                    item.SaleInvoiceDetailId,
                    item.ItemId,
                    item.ReturnQty,
                    item.Rate,
                    item.Amount
                );
            }

            return table;
        }
    }
}

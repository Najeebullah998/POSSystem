using Dapper;
using Microsoft.Data.SqlClient;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using System.Data;

namespace POSSystem.Repositories
{
    public class PosRepository : IPosRepository
    {
        private readonly DapperContext _context;

        public PosRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<List<Customers>> GetDropdownAsync(int companyId, int branchId)
        {
            using (var con = _context.CreateConnection())
            {
                var result = await con.QueryAsync<Customers>(
                    @"
            SELECT 
                CustomerId,
                CustomerName
            FROM Customers
            WHERE CompanyId = @CompanyId
              AND BranchId = @BranchId
            ORDER BY CustomerName
            ",
                    new
                    {
                        CompanyId = companyId,
                        BranchId = branchId
                    });

                return result.ToList();
            }
        }
        public async Task<string> GenerateInvoiceNoAsync(int branchId)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
            SELECT 
                'INV-' + RIGHT(
                    '000000' + CAST(
                        ISNULL(MAX(InvoiceId), 0) + 1 AS VARCHAR(6)
                    ),
                    6
                )
            FROM PosInvoice
            WHERE  BranchId = @BranchId";

                return await connection.ExecuteScalarAsync<string>(
                    sql,
                    new
                    {
                        BranchId = branchId
                    });
            }
        }
        public IEnumerable<ItemSearchVM> SearchItems(string term)
        {
            using (var con = _context.CreateConnection())
            {
                var sql = @"
            SELECT TOP (20)
                ItemId,
                ItemName,
                Barcode,
                SalePrice,
                StockQty
            FROM Item
            WHERE IsActive = 1
              AND (
                    ItemName LIKE '%' + @Term + '%'
                 OR Barcode LIKE '%' + @Term + '%'
              )
            ORDER BY ItemName";

                return con.Query<ItemSearchVM>(sql, new
                {
                    Term = term
                });
            }
        }
        public bool SaveInvoice(PosInvoiceVm model)
        {
            using (var db = _context.CreateConnection())
            {
                var dt = new DataTable();

                // ==========================================
                // Invoice Detail TVP
                // ==========================================

                dt.Columns.Add("ItemId", typeof(int));
                dt.Columns.Add("Quantity", typeof(decimal));
                dt.Columns.Add("Rate", typeof(decimal));
                dt.Columns.Add("Amount", typeof(decimal));

                // Pharmacy Fields
                dt.Columns.Add("BatchNo", typeof(string));
                dt.Columns.Add("ManufacturingDate", typeof(DateTime));
                dt.Columns.Add("ExpiryDate", typeof(DateTime));


                foreach (var item in model.Items)
                {
                    dt.Rows.Add(
                        item.ItemId,
                        item.Quantity,
                        item.Rate,
                        item.Amount,

                        // Currently NULL for normal POS
                        DBNull.Value,
                        DBNull.Value,
                        DBNull.Value
                    );
                }


                // ==========================================
                // Stored Procedure Parameters
                // ==========================================

                var parameters = new DynamicParameters();

                parameters.Add("@InvoiceNo", model.InvoiceNo);
                parameters.Add("@InvoiceDate", model.InvoiceDate);
                parameters.Add("@CustomerId", model.CustomerId);

                // Company + Branch
                parameters.Add("@CompanyId", model.CompanyId);
                parameters.Add("@BranchId", model.BranchId);

                parameters.Add("@UserId", model.UserId);

                parameters.Add("@TotalAmount", model.TotalAmount);
                parameters.Add("@Discount", model.Discount);
                parameters.Add("@NetAmount", model.NetAmount);

                // Payment
                parameters.Add("@PaymentMode", model.PaymentMode);
                parameters.Add("@PaidAmount", model.NetAmount);

                parameters.Add("@CreatedBy", model.UserId);


                // ==========================================
                // Detail TVP
                // ==========================================

                parameters.Add(
                    "@InvoiceDetails",
                    dt.AsTableValuedParameter("dbo.PosInvoiceDetailType")
                );


                // ==========================================
                // Execute Stored Procedure
                // ==========================================

                var result = db.Execute(
                    "sp_SavePosInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result > 0;
            }
        }
        public bool UpdateInvoice(PosInvoiceVm model)
        {
            using (var db = _context.CreateConnection())
            {
                var dt = new DataTable();

                dt.Columns.Add("ItemId", typeof(int));
                dt.Columns.Add("Quantity", typeof(decimal));
                dt.Columns.Add("Rate", typeof(decimal));
                dt.Columns.Add("Amount", typeof(decimal));

                dt.Columns.Add("BatchNo", typeof(string));
                dt.Columns.Add("ManufacturingDate", typeof(DateTime));
                dt.Columns.Add("ExpiryDate", typeof(DateTime));

                foreach (var item in model.Items)
                {
                    dt.Rows.Add(
                        item.ItemId,
                        item.Quantity,
                        item.Rate,
                        item.Amount,
                        string.IsNullOrEmpty(item.BatchNo)
                            ? (object)DBNull.Value
                            : item.BatchNo,
                        item.ManufacturingDate.HasValue
                            ? (object)item.ManufacturingDate.Value
                            : DBNull.Value,
                        item.ExpiryDate.HasValue
                            ? (object)item.ExpiryDate.Value
                            : DBNull.Value
                    );
                }

                var parameters = new DynamicParameters();

                parameters.Add("@InvoiceId", model.InvoiceId);
                parameters.Add("@InvoiceNo", model.InvoiceNo);
                parameters.Add("@InvoiceDate", model.InvoiceDate);
                parameters.Add("@CustomerId", model.CustomerId);
                parameters.Add("@BranchId", model.BranchId);
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@TotalAmount", model.TotalAmount);
                parameters.Add("@Discount", model.Discount);
                parameters.Add("@NetAmount", model.NetAmount);
                parameters.Add("@ModifiedBy", model.UserId);

                parameters.Add(
                    "@InvoiceDetails",
                    dt.AsTableValuedParameter("dbo.PosInvoiceDetailType")
                );

                db.Execute(
                    "sp_UpdatePosInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Procedure completed successfully
                return true;
            }
        }

        public bool DeleteInvoice(int invoiceId, int modifiedBy)
        {
            using (var db = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@InvoiceId", invoiceId);
                parameters.Add("@ModifiedBy", modifiedBy);

                db.Execute(
                    "sp_DeletePosInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
        }

        public async Task<SaleClosingSummary> GetSaleClosingSummaryAsync(int companyId,int branchId,int userId,DateTime closingDate)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<SaleClosingSummary>(
                "sp_GetSaleClosingSummary",
                new
                {
                    CompanyId = companyId,
                    BranchId = branchId,
                    UserId = userId,
                    ClosingDate = closingDate
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<(PosInvoiceVm Invoice, List<PosInvoiceDetailVm> Details)> GetBillByIdAsync(int invoiceId)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@InvoiceId", invoiceId);

                using (var multi = await connection.QueryMultipleAsync(
                    "sp_GetBillById",
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    var invoice = await multi.ReadFirstOrDefaultAsync<PosInvoiceVm>();
                    var details = (await multi.ReadAsync<PosInvoiceDetailVm>()).ToList();

                    return (invoice, details);
                }
            }
        }

        public async Task<IEnumerable<PosInvoiceVm>> GetCompleteBillsAsync(
    int companyId,
    int branchId,
    DateTime? fromDate,
    DateTime? toDate,
    string invoiceNo)
        {
            using (var db = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyId", companyId);
                parameters.Add("@BranchId", branchId);
                parameters.Add("@FromDate", fromDate);
                parameters.Add("@ToDate", toDate);
                parameters.Add("@InvoiceNo", invoiceNo);

                return await db.QueryAsync<PosInvoiceVm>(
                    "sp_GetCompleteBills",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }
        public async Task<SaleClosingSaveResult> SaveAsync(
    int companyId,
    int branchId,
    int userId,
    DateTime closingDate,
    decimal openingCash,
    decimal totalSales,
    decimal totalReturns,
    decimal netSales,
    decimal cashSales,
    decimal easyPaisaSales,
    decimal otherSales,
    decimal actualCash,
    string remarks)
        {
            using (var con = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyId", companyId);
                parameters.Add("@BranchId", branchId);
                parameters.Add("@UserId", userId);
                parameters.Add("@ClosingDate", closingDate.Date);

                parameters.Add("@OpeningCash", openingCash);

                parameters.Add("@TotalSales", totalSales);
                parameters.Add("@TotalReturns", totalReturns);
                parameters.Add("@NetSales", netSales);

                parameters.Add("@CashSales", cashSales);
                parameters.Add("@EasyPaisaSales", easyPaisaSales);
                parameters.Add("@OtherSales", otherSales);

                parameters.Add("@ActualCash", actualCash);

                parameters.Add("@Remarks", remarks);

                var result = await con.QueryFirstOrDefaultAsync<SaleClosingSaveResult>(
                    "sp_SaveSaleClosing",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
        }
    }
}

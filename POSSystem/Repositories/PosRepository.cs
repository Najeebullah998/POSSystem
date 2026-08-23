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
                // 🔥 Create DataTable (TVP)
                var dt = new DataTable();
                dt.Columns.Add("ItemId", typeof(int));
                dt.Columns.Add("Quantity", typeof(decimal));
                dt.Columns.Add("Rate", typeof(decimal));
                dt.Columns.Add("Amount", typeof(decimal));

                foreach (var item in model.Items)
                {
                    dt.Rows.Add(item.ItemId, item.Quantity, item.Rate, item.Amount);
                }

                var parameters = new DynamicParameters();
                parameters.Add("@InvoiceNo", model.InvoiceNo);
                parameters.Add("@InvoiceDate", model.InvoiceDate);
                parameters.Add("@CustomerId", model.CustomerId);
                parameters.Add("@BranchId", model.BranchId);
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@TotalAmount", model.TotalAmount);
                parameters.Add("@Discount", model.Discount);
                parameters.Add("@NetAmount", model.NetAmount);
                parameters.Add("@CreatedBy", model.UserId);

                parameters.Add("@InvoiceDetails",
                    dt.AsTableValuedParameter("dbo.PosInvoiceDetailType"));

                var result = db.Execute("sp_SavePosInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result > 0;
            }
        }
    }
}

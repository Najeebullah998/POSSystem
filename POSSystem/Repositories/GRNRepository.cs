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

                // CompanyId is required by sp_SaveGRN
                param.Add("@CompanyId", model.CompanyId);

                param.Add("@BranchId", model.BranchId);
                param.Add("@WarehouseId", model.WarehouseId);
                param.Add("@TotalAmount", model.TotalAmount);
                param.Add("@Remarks", model.Remarks);
                param.Add("@CreatedBy", model.CreatedBy);


                //===========================
                // Detail TVP
                //===========================

                DataTable detailTable =
                    GetGRNDetailTable(model.Details);

                param.Add(
                    "@Details",
                    detailTable.AsTableValuedParameter("GRNDetailType")
                );


                //===========================
                // Execute Stored Procedure
                //===========================

                var result = await con.QuerySingleAsync<dynamic>(
                    "sp_SaveGRN",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                return (int)result.GRNId;
            }
        }


        private DataTable GetGRNDetailTable(
            List<GRNDetailVM> details)
        {
            DataTable dt = new DataTable();

            //===========================
            // Columns must match
            // GRNDetailType
            //===========================

            dt.Columns.Add("ItemId", typeof(int));

            dt.Columns.Add(
                "ReceivedQty",
                typeof(decimal)
            );

            dt.Columns.Add(
                "Rate",
                typeof(decimal)
            );

            dt.Columns.Add(
                "Amount",
                typeof(decimal)
            );

            // Pharmacy fields
            dt.Columns.Add(
                "BatchNo",
                typeof(string)
            );

            dt.Columns.Add(
                "ManufacturingDate",
                typeof(DateTime)
            );

            dt.Columns.Add(
                "ExpiryDate",
                typeof(DateTime)
            );


            //===========================
            // Add Details
            //===========================

            foreach (var item in details)
            {
                DataRow row = dt.NewRow();

                row["ItemId"] = item.ItemId;

                row["ReceivedQty"] = item.ReceivedQty;

                row["Rate"] = item.Rate;

                row["Amount"] = item.Amount;


                //===========================
                // Pharmacy Fields
                //===========================

                row["BatchNo"] =
                    string.IsNullOrWhiteSpace(item.BatchNo)
                        ? DBNull.Value
                        : item.BatchNo;


                row["ManufacturingDate"] =
                    item.ManufacturingDate.HasValue
                        ? item.ManufacturingDate.Value
                        : DBNull.Value;


                row["ExpiryDate"] =
                    item.ExpiryDate.HasValue
                        ? item.ExpiryDate.Value
                        : DBNull.Value;


                dt.Rows.Add(row);
            }

            return dt;
        }

        public async Task<GRNHeaderVM> GetPurchaseOrderByIdAsync(int purchaseOrderId,int companyId,int branchId)
        {
            using (var con = _context.CreateConnection())
            {
                using (var multi = await con.QueryMultipleAsync(
                    "sp_GetPurchaseOrderForGRN",
                    new
                    {
                        PurchaseOrderId = purchaseOrderId,
                        CompanyId = companyId,
                        BranchId = branchId
                    },
                    commandType: CommandType.StoredProcedure))
                {
                    var header = await multi.ReadFirstOrDefaultAsync<GRNHeaderVM>();

                    if (header != null)
                    {
                        header.Details =
                            (await multi.ReadAsync<GRNDetailVM>()).ToList();
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

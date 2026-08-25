using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Core.Types;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;

namespace POSSystem.Controllers
{
    public class POSController : Controller
    {
        private readonly DapperContext _context;
        private readonly IPosRepository _repo;
        public POSController(DapperContext context,IPosRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateSale()
        {
            return View();
        }

        [HttpGet]
        public JsonResult SearchItems(string term)
        {
            var data = _repo.SearchItems(term);

            return Json(data);
        }
        public JsonResult GetItemByBarcode(string barcode)
        {
            using (var db = _context.CreateConnection())
            {
                string query = @"
                SELECT 
                    i.ItemId AS itemId,
                    i.ItemName AS itemName,
                    i.SalePrice AS salePrice,
                    ISNULL(s.Quantity,0) AS stockQty
                FROM Items i
                LEFT JOIN Stock s ON i.ItemId = s.ItemId
                WHERE (i.Barcode = @Barcode 
                       OR i.ItemName LIKE '%' + @Barcode + '%')
                AND ISNULL(i.IsDeleted,0) = 0
            ";

                var item = db.QueryFirstOrDefault(query, new { Barcode = barcode });

                return Json(item);
            }
        }

        [HttpPost]
        public IActionResult SaveInvoice([FromBody] PosInvoiceVm model)
        {
            if (model == null || model.Items == null || model.Items.Count == 0)
            {
                return Json(new { success = false, message = "No items found!" });
            }

            var invoiceId = _repo.SaveInvoice(model);

            return Json(new { success = true, invoiceId });
        }
        [HttpPost]
        public IActionResult UpdateInvoice([FromBody] PosInvoiceVm model)
        {
            if (model == null || model.InvoiceId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid Invoice ID!"
                });
            }

            if (model.Items == null || model.Items.Count == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "No items found!"
                });
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                if (!userId.HasValue)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User session expired. Please login again."
                    });
                }

                model.UserId = userId.Value;

                var result = _repo.UpdateInvoice(model);

                return Json(new
                {
                    success = result,
                    invoiceId = model.InvoiceId,
                    message = result
                        ? "Invoice updated successfully!"
                        : "Invoice could not be updated."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult DeleteBill(int invoiceId)
        {
            if (invoiceId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid Invoice ID!"
                });
            }

            try
            {
                // Get logged-in user from session
                var userId = HttpContext.Session.GetInt32("UserId");

                if (!userId.HasValue)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User session expired. Please login again."
                    });
                }

                // Delete invoice and restore stock
                var result = _repo.DeleteInvoice(invoiceId, userId.Value);

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Bill deleted successfully!"
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Bill could not be deleted."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        public IActionResult CompleteBill()
        {
            return View();
        }

        public async Task<IActionResult> SaleClosing()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSaleClosingSummary(DateTime closingDate)
        {
            try
            {
                var branchId = Convert.ToInt32(HttpContext.Session.GetInt32("BranchId"));
                var userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));

                var result = await _repo.GetSaleClosingSummaryAsync(
                    branchId,
                    userId,
                    closingDate
                );

                if (result == null)
                {
                    return Json(new SaleClosingSummary
                    {
                        TotalBills = 0,
                        GrossSales = 0,
                        Discount = 0,
                        NetSales = 0,
                        TotalReturns = 0,
                        NetAfterReturns = 0,
                        Cash = 0,
                        EasyPaisa = 0,
                        Other = 0,
                        TotalPayments = 0,
                        ExpectedCash = 0
                    });
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetBillById(int invoiceId)
        {
            try
            {
                var result = await _repo.GetBillByIdAsync(invoiceId);

                if (result.Invoice == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Bill not found."
                    });
                }

                return Json(new
                {
                    success = true,
                    invoice = result.Invoice,
                    details = result.Details
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCompleteBills(
    DateTime? fromDate,
    DateTime? toDate,
    string invoiceNo)
        {
            try
            {
                int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

                if (branchId == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Branch not found in session."
                    });
                }

                var bills = await _repo.GetCompleteBillsAsync(
                    branchId,
                    fromDate,
                    toDate,
                    invoiceNo
                );

                return Json(new
                {
                    success = true,
                    data = bills
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}

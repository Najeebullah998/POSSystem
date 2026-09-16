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

        public async Task<IActionResult> CreateSale(int? invoiceId)
        {
            int companyId =
                HttpContext.Session.GetInt32("CompanyId") ?? 0;

            int branchId =
                HttpContext.Session.GetInt32("BranchId") ?? 0;

            if (companyId <= 0 || branchId <= 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new PosInvoiceVm();

            model.Customerslist =
                await _repo.GetDropdownAsync(companyId, branchId);

            ViewBag.InvoiceId =
                invoiceId ?? 0;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateInvoiceNo()
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
                int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

                if (companyId == 0 || branchId == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Company or Branch session not found."
                    });
                }

                var invoiceNo = await _repo.GenerateInvoiceNoAsync(branchId);

                return Json(new
                {
                    success = true,
                    invoiceNo = invoiceNo
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
        public JsonResult SearchItems(string term)
        {
            var data = _repo.SearchItems(term);

            return Json(data);
        }
        public JsonResult GetItemByBarcode(string barcode)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            using (var db = _context.CreateConnection())
            {
                string query = @"
            SELECT 
                i.ItemId AS itemId,
                i.ItemName AS itemName,

                i.SalePrice AS salePrice,
                i.SaleQuantity AS saleQuantity,

                CASE 
                    WHEN ISNULL(i.SaleQuantity, 0) > 0
                    THEN i.SalePrice / i.SaleQuantity
                    ELSE 0
                END AS rate,

                ISNULL(SUM(s.Quantity), 0) AS stockQty

            FROM Items i

            LEFT JOIN Stock s 
                ON i.ItemId = s.ItemId
                AND s.CompanyId = @CompanyId
                AND s.BranchId = @BranchId
                AND ISNULL(s.IsDeleted, 0) = 0
                AND ISNULL(s.IsActive, 1) = 1

            WHERE 
                (
                    i.Barcode = @Barcode
                    OR i.ItemName LIKE '%' + @Barcode + '%'
                )

                AND ISNULL(i.IsDeleted, 0) = 0
                AND ISNULL(i.IsActive, 1) = 1

                AND i.CompanyId = @CompanyId
                AND i.BranchId = @BranchId

            GROUP BY
                i.ItemId,
                i.ItemName,
                i.SalePrice,
                i.SaleQuantity
        ";

                var item = db.QueryFirstOrDefault(
                    query,
                    new
                    {
                        Barcode = barcode,
                        CompanyId = companyId,
                        BranchId = branchId
                    }
                );

                return Json(item);
            }
        }

        [HttpPost]
        public IActionResult SaveInvoice([FromBody] PosInvoiceVm model)
        {
            try
            {
                if (model == null || model.Items == null || model.Items.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No items found!"
                    });
                }

                // ==========================================
                // Get IDs from Session
                // ==========================================

                int companyId =
                    HttpContext.Session.GetInt32("CompanyId") ?? 0;

                int branchId =
                    HttpContext.Session.GetInt32("BranchId") ?? 0;

                int userId =
                    HttpContext.Session.GetInt32("UserId") ?? 0;


                // ==========================================
                // Validate Session
                // ==========================================

                if (companyId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Company not found in session."
                    });
                }

                if (branchId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Branch not found in session."
                    });
                }

                if (userId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User not found in session."
                    });
                }


                // ==========================================
                // Override Frontend Values
                // ==========================================

                model.CompanyId = companyId;
                model.BranchId = branchId;
                model.UserId = userId;


                // CreatedBy should also be logged-in user
                model.CreatedBy = userId;


                // ==========================================
                // Save Invoice
                // ==========================================

                var invoiceId = _repo.SaveInvoice(model);


                return Json(new
                {
                    success = true,
                    invoiceId = invoiceId
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
        public async Task<IActionResult> UpdateInvoice([FromBody] PosInvoiceVm model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid request."
                    });
                }


                if (model.InvoiceId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid Invoice ID."
                    });
                }


                if (model.Items == null ||
                    model.Items.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No items found!"
                    });
                }


                // ==========================================
                // SESSION
                // ==========================================

                int companyId =
                    HttpContext.Session.GetInt32("CompanyId") ?? 0;

                int branchId =
                    HttpContext.Session.GetInt32("BranchId") ?? 0;

                int userId =
                    HttpContext.Session.GetInt32("UserId") ?? 0;


                // ==========================================
                // SESSION VALIDATION
                // ==========================================

                if (companyId <= 0 ||
                    branchId <= 0 ||
                    userId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "Session expired. Please login again."
                    });
                }


                // ==========================================
                // SECURITY VALUES
                // ==========================================

                model.CompanyId =
                    companyId;

                model.BranchId =
                    branchId;

                model.UserId =
                    userId;

                model.ModifiedBy =
                    userId;


                // ==========================================
                // UPDATE
                // ==========================================

                bool result =
                    await _repo.UpdatePosInvoiceAsync(
                        companyId,
                        branchId,
                        model.InvoiceId,
                        model,
                        model.Items,
                        userId
                    );


                return Json(new
                {
                    success = result,

                    invoiceId =
                        model.InvoiceId,

                    message =
                        result
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
        public IActionResult DeleteInvoiceDetail(int invoiceDetailId)
        {
            if (invoiceDetailId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid Invoice Detail ID!"
                });
            }

            try
            {
                // Get logged-in user
                var userId = HttpContext.Session.GetInt32("UserId");

                if (!userId.HasValue)
                {
                    return Json(new
                    {
                        success = false,
                        message = "User session expired. Please login again."
                    });
                }

                // Get Company and Branch from session
                var companyId = HttpContext.Session.GetInt32("CompanyId");
                var branchId = HttpContext.Session.GetInt32("BranchId");

                if (!companyId.HasValue || !branchId.HasValue)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Company or Branch session expired. Please login again."
                    });
                }

                // Delete invoice detail and restore stock
                var result = _repo.DeleteInvoiceDetail(
                    invoiceDetailId,
                    companyId.Value,
                    branchId.Value,
                    userId.Value
                );

                if (result)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Item deleted successfully and stock restored."
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Item could not be deleted."
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
                var companyId = Convert.ToInt32(HttpContext.Session.GetInt32("CompanyId"));

                var result = await _repo.GetSaleClosingSummaryAsync(
                    companyId,
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
                int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
                int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

                if (companyId <= 0 || branchId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Company or Branch session expired. Please login again."
                    });
                }

                if (invoiceId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid Invoice ID."
                    });
                }

                var result = await _repo.GetBillByIdAsync(
                    companyId,
                    branchId,
                    invoiceId
                );

                if (result.Header == null)
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
                    invoice = result.Header,
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
        public async Task<IActionResult> GetCompleteBills(DateTime? fromDate,DateTime? toDate,string invoiceNo)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
                int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

                if (companyId == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Company not found in session."
                    });
                }

                if (branchId == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Branch not found in session."
                    });
                }

                var bills = await _repo.GetCompleteBillsAsync(companyId,branchId,fromDate,toDate,invoiceNo);

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

        public async Task<IActionResult> SaveSaleClosing()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveSaleClosing([FromBody] SaleClosingSaveVM model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid request."
                    });
                }

                // Get CompanyId, BranchId and UserId from Session
                int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
                int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
                int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

                // Validate session
                if (companyId <= 0 || branchId <= 0 || userId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Session expired. Please login again."
                    });
                }

                // Validate closing date
                if (model.ClosingDate == default(DateTime))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Closing date is required."
                    });
                }

                // Save closing
                var result = await _repo.SaveAsync(
                    companyId,
                    branchId,
                    userId,
                    model.ClosingDate,
                    model.OpeningCash,
                    model.TotalSales,
                    model.TotalReturns,
                    model.NetSales,
                    model.CashSales,
                    model.EasyPaisaSales,
                    model.OtherSales,
                    model.ActualCash,
                    model.Remarks
                );

                if (result == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Sale closing could not be saved."
                    });
                }

                return Json(new
                {
                    success = true,
                    message = result.Message,
                    closingId = result.ClosingId,
                    closingNo = result.ClosingNo,
                    expectedCash = result.ExpectedCash,
                    actualCash = result.ActualCash,
                    cashDifference = result.CashDifference
                });
            }
            catch (SqlException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while saving sale closing."
                });
            }
        }
    }
}

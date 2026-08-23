using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Protocol.Core.Types;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;

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
    }
}

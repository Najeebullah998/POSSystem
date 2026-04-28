using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using POSSystem.DATA;

namespace POSSystem.Controllers
{
    public class POSController : Controller
    {
        private readonly DapperContext _context;

        public POSController(DapperContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateSale()
        {
            return View();
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
    }
}

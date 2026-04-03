using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;

namespace POSSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomer _repo;

        public CustomerController(ICustomer repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _repo.GetAllAsync();
            return Json(customers);
        }
        public IActionResult AddCustomers()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCustomers([FromBody] Customers customers)
        {
            if (customers == null)
                return Json(new { success = false, message = "Invalid data" });

            customers.CreatedBy = 1; // session se le sakte ho
            customers.IsActive = customers.IsActive;

            int id = await _repo.AddAsync(customers);

            if (id > 0)
            {
                return Json(new { success = true, message = "Customer saved successfully" });
            }

            return Json(new { success = false, message = "Error saving customer" });
        }
    }
}

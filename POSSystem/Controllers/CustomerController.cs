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

        [HttpGet]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _repo.GetByIdAsync(id);
            return Json(customer);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCustomer([FromBody] Customers customers)
        {
            if (customers == null || customers.CustomerId == 0)
                return Json(new { success = false, message = "Invalid data" });

            customers.ModifiedBy = 1;

            await _repo.UpdateAsync(customers);

            return Json(new { success = true, message = "Customer updated successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            await _repo.DeleteAsync(id);
            return Json(new { success = true, message = "Customer deleted successfully" });
        }
    }
}

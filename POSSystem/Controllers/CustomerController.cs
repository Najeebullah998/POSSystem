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
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            var customers = await _repo.GetAllAsync(companyId, branchId);

            return Json(customers);
        }


        [HttpGet]
        public IActionResult AddCustomers()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddCustomers(
            [FromBody] Customers customers)
        {
            if (customers == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            customers.CreatedBy = userId;

            int id = await _repo.AddAsync(
                customers,
                companyId,
                branchId
            );

            if (id > 0)
            {
                return Json(new
                {
                    success = true,
                    message = "Customer saved successfully",
                    customerId = id
                });
            }

            return Json(new
            {
                success = false,
                message = "Error saving customer"
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

            var customer = await _repo.GetByIdAsync(
                id,
                companyId,
                branchId
            );

            return Json(customer);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(
            [FromBody] Customers customers)
        {
            if (customers == null || customers.CustomerId == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            customers.ModifiedBy = userId;

            await _repo.UpdateAsync(
                customers,
                companyId,
                branchId
            );

            return Json(new
            {
                success = true,
                message = "Customer updated successfully"
            });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
            int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            await _repo.DeleteAsync(
                id,
                companyId,
                branchId,
                userId
            );

            return Json(new
            {
                success = true,
                message = "Customer deleted successfully"
            });
        }
    }
}

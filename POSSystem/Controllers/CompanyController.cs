using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Repositories;

namespace POSSystem.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _repo;

        public CompanyController(ICompanyRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCompany()
        {
            var Company = await _repo.GetAllAsync();
            return Json(Company);
        }
        public IActionResult AddCompany()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCompany(Company company, IFormFile LogoFile)
        {
            if (company == null)
                return Json(new { success = false, message = "Invalid data" });

            string filePath = "";

            if (LogoFile != null)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                string fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(stream);
                }

                filePath = "/images/" + fileName;
            }

            // DB me path save karo
            company.LogoPath = filePath;

            int id = await _repo.AddAsync(company);

            if (id > 0)
            {
                return Json(new { success = true, message = "Company saved successfully" });
            }

            return Json(new { success = false, message = "Error saving Company" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var Company = await _repo.GetByIdAsync(id);
            return Json(Company);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCompany(Company company, IFormFile LogoFile)
        {
            if (company == null || company.CompanyId == 0)
                return Json(new { success = false, message = "Invalid data" });

            // 🔹 pehly DB se old record lao
            var existingCompany = await _repo.GetByIdAsync(company.CompanyId);

            if (existingCompany == null)
                return Json(new { success = false, message = "Company not found" });

            string filePath = existingCompany.LogoPath; // default old path

            // 🔥 agar new file ayi hai
            if (LogoFile != null)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                // 🔴 OLD FILE DELETE
                if (!string.IsNullOrEmpty(existingCompany.LogoPath))
                {
                    string oldFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingCompany.LogoPath.TrimStart('/'));

                    if (System.IO.File.Exists(oldFileFullPath))
                    {
                        System.IO.File.Delete(oldFileFullPath);
                    }
                }

                // 🟢 NEW FILE SAVE
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                string fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(stream);
                }

                filePath = "/images/" + fileName;
            }

            // 🔹 updated path set karo
            company.LogoPath = filePath;

            await _repo.UpdateAsync(company);

            return Json(new { success = true, message = "Company updated successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            await _repo.DeleteAsync(id);
            return Json(new { success = true, message = "Company deleted successfully" });
        }
    }
}

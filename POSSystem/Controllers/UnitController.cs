using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;

namespace POSSystem.Controllers
{
    public class UnitController : Controller
    {
        private readonly IUnitRepository _repo;

        public UnitController(IUnitRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateUnit()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var branchId = HttpContext.Session.GetInt32("BranchId");

            if (companyId == null || branchId == null)
                return Json(new { success = false, message = "Session expired." });

            var units = await _repo.GetAllAsync(
                companyId.Value,
                branchId.Value);

            return Json(new
            {
                success = true,
                data = units
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var branchId = HttpContext.Session.GetInt32("BranchId");

            if (companyId == null || branchId == null)
                return Json(new { success = false, message = "Session expired." });

            var unit = await _repo.GetByIdAsync(
                id,
                companyId.Value,
                branchId.Value);

            if (unit == null)
                return Json(new
                {
                    success = false,
                    message = "Unit not found."
                });

            return Json(new
            {
                success = true,
                data = unit
            });
        }
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] UnitViewModel unit)
        {
            try
            {
                Console.WriteLine("========== UNIT SAVE ==========");
                Console.WriteLine("UnitId: " + unit?.UnitId);
                Console.WriteLine("UnitName: " + unit?.UnitName);
                Console.WriteLine("IsActive: " + unit?.IsActive);

                if (unit == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Unit data is null."
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                        .Select(x => new
                        {
                            Field = x.Key,
                            Errors = x.Value!.Errors
                                .Select(e => e.ErrorMessage)
                                .ToList()
                        })
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = "Please enter valid data.",
                        errors = errors
                    });
                }

                var companyId = HttpContext.Session.GetInt32("CompanyId");
                var branchId = HttpContext.Session.GetInt32("BranchId");
                var userId = HttpContext.Session.GetInt32("UserId");

                if (companyId == null ||
                    branchId == null ||
                    userId == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Session expired."
                    });
                }

                unit.CompanyId = companyId.Value;
                unit.BranchId = branchId.Value;

                if (unit.UnitId > 0)
                {
                    unit.ModifiedBy = userId.Value;
                }
                else
                {
                    unit.CreatedBy = userId.Value;
                }

                var result = await _repo.SaveAsync(unit);

                return Json(new
                {
                    success = result > 0,
                    message = unit.UnitId > 0
                        ? "Unit updated successfully."
                        : "Unit saved successfully.",
                    id = result
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
        public async Task<IActionResult> Delete(int id)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            var branchId = HttpContext.Session.GetInt32("BranchId");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (companyId == null ||
                branchId == null ||
                userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Session expired."
                });
            }

            var result = await _repo.DeleteAsync(
                id,
                companyId.Value,
                branchId.Value,
                userId.Value);

            return Json(new
            {
                success = result,
                message = result
                    ? "Unit deleted successfully."
                    : "Unit could not be deleted."
            });
        }
    }
}

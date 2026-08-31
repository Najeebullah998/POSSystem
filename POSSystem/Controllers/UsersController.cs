using Microsoft.AspNetCore.Mvc;
using POSSystem.Entities;
using POSSystem.Interfaces;

public class UsersController : Controller
{
    private readonly IUsers _repo;

    public UsersController(IUsers repo)
    {
        _repo = repo;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
        int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

        var data = await _repo.GetAllAsync(companyId, branchId);

        return Json(data);
    }


    [HttpGet]
    public async Task<IActionResult> GetUserById(int id)
    {
        int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0;
        int branchId = HttpContext.Session.GetInt32("BranchId") ?? 0;

        var user = await _repo.GetByIdAsync(
            id,
            companyId,
            branchId
        );

        return Json(user);
    }


    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] Users user)
    {
        if (user == null)
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

        user.CreatedBy = userId;

        int id = await _repo.AddAsync(
            user,
            companyId,
            branchId
        );

        return Json(new
        {
            success = id > 0,
            message = id > 0
                ? "User saved successfully"
                : "Error saving user",
            userId = id
        });
    }


    [HttpPost]
    public async Task<IActionResult> UpdateUser([FromBody] Users user)
    {
        if (user == null || user.UserId == 0)
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

        user.ModifiedBy = userId;

        await _repo.UpdateAsync(
            user,
            companyId,
            branchId
        );

        return Json(new
        {
            success = true,
            message = "User updated successfully"
        });
    }


    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
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
            message = "User deleted successfully"
        });
    }


    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {

        var roles = await _repo.GetRolesAsync();
        return Json(roles);
    }


    [HttpGet]
    public async Task<IActionResult> GetBranches()
    {
        int companyId = HttpContext.Session.GetInt32("CompanyId") ?? 0; 

        var branches = await _repo.GetBranchesAsync(companyId);

        return Json(branches);
    }


}
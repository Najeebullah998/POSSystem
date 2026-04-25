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
        var data = await _repo.GetAllAsync();
        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        return Json(user);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] Users user)
    {
        user.CreatedBy = 1;

        int id = await _repo.AddAsync(user);

        return Json(new { success = id > 0, message = "User saved" });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser([FromBody] Users user)
    {
        user.ModifiedBy = 1;

        await _repo.UpdateAsync(user);

        return Json(new { success = true, message = "User updated" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _repo.DeleteAsync(id);
        return Json(new { success = true, message = "User deleted" });
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        return Json(await _repo.GetRolesAsync());
    }

    [HttpGet]
    public async Task<IActionResult> GetBranches()
    {
        return Json(await _repo.GetBranchesAsync());
    }
}
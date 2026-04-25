using BCrypt.Net;
using Dapper;
using Microsoft.Data.SqlClient;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;

public class AccountRepository
{
    private readonly DapperContext _context;

    public AccountRepository(DapperContext context)
    {
        _context = context;
    }

    public LoginVm Login(string username, string password)
    {
        
        string query = @"
           select u.UserId, u.UserName, u.PasswordHash, r.RoleName, 
           b.BranchId, b.BranchName, 
           c.CompanyId, c.CompanyName
           from users u
           inner join Roles r on u.RoleId = r.RoleId
           inner join Branches b on u.BranchId = b.BranchId
           inner join Companies c on b.CompanyId = c.CompanyId
           WHERE u.UserName = @UserName";

        using var db = _context.CreateConnection();
        var user = db.QueryFirstOrDefault<LoginVm>(query, new { UserName = username });
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return user;
        }

        return null;
    }

}
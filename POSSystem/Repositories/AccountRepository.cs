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
        SELECT 
            u.UserId,
            u.UserName,
            u.PasswordHash,
            r.RoleName,

            b.BranchId,
            b.BranchName,

            c.CompanyId,
            c.CompanyName,

            u.BusinessTypeId,
            bt.BusinessName AS BusinessName

        FROM Users u

        INNER JOIN Roles r
            ON u.RoleId = r.RoleId

        INNER JOIN Branches b
            ON u.BranchId = b.BranchId

        INNER JOIN Companies c
            ON b.CompanyId = c.CompanyId

        LEFT JOIN BusinessType bt
            ON u.BusinessTypeId = bt.BusinessTypeId

        WHERE u.UserName = @UserName";

        using var db = _context.CreateConnection();

        var user = db.QueryFirstOrDefault<LoginVm>(
            query,
            new { UserName = username }
        );

        if (user != null &&
            BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return user;
        }

        return null;
    }

}
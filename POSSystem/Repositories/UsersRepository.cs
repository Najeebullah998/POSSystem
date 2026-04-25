using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using BCrypt.Net;

public class UserRepository : IUsers
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Users user)
    {
        var query = @"
    INSERT INTO Users
    (UserName, PasswordHash, RoleId, BranchId, IsActive, IsDeleted, CreatedOn, CreatedBy)
    VALUES
    (@UserName, @PasswordHash, @RoleId, @BranchId, @IsActive, 0, GETDATE(), @CreatedBy);

    SELECT CAST(SCOPE_IDENTITY() as int);";
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

        using var con = _context.CreateConnection();
        return await con.ExecuteScalarAsync<int>(query, user);
    }

    public async Task UpdateAsync(Users user)
    {
        string query;

        if (!string.IsNullOrEmpty(user.Password))
        {
            // ✅ Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

            query = @"
        UPDATE Users SET
            UserName = @UserName,
            PasswordHash = @PasswordHash,
            RoleId = @RoleId,
            BranchId = @BranchId,
            IsActive = @IsActive,
            ModifiedOn = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE UserId = @UserId AND IsDeleted = 0";
        }
        else
        {
            // ✅ Keep old password
            query = @"
        UPDATE Users SET
            UserName = @UserName,
            RoleId = @RoleId,
            BranchId = @BranchId,
            IsActive = @IsActive,
            ModifiedOn = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE UserId = @UserId AND IsDeleted = 0";
        }

        using var con = _context.CreateConnection();
        await con.ExecuteAsync(query, user);
    }

    public async Task DeleteAsync(int id)
    {
        var query = @"
        UPDATE Users
        SET IsDeleted = 1, IsActive = 0
        WHERE UserId = @Id";

        using var con = _context.CreateConnection();
        await con.ExecuteAsync(query, new { Id = id });
    }

    public async Task<IEnumerable<dynamic>> GetAllAsync()
    {
        var query = @"
    SELECT 
        u.UserId AS userId,
        u.UserName AS userName,
        r.RoleName AS roleName,
        b.BranchName AS branchName,
        u.IsActive AS isActive
    FROM Users u
    JOIN Roles r ON u.RoleId = r.RoleId
    JOIN Branches b ON u.BranchId = b.BranchId
    WHERE u.IsDeleted = 0";

        using var con = _context.CreateConnection();
        return await con.QueryAsync(query);
    }

    public async Task<Users?> GetByIdAsync(int id)
    {
        var query = "SELECT * FROM Users WHERE UserId = @Id AND IsDeleted = 0";

        using var con = _context.CreateConnection();
        return await con.QueryFirstOrDefaultAsync<Users>(query, new { Id = id });
    }

    public async Task<IEnumerable<dynamic>> GetRolesAsync()
    {
        using var con = _context.CreateConnection();

        var query = @"
    SELECT 
        RoleId AS roleId,
        RoleName AS roleName
    FROM Roles 
    WHERE IsDeleted = 0";

        return await con.QueryAsync(query);
    }

    public async Task<IEnumerable<dynamic>> GetBranchesAsync()
    {
        using var con = _context.CreateConnection();

        var query = @"
    SELECT 
        BranchId AS branchId,
        BranchName AS branchName
    FROM Branches 
    WHERE IsDeleted = 0 AND IsActive = 1";

        return await con.QueryAsync(query);
    }
}
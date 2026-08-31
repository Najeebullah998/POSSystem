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

    public async Task<int> AddAsync(
     Users user,
     int companyId,
     int branchId)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

        var query = @"
        DECLARE @UserId INT;

        SELECT @UserId = ISNULL(MAX(UserId), 0) + 1
        FROM Users;

        INSERT INTO Users
        (
            UserId,
            CompanyId,
            BranchId,
            UserName,
            PasswordHash,
            RoleId,
            IsActive,
            IsDeleted,
            CreatedOn,
            CreatedBy
        )
        VALUES
        (
            @UserId,
            @CompanyId,
            @BranchId,
            @UserName,
            @PasswordHash,
            @RoleId,
            @IsActive,
            0,
            GETDATE(),
            @CreatedBy
        );

        SELECT @UserId;
    ";

        using var con = _context.CreateConnection();

        return await con.ExecuteScalarAsync<int>(query, new
        {
            CompanyId = companyId,
            BranchId = branchId,
            user.UserName,
            user.PasswordHash,
            user.RoleId,
            user.IsActive,
            user.CreatedBy
        });
    }


    public async Task UpdateAsync(
        Users user,
        int companyId,
        int branchId)
    {
        string query;

        if (!string.IsNullOrEmpty(user.Password))
        {
            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(user.Password);

            query = @"
            UPDATE Users
            SET
                UserName = @UserName,
                PasswordHash = @PasswordHash,
                RoleId = @RoleId,
                IsActive = @IsActive,
                ModifiedOn = GETDATE(),
                ModifiedBy = @ModifiedBy
            WHERE UserId = @UserId
              AND CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND IsDeleted = 0";
        }
        else
        {
            query = @"
            UPDATE Users
            SET
                UserName = @UserName,
                RoleId = @RoleId,
                IsActive = @IsActive,
                ModifiedOn = GETDATE(),
                ModifiedBy = @ModifiedBy
            WHERE UserId = @UserId
              AND CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND IsDeleted = 0";
        }

        using var con = _context.CreateConnection();

        await con.ExecuteAsync(query, new
        {
            user.UserId,
            user.UserName,
            user.PasswordHash,
            user.Password,
            user.RoleId,
            user.IsActive,
            user.ModifiedBy,
            CompanyId = companyId,
            BranchId = branchId
        });
    }


    public async Task DeleteAsync(
        int id,
        int companyId,
        int branchId,
        int userId)
    {
        var query = @"
        UPDATE Users
        SET
            IsDeleted = 1,
            IsActive = 0,
            ModifiedOn = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE UserId = @Id
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

        using var con = _context.CreateConnection();

        await con.ExecuteAsync(query, new
        {
            Id = id,
            CompanyId = companyId,
            BranchId = branchId,
            ModifiedBy = userId
        });
    }


    public async Task<IEnumerable<dynamic>> GetAllAsync(
        int companyId,
        int branchId)
    {
        var query = @"
        SELECT
            u.UserId AS userId,
            u.UserName AS userName,
            r.RoleName AS roleName,
            b.BranchName AS branchName,
            u.IsActive AS isActive
        FROM Users u
        JOIN Roles r
            ON u.RoleId = r.RoleId
        JOIN Branches b
            ON u.BranchId = b.BranchId
            AND u.CompanyId = b.CompanyId
        WHERE u.CompanyId = @CompanyId
          AND u.IsDeleted = 0
        ORDER BY u.UserName";

        using var con = _context.CreateConnection();

        return await con.QueryAsync(query, new
        {
            CompanyId = companyId,
            BranchId = branchId
        });
    }


    public async Task<Users?> GetByIdAsync(
        int id,
        int companyId,
        int branchId)
    {
        var query = @"
        SELECT *
        FROM Users
        WHERE UserId = @Id
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

        using var con = _context.CreateConnection();

        return await con.QueryFirstOrDefaultAsync<Users>(
            query,
            new
            {
                Id = id,
                CompanyId = companyId,
                BranchId = branchId
            });
    }



    public async Task<IEnumerable<dynamic>> GetRolesAsync()
    {
        using var con = _context.CreateConnection();

        var query = @"
        SELECT
            RoleId AS roleId,
            RoleName AS roleName
        FROM Roles
          WHERE IsDeleted = 0
          AND IsActive = 1
        ORDER BY RoleName";

        return await con.QueryAsync(query);
    }



    public async Task<IEnumerable<dynamic>> GetBranchesAsync(int companyId)
    {
        using var con = _context.CreateConnection();

        var query = @"
        SELECT
            BranchId AS branchId,
            BranchName AS branchName
        FROM Branches
        WHERE CompanyId = @CompanyId
          AND IsDeleted = 0
          AND IsActive = 1
        ORDER BY BranchName";

        return await con.QueryAsync(query, new
        {
            CompanyId = companyId
        });
    }
}
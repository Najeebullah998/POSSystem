using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;
using POSSystem.Interfaces;
using POSSystem.Entities;

public class UnitRepository : IUnitRepository
{
    private readonly DapperContext _context;

    public UnitRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UnitViewModel>> GetAllAsync(
        int companyId,
        int branchId)
    {
        var query = @"
            SELECT
                UnitId,
                UnitName,
                IsActive,
                IsDeleted,
                CreatedOn,
                CreatedBy,
                ModifiedOn,
                ModifiedBy,
                CompanyId,
                BranchId
            FROM Units
            WHERE CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND ISNULL(IsDeleted, 0) = 0
            ORDER BY UnitId DESC";

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<UnitViewModel>(
            query,
            new
            {
                CompanyId = companyId,
                BranchId = branchId
            });
    }

    public async Task<UnitViewModel> GetByIdAsync(
        int unitId,
        int companyId,
        int branchId)
    {
        var query = @"
            SELECT
                UnitId,
                UnitName,
                IsActive,
                IsDeleted,
                CreatedOn,
                CreatedBy,
                ModifiedOn,
                ModifiedBy,
                CompanyId,
                BranchId
            FROM Units
            WHERE UnitId = @UnitId
              AND CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND ISNULL(IsDeleted, 0) = 0";

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<UnitViewModel>(
            query,
            new
            {
                UnitId = unitId,
                CompanyId = companyId,
                BranchId = branchId
            });
    }

    public async Task<int> SaveAsync(UnitViewModel model)
    {
        using var connection = _context.CreateConnection();

        if (model.UnitId == 0)
        {
            var query = @"
            DECLARE @NewUnitId INT;

            SELECT @NewUnitId = ISNULL(MAX(UnitId), 0) + 1
            FROM Units
            WHERE CompanyId = @CompanyId
              AND BranchId = @BranchId;

            INSERT INTO Units
            (
                UnitId,
                UnitName,
                IsActive,
                IsDeleted,
                CreatedOn,
                CreatedBy,
                CompanyId,
                BranchId
            )
            VALUES
            (
                @NewUnitId,
                @UnitName,
                @IsActive,
                0,
                GETDATE(),
                @CreatedBy,
                @CompanyId,
                @BranchId
            );

            SELECT @NewUnitId;";

            return await connection.ExecuteScalarAsync<int>(
                query,
                new
                {
                    UnitName = model.UnitName,
                    IsActive = model.IsActive,
                    CreatedBy = model.CreatedBy,
                    CompanyId = model.CompanyId,
                    BranchId = model.BranchId
                });
        }
        else
        {
            var query = @"
            UPDATE Units
            SET
                UnitName = @UnitName,
                IsActive = @IsActive,
                ModifiedOn = GETDATE(),
                ModifiedBy = @ModifiedBy
            WHERE UnitId = @UnitId
              AND CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND ISNULL(IsDeleted, 0) = 0;

            SELECT @UnitId;";

            return await connection.ExecuteScalarAsync<int>(
                query,
                new
                {
                    UnitId = model.UnitId,
                    UnitName = model.UnitName,
                    IsActive = model.IsActive,
                    ModifiedBy = model.ModifiedBy,
                    CompanyId = model.CompanyId,
                    BranchId = model.BranchId
                });
        }
    }

    public async Task<bool> DeleteAsync(
        int unitId,
        int companyId,
        int branchId,
        int userId)
    {
        var query = @"
            UPDATE Units
            SET
                IsDeleted = 1,
                IsActive = 0,
                ModifiedOn = GETDATE(),
                ModifiedBy = @UserId
            WHERE UnitId = @UnitId
              AND CompanyId = @CompanyId
              AND BranchId = @BranchId
              AND ISNULL(IsDeleted, 0) = 0";

        using var connection = _context.CreateConnection();

        var result = await connection.ExecuteAsync(
            query,
            new
            {
                UnitId = unitId,
                CompanyId = companyId,
                BranchId = branchId,
                UserId = userId
            });

        return result > 0;
    }
}
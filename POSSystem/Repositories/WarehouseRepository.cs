using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;

namespace POSSystem.Repositories
{
    public class WarehouseRepository :IWarehouse
    {
        private readonly DapperContext _context;

        public WarehouseRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> AddAsync(
     Warehouse warehouse,
     int companyId,
     int branchId)
        {
            var query = @"
        DECLARE @WarehouseId INT;

        SELECT @WarehouseId = ISNULL(MAX(WarehouseId), 0) + 1
        FROM Warehouses

        INSERT INTO Warehouses
        (
            WarehouseId,
            CompanyId,
            BranchId,
            WarehouseName,
            Locations,
            IsActive,
            IsDeleted,
            CreatedOn,
            CreatedBy
        )
        VALUES
        (
            @WarehouseId,
            @CompanyId,
            @BranchId,
            @WarehouseName,
            @Locations,
            @IsActive,
            0,
            GETDATE(),
            @CreatedBy
        );

        SELECT @WarehouseId;
    ";

            using var con = _context.CreateConnection();

            return await con.ExecuteScalarAsync<int>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId,
                warehouse.WarehouseName,
                warehouse.Locations,
                warehouse.IsActive,
                warehouse.CreatedBy
            });
        }


        public async Task DeleteAsync(
            int id,
            int companyId,
            int branchId,
            int userId)
        {
            var query = @"
        UPDATE Warehouses
        SET 
            IsDeleted = 1,
            IsActive = 0,
            DeletedOn = GETDATE(),
            DeletedBy = @DeletedBy
        WHERE WarehouseId = @Id
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            await con.ExecuteAsync(query, new
            {
                Id = id,
                CompanyId = companyId,
                BranchId = branchId,
                DeletedBy = userId
            });
        }


        public async Task<IEnumerable<Warehouse>> GetAllAsync(
     int companyId,
     int branchId)
        {
            var query = @"
        SELECT 
            w.WarehouseId,
            w.CompanyId,
            w.BranchId,
            w.WarehouseName,
            b.BranchName,
            w.Locations,
            w.IsActive,
            w.IsDeleted
        FROM Warehouses w
        LEFT JOIN Branches b 
            ON w.BranchId = b.BranchId
            AND w.CompanyId = b.CompanyId
        WHERE w.CompanyId = @CompanyId
          AND w.BranchId = @BranchId
          AND w.IsActive = 1
          AND w.IsDeleted = 0
        ORDER BY w.WarehouseName";

            using var con = _context.CreateConnection();

            return await con.QueryAsync<Warehouse>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId
            });
        }


        public async Task<Warehouse?> GetByIdAsync(
            int id,
            int companyId,
            int branchId)
        {
            var query = @"
        SELECT *
        FROM Warehouses
        WHERE WarehouseId = @Id
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            return await con.QueryFirstOrDefaultAsync<Warehouse>(
                query,
                new
                {
                    Id = id,
                    CompanyId = companyId,
                    BranchId = branchId
                });
        }


        public async Task UpdateAsync(
            Warehouse warehouse,
            int companyId,
            int branchId)
        {
            var query = @"
        UPDATE Warehouses
        SET 
            WarehouseName = @WarehouseName,
            Locations = @Locations,
            IsActive = @IsActive,
            ModifiedOn = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE WarehouseId = @WarehouseId
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            await con.ExecuteAsync(query, new
            {
                warehouse.WarehouseId,
                warehouse.WarehouseName,
                warehouse.Locations,
                warehouse.IsActive,
                warehouse.ModifiedBy,
                CompanyId = companyId,
                BranchId = branchId
            });
        }
    }
}

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
        public async Task<int> AddAsync(Warehouse warehouse)
        {
            var query = @"
        INSERT INTO Warehouses
        (WarehouseName,BranchId, Locations,IsActive, IsDeleted, CreatedOn, CreatedBy)
        VALUES
        (@WarehouseName,@BranchId, @Locations,@IsActive, 0, GETDATE(), @CreatedBy);

        SELECT CAST(SCOPE_IDENTITY() as int);
         ";

            using var con = _context.CreateConnection();
            return await con.ExecuteScalarAsync<int>(query, warehouse);
        }

        public async Task DeleteAsync(int id)
        {
            var query = @"
                         UPDATE Warehouses
                         SET 
                         IsDeleted = 1,
                         IsActive  = 0
                         WHERE WarehouseId = @Id";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, new { Id = id });
        }


        public async Task<IEnumerable<Warehouse>> GetAllAsync()
        {
            var query = @"Select * from Warehouses WHERE IsActive=1 AND IsDeleted=0";
            var con = _context.CreateConnection();
            return await con.QueryAsync<Warehouse>(query);
        }

        public async Task<Warehouse?> GetByIdAsync(int id)
        {
            var query = @"
        SELECT *
        FROM Warehouses
        WHERE WarehouseId = @Id
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            return await con.QueryFirstOrDefaultAsync<Warehouse>(query, new { Id = id });
        }

        public async Task UpdateAsync(Warehouse warehouse)
        {
            var query = @"
        UPDATE Warehouses
        SET 
        WarehouseName = @WarehouseName,
        BranchId = @BranchId,
        Locations = @Locations,
        IsActive     = @IsActive,
        ModifiedOn  = GETDATE(),
        ModifiedBy  = @ModifiedBy
        WHERE WarehouseId = @WarehouseId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, warehouse);
        }
    }
}

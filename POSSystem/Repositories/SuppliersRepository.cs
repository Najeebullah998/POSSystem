using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;

namespace POSSystem.Repositories
{
    public class SuppliersRepository : ISupplier
    {
        private readonly DapperContext _context;

        public SuppliersRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Supplier supplier)
        {
            var query = @"
                INSERT INTO Suppliers
                (
                    SupplierName,
                    Phone,
                    Email,
                    IsActive,
                    IsDeleted,
                    CreatedOn,
                    CreatedBy
                )
                VALUES
                (
                    @SupplierName,
                    @Phone,
                    @Email,
                    @IsActive,
                    0,
                    GETDATE(),
                    @CreatedBy
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            using var con = _context.CreateConnection();
            return await con.ExecuteScalarAsync<int>(query, supplier);
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var query = @"
                SELECT *
                FROM Suppliers
                WHERE IsDeleted = 0
                ORDER BY SupplierName";

            using var con = _context.CreateConnection();
            return await con.QueryAsync<Supplier>(query);
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            var query = @"
                SELECT *
                FROM Suppliers
                WHERE SupplierId = @Id
                AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            return await con.QueryFirstOrDefaultAsync<Supplier>(query, new { Id = id });
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            var query = @"
                UPDATE Suppliers
                SET
                    SupplierName = @SupplierName,
                    Phone = @Phone,
                    Email = @Email,
                    IsActive = @IsActive,
                    ModifiedOn = GETDATE(),
                    ModifiedBy = @ModifiedBy
                WHERE SupplierId = @SupplierId
                AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, supplier);
        }

        public async Task DeleteAsync(int id)
        {
            var query = @"
                UPDATE Suppliers
                SET
                    IsDeleted = 1,
                    IsActive = 0,
                    ModifiedOn = GETDATE()
                WHERE SupplierId = @Id";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, new { Id = id });
        }

        //public Task<IEnumerable<dynamic>> GetBranchesAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<dynamic>> GetRolesAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //Task<IEnumerable<dynamic>> ISupplier.GetAllAsync()
        //{
        //    throw new NotImplementedException();
        //}
    }
}

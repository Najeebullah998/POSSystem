using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;

namespace POSSystem.Repositories
{
    public class CustomerRepository : ICustomer
    {
        private readonly DapperContext _context;

        public CustomerRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> AddAsync(Customers customers)
        {
            var query = @"
        INSERT INTO Customers
        (CustomerName,Phone, IsActive, IsDeleted, CreatedOn, CreatedBy)
        VALUES
        (@CustomerName,@Phone, @IsActive, 0, GETDATE(), @CreatedBy);

        SELECT CAST(SCOPE_IDENTITY() as int);
         ";

            using var con = _context.CreateConnection();
            return await con.ExecuteScalarAsync<int>(query, customers);
        }

        public async Task DeleteAsync(int id)
        {
            var query = @"
                         UPDATE Customers
                         SET 
                         IsDeleted = 1,
                         IsActive  = 0
                         WHERE CustomerId = @Id";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, new { Id = id });
        }


        public async Task<IEnumerable<Customers>> GetAllAsync()
        {
            var query = @"Select * from Customers WHERE IsActive=1 AND IsDeleted=0";
            var con = _context.CreateConnection();
            return await con.QueryAsync<Customers>(query);
        }

        public async Task<Customers?> GetByIdAsync(int id)
        {
            var query = @"
        SELECT *
        FROM Customers
        WHERE CustomerId = @Id
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            return await con.QueryFirstOrDefaultAsync<Customers>(query, new { Id = id });
        }

        public async Task UpdateAsync(Customers customers)
        {
            var query = @"
        UPDATE Customers
        SET 
        CustomerName = @CustomerName,
        Phone = @Phone,
        IsActive     = @IsActive,
        ModifiedOn  = GETDATE(),
        ModifiedBy  = @ModifiedBy
        WHERE CustomerId = @CustomerId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, customers);
        }
    }
}

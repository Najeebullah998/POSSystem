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
        public async Task<int> AddAsync(
      Customers customers,
      int companyId,
      int branchId)
        {
            var query = @"
        DECLARE @CustomerId INT;

        SELECT @CustomerId = ISNULL(MAX(CustomerId), 0) + 1
        FROM Customers;

        INSERT INTO Customers
        (
            CustomerId,
            CompanyId,
            BranchId,
            CustomerName,
            Phone,
            IsActive,
            IsDeleted,
            CreatedOn,
            CreatedBy
        )
        VALUES
        (
            @CustomerId,
            @CompanyId,
            @BranchId,
            @CustomerName,
            @Phone,
            @IsActive,
            0,
            GETDATE(),
            @CreatedBy
        );

        SELECT @CustomerId;
    ";

            using var con = _context.CreateConnection();

            return await con.ExecuteScalarAsync<int>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId,
                customers.CustomerName,
                customers.Phone,
                customers.IsActive,
                customers.CreatedBy
            });
        }


        public async Task DeleteAsync(
            int id,
            int companyId,
            int branchId,
            int userId)
        {
            var query = @"
        UPDATE Customers
        SET
            IsDeleted = 1,
            IsActive = 0,
            ModifiedOn = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE CustomerId = @Id
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


        public async Task<IEnumerable<Customers>> GetAllAsync(
            int companyId,
            int branchId)
        {
            var query = @"
        SELECT *
        FROM Customers
        WHERE CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsActive = 1
          AND IsDeleted = 0
        ORDER BY CustomerName";

            using var con = _context.CreateConnection();

            return await con.QueryAsync<Customers>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId
            });
        }


        public async Task<Customers?> GetByIdAsync(
            int id,
            int companyId,
            int branchId)
        {
            var query = @"
        SELECT *
        FROM Customers
        WHERE CustomerId = @Id
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            return await con.QueryFirstOrDefaultAsync<Customers>(
                query,
                new
                {
                    Id = id,
                    CompanyId = companyId,
                    BranchId = branchId
                });
        }


        public async Task UpdateAsync(
            Customers customers,
            int companyId,
            int branchId)
        {
            var query = @"
        UPDATE Customers
        SET
            CustomerName = @CustomerName,
            Phone = @Phone,
            IsActive = @IsActive,
            ModifiedOn = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE CustomerId = @CustomerId
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            await con.ExecuteAsync(query, new
            {
                customers.CustomerId,
                customers.CustomerName,
                customers.Phone,
                customers.IsActive,
                customers.ModifiedBy,
                CompanyId = companyId,
                BranchId = branchId
            });
        }
    }
}

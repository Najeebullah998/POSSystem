using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;

namespace POSSystem.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;

        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Company company)
        {
            var query = @"
    INSERT INTO Companies
    (
        CompanyName,
        OwnerName,
        Phone,
        Email,
        Website,
        Address,
        City,
        Country,
        TaxNumber,
        RegistrationNumber,
        LogoPath,
        IsActive,
        IsDeleted,
        CreatedAt
    )
    VALUES
    (
        @CompanyName,
        @OwnerName,
        @Phone,
        @Email,
        @Website,
        @Address,
        @City,
        @Country,
        @TaxNumber,
        @RegistrationNumber,
        @LogoPath,
        @IsActive,0,
        GETDATE()
    );

    SELECT CAST(SCOPE_IDENTITY() as int);
    ";

            using var con = _context.CreateConnection();
            return await con.ExecuteScalarAsync<int>(query, company);
        }


        public async Task DeleteAsync(int id)
        {
            var query = @"
                         UPDATE Companies
                         SET 
                         IsDeleted = 1,
                         IsActive  = 0
                         WHERE CompanyId = @Id";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, new { Id = id });
        }


        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            var query = @"Select * from Companies WHERE IsActive=1 AND IsDeleted=0";
            var con = _context.CreateConnection();
            return await con.QueryAsync<Company> (query);
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            var query = @"
        SELECT *
        FROM Companies
        WHERE CompanyId = @Id
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            return await con.QueryFirstOrDefaultAsync<Company> (query, new { Id = id });
        }

        public async Task UpdateAsync(Company company)
        {
            var query = @"
    UPDATE Companies
    SET 
        CompanyName = @CompanyName,
        OwnerName = @OwnerName,
        Phone = @Phone,
        Email = @Email,
        Website = @Website,
        Address = @Address,
        City = @City,
        Country = @Country,
        TaxNumber = @TaxNumber,
        RegistrationNumber = @RegistrationNumber,
        LogoPath = @LogoPath,
        IsActive = @IsActive,
        UpdatedAt = GETDATE()
    WHERE CompanyId = @CompanyId
    AND IsDeleted = 0;
    ";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, company);
        }


    }
}

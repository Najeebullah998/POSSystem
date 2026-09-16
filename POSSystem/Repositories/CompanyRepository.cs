using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IEnumerable<SelectListItem>> GetBusinessTypeDropdownAsync()
        {
            var query = @"
        SELECT 
            BusinessTypeId AS Value,
            BusinessName AS Text
        FROM BusinessType
        WHERE IsActive = 1
        ORDER BY BusinessName";

            using var con = _context.CreateConnection();

            var result = await con.QueryAsync<SelectListItem>(query);

            return result;
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
    CreatedAt,
    BusinessTypeId
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
    @IsActive,
    0,
    GETDATE(),
    @BusinessTypeId
);

SELECT CAST(SCOPE_IDENTITY() AS int);
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
            IsActive = 0
        WHERE CompanyId = @Id";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, new { Id = id });
        }


        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            var query = @"
        SELECT *
        FROM Companies
        WHERE IsActive = 1
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            return await con.QueryAsync<Company>(query);
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            var query = @"
        SELECT *
        FROM Companies
        WHERE CompanyId = @Id
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            return await con.QueryFirstOrDefaultAsync<Company>(
                query,
                new { Id = id }
            );
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
    BusinessTypeId = @BusinessTypeId,
    UpdatedAt = GETDATE()
WHERE CompanyId = @CompanyId
  AND IsDeleted = 0;
";

            using var con = _context.CreateConnection();

            await con.ExecuteAsync(query, company);
        }


    }
}

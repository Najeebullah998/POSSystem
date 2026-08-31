using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;

namespace POSSystem.Repositories
{
    public class CategoryRepository : ICategory
    {
        private readonly DapperContext _context;

        public CategoryRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> AddAsync(ItemCategory category, int companyId, int branchId)
        {
            var query = @"
        INSERT INTO ItemCategories
        (
            CompanyId,
            BranchId,
            CategoryName,
            IsActive,
            IsDeleted,
            CreatedOn,
            CreatedBy
        )
        VALUES
        (
            @CompanyId,
            @BranchId,
            @CategoryName,
            @IsActive,
            0,
            GETDATE(),
            @CreatedBy
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

            using var con = _context.CreateConnection();

            return await con.ExecuteScalarAsync<int>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId,
                category.CategoryName,
                category.IsActive,
                category.CreatedBy
            });
        }

        public async Task DeleteAsync(int id, int companyId, int branchId, int userId)
        {
            var query = @"
        UPDATE ItemCategories
        SET 
            IsDeleted = 1,
            IsActive = 0,
            DeletedOn = GETDATE(),
            DeletedBy = @DeletedBy
        WHERE CategoryId = @Id
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId";

            using var con = _context.CreateConnection();

            await con.ExecuteAsync(query, new
            {
                Id = id,
                CompanyId = companyId,
                BranchId = branchId,
                DeletedBy = userId
            });
        }
        public async Task<IEnumerable<ItemCategory>> GetAllAsync(int companyId, int branchId)
        {
            var query = @"
        SELECT *
        FROM ItemCategories
        WHERE CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsActive = 1
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            return await con.QueryAsync<ItemCategory>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId
            });
        }
        public async Task<ItemCategory?> GetByIdAsync(int id, int companyId, int branchId)
        {
            var query = @"
        SELECT *
        FROM ItemCategories
        WHERE CategoryId = @Id
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            return await con.QueryFirstOrDefaultAsync<ItemCategory>(query, new
            {
                Id = id,
                CompanyId = companyId,
                BranchId = branchId
            });
        }
        public async Task UpdateAsync(ItemCategory category, int companyId, int branchId)
        {
            var query = @"
        UPDATE ItemCategories
        SET 
            CategoryName = @CategoryName,
            IsActive = @IsActive,
            ModifiedOn = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE CategoryId = @CategoryId
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            await con.ExecuteAsync(query, new
            {
                category.CategoryId,
                category.CategoryName,
                category.IsActive,
                category.ModifiedBy,
                CompanyId = companyId,
                BranchId = branchId
            });
        }
    }
}

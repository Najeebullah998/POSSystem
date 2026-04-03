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
        public async Task<int> AddAsync(ItemCategory category)
        {
            var query = @"
        INSERT INTO ItemCategories
        (CategoryName, IsActive, IsDeleted, CreatedOn, CreatedBy)
        VALUES
        (@CategoryName, @IsActive, 0, GETDATE(), @CreatedBy);

        SELECT CAST(SCOPE_IDENTITY() as int);
         ";

            using var con = _context.CreateConnection();
            return await con.ExecuteScalarAsync<int>(query, category);
        }

        public async Task DeleteAsync(int id)
        {
            var query = @"
                         UPDATE ItemCategories
                         SET 
                         IsDeleted = 1,
                         IsActive  = 0
                         WHERE CategoryId = @Id";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, new { Id = id });
        }


        public async Task<IEnumerable<ItemCategory>> GetAllAsync()
        {
            var query = @"Select * from ItemCategories WHERE IsActive=1 AND IsDeleted=0";
            var con = _context.CreateConnection();
            return await con.QueryAsync<ItemCategory>(query);
        }

        public async Task<ItemCategory?> GetByIdAsync(int id)
        {
            var query = @"
        SELECT *
        FROM ItemCategories
        WHERE CategoryId = @Id
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            return await con.QueryFirstOrDefaultAsync<ItemCategory>(query, new { Id = id });
        }

        public async Task UpdateAsync(ItemCategory category)
        {
            var query = @"
        UPDATE ItemCategories
        SET 
        CategoryName = @CategoryName,
        IsActive     = @IsActive,
        ModifiedOn  = GETDATE(),
        ModifiedBy  = @ModifiedBy
        WHERE CategoryId = @CategoryId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, category);
        }
    }
}

using Dapper;
using POSSystem.DATA;
using POSSystem.Entities;
using POSSystem.Interfaces;

namespace POSSystem.Repositories
{
    public class ItemsRepository :Iitems
    {
        private readonly DapperContext _context;

        public ItemsRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<int> AddAsync(Item item)
        {
            var query = @"
                         INSERT INTO Items
                         (Barcode, ItemName, CategoryId, UnitId, SalePrice, CostPrice, IsActive, IsDeleted, CreatedOn, CreatedBy)
                         VALUES
                         (@Barcode, @ItemName, @CategoryId, @UnitId, @SalePrice, @CostPrice, @IsActive, 0, GETDATE(), @CreatedBy);
                         
                         SELECT CAST(SCOPE_IDENTITY() as int);
                         ";

            using var con = _context.CreateConnection();
            return await con.ExecuteScalarAsync<int>(query, item);
        }

        public async Task DeleteAsync(int id)
        {
            var query = @"
                         UPDATE Items
                         SET 
                         IsDeleted = 1,
                         IsActive  = 0
                         WHERE ItemId = @Id";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, new { Id = id });
        }


        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            var query = @"Select * from Items WHERE IsActive=1 AND IsDeleted=0";
            var con = _context.CreateConnection();
            return await con.QueryAsync<Item>(query);
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            var query = @"
        SELECT *
        FROM Items
        WHERE ItemId = @Id
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            return await con.QueryFirstOrDefaultAsync<Item>(query, new { Id = id });
        }

        public async Task UpdateAsync(Item item)
        {
            var query = @"
                UPDATE Items 
                SET 
                    Barcode = @Barcode,
                    ItemName = @ItemName,
                    CategoryId = @CategoryId,
                    UnitId = @UnitId,
                    SalePrice = @SalePrice,
                    CostPrice = @CostPrice,
                    IsActive = @IsActive,
                    ModifiedOn = GETDATE(),
                    ModifiedBy = @ModifiedBy
                WHERE 
                    ItemId = @ItemId 
                    AND IsDeleted = 0";

            using var con = _context.CreateConnection();
            await con.ExecuteAsync(query, item);
        }
    }
}

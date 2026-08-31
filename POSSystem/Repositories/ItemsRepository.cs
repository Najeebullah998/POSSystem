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
        public async Task<int> AddAsync(Item item,int companyId,int branchId)
        {
            var query = @"
        DECLARE @ItemId INT;

        SELECT @ItemId = ISNULL(MAX(ItemId), 0) + 1
        FROM Items;

        INSERT INTO Items
        (
            ItemId,
            CompanyId,
            BranchId,
            Barcode,
            ItemName,
            CategoryId,
            UnitId,
            SalePrice,
            CostPrice,
            IsActive,
            IsDeleted,
            CreatedOn,
            CreatedBy
        )
        VALUES
        (
            @ItemId,
            @CompanyId,
            @BranchId,
            @Barcode,
            @ItemName,
            @CategoryId,
            @UnitId,
            @SalePrice,
            @CostPrice,
            @IsActive,
            0,
            GETDATE(),
            @CreatedBy
        );

        SELECT @ItemId;
    ";

            using var con = _context.CreateConnection();

            return await con.ExecuteScalarAsync<int>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId,
                item.Barcode,
                item.ItemName,
                item.CategoryId,
                item.UnitId,
                item.SalePrice,
                item.CostPrice,
                item.IsActive,
                item.CreatedBy
            });
        }


        public async Task DeleteAsync(
            int id,
            int companyId,
            int branchId,
            int userId)
        {
            var query = @"
        UPDATE Items
        SET
            IsDeleted = 1,
            IsActive = 0,
            ModifiedOn = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE ItemId = @Id
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


        public async Task<IEnumerable<Item>> GetAllAsync(
            int companyId,
            int branchId)
        {
            var query = @"
        SELECT
            i.ItemId,
            i.CompanyId,
            i.BranchId,
            i.ItemName,
            i.Barcode,
            i.CategoryId,
            c.CategoryName,
            i.UnitId,
            u.UnitName,
            i.SalePrice,
            i.CostPrice,
            i.IsActive,
            c.CategoryName,
            u.UnitName
        FROM Items i

        LEFT JOIN ItemCategories c
            ON i.CategoryId = c.CategoryId
            AND i.CompanyId = c.CompanyId

        LEFT JOIN Units u
            ON i.UnitId = u.UnitId
            AND i.CompanyId = u.CompanyId

        WHERE i.CompanyId = @CompanyId
          AND i.BranchId = @BranchId
          AND i.IsDeleted = 0

        ORDER BY i.ItemName";

            using var con = _context.CreateConnection();

            return await con.QueryAsync<Item>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId
            });
        }


        public async Task<Item?> GetByIdAsync(
            int id,
            int companyId,
            int branchId)
        {
            var query = @"
        SELECT *
        FROM Items
        WHERE ItemId = @Id
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            return await con.QueryFirstOrDefaultAsync<Item>(
                query,
                new
                {
                    Id = id,
                    CompanyId = companyId,
                    BranchId = branchId
                });
        }


        public async Task UpdateAsync(
            Item item,
            int companyId,
            int branchId)
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
        WHERE ItemId = @ItemId
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            await con.ExecuteAsync(query, new
            {
                item.ItemId,
                item.Barcode,
                item.ItemName,
                item.CategoryId,
                item.UnitId,
                item.SalePrice,
                item.CostPrice,
                item.IsActive,
                item.ModifiedBy,
                CompanyId = companyId,
                BranchId = branchId
            });
        }
    }
}

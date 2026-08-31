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

        public async Task<int> AddAsync(Supplier supplier,int companyId,int branchId)
        {
            var query = @"
        DECLARE @SupplierId INT;

        SELECT @SupplierId = ISNULL(MAX(SupplierId), 0) + 1
        FROM Suppliers;

        INSERT INTO Suppliers
        (
            SupplierId,
            CompanyId,
            BranchId,
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
            @SupplierId,
            @CompanyId,
            @BranchId,
            @SupplierName,
            @Phone,
            @Email,
            @IsActive,
            0,
            GETDATE(),
            @CreatedBy
        );

        SELECT @SupplierId;
    ";

            using var con = _context.CreateConnection();

            return await con.ExecuteScalarAsync<int>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId,
                supplier.SupplierName,
                supplier.Phone,
                supplier.Email,
                supplier.IsActive,
                supplier.CreatedBy
            });
        }


        public async Task<IEnumerable<Supplier>> GetAllAsync(int companyId, int branchId)
        {
            var query = @"
        SELECT *
        FROM Suppliers
        WHERE CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0
        ORDER BY SupplierName";

            using var con = _context.CreateConnection();

            return await con.QueryAsync<Supplier>(query, new
            {
                CompanyId = companyId,
                BranchId = branchId
            });
        }


        public async Task<Supplier?> GetByIdAsync(int id,int companyId,int branchId)
        {
            var query = @"
        SELECT *
        FROM Suppliers
        WHERE SupplierId = @Id
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            return await con.QueryFirstOrDefaultAsync<Supplier>(query, new
            {
                Id = id,
                CompanyId = companyId,
                BranchId = branchId
            });
        }


        public async Task UpdateAsync(Supplier supplier,int companyId,int branchId)
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
          AND CompanyId = @CompanyId
          AND BranchId = @BranchId
          AND IsDeleted = 0";

            using var con = _context.CreateConnection();

            await con.ExecuteAsync(query, new
            {
                supplier.SupplierId,
                supplier.SupplierName,
                supplier.Phone,
                supplier.Email,
                supplier.IsActive,
                supplier.ModifiedBy,
                CompanyId = companyId,
                BranchId = branchId
            });
        }


        public async Task DeleteAsync(int id,int companyId,int branchId,int userId)
        {
            var query = @"
        UPDATE Suppliers
        SET
            IsDeleted = 1,
            IsActive = 0,
            DeletedOn = GETDATE(),
            DeletedBy = @DeletedBy
        WHERE SupplierId = @Id
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

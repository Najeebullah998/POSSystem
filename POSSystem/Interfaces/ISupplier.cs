using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface ISupplier
    {
        Task<int> AddAsync(Supplier supplier, int companyId, int branchId);

        Task UpdateAsync(Supplier supplier, int companyId, int branchId);

        Task DeleteAsync(int id, int companyId, int branchId, int userId);

        Task<Supplier?> GetByIdAsync(int id, int companyId, int branchId);

        Task<IEnumerable<Supplier>> GetAllAsync(int companyId, int branchId);
    }
}
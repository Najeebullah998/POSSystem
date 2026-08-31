using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IWarehouse
    {
        Task<int> AddAsync(Warehouse warehouse, int companyId, int branchId);

        Task UpdateAsync(Warehouse warehouse, int companyId, int branchId);

        Task DeleteAsync(int id, int companyId, int branchId, int userId);

        Task<Warehouse?> GetByIdAsync(int id, int companyId, int branchId);

        Task<IEnumerable<Warehouse>> GetAllAsync(int companyId, int branchId);
    }
}

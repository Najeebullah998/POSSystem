using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface ICategory
    {
        Task<int> AddAsync(ItemCategory category, int companyId, int branchId);

        Task UpdateAsync(ItemCategory category, int companyId, int branchId);
        Task DeleteAsync(int id, int companyId, int branchId, int userId);
        Task<ItemCategory?> GetByIdAsync(int id, int companyId, int branchId);
        Task<IEnumerable<ItemCategory>> GetAllAsync(int companyId, int branchId);
    }
}

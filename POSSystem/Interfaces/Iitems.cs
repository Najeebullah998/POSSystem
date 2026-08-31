using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface Iitems
    {
        Task<int> AddAsync(Item item, int companyId, int branchId);

        Task UpdateAsync(Item item, int companyId, int branchId);

        Task DeleteAsync(int id, int companyId, int branchId, int userId);

        Task<Item?> GetByIdAsync(int id, int companyId, int branchId);

        Task<IEnumerable<Item>> GetAllAsync(int companyId, int branchId);
    }
}

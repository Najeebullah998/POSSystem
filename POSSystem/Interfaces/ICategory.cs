using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface ICategory
    {
        Task<int> AddAsync(ItemCategory category);

        Task UpdateAsync(ItemCategory category);

        Task DeleteAsync(int id);

        Task<ItemCategory?> GetByIdAsync(int id);

        Task<IEnumerable<ItemCategory>> GetAllAsync();
    }
}

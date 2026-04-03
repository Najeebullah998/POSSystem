using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface Iitems
    {
        Task<int> AddAsync(Item item);

        Task UpdateAsync(Item item);

        Task DeleteAsync(int id);

        Task<Item?> GetByIdAsync(int id);

        Task<IEnumerable<Item>> GetAllAsync();
    }
}

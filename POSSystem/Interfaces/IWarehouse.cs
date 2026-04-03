using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IWarehouse
    {
        Task<int> AddAsync(Warehouse warehouse);

        Task UpdateAsync(Warehouse warehouse);

        Task DeleteAsync(int id);

        Task<Warehouse?> GetByIdAsync(int id);

        Task<IEnumerable<Warehouse>> GetAllAsync();
    }
}

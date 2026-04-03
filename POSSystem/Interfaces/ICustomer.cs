using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface ICustomer
    {
        Task<int> AddAsync(Customers customers);

        Task UpdateAsync(Customers customers);

        Task DeleteAsync(int id);

        Task<Customers?> GetByIdAsync(int id);

        Task<IEnumerable<Customers>> GetAllAsync();
    }
}

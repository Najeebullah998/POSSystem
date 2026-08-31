using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface ICustomer
    {
        Task<int> AddAsync(Customers customers, int companyId, int branchId);

        Task UpdateAsync(Customers customers, int companyId, int branchId);

        Task DeleteAsync(int id, int companyId, int branchId, int userId);

        Task<Customers?> GetByIdAsync(int id, int companyId, int branchId);

        Task<IEnumerable<Customers>> GetAllAsync(int companyId, int branchId);
    }
}

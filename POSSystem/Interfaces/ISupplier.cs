using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface ISupplier  
    {
        Task<int> AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(int id);
        Task<Supplier?> GetByIdAsync(int id);
        Task<IEnumerable<Supplier>> GetAllAsync();

        //Task<IEnumerable<dynamic>> GetRolesAsync();
        //Task<IEnumerable<dynamic>> GetBranchesAsync();
    }
}
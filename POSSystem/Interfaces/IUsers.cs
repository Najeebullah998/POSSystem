using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IUsers   // ✅ MUST be public
    {
        Task<int> AddAsync(Users user);
        Task UpdateAsync(Users user);
        Task DeleteAsync(int id);
        Task<Users?> GetByIdAsync(int id);
        Task<IEnumerable<dynamic>> GetAllAsync();

        Task<IEnumerable<dynamic>> GetRolesAsync();
        Task<IEnumerable<dynamic>> GetBranchesAsync();
    }
}
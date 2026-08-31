using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IUsers
    {
        Task<int> AddAsync(Users user, int companyId, int branchId);

        Task UpdateAsync(Users user, int companyId, int branchId);

        Task DeleteAsync(int id, int companyId, int branchId, int userId);

        Task<Users?> GetByIdAsync(int id, int companyId, int branchId);

        Task<IEnumerable<dynamic>> GetAllAsync(int companyId, int branchId);

        Task<IEnumerable<dynamic>> GetRolesAsync();

        Task<IEnumerable<dynamic>> GetBranchesAsync(int companyId);
    }
}
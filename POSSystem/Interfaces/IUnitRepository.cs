using POSSystem.Entities;

namespace POSSystem.Interfaces
{
    public interface IUnitRepository
    {
        Task<IEnumerable<UnitViewModel>> GetAllAsync(int companyId, int branchId);

        Task<UnitViewModel> GetByIdAsync(
            int unitId,
            int companyId,
            int branchId);

        Task<int> SaveAsync(UnitViewModel model);

        Task<bool> DeleteAsync(
            int unitId,
            int companyId,
            int branchId,
            int userId);
    }
}

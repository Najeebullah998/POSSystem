
using POSSystem.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace POSSystem.Interfaces
{
    public interface ICompanyRepository
    {
        Task<int> AddAsync(Company company);

        Task UpdateAsync(Company company);

        Task DeleteAsync(int id);

        Task<Company?> GetByIdAsync(int id);

        Task<IEnumerable<Company>> GetAllAsync();
    }

}

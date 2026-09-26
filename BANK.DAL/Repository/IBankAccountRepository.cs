using BANK.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK.DAL.Repository
{
    public interface IBankAccountRepository
    {
        Task<IEnumerable<BankAccount>> GetAllAsync();

        Task<BankAccount?> GetByIdAsync(int id);

        Task<BankAccount> CreateAsync(BankAccount account);

        Task UpdateAsync(BankAccount account);

        Task DeleteAsync(BankAccount account);
    }
}

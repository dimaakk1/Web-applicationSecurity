using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BANK.DAL.Entity;

namespace BANK.BLL.Services
{
    public interface IBankAccountService
    {
        Task<IEnumerable<BankAccount>> GetAllAsync();

        Task<BankAccount?> GetByIdAsync(int id);

        Task<BankAccount> CreateAsync(BankAccount account);

        Task<bool> UpdateAsync(int id, BankAccount updatedAccount);

        Task<bool> DeleteAsync(int id);
    }
}

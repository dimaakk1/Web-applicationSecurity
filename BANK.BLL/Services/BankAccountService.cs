using BANK.DAL.Entity;
using BANK.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK.BLL.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _repository;

        public BankAccountService(IBankAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BankAccount>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<BankAccount?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<BankAccount> CreateAsync(BankAccount account)
        {
            if (account.Balance < 0)
            {
                throw new ArgumentException(
                    "Balance cannot be negative.");
            }

            return await _repository.CreateAsync(account);
        }

        public async Task<bool> UpdateAsync(
            int id,
            BankAccount updatedAccount)
        {
            var account = await _repository.GetByIdAsync(id);

            if (account == null)
            {
                return false;
            }

            if (updatedAccount.Balance < 0)
            {
                throw new ArgumentException(
                    "Balance cannot be negative.");
            }

            account.OwnerName = updatedAccount.OwnerName;
            account.Balance = updatedAccount.Balance;

            await _repository.UpdateAsync(account);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var account = await _repository.GetByIdAsync(id);

            if (account == null)
            {
                return false;
            }

            await _repository.DeleteAsync(account);

            return true;
        }
    }
}

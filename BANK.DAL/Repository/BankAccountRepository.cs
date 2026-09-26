using BANK.DAL.Data;
using BANK.DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK.DAL.Repository
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly AppDbContext _context;

        public BankAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BankAccount>> GetAllAsync()
        {
            return await _context.BankAccounts.ToListAsync();
        }

        public async Task<BankAccount?> GetByIdAsync(int id)
        {
            return await _context.BankAccounts
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BankAccount> CreateAsync(BankAccount account)
        {
            _context.BankAccounts.Add(account);

            await _context.SaveChangesAsync();

            return account;
        }

        public async Task UpdateAsync(BankAccount account)
        {
            _context.BankAccounts.Update(account);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BankAccount account)
        {
            _context.BankAccounts.Remove(account);

            await _context.SaveChangesAsync();
        }
    }
}

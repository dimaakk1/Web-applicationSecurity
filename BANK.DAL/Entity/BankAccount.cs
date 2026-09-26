using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANK.DAL.Entity
{
    public class BankAccount
    {
        public int Id { get; set; }

        public string OwnerName { get; set; } = string.Empty;

        public decimal Balance { get; set; }
    }
}

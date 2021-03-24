using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Wallet.Data.Repositories
{
    public class AccountRepository : GenericRepository<Accounts>, IAccountRepository
    {
        public AccountRepository(WALLETContext context) : base(context)
        {
        }

        public double GetAccountBalance(int id, string currency)
        {
            var balance = _context.AccountBalance.FromSqlRaw($"EXEC SP_GetBalance {id}, {currency}").ToList();
            return balance[0].Balance;
        }

        public List<Accounts> GetUserAccounts(int id)
        {
            return _context.Accounts.Where(x => x.UserId == id).ToList();
        }

        public int GetAccountId(int id_user, string currency)
        {
            Accounts account = _context.Accounts.FirstOrDefault(e => e.UserId == id_user && e.Currency == currency);
            return account.Id;
        }
    }
}

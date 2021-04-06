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

        public List<Accounts> GetUserAccounts(int id)
        {
            return _context.Accounts.Where(x => x.UserId == id).ToList();
        }

        public int GetAccountId(int id_user, string currency)
        {
            Accounts account = _context.Accounts.FirstOrDefault(e => e.UserId == id_user && e.Currency == currency);
            return account.Id;
        }

        public Accounts GetAccountById(int id)
        {
            return _context.Accounts.FirstOrDefault(x => x.Id == id);
        }
    }
}

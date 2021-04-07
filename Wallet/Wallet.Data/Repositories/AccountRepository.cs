using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Wallet.Entities;

namespace Wallet.Data.Repositories
{
    public class AccountRepository : GenericRepository<Accounts>, IAccountRepository
    {
        public AccountRepository(WALLETContext context) : base(context)
        {
        }

        public double GetAccountBalance(int user_id, string currency)
        {
            var balance = _context.AccountBalance.FromSqlRaw($"EXEC SP_GetBalance {user_id}, {currency}").ToList();
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

        public Accounts GetAccountById(int id)
        {
            return _context.Accounts.FirstOrDefault(x => x.Id == id);
        }

        public AccountsUsersModel GetAccountsUsers(int user_id)
        {
            AccountsUsersModel accounts = new AccountsUsersModel();
            accounts.IdUSD = GetAccountId(user_id, "USD");
            accounts.IdARS= GetAccountId(user_id, "ARS");
            return accounts;
        }

        public bool ValidateAccounts(AccountsUsersModel accounts)
        {
            //return false if any of the data is wrong
            if(accounts.IdUSD > 0 && accounts.IdUSD != null && accounts.IdARS > 0 && accounts.IdARS != null) { return true; }
            else { return false; }
        }
    }
}

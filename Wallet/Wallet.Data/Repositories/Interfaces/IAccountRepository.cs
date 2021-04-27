using System.Collections.Generic;
using Wallet.Data.Models;
using Wallet.Entities;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Accounts>
    {
        List<Accounts> GetUserAccounts(int id);
        int GetAccountId(int id_user, string currency);
        Accounts GetAccountById(int id);
        AccountsUserModel GetAccountsUsers(int user_id);
        bool InvalidAccounts(AccountsUserModel accounts);
    }
}

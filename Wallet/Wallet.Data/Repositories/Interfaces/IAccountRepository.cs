using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Accounts>
    {
        List<Accounts> GetUserAccounts(int id);
        int GetAccountId(int id_user, string currency);
        Accounts GetAccountById(int id);
    }
}

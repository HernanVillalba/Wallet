using System.Collections.Generic;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public interface IAccountBusiness
    {
        BalanceModel GetBalances(int id);
        List<AccountModel> GetAccountsWithBalance(int id);
        double GetAccountBalance(int id, string currency);
    }
}

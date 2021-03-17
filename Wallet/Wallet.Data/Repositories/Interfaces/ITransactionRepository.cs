using System.Collections.Generic;
using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transactions>
    {
        IEnumerable<Transactions> SP_ListTransactions(string stored_procedure, int user_id);
    }
}

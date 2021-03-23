using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transactions>
    {
        IEnumerable<Transactions> SP_GetTransactionsUser(string stored_procedure, int user_id);
        Transactions FindTransaction(int id_transaction, int USD_account_id, int ARS_account_id);
        IEnumerable<Transactions> FilterTransaction(Transactions transaction);
        Task<IEnumerable<Transactions>> GetTransactionsUser(int ARS_id, int USD_id);
    }
}

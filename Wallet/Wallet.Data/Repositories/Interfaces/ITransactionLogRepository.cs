using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface ITransactionLogRepository : IGenericRepository<TransactionLog>
    {
        Task<List<TransactionLog>> GetByTransactionId(int transaction_id);
    }
}

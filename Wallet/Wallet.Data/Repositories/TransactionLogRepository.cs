using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class TransactionLogRepository : GenericRepository<TransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(WALLETContext context) : base(context)
        {

        }

        public Task<List<TransactionLog>> GetByTransactionId(int transaction_id)
        {
            var list = _context.TransactionLog.Where(e => e.TransactionId == transaction_id).ToListAsync();
            return list;
        }
    }
}

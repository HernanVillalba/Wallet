using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class TrasactionRepository : GenericRepository<Transactions>, ITransactionRepository
    {
        public TrasactionRepository(WALLETContext context) : base(context)
        {

        }

        public IEnumerable<Transactions> SP_ListTransactions(string stored_procedure, int user_id)
        {
            return _context.Transactions.FromSqlRaw("EXEC " + stored_procedure, user_id);
        }
    }
}

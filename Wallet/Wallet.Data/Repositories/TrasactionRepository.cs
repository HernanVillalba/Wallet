using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Transactions FilterTransaction(Transactions transaction)
        {
            return _context.Transactions.Find(transaction);
        }

        public Transactions FindTransaction(int id_transaction, int USD_account_id, int ARS_account_id)
        {
            return _context.Transactions
                .FirstOrDefault(e=>e.Id == id_transaction && (e.AccountId==ARS_account_id || e.AccountId==USD_account_id));
        }

        public IEnumerable<Transactions> SP_ListTransactionByUser(string stored_procedure, int user_id)
        {
            return _context.Transactions.FromSqlRaw("EXEC " + stored_procedure, user_id);

        }

        public IEnumerable<Transactions> SP_TrasactionsByUser(string stored_procedure, int user_id)
        public IEnumerable<Transactions> SP_GetTransactionsUser(string stored_procedure, int user_id)
        {
            return _context.Transactions.FromSqlRaw("EXEC " + stored_procedure, user_id);
        }
    }
}

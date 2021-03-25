using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Data.Repositories
{
    public class TrasactionRepository : GenericRepository<Transactions>, ITransactionRepository
    {
        public TrasactionRepository(WALLETContext context) : base(context)
        {

        }

        public IEnumerable<Transactions> FilterTransaction(TransactionFilterModel t)
        {
            IEnumerable<Transactions> list =
                _context.Transactions
                .Where
                (e => (e.AccountId == t.ARS_id || e.AccountId == t.USD_id) && (e.Concept.ToLower().Contains(t.Concept.ToLower()) || e.Type.ToLower().Contains(t.Type.ToLower()) ))
                .OrderByDescending(e => e.Date);
            return list;
        }

        public Transactions FindTransaction(int id_transaction, int USD_account_id, int ARS_account_id)
        {
            return _context.Transactions
                .FirstOrDefault(e => e.Id == id_transaction && (e.AccountId == ARS_account_id || e.AccountId == USD_account_id));
        }


        public IEnumerable<Transactions> SP_GetTransactionsUser(string stored_procedure, int user_id)
        {
            return _context.Transactions.FromSqlRaw("EXEC " + stored_procedure, user_id);
        }
        public async Task<IEnumerable<Transactions>> GetTransactionsUser(int ARS_id, int USD_id)
        {
            //los id recibidos son de la account del user
            return await _context.Transactions
                   .Where(e => e.AccountId == ARS_id || e.AccountId == USD_id)
                   .OrderByDescending(e => e.Date).ToListAsync();
        }
    }
}

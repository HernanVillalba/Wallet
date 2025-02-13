﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Data.Repositories
{
    public class TransactionRepository : GenericRepository<Transactions>, ITransactionRepository
    {
        public TransactionRepository(WALLETContext context) : base(context)
        {

        }

        public Task<List<Transactions>> FilterTransaction(TransactionFilterModel t, int usd_id, int ars_id)
        {
            Task<List<Transactions>> list = null;
            if (t.Concept != "" && t.Concept != null)
            {
                list =  _context.Transactions.Where(e => e.AccountId == t.AccountId && e.Concept.ToLower().Contains(t.Concept.ToLower()) ).ToListAsync();
            }
            else if (t.Type != "" && t.Type != null)
            {
                list = _context.Transactions.Where(e => e.AccountId == t.AccountId && e.Type.ToLower().Contains(t.Type.ToLower()) ).ToListAsync();
            }
            else if(t.AccountId != null)
            {
                list = _context.Transactions.Where(e => e.AccountId == t.AccountId).ToListAsync();
            }
            else
            {
                list =  _context.Transactions.Where(e => e.AccountId == usd_id || e.AccountId == ars_id).ToListAsync();
            }

            return list;
        }

        public Transactions FindTransaction(int id_transaction, int USD_account_id, int ARS_account_id)
        {
            var tra =_context.Transactions
                .Include(t => t.Category)
                .FirstOrDefault(e => e.Id == id_transaction && (e.AccountId == ARS_account_id || e.AccountId == USD_account_id));
            return tra;
        }
        public async Task<IEnumerable<Transactions>> GetTransactionsUser(int ARS_id, int USD_id)
        {
            //los id recibidos son de la account del user
            return await _context.Transactions
                   .Where(e => e.AccountId == ARS_id || e.AccountId == USD_id)
                   .OrderByDescending(e => e.Date).ToListAsync();
        }

        public IEnumerable<Transactions> GetTransactionsForAccount(int accountId)
        {
            return _context.Transactions.Where(e => e.AccountId == accountId).ToList();
        }

        public IEnumerable<Transactions> GetTransactionsByUserId(int user_id)
        {
            return _context.Transactions.Where(e=>e.Account.UserId == user_id).ToList();
        }
    }
}

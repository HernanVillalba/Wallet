using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Data.Repositories
{
    public class RefundRequestRepository : GenericRepository<RefundRequest>, IRefundRequestRepository
    {
        public RefundRequestRepository(WALLETContext context) : base(context)
        {

        }

        public IEnumerable<RefundRequest> GetAllByAccountsId(AccountsUserModel accounts)
        {
            var list = _context.RefundRequest.Where(e => e.SourceAccountId == accounts.IdARS || e.TargetAccountId == accounts.IdARS ||
                e.SourceAccountId == accounts.IdUSD || e.SourceAccountId == accounts.IdUSD)
                .ToList();

            return list;
        }

        public bool PendingRequestExist(int trasaction_id)
        {
            //check if there are any pending request
            var algo = _context.RefundRequest.FirstOrDefault(e => e.TransactionId == trasaction_id && e.Status == "Pending");
            if (algo != null) { return true; }
            else { return false; }
        }

        public bool ValidateRefundRequest(RefundRequest refund)
        {
            if (refund.TransactionId > 0 && refund.SourceAccountId > 0 && refund.TargetAccountId > 0 && refund != null) { return true; }
            else { return false; }
        }

        public RefundRequest GetByIdExtended(int id, params Expression<Func<RefundRequest, object>>[] includes)
        {
            var query = _context.Set<RefundRequest>().AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.FirstOrDefault(m => m.Id == id);
        }
    }
}

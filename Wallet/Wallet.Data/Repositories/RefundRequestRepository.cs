using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
    }
}

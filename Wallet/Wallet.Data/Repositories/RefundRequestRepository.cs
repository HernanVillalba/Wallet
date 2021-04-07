using System.Linq;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class RefundRequestRepository : GenericRepository<RefundRequest>, IRefundRequestRepository
    {
        public RefundRequestRepository(WALLETContext context) : base(context)
        {

        }

        public bool PendingRequestExist(int trasaction_id)
        {
            //check if there are any pending request
            var algo = _context.RefundRequest.FirstOrDefault(e => e.TransactionId == trasaction_id && e.Status == "Pending");
            if(algo != null) { return true; }
            else { return false; }
        }

        public bool ValidateRefundRequest(RefundRequest refund)
        {
            if(refund.TransactionId > 0 && refund.SourceAccountId > 0 && refund.TargetAccountId > 0 && refund != null) { return true; }
            else { return false; }
        }
    }
}

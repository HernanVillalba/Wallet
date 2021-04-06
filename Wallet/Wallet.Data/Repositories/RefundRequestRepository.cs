using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class RefundRequestRepository : GenericRepository<RefundRequest>, IRefundRequestRepository
    {
        public RefundRequestRepository(WALLETContext context) : base(context)
        {

        }
    }
}

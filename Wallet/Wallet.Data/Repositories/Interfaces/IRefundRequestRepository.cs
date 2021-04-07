using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IRefundRequestRepository : IGenericRepository<RefundRequest>
    {
        bool ValidateRefundRequest(RefundRequest refund);
        bool PendingRequestExist(int trasaction_id);
    }
}

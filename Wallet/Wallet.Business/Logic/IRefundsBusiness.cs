using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public interface IRefundsBusiness
    {
        Task Create(RefundRequestCreateModel refund, int? user_id);
        IEnumerable<RefundRequestModel> GetAll(int? user_id);
        Task Accept(int userId, int refundRequestId);
        RefundRequestModel Details(int refundRequestId);
    }
}

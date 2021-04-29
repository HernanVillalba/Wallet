using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public interface IRefundsBusiness
    {
        Task Create(RefundRequestCreateModel refund, int? user_id);
        IEnumerable<RefundRequestModel> GetAll(int? user_id);
        Task Accept(int userId, int refundRequestId);
        Task Cancel(int userId, int refundRequestId);
        Task Reject(int userId, int refundRequestId);
        RefundRequestModel Details(int refundRequestId);
        Task EmailStatusChange(Users sourceUser, string status, int transactionId, Users targetUser);
    }
}

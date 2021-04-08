using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public interface IRefundsBusiness
    {
        void Create(RefundRequestCreateModel refund, int? user_id);
        Task<IEnumerable<RefundRequestModel>> GetAll(int? user_id);
    }
}

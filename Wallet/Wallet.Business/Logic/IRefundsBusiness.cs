using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;

namespace Wallet.Business.Logic
{
    public interface IRefundsBusiness
    {
        Task<IEnumerable<RefundRequest>> algo();
    }
}

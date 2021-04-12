using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Entities;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IRefundRequestRepository : IGenericRepository<RefundRequest>
    {
        bool ValidateRefundRequest(RefundRequest refund);
        bool PendingRequestExist(int trasaction_id);
        IEnumerable<RefundRequest> GetAllByAccountsId(AccountsUserModel accounts);
        RefundRequest GetByIdExtended(int id, params Expression<Func<RefundRequest, object>>[] includes);
    }
}

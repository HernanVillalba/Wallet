using System.Collections.Generic;
using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IFixedTermDepositRepository : IGenericRepository<FixedTermDeposit>
    {
        IEnumerable<FixedTermDeposit> GetAllByUserId(int userId);
    }
}

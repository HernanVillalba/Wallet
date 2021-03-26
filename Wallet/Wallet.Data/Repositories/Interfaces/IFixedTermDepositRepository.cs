using System.Collections.Generic;
using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IFixedTermDepositRepository : IGenericRepository<FixedTermDeposits>
    {
        IEnumerable<FixedTermDeposits> GetAllByUserId(int userId);
    }
}

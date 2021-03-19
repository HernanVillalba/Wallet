using System.Collections.Generic;
using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IFixedTermDepositRepository : IGenericRepository<FixedTermDeposit>
    {
        IEnumerable<FixedTermDeposit> ExecuteStoredProcedure(string stored_procedure);
    }
}

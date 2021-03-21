using System.Collections.Generic;
using Wallet.Data.ModelsAPI;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IFixedTermDepositRepository : IGenericRepository<FixedTermDeposit>
    {
        IEnumerable<FixedTermDeposit> ExecuteStoredProcedure(string stored_procedure);
    }
}

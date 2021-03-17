using System;
using System.Threading.Tasks;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ITransactionRepository Transactions { get; }
        IFixedTermDepositRepository FixedTermDeposits { get; }
        Task<int> Complete();
    }
}

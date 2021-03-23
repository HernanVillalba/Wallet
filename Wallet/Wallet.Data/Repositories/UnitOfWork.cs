using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WALLETContext _context;
        public UnitOfWork(WALLETContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Transactions = new TrasactionRepository(_context);
            FixedTermDeposits = new FixedTermDepositRepository(_context);
            Accounts = new AccountRepository(_context);
        }
        public IUserRepository Users { get; set; }
        public ITransactionRepository Transactions { get; set; }
        public IFixedTermDepositRepository FixedTermDeposits { get; set; }
        public IAccountRepository Accounts { get; set; }
        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

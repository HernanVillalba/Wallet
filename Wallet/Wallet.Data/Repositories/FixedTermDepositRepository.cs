using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class FixedTermDepositRepository : GenericRepository<FixedTermDepositRepository>, IFixedTermDepositRepository
    {
        public FixedTermDepositRepository(WALLETContext context) : base(context) {}


    }
}

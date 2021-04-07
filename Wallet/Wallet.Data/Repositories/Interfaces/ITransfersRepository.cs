using Wallet.Data.Models;

namespace Wallet.Data.Repositories.Interfaces
{
    public interface ITransfersRepository : IGenericRepository<Transfers>
    {
        Transfers GetTransfer(int origin_transaction_id);
        bool ValidateTransfer(Transfers transfer);
    }
}

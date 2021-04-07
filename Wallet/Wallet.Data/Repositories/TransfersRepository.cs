using System.Linq;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Data.Repositories
{
    public class TransfersRepository :GenericRepository<Transfers>, ITransfersRepository
    {
        public TransfersRepository(WALLETContext context) :base(context)
        {

        }

        public Transfers GetTransfer(int origin_transaction_id)
        {
            return _context.Transfers.FirstOrDefault(e=>e.OriginTransactionId == origin_transaction_id);
        }

        public bool ValidateTransfer(Transfers transfer)
        {
            if (transfer != null && transfer.OriginTransactionId > 0 && transfer.DestinationTransactionId > 0) { return true; }
            else { return false; }
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public interface ITransactionBusiness
    {
        Task<IEnumerable<Transactions>> GetAll(TransactionFilterModel tfm, int user_id);
        Task<List<Transactions>> Filter(TransactionFilterModel transaction, int user_id);
        Task Create(TransactionCreateModel newT, int user_id);
        Task Edit(int? id, TransactionEditModel NewTransaction, int user_id);
        Transactions Details(int? id, int user_id);
        Task BuyCurrency(TransactionBuyCurrency tbc, int user_id);
        Task Transfer(TransferModel newTransfer, int id);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public interface IFixedTermDepositBusiness
    {
        IEnumerable<FixedTermDepositModel> GetAllByUserId(int userId);

        Task CreateFixedTermDeposit(FixedTermDepositCreateModel fixedTermDeposit, int userId);

        Task CloseFixedTermDeposit(int fixedTermDepositId, int userId);

        InterestsCalculationModel calculateProfit(string currency, double amount, DateTime from, DateTime to);
    }
}

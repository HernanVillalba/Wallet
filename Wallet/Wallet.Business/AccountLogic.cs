using System;
using System.Collections.Generic;
using System.Text;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Business
{
    public class AccountLogic : IAccountLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        public AccountLogic(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<double> SelectBalances(int id, params string[] currency)
        {
            var balances = new List<double>();
            foreach (string c in currency)
            {
                balances.Add(_unitOfWork.Accounts.GetAccountBalance(id, c));
            }
            return balances;
        }
    }
}

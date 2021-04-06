using AutoMapper;
using System.Collections.Generic;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public class AccountBusiness : IAccountBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AccountBusiness(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public BalanceModel GetBalances(int id)
        {
            BalanceModel balance = new BalanceModel()
            {
                ArgBalance = GetAccountBalance(id, "ARS"),
                UsdBalance = GetAccountBalance(id, "USD")
            };
            return balance;
        }

        public List<AccountModel> GetAccountsWithBalance(int id)
        {
                var accounts = _unitOfWork.Accounts.GetUserAccounts(id);
                List<AccountModel> accountsWithBalance = new List<AccountModel>();
                foreach (Accounts a in accounts)
                {
                    accountsWithBalance.Add(new AccountModel
                    {
                        Id = a.Id,
                        Currency = a.Currency,
                        Balance = GetAccountBalance(id, a.Currency)
                    });
                }
                return accountsWithBalance;
        }

        public double GetAccountBalance(int id, string currency)
        {
            int accountId = _unitOfWork.Accounts.GetAccountId(id, currency);
            var transactions = _unitOfWork.Transactions.GetTransactionsForAccount(accountId);
            double total = 0;
            foreach (Transactions t in transactions)
            {
                if (t.Type == "Topup")
                {
                    total += t.Amount;
                }
                else
                {
                    total -= t.Amount;
                }
            }
            return total;
        }
    }
}

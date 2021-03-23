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
                ArgBalance = _unitOfWork.Accounts.GetAccountBalance(id, "ARS"),
                UsdBalance = _unitOfWork.Accounts.GetAccountBalance(id, "USD")
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
                    Balance = _unitOfWork.Accounts.GetAccountBalance(id, a.Currency)
                });
            }
            return accountsWithBalance;
        }
    }
}

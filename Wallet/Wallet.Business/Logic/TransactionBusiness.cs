using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;
using X.PagedList;

namespace Wallet.Business.Logic
{
    public class TransactionBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TransactionBusiness(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Transactions>> GetAll(int? page)
        {
            if (page == null || page <= 0) { page = 1; }
            int pageNumber = (int)page, pageSize = 10;
            int user_id = 4;
            int ARS_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS"), USD_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            IEnumerable<Transactions> listDB = await _unitOfWork.Transactions.GetTransactionsUser(ARS_id, USD_id);
            IEnumerable<Transactions> paginatedTransactions = await listDB.ToPagedList(pageNumber, pageSize).ToListAsync();

            return paginatedTransactions;
        }

        public async Task Create(TransactionCreateModel newT, int user_id)
        {
            int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");

            Transactions transaction = _mapper.Map<Transactions>(newT);
            transaction.AccountId = ARS_account_id;

            _unitOfWork.Transactions.Insert(transaction);
            await _unitOfWork.Complete();
        }

        public async Task<bool> EditTransaction(int? id, TransactionEditModel NewTransaction, int user_id)
        {
            int USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            var transaction_buscada = _unitOfWork.Transactions.FindTransaction((int)id, USD_account_id, ARS_account_id);

            if (transaction_buscada != null)
            {
                transaction_buscada.Concept = NewTransaction.Concept;
                _unitOfWork.Transactions.Update(transaction_buscada);
                await _unitOfWork.Complete();
                return true;
            }
            else { return false; }
        }

        public Transactions GetDetails(int? id, int user_id)
        {
            int? ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int? USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");

            if (ARS_account_id != null && USD_account_id != null)
            {
                var transaction = _unitOfWork.Transactions
                    .FindTransaction((int)id, (int)USD_account_id, (int)ARS_account_id);

                if (transaction != null) { return transaction; }
                else { return null; }

            }
            return null;
        }
        public IEnumerable<Transactions> Filter(TransactionFilterModel transaction, int user_id)
        {
            int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");

            //si el id de account es null o menor a 0 se asume que busca en pesos
            if (transaction.AccountId == null || transaction.AccountId <= 0)
            {
                transaction.ARS_id = ARS_account_id;
                transaction.USD_id = USD_account_id;
            }

            if(transaction.AccountId != ARS_account_id || transaction.AccountId != USD_account_id) //si el id de la account ingresado es distinta a alguna de la suyas, se asume que busca en pesos
            {
                transaction.ARS_id = ARS_account_id;
                transaction.USD_id = USD_account_id;
            }

            //Transactions transactionDB = _mapper.Map<Transactions>(transaction);
            //IEnumerable<Transactions> List = _unitOfWork.Transactions.FilterTransaction(transactionDB);
            IEnumerable<Transactions> List = _unitOfWork.Transactions.FilterTransaction(transaction);
            return List;
        }

        public async Task<string> Transfer(TransferModel newTransfer, int id)
        {
            //get accounts to compare
            var senderAccount = _unitOfWork.Accounts.GetAccountById(newTransfer.AccountId);
            var recipientAccount = _unitOfWork.Accounts.GetAccountById(newTransfer.RecipientAccountId);
            if (senderAccount == null || recipientAccount == null)
            {
                return ("Alguna de las cuentas ingresadas no existe");
            }
            //set conditions to validate the transfer
            bool isSameAccount = newTransfer.AccountId == newTransfer.RecipientAccountId;
            bool isSameCurrency = senderAccount.Currency == recipientAccount.Currency;
            bool isAccountOwner = senderAccount.UserId == id;
            //validate the transfer
            if (isSameAccount || !isSameCurrency || !isAccountOwner)
            {
                return ("Ingrese cuentas válidas");
            }
            //get balance and validate
            var balance = _unitOfWork.Accounts.GetAccountBalance(senderAccount.UserId, senderAccount.Currency);
            if (newTransfer.Amount > balance)
            {
                return ("Saldo insuficiente");
            }
            //after validation create transactions on both accounts
            Transactions transferTopup = new Transactions
            {
                Amount = newTransfer.Amount,
                Concept = $"Transfer from account {newTransfer.AccountId}",
                Type = "Topup",
                AccountId = newTransfer.RecipientAccountId,
                Editable = false
            };
            Transactions transferPayment = new Transactions
            {
                Amount = newTransfer.Amount,
                Concept = $"Transfer to account {newTransfer.RecipientAccountId}",
                Type = "Payment",
                AccountId = newTransfer.AccountId,
                Editable = false
            };
            //try inserting into database
            _unitOfWork.Transactions.Insert(transferTopup);
            _unitOfWork.Transactions.Insert(transferPayment);
            await _unitOfWork.Complete();
            return ("Transferencia realizada");


            //ADAPT WHEN BUSINESS IS UPDATED

            //var transferTopup = new TransactionCreateModel
            //{
            //    Amount = newTransfer.Amount,
            //    Concept = "Transfer",
            //    Type = "Topup"
            //};
            //var transferPayment = new TransactionCreateModel
            //{
            //    Amount = newTransfer.Amount,
            //    Concept = "Transfer",
            //    Type = "Payment"
            //};


        }
    }
}

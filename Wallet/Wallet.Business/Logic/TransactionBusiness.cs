using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Business.Exceptions;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;
using X.PagedList;

namespace Wallet.Business.Logic
{
    public class TransactionBusiness : ITransactionBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRatesBusiness _rates;
        private readonly IAccountBusiness _accountBusiness;

        public TransactionBusiness(IUnitOfWork unitOfWork, IMapper mapper, IRatesBusiness ratesBusiness, IAccountBusiness accountBusiness)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rates = ratesBusiness;
            _accountBusiness = accountBusiness;
        }

        public async Task<IEnumerable<TransactionModel>> GetAll(TransactionFilterModel tfm, int user_id, int page)
        {
            if (user_id <= 0) { throw new BusinessException(ErrorMessages.User_Date_Not_Found); }
            IEnumerable<Transactions> listDB;

            //si busca con algún filtro
            if ((tfm.Type != "" && tfm.Type != null) ||
                (tfm.Concept != "" && tfm.Concept != null) ||
                (tfm.AccountId != null && tfm.AccountId >= 1))
            {
                listDB = await Filter(tfm, user_id);
            }
            //si busca sin filtros
            else
            {
                AccountsUserModel a = _unitOfWork.Accounts.GetAccountsUsers(user_id);

                if (_unitOfWork.Accounts.InvalidAccounts(a))
                {
                    throw new BusinessException(ErrorMessages.User_Date_Not_Found);
                }
                else
                {
                    listDB = await _unitOfWork.Transactions.GetTransactionsUser((int)a.IdARS, (int)a.IdUSD);
                }
            }

            IEnumerable<TransactionModel> list = _mapper.Map<IEnumerable<TransactionModel>>(listDB);

            //paginado con xpagelist
            if (page <= 0) { page = 1; } //asigna la primer página
            int pageNumber = (int)page, pageSize = 10; //10 registros por página
            list = await list.ToPagedList(pageNumber, pageSize).ToListAsync();

            return list;
        }

        public Task<List<Transactions>> Filter(TransactionFilterModel transaction, int user_id)
        {
            AccountsUserModel acc = new AccountsUserModel()
            {
                IdARS = _unitOfWork.Accounts.GetAccountId(user_id, "ARS"),
                IdUSD = _unitOfWork.Accounts.GetAccountId(user_id, "USD")
            };

            if (_unitOfWork.Accounts.InvalidAccounts(acc)) { throw new BusinessException(ErrorMessages.User_Date_Not_Found); }

            //si el id de account es null o menor a 0 se asume que busca en pesos
            if (transaction.AccountId == null || transaction.AccountId <= 0)
            {
                transaction.AccountId = (int)acc.IdARS;
            }

            if (transaction.AccountId != acc.IdARS && transaction.AccountId != acc.IdUSD) //si el id de la account ingresado es distinta a alguna de la suyas, se asume que busca en pesos
            {
                transaction.AccountId = (int)acc.IdARS;
            }
            Task<List<Transactions>> List = _unitOfWork.Transactions.FilterTransaction(transaction, (int)acc.IdUSD, (int)acc.IdARS);
            return List;
        }

        public async Task Create(TransactionCreateModel newT, int user_id)
        {
            AccountsUserModel acc = new AccountsUserModel()
            {
                IdARS = _unitOfWork.Accounts.GetAccountId(user_id, "ARS")
            };


            if (acc.IdARS == null || acc.IdARS <= 0 || user_id <= 0)
            {
                throw new BusinessException(ErrorMessages.User_Date_Not_Found);
            }

            var saldo = _accountBusiness.GetAccountBalance(user_id, "ARS");

            if ((newT.Type.ToLower() == "payment") && (saldo - newT.Amount < 0))
            {
                throw new InvalidException(ErrorMessages.Not_Enough_Balance);
            }

            Transactions transaction = _mapper.Map<Transactions>(newT);
            transaction.AccountId = (int)acc.IdARS;

            _unitOfWork.Transactions.Insert(transaction);
            await _unitOfWork.Complete();
        }

        public async Task Edit(int? id, TransactionEditModel NewTransaction, int user_id)
        {
            if (id == null || id <= 0)
            {
                throw new BusinessException(ErrorMessages.User_Date_Not_Found);
            }

            AccountsUserModel acc = new AccountsUserModel
            {
                IdARS = _unitOfWork.Accounts.GetAccountId(user_id, "ARS"),
                IdUSD = _unitOfWork.Accounts.GetAccountId(user_id, "USD")
            };

            if (_unitOfWork.Accounts.InvalidAccounts(acc))
            {
                throw new BusinessException(ErrorMessages.User_Date_Not_Found);
            }

            var transaction_buscada = _unitOfWork.Transactions.FindTransaction((int)id, (int)acc.IdUSD, (int)acc.IdARS);

            if (transaction_buscada != null)
            {
                if ((bool)transaction_buscada.Category.Editable)
                {
                    var transactionLog = new TransactionLog
                    {
                        TransactionId = transaction_buscada.Id,
                        NewValue = NewTransaction.Concept
                    };
                    transaction_buscada.Concept = NewTransaction.Concept;
                    _unitOfWork.TransactionLog.Insert(transactionLog); //inserto el nuevo cambio en transaction log
                    _unitOfWork.Transactions.Update(transaction_buscada);
                    await _unitOfWork.Complete();
                    return;
                }
                else { throw new InvalidException(ErrorMessages.Operation_Cannot_Be_Performed); }
            }
            else { throw new InvalidException(ErrorMessages.Resource_Not_Found); }
        }

        public async Task<TransactionDetailsModel> Details(int? t_id, int user_id)
        {
            if (t_id == null || t_id <= 0) { throw new InvalidException(ErrorMessages.Incorrect_Data); }
            if (user_id <= 0) { throw new BusinessException(ErrorMessages.User_Date_Not_Found); }
            AccountsUserModel acc = new AccountsUserModel
            {
                IdARS = _unitOfWork.Accounts.GetAccountId(user_id, "ARS"),
                IdUSD = _unitOfWork.Accounts.GetAccountId(user_id, "USD")
            };

            if (_unitOfWork.Accounts.InvalidAccounts(acc))
            {
                throw new BusinessException(ErrorMessages.User_Date_Not_Found);
            }

            var transaction = _unitOfWork.Transactions.FindTransaction((int)t_id, (int)acc.IdUSD, (int)acc.IdARS);

            if (transaction != null)
            {
                TransactionDetailsModel tdm = _mapper.Map<TransactionDetailsModel>(transaction);

                // I ask if its editable to show the field
                if ((bool)transaction.Category.Editable) { tdm.Editable = true; }
                else { tdm.Editable = false; }

                tdm.TransactionLog = _mapper.Map<List<TransactionLogModel>>(await _unitOfWork.TransactionLog.GetByTransactionId(transaction.Id));
                return tdm;
            }
            else
            {
                throw new InvalidException(ErrorMessages.Resource_Not_Found);
            }
        }

        public async Task Transfer(TransferModel newTransfer, int id)
        {
            //get accounts to compare
            var senderAccount = _unitOfWork.Accounts.GetAccountById(newTransfer.AccountId);
            var recipientAccount = _unitOfWork.Accounts.GetAccountById(newTransfer.RecipientAccountId);

            if (senderAccount == null || recipientAccount == null)
            {
                throw new BusinessException(ErrorMessages.User_Date_Not_Found);
            }

            //set conditions to validate the transfer
            bool isSameAccount = newTransfer.AccountId == newTransfer.RecipientAccountId;
            bool isSameCurrency = senderAccount.Currency == recipientAccount.Currency;
            bool isAccountOwner = senderAccount.UserId == id;
            //validate the transfer
            if (isSameAccount || !isSameCurrency || !isAccountOwner)
            {
                throw new InvalidException(ErrorMessages.Incorrect_Data);
            }
            //get balance and validate
            var balance = _accountBusiness.GetAccountBalance(senderAccount.UserId, senderAccount.Currency);
            if (newTransfer.Amount > balance)
            {
                throw new InvalidException(ErrorMessages.Not_Enough_Balance);
            }
            //after validation create transactions on both accounts
            Transactions transferTopup = new Transactions
            {
                Amount = newTransfer.Amount,
                Concept = $"Transferencia de cuenta {newTransfer.AccountId}",
                Type = "Topup",
                AccountId = newTransfer.RecipientAccountId,
                CategoryId = 4
            };
            Transactions transferPayment = new Transactions
            {
                Amount = newTransfer.Amount,
                Concept = $"Transferencia a la cuenta {newTransfer.RecipientAccountId}",
                Type = "Payment",
                AccountId = newTransfer.AccountId,
                CategoryId = 4
            };
            //try inserting into database
            _unitOfWork.Transactions.Insert(transferPayment);
            _unitOfWork.Transactions.Insert(transferTopup);
            await _unitOfWork.Complete();
            if (transferTopup.Id > 0 && transferPayment.Id > 0)
            {
                Transfers transfer = new Transfers()
                {
                    OriginTransactionId = transferPayment.Id,
                    DestinationTransactionId = transferTopup.Id
                };
                _unitOfWork.Transfers.Insert(transfer);
                await _unitOfWork.Complete();
            }
            else { throw new BusinessException(ErrorMessages.Operation_Cannot_Be_Performed); }
        }

        public async Task BuyCurrency(TransactionBuyCurrency tbc, int user_id)
        {
            //Get accounts and balances
            int ARS_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int USD_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            double balance_ARS = _accountBusiness.GetAccountBalance(user_id, "ARS");
            double balance_USD = _accountBusiness.GetAccountBalance(user_id, "USD");
            double cost;
            Rates rates = await _rates.GetRates();
            Transactions transactionOrigin;
            Transactions transactionDestiny;
            if (tbc.Type == "Compra")
            {
                cost = tbc.Amount * rates.BuyingPrice;
                if (balance_ARS >= cost)
                {
                    //en USD
                    transactionOrigin = new Transactions
                    {
                        Amount = tbc.Amount,
                        Concept = "Compra de divisas",
                        Type = "Topup",
                        AccountId = USD_accountId,
                        CategoryId = 2
                    };
                    //en ARS
                    transactionDestiny = new Transactions
                    {
                        Amount = cost,
                        Concept = "Compra de divisas",
                        Type = "Payment",
                        AccountId = ARS_accountId,
                        CategoryId = 2
                    };
                }
                else { throw new InvalidException(ErrorMessages.Not_Enough_Balance); }
            }
            else
            {
                cost = tbc.Amount * rates.SellingPrice;
                if (tbc.Amount <= balance_USD)
                {
                    //en USD
                    transactionOrigin = new Transactions
                    {
                        AccountId = (int)USD_accountId,
                        Amount = tbc.Amount,
                        Concept = "Compra de divisas",
                        Type = "Payment",
                        CategoryId = 2
                    };
                    //en ARS
                    transactionDestiny = new Transactions
                    {
                        AccountId = (int)ARS_accountId,
                        Amount = cost,
                        Concept = "Compra de divisas",
                        Type = "Topup",
                        CategoryId = 2
                    };

                }
                else { throw new InvalidException(ErrorMessages.Not_Enough_Balance); }
            }
            _unitOfWork.Transactions.Insert(transactionOrigin);
            _unitOfWork.Transactions.Insert(transactionDestiny);
            await _unitOfWork.Complete();
            return;
        }

    }
}

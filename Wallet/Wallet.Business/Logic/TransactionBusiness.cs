using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            if (user_id <= 0) { throw new CustomException(400, "Id de usuario no válido"); }
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
                if (_unitOfWork.Accounts.ValidateAccounts(a))
                {
                    listDB = await _unitOfWork.Transactions.GetTransactionsUser((int)a.IdARS, (int)a.IdUSD);
                }
                else { throw new CustomException(404, "No se encontró algunas de las cuentas del usuario"); }
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
            int? ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int? USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            if (ARS_account_id == null || USD_account_id == null) { throw new CustomException(404, "No se encontró algunas de las cuentas del usuario"); }

            //si el id de account es null o menor a 0 se asume que busca en pesos
            if (transaction.AccountId == null || transaction.AccountId <= 0)
            {
                transaction.AccountId = (int)ARS_account_id;
            }

            if (transaction.AccountId != ARS_account_id && transaction.AccountId != USD_account_id) //si el id de la account ingresado es distinta a alguna de la suyas, se asume que busca en pesos
            {
                transaction.AccountId = (int)ARS_account_id;
            }
            Task<List<Transactions>> List = _unitOfWork.Transactions.FilterTransaction(transaction, (int)USD_account_id, (int)ARS_account_id);
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
                throw new CustomException(404, "No se pudo obtener alguno de los datos del usuario");
            }
            var saldo = _accountBusiness.GetAccountBalance(user_id, "ARS");
            if (newT.Type == "Payment" && _accountBusiness.GetAccountBalance(user_id, "ARS") - newT.Amount < 0)
            {
                throw new CustomException(400, "No hay saldo suficiente para realizar la transacción");
            }

            Transactions transaction = _mapper.Map<Transactions>(newT);
            transaction.AccountId = (int)acc.IdARS;
            _unitOfWork.Transactions.Insert(transaction);
            await _unitOfWork.Complete();
        }

        public async Task Edit(int? id, TransactionEditModel NewTransaction, int user_id)
        {
            AccountsUserModel acc = new AccountsUserModel();
            acc.IdARS = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            acc.IdUSD = _unitOfWork.Accounts.GetAccountId(user_id, "USD");

            if (acc.IdUSD == null || acc.IdUSD <= 0 || acc.IdARS == null || acc.IdARS <= 0)
            {
                throw new CustomException(404, "No se encontró alguna de las cuentas del usuario");
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
                else { throw new CustomException(400, "La transacción no es editable"); }
            }
            else { throw new CustomException(400, "No se encontró la transacción"); }
        }

        public async Task<TransactionDetailsModel> Details(int? id, int user_id)
        {
            if (user_id <= 0) { throw new CustomException(404, "Id de usario no válido"); }
            AccountsUserModel acc = new AccountsUserModel();
            acc.IdARS = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            acc.IdUSD = _unitOfWork.Accounts.GetAccountId(user_id, "USD");

            if (acc.IdARS != null && acc.IdUSD != null)
            {
                var transaction = _unitOfWork.Transactions.FindTransaction((int)id, (int)acc.IdUSD, (int)acc.IdARS);

                if (transaction != null)
                {
                    TransactionDetailsModel tdm = _mapper.Map<TransactionDetailsModel>(transaction);
                    tdm.TransactionLog = _mapper.Map<List<TransactionLogModel>>(await _unitOfWork.TransactionLog.GetByTransactionId(transaction.Id));
                    return tdm;
                }
                else { throw new CustomException(400, "No se encontró la transacción"); }

            }
            else { throw new CustomException(404, "No se encontró alguna de las cuentas del usuario"); }
        }

        public async Task Transfer(TransferModel newTransfer, int id)
        {
            //get accounts to compare
            var senderAccount = _unitOfWork.Accounts.GetAccountById(newTransfer.AccountId);
            var recipientAccount = _unitOfWork.Accounts.GetAccountById(newTransfer.RecipientAccountId);
            if (senderAccount == null || recipientAccount == null)
            {
                throw new CustomException(404, "Alguna de las cuentas ingresadas no existe");
            }
            //set conditions to validate the transfer
            bool isSameAccount = newTransfer.AccountId == newTransfer.RecipientAccountId;
            bool isSameCurrency = senderAccount.Currency == recipientAccount.Currency;
            bool isAccountOwner = senderAccount.UserId == id;
            //validate the transfer
            if (isSameAccount || !isSameCurrency || !isAccountOwner)
            {
                throw new CustomException(400, "Alguno de los datos ingresados es incorrecto");
            }
            //get balance and validate
            var balance = _accountBusiness.GetAccountBalance(senderAccount.UserId, senderAccount.Currency);
            if (newTransfer.Amount > balance)
            {
                throw new CustomException(400, "Saldo insuficiente");
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
            else { throw new CustomException(404, "No se creó la transferencia"); }
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
                else { throw new CustomException(400, "Saldo insuficiente"); }
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
                else { throw new CustomException(400, "Saldo insuficiente"); }
            }
            _unitOfWork.Transactions.Insert(transactionOrigin);
            _unitOfWork.Transactions.Insert(transactionDestiny);
            await _unitOfWork.Complete();
            return;
        }

    }
}

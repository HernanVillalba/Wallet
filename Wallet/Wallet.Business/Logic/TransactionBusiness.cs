using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public class TransactionBusiness : ITransactionBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRatesBusiness _rates;

        public TransactionBusiness(IUnitOfWork unitOfWork, IMapper mapper, IRatesBusiness ratesBusiness)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rates = ratesBusiness;
        }

        public async Task<IEnumerable<Transactions>> GetAll(TransactionFilterModel tfm, int user_id)
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
                int ARS_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS"),
                    USD_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
                listDB = await _unitOfWork.Transactions.GetTransactionsUser(ARS_id, USD_id);
            }

            return listDB;
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
            int? ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            if (ARS_account_id == null || ARS_account_id <= 0 || user_id <= 0) { throw new CustomException(404, "No se pudo obtener alguno de los datos del usuario"); }
            newT.AccountId = ARS_account_id;
            Transactions transaction = _mapper.Map<Transactions>(newT);
            _unitOfWork.Transactions.Insert(transaction);
            await _unitOfWork.Complete();
        }

        public async Task Edit(int? id, TransactionEditModel NewTransaction, int user_id)
        {
            int? USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            int? ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");

            if (USD_account_id == null || USD_account_id <= 0 || ARS_account_id == null || ARS_account_id <= 0)
            {
                throw new CustomException(404, "No se encontró alguna de las cuentas del usuario");
            }

            var transaction_buscada = _unitOfWork.Transactions.FindTransaction((int)id, (int)USD_account_id, (int)ARS_account_id);

            if (transaction_buscada != null)
            {
                if ((bool)transaction_buscada.Editable)
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
            int? ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int? USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");

            if (ARS_account_id != null && USD_account_id != null)
            {
                var transaction = _unitOfWork.Transactions.FindTransaction((int)id, (int)USD_account_id, (int)ARS_account_id);
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

        //public async Task BuyCurrency(TransactionBuyCurrency tbc, int user_id)
        //{
        //    TransactionCreateModel transaction = new TransactionCreateModel(); //transacción que uso para crearla
        //    DollarBusiness db = new DollarBusiness(); //separé la logica del consumo de la API en DollarBusiness
        //    var dollar = db.GetDollarByName("Dolar blue"); //aca trae el dolar blue, pero puede traer otros como el oficial

        //    if (dollar == null) { throw new CustomException(404, "No se pudo obtener el valor del dólar"); }

        //    //datos necesarios del usario para realizar las transacciones
        //    int? ARS_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
        //    int? USD_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
        //    double? balance_ARS = _unitOfWork.Accounts.GetAccountBalance(user_id, "ARS");
        //    double? balance_USD = _unitOfWork.Accounts.GetAccountBalance(user_id, "USD");
        //    double cost; // costo de las operaciones


        //    ///Logica para ahorrar código///
        //    ///entender la lógica de que comprar dólares es vender pesos y
        //    ///vender dólares es comprar pesos, me ahorró mucho código repetitivo

        //    if ((tbc.Type.ToLower() == "compra" && tbc.Currency.ToLower() == "usd") ||
        //        (tbc.Type.ToLower() == "venta" && tbc.Currency.ToLower() == "ars")) // si quiere comprar usd, quiere vender pesos
        //    {
        //        cost = tbc.Amount * Convert.ToDouble(dollar.Casa.Venta);

        //        if (balance_ARS >= cost) // si se cumple, quiere decir que tengo saldo suficiente
        //        {
        //            //en USD
        //            Transactions transactionOrigin = new Transactions
        //            {
        //                Amount = tbc.Amount,
        //                Concept = "Compra de divisas",
        //                Type = "Topup",
        //                AccountId = (int)USD_accountId,
        //                Editable = false
        //            };
        //            //en ARS
        //            Transactions transactionsDestiny = new Transactions
        //            {
        //                Amount = cost,
        //                Concept = "Compra de divisas",
        //                Type = "Payment",
        //                AccountId = (int)ARS_accountId,
        //                Editable = false
        //            };
        //            _unitOfWork.Transactions.Insert(transactionOrigin);
        //            _unitOfWork.Transactions.Insert(transactionsDestiny);
        //            await _unitOfWork.Complete();
        //            return;
        //        }
        //        else { throw new CustomException(400, "Saldo insuficiente"); }
        //    }
        //    else if ((tbc.Type.ToLower() == "venta" && tbc.Currency.ToLower() == "usd") ||
        //             (tbc.Type.ToLower() == "compra" && tbc.Currency.ToLower() == "ars")) //si quiere vender dolares, quiere comprar pesos
        //    {
        //        cost = tbc.Amount * Convert.ToDouble(dollar.Casa.Compra);
        //        if (tbc.Amount <= balance_USD) //si la cantidad que quiero vender...
        //        {
        //            //en USD
        //            Transactions transactionsOrigin = new Transactions
        //            {
        //                AccountId = (int)USD_accountId,
        //                Amount = tbc.Amount,
        //                Concept = "Compra de divisas",
        //                Type = "Payment",
        //                Editable = false
        //            };
        //            //en ARS
        //            Transactions transactionsDestiny = new Transactions
        //            {
        //                AccountId = (int)ARS_accountId,
        //                Amount = cost,
        //                Concept = "Compra de divisas",
        //                Type = "Topup",
        //                Editable = false
        //            };

        //            _unitOfWork.Transactions.Insert(transactionsOrigin);
        //            _unitOfWork.Transactions.Insert(transactionsDestiny);
        //            await _unitOfWork.Complete();
        //            return;
        //        }
        //        else { throw new CustomException(400, "Saldo insuficiente"); }
        //    }
        //    else { throw new CustomException(400, "Algunos de los datos ingresados son incorrectos"); }
        //}

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
            var balance = _unitOfWork.Accounts.GetAccountBalance(senderAccount.UserId, senderAccount.Currency);
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
                Editable = false
            };
            Transactions transferPayment = new Transactions
            {
                Amount = newTransfer.Amount,
                Concept = $"Transferencia a la cuenta {newTransfer.RecipientAccountId}",
                Type = "Payment",
                AccountId = newTransfer.AccountId,
                Editable = false
            };
            //try inserting into database
            _unitOfWork.Transactions.Insert(transferTopup);
            _unitOfWork.Transactions.Insert(transferPayment);
            await _unitOfWork.Complete();
        }

        public async Task BuyCurrency(TransactionBuyCurrency tbc, int user_id)
        {
            //Get accounts and balances
            int ARS_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int USD_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            double balance_ARS = _unitOfWork.Accounts.GetAccountBalance(user_id, "ARS");
            double balance_USD = _unitOfWork.Accounts.GetAccountBalance(user_id, "USD");
            double cost;
            Rates rates = await _rates.GetRates();

            if (tbc.Type == "Compra")
            {
                cost = tbc.Amount * rates.BuyingPrice;
                if (balance_ARS >= cost)
                {
                    //en USD
                    Transactions transactionOrigin = new Transactions
                    {
                        Amount = tbc.Amount,
                        Concept = "Compra de divisas",
                        Type = "Topup",
                        AccountId = USD_accountId,
                        Editable = false
                    };
                    //en ARS
                    Transactions transactionsDestiny = new Transactions
                    {
                        Amount = cost,
                        Concept = "Compra de divisas",
                        Type = "Payment",
                        AccountId = ARS_accountId,
                        Editable = false
                    };
                    _unitOfWork.Transactions.Insert(transactionOrigin);
                    _unitOfWork.Transactions.Insert(transactionsDestiny);
                    await _unitOfWork.Complete();
                    return;
                }
                else { throw new CustomException(400, "Saldo insuficiente"); }
            }        
            else
            {
                cost = tbc.Amount * rates.SellingPrice;
                if (tbc.Amount <= balance_USD)
                {
                    //en USD
                    Transactions transactionsOrigin = new Transactions
                    {
                        AccountId = (int)USD_accountId,
                        Amount = tbc.Amount,
                        Concept = "Compra de divisas",
                        Type = "Payment",
                        Editable = false
                    };
                    //en ARS
                    Transactions transactionsDestiny = new Transactions
                    {
                        AccountId = (int)ARS_accountId,
                        Amount = cost,
                        Concept = "Compra de divisas",
                        Type = "Topup",
                        Editable = false
                    };

                    _unitOfWork.Transactions.Insert(transactionsOrigin);
                    _unitOfWork.Transactions.Insert(transactionsDestiny);
                    await _unitOfWork.Complete();
                    return;
                }
                else { throw new CustomException(400, "Saldo insuficiente"); }            
            }
        }

    }
}

using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;
using X.PagedList;
using Newtonsoft.Json;
using System.Net;
using Wallet.Business.Operations;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using System.Net.Http;
using System;
using System.Net.Http.Headers;

namespace Wallet.Business.Logic
{
    public class TransactionBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public object CreateMap { get; private set; }

        public TransactionBusiness(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Transactions>> GetAll(int user_id)
        {
            if (user_id <= 0) { throw new CustomException(400, "Id de usuario no válido"); }

            int ARS_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS"),
                USD_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            IEnumerable<Transactions> listDB = await _unitOfWork.Transactions.GetTransactionsUser(ARS_id, USD_id);

            if (listDB != null && listDB.Count() > 0) { return listDB; }
            else { throw new CustomException(404, "No hay transacciones para mostrar"); }
        }

        public async Task Create(TransactionCreateModel newT)
        {
            Transactions transaction = _mapper.Map<Transactions>(newT);
            _unitOfWork.Transactions.Insert(transaction);
            await _unitOfWork.Complete();
        }

        public async Task<string> EditTransaction(int? id, TransactionEditModel NewTransaction, int user_id)
        {
            int USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            var transaction_buscada = _unitOfWork.Transactions.FindTransaction((int)id, USD_account_id, ARS_account_id);

            if (transaction_buscada != null)
            {
                if ((bool)transaction_buscada.Editable)
                {
                    transaction_buscada.Concept = NewTransaction.Concept;
                    _unitOfWork.Transactions.Update(transaction_buscada);
                    await _unitOfWork.Complete();
                    return "Transacción actualizada con éxito";
                }
                else { throw new CustomException(400, "La transacción no se puede editar"); }
            }
            else { throw new CustomException(400, "No se encontró la transacción"); }
        }

        public Transactions Details(int? id, int user_id)
        {
            if (user_id <= 0) { throw new CustomException(404, "Id de usario no válido"); }
            int? ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int? USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");

            if (ARS_account_id != null && USD_account_id != null)
            {
                var transaction = _unitOfWork.Transactions
                    .FindTransaction((int)id, (int)USD_account_id, (int)ARS_account_id);

                if (transaction != null) { return transaction; }
                else { throw new CustomException(400, "No se encontró la transacción"); }

            }
            else { throw new CustomException(404, "No se encontraron las cuentas del usuario"); }
        }
        public IEnumerable<Transactions> Filter(TransactionFilterModel transaction, int user_id)
        {
            int? ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int? USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            if (ARS_account_id == null || USD_account_id == null) { throw new CustomException(404, "No se pudo encontrar las cuentas del usuario"); }

            //si el id de account es null o menor a 0 se asume que busca en pesos
            if (transaction.AccountId == null || transaction.AccountId <= 0)
            {
                transaction.ARS_Id = (int)ARS_account_id;
                transaction.USD_Id = (int)USD_account_id;
            }

            if (transaction.AccountId != ARS_account_id || transaction.AccountId != USD_account_id) //si el id de la account ingresado es distinta a alguna de la suyas, se asume que busca en pesos
            {
                transaction.ARS_Id = (int)ARS_account_id;
                transaction.USD_Id = (int)USD_account_id;
            }
            IEnumerable<Transactions> List = _unitOfWork.Transactions.FilterTransaction(transaction);
            if (List != null && List.Count() > 0) { return List; }
            else { throw new CustomException(404, "No se encontraron transacciones"); }
        }

        public async Task BuyCurrency(TransactionBuyCurrency tbc, int user_id)
        {
            TransactionCreateModel transaction = new TransactionCreateModel(); //transacción que uso para crearla
            DollarBusiness db = new DollarBusiness(); //separé la logica del consumo de la API en DollarBusiness
            var dollar = db.GetDollarByName("Dolar blue"); //aca trae el dolar blue, pero puede traer otros como el oficial

            if (dollar == null) { throw new CustomException(404, "No se pudo obtener el valor actual del dólar"); }

            //datos necesarios del usario para realizar las transacciones
            int? ARS_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int? USD_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            double? balance_ARS = _unitOfWork.Accounts.GetAccountBalance(user_id, "ARS");
            double? balance_USD = _unitOfWork.Accounts.GetAccountBalance(user_id, "USD");
            double cost; // costo de las operaciones


            ///Logica para ahorrar código///
            ///entender la lógica de que comprar dólares es vender pesos y
            ///vender dólares es comprar pesos, me ahorró mucho código repetitivo

            if ((tbc.Type.ToLower() == "compra" && tbc.Currency == "USD") || (tbc.Type.ToLower() == "venta" && tbc.Currency == "ARS")) // si quiere comprar usd, quiere vender pesos
            {
                cost = tbc.Amount * Convert.ToDouble(dollar.Casa.Venta);

                if (balance_ARS >= cost) // si se cumple, quiere decir que tengo saldo suficiente
                {
                    //en USD
                    Transactions transactionOrigin = new Transactions
                    {
                        Amount = tbc.Amount,
                        Concept = "Compra de divisas",
                        Type = "Topup",
                        AccountId = (int)USD_accountId
                    };
                    //en ARS
                    Transactions transactionsDestiny = new Transactions
                    {
                        Amount = cost,
                        Type = "Payment",
                        AccountId = (int)ARS_accountId
                    };
                    _unitOfWork.Transactions.Insert(transactionOrigin);
                    _unitOfWork.Transactions.Insert(transactionsDestiny);
                    await _unitOfWork.Complete();
                    return;
                }
                else { throw new CustomException(400, "Saldo insuficiente para realizar la transacción"); }
            }
            else if ((tbc.Type.ToLower() == "venta" && tbc.Currency == "USD") || (tbc.Type.ToLower() == "compra" && tbc.Currency == "ARS")) //si quiere vender dolares, quiere comprar pesos
            {
                cost = tbc.Amount * Convert.ToDouble(dollar.Casa.Compra);
                if (tbc.Amount <= balance_USD) //si la cantidad que quiero vender...
                {
                    //en USD
                    Transactions transactionsOrigin = new Transactions
                    {
                        Amount = tbc.Amount,
                        Concept = "Compra de divisas",
                        Type = "Payment",
                        AccountId = (int)USD_accountId
                    };
                    //en ARS
                    Transactions transactionsDestiny = new Transactions
                    {
                        Amount = cost,
                        Concept = "Compra de divisas",
                        Type = "Topup",
                        AccountId = (int)ARS_accountId
                    };

                    _unitOfWork.Transactions.Insert(transactionsOrigin);
                    _unitOfWork.Transactions.Insert(transactionsDestiny);
                    await _unitOfWork.Complete();
                    return;
                }
                else { throw new CustomException(400, "Saldo insuficiente para realizar la transacción"); }
            }
            else { throw new CustomException(400, "Los datos ingresados son incorrectos"); }
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
            var balance = _unitOfWork.Accounts.GetAccountBalance(senderAccount.UserId, senderAccount.Currency);
            if (newTransfer.Amount > balance)
            {
                throw new CustomException(400, "Saldo insuficiente");
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
        }
    }
}

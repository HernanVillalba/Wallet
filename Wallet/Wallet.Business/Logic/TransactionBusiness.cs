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

        public async Task Create(TransactionCreateModel newT)
        {
            Transactions transaction = _mapper.Map<Transactions>(newT);

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

            if (transaction.AccountId != ARS_account_id || transaction.AccountId != USD_account_id) //si el id de la account ingresado es distinta a alguna de la suyas, se asume que busca en pesos
            {
                transaction.ARS_id = ARS_account_id;
                transaction.USD_id = USD_account_id;
            }
            IEnumerable<Transactions> List = _unitOfWork.Transactions.FilterTransaction(transaction);
            return List;
        }

        public async Task<string> BuyCurrency(TransactionBuyCurrency tbc, int user_id)
        {
            TransactionCreateModel transaction = new TransactionCreateModel(); //transacción que uso para crearla
            DollarBusiness db = new DollarBusiness(); //separé la logica del consumo de la API en DollarBusiness
            var dollar = db.GetDollarByName("Dolar blue"); //aca trae el dolar blue, pero puede traer otros como el oficial

            //datos necesarios del usario para realizar las transacciones
            int? ARS_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int? USD_accountId = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            double? balance_ARS = _unitOfWork.Accounts.GetAccountBalance(user_id, "ARS");
            double? balance_USD = _unitOfWork.Accounts.GetAccountBalance(user_id, "USD");


            ///Logica para ahorrar código///
            ///entender la lógica de que comprar dólares es vender pesos y
            ///vender dólares es comprar pesos, me ahorró mucho código repetitivo
            ///sumary///

            double cost; // costo de las operaciones
            if ((tbc.Type.ToLower() == "compra" && tbc.Currency == "USD") ||
                (tbc.Type.ToLower() == "venta" && tbc.Currency == "ARS")) // si quiere comprar usd, quiere vender pesos
            {
                cost = tbc.Amount * Convert.ToDouble(dollar.Casa.Venta);
                if (balance_ARS >= cost) // si se cumple, quiere decir que tengo saldo suficiente
                {
                    //realizo transaccion para la cuenta en usd (origen)
                    transaction.Amount = tbc.Amount; //lo que compré
                    transaction.Concept = "Compra de divisas";
                    transaction.Type = "Topup";
                    transaction.AccountId = (int)USD_accountId;
                    await Create(transaction); //creo la transacción como topup

                    //realizo la transaccion para la cuenta en ars (destino)
                    transaction.Amount = cost; //lo que me salió la operación
                    transaction.Type = "Payment";
                    transaction.AccountId = (int)ARS_accountId;
                    await Create(transaction);
                    return "Transacción realizada con éxito.";
                }
                else { return "Saldo insuficiente para realizar la transacción"; }
            }
            else if ((tbc.Type.ToLower() == "venta" && tbc.Currency == "USD") ||
                    (tbc.Type.ToLower() == "compra" && tbc.Currency == "ARS")) //si quiere vender dolares, quiere comprar pesos
            {
                cost = tbc.Amount * Convert.ToDouble(dollar.Casa.Compra);
                if (tbc.Amount <= balance_USD) //si la cantidad que quiero vender...
                {
                    //realizo la transaccion para la cuenta en usd (origen)
                    transaction.Amount = tbc.Amount;
                    transaction.Concept = "Compra de divisas";
                    transaction.Type = "Payment"; //aparece como venta en dolares
                    transaction.AccountId = (int)USD_accountId;
                    await Create(transaction);

                    //realizo la transacción para la cuenta en ars (destino)
                    transaction.Amount = cost; //lo que me dieron por la venta de los usd
                    transaction.Type = "Topup"; //aparece como recarga en pesos...
                    transaction.AccountId = (int)ARS_accountId;
                    await Create(transaction);
                    return "Transacción realizada con éxito";
                }
                else { return "Saldo insuficiente para realizar la transacción"; }
            }
            return "Los datos ingresados son incorrectos";

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
        }
    }
}

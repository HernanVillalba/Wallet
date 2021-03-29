using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public class FixedTermDepositBusiness : IFixedTermDepositBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FixedTermDepositBusiness(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<FixedTermDepositModel> GetAllByUserId(int userId)
        {
            if (userId <= 0)
                throw new CustomException(400, "Id inválido");

            // Get the fixed term deposits from database
            var fixedTermDepositsDB = _unitOfWork.FixedTermDeposits.GetAllByUserId(userId);

            // Map entity model to view model
            var fixedTermDeposits = _mapper.Map<IEnumerable<FixedTermDeposits>,IEnumerable<FixedTermDepositModel>>(fixedTermDepositsDB);
            
            return fixedTermDeposits;
        }

        public async Task CreateFixedTermDeposit(FixedTermDepositCreateModel fixedTermDeposit, int userId)
        {
            if (userId <= 0)
                throw new CustomException(400, "Id inválido");

            // Must determine if there is enough balance in the account to afford the fixed term deposit opening

            // We need first the currency to call the stored procedure which calculates the balance
            var account = _unitOfWork.Accounts.GetById(fixedTermDeposit.AccountId);
            if (account == null)
                throw new CustomException(404, "Cuenta inexistente");

            string currency = account.Currency;

            // Execute the respective stored procedure to get the balance
            var balance = _unitOfWork.Accounts.GetAccountBalance(userId, currency);

            if (balance - fixedTermDeposit.Amount < 0)
            {
                // If there isn't enough balance in the account, we cannot continue
                throw new CustomException(400, "No hay suficiente dinero para realizar la operación.");
            }

            // We have enough balance. Lets create the fixed term deposit

            // First make a payment transaction
            Transactions newTransaction = new Transactions();
            newTransaction.AccountId = fixedTermDeposit.AccountId;
            newTransaction.Amount = fixedTermDeposit.Amount;
            newTransaction.Concept = "Plazo Fijo (Apertura)";
            newTransaction.Type = "Payment";
            newTransaction.Editable = false;
            _unitOfWork.Transactions.Insert(newTransaction);

            // Having the transaction placed, it's time to make the fixed term deposit

            // Mapping the model received to entity model
            FixedTermDeposits newFixedTermDeposit = _mapper.Map<FixedTermDeposits>(fixedTermDeposit);

            // Insert the new fixed term deposit to unit of work
            _unitOfWork.FixedTermDeposits.Insert(newFixedTermDeposit);

            // Save changes to database
            await _unitOfWork.Complete();
        }

        public async Task CloseFixedTermDeposit(int fixedTermDepositId)
        {
            if (fixedTermDepositId <= 0)
                throw new CustomException(400, "Id inválido");

            // First check if this fixed term deposit exists
            var fixedTermDeposit = _unitOfWork.FixedTermDeposits.GetById((int)fixedTermDepositId);
            if (fixedTermDeposit == null)
                throw new CustomException(404, "Plazo fijo inexistente"); // Fixed term deposit doesn't exist

            // Now that we know it exists, we have to change the closing date,
            // calculate the days and apply the topup transaction

            // But first we have to check if it has already been closed
            if (fixedTermDeposit.ClosingDate.HasValue)
                throw new CustomException(400, "El plazo fijo fue cerrado anteriormente");

            // If it hasn't been closed, we can proceed to close it

            fixedTermDeposit.ClosingDate = DateTime.Now; // Closing date isn't null anymore
            TimeSpan difference = ((DateTime)fixedTermDeposit.ClosingDate) - fixedTermDeposit.CreationDate;
            int days = difference.Days;
            // [ASK] if it has to be business days

            if(days < 1)
            {
                throw new CustomException(400, "Debe esperar al menos 24 hrs para cerrar el plazo fijo");
            }

            // Apply 1% for each day, with compound interest
            double gainRate = 1 / 100d; // 1%
            double total = fixedTermDeposit.Amount * Math.Pow(1 + gainRate, days);
            // [ASK] if we can parametrize that 1% to be another number in some configuration table,
            // just to avoid hard coded it

            // Now, we have to add a topup transaction with total value
            Transactions newTransaction = new Transactions();
            newTransaction.AccountId = fixedTermDeposit.AccountId;
            newTransaction.Amount = total;
            newTransaction.Concept = "Plazo Fijo (Cierre)";
            newTransaction.Type = "Topup";
            newTransaction.Editable = false;
            _unitOfWork.Transactions.Insert(newTransaction);

            // Having the transaction placed, it's time to update the fixed term deposit
            // since we changed the closing date
            _unitOfWork.FixedTermDeposits.Update(fixedTermDeposit);

            // Save changes to database
            await _unitOfWork.Complete();
        }
    }
}

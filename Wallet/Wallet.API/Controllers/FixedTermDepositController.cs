using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.API.Models;
using Wallet.Data.ModelsAPI;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FixedTermDepositController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FixedTermDepositController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                // Get UserId (you can hardcode it to be 1, 2 or whatever to test the function)
                var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

                // Load and execute the respective stored procedure to retrieve all the
                // fixed term deposits of the two user's accounts
                string storedProcedure = "SP_GetUserFixedTermDeposit " + userId;
                var fixedDeposits = _unitOfWork.FixedTermDeposits.ExecuteStoredProcedure(storedProcedure).ToList();

                // Check if the response was valid, otherwise throw Error 400
                if(fixedDeposits == null)
                {
                    return BadRequest();
                }
                return Ok(fixedDeposits);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateFixedTermDeposit([FromBody] FixedTermDepositModel fixedTermDeposit)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    // Get UserId (you can hardcode it to be 1, 2 or whatever to test the function)
                    var userId = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

                    // Must determine if there is enough balance in the account to afford the fixed term deposit opening

                    // We need first the currency to call the stored procedure which calculates the balance
                    var account = _unitOfWork.Accounts.GetById(fixedTermDeposit.AccountId);
                    if(account == null)
                    {
                        return BadRequest("Cuenta inexistente.");
                    }
                    string currency = account.Currency;

                    // Execute the respective stored procedure to get the balance
                    var balance = _unitOfWork.Accounts.GetAccountBalance(userId, currency);

                    if(balance - fixedTermDeposit.Amount < 0)
                    {
                        // If there isn't enough balance in the account, we cannot continue
                        return BadRequest("No hay suficiente dinero para realizar la operación.");
                    }

                    // We have enough balance. Lets create the fixed term deposit

                    // First make a payment transaction
                    Transactions newTransaction = new Transactions();
                    newTransaction.AccountId = fixedTermDeposit.AccountId;
                    newTransaction.Amount = fixedTermDeposit.Amount;
                    newTransaction.Concept = "Plazo Fijo (Apertura)";
                    newTransaction.Type = "Payment";
                    _unitOfWork.Transactions.Insert(newTransaction);

                    // Having the transaction placed, it's time to make the fixed term deposit

                    // Mapping the model received to entity model
                    FixedTermDeposit newFixedTermDeposit = _mapper.Map<FixedTermDeposit>(fixedTermDeposit);
                    
                    // Insert the new fixed term deposit to unit of work
                    _unitOfWork.FixedTermDeposits.Insert(newFixedTermDeposit);

                    // Save changes and return 200 OK
                    await _unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("Datos de entrada inválidos.");
        }

        [HttpPost("Close/{id}")]
        public async Task<IActionResult> CloseFixedTermDeposit(int? id)
        {
            // Think if the function has to receive only the id or the entire fixed term deposit model
            if (id == null || id <= 0) return BadRequest("Id inválido.");
            try
            {
                // First check if this fixed term deposit exists
                var fixedTermDeposit = _unitOfWork.FixedTermDeposits.GetById((int)id);
                if(fixedTermDeposit == null)
                {
                    return BadRequest("Plazo fijo inexistente."); // Fixed term deposit doesn't exist
                }

                // Now that we know it exists, we have to change the closing date,
                // calculate the days and apply the topup transaction

                fixedTermDeposit.ClosingDate = DateTime.Now; // Closing date isn't null anymore
                TimeSpan difference = ((DateTime)fixedTermDeposit.ClosingDate) - fixedTermDeposit.CreationDate;
                int days = difference.Days;
                // [ASK] if it has to be business days

                // Apply 1% for each day, with compound interest
                double gainRate = 1/100d; // 1%
                double total = fixedTermDeposit.Amount * Math.Pow(1 + gainRate, days);
                // [ASK] if we can parametrize that 1% to be another number in some configuration table,
                // just to avoid hard coded it

                // Now, we have to add a topup transaction with total value
                Transactions newTransaction = new Transactions();
                newTransaction.AccountId = fixedTermDeposit.AccountId;
                newTransaction.Amount = total;
                newTransaction.Concept = "Plazo Fijo (Cierre)";
                newTransaction.Type = "Topup";
                _unitOfWork.Transactions.Insert(newTransaction);

                // Having the transaction placed, it's time to update the fixed term deposit
                // since we changed the closing date
                _unitOfWork.FixedTermDeposits.Update(fixedTermDeposit);

                // Save changes and return 200 OK
                await _unitOfWork.Complete();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

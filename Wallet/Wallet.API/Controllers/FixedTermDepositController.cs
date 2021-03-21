using AutoMapper;
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

        [HttpPost]
        public async Task<IActionResult> PostFixedTermDeposit([FromBody] FixedTermDepositModel fixedTermDeposit)
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
                    newTransaction.Concept = "Plazo Fijo";
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
    }
}

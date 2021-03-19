using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wallet.API.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Data.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Wallet.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TransactionsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            //tengo que saber el id del user traido desde el login, por ahora lista todas las transacciones
            //asignando su id a mano.
            var user_id = 1;
            string SP = "SP_ListTransactionsByUser " + user_id;
            var transactions = _unitOfWork.Transactions.SP_TrasactionsByUser(SP, user_id);
            if (transactions != null) { return Ok(transactions); }
            else { return BadRequest(); }
        }
            try
            {
                var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                string SP = "SP_GetTransactionsUser " + user_id;
                var list_transactions = _unitOfWork.Transactions.SP_GetTransactionsUser(SP, user_id);

                if (list_transactions != null) { return Ok(list_transactions); }
                else { return BadRequest("No hay transacciones para mostrar"); }
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TransactionModel NewTransaction)
        {
            //por ahora solo se realalizan transacciones en ARS
            if (ModelState.IsValid)
            {
                try
                {
                    var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                    int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");

                    Transactions transaction = _mapper.Map<Transactions>(NewTransaction);
                    transaction.AccountId = ARS_account_id;

                    _unitOfWork.Transactions.Insert(transaction);
                    await _unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else { return BadRequest(); }
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id, [FromBody] TransactionModel NewTransaction)
        {
            //var transaction_buscada = _unitOfWork.
            if (!ModelState.IsValid || id == null || id <= 0) { return BadRequest(); }
            else
            {
                try
                {
                    var transaction = _unitOfWork.Transactions.GetById((int)id);
                    if (transaction == null) { return BadRequest(); }
                    transaction.Concept = NewTransaction.Concept;
                    _unitOfWork.Transactions.Update(transaction);
                    await _unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
        }

    }
        [HttpPatch("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id, [FromBody] TransactionEditModel NewTransaction)
        {
            if (id == null || id <= 0)
            {
                return BadRequest();
            }
            var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

            int USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");
            int ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            var transaction_buscada = _unitOfWork.Transactions.FindTransaction((int)id, USD_account_id, ARS_account_id);

            if (ModelState.IsValid && transaction_buscada != null)
            {
                try
                {
                    transaction_buscada.Concept = NewTransaction.Concept;
                    _unitOfWork.Transactions.Update(transaction_buscada);
                    await _unitOfWork.Complete();
                    return Ok();
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
            else { return BadRequest(); }
        }

        [HttpGet("Details/{id}")]
        public IActionResult Details(int? id)
        {
            if (id == null || id <= 0) { return BadRequest(); }

            var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);

            int? ARS_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
            int? USD_account_id = _unitOfWork.Accounts.GetAccountId(user_id, "USD");

            if (ARS_account_id != null && USD_account_id != null)
            {
                var transaction = _unitOfWork.Transactions
                    .FindTransaction((int)id, (int)USD_account_id, (int)ARS_account_id);

                if (transaction != null) { return Ok(transaction); }
                else { return BadRequest("No se encontró la transacción"); }

            }
            else { return BadRequest("No se encontraron las cuentas del usuario"); }
        }

        [HttpPost("Filter")]
        public IActionResult Filter([FromBody] TransactionSearchModel transaction)
        {
            if (transaction != null)
            {
                try
                {
                    if (transaction.AccountId == null)
                    {
                        var user_id = int.Parse(User.Claims.First(i => i.Type == "UserId").Value);
                        transaction.AccountId = _unitOfWork.Accounts.GetAccountId(user_id, "ARS");
                    }
                    Transactions transactionDB = _mapper.Map<Transactions>(transaction);
                    transactionDB = _unitOfWork.Transactions.FilterTransaction(transactionDB);

                    if (transaction != null) { return Ok(transactionDB); }
                    else { return BadRequest("No se encontró la transacción"); }
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
            else { return BadRequest(); }
        }
    }

}
